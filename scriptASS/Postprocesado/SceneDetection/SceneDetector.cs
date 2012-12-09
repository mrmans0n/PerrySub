using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using DirectShowLib;

namespace DxScanScenes
{
    /// <summary>
    /// 
    /// </summary>
    public class SceneDetector : ISampleGrabberCB
    {
        private ISceneDetectionStrategy detectionStrategy;

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneDetector"/> class.
        /// </summary>
        /// <param name="videoWidth">Width of the video.</param>
        /// <param name="videoHeight">Height of the video.</param>
        /// <param name="bitsPerPixel">The bits per pixel.</param>
        public SceneDetector(int videoWidth, int videoHeight, int bitsPerPixel)
        {
            Debug.WriteLine("videoWidth: " + videoWidth, GetType().Name);
            Debug.WriteLine("videoHeight: " + videoHeight, GetType().Name);
            Debug.WriteLine("bitsPerPixel: " + bitsPerPixel, GetType().Name);

            detectionStrategy =
                new AverageRGBDiffDetectionStrategy(videoWidth, videoHeight, bitsPerPixel);
        }

        /// <summary>
        /// Gets or sets the detection strategy.
        /// </summary>
        /// <value>The detection strategy.</value>
        public ISceneDetectionStrategy DetectionStrategy
        {
            get
            {
                return detectionStrategy;
            }
            set
            {
                detectionStrategy = value;
            }
        }

        #region ISampleGrabberCB Members

        /// <summary>
        /// Implementation of ISampleGrabberCB.
        /// </summary>
        int ISampleGrabberCB.SampleCB(double sampleTime, IMediaSample pSample)
        {
            IntPtr pBuffer;
            int hr = pSample.GetPointer(out pBuffer);
            DsError.ThrowExceptionForHR(hr);

            Analyze(sampleTime, pBuffer, pSample.GetSize());

            Marshal.ReleaseComObject(pSample);
            return 0;
        }

        /// <summary>
        /// Implementation of ISampleGrabberCB.
        /// </summary>
        int ISampleGrabberCB.BufferCB(double sampleTime, IntPtr pBuffer, int bufferLen)
        {
            Analyze(sampleTime, pBuffer, bufferLen);
            return 0;
        }

        #endregion

        /// <summary>
        /// Event fired when a new scene is detected.
        /// </summary>
        public event EventHandler<MediaSampleEventArgs> NewScene;
        public event EventHandler<MediaSampleEventArgs> NewFrame;

        /// <summary>
        /// Analyzes media samples looking for scene changes.
        /// </summary>
        /// <param name="sampleTime">The sample time, in seconds.</param>
        /// <param name="pBuffer">The pointer to the media sample data.</param>
        /// <param name="bufferLength">The buffer length.</param>
        /// <remarks>
        /// This method accepts media sample data provided via either of the <see cref="DirectShowLib.ISampleGrabberCB"/> callback methods.
        /// </remarks>
        private void Analyze(double sampleTime, IntPtr pBuffer, int bufferLength)
        {
            double sceneChangeTime = 0;
            bool sceneChanged =
                detectionStrategy.SceneChanged(sampleTime, pBuffer, bufferLength,
                                               ref sceneChangeTime);

            

            if (sceneChanged)
            {
                OnNewScene(sceneChangeTime);
            }
            OnNewFrame(sceneChangeTime);

        }

        /// <summary>
        /// Called when a scene change is detected.
        /// </summary>
        /// <param name="sampleTime">The sample time, in seconds.</param>
        protected void OnNewScene(double sampleTime)
        {
            if (NewScene != null)
            {
                NewScene.BeginInvoke(this, new MediaSampleEventArgs(sampleTime),
                                     ProcessedSceneChange, null);
            }
        }

        protected void OnNewFrame(double sampleTime)
        {
            if (NewFrame != null)
            {
                NewFrame.BeginInvoke(this, new MediaSampleEventArgs(sampleTime),
                                     ProcessedFrameChange, null);
            }
        }

        /// <summary>
        /// Handler to call EndInvoke() for the asynchronous invocations of NewScene event.
        /// </summary>
        /// <param name="result"></param>
        private void ProcessedSceneChange(IAsyncResult result)
        {
            NewScene.EndInvoke(result);
        }

        private void ProcessedFrameChange(IAsyncResult result)
        {
            NewFrame.EndInvoke(result);
        }
    }
}