/****************************************************************************
While the underlying libraries are covered by LGPL, this sample is released 
as public domain.  It is distributed in the hope that it will be useful, but 
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
or FITNESS FOR A PARTICULAR PURPOSE.  
*****************************************************************************/

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

using DirectShowLib;

namespace DxScanScenes
{

    public delegate void DetectadoCambioEscena(object sender, CambioEscenaEventArgs e); 
    public delegate void DetectadoCambioFrame(object sender, CambioEscenaEventArgs e); 

    public class CambioEscenaEventArgs : EventArgs
    {
        public readonly double TiempoSample;

        public CambioEscenaEventArgs(double tiempo)
        {
            TiempoSample = tiempo;
        }
    }

    /// <summary> Summary description for MainForm. </summary>
    internal class Capture : ISampleGrabberCB, IDisposable
    {
        #region Member variables

        /// <summary> graph builder interface. </summary>
        private IFilterGraph2 m_graphBuilder = null;
        IMediaControl m_mediaCtrl = null;
        IMediaEvent m_MediaEvent = null;

        /// <summary> Dimensions of the image, calculated once in constructor. </summary>
        public int m_videoWidth;
        public int m_videoHeight;
        private int m_bitsPerPixel;
        public int m_Count = 0;
        public int m_Scenes = 0;
        public SceneDetector m_Detector = null;
        public volatile bool StopCondition = false;
        public ArrayList DetectedKeyFrames = new ArrayList();
        public event DetectadoCambioEscena EnCambioEscena;
        public event DetectadoCambioFrame EnNuevoFrame;

#if DEBUG
        // Allow you to "Connect to remote graph" from GraphEdit
        DsROTEntry m_rot = null;
#endif

        #endregion

        #region API

        [DllImport("Kernel32.dll", EntryPoint="RtlMoveMemory")]
        private static extern void CopyMemory(IntPtr Destination, IntPtr Source, [MarshalAs(UnmanagedType.U4)] uint Length);

        #endregion

        /// <summary> File name to scan</summary>
        public Capture(string FileName)
        {
            try
            {
                // Set up the capture graph
                SetupGraph(FileName);
            }
            catch
            {
                Dispose();
                throw;
            }
        }
        /// <summary> release everything. </summary>
        public void Dispose()
        {
            CloseInterfaces();
        }
        // Destructor
        ~Capture()
        {
            CloseInterfaces();
        }


        /// <summary> capture the next image </summary>
        public void Start()
        {
            int hr = m_mediaCtrl.Run();
            DsError.ThrowExceptionForHR( hr );
        }


        public void WaitUntilDone()
        {
            int hr = 0;
            EventCode evCode;
            const int E_Abort = unchecked((int)0x80004004);

            do
            {
                //System.Windows.Forms.Application.DoEvents();
                if (StopCondition) break; // AÑADIDO
                hr = this.m_MediaEvent.WaitForCompletion( 100, out evCode );
            } while (hr == E_Abort);
            //DsError.ThrowExceptionForHR(hr);
        }

        /// <summary> build the capture graph for grabber. </summary>
        private void SetupGraph(string FileName)
        {
            int hr;

            ISampleGrabber sampGrabber = null;
            IBaseFilter	baseGrabFlt = null;
            IBaseFilter capFilter = null;
            IBaseFilter nullrenderer = null;

            // Get the graphbuilder object
            m_graphBuilder = new FilterGraph() as IFilterGraph2;
            m_mediaCtrl = m_graphBuilder as IMediaControl;
            m_MediaEvent = m_graphBuilder as IMediaEvent;

            IMediaFilter mediaFilt = m_graphBuilder as IMediaFilter;

            try
            {
#if DEBUG
                m_rot = new DsROTEntry( m_graphBuilder );
#endif

                // Add the video source
                hr = m_graphBuilder.AddSourceFilter(FileName, "Ds.NET FileFilter", out capFilter);
                DsError.ThrowExceptionForHR( hr );

                // Get the SampleGrabber interface
                sampGrabber = new SampleGrabber() as ISampleGrabber;
                baseGrabFlt = sampGrabber as IBaseFilter;

                ConfigureSampleGrabber(sampGrabber);

                // Add the frame grabber to the graph
                hr = m_graphBuilder.AddFilter( baseGrabFlt, "Ds.NET Grabber" );
                DsError.ThrowExceptionForHR( hr );

                // ---------------------------------
                // Connect the file filter to the sample grabber

                // Hopefully this will be the video pin, we could check by reading it's mediatype
                IPin iPinOut = DsFindPin.ByDirection(capFilter, PinDirection.Output, 0);

                // Get the input pin from the sample grabber
                IPin iPinIn = DsFindPin.ByDirection(baseGrabFlt, PinDirection.Input, 0);

                hr = m_graphBuilder.Connect(iPinOut, iPinIn);
                DsError.ThrowExceptionForHR( hr );

                // Add the null renderer to the graph
                nullrenderer = new NullRenderer() as IBaseFilter;
                hr = m_graphBuilder.AddFilter( nullrenderer, "Null renderer" );
                DsError.ThrowExceptionForHR( hr );

                // ---------------------------------
                // Connect the sample grabber to the null renderer

                iPinOut = DsFindPin.ByDirection(baseGrabFlt, PinDirection.Output, 0);
                iPinIn = DsFindPin.ByDirection(nullrenderer, PinDirection.Input, 0);
                
                hr = m_graphBuilder.Connect(iPinOut, iPinIn);
                DsError.ThrowExceptionForHR( hr );

                // Turn off the clock.  This causes the frames to be sent
                // thru the graph as fast as possible
                hr = mediaFilt.SetSyncSource(null);
                DsError.ThrowExceptionForHR( hr );

                // Read and cache the image sizes
                SaveSizeInfo(sampGrabber);


                this.m_Detector = new SceneDetector(m_videoWidth, m_videoHeight, m_bitsPerPixel);
                this.m_Detector.NewScene += new EventHandler<MediaSampleEventArgs>(m_detector_NewScene);
                this.m_Detector.NewFrame += new EventHandler<MediaSampleEventArgs>(m_detector_NewFrame);
            }
            finally
            {
                if (capFilter != null)
                {
                    Marshal.ReleaseComObject(capFilter);
                    capFilter = null;
                }
                if (sampGrabber != null)
                {
                    Marshal.ReleaseComObject(sampGrabber);
                    sampGrabber = null;
                }
                if (nullrenderer != null)
                {
                    Marshal.ReleaseComObject(nullrenderer);
                    nullrenderer = null;
                }
            }
        }

        /// <summary> Read and store the properties </summary>
        private void SaveSizeInfo(ISampleGrabber sampGrabber)
        {
            int hr;

            // Get the media type from the SampleGrabber
            AMMediaType media = new AMMediaType();
            hr = sampGrabber.GetConnectedMediaType( media );
            DsError.ThrowExceptionForHR( hr );

            if( (media.formatType != FormatType.VideoInfo) || (media.formatPtr == IntPtr.Zero) )
            {
                throw new NotSupportedException( "Unknown Grabber Media Format" );
            }

            // Grab the size info
            VideoInfoHeader videoInfoHeader = (VideoInfoHeader) Marshal.PtrToStructure( media.formatPtr, typeof(VideoInfoHeader) );
            m_videoWidth = videoInfoHeader.BmiHeader.Width;
            m_videoHeight = videoInfoHeader.BmiHeader.Height;
            m_bitsPerPixel = videoInfoHeader.BmiHeader.BitCount;

            DsUtils.FreeAMMediaType(media);
            media = null;
        }

        /// <summary> Set the options on the sample grabber </summary>
        private void ConfigureSampleGrabber(ISampleGrabber sampGrabber)
        {
            AMMediaType media;
            int hr;

            // Set the media type to Video/RBG24
            media = new AMMediaType();
            media.majorType	= MediaType.Video;
            media.subType	= MediaSubType.RGB24;
            media.formatType = FormatType.VideoInfo;
            hr = sampGrabber.SetMediaType( media );
            DsError.ThrowExceptionForHR( hr );

            DsUtils.FreeAMMediaType(media);
            media = null;

            // Choose to call BufferCB instead of SampleCB
            hr = sampGrabber.SetCallback( this, 1 );
            DsError.ThrowExceptionForHR( hr );
        }

        /// <summary> Shut down capture </summary>
        private void CloseInterfaces()
        {
            int hr;

            try
            {
                if( m_mediaCtrl != null )
                {
                    // Stop the graph
                    hr = m_mediaCtrl.Stop();
                    m_mediaCtrl = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

#if DEBUG
            if (m_rot != null)
            {
                m_rot.Dispose();
            }
#endif

            if (m_graphBuilder != null)
            {
                Marshal.ReleaseComObject(m_graphBuilder);
                m_graphBuilder = null;
            }
            GC.Collect();
        }

        /// <summary> sample callback, NOT USED. </summary>
        int ISampleGrabberCB.SampleCB( double SampleTime, IMediaSample pSample )
        {
            Marshal.ReleaseComObject(pSample);
            return 0;
        }

        /// <summary> buffer callback, COULD BE FROM FOREIGN THREAD. </summary>
        unsafe int ISampleGrabberCB.BufferCB( double SampleTime, IntPtr pBuffer, int BufferLen )
        {
            ((ISampleGrabberCB) this.m_Detector).BufferCB(SampleTime, pBuffer, BufferLen);

            // Increment frame number.  Done this way, frame are zero indexed.
            m_Count++;
            return 0;
        }

        void m_detector_NewScene(object sender, MediaSampleEventArgs e)
        {
            DetectedKeyFrames.Add(e.SampleTime);
            if (EnCambioEscena != null)
                EnCambioEscena(this, new CambioEscenaEventArgs(e.SampleTime));
            this.m_Scenes++;            
        }

        void m_detector_NewFrame(object sender, MediaSampleEventArgs e)
        {
            if (EnNuevoFrame != null)
                EnNuevoFrame(this, new CambioEscenaEventArgs(e.SampleTime));
        }

    }
}
