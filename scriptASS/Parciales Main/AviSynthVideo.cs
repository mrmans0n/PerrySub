using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace scriptASS
{
    partial class mainW
    {
        private AviSynthScriptEnvironment avsEnv, VideoBaseEnv;
        private AviSynthClip avsClip, VideoBaseAvs;
        Object cerrojazo = new Object();

        #region SOPORTE AVS

        private void AVSRefreshSubs()
        {
            try
            {
                if (File.Exists(editedFile)) File.Delete(editedFile);
            }
            catch { }

            try
            {
                
                string tempfile = editedFile;
                int c = 1;
                while (File.Exists(tempfile))
                    tempfile = editedFile + (c++); // generar nombre secuencial (más barato que aletorio)

                lock (cerrojazo)
                {
                    script.SaveToFile(tempfile);
                    avsClip.ClipExchange(avsClip.GetAVS(), 0, 1);
                    avsClip.AviSynthInvoke(avsClip.GetAVS(), 1, "TextSub", true, tempfile);
                }
                seekToFrame(FrameIndex);
            }
            catch (Exception ex)
            {
                errorMsg("Error refrescando el subtítulo.\n" + ex.Data);
            }
        }

        private void updatePreviewAVS()
        {
            if (isVideoLoaded && VideoBoxType == PreviewType.AviSynth)
            {
                try
                {
                    lock (cerrojazo)
                        AVSRefreshSubs();
                }
                catch
                {
                    CallUndo();
                    lock (cerrojazo)
                        AVSRefreshSubs();
                }
            }
        }

        public void seekToFrame(int f)
        {
            lock (cerrojazo)
            {
                ActualFrame = AviSynthFunctions.getBitmapFromFrame(avsClip, 1, f);
                videoPictureBox.Image = ActualFrame;
            }
        }

        public int getFrameIndex()
        {
            return FrameIndex;
        }

        public void setFrameIndex(int f)
        {
            //FrameIndexMutex.WaitOne(Convert.ToInt32(1000 / videoInfo.FrameRate), true);
            FrameIndex = f;
            //FrameIndexMutex.ReleaseMutex();
        }

        public void seekToFrameAndUpdate(int f)
        {
            seekToFrame(f);
            setFrameIndex(f);
        }
        
        private void CheckIndexFile(string fname)
        {
            FileInfo videofile = new FileInfo(fname);
            string ffindexfile = Path.Combine(indexDir, videofile.Name + ".ffindex");
            string copiedindexfile = Path.Combine(videofile.DirectoryName, videofile.Name + ".ffindex");

            if (File.Exists(copiedindexfile) && !File.Exists(ffindexfile))
            {
                File.Copy(copiedindexfile, ffindexfile, true);
                return;
            }

            if (File.Exists(ffindexfile))
            {
                if (!File.Exists(copiedindexfile))
                {
                    File.Copy(ffindexfile, copiedindexfile, true);
                    UsedCacheFiles.Add(copiedindexfile);
                }
            }
        }

        private VideoSourceType GetVideoSourceFilter(string filename)
        {
            bool AviSourceCompatible = filename.EndsWith(".avi", StringComparison.InvariantCultureIgnoreCase) || filename.EndsWith(".divx", StringComparison.InvariantCultureIgnoreCase) || filename.EndsWith(".xvid", StringComparison.InvariantCultureIgnoreCase);
            bool ImportCompatible = filename.EndsWith(".avs", StringComparison.InvariantCultureIgnoreCase) || filename.EndsWith(".avsi", StringComparison.InvariantCultureIgnoreCase);
            AviSynthClip avsclip = null, avsclip_test;
            AviSynthScriptEnvironment avsenv, avsenv_test;

            avsenv_test = new AviSynthScriptEnvironment();
            avsclip_test = avsenv_test.ParseScript("BlankClip");
            avsclip_test.AviSynthInvoke(avsclip_test.GetAVS(), 0, "LoadPlugin", false, Path.Combine(Application.StartupPath, "ffms2.dll"));
            avsclip_test.AviSynthInvoke(avsclip_test.GetAVS(), 0, "LoadPlugin", false, Path.Combine(Application.StartupPath, "DirectShowSource.dll"));
            avsclip_test.AviSynthInvoke(avsclip_test.GetAVS(), 0, "LoadPlugin", false, Path.Combine(Application.StartupPath, "avss.dll"));


            foreach (string sfilter in VideoFilterPriority) //Enum.GetNames(typeof(VideoSourceType))
            {
                if (sfilter.Equals(VideoSourceType.AviSource.ToString()))
                {
                    if (AviSourceCompatible) // avisource es un core filter, existe siempre.
                    {
                        try
                        {
                            avsenv = new AviSynthScriptEnvironment();
                            avsclip = avsenv.ParseScript("AviSource(\"" + filename + "\")");
                            avsclip.cleanup(true);
                            avsclip_test.cleanup(true);
                            return VideoSourceType.AviSource;
                        }
                        catch { if (avsclip != null) avsclip.cleanup(true); }
                    }
                }
                else if (sfilter.Equals(VideoSourceType.Import.ToString()))
                {
                    if (ImportCompatible)
                    {
                        try
                        {
                            avsenv = new AviSynthScriptEnvironment();
                            avsclip = avsenv.ParseScript("Import(\"" + filename + "\")");
                            avsclip.cleanup(true);
                            avsclip_test.cleanup(true);
                            return VideoSourceType.Import;
                        }
                        catch { if (avsclip != null) avsclip.cleanup(true); }
                    }
                }
                else
                {
                    bool isFFMS2 = sfilter.Equals(VideoSourceType.FFVideoSource.ToString());
                    bool isDSS = sfilter.Equals(VideoSourceType.DirectShowSource.ToString());
                    bool isDSS2 = sfilter.Equals(VideoSourceType.DSS2.ToString());

                    try
                    {
                        if (avsclip_test.FuncExists(avsclip_test.GetAVS(), sfilter))// || isFFMS2)
                        {

                            avsenv = new AviSynthScriptEnvironment();
                            avsclip = avsenv.ParseScript("BlankClip");
                            if (isFFMS2)
                            {
                                avsclip.AviSynthInvoke(avsclip.GetAVS(), 0, "LoadPlugin", false,
                                                       Path.Combine(Application.StartupPath, "ffms2.dll"));
                                CheckIndexFile(filename);
                            }
                        
                            if (isDSS) avsclip.AviSynthInvoke(avsclip.GetAVS(), 0, "LoadPlugin", false, Path.Combine(Application.StartupPath, "DirectShowSource.dll"));
                            if (isDSS2) avsclip.AviSynthInvoke(avsclip.GetAVS(), 0, "LoadPlugin", false, Path.Combine(Application.StartupPath, "avss.dll"));
                            //avsclip = avsenv.ParseScript(sfilter + "(\"" + filename + "\")");
                            avsclip.AviSynthInvoke(avsclip.GetAVS(), 0, sfilter, false, filename);
                            avsclip.cleanup(true);
                            avsclip_test.cleanup(true);
                            return (VideoSourceType)Enum.Parse(typeof(VideoSourceType), sfilter);
                        }
                        else errorMsg("No encontrada la función ffvideosource");
                    }
                    catch
                    {
                        if (avsclip != null) avsclip.cleanup(true);
                    }
                    
                }
            }
            if (avsclip != null) avsclip.cleanup(true);
            avsclip_test.cleanup(true);
            throw new AviSynthException("Imposible cargar el vídeo en modo AviSynth.");

            return VideoSourceType.AviSource; // codigo inalcanzable
        }

        private AudioSourceType GetAudioSourceFilter(string filename)
        {
            bool AviSourceCompatible = filename.EndsWith(".avi", StringComparison.InvariantCultureIgnoreCase) || filename.EndsWith(".divx", StringComparison.InvariantCultureIgnoreCase) || filename.EndsWith(".xvid", StringComparison.InvariantCultureIgnoreCase);
            bool ImportCompatible = filename.EndsWith(".avs", StringComparison.InvariantCultureIgnoreCase) || filename.EndsWith(".avsi", StringComparison.InvariantCultureIgnoreCase);
            bool WavSourceCompatible = filename.EndsWith(".wav", StringComparison.InvariantCultureIgnoreCase);

            AviSynthClip avsclip = null, avsclip_test;
            AviSynthScriptEnvironment avsenv, avsenv_test;

            avsenv_test = new AviSynthScriptEnvironment();
            avsclip_test = avsenv_test.ParseScript("BlankClip");
            avsclip_test.AviSynthInvoke(avsclip_test.GetAVS(), 0, "LoadPlugin", false, Path.Combine(Application.StartupPath, "ffms2.dll"));
            avsclip_test.AviSynthInvoke(avsclip_test.GetAVS(), 0, "LoadPlugin", false, Path.Combine(Application.StartupPath, "DirectShowSource.dll"));
            avsclip_test.AviSynthInvoke(avsclip_test.GetAVS(), 0, "LoadPlugin", false, Path.Combine(Application.StartupPath, "avss.dll"));

            foreach (string sfilter in AudioFilterPriority)//Enum.GetNames(typeof(AudioSourceType)))
            {
                if (sfilter.Equals(AudioSourceType.AviSource.ToString()))
                {
                    if (AviSourceCompatible) // avisource es un core filter, existe siempre.
                    {
                        try
                        {
                            avsenv = new AviSynthScriptEnvironment();
                            avsclip = avsenv.ParseScript("AviSource(\"" + filename + "\")");
                            if (avsclip.SamplesCount <= 0)
                            {
                                avsclip.cleanup(true);
                                avsclip_test.cleanup(true);
                                throw new AviSynthException("No puede abrirse un audio sin samples.");
                            }
                            avsclip.cleanup(true);
                            avsclip_test.cleanup(true);
                            return AudioSourceType.AviSource;
                        }
                        catch
                        {
                            if (avsclip != null) avsclip.cleanup(true);
                        }
                    }
                }
                else if (sfilter.Equals(AudioSourceType.Import.ToString()))
                {
                    if (ImportCompatible)
                    {
                        try
                        {
                            avsenv = new AviSynthScriptEnvironment();
                            avsclip = avsenv.ParseScript("Import(\"" + filename + "\")"); 
                            if (avsclip.SamplesCount <= 0)
                            {
                                avsclip.cleanup(true);
                                avsclip_test.cleanup(true);
                                throw new AviSynthException("No puede abrirse un audio sin samples.");
                            }
                            avsclip.cleanup(true);
                            avsclip_test.cleanup(true);
                            return AudioSourceType.Import;
                        }
                        catch { if (avsclip != null) avsclip.cleanup(true); }
                    }
                }
                else
                {
                    bool isFFMS2 = sfilter.Equals(AudioSourceType.FFAudioSource.ToString());
                    bool isDSS = sfilter.Equals(AudioSourceType.DirectShowSource.ToString());
                    bool isWS = sfilter.Equals(AudioSourceType.WavSource.ToString());
                    
                    try
                    {
                        if (!isWS || (isWS && WavSourceCompatible))
                        {
                            if (avsclip_test.FuncExists(avsclip_test.GetAVS(), sfilter))
                            {
                                avsenv = new AviSynthScriptEnvironment();
                                avsclip = avsenv.ParseScript("BlankClip");

                                if (isFFMS2)
                                {
                                    avsclip.AviSynthInvoke(avsclip.GetAVS(), 0, "LoadPlugin", false,
                                                           Path.Combine(Application.StartupPath, "ffms2.dll"));
                                    CheckIndexFile(filename);
                                }
                                if (isDSS) avsclip.AviSynthInvoke(avsclip.GetAVS(), 0, "LoadPlugin", false, Path.Combine(Application.StartupPath, "DirectShowSource.dll"));

                                avsclip.AviSynthInvoke(avsclip.GetAVS(), 0, sfilter, false, filename);

                                if (avsclip.SamplesCount <= 0)
                                {
                                    avsclip.cleanup(true);
                                    avsclip_test.cleanup(true);
                                    throw new AviSynthException("No puede abrirse un audio sin samples.");
                                }
                                else
                                {
                                    avsclip.cleanup(true);
                                    avsclip_test.cleanup(true);
                                    return (AudioSourceType) Enum.Parse(typeof (AudioSourceType), sfilter);
                                }
                            }
                        }
                    }
                    catch
                    {
                        if (avsclip != null) avsclip.cleanup(true);
                    }
                }
            }
            if (avsclip != null) avsclip.cleanup(true);
            if (avsclip_test != null) avsclip_test.cleanup(true);

            return AudioSourceType.None; // codigo inalcanzable
        }


        private void openAVS(string fname)
        {
            videoPictureBox.Visible = true;
            if (VideoBoxType == PreviewType.DirectShow)
            {
                VideoBoxType = PreviewType.AviSynth;

                videoWindow.put_Visible(DirectShowLib.OABool.False);

                setStatus("Cargando modo AviSynth. Dependiendo de los filtros, puede tardar unos segundos...");
                try
                {
                    OpenSourceType = GetVideoSourceFilter(fname);
                    OpenAudioSourceType = HasAudio(fname) ? GetAudioSourceFilter(fname) : AudioSourceType.None;

                    if (avsClip != null) avsClip.cleanup(true);
                    if (avsaudio != null) avsaudio.cleanup(true);                

                }
                catch (AviSynthException x)
                {
                    errorMsg("No se puede entrar en el modo AviSynth.\nMensaje de error : " + x.Message);
                    VideoBoxType = PreviewType.DirectShow;
                    videoWindow.put_Visible(DirectShowLib.OABool.True);
                    return;
                }
               
                try
                {
                    avsEnv = new AviSynthScriptEnvironment();
                    avsClip = avsEnv.ParseScript("BlankClip");
                    avsaudio = avsEnv.ParseScript("BlankClip");

                    string vsf = AviSynthFunctions.getVSFilterPath(avsClip);
                    if (vsf != null)
                        avsClip.AviSynthInvoke(avsClip.GetAVS(), 0, "LoadPlugin", false, vsf);

                    switch (OpenSourceType)
                    {
                        case VideoSourceType.FFVideoSource:
                            avsClip.AviSynthInvoke(avsClip.GetAVS(), 0, "LoadPlugin", false, Path.Combine(Application.StartupPath, "ffms2.dll"));
                            break;
                        case VideoSourceType.DirectShowSource:
                            avsClip.AviSynthInvoke(avsClip.GetAVS(), 0, "LoadPlugin", false, Path.Combine(Application.StartupPath, "DirectShowSource.dll"));
                            break;
                        case VideoSourceType.DSS2:
                            avsClip.AviSynthInvoke(avsClip.GetAVS(), 0, "LoadPlugin", false, Path.Combine(Application.StartupPath, "avss.dll"));
                            break;
                    }

                    avsClip.AviSynthInvoke(avsClip.GetAVS(), 0, OpenSourceType.ToString(), false, fname);
                    avsClip.AviSynthInvoke(avsClip.GetAVS(), 0, "killaudio", false, "Nulo");
                    avsClip.AviSynthInvoke(avsClip.GetAVS(), 0, "converttorgb32", false, "Nulo");

                    avsClip.AviSynthInvoke(avsClip.GetAVS(), 1, OpenSourceType.ToString(), false, fname);
                    avsClip.AviSynthInvoke(avsClip.GetAVS(), 1, "killaudio", false, "Nulo");
                    avsClip.AviSynthInvoke(avsClip.GetAVS(), 1, "converttorgb32", false, "Nulo");

                    try
                    {
                        if (OpenAudioSourceType != AudioSourceType.None)
                        {
                            switch (OpenAudioSourceType)
                            {
                                case AudioSourceType.FFAudioSource:
                                    avsaudio.AviSynthInvoke(avsaudio.GetAVS(), 0, "LoadPlugin", false, Path.Combine(Application.StartupPath, "ffms2.dll"));
                                    break;
                                case AudioSourceType.DirectShowSource:
                                    avsaudio.AviSynthInvoke(avsaudio.GetAVS(), 0, "LoadPlugin", false, Path.Combine(Application.StartupPath, "DirectShowSource.dll"));
                                    break;
                            }

                            avsaudio.AviSynthInvoke(avsaudio.GetAVS(), 0, OpenAudioSourceType.ToString(), false, fname);
                            avsaudio.AviSynthInvoke(avsaudio.GetAVS(), 0, "killvideo", false, "Nulo");
                        }
                    }
                    catch { }

                    updatePreviewAVS();

                }
                catch (AviSynthException x)
                {
                    errorMsg("No se puede entrar en el modo AviSynth. Comprueba que tienes instalado el AviSynth, que tienes registrado el VSFilter.dll (o mételo en la carpeta \\plugins del directorio de AviSynth), y que no intentas abrir nada raro.\nMensaje de error : " + x.Message);
                    return;
                }
                isVideoLoaded = true;
                butClip.Enabled = true;

                updateMenuEnables();

                videoInfo.Resolution = new Size(avsClip.VideoWidth, avsClip.VideoHeight);
                videoInfo.FrameTotal = avsClip.num_frames;

                setStatus("Vídeo " + fname + " cargado. [AviSynth (V: " + OpenSourceType.ToString() + ", A:" + OpenAudioSourceType.ToString() + ")]");
                script.GetHeader().SetHeaderValue("Video File", fname);

                if (isKeyframeGuessNeeded())
                    KeyframeGuess(false);
            }
        }
        #endregion

    }
}
