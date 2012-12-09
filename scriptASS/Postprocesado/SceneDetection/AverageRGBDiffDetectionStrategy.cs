using System;
using System.Diagnostics;

namespace DxScanScenes
{
   /// <summary>
   /// Scene detection strategy which compares the average difference between 
   /// RGB bytes values in each successive frame. 
   /// </summary>
   public class AverageRGBDiffDetectionStrategy : ISceneDetectionStrategy
   {
      /// <summary>
      /// Event fired to indicate that sample-related data has been generated. 
      /// This event may be fired more than once per media sample and is 
      /// primarily to be used for logging and debugging of detection strategies.
      /// </summary>
      public event EventHandler<SampleDataEventArgs> DataGenerated;

      // RGB difference threshold which indicates a scene change when at baseline RGB level 
      private const double BaselineRgbDiffThreshold = 21;
      private const double BaselineRgbLevel = 90;
      // min and max thresholds based on the ranges found in real data
      private const double MinRgbDiffThreshold = 5;
      private const double MaxRgbDiffThreshold = 45;
      // this much above or below threshold means we're in the uncertain range
      private const double BaselineUncertainty = 5;
      // amount to change diff threshold in relation to changes in RGB level (21/90, 21.395/91, etc.)
      private const double ThresholdToLevelRatio = .395;

      /// <summary>
      /// Number of random samples to take from each media frame analyzed.
      /// </summary>
      public const int DefaultSampleSize = 2000;

      // positions in the frame data array at which to sample
      private int[] sampleLocations;

      // buffer of samples taken on last call to SceneChanged()
      private byte[] prevSamples;

      private int prevAvgDiffChange;
      private int prevAvgDiff;
      private double prevSampleTime;
      private int prevAvgRgbLevel;

      /// <summary>
      /// Initializes a new instance of the <see cref="AverageRGBDiffDetectionStrategy"/> class.
      /// </summary>
      /// <param name="videoWidth">Width of the video.</param>
      /// <param name="videoHeight">Height of the video.</param>
      /// <param name="bitsPerPixel">The bits per pixel.</param>
      public AverageRGBDiffDetectionStrategy(int videoWidth, int videoHeight, int bitsPerPixel)
      {
         Random random = new Random();
         // data is received as an array of bytes with each n-bytes representing a pixel, where n = bitsPerPixel / 8
         int dataSize = videoWidth * videoHeight * (bitsPerPixel / 8);
         int sampleCount = Math.Min(DefaultSampleSize, dataSize);

         sampleLocations = new int[sampleCount];
         prevSamples = new byte[sampleCount];

         for (int k = 0; k < sampleLocations.Length; k++)
         {
            sampleLocations[k] = random.Next(dataSize);
         }
      }

      /// <summary>
      /// Checks for a scene change between the previous frame
      /// and the current frame.
      /// </summary>
      /// <param name="sampleTime">The sample time in seconds.</param>
      /// <param name="pBuffer">The p buffer.</param>
      /// <param name="bufferLength">Length of the buffer.</param>
      /// <param name="sceneChangeTime">The scene change time, when a
      /// scene change is detected.</param>
      /// <returns>
      /// true if a scene change was detected; otherwise returns false
      /// </returns>
      public unsafe bool SceneChanged(double sampleTime, IntPtr pBuffer, int bufferLength,
                                      ref double sceneChangeTime)
      {
         bool sceneChanged = false;

         Byte* b = (Byte*) pBuffer;
         int lastLocation = 0;
         int sumDiffs = 0;
         int sumRgbLevels = 0;

         for (int k = 0; k < sampleLocations.Length; k++)
         {
            b += sampleLocations[k] - lastLocation;
            // using xor rather than simple difference amplifies all differences, making scene changes more distinct
            sumDiffs += (*b ^ prevSamples[k]);
            sumRgbLevels += *b;

            lastLocation = sampleLocations[k];
            prevSamples[k] = *b;
         }

         int avgRgbLevel = sumRgbLevels / sampleLocations.Length;
         int avgDiff = sumDiffs / sampleLocations.Length;
         int avgDiffChange = avgDiff - prevAvgDiff;

         OnDataGenerated(
            new SampleDataEventArgs(sampleTime, new object[] {avgDiff, avgDiffChange, avgRgbLevel},
                                    new string[] {"AvgRgbDiff", "AvgRgbDiffChange", "AvgRgbLevel"}));

         if (WasPrevNewScene(avgDiffChange, avgRgbLevel))
         {
             Debug.WriteLine("New Scene Time: \t" + sampleTime, GetType().Name);
             Debug.WriteLine("Current/Prev Diff: \t" + avgDiff + "/" + prevAvgDiff, GetType().Name);

             sceneChanged = true;
             sceneChangeTime = prevSampleTime;
         }
         else // aunque no sea cambio la sacamos
             sceneChangeTime = prevSampleTime;

         prevAvgRgbLevel = avgRgbLevel;
         prevAvgDiffChange = avgDiffChange;
         prevAvgDiff = avgDiff;
         prevSampleTime = sampleTime;

         return sceneChanged;
      }

      private bool WasPrevNewScene(int avgDiffChange, int avgRgbLevel)
      {
         double prevRgbLevelVariance = prevAvgRgbLevel - BaselineRgbLevel;
         double prevDiffThreshold = (prevRgbLevelVariance * ThresholdToLevelRatio) +
                                    BaselineRgbDiffThreshold;
         prevDiffThreshold = (int)Math.Min(MaxRgbDiffThreshold, prevDiffThreshold);
         prevDiffThreshold = (int)Math.Max(MinRgbDiffThreshold, prevDiffThreshold);

         if (prevAvgDiffChange > prevDiffThreshold && avgDiffChange < (-prevAvgDiffChange * .5))
         {
            return true;
         }

         return false;
      }

      /// <summary>
      /// Raises the <see cref="DataGenerated"/> event.
      /// </summary>
      /// <param name="e">The <see cref="FilmRoom.Capture.SampleDataEventArgs"/> instance containing the event data.</param>
      [Conditional("DEBUG")]
      protected void OnDataGenerated(SampleDataEventArgs e)
      {
         if (DataGenerated != null)
         {
            DataGenerated.BeginInvoke(this, e, ProcessedDataGenerated, null);
         }
      }

      /// <summary>
      /// Handler to call EndInvoke() for the asynchronous invocations of DataGenerated event.
      /// </summary>
      /// <param name="result"></param>
      private void ProcessedDataGenerated(IAsyncResult result)
      {
         DataGenerated.EndInvoke(result);
      }
   }
}