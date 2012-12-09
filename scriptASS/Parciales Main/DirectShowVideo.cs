using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using DirectShowLib;
using System.Collections;
using System.Runtime.InteropServices;
using System.Drawing;

namespace scriptASS
{
    partial class mainW 
    {

        Hashtable frameTime;

        #region Variables DShow
        private IGraphBuilder graphBuilder;
        public IMediaControl mediaControl;
        //private IMediaEventEx mediaEventEx;
        private IVideoWindow videoWindow;
        //private IBasicAudio basicAudio;
        private IBasicVideo basicVideo;
        public IMediaSeeking mediaSeeking;
        private IMediaPosition mediaPosition;
        //private IVideoFrameStep frameStep;
        #endregion

        #region SOPORTE DSHOW

        public void openVid(string fileName, int frame)
        {

            openVid(fileName);
            FrameIndex = frame;

            videoPanel.Visible = true;
            isVideoLoaded = true;
            updateMenuEnables();
        }

        public void openVid(string fileName)
        {

            if (!File.Exists(fileName))
            {
                errorMsg("El archivo '" + fileName + "' no existe.");
                videoPanel.Visible = false;
                isVideoLoaded = false;
                drawPositions();
                return;
            }

            if (VideoBoxType == PreviewType.AviSynth)
            {
                avsClip = null;
                //butPause.Enabled = true;
                //butPlayR.Enabled = true;
                //butPlay.Enabled = true;
                //butStop.Enabled = true;
                videoPictureBox.Visible = false;
                //closeVidDShow(); // añadido
            }

            if (mediaControl != null)
            {
                mediaControl.Stop();
                videoWindow.put_Visible(DirectShowLib.OABool.False);
                videoWindow.put_Owner(IntPtr.Zero);
            }

            // Dshow :~~

            graphBuilder = (IGraphBuilder)new FilterGraph();
            graphBuilder.RenderFile(fileName, null);
            mediaControl = (IMediaControl)graphBuilder;
            // mediaEventEx = (IMediaEventEx)this.graphBuilder;
            mediaSeeking = (IMediaSeeking)graphBuilder;
            mediaPosition = (IMediaPosition)graphBuilder;
            basicVideo = graphBuilder as IBasicVideo;
            videoWindow = graphBuilder as IVideoWindow;

            VideoBoxType = PreviewType.DirectShow;

            // sacando información
            
            int x, y; double atpf;
            basicVideo.GetVideoSize(out x, out y);
            if (x == 0 || y == 0)
            {
                errorMsg("No se puede abrir un vídeo sin dimensiones.");
                videoPanel.Visible = false;
                isVideoLoaded = false;
                drawPositions();
                return;
            }

            if (videoInfo == null) videoInfo = new VideoInfo(fileName);

            videoInfo.Resolution = new Size(x, y);

            basicVideo.get_AvgTimePerFrame(out atpf);
            videoInfo.FrameRate = Math.Round(1 / atpf, 3);

            //labelResFPS.Text = x.ToString() + "x" + y.ToString() + " @ " + videoInfo.FrameRate.ToString() + " fps";
            textResX.Text = x.ToString();
            textResY.Text = y.ToString();
            textFPS.Text = videoInfo.FrameRate.ToString();

            if (File.Exists(Application.StartupPath+"\\MediaInfo.dll") && File.Exists(Application.StartupPath+"\\MediaInfoWrapper.dll"))
            {
                treeView1.Enabled = true;
                try
                {
                    RetrieveMediaFileInfo(fileName);
                }
                catch { treeView1.Enabled = false; }
            } 
            else treeView1.Enabled = false;

            if (x != 0)
            {
                vidScaleFactor.Enabled = true;

                try
                {
                    vidScaleFactor.Text = getFromConfigFile("mainW_Zoom");
                }
                catch
                {
                    vidScaleFactor.Text = "50%";
                };

                double p = double.Parse(vidScaleFactor.Text.Substring(0, vidScaleFactor.Text.IndexOf('%')));
                p = p / 100;

                int new_x = (int)(x * p);
                int new_y = (int)(y * p);

                videoWindow.put_Height(new_x);
                videoWindow.put_Width(new_y);
                videoWindow.put_Owner(videoPanel.Handle);
                videoPanel.Size = new System.Drawing.Size(new_x, new_y);
                videoWindow.SetWindowPosition(0, 0, videoPanel.Width, videoPanel.Height);
                videoWindow.put_WindowStyle(WindowStyle.Child);
                videoWindow.put_Visible(DirectShowLib.OABool.True);

            }
            else vidScaleFactor.Enabled = false;

            // timer
            actualizaFrames.Interval = 10;
            actualizaFrames.Enabled = true;

            //mediaControl.Run();            
            drawPositions();

            framesFin.Enabled = true;
            buttonAddFrameInicio.Enabled = buttonAddFrameInicio.Visible = true;
            framesInicio.Enabled = true;
            buttonAddFrameFin.Enabled = buttonAddFrameFin.Visible = true;
            butClip.Enabled = false;

            mediaSeeking.SetTimeFormat(DirectShowLib.TimeFormat.Frame);

            videoInfo.FrameTotal = VideoUnitConversion.getTotal(mediaSeeking, videoInfo.FrameRate);          
            seekBar.Maximum = FrameTotal;
            seekBar.TickFrequency = seekBar.Maximum / 10;

            // VFW ( __ SOLO AVIs __ )

            try
            {
                AVIFileWrapper.AVIFileInit();
                int aviFile = 0;
                IntPtr aviStream;
                int res = AVIFileWrapper.AVIFileOpen(ref aviFile, fileName, 0x20, 0);
                res = AVIFileWrapper.AVIFileGetStream(aviFile, out aviStream, 1935960438, 0);

                videoInfo.KeyFrames = new ArrayList();
                int nFrames = FrameTotal;


                for (int i = 0; i < nFrames; i++)
                {
                    if (isKeyFrame(aviStream.ToInt32(), i))
                        videoInfo.KeyFrames.Add(i);
                }

                setStatus(videoInfo.KeyFrames.Count + " detectados");

                AVIFileWrapper.AVIStreamRelease(aviStream);
                AVIFileWrapper.AVIFileRelease(aviFile);
                AVIFileWrapper.AVIFileExit();
                KeyframesAvailable = true;
                /*
                nextK.Enabled = true;
                prevK.Enabled = true;

                drawKeyFrameBox();
                 */ 

            }
            catch
            {
                /*
                nextK.Enabled = false;
                prevK.Enabled = false;
                keyFrameBox.Visible = false;
                 */ 
                KeyframesAvailable = false;
            }

            if (openFile != null && al.Count > 1 && gridASS.RowCount>0)
            {
                gridASS.Rows[1].Selected = true;
                gridASS.Rows[0].Selected = true;
                gridASS.Rows[1].Selected = false;

            }

            frameTime = new Hashtable();
            for (int i = 0; i < FrameTotal; i++)
                frameTime.Add(i, new Tiempo((double)((double)i / videoInfo.FrameRate)));

            isVideoLoaded = true;

            updateMenuEnables();
            mediaControl.Pause();

            setStatus("Vídeo " + fileName + " cargado. [DirectShow]");

            script.GetHeader().SetHeaderValue("Video File", fileName);

            if (isKeyframeGuessNeeded())
                KeyframeGuess(false);
            //FrameIndex = 0;
        }

        private bool isKeyFrame(int aviStr, int nFrame)
        {
            int f = AVIFileWrapper.AVIStreamFindSample(aviStr, nFrame, 0x01 | 0x10);
            return (f == nFrame);
        }


        private void closeVidDShow()
        {
            if (mediaControl != null)
                mediaControl.Stop();

            if (videoWindow != null)
            {
                videoWindow.put_Visible(DirectShowLib.OABool.False);
                videoWindow.put_Owner(IntPtr.Zero);
            }
            actualizaFrames.Enabled = false;
            PlayInRange.Enabled = false;

            //graphBuilder.Abort();                        
            mediaControl = null;
            //mediaEventEx = null;
            videoWindow = null;
            //basicAudio = null;
            basicVideo = null;
            mediaSeeking = null;
            mediaPosition = null;
            //frameStep = null;

            Marshal.ReleaseComObject(graphBuilder);
            graphBuilder = null;
            GC.Collect();
            isVideoLoaded = true;
            updateMenuEnables();

        }

        private void closeVid()
        {
            isVideoLoaded = false;
            if (VideoBoxType == PreviewType.DirectShow) closeVidDShow();

            videoPanel.Visible = false;
            drawPositions();
            

            framesFin.Enabled = false;
            buttonAddFrameInicio.Enabled = buttonAddFrameInicio.Visible = false;
            framesInicio.Enabled = false;
            buttonAddFrameFin.Enabled = buttonAddFrameFin.Visible = false;
            butClip.Enabled = false;

            updateMenuEnables();
            videoInfo = null;
            //avsClip = null;

        }


        #endregion

    }
}
