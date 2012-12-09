using System;

namespace DxScanScenes
{
    /// <summary>
    /// Holds scene-change event information.
    /// </summary>
    public class MediaSampleEventArgs : EventArgs
    {
        private double sampleTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaSampleEventArgs"/> class.
        /// </summary>
        /// <param name="sampleTime">The sample time in seconds.</param>
        public MediaSampleEventArgs(double sampleTime)
        {
            this.sampleTime = sampleTime;
        }

        /// <summary>
        /// Gets the sample time of the scene change in seconds.
        /// </summary>
        /// <value>The sample time.</value>
        public double SampleTime
        {
            get
            {
                return sampleTime;
            }
        }
    }
}