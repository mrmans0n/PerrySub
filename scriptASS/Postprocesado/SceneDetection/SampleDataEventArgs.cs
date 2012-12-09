namespace DxScanScenes
{
    /// <summary>
    /// Holds information about processing of a media frame.
    /// </summary>
    /// <remarks>
    /// This is a very generic holder for a piece of sample/frame related 
    /// data and a label that identifies it.
    /// </remarks>
    public class SampleDataEventArgs : MediaSampleEventArgs
    {
        private string[] dataLabels;
        private object[] dataValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleDataEventArgs"/> class.
        /// </summary>
        /// <param name="sampleTime">The sample time in seconds.</param>
        /// <param name="dataValues">The data values.</param>
        /// <param name="dataLabels">The data labels.</param>
        public SampleDataEventArgs(double sampleTime, object[] dataValues, string[] dataLabels)
            : base(sampleTime)
        {
            this.dataValues = dataValues;
            this.dataLabels = dataLabels;
        }

        /// <summary>
        /// Gets the processed data.
        /// </summary>
        /// <value>The processed data.</value>
        public object[] DataValues
        {
            get
            {
                return dataValues;
            }
        }

        /// <summary>
        /// Gets the data label.
        /// </summary>
        /// <value>The data label.</value>
        public string[] DataLabels
        {
            get
            {
                return dataLabels;
            }
        }
    }
}