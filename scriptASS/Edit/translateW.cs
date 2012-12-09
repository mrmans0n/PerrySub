using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using DirectShowLib;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Generic;

namespace scriptASS
{
    public partial class translateW : Form
    {
        ArrayList Diccionarios = new ArrayList();

        private string StatusFile = Path.Combine(Application.StartupPath, "translateW.info");
        private int actualEdit = 0;
        private string PosicionActual = "0:00:00.00/0:00:00.00";

        private long stopTime;

        private mainW mW;
        private ArrayList al, autoComplete;
        //private double fps;

        //private string openFileVideo;
        private VideoInfo videoInfo;

        private DateTime TiempoInicio = DateTime.Now;
        private int PulsacionesTotales = 0;

        private RPC_Client cliente;

        delegate void SetUriInWebBrowser(WebBrowser w, Uri uri);

        private bool IsModified = false;
        private Stack<Tag> openTags;

        #region Variables DShow
        private int VolumeFull = 0;
        private int VolumeSilence = -10000;
        private IGraphBuilder graphBuilder;
        private IMediaControl mediaControl;
        //private IMediaEventEx mediaEventEx;
        private IVideoWindow videoWindow;
        private IBasicAudio basicAudio;
        private IBasicVideo basicVideo;
        private IMediaSeeking mediaSeeking;
        private IMediaPosition mediaPosition;
        //private IVideoFrameStep frameStep;
        #endregion



        public translateW(mainW mW, VideoInfo vinfo)
        {
            InitializeComponent();
            this.mW = mW;
            this.videoInfo = vinfo;
            this.KeyDown += new KeyEventHandler(translateW_KeyDown);
            //this.FormClosing += new FormClosingEventHandler(translateW_FormClosing);
            this.Disposed += new EventHandler(translateW_Disposed);
            this.FormClosing += new FormClosingEventHandler(translateW_FormClosing);
            openTags = new Stack<Tag>();
        }


        void translateW_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool massugu_Go = false;

            if (!modeSelector.Checked || !IsModified)
                massugu_Go = true;
            else
                massugu_Go = (MessageBox.Show("Se está intentando cerrar la ventana sin haber guardado los cambios.\n¿Estás seguro de continuar cerrando el Asistente de Traducción?", mainW.appTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes);

            if (massugu_Go)
            {
                try
                {
                    mediaControl.Stop();
                }
                catch { }
                
                mW.Enabled = true;
                if (!modeSelector.Checked)
                    mW.updateGridWithArrayList(al);
                try
                {
                    mW.openVid(videoInfo.FileName, videoInfo.FrameIndex);
                } catch (Exception ex)
                {
                    mW.errorMsg("Error abriendo el vídeo " + videoInfo.FileName + " por el frame " +
                                videoInfo.FrameIndex);
                }
                closeAll();
                mW.drawPositions();

                this.Dispose();
            }
            else
                e.Cancel = true;
            
        }

        void translateW_Disposed(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(StatusFile))
                    File.Delete(StatusFile);
            }
            catch { }

            //mW.Enabled = true;
            mW.openVid(videoInfo.FileName, videoInfo.FrameIndex);
            closeAll();
            mW.drawPositions();
            //mW.Focus();
        }

        private void FocusTab(int idx)
        {
            try
            {
                tabControl1.SelectedIndex = idx;
                textTradu.Focus();
            }
            catch { }
        }

        // interceptar media keys

        protected override void WndProc(ref Message msg)
        {
            switch (msg.Msg)
            {
                case 0x319: // WM_APPCOMMAND message
                    // extract cmd from LPARAM (as GET_APPCOMMAND_LPARAM macro does)
                    int cmd = (int)((uint)msg.LParam >> 16 & ~0xf000);
                    switch (cmd)
                    {
                        case 13:  // APPCOMMAND_MEDIA_STOP
                            timer2.Enabled = false;
                            mediaControl.Stop();
                            break;
                        case 14:  // APPCOMMAND_MEDIA_PLAY_PAUSE
                            PlayPause();
                            break;
                        case 11:  // APPCOMMAND_MEDIA_NEXTTRACK
                            if (audioMode.Checked)
                                moveVideoSeconds(5);
                            break;
                        case 12:  // APPCOMMAND_MEDIA_PREVIOUSTRACK
                            if (audioMode.Checked)
                                moveVideoSeconds(-5);
                            break;
                        default:
                            break;
                    }
                    
                    break;
                case 0x104: // WM_SYSCOMMAND
                    int cmd2 = (int)((uint)msg.LParam >> 16 & ~0xf000);
                    //MessageBox.Show(cmd2.ToString());
                    break;
            }
            base.WndProc(ref msg);
        }


        void translateW_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.D1: // contexto
                    if (e.Control && e.Shift) tabControl1.SelectedTab = tabContexto;
                    else if (e.Alt)
                        if (audioMode.Checked)
                            moveVideoSeconds(-5);
                    break;
                case Keys.D2: // opciones
                    if (e.Control && e.Shift) tabControl1.SelectedTab = tabOpciones;
                    else if (e.Alt)
                        if (audioMode.Checked)
                            moveVideoSeconds(-10);
                    break;
                case Keys.D3: // visor
                    if (e.Control && e.Shift) tabControl1.SelectedTab = tabVisor;
                    else if (e.Alt)
                        if (audioMode.Checked)
                            moveVideoSeconds(5);
                    break;
                case Keys.D4: // dicc. terminos
                    if (e.Control && e.Shift) tabControl1.SelectedTab = tabDiccionario;
                    else if (e.Alt)
                        if (audioMode.Checked)
                            moveVideoSeconds(10);

                    break; 
                case Keys.D5:
                    if (e.Control && e.Shift) FocusTab(4);
                    break;
                case Keys.D6:
                    if (e.Control && e.Shift) FocusTab(5);
                    break;
                case Keys.D7:
                    if (e.Control && e.Shift) FocusTab(6);
                    break;
                case Keys.D8:
                    if (e.Control && e.Shift) FocusTab(7);
                    break;
                case Keys.D9:
                    if (e.Control && e.Shift) FocusTab(8);
                    break;
                case Keys.D0:
                    if (e.Control && e.Shift) FocusTab(9);
                    break;
                case Keys.Tab:
                    if (e.Control)
                    {
                        if (tabControl1.SelectedIndex < tabControl1.Controls.Count-1)
                            tabControl1.SelectedIndex++;
                        else 
                            tabControl1.SelectedIndex = 0;

                        textTradu.Focus();
                        e.Handled = true;
                    }
                    break;

                case Keys.Play:
                case Keys.F2:
                    PlayPause();
                    break;
                case Keys.F3:
                    if (!modeSelector.Checked)
                        PreTiming(false);
                    break;
                case Keys.F4:
                    string tradu = textTradu.Text;
                    if (tradu.IndexOf(':') != -1)
                    {
                        lineaASS lass = (lineaASS)al[actualEdit];
                        string[] pjtexto = tradu.Split(new char[] { ':' }, 2);
                        string pj = pjtexto[0].Trim();
                        tradu = pjtexto[1].Trim();
                        textTradu.Text = tradu;
                        textPersonaje.Text = pj;
                    }
                    break;
            }
        }

        private void PlayPause()
        {
            DirectShowLib.FilterState state;
            mediaControl.GetState(0, out state);
            if (state == FilterState.Running)
                mediaControl.Pause();
            else
                mediaControl.Run();

            if (!modeSelector.Checked)
                PreTiming(true); 
        }

        private void moveVideoSeconds(int secs)
        {
            
            int curpos = VideoUnitConversion.getCurPos(mediaSeeking, videoInfo.FrameRate);
            int maxpos = VideoUnitConversion.getTotal(mediaSeeking, videoInfo.FrameRate);
            int numframes = (int)Math.Round((double)secs * videoInfo.FrameRate);
            int newpos = 0;
            if (numframes < 0 && numframes > curpos) newpos = 0;
            else 
                if ((curpos + numframes) > maxpos)
                    newpos = maxpos;
                else
                    newpos = curpos+numframes;
            VideoUnitConversion.setNewPos(mediaSeeking, newpos, videoInfo.FrameRate); //mediaSeeking.SetPositions(l, AMSeekingSeekingFlags.AbsolutePositioning, null, AMSeekingSeekingFlags.NoPositioning);
        }

        private void updateMainW()
        {
            for (int i = 0; i < gridCont.RowCount; i++)
            {
                lineaASS tmp = (lineaASS)al[i];                
                tmp.texto = gridCont[1, i].Value.ToString();
                tmp.personaje = gridCont[0, i].Value.ToString();
                if (!checkSaveTime.Checked && !modeSelector.Checked)
                {
                    tmp.t_inicial.setTiempo(0);
                    tmp.t_final.setTiempo(0);
                }
                al[i] = tmp;
            }
        }

        private void closeAll()
        {
            timer1.Enabled = false;
            timer2.Enabled = false;
            AutoSaveTimer.Enabled = false;
            estadoConexion.Enabled = false;
            videoPanel.Visible = false;

            if (videoWindow != null)
            {
                videoWindow.put_Visible(DirectShowLib.OABool.False);
                videoWindow.put_Owner(IntPtr.Zero);
            }
            videoPanel.Dispose();
            if (mediaControl!=null) mediaControl.Stop();
            // if (graphBuilder!=null) graphBuilder.Abort();
            
            mediaControl = null;
            //mediaEventEx = null;
            videoWindow = null;
            basicAudio = null;
            basicVideo = null;
            mediaSeeking = null;
            mediaPosition = null;

            try
            {

                Marshal.ReleaseComObject(graphBuilder);
                graphBuilder = null;
            }
            catch { } 

            GC.Collect();

            //frameStep = null;            

        }

        private void translateW_Load(object sender, EventArgs e)
        {
            //this.MaximumSize = this.Size;
            //this.MinimumSize = this.Size;
            toolStripStatusLabel2.Text = "Cargando el Asistente de Traducción...";
            // cargamos script
            autoComplete = new ArrayList();
            al = mW.al;
            gridCont.RowCount = al.Count;

            bool hasAutoComplete = (mW.script.GetHeader().GetHeaderValue("AutoComplete") != string.Empty);

            for (int i = 0; i < al.Count; i++)
            {
                lineaASS lass = (lineaASS)al[i];
                gridCont[0, i].Value = lass.personaje;
                if (!autoComplete.Contains(lass.personaje) && !hasAutoComplete)
                    if (lass.personaje.Trim()!="")
                        autoComplete.Add(lass.personaje);
                gridCont[1, i].Value = lass.texto;
            }
            if (hasAutoComplete) InsertAutoCompleteFromScript();

            labelLineaActual.Text = "1 de " + (al.Count) + " (0%)";
            textPersonaje.Text = gridCont[0, 0].Value.ToString();
            textOrig.Text = gridCont[1, 0].Value.ToString();

            // cargamos video

            graphBuilder = (IGraphBuilder)new FilterGraph();
            graphBuilder.RenderFile(videoInfo.FileName, null);
            mediaControl = (IMediaControl)graphBuilder;
            // mediaEventEx = (IMediaEventEx)this.graphBuilder;
            mediaSeeking = (IMediaSeeking)graphBuilder;
            mediaPosition = (IMediaPosition)graphBuilder;
            basicVideo = graphBuilder as IBasicVideo;
            basicAudio = graphBuilder as IBasicAudio;
            videoWindow = graphBuilder as IVideoWindow;

            try
            {
                int x, y; double atpf;
                basicVideo.GetVideoSize(out x, out y);
                basicVideo.get_AvgTimePerFrame(out atpf);
                videoInfo.FrameRate = Math.Round(1 / atpf, 3);

                int new_x = videoPanel.Width;
                int new_y = (new_x * y) / x;
                videoWindow.put_Height(new_x);
                videoWindow.put_Width(new_y);
                videoWindow.put_Owner(videoPanel.Handle);
                videoPanel.Size = new System.Drawing.Size(new_x, new_y);
                videoWindow.SetWindowPosition(0, 0, videoPanel.Width, videoPanel.Height);
                videoWindow.put_WindowStyle(WindowStyle.Child);
                videoWindow.put_Visible(DirectShowLib.OABool.True);
                mediaSeeking.SetTimeFormat(DirectShowLib.TimeFormat.Frame);

                mediaControl.Run();
            }
            catch { mW.errorMsg("Imposible cargar el vídeo. Debe haber algún problema con el mismo, y el asistente será muy inestable"); }
            // activamos timers & handlers

            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Enabled = true;
            timer2.Tick += new EventHandler(timer2_Tick);
            AutoSaveTimer.Tick += new EventHandler(timer3_Tick);
            AutoSaveTimer.Enabled = true;

            gridCont.CellClick += new DataGridViewCellEventHandler(gridCont_CellClick);
            textPersonaje.TextChanged += new EventHandler(textPersonaje_TextChanged);
            textTradu.TextChanged += new EventHandler(textTradu_TextChanged);
            textTradu.KeyUp += new KeyEventHandler(textBox1_KeyUp);
            textTradu.KeyDown += new KeyEventHandler(textTradu_KeyDown);
            textTradu.KeyPress += new KeyPressEventHandler(textTradu_KeyPress);
            textPersonaje.KeyDown += new KeyEventHandler(textPersonaje_KeyDown);
            textPersonaje.KeyPress += new KeyPressEventHandler(textPersonaje_KeyPress);
            button8.GotFocus += new EventHandler(button8_GotFocus);
            button9.GotFocus += new EventHandler(button9_GotFocus);
            gridCont.DoubleClick += new EventHandler(gridCont_DoubleClick);
            gridCont.SelectionChanged += new EventHandler(gridCont_SelectionChanged);
            gridCont.KeyUp += new KeyEventHandler(gridCont_KeyUp);
            listBox1.KeyUp += new KeyEventHandler(listBox1_KeyUp);
            textToAdd.KeyPress += new KeyPressEventHandler(textToAdd_KeyPress);
            progressBar1.MouseDown += new MouseEventHandler(progressBar1_MouseDown);
            tiempoInicio_.TimeValidated += new TimeTextBox.OnTimeTextBoxValidated(tiempo_TimeValidated);
            tiempoFin_.TimeValidated += new TimeTextBox.OnTimeTextBoxValidated(tiempo_TimeValidated);
            this.Move += new EventHandler(translateW_Move);

            //textTradu.ContextMenu = new ASSTextBoxRegExDefaultContextMenu(textTradu);

            mediaControl.Pause();

            // cargar de config

            try
            {
                checkAutoComplete.Checked = Convert.ToBoolean(mW.getFromConfigFile("translateW_autoC"));
                checkTagSSA.Checked = Convert.ToBoolean(mW.getFromConfigFile("translateW_tagSSA"));
                checkComment.Checked = Convert.ToBoolean(mW.getFromConfigFile("translateW_Comment"));
                checkVideo.Checked = Convert.ToBoolean(mW.getFromConfigFile("translateW_aud"));
                checkAudio.Checked = Convert.ToBoolean(mW.getFromConfigFile("translateW_vid"));
                checkSaveTime.Checked = Convert.ToBoolean(mW.getFromConfigFile("translateW_preTime"));
            }
            catch {}

            try
            {
                AutoSaveTimer.Interval = int.Parse(mW.getFromConfigFile("translateW_AutoSaveInterval"));
            }
            catch
            {
                AutoSaveTimer.Interval = 30000;
            }

            // fin de inicializacion vil
            textTradu.Focus();

            try
            {
                string[] bleh = mW.getFromConfigFileA("translateW_Reference");
                for (int i = 0; i < bleh.Length; i++)
                    Diccionarios.Add(bleh[i]);
            }
            catch
            {
                Diccionarios.Add("WordReference|http://www.wordreference.com/");
                Diccionarios.Add("Wikipedia|http://es.wikipedia.org");
                Diccionarios.Add("RAE|http://www.rae.es");
                Diccionarios.Add("Dictionary|http://dictionary.reference.com/");
            }
            diccionarios.DataSource = Diccionarios;

            CreateReferenceTabs();
            UpdateStatusFile();

            TiempoInicio = DateTime.Now;

            Estadisticas.Interval = 1000;
            Estadisticas.Tick += new EventHandler(Estadisticas_Tick);
            Estadisticas.Enabled = true;

            InitRPC();

            archivosView.KeyDown += new KeyEventHandler(archivosView_KeyDown);
            archivosView.SelectedIndexChanged += new EventHandler(archivosView_SelectedIndexChanged);

            textTradu.EnableSpellChecking = mW.spellEnabled;

            if (mW.spellEnabled)
            {
                textTradu.DictionaryPath = mW.dictDir;
                textTradu.Dictionary = mW.ActiveDict;
            }

            toolStripStatusLabel2.Text = "Asistente cargado correctamente.";

            bool showpopup = true;

            try
            {
                showpopup = Convert.ToBoolean(mW.getFromConfigFile("translateW_ShowPopup"));

            }
            catch
            {
                mW.updateReplaceConfigFile("translateW_ShowPopup", showpopup.ToString());
            }

            if (showpopup)
            {
                TranslationStyle estilo = TranslationStyle.FromScriptWithActors;
                translateW_Popup pop = new translateW_Popup(mW);
                switch (pop.ShowDialog())
                {
                    case DialogResult.Yes:
                        estilo = TranslationStyle.FromScriptWithActors;
                        break;
                    case DialogResult.No:
                        estilo = TranslationStyle.FromScriptWithoutActors;
                        break;
                    case DialogResult.Cancel:
                        estilo = TranslationStyle.FromScratch;
                        break;
                    case DialogResult.Ignore:
                        estilo = TranslationStyle.FromScratchAudio;
                        break;
                }

                switch (estilo)
                {
                    case TranslationStyle.FromScriptWithActors:
                        modeSelector.Checked = true;
                        splitText.Checked = false;
                        audioMode.Checked = false;
                        break;
                    case TranslationStyle.FromScriptWithoutActors:
                        modeSelector.Checked = true;
                        splitText.Checked = true;
                        audioMode.Checked = false;
                        break;
                    case TranslationStyle.FromScratch:
                        modeSelector.Checked = false;
                        splitText.Checked = false;
                        audioMode.Checked = false;
                        break;
                    case TranslationStyle.FromScratchAudio:
                        modeSelector.Checked = false;
                        splitText.Checked = true;
                        audioMode.Checked = true;
                        break;
                }
            }

        }

        void textTradu_TextChanged(object sender, EventArgs e)
        {
            CharsNumber.Text = lineaASS.cleanText(textTradu.Text).Length.ToString();
        }

        void gridCont_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C && !e.Alt && !e.Shift) // fix copiar
            {
                try { Clipboard.SetText(Clipboard.GetText().Replace("\t", ": ")); }
                catch { }
            }
        }

        void Estadisticas_Tick(object sender, EventArgs e)
        {
            DateTime TiempoActual = DateTime.Now;
            TimeSpan dt = TiempoActual - TiempoInicio;
            
            tiempoActivo.Text = dt.Hours + "h " + dt.Minutes + "m " + dt.Seconds + "s";
            PPM.Text = Convert.ToString(Math.Round((double)PulsacionesTotales / dt.TotalMinutes,1));
            UpdateStatusFile();
        }

        void tiempo_TimeValidated(object sender, EventArgs e)
        {
            lineaASS lass = (lineaASS)al[actualEdit];
            lass.t_inicial.setTiempo(tiempoInicio_.Text);
            lass.t_final.setTiempo(tiempoFin_.Text);
         }

        void gridCont_SelectionChanged(object sender, EventArgs e)
        {
            if (gridCont.SelectedRows.Count < 1 || gridCont.SelectedRows[0].Index < 0) return;

            int idx = gridCont.SelectedRows[0].Index;
            if (idx < 0) return;
            int tot = gridCont.Rows.Count;

            double perc = Math.Truncate((((double)idx + 1) * 100) / (double)tot);

            var PosActInicio = (double)(VideoUnitConversion.getCurPos(mediaSeeking, videoInfo.FrameRate) / videoInfo.FrameRate);
            var PosActTotal = (double)VideoUnitConversion.getTotal(mediaSeeking, videoInfo.FrameRate) / videoInfo.FrameRate;
            double perc_video =  Math.Truncate((PosActInicio/PosActTotal)*100);

            labelLineaActual.Text = (idx + 1) + " de " + tot + " (" + ( (audioMode.Checked)? perc_video : perc) + "%)";
            UpdateLineaN(idx);
        }


        void translateW_Move(object sender, EventArgs e)
        {
            if (videoWindow == null) return;
            videoWindow.put_Visible(OABool.False);
            videoWindow.SetWindowPosition(0,0, videoPanel.Width, videoPanel.Height);
            videoWindow.put_Visible(OABool.True);
        }

        void progressBar1_MouseDown(object sender, MouseEventArgs e)
        {
            int total = progressBar1.Size.Width;
            double p = (e.X * 100) / total;

            long dur = VideoUnitConversion.getTotal(mediaSeeking,videoInfo.FrameRate);
            double toSeek = dur * (p / 100);
            int toSeekInt = (int)toSeek;
            VideoUnitConversion.setNewPos(mediaSeeking,toSeekInt,videoInfo.FrameRate);
        }

        #region BEEP HACK 
        
        // hack para evitar el mec de las textboxes de 1 sola linea ._.

        void textPersonaje_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Convert.ToInt32(e.KeyChar) == 13)
            {
                System.Windows.Forms.SendKeys.Send("");

                e.Handled = true;
                textPersonaje.Focus();
            }

        }

        void textTradu_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (Convert.ToInt32(e.KeyChar)){
                case 13:
                    System.Windows.Forms.SendKeys.Send("");
                    listBox1.Visible = false;
                    e.Handled = true;
                    textTradu.Focus();
                    break;
                case 27:
                    System.Windows.Forms.SendKeys.Send("");
                    listBox1.Visible = false;
                    e.Handled = true;
                    break;
                default:
                    break;
            }            
        }

        void textToAdd_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (Convert.ToInt32(e.KeyChar))
            {
                case 13:
                    System.Windows.Forms.SendKeys.Send("");
                    e.Handled = true;
                    if (!autoComplete.Contains(textToAdd.Text))
                        autoComplete.Add(textToAdd.Text);
                    updateAutoCompleteList();
                    textToAdd.Visible = false;
                    textToAdd.Text = String.Empty;
                    button1.Focus();
                    break;
                case 27:
                    System.Windows.Forms.SendKeys.Send("");
                    e.Handled = true;
                    textToAdd.Visible = false;
                    textToAdd.Text = String.Empty;
                    button1.Focus();
                    break;
                default: break;
            };
        }
        #endregion

        /*void translateW_Disposed(object sender, EventArgs e)
        {
            closeAll();
        }*/

        void gridCont_DoubleClick(object sender, EventArgs e)
        {
            lineaASS lass = al[actualEdit] as lineaASS;
            double d = Math.Round(lass.t_inicial.getTiempo() * videoInfo.FrameRate);
            double d2 = Math.Round(lass.t_final.getTiempo() * videoInfo.FrameRate);
            long l = long.Parse(d.ToString());
            stopTime = long.Parse(d2.ToString());
            VideoUnitConversion.setNewPos(mediaSeeking, l, videoInfo.FrameRate); //mediaSeeking.SetPositions(l, AMSeekingSeekingFlags.AbsolutePositioning, null, AMSeekingSeekingFlags.NoPositioning);
            mediaControl.Run();
            timer2.Enabled = true;
        }

        void button9_GotFocus(object sender, EventArgs e)
        {
            textPersonaje.Focus();
        }

        void button8_GotFocus(object sender, EventArgs e)
        {
            textTradu.Focus();
        }

        void textPersonaje_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    gridCont[0, actualEdit].Value = textPersonaje.Text;
                    break;
                default: break;
            }
        }

        private void actualizarScroll()
        {
            int visibles = gridCont.DisplayedRowCount(false);

            if (al.Count > visibles/2)
            {
                if (actualEdit >= visibles/2)
                    gridCont.FirstDisplayedScrollingRowIndex = actualEdit - visibles/2;
            }

        }

        private void autoSave()
        {
            string fn = mW.autosaveDir+"\\"+ mW.openFile.Substring(mW.openFile.LastIndexOf('\\')+1) + ".TranslateW.AUTOSAVE";

            toolStripStatusLabel2.Text = "Guardando " + fn + "...";
            //mW.SaveFile(fn);
            ZipSubtitleScript zips = new ZipSubtitleScript(mW.script);
            zips.SaveToZip(fn);

            toolStripStatusLabel2.Text = "[" + System.DateTime.Now.ToString() + "] " + fn + " guardado.";
        }

        void timer2_Tick(object sender, EventArgs e)
        {
            long current;
            current = VideoUnitConversion.getCurPos(mediaSeeking, videoInfo.FrameRate); //mediaSeeking.GetCurrentPosition(out current);

            if (current > stopTime)
            {
                mediaControl.Stop();
                timer2.Enabled = false;
            }
        }

        void timer3_Tick(object sender, EventArgs e)
        {
            autoSave();
        }


        private string doParse(string orig)
        {
            string temp = orig; // código más viejo que la tos, si usase regex sería mucho mejor... :P
            if (checkTagSSA.Checked)
            {

                do
                {
                    int idxOpn = temp.IndexOf(@"{\");
                    int idxCls = temp.IndexOf('}');
                    if ((idxOpn > idxCls)||(idxOpn==-1)) break;
                    temp = temp.Remove(idxOpn, (idxCls - idxOpn)+1);
                } while (temp.IndexOf(@"{\") != -1);

            }

            if (checkComment.Checked)
            {
                do
                {
                    int idxOpn = temp.IndexOf('{');
                    int idxCls = temp.IndexOf('}');
                    if ((idxOpn > idxCls) || (idxOpn == -1)) break;
                    temp = temp.Remove(idxOpn, (idxCls - idxOpn) + 1);
                } while (temp.IndexOf('{') != -1);

            }

            return temp;
        }

        void textTradu_KeyDown(object sender, KeyEventArgs e)
        {
            PulsacionesTotales++;
            int cosa = 0;
            switch (e.KeyCode)
            {
                    // posiciones

                case Keys.NumPad1:
                    if (e.Control)
                        addTagSSA("an1", "");
                    break;

                case Keys.NumPad2:
                    if (e.Control)
                        addTagSSA("an2", "");
                    break;

                case Keys.NumPad3:
                    if (e.Control)
                        addTagSSA("an3", "");
                    break;

                case Keys.NumPad4:
                    if (e.Control)
                        addTagSSA("an4", "");
                    break;

                case Keys.NumPad5:
                    if (e.Control)
                        addTagSSA("an5", "");
                    break;

                case Keys.NumPad6:
                    if (e.Control)
                        addTagSSA("an6", "");
                    break;

                case Keys.NumPad7:
                    if (e.Control)
                        addTagSSA("an7", "");
                    break;

                case Keys.NumPad8:
                    if (e.Control)
                        addTagSSA("an8", "");
                    break;
                case Keys.NumPad9:
                    if (e.Control)
                        addTagSSA("an9","");
                    break;

                    // --

                case Keys.Down:
                    SendKeys.Send("");
                    e.Handled = true;
                    break;

                case Keys.S:
                    if (e.Control)
                        saveFile();
                    e.Handled = true;
                    break;
                case Keys.B:
                    if (e.Control)
                        addBold();
                    e.Handled = true;
                    break;
                case Keys.I:
                    if (e.Alt)
                        addItalic();
                    e.Handled = true;
                    break;
                case Keys.U:
                    if (e.Control)
                        addUnderline();
                    e.Handled = true;
                    break;

                case Keys.Insert:
                    int old_start = textTradu.SelectionStart;
                    textTradu.Text += doParse(textOrig.Text);
                    textTradu.Focus();
                    textTradu.SelectionStart = old_start;
                    textTradu.SelectionLength = 0;
                    e.Handled = true;
                    break;
                case Keys.End:
                    if ((textTradu.Text == "")||(textTradu.SelectionStart==textTradu.Text.Length))
                    {
                        SendKeys.Send("");
                        e.Handled = true;
                    }
                    lineaASS lass2 = al[actualEdit] as lineaASS;
                    double i = Math.Round(lass2.t_inicial.getTiempo() * videoInfo.FrameRate);
                    double f = Math.Round(lass2.t_final.getTiempo() * videoInfo.FrameRate);
                    long l_i = long.Parse(i.ToString());
                    stopTime = long.Parse(f.ToString());                    
                    VideoUnitConversion.setNewPos(mediaSeeking, l_i, videoInfo.FrameRate);
                    mediaControl.Run();
                    timer2.Enabled = true;
                    textTradu.Focus();                    
                    break;
                case Keys.PageUp:
                    SendKeys.Send("");
                    e.Handled = true;
                    if (actualEdit == 0) break;
                    actualEdit--;
                    lineaASS lass0 = (lineaASS)al[actualEdit];
                    textPersonaje.Text = lass0.personaje;
                    textOrig.Text = lass0.texto;
                    textTradu.Text = lass0.texto;                    
                    gridCont.Rows[actualEdit].Selected = true;
                    gridCont.Rows[actualEdit + 1].Selected = false;
                    actualizarScroll();
                    textTradu.Focus();
                    e.Handled = true;
                    break;
                case Keys.PageDown:
                    SendKeys.Send("");
                    e.Handled = true;
                    if (actualEdit == al.Count - 1) break;
                    actualEdit++;
                    lineaASS lass3 = (lineaASS)al[actualEdit];
                    textPersonaje.Text = lass3.personaje;
                    textOrig.Text = lass3.texto;
                    textTradu.Text = string.Empty;
                    //label1.Text = "Línea " + (actualEdit + 1) + " de " + (al.Count) + " " + Math.Truncate((double)actualEdit + 1 / (double)al.Count) + "%";
                    gridCont.Rows[actualEdit].Selected = true;
                    gridCont.Rows[actualEdit - 1].Selected = false;
                    actualizarScroll();
                    textTradu.Focus();
                    e.Handled = true;
                    break;

                case Keys.Enter:                    
                    lineaASS lass = al[actualEdit] as lineaASS;
                    if (!textTradu.Text.Equals(string.Empty))
                    {
                        string tradu = textTradu.Text;
                        if (tradu.StartsWith(@"//"))
                        {
                            lass.clase = (lass.clase.Equals("Dialogue")) ? "Comment" : "Dialogue";
                            tradu = tradu.Substring(2);
                        }
                        if (!modeSelector.Checked || splitText.Checked)
                            if (tradu.IndexOf(':') != -1)
                            {
                                string[] pjtexto = tradu.Split(new char[] { ':' }, 2);
                                string pj = pjtexto[0].Trim();
                                tradu = pjtexto[1].Trim();
                                gridCont[0, actualEdit].Value = pj;
                                lass.personaje = pj;
                            }
                            else
                            {
                                if (actualEdit > 0)
                                {                                    
                                    lineaASS anterior = (lineaASS)al[actualEdit - 1];
                                    string pj = (textPersonaje.Text.Equals("")) ? anterior.personaje : textPersonaje.Text;                                    
                                    lass.personaje = pj;
                                    gridCont[0, actualEdit].Value = pj;
                                }
                            }
                        gridCont[1, actualEdit].Value = tradu;
                        lass.texto = tradu;

                    }
                    else if ((actualEdit > 0) && (!modeSelector.Checked || splitText.Checked)) // propagar pj
                    {
                        lineaASS anterior = (lineaASS)al[actualEdit - 1];
                        string pj = (textPersonaje.Text.Equals("")) ? anterior.personaje : textPersonaje.Text;
                        lass.personaje = pj;
                        gridCont[0, actualEdit].Value = pj;
                        gridCont[1, actualEdit].Value = lass.texto;
                        //lass.texto = anterior.texto;
                    }
                    textTradu.Text = String.Empty;
                    button8.Focus();
                    e.Handled = true;
                    if (actualEdit == al.Count - 1)
                    {
                        if (!modeSelector.Checked)
                        {
                            gridCont.RowCount++;
                            al.Add(new lineaASS());
                            gridCont[0, actualEdit + 1].Value = string.Empty;
                            gridCont[1, actualEdit + 1].Value = string.Empty;                            
                        } else break;
                    }
                    actualEdit++;
                    textPersonaje.Text = gridCont[0, actualEdit].Value.ToString();
                    textOrig.Text = gridCont[1, actualEdit].Value.ToString();

                    if (!modeSelector.Checked) 
                        PreTiming(true);

                    //label1.Text = "Línea " + (actualEdit + 1) + " de " + (al.Count) + " " + Math.Truncate((double)actualEdit + 1 / (double)al.Count) + "%";
                    gridCont.Rows[actualEdit - 1].Selected = false;
                    gridCont.Rows[actualEdit].Selected = true;
                    actualizarScroll();
                    IsModified = true;
                    e.Handled = true;
                    break;
                default: break;
            }
        }


        void textPersonaje_TextChanged(object sender, EventArgs e)
        {
            gridCont[0, actualEdit].Value = textPersonaje.Text;
            IsModified = true;
        }

        private void UpdateLineaN(int idx)
        {
            if (idx < 0) return;
            actualEdit = idx;
            lineaASS lass = al[idx] as lineaASS;
            //label1.Text = "Línea " + (idx + 1) + " de " + (al.Count) + " " + Math.Truncate((double)idx + 1 / (double)al.Count)) + "%"; ;
            if (mediaControl == null) return;

            if (modeSelector.Checked)
            {
                // double d = Math.Round(lass.t_inicial.getTiempo() * videoInfo.FrameRate);
                double d = Math.Round(lass.t_inicial.getTiempo() * videoInfo.FrameRate);
                long l = long.Parse(d.ToString());
                VideoUnitConversion.setNewPos(mediaSeeking, l, videoInfo.FrameRate); //mediaSeeking.SetPositions(l, AMSeekingSeekingFlags.AbsolutePositioning, null, AMSeekingSeekingFlags.NoPositioning);
            }
            else
            {
                tiempoInicio_.Text = lass.t_inicial.ToString();
                tiempoFin_.Text = lass.t_final.ToString();
            }

            textOrig.BackColor = (lass.clase.Equals("Dialogue",StringComparison.InvariantCultureIgnoreCase)) ? Color.LightGray : Color.SeaGreen;

            textOrig.Text = lass.texto;
            textTradu.Text = String.Empty;
            textTradu.CleanUndoRedoInfo();
            textPersonaje.Text = lass.personaje;
            UpdateStatusFile();
            openTags = new Stack<Tag>();
            // TODO: Analizar lo ya escrito
        }

        void gridCont_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                return;
            }
            UpdateLineaN(e.RowIndex);
        }

        void timer1_Tick(object sender, EventArgs e)
        {
            long cur, total;
            double perc = 0;
            cur = VideoUnitConversion.getCurPos(mediaSeeking, videoInfo.FrameRate); //mediaSeeking.GetCurrentPosition(out cur);
            total = VideoUnitConversion.getTotal(mediaSeeking, videoInfo.FrameRate); //mediaSeeking.GetDuration(out total);

            if (cur > 0) perc = ((cur * 100) / total);
            progressBar1.Value = Convert.ToInt32(perc);
            var PosActInicio = (double) (VideoUnitConversion.getCurPos(mediaSeeking, videoInfo.FrameRate)/videoInfo.FrameRate);
            var PosActTotal = (double) VideoUnitConversion.getTotal(mediaSeeking, videoInfo.FrameRate)/videoInfo.FrameRate;
            PosicionActual = Tiempo.SecondToTimeString(PosActInicio) + "/" + Tiempo.SecondToTimeString(PosActTotal);
            progressBar1.ToolTipText = "Tiempo: " + PosicionActual;
            UpdateCaption();

        }

        private void saveFile()
        {
            updateMainW();
            mW.updateGridWithArrayList(al);
            mW.saveFile(mW.openFile);
            toolStripStatusLabel2.Text = "Archivo grabado correctamente a " + mW.openFile;
        }


        #region ESTILOS DE SUBS

        private void addTagSSA(string tag_open, string tag_close)
        {

            int sel_start = textTradu.SelectionStart;
            int sel_length = textTradu.SelectionLength;
            bool closes = false;

            if (!tag_close.Equals(String.Empty))
            {
                if (sel_length > 0)
                {
                    textTradu.Text = lineaASS.insertTag(textTradu.Text, tag_close, sel_start + sel_length);
                    closes = true;
                }
            }

            if (!closes && openTags.Count > 0)
            {
               Tag last = (Tag)openTags.Peek();
                // si se vuelve a abrir un tag que ya estaba abierto, lo que ponemos es un tag de cerrado
                if (last.Open.Equals(tag_open, StringComparison.InvariantCultureIgnoreCase))
                {
                    openTags.Pop();
                    int lenlen2 = textTradu.Text.Length;
                    textTradu.Text = lineaASS.insertTag(textTradu.Text, last.Close, sel_start);
                    lenlen2 = textTradu.Text.Length - lenlen2;

                    textTradu.SelectionStart = sel_start + lenlen2;

                    return;
                }

            }

            int lenlen = textTradu.Text.Length;
            textTradu.Text = lineaASS.insertTag(textTradu.Text, tag_open, sel_start);
            lenlen = textTradu.Text.Length - lenlen;

            textTradu.SelectionStart = sel_start + lenlen;

            if (!closes && !String.IsNullOrEmpty(tag_close))
            {
                openTags.Push(new Tag(tag_open, tag_close));
            }

        }

        private void addItalic()
        {

            addTagSSA("i1", "i0");
        }

        private void addBold()
        {
            addTagSSA("b1", "b0");
        }

        private void addUnderline()
        {
            addTagSSA("u1", "u0");

        }

        private void addFont()
        {
            FontDialog fd = new FontDialog();
            fd.ShowColor = false;
            fd.ShowEffects = false;

            if (fd.ShowDialog() == DialogResult.OK)
            {
                addTagSSA("fn" + fd.Font.Name, "");
            }
        }
        #endregion

        #region AUTOCOMPLETE
        private void textBox1_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (!checkAutoComplete.Checked)
            {
                listBox1.Visible = false;
                return;
            }

            switch (e.KeyValue)
            {
                case (40): // DOWN 
                    if (listBox1.Visible)
                    {
                        listBox1.Focus();
                        listBox1.SetSelected(0, true);
                    }                    
                    break;

                case (27): // ESCAPE 
                    if (listBox1.Visible)
                    {
                        listBox1.Visible = false;
                        textTradu.Focus();
                    }
                    break;

                case (13): // ENTER 
                    listBox1.Visible = false;
                    //button8.Focus();                    
                    break;

                case (32): // SPACE 
                    listBox1.Visible = false;
                    break;

                default:
                    string LastWord = GetLastWord();
                    if (LastWord != "")
                    {
                        listBox1.Items.Clear();
                        listBox1.Visible = fillListView(LastWord);
                    }
                    else
                    {
                        listBox1.Visible = false;
                    }
                    break;
            }
        }

        private void listBox1_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            switch (e.KeyValue)
            {
                case (27): // ESCAPE 
                    listBox1.Visible = false;
                    textTradu.Focus();
                    break;
                case (13): // ENTER 
                    ReplaceCurrentWord();
                    textTradu.Focus();
                    break;
                default:
                    break;
            }
        }

        private void ReplaceCurrentWord()
        {
            // int IndexCurrentLine = textTradu.GetLineFromCharIndex(textTradu.SelectionStart);
            string line = textTradu.Text;
            string LastWord = GetLastWord();
            string NewWord = listBox1.Text;
            string LettersToAdd = NewWord.Substring(LastWord.Length);
            listBox1.Visible = false;
            textTradu.Select(textTradu.Text.Length, 0);
            textTradu.AppendText(LettersToAdd);
        }


        private string GetLastWord()
        {
            string LastWord = "";
            Point pt;
            int IndexCurrentCol, index;
            index = textTradu.SelectionStart;            
            pt = textTradu.GetPositionFromCharIndex(index);
            pt.X = 0;
            IndexCurrentCol = index - textTradu.GetCharIndexFromPosition(pt);

            string line = textTradu.Text; //textTradu.Lines[IndexCurrentLine].ToString();
            int index_last_space;
            string AfterCursor;

            if ((IndexCurrentCol > 0) && (line != ""))
            {
                string line2= line.Replace("¡"," ").Replace("¿"," ");
                index_last_space = line2.LastIndexOf(" ");

                if (IndexCurrentCol < index_last_space) // <------ ?!?!?!!?
                {
                    AfterCursor = line.Substring(IndexCurrentCol);
                    if (AfterCursor.StartsWith(" "))
                    {
                        string subline = line.Substring(0, IndexCurrentCol);
                        int index_last_space_subline = subline.LastIndexOf(" ");
                        if (index_last_space_subline < 0)
                        {
                            LastWord = subline;
                        }
                        else
                        {
                            LastWord = subline.Substring(index_last_space_subline + 1);
                        }
                    }
                    else
                    {
                        LastWord = "";
                    }
                }
                else
                {
                    if (index_last_space >= 0) // aqui he añadido un igual, por si peta saber que quitar :D
                    {
                        LastWord = line.Substring(index_last_space + 1);
                    }
                    else
                    {
                        LastWord = line;
                    }
                }
            }
            return LastWord;
        }


        public bool fillListView(string CurrentWord)
        {
            foreach (string s in autoComplete)
            {
                if (s.ToLower().StartsWith(CurrentWord.ToLower())) listBox1.Items.Add(s);
            }

            bool status = false;
            if (listBox1.Items.Count != 0)
            {
                status = true;
                Point pt;
                int index;
                index = textTradu.SelectionStart;
                pt = textTradu.GetPositionFromCharIndex(index);
                pt.X += textTradu.Location.X + Convert.ToInt16(textTradu.Font.Size * 0.5);
                pt.Y += textTradu.Location.Y + (int)textTradu.Font.Size * 2;
                listBox1.Location = pt;
                listBox1.BringToFront();
            }
            return status;
        }

        #endregion

        #region OPCIONES

        private void InsertAutoCompleteFromScript()
        {
            string autoc = mW.script.GetHeader().GetHeaderValue("AutoComplete");
            if (autoc== string.Empty) return;
            string[] buh = autoc.Split('|');
            for (int i = 0; i < buh.Length; i++)
            {
                if (!autoComplete.Contains(buh[i]))
                    autoComplete.Add(buh[i]);
            }
        }

        private void updateAutoCompleteList()
        {
            listBox2.DataSource = null;
            listBox2.DataSource = autoComplete;
            UpdateScriptAutoComplete();
        }

        void tabPage6_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            updateAutoCompleteList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textToAdd.Visible = true;
            textToAdd.Focus();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            int del = listBox2.SelectedIndex;
            if (del == -1) return;
            autoComplete.RemoveAt(del);
            //listBox2.Items.RemoveAt(del);
            updateAutoCompleteList();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            try
            {
                openFileDialog1.InitialDirectory = mW.getFromConfigFile("mainW_WorkDirectory");
            }
            catch
            {
                openFileDialog1.InitialDirectory = System.Environment.SpecialFolder.MyDocuments.ToString();
            }
            openFileDialog1.Filter = "Archivos de texto (*.txt)|*.txt";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamReader sr = FileAccessWrapper.OpenTextFile(openFileDialog1.FileName);
                String linea;
                while ((linea = sr.ReadLine()) != null)
                {
                    if (!autoComplete.Contains(linea))
                        autoComplete.Add(linea);
                }
                sr.Close();
                updateAutoCompleteList();
            }

        }
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkVideo.Checked) videoPanel.Visible = true;
            else videoPanel.Visible = false;
        }

        private void checkAudio_CheckedChanged(object sender, EventArgs e)
        {

            if (checkAudio.Checked)
                basicAudio.put_Volume(VolumeFull);
            else
                basicAudio.put_Volume(VolumeSilence);
        }

        private void checkTagSSA_CheckedChanged(object sender, EventArgs e)
        {
            if (checkTagSSA.Checked)
                checkComment.Enabled = true;
            else
            {
                checkComment.Enabled = false;
                checkComment.Checked = false;
            }
        }

        #endregion

        private void button13_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            autoComplete.Clear();
            updateAutoCompleteList();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            timer2.Enabled = false;
            mediaControl.Run();

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            timer2.Enabled = false;
            mediaControl.Pause();

        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            timer2.Enabled = false;
            mediaControl.Stop();

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            saveFile();
            IsModified = false;
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            //mW.Enabled = true;
            updateMainW();

            mW.updateGridWithArrayList(al);
            mW.openVid(videoInfo.FileName, videoInfo.FrameIndex);
            IsModified = false;
            //updateMainW();
            //mW.updateGridWithArrayList(al);
            closeAll();

            mW.drawPositions();

            mW.updateReplaceConfigFile("translateW_autoC", checkAutoComplete.Checked.ToString());
            mW.updateReplaceConfigFile("translateW_tagSSA", checkTagSSA.Checked.ToString());
            mW.updateReplaceConfigFile("translateW_Comment", checkComment.Checked.ToString());
            mW.updateReplaceConfigFile("translateW_aud", checkVideo.Checked.ToString());
            mW.updateReplaceConfigFile("translateW_vid", checkAudio.Checked.ToString());
            this.Dispose();

        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            audioMode.Checked = !audioMode.Checked;
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            addBold();
            textTradu.Focus();

        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            addItalic();
            textTradu.Focus();


        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            addUnderline();
            textTradu.Focus();
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            addFont();
        }

        private void UpdateConfig()
        {
            string res = "";
            foreach (string s in Diccionarios)
                res += s + ",";
            if (res.Length>0)
                res = res.Substring(0, res.Length - 1);
            mW.updateReplaceConfigFile("translateW_Reference", res);

        }
        
        private void button3_Click(object sender, EventArgs e)
        {
            if (dicName.Text == "" || dicUrl.Text == "")
            {
                mW.errorMsg("Debes poner un nombre de diccionario y su web");
                return;
            }
            diccionarios.DataSource = null;
            Diccionarios.Add(dicName.Text + "|" + dicUrl.Text);
            diccionarios.DataSource = Diccionarios;
            UpdateConfig();

            CreateReferenceTabs();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (diccionarios.SelectedIndex == -1) return;
            Diccionarios.RemoveAt(diccionarios.SelectedIndex);
            
            diccionarios.DataSource = null;                        
            diccionarios.DataSource = Diccionarios;
            UpdateConfig();

            CreateReferenceTabs();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Diccionarios.Clear();
            Diccionarios.Add("WordReference|http://www.wordreference.com/");
            Diccionarios.Add("Wikipedia|http://es.wikipedia.org");
            Diccionarios.Add("RAE|http://www.rae.es");
            Diccionarios.Add("Dictionary|http://dictionary.reference.com/");
            diccionarios.DataSource = null;
            diccionarios.DataSource = Diccionarios;
            UpdateConfig();

            CreateReferenceTabs();

        }

        private void CreateReferenceTabs()
        {

            for (int i = tabControl1.Controls.Count-1; i > 3 ; i--) // 0 contexto 1 opciones 2 visor 3 diccionario
                tabControl1.Controls.RemoveAt(i);

            int idx = 0;
            foreach (string str in Diccionarios)
            {
                string[] partes = str.Split('|');
                string name = partes[0];
                string url = partes[1];

                TabPage new_tab = new TabPage("tab"+idx);
                new_tab.Text = name;

                WebBrowser wb = new WebBrowser();
                wb.Dock = System.Windows.Forms.DockStyle.Fill;
                wb.Location = new System.Drawing.Point(0, 0);
                wb.MinimumSize = new System.Drawing.Size(20, 20);
                wb.Name = "webBrowser"+idx;
                wb.Size = new System.Drawing.Size(984, 382);
                wb.TabIndex = 2+idx;
                //wb.Url = new System.Uri(url, System.UriKind.Absolute);
                wb.Url = new System.Uri("about:blank", System.UriKind.Absolute);
                wb.ScriptErrorsSuppressed = true;
                //wb.
                wb.Parent = new_tab;
                new_tab.Controls.Add(wb);
                
                tabControl1.Controls.Add(new_tab);
                
                translateW_T tW_T = new translateW_T(url, wb, this);
                Thread t = new Thread(new ThreadStart(tW_T.Run));
                t.Priority = ThreadPriority.Lowest;
                t.Start();

                idx++;
            }
        }

        private void UpdateCaption()
        {
            string titulo = "[" + PosicionActual + "] Asistente de Traducción ";
            titulo += (modeSelector.Checked) ?
                "[Desde Script]" :
                "[Desde Cero]";
            titulo += (splitText.Checked) ?
                "[Separar Personajes]" :
                "[Conservar Personajes]";
            titulo += (audioMode.Checked) ?
                "[Desde Audio]" :
                "[Desde Vídeo]";
            this.Text = titulo;
        }

        private void modeSelector_Click(object sender, EventArgs e)
        {
            panelTiempos.Visible = groupBox6.Enabled = !modeSelector.Checked;
            splitText.Enabled = modeSelector.Checked;

            textOrig.ForeColor = Color.Black;
            textOrig.BackColor = (modeSelector.Checked) ?
                Color.LightGray :
                Color.DarkGray;

            UpdateCaption();

            toolStripStatusLabel2.Text = (modeSelector.Checked) ?
                "Activado Modo Clásico : Traducción sobre Script" :
                "Activado Modo PreTiming : Traducción desde cero con Pre-Sincronización";
            UpdateStatusFile();
        }

        private void PreTiming(bool isStart)
        {
            Tiempo actual = new Tiempo();
            int nframe = VideoUnitConversion.getCurPos(mediaSeeking, videoInfo.FrameRate);
            double s = ((double)nframe / videoInfo.FrameRate);
            actual.setTiempo(s);
            lineaASS lass = (lineaASS)al[actualEdit];

            if (isStart)
            {
                tiempoInicio_.Text = Tiempo.SecondToTimeString(s);
                lass.t_inicial.setTiempo(s);
                tiempoFin_.Text = Tiempo.SecondToTimeString(s+2.00);
                lass.t_final.setTiempo(s+2.00);
            }
            else
            {
                tiempoFin_.Text = Tiempo.SecondToTimeString(s);
                lass.t_final.setTiempo(s);
            }

        }

        private void UpdateScriptAutoComplete()
        {
            if (autoComplete.Count==0) return;

            string lista = "";
            foreach (string s in autoComplete)
            {
                lista += s + "|";
            }
            
            lista = lista.Substring(0, lista.Length - 1);
            mW.script.GetHeader().SetHeaderValue("AutoComplete", lista);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            try
            {
                openFileDialog1.InitialDirectory = mW.getFromConfigFile("mainW_WorkDirectory");
            }
            catch
            {
                openFileDialog1.InitialDirectory = System.Environment.SpecialFolder.MyDocuments.ToString();
            } 
            
            openFileDialog1.Filter = "Archivo de Subtítulos (*.ass)|*.ass";
            //openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    SubtitleScript s = new SubtitleScript(openFileDialog1.FileName);
                    string l = "";
                    if ((l = s.GetHeader().GetHeaderValue("AutoComplete")) != string.Empty)
                    {
                        string[] perry = l.Split('|');
                        for (int i = 0; i < perry.Length; i++)
                        {
                            if (!autoComplete.Contains(perry[i]))
                                autoComplete.Add(perry[i]);
                        }
                        updateAutoCompleteList();
                    }
                }
                catch { }
                
            }

        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            string seleccion = textTradu.SelectedText.Trim();
            if (seleccion.Length < 1) return;

            if (!autoComplete.Contains(seleccion))
            {
                autoComplete.Add(seleccion);
                updateAutoCompleteList();
                toolStripStatusLabel2.Text = "Añadido '" + seleccion + "' a la lista de AutoCompletar.";
            }
        }

        private void UpdateStatusFile()
        {
            try
            {
                TextWriter o = new StreamWriter(StatusFile, false, System.Text.Encoding.UTF8);
                FileInfo f = new FileInfo(videoInfo.FileName);
                o.WriteLine("Video: " + f.Name);
                f = new FileInfo(mW.openFile);
                o.WriteLine("Script: " + f.Name);
                o.WriteLine("Info: " + labelLineaActual.Text);
                o.WriteLine("Mode: " + ((modeSelector.Checked) ? "Classic" : "Pretiming"));
                o.WriteLine("Frames: " + VideoUnitConversion.getCurPos(mediaSeeking, videoInfo.FrameRate) + "/" + VideoUnitConversion.getTotal(mediaSeeking, videoInfo.FrameRate));
                o.WriteLine("FPS: " + videoInfo.FrameRate);
                o.WriteLine("Tiempo: " + Tiempo.SecondToTimeString((double)VideoUnitConversion.getCurPos(mediaSeeking, videoInfo.FrameRate) / videoInfo.FrameRate) + "/" + Tiempo.SecondToTimeString((double)VideoUnitConversion.getTotal(mediaSeeking, videoInfo.FrameRate) / videoInfo.FrameRate));
                o.WriteLine("Version: " + mainW.appTitle);
                o.WriteLine("TiempoActivo: " + tiempoActivo.Text);
                o.WriteLine("PPM: " + PPM.Text);
                o.Close();
            }
            catch
            {
                toolStripStatusLabel2.Text = "Error guardando archivo de estado.";
            }
        }

        private void checkSaveTime_CheckedChanged(object sender, EventArgs e)
        {
            mW.updateReplaceConfigFile("translateW_preTime", checkSaveTime.Checked.ToString());
        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            findOthersW fow = new findOthersW(this.mW);
            fow.Show();
        }

        // añadir archivo
        private void button24_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            try
            {
                openFileDialog1.InitialDirectory = mW.getFromConfigFile("mainW_WorkDirectory");
            }
            catch
            {
                openFileDialog1.InitialDirectory = System.Environment.SpecialFolder.MyDocuments.ToString();
            }
            openFileDialog1.Filter = "Archivos de subtítulos conocidos (*.ass; *.ssa; *.srt; *.txt)|*.ass; *.ssa; *.srt; *.txt";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                archivosView.Items.Add(openFileDialog1.FileName, 0);
            }
        }
        // añadir carpeta
        private void button25_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            
            try
            {
                fbd.SelectedPath = mW.getFromConfigFile("mainW_WorkDirectory");
            }
            catch
            {
                fbd.SelectedPath = System.Environment.SpecialFolder.MyDocuments.ToString();
            }
            
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                if (checkRecursiveAdd.Checked)
                {
                    BusquedaNormal(fbd.SelectedPath); // mirar tb raiz
                    BusquedaRecursiva(fbd.SelectedPath);
                }
                else
                    BusquedaNormal(fbd.SelectedPath);

            }
        }

        // copiados de findOthersW
        private ArrayList BuscarExtensiones(string dir)
        {
            ArrayList tmp = new ArrayList();
            tmp.AddRange(Directory.GetFiles(dir, "*.ass"));
            tmp.AddRange(Directory.GetFiles(dir, "*.ssa"));
            tmp.AddRange(Directory.GetFiles(dir, "*.srt"));
            tmp.AddRange(Directory.GetFiles(dir, "*.txt"));
            return tmp;
        }

        private void BusquedaNormal(string dir)
        {
            foreach (string f in BuscarExtensiones(dir))
            {
                // distinguir icono/tipo de archivo
                int tipo = 0;
                if (f.EndsWith("txt", StringComparison.InvariantCultureIgnoreCase))
                    tipo = 3;
                else if (f.EndsWith("srt", StringComparison.InvariantCultureIgnoreCase))
                    tipo = 1;
                else if (f.EndsWith("ssa", StringComparison.InvariantCultureIgnoreCase))
                    tipo = 2;

                archivosView.Items.Add(f, tipo);
            }
        }


        private void BusquedaRecursiva(string dir)
        {
            try
            {
                foreach (string d in Directory.GetDirectories(dir))
                {
                    BusquedaNormal(d);
                    BusquedaRecursiva(d);
                }
            }
            catch { }
        }

        void archivosView_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    for (int i = archivosView.SelectedItems.Count - 1; i > -1; i--)
                    {
                        archivosView.Items.RemoveAt(archivosView.SelectedIndices[i]);
                    }
                    break;
            }
        }


        void archivosView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (archivosView.SelectedIndices.Count != 1)
                return;

            SubtitleScript script = new SubtitleScript(archivosView.SelectedItems[0].Text);

            string archivo = "";
            foreach (lineaASS linea in script.GetLineArrayList().GetFullArrayList())
            {
                archivo += ((linea.personaje!="")? linea.personaje + ": " : "")+ linea.texto;
                archivo += "\n";
            }

            richTextBox1.Text = archivo;
            tabControl1.TabPages["tabVisor"].Text = "Visor ("+archivosView.SelectedItems[0].Text+")";
        }

        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            // + volumen
            if (basicAudio != null)
            {
                int bleh;
                basicAudio.get_Volume(out bleh);
                basicAudio.put_Volume(bleh + 500);
            }
        }

        private void toolStripButton14_Click(object sender, EventArgs e)
        {
            // - volumen
            if (basicAudio != null)
            {
                int bleh;
                basicAudio.get_Volume(out bleh);
                basicAudio.put_Volume(bleh - 500);
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            if (diccionarios.SelectedIndex <1) return;

            int idx = diccionarios.SelectedIndex;

            string dastring = Diccionarios[idx].ToString();

            Diccionarios.RemoveAt(idx);
            Diccionarios.Insert(idx - 1, dastring);

            diccionarios.DataSource = null;
            diccionarios.DataSource = Diccionarios;
            UpdateConfig();

            CreateReferenceTabs();

            diccionarios.SelectedIndex = idx - 1;

        }

        private void button21_Click(object sender, EventArgs e)
        {
            if (diccionarios.SelectedIndex ==-1 || diccionarios.SelectedIndex==diccionarios.Items.Count-1) return;

            int idx = diccionarios.SelectedIndex;

            string dastring = Diccionarios[idx].ToString();

            Diccionarios.RemoveAt(idx);
            Diccionarios.Insert(idx + 1, dastring);

            diccionarios.DataSource = null;
            diccionarios.DataSource = Diccionarios;
            UpdateConfig();

            CreateReferenceTabs();

            diccionarios.SelectedIndex = idx + 1;
        }

        private void button22_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex < 1) return;

            int idx = listBox2.SelectedIndex;

            string dastring = autoComplete[idx].ToString();

            autoComplete.RemoveAt(idx);
            autoComplete.Insert(idx - 1, dastring);

            listBox2.DataSource = null;
            listBox2.DataSource = autoComplete;
            UpdateConfig();

            listBox2.SelectedIndex = idx - 1;
        }

        private void button23_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex == -1 || listBox2.SelectedIndex == listBox2.Items.Count - 1) return;

            int idx = listBox2.SelectedIndex;

            string dastring = autoComplete[idx].ToString();

            autoComplete.RemoveAt(idx);
            autoComplete.Insert(idx + 1, dastring);
            
            listBox2.DataSource = null;
            listBox2.DataSource = autoComplete;
            UpdateConfig();

            listBox2.SelectedIndex = idx + 1;
        }

        private void InsertNewLine(int mode)
        {
            int selected = gridCont.SelectedRows[0].Index;
            gridCont.Rows.Insert(selected + mode, new DataGridViewRow());
            gridCont[0, selected + mode].Value = string.Empty; // rellenamos para q no pete
            gridCont[1, selected + mode].Value = string.Empty;
            mW.InsertNewLine(mode, selected);
        }

        private void antesDeLaFilaSeleccionadaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertNewLine(0);
        }

        private void despuésDeLaFilaSeleccionadaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertNewLine(1);
        }

        private void borrarLíneaDeDiálogoToolStripMenuItem_Click(object sender, EventArgs e)
        {

            List<int> brau = new List<int>();
            for (int i = 0; i < gridCont.SelectedRows.Count; i++)
                brau.Add(gridCont.SelectedRows[i].Index);
            brau.Sort();
            brau.Reverse();

            foreach(int i in brau)
            {
                gridCont.Rows.RemoveAt(i);
                mW.DeleteSelectedLine(i);
            }
        }

        // resize de la ventana de vídeo
        private void videoPanel_SizeChanged(object sender, EventArgs e)
        {
            int x, y; 
            basicVideo.GetVideoSize(out x, out y);

            int new_x = videoPanel.Width;
            int new_y = (new_x * y) / x;
            videoWindow.put_Height(new_x);
            videoWindow.put_Width(new_y);
            videoWindow.put_Owner(videoPanel.Handle);
            videoPanel.Size = new System.Drawing.Size(new_x, new_y);
            videoWindow.SetWindowPosition(0, 0, videoPanel.Width, videoPanel.Height);
            videoWindow.put_WindowStyle(WindowStyle.Child);
            videoWindow.put_Visible(DirectShowLib.OABool.True);
        }

        // clase helper para guardar los tags
        internal class Tag
        {
            private String open;

            public String Open
            {
                get { return open; }
                set { open = value; }
            }
            private String close;

            public String Close
            {
                get { return close; }
                set { close = value; }
            }

            private int start;

            public int Start
            {
                get { return start; }
                set { start = value; }
            }

            internal Tag(String open, String close)
            {
                this.open = open;
                this.close = close;
            }
        }

    }
}