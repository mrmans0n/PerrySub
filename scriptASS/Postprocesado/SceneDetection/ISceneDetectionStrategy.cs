using System;

namespace DxScanScenes
{
    public interface ISceneDetectionStrategy
    {
        /// <summary>
        /// Event fired to indicate that sample-related data has been generated. 
        /// This event may be fired more than once per media sample and is 
        /// primarily to be used for logging and debugging of detection strategies.
        /// </summary>
        event EventHandler<SampleDataEventArgs> DataGenerated;

        /// <summary>
        /// Analyzes a media sample to detect a scene change.
        /// </summary>
        /// <param name="sampleTime">The sample time in seconds.</param>
        /// <param name="pBuffer">The p buffer.</param>
        /// <param name="bufferLength">Length of the buffer.</param>
        /// <param name="sceneChangeTime">The scene change time, when a 
        /// scene change is detected.</param>
        /// <returns>true if a scene change was detected; otherwise returns false</returns>
        bool SceneChanged(double sampleTime, IntPtr pBuffer, int bufferLength,
                          ref double sceneChangeTime);
    }
}