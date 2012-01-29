using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.DirectX.DirectSound;

namespace scriptASS
{
    partial class mainW
    {
        private Bitmap AudioGrid = null;
        private Bitmap AudioWave = null;
        private Bitmap AudioWave_All = null;
        public WaveType AudioWaveType = WaveType.Normal;
        private int AudioGrid_pxPerSecond = 100;
        private double audioGrid_Inicio = -1;
        private double audioGrid_Fin = -1;

        private double audioGrid_Inicio_500 = -1;
        private double audioGrid_Fin_500 = -1;

        private double InicioPlay = 0.0;

        private AviSynthClip avsaudio;
        private AVStoDirectSound avs2ds;
        public double AudioWave_Zoom = 2.00;
        public int AudioWave_Mode = 1;
        public static int idx0rz;
        private static bool audioLoadFinished;
        private int AudioBytesMax;
        Thread AudioLoad;
        progress2W AudioLoad_Progress;
        public static bool AudioLoad_isCancelled = false;
        private IntPtr AudioFull = IntPtr.Zero;
        private byte[] AudioTemp = null;
        public SecondaryBuffer audio;
        ReproductionState AudioState = ReproductionState.Stop;

        #region AUDIO TIMING

        public void updateAudioGrid()
        {
            if (!isAudioLoaded) return;
            if (this.WindowState == FormWindowState.Minimized) return;

            DateTime inicio = DateTime.Now;
            if (AudioGrid != null) AudioGrid.Dispose();
            AudioGrid = new Bitmap(panelAudioBox.Width, AudioGridBox.Height, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);

            if (AudioWave != null)
                AudioGrid = BitmapFunctions.FastCrop(AudioWave, AudioGridBox.Width, AudioGridBox.Height, 0);

            AudioWave_All = BitmapFunctions.GenerateAudioGridDisplay(AudioGrid, AudioGrid_pxPerSecond, hSliderBar.Value, false);

            if (gridASS.SelectedRows[0].Index > 0)
            {
                lineaASS anterior = (lineaASS)al[gridASS.SelectedRows[0].Index - 1];
                //AudioWave_All = BitmapFunctions.GenerateAudioGridDisplayMarkers(AudioWave_All, anterior.t_inicial.getTiempo(), anterior.t_final.getTiempo(), (double)hSliderBar.Value / 10, AudioGrid_pxPerSecond, false);
                AudioWave_All = BitmapFunctions.GenerateAudioGridDisplayMarkers(AudioWave_All, anterior.t_inicial.getTiempo(), anterior.t_final.getTiempo(), (double)hSliderBar.Value / 10, AudioGrid_pxPerSecond, false, anterior, textWave.Checked);
            }
            lineaASS actual = (lineaASS)al[gridASS.SelectedRows[0].Index];
            AudioWave_All = BitmapFunctions.GenerateAudioGridDisplayMarkers(AudioWave_All, AudioGrid_Inicio, AudioGrid_Fin, (double)hSliderBar.Value / 10, AudioGrid_pxPerSecond, true, actual,textWave.Checked);

            // fix fft
            AudioWave_All = BitmapFunctions.GenerateAudioGridDisplay(AudioWave_All, AudioGrid_pxPerSecond, hSliderBar.Value, true);
            
            AudioGridBox.Image = AudioWave_All;

            DateTime fin = DateTime.Now;
            TimeSpan duracion = fin - inicio;

            hSliderBar.LargeChange = (int)(((double)panelAudioBox.Width / (double)AudioGrid_pxPerSecond) * 0.75 * 10);
            setStatus("Tiempo de refresco: " + duracion.Milliseconds + "ms.");


        }

        //private void loadAudio(AviSynthClip avs)
        private void loadAudio()
        {

            if (AudioFull != IntPtr.Zero)
            {
                try
                {
                    Marshal.FreeHGlobal(AudioFull);
                }
                catch { }
            }

            // OpenSourceType == VideoSourceType.FFVideoSource

            if (avsaudio.SamplesCount == 0) // si no hay samples cargados tenemos que cargarlos como sea
            {
                throw new PerrySubException("El audio contiene 0 muestras. Cambia el filtro prioritario de audio si tienes problemas.");
            }

            avsaudio.AviSynthInvoke(avsaudio.GetAVS(), 0, "ConvertAudioTo16bit", true, "Nulo");
            avsaudio.AviSynthInvoke(avsaudio.GetAVS(), 0, "ConvertToMono", true, "Nulo");
            //avs.AviSynthInvoke(avs.GetAVS(), 0, "ResampleAudio", true, "22050"); // excepcion en vista, a saber pq

            double rseg = (double)avsaudio.SamplesCount / (double)avsaudio.AudioSampleRate;
            hSliderBar.Maximum = (int)(rseg * 10);

            //avsaudio = avs;
            avs2ds = new AVStoDirectSound(this, avsaudio);

            unmanagedMemoryAudioLoad(avsaudio, avs2ds);

            //updateAudioGrid();
            AudioActualPosition_Refresh.Tick += new EventHandler(AudioActualPosition_Refresh_Tick);
            AudioActualPosition_Refresh_500.Tick += new EventHandler(AudioActualPosition_Refresh_500_Tick);

        }

        private void unmanagedMemoryAudioLoad(AviSynthClip avs, AVStoDirectSound avs2ds)
        {

            AudioLoad_Progress = new progress2W(this);
            AudioLoad_Progress.Show();
            AudioLoad_Progress.Focus();

            AudioBytesMax = (int)avs.SamplesCount * avs.ChannelsCount * avs.BytesPerSample;
            AudioLoad_Progress.progressBar.Maximum = AudioBytesMax;

            try
            {
                mainW.AudioLoad_isCancelled = false;
                AudioFull = Marshal.AllocHGlobal(AudioBytesMax);
                T_LoadFullAudio load = new T_LoadFullAudio(avs, avs2ds, AudioFull, this);
                AudioLoad = new Thread(new ThreadStart(load.LoadAudio));
                AudioLoad.Priority = ThreadPriority.Lowest;

                progressBarUpdater.Interval = 2000;
                progressBarUpdater.Tick += new EventHandler(progressBarUpdater_Tick);
                AudioLoad.Start();
                progressBarUpdater.Enabled = true;

            }
            catch (OutOfMemoryException)
            {
                errorMsg("No hay suficiente memoria RAM disponible. Operación cancelada.");
                mainW.AudioLoad_isCancelled = true;
            }

        }

        void progressBarUpdater_Tick(object sender, EventArgs e)
        {
            if (AudioLoadFinished)
            {
                progressBarUpdater.Enabled = false;
                AudioLoad_Progress.Dispose();
                drawPositions();

            }
            else if (mainW.AudioLoad_isCancelled)
            {
                if (mainW.AudioLoad_isCancelled)
                {
                    progressBarUpdater.Enabled = false;
                    AudioLoad.Abort();

                    AudioLoad_Progress.Dispose();
                    CloseAudio();
                    MessageBox.Show("Carga del audio cancelada por el usuario", "PerrySub", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    isAudioLoaded = false;
                }

            }
            else
            {
                if (mainW.idx0rz < AudioLoad_Progress.progressBar.Maximum)
                    AudioLoad_Progress.progressBar.Value = mainW.idx0rz;
                else AudioLoadFinished = true;
            }

        }


        private void activarGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            script.GetHeader().SetHeaderValue("Audio File", "?video");
            LoadAudioFromVideo();
        }

        private void LoadAudioFromVideo()
        {
            try
            {
                isAudioLoaded = true;
                // panelAudioBox.Visible = true;
                loadAudio();
                // drawPositions();
                updateMenuEnables();
            }
            catch (Exception x)
            {
                errorMsg("Ha habido un error intentando cargar el audio del vídeo.\nMensaje: " + x.Message);
                isAudioLoaded = false;
                panelAudioBox.Visible = false;
                this.Enabled = true;
                if (AudioLoad_Progress!=null) AudioLoad_Progress.Dispose();
            }
        }

        private AviSynthClip LoadAudioWithDependencies(AviSynthClip avsc, string filename)
        {
            OpenAudioSourceType = GetAudioSourceFilter(filename);
            AviSynthScriptEnvironment env = new AviSynthScriptEnvironment();
            avsc = env.ParseScript("BlankClip");
            switch (OpenAudioSourceType)
            {
                case AudioSourceType.DirectShowSource:
                    avsc.AviSynthInvoke(avsc.GetAVS(), 0, "LoadPlugin", false, System.IO.Path.Combine(Application.StartupPath, "DirectShowSource.dll"));
                    break;
                case AudioSourceType.FFAudioSource:
                    avsc.AviSynthInvoke(avsc.GetAVS(), 0, "LoadPlugin", false, System.IO.Path.Combine(Application.StartupPath, "ffms2.dll"));
                    break;
                default:
                    break;
            }
            avsc.AviSynthInvoke(avsc.GetAVS(), 0, OpenAudioSourceType.ToString(), false, filename);
            avsc.AviSynthInvoke(avsc.GetAVS(), 0, "killvideo", false, "Nulo");
            return avsc;
        }

        private void LoadAudioFromFile(string fileName)
        {
            try
            {               
                setStatus("Creando el entorno AviSynth para el audio. Esta operación puede tardar unos segundos dependiendo del filtro de mayor prioridad definido en las opciones.");
                avsaudio = LoadAudioWithDependencies(avsaudio, fileName);

                setStatus("Audio cargado con " + OpenAudioSourceType.ToString());

                isAudioLoaded = true;
                //panelAudioBox.Visible = true;
                loadAudio();
                updateMenuEnables();
                putRecentAUD();
                updateConcatenateConfigFile("LastAUD", fileName);
                script.GetHeader().SetHeaderValue("Audio File", fileName);
            }
            catch (Exception x)
            {
                errorMsg("Ha habido un error intentando cargar el audio '" + fileName + "'\nMensaje: " + x.Message);
                isAudioLoaded = false;
                panelAudioBox.Visible = false;
                this.Enabled = true;
            }
            //drawPositions();

        }

        private void abrirAudioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();            
            ofd.Filter = "Archivos de sonido (" + knownAudioExtensions + ")|" + knownAudioExtensions;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                LoadAudioFromFile(ofd.FileName);
            }
        }

        public void updateAudioWave()
        {
            if (!isAudioLoaded) return;
            if (!AudioLoadFinished) return;
            if (this.WindowState == FormWindowState.Minimized) return;

            AudioWave = new Bitmap(panelAudioBox.Width + AudioGrid_pxPerSecond, AudioGridBox.Height, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);
            double begin = (double)hSliderBar.Value / 10.00;
            double off = begin + ((double)panelAudioBox.Width / (double)AudioGrid_pxPerSecond) + 1;

            unsafe
            {
                byte* buf = (byte*)AudioFull.ToPointer();
                byte* buf_final = buf;
                byte* buf_max = buf + mainW.idx0rz;

                int principio = (int)(begin * avsaudio.AudioSampleRate * avsaudio.BytesPerSample);
                if (principio % 2 == 1) principio++;

                buf += principio;
                buf_final += (int)(off * avsaudio.AudioSampleRate * avsaudio.BytesPerSample);

                if (buf_final > buf_max) buf_final = buf_max;

                int size = (int)(buf_final - buf);
                if (size % 2 == 1) size++;

                byte[] bufferEntero = new byte[size];
                IntPtr buff = new IntPtr(buf);

                Marshal.Copy(buff, bufferEntero, 0, size);
                AudioWave = avs2ds.DrawWave(bufferEntero, AudioGridBox.Width, AudioGridBox.Height, AudioWave_Zoom, (int)AudioWaveType);

            }
        }

        private void hSliderBar_Scroll(object sender, ScrollEventArgs e)
        {
            updateAudioWave();
            updateAudioGrid();
        }

        void hSliderBar_ValueChanged(object sender, EventArgs e)
        {
            if (!isAudioLoaded) return;
            if (AudioWave != null)
            {
                updateAudioWave();
            }
            updateAudioGrid();
        }

        public static bool AudioLoadFinished
        {
            get { return mainW.audioLoadFinished; }
            set { mainW.audioLoadFinished = value; }
        }

        void AudioGridBox_MouseClick(object sender, MouseEventArgs e)
        {
            int ints = hSliderBar.Value / 10;
            double s = (double)ints;
            double r = hSliderBar.Value % 10;
            int pos = e.X;
            double brau = pos + (r * ((double)AudioGrid_pxPerSecond / 10));
            double p = (brau / (double)AudioGrid_pxPerSecond);
            double sp = s + p;

            if (e.Button == MouseButtons.Left)
                AudioGrid_Inicio = sp;
            if (e.Button == MouseButtons.Right)
                AudioGrid_Fin = sp;
            updateAudioGrid();
            AudioGridBox.Focus();
            //selectedLine.Focus();

        }

        void AudioGridBox_MouseMove(object sender, MouseEventArgs e)
        {
            int ints = hSliderBar.Value / 10;
            double s = (double)ints;
            double r = hSliderBar.Value % 10;
            int pos = e.X;
            double brau = (double)pos + (r * (double)((double)AudioGrid_pxPerSecond / 10));
            double p = (brau / (double)AudioGrid_pxPerSecond);
            double sp = s + p;
            timeMouseHover.Text = Tiempo.SecondToTimeString(sp);
        }


        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            AudioGrid_pxPerSecond = ppsTrackBar.Value * 10;
            if (AudioWave != null)
                updateAudioWave();
            updateAudioGrid();
            updateReplaceConfigFile("mainW_AudioGrid_PxPS", AudioGrid_pxPerSecond.ToString());
        }

        private void zoomTrackBar_Scroll(object sender, EventArgs e)
        {
            AudioWave_Zoom = (double)zoomTrackBar.Value / 100;
            updateAudioWave();
            updateAudioGrid();
            updateReplaceConfigFile("mainW_AudioWave_Zoom", estiloV4.d2s(AudioWave_Zoom));
        }

        private void AudioPlayEx(PlayOptions options)
        {
            if (AudioState == ReproductionState.Play)
            {
                AudioActualPosition_Refresh_500.Enabled = false;
                AudioState = ReproductionState.Stop;
                audio.Stop();
                audio.Dispose();
                audio = null;
                return;
            }

            double inicio = Tiempo.TimeToSecondDouble(textInicio.Text);
            double fin = 0;

            if (AudioGrid_Inicio == -1) return;

            if (AudioGrid_Fin != -1)
                fin = Tiempo.TimeToSecondDouble(textFin.Text);                
            else
                if (options == PlayOptions.BeforeEnd || options == PlayOptions.AfterEnd)
                    return;

            switch (options)
            {
                case PlayOptions.BeforeStart:

                    if (inicio < .5)
                    {
                        inicio = 0;
                        fin = .5;
                    }
                    else
                    {
                        inicio -= .5;
                        fin = inicio + .5;
                    }

                    break;
                case PlayOptions.AfterStart:
                    fin = inicio + .5;
                    break;


                case PlayOptions.BeforeEnd:
                    fin = Tiempo.TimeToSecondDouble(textFin.Text);
                    inicio = fin - .5;

                    if (inicio < 0) inicio = 0;
                    break;
                case PlayOptions.AfterEnd:
                    inicio = Tiempo.TimeToSecondDouble(textFin.Text);
                    fin = inicio + .5;

                    break;
                case PlayOptions.Normal:
                    inicio = Tiempo.TimeToSecondDouble(textInicio.Text);
                    fin = Tiempo.TimeToSecondDouble(textFin.Text);
                    break;
            }

            audioGrid_Inicio_500 = inicio;
            audioGrid_Fin_500 = fin;

            InicioPlay = inicio;

            unsafe
            {

                byte* buf = (byte*)AudioFull.ToPointer();
                byte* buf_final = buf;
                buf += (int)(inicio * avsaudio.AudioSampleRate * avsaudio.BytesPerSample);

                IntPtr tmp = new IntPtr(buf);
                int tesuto = tmp.ToInt32();

                if ((tesuto % 2) == 1) buf++;

                //if (tesuto >= mainW.idx0rz) return;

                int max_or_mult = (int)Math.Min((int)(fin * avsaudio.AudioSampleRate * avsaudio.BytesPerSample), mainW.idx0rz);
                buf_final += max_or_mult;

                int size = (int)(buf_final - buf);

                if (size < 0) return;

                AudioTemp = new byte[size];
                IntPtr buff = new IntPtr(buf);

                Marshal.Copy(buff, AudioTemp, 0, size);
                audio = avs2ds.PreparaAudio(AudioTemp);
                audio.Play(0, BufferPlayFlags.Default);
                AudioState = ReproductionState.Play;
                AudioActualPosition_Refresh_500.Enabled = true;
            }
        }

        private void AudioPlay()
        {
            if (AudioState == ReproductionState.Play)
            {
                try
                {
                    AudioActualPosition_Refresh.Enabled = false;
                    AudioState = ReproductionState.Stop;
                    audio.Stop();
                    audio.Dispose();
                    audio = null;
                }
                catch { }
                return;
            }
            else // estabamos parados, podemos seguir ._.
            {

                if (AudioGrid_Inicio != -1)
                {

                    /*
                     * CAMBIAR ESTO PARA COMPROBAR SI TIEMPO = 0
                     */ 
                    if (AudioGrid_Fin <= AudioGrid_Inicio)
                        AudioGrid_Fin = -1;

                    if (AudioTemp != null)
                    {
                        AudioTemp = new byte[0];
                        GC.Collect();
                    }

                    if (audio != null)
                    {
                        try
                        {
                            audio.Stop();
                            audio.Dispose();
                        }
                        catch { }
                    }

                    try
                    {

                        double inicio = Tiempo.TimeToSecondDouble(textInicio.Text);
                        InicioPlay = inicio;
                        double fin = 0;
                        if (AudioGrid_Fin != -1)
                            fin = Tiempo.TimeToSecondDouble(textFin.Text);

                        unsafe
                        {

                            byte* buf = (byte*)AudioFull.ToPointer();
                            byte* buf_final = buf;
                            buf += (int)(inicio * avsaudio.AudioSampleRate * avsaudio.BytesPerSample);

                            IntPtr tmp = new IntPtr(buf);
                            int tesuto = tmp.ToInt32();

                            if ((tesuto % 2) == 1) buf++;

                            if (AudioGrid_Fin != -1)
                            {
                                int max_or_mult = (int)Math.Min((int)(fin * avsaudio.AudioSampleRate * avsaudio.BytesPerSample), mainW.idx0rz);
                                buf_final += max_or_mult;
                            }
                            else
                                buf_final += mainW.idx0rz;

                            int size = (int)(buf_final - buf); // OJO
                            if ((size * (int)AudioMultiplicadorBuffer.Value) < mainW.idx0rz)
                                size = size * (int)AudioMultiplicadorBuffer.Value;
                            
                            if (size < 0) return;
                            
                            AudioTemp = new byte[size];
                            IntPtr buff = new IntPtr(buf);

                            Marshal.Copy(buff, AudioTemp, 0, size);
                            audio = avs2ds.PreparaAudio(AudioTemp);
                            audio.Play(0, BufferPlayFlags.Default);
                            AudioState = ReproductionState.Play;
                            
                            AudioActualPosition_Refresh.Interval = (int)AudioActualPositionInterval.Value * 10;
                            AudioActualPosition_Refresh.Enabled = true;
                        }
                    }
                    catch { }
                }
            }
        }


        void AudioActualPosition_Refresh_500_Tick(object sender, EventArgs e)
        {
            if (AudioState != ReproductionState.Play || audio == null)
            {
                updateAudioGrid();
                AudioState = ReproductionState.Stop;
                AudioActualPosition_Refresh_500.Enabled = false;
                
                return;
            }

            double pos_inicio = InicioPlay; //audioGrid_Inicio_500;
            double pos_relativa = (double)((double)audio.PlayPosition) / ((double)avsaudio.AudioSampleRate * (double)avsaudio.BytesPerSample);
            AudioGridBox.Image = BitmapFunctions.GenerateAudioGridDisplayActual(new Bitmap(AudioWave_All), pos_inicio + pos_relativa, (double)hSliderBar.Value / 10, AudioGrid_pxPerSecond);

            if ((pos_inicio + pos_relativa > audioGrid_Fin_500) || (pos_relativa==0) )
            {
                updateAudioGrid();
                AudioActualPosition_Refresh_500.Enabled = false;
                AudioState = ReproductionState.Stop;
                audio.Stop();
            }


        }


        void AudioActualPosition_Refresh_Tick(object sender, EventArgs e)
        {
            if (AudioState != ReproductionState.Play || audio == null)
            {
                updateAudioGrid();
                AudioActualPosition_Refresh.Enabled = false;
                AudioState = ReproductionState.Stop;
                return;
            }

            //Bitmap t = (Bitmap) ;
            //AudioGridBox.Image = AudioGridBox.Image

            double pos_inicio = InicioPlay; //Tiempo.TimeToSecondDouble(textInicio.Text);
            double pos_relativa = (double)((double)audio.PlayPosition) / ((double)avsaudio.AudioSampleRate * (double)avsaudio.BytesPerSample);
            AudioGridBox.Image = BitmapFunctions.GenerateAudioGridDisplayActual(new Bitmap(AudioWave_All), pos_inicio + pos_relativa, (double)hSliderBar.Value / 10, AudioGrid_pxPerSecond);

            if (AudioGrid_Fin != -1)
            {
                if (pos_inicio + pos_relativa > AudioGrid_Fin)
                {
                    updateAudioGrid();
                    AudioActualPosition_Refresh.Enabled = false;
                    AudioState = ReproductionState.Stop;
                    audio.Stop();
                }

            }

        }

        void AudioGridBox_KeyPress(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Q:
                    AudioPlayEx(PlayOptions.BeforeStart);
                    break;
                case Keys.W:
                    AudioPlayEx(PlayOptions.AfterEnd);
                    break;
                case Keys.E:
                    AudioPlayEx(PlayOptions.AfterStart);
                    break;
                case Keys.D:
                    AudioPlayEx(PlayOptions.BeforeEnd);
                    break;
                case Keys.Space:
                case Keys.S:
                case Keys.R:
                case Keys.T:
                    AudioPlay();
                    break;
                case Keys.Enter:
                case Keys.G:
                    int idx = gridASS.SelectedRows[0].Index;
                    if (idx < gridASS.Rows.Count - 1)
                        moveViewRows(idx);
                    commitChanges();
                    break;
                case Keys.Left:
                case Keys.Z:
                    selPrevLine();
                    break;
                case Keys.Right:
                case Keys.X:
                    selNextLine();
                    break;
                case Keys.F:
                    if (e.Control) switchSpectrumAnalyzer();
                    else
                    {
                        if (hSliderBar.Value < (hSliderBar.Maximum - (((double)panelAudioBox.Width / (double)AudioGrid_pxPerSecond) * 10)))
                        hSliderBar.Value += 10;
                    }
                    break;
                case Keys.A:
                    if (hSliderBar.Value>10)
                        hSliderBar.Value -= 10;
                    break;
            }
            e.Handled = true;
            SendKeys.Send("");
        }

        private void switchSpectrumAnalyzer()
        {
            if (AudioWaveType == WaveType.Normal)
                AudioWaveType = WaveType.FFT;
            else if (AudioWaveType == WaveType.FFT)
                AudioWaveType = WaveType.FFTWave;
            else 
                AudioWaveType = WaveType.Normal;

            updateAudioWave();
            updateAudioGrid();

        }

        private void toolStripButton4_Click_1(object sender, EventArgs e)
        {
            switchSpectrumAnalyzer();
        }

        private void CloseAudio()
        {
            isAudioLoaded = false;
            if (AudioActualPosition_Refresh.Enabled) AudioActualPosition_Refresh.Enabled = false;
            //if (AudioWave != null) AudioWave = new Bitmap(AudioWave.Width, AudioWave.Height); // AudioWave.Dispose();
            //if (AudioGrid != null) AudioGrid = new Bitmap(AudioGrid.Width, AudioGrid.Height); // AudioGrid.Dispose();
            //if (AudioFull != IntPtr.Zero) 
            Marshal.FreeHGlobal(AudioFull);
            if (audio != null) audio.Dispose();

            updateMenuEnables();
            drawPositions();

        }

        private void cerrarAudioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseAudio();
        }

        public double AudioGrid_Inicio
        {
            get { return audioGrid_Inicio; }
            set
            {
                audioGrid_Inicio = value;
                if (value != -1)
                {
                    textInicio.Text = Tiempo.SecondToTimeString(value);
                }
            }
        }

        public double AudioGrid_Fin
        {
            get { return audioGrid_Fin; }
            set
            {
                audioGrid_Fin = value;
                if (value != -1)
                {
                    textFin.Text = Tiempo.SecondToTimeString(value);
                }
            }
        }
        #endregion

        #region AUDIO CONTROLS
        private void button9_Click(object sender, EventArgs e)
        {
            commitChanges();
            AudioGridBox.Focus();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            AudioPlay();
            AudioGridBox.Focus();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            AudioGridBox.Focus();

        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            int idx = gridASS.SelectedRows[0].Index;
            if (idx < gridASS.Rows.Count - 1)
                moveViewRows(idx);

            commitChanges();
            AudioGridBox.Focus();
        }
        private void nextLine_Click(object sender, EventArgs e)
        {
            selNextLine();
            AudioGridBox.Focus();
        }

        private void prevLine_Click(object sender, EventArgs e)
        {
            selPrevLine();
            AudioGridBox.Focus();
        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            AudioPlayEx(PlayOptions.BeforeStart);
            AudioGridBox.Focus();
        }

        private void toolStripButton11_Click_1(object sender, EventArgs e)
        {
            AudioPlayEx(PlayOptions.AfterStart);
            AudioGridBox.Focus();
        }

        private void overwriteMode_Click(object sender, EventArgs e)
        {
            updateReplaceConfigFile("mainW_AudioOverWriteMode", overwriteMode.Checked.ToString());
        }

        private void textWave_Click(object sender, EventArgs e)
        {
            updateReplaceConfigFile("mainW_AudioTextOverWave", textWave.Checked.ToString());
            updateAudioGrid();
        }


        private void toolStripButton9_Click_1(object sender, EventArgs e)
        {
            AudioPlayEx(PlayOptions.BeforeEnd);
            AudioGridBox.Focus();
        }

        private void toolStripButton8_Click_1(object sender, EventArgs e)
        {
            AudioPlayEx(PlayOptions.AfterEnd);
            AudioGridBox.Focus();
        }

        private void selNextLine()
        {
            int j = goNextLine();
            lineaASS lass = (lineaASS)al[j];

            AudioGrid_Inicio = lass.t_inicial.getTiempo();
            AudioGrid_Fin = lass.t_final.getTiempo();

            int demas = Convert.ToInt32(Math.Floor(((double)panelAudioBox.Width / (double)AudioGrid_pxPerSecond)));

            int x = (int)(AudioGrid_Inicio * 10) - 10;
            int x2 = Math.Min(x, hSliderBar.Maximum - demas);

            hSliderBar.Value = (AudioGrid_Inicio >= 1) ? x2 : 0;
            updateAudioGrid();

        }

        private void selPrevLine()
        {
            int j = goPrevLine();
            lineaASS lass = (lineaASS)al[j];

            AudioGrid_Inicio = lass.t_inicial.getTiempo();
            AudioGrid_Fin = lass.t_final.getTiempo();

            int demas = Convert.ToInt32(Math.Floor(((double)panelAudioBox.Width / (double)AudioGrid_pxPerSecond)));

            int x = (int)(AudioGrid_Inicio * 10) - 10;
            int x2 = Math.Min(x, hSliderBar.Maximum - demas);

            hSliderBar.Value = (AudioGrid_Inicio >= 1) ? x2 : 0;
            updateAudioGrid();

        }
        private void toolStripButton6_Click_2(object sender, EventArgs e)
        {
            karaokeTimerW kt = new karaokeTimerW(this, avs2ds, avsaudio, AudioFull);
            this.Enabled = false;
            kt.Show();
            kt.Focus();
        }

        private void AudioActualPositionInterval_ValueChanged(object sender, EventArgs e)
        {
            AudioActualPosition_Refresh.Interval = (int)AudioActualPositionInterval.Value * 10;
            AudioActualPosition_Refresh_500.Interval = (int)AudioActualPositionInterval.Value * 10;
            updateReplaceConfigFile("mainW_AudioInterval", AudioActualPositionInterval.Value.ToString());
        }

        private void AudioMultiplicadorBuffer_ValueChanged(object sender, EventArgs e)
        {
            updateReplaceConfigFile("mainW_AudioMultiplier", AudioMultiplicadorBuffer.Value.ToString());
        }

        #endregion
    }
}
