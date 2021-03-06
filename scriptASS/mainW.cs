using System;
using System.Collections;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Windows;
using System.Runtime.InteropServices;
using DirectShowLib;
using System.Timers;
using System.Threading;
using Microsoft.DirectX.DirectSound;
using System.Text.RegularExpressions;
using System.Net;
using System.Diagnostics;
using System.Reflection;
using System.Globalization;
using System.Drawing.Imaging;
using DxScanScenes;
using MediaInfoWrapper;
using System.Security.Principal;
using FFMS2;
using BackgroundWorker;
using System.Collections.Generic;
using scriptASS.Misc;

namespace scriptASS
{

    #region ENUMS

    public enum ShiftStyle : int
    {
        Normal = 0,
        Selected = 1,
        BeforeSelected = 2,
        AfterSelected = 3
    }

    public enum ReproductionState : int
    {
        Play = 0,
        Stop = 1,
        Pause = 2
    }
    public enum WaveType : int
    {
        Normal = 0,
        FFT = 4,
        FFTWave = 5
    }

    public enum PlayOptions : int
    {
        Normal = 0,
        BeforeStart = 1,
        AfterStart = 2,
        BeforeEnd = 3,
        AfterEnd = 4
    }

    public enum PlayMode : int
    {
        Full = 0,
        Selection = 1,
        Other = 2
    }
    public enum ScriptFileType : int
    {
        ASS_SSA = 0,
        TXT = 1,
        SRT = 2,
        Unknown = 3
    }

    public enum PreviewType : int
    {
        DirectShow = 0,
        AviSynth = 1
    };

    public enum VideoSourceType
    {
        Import,
        FFVideoSource,
        DSS2,
        AviSource,
        DirectShowSource,
        MPEG2Source
    }

    public enum AudioSourceType
    {
        None,
        WavSource,
        Import,
        FFAudioSource,
        DirectShowSource,                
        AviSource
    }

    public enum CopyStyle : int
    {
        Normal = 0,
        TInicial = 1,
        TFinal = 2,
        Tiempo = 3,
        Estilo = 4,
        Personaje = 5,
        EstiloPersonaje = 6,
        SinTexto = 7,
        SoloTexto = 8
    }
    #endregion

    public partial class mainW : Form
    {

        #region DEFINICION DE VARIABLES

        public static string appTitle = "PerrySub " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        private static string changelogUrl = Path.Combine(Application.StartupPath, "changelog.txt");
        private static string documentationUrl = "http://sub.perry.es/wiki";
        private static string developblogUrl = "http://sub.perry.es";
        public static string defaultDictionaries = "rin.perry.es";

        public string openFile = null;

        public VideoInfo videoInfo = null;
        public VideoInfo oldVideoInfo = null;

        ScriptFileType type = ScriptFileType.Unknown;

        private bool InvokedFromCommandline = false;
        private bool CommandLineNewFile = false;

        private MouseButtons keboton = MouseButtons.Left;

        public UndoRedo UndoRedo = new UndoRedo();
        private Stack StatusStripEventList = new Stack();

        private const int FramesToSkip = 1;
        private const int MaxRecentItems = 10 - 1;

        private PreviewType VideoBoxType = PreviewType.DirectShow;
        private VideoSourceType OpenSourceType = VideoSourceType.AviSource;
        private AudioSourceType OpenAudioSourceType = AudioSourceType.AviSource;
        public ReproductionState VideoState = ReproductionState.Stop;

        private bool isVideoLoaded = false;
        private bool isAudioLoaded = false;
        private bool isModified = false;
        private bool clipperActive = false;
        private bool loadingDragDrop = false;

        public SubtitleScript script;

        public ArrayList al; 
        public ArrayList head; 
        public ArrayList v4;

        public Hashtable h;

        private long endSeek;

        private int mouseDownX = -1, mouseDownY = -1;
        private Bitmap ActualFrame;
        private Bitmap keyFrameBase;

        private List<string> UsedCacheFiles = new List<string>();

        BackgroundWork back;

        public bool spellEnabled = true;
        public bool isClosing = false;

        private string activeDict = "es_ES";
        public string ActiveDict
        {
            get { return activeDict; }
            set
            {
                activeDict = value;
                selectedLine.Dictionary = activeDict;
            }
        }

        public readonly string configName = Path.Combine(Application.StartupPath, "PerrySub.cfg");
        public readonly string editedFile = Path.Combine(Application.StartupPath, "PerrySub.onWork");
        public readonly string stylesDir = Path.Combine(Application.StartupPath, "Styles");
        public readonly string notesDir = Path.Combine(Application.StartupPath, "Notes");
        public readonly string autosaveDir = Path.Combine(Application.StartupPath, "AutoSave");
        public readonly string v1Dir = Path.Combine(Application.StartupPath, "v1");
        public readonly string templateDir = Path.Combine(Application.StartupPath, "Templates");
        public readonly string cacheDir = Path.Combine(Application.StartupPath, "Cache");
        public readonly string dictDir = Path.Combine(Application.StartupPath, "Dictionaries");
        public readonly string indexDir = Path.Combine(Application.StartupPath, "Indexes");

        private readonly string langes = "lang_es_ES.zip";

        private readonly string newFileName = Path.Combine(Application.StartupPath, "Nuevo archivo de PerrySub.ass");

        private string[] videoExtensions = { "avi", "mkv", "mp4", "ogm", "wmv", "avs", "avsi", "3gp", "flv", "divx", "xvid", "mov" };
        private string[] audioExtensions = { "wav", "mp3", "ogg", "mp4", "aac", "mka" };
        private string[] subtitleExtensions = { "ass", "ssa", "txt", "srt" };

        private string knownVideoExtensions = "";
        private string knownAudioExtensions = "";
        private string knownSubtitleExtensions = "";

        private string[] VideoFilterPriority;
        private string[] AudioFilterPriority;

        [DllImport("Shell32.dll")]
        static extern void SHAddToRecentDocs(int flags, string file);
        const int SHARD_PATH = 2;

        //private Configuration config;
        Config config;

        private bool keyframesAvailable = false;
        public bool KeyframesAvailable
        {
            get
            {
                return keyframesAvailable;
            }
            set
            {
                keyframesAvailable = value;
                prevK.Visible = nextK.Visible = value;
                drawKeyFrameBox();
                updateMenuEnables();
            }
        }

        #endregion

        #region Strings ASS
        public readonly string headerMark = "[Script Info]";
        public readonly string stylesMark = "[V4+ Styles]\nFormat: Name, Fontname, Fontsize, PrimaryColour, SecondaryColour, OutlineColour, BackColour, Bold, Italic, Underline, StrikeOut, ScaleX, ScaleY, Spacing, Angle, BorderStyle, Outline, Shadow, Alignment, MarginL, MarginR, MarginV, Encoding";
        public readonly string dialoguesMark = "[Events]\nFormat: Layer, Start, End, Style, Name, MarginL, MarginR, MarginV, Effect, Text";
        #endregion

        #region COLORES
        public static Color CommentFore = Color.Red;
        public static Color CommentBack = Color.AntiqueWhite;
        public static Color RowBack = Color.White;
        public static Color RowFore = Color.Black;
        #endregion

        #region Constructores

        public mainW()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-ES");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-ES");

            InitializeComponent();
            InitConfigutation();

            GenerateVideoExtensions();
            GenerateAudioExtensions();
            GenerateSubtitleExtensions();

            this.Text = appTitle + " [" + Application.CurrentCulture.Name + "]";
            updateMenuEnables();
        }

        public mainW(string[] args) : this()
        {
            InvokedFromCommandline = true;

            if (args[0].Equals("-new", StringComparison.InvariantCultureIgnoreCase))
            {
                CommandLineNewFile = true;
                return;
            }

            FileInfo f = new FileInfo(args[0]);

            switch (f.Extension.ToLower())
            {
                case ".ass":
                case ".ssa":
                case ".srt":
                case ".txt":

                    openFile = args[0];

                    chgCaption(openFile);
                    loadFileNoParms();
                    break;

                case ".avs":
                case ".avsi":

                    try
                    {
                        avsW avs0rz = new avsW(this, args[0]);
                        avs0rz.ShowDialog();
                        //avs0rz.Focus();
                        //this.Enabled = false;
                        //this.WindowState = FormWindowState.Minimized;
                    }
                    catch { errorMsg("Error cargando el archivo '" + args[0] + "'"); }

                    break;

                default:
                    errorMsg("Formato de archivo no reconocido.");
                    break;
            }

        }

        private void InitConfigutation()
        {
            config = new Config(configName);
            config.LoadConfig();
        }


        #endregion

        #region Variables Especiales xD

        private Object _framelock = new Object();
        
        public int FrameIndex
        {
            get
            {
                if (VideoBoxType == PreviewType.DirectShow)
                    return VideoUnitConversion.getCurPos(mediaSeeking, videoInfo.FrameRate);        // creo q no es necesario o_o
                return videoInfo.FrameIndex;
            }
            set
            {
                lock (_framelock)
                {

                    videoInfo.FrameIndex = value;
                    if (value >= FrameTotal) videoInfo.FrameIndex = FrameTotal - 1;

                    if (script != null)
                        script.GetHeader().SetHeaderValue("Video Position", value.ToString());

                    if (VideoBoxType == PreviewType.AviSynth)
                    {
                        seekToFrame(videoInfo.FrameIndex);
                    }
                    if (VideoBoxType == PreviewType.DirectShow)
                        VideoUnitConversion.setNewPos(mediaSeeking, videoInfo.FrameIndex + 1, videoInfo.FrameRate);
                    updateTextBoxWithFPS();
                    updateKeyFrameBox();
                }
            }
        }

        public int FrameTotal
        {
            get
            {
                /*
                if (VideoBoxType == PreviewType.DirectShow)
                    if (mediaSeeking != null)
                        return VideoUnitConversion.getTotal(mediaSeeking, videoInfo.FrameRate);
                    else
                        return -1;
                return avsClip.num_frames;
                 */
                return videoInfo.FrameTotal;
            }
        }

        public int VideoWidth
        {
            get
            {
                /*
                if (VideoBoxType == PreviewType.DirectShow)
                    if (basicVideo != null)
                    {
                        int x, y;
                        basicVideo.GetVideoSize(out x, out y);
                        return x;
                    }
                    else return -1;
                return avsClip.VideoWidth;*/
                return videoInfo.Resolution.Width;
            }
        }

        public int VideoHeight
        {
            get
            {
                /*
                if (VideoBoxType == PreviewType.DirectShow)
                    if (basicVideo != null)
                    {
                        int x, y;
                        basicVideo.GetVideoSize(out x, out y);
                        return y;
                    }
                    else return -1;
                return avsClip.VideoHeight;
                 */
                return videoInfo.Resolution.Height;
            }
        }

        public bool IsModified
        {
            get { return isModified; }
            set
            {
                isModified = value;
                if (isModified)
                    chgCaption(openFile + " * ");
                else
                    chgCaption(openFile);

            }
        }

        #endregion

        #region FUNCIONES VARIAS

        private void GenerateVideoExtensions()
        {
            knownVideoExtensions = "*."+videoExtensions[0];
            for (int i=1; i<videoExtensions.Length; i++)                
                knownVideoExtensions += "; *."+ videoExtensions[i];            
        }

        private void GenerateAudioExtensions()
        {
            knownAudioExtensions = "*."+audioExtensions[0];
            for (int i=1; i<audioExtensions.Length; i++)                
                knownAudioExtensions += "; *."+ audioExtensions[i];            
        }

        private void GenerateSubtitleExtensions()
        {
            knownSubtitleExtensions = "*." + subtitleExtensions[0];
            for (int i = 1; i < subtitleExtensions.Length; i++)
                knownSubtitleExtensions += "; *." + subtitleExtensions[i];
        }

        private void CallUndo()
        {
            if (UndoRedo.UndoLevels > 0)
            {
                script = UndoRedo.GetUndo(script); //al = (ArrayList)UndoStack.Pop();
                al = script.GetLines();
                v4 = script.GetStyles();
                head = script.GetHeaders();

                updateGridWithArrayList(al);
                updatePanelTextTools();
            }

        }

        private void CallRedo()
        {
            if (UndoRedo.RedoLevels > 0)
            {

                script = UndoRedo.GetRedo(script);
                al = script.GetLines();
                v4 = script.GetStyles();
                head = script.GetHeaders();

                updateGridWithArrayList(al);
                updatePanelTextTools();
            }
        }

        public void moveViewRows(int idx)
        {
            int lasqueseven = (gridASS.Height - gridASS.ColumnHeadersHeight) / gridASS.RowTemplate.Height;

            if (al.Count > lasqueseven / 2)
            {
                if (idx >= lasqueseven / 2)
                    gridASS.FirstDisplayedScrollingRowIndex = idx - (lasqueseven / 2);
            }

        }

        public void updateGridWithArrayList(ArrayList a)
        {
            //IsModified = true;
            updatePreviewAVS();
            al = a;

            idxListBox.Items.Clear();

            for (int i = 0; i < (script.LineCount / LineArrayList.LineArrayListMax) + 1; i++)
                idxListBox.Items.Add(i * LineArrayList.LineArrayListMax + "+");

            //idxListBox.SelectedIndex = 0;

            refreshGrid();
            editAttach.Checked = (script.HasAttachments);

            groupBox3.Visible = (script.LineCount > LineArrayList.LineArrayListMax);

        }

        private void commitChanges()
        {
            if (gridASS.SelectedRows.Count > 0)
            {
                IsModified = true;
                //gridASS.SelectedRows[0].Cells["Texto"].Value = selectedLine.Text;
                int actual = gridASS.SelectedRows[0].Index;
                UndoRedo.AddUndo(script, "Cambios en línea " + (actual + 1));
                lineaASS lass = (lineaASS)al[actual];
                lineaASS siguiente = (actual < al.Count - 1) ? (lineaASS)al[actual + 1] : null;
                updateLineaASSwithPanelTextTools(lass);

                if ((System.Windows.Forms.Control.ModifierKeys & Keys.Control) == Keys.Control)
                {

                    if (isAudioLoaded)
                    {
                        if (overwriteMode.Checked)
                        {
                            double cuantoscaben = ((double)AudioGridBox.Width / (double)AudioGrid_pxPerSecond);
                            double dondeempieza = ((double)hSliderBar.Value / 10);
                            //double dondeacaba = cuantoscaben + dondeempieza;
                            double enqueposi = audioGrid_Fin - dondeempieza;

                            AudioGrid_Inicio = AudioGrid_Fin;

                            if (cuantoscaben * 0.75 < audioGrid_Fin - dondeempieza)
                            {
                                try
                                {
                                    hSliderBar.Value = (int)(((cuantoscaben * 0.75) + dondeempieza) * 10);
                                }
                                catch (ArgumentOutOfRangeException)
                                {
                                    hSliderBar.Value = (int)(hSliderBar.Maximum - cuantoscaben);
                                }
                            }

                            AudioGrid_Fin = AudioGrid_Inicio + 2.00;

                            textInicio.Text = Tiempo.SecondToTimeString(AudioGrid_Inicio);
                            textFin.Text = Tiempo.SecondToTimeString(AudioGrid_Fin);
                        }
                        else
                        {
                            if (siguiente != null)
                            {

                                AudioGrid_Inicio = siguiente.t_inicial.getTiempo();
                                AudioGrid_Fin = siguiente.t_final.getTiempo();

                                int demas = Convert.ToInt32(Math.Floor(((double)panelAudioBox.Width / (double)AudioGrid_pxPerSecond)));

                                int x = (int)(AudioGrid_Inicio * 10) - 10;
                                int x2 = Math.Min(x, hSliderBar.Maximum - demas);

                                hSliderBar.Value = (AudioGrid_Inicio >= 1) ? x2 : 0;
                                updateAudioGrid();
                            }
                        }
                    }

                    if (actual == al.Count - 1)
                    {
                        al.Add(new lineaASS());
                    }
                    updateGridWithArrayList(al);
                    gridASS.Rows[actual + 1].Selected = true;
                    gridASS.Rows[actual].Selected = false;
                }
                int pos = selectedLine.SelectionStart;
                int len = selectedLine.SelectionLength;
                //updateLineaASSwithPanelTextTools(lass);
                updateGridWithArrayList(al);
                updateAudioGrid();
                updatePreviewAVS();
                updatePanelTextTools();
                try
                {
                    selectedLine.Select(pos, len);
                }
                catch { }
            }
        }

        public void selectRow(int idx)
        {
            gridASS.Rows[idx].Selected = true;
        }

        public void clearSelectedRows()
        {
            for (int i = 0; i < gridASS.Rows.Count; i++) gridASS.Rows[i].Selected = false;
        }

        private void updateMenuEnables()
        {
            audioToolStripMenuItem.Enabled = (openFile != null);
            //videoToolStripMenuItem.Enabled = (openFile != null);

            abrirVideoMenuItem.Enabled = (openFile != null);
            abrirVideoRecienteMenuItem.Enabled = (openFile != null);

            ediciónToolStripMenuItem.Enabled = (openFile != null);
            postProcesadoToolStripMenuItem.Enabled = (openFile != null);
            guardarCambiosToolStripMenuItem.Enabled = (openFile != null);
            guardarToolStripMenuItem.Enabled = (openFile != null);

            //textoPlanoToolStripMenuItem.Enabled = (openFile != null);
            //archivoSRTToolStripMenuItem.Enabled = (openFile != null);
            exportarToolStripMenuItem.Enabled = (openFile != null);
            cerrarScriptToolStripMenuItem.Enabled = (openFile != null);

            concatenarLíneasPróximasToolStripMenuItem.Enabled = (openFile != null);
            ajustarSubtítulosAKeyframesToolStripMenuItem.Enabled = isVideoLoaded && KeyframesAvailable;
            buscarKeyframesToolStripMenuItem.Enabled = isVideoLoaded; // && KeyframesAvailable;
            umbralDeLíneasMáximasEnPantallaToolStripMenuItem.Enabled = isVideoLoaded;

            buscarToolStripMenuItem.Enabled = (openFile != null);
            buscarSiguienteToolStripMenuItem.Enabled = buscarToolStripMenuItem.Enabled && toFind.Count > 0; // && search mode on
            reemplazarToolStripMenuItem.Enabled = (openFile != null);
            buscarEnOtrosArchivosToolStripMenuItem.Enabled = true;

            correctorOrtográficoToolStripMenuItem.Enabled = (openFile != null);

            verHistorialDeDeshacerRehacerToolStripMenuItem.Enabled = (openFile != null);

            asistenteDeTraducciónToolStripMenuItem.Enabled = isVideoLoaded && mediaControl != null;
            audioActive.Enabled = isVideoLoaded && avsClip != null;
            //calculadoraDeBitrateToolStripMenuItem.Enabled = isVideoLoaded;
            cerrarAudioToolStripMenuItem.Enabled = isAudioLoaded;
            cerrarVídeoToolStripMenuItem.Enabled = isVideoLoaded;
            previewToolStripMenuItem.Enabled = isVideoLoaded;
            tiempoFinalConElTiempoDelFrameActualToolStripMenuItem.Enabled = isVideoLoaded;
            ponerTiempoInicialConElTiempoDelFrameActualToolStripMenuItem.Enabled = isVideoLoaded;
            moverVídeoAlFinalprincipioDeLaLíneaActualToolStripMenuItem.Enabled = isVideoLoaded;
            moverVídeoAPrincipioDeLaLíneaActualToolStripMenuItem.Enabled = isVideoLoaded;
            modoKaraokeToolStripMenuItem.Enabled = isAudioLoaded;
            vídeoPlayPauseToolStripMenuItem.Enabled = isVideoLoaded;

            sincronizarDeVídeoMembSubToolStripMenuItem.Enabled = isVideoLoaded && mediaControl != null;
            membSubMarcarFinalDeLíneaToolStripMenuItem.Enabled = sincronizarDeVídeoMembSubToolStripMenuItem.Checked;
            membSubMarcarInicioDeLíneaToolStripMenuItem.Enabled = sincronizarDeVídeoMembSubToolStripMenuItem.Checked;
            membSubPlayPauseToolStripMenuItem.Enabled = sincronizarDeVídeoMembSubToolStripMenuItem.Checked;

            // menu ver

            //verVideoTools.Enabled = isVideoLoaded;
            //panelTools2.Visible = isVideoLoaded && verVideoTools.Checked;
            //verTextTools.Enabled = (openFile != null);

            // toolbar

            guardarToolStripButton.Enabled = guardarToolStripMenuItem.Enabled;

            editFind.Enabled = buscarToolStripMenuItem.Enabled;
            editFindNext.Enabled = buscarSiguienteToolStripMenuItem.Enabled;
            editReplace.Enabled = reemplazarToolStripMenuItem.Enabled;
            editFindOthers.Enabled = buscarEnOtrosArchivosToolStripMenuItem.Enabled;

            editSpellCheck.Enabled = correctorOrtográficoToolStripMenuItem.Enabled;
            editTrans.Enabled = asistenteDeTraducciónToolStripMenuItem.Enabled;
            editShift.Enabled = (openFile != null) && massShiftTimesToolStripMenuItem1.Enabled;
            editMassStyler.Enabled = (openFile != null) && massStylerToolStripMenuItem.Enabled;
            editStyles.Enabled = (openFile != null) && editarEstilosToolStripMenuItem.Enabled;
            editNotes.Enabled = (openFile != null) && gestiónDeNotasDeTraducciónToolStripMenuItem.Enabled;
            editHeaders.Enabled = (openFile != null) && editarCabeceraASSToolStripMenuItem.Enabled;
            editAttach.Enabled = (openFile != null) && adjuntarImágenesFuentesToolStripMenuItem.Enabled;
            editAdminFont.Enabled = (openFile != null) && administrarFuentesToolStripMenuItem.Enabled;

            //videoCalc.Enabled = isVideoLoaded;
            videoMemb.Enabled = sincronizarDeVídeoMembSubToolStripMenuItem.Enabled;
            videoPreviewAVS.Enabled = isVideoLoaded;

            ppConcat.Enabled = concatenarLíneasPróximasToolStripMenuItem.Enabled;
            ppMaxLines.Enabled = umbralDeLíneasMáximasEnPantallaToolStripMenuItem.Enabled;
            ppAdjustKeyF.Enabled = ajustarSubtítulosAKeyframesToolStripMenuItem.Enabled;

            ajustarLíneaALosKeyFramesMásCercanosToolStripMenuItem.Enabled = ppAdjustKeyF.Enabled;

            ppCol.Enabled = marcarColisionesDeLaLíneaSeleccionadaToolStripMenuItem.Enabled;

            toolStripTemplate.Enabled = TemplateName.Enabled = templateBtn.Enabled = (openFile != null);

            gridASS.RowsDefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
            gridASS.MultiSelect = true;

        }

        private void updateLineaASSwithPanelTextTools(lineaASS lass)
        {
            lass.texto = selectedLine.Text;
            lass.t_inicial.setTiempo(textInicio.Text);
            lass.t_final.setTiempo(textFin.Text);
            lass.personaje = textPers.Text;
            lass.estilo = textEst.Text;
            lass.vertical = int.Parse(textVert.Text);
            lass.izquierda = int.Parse(textIzq.Text);
            lass.derecha = int.Parse(textDcha.Text);
            //lass.clase = (/*checkComment.Checked*/ComentarDescomentar.BackColor.Equals(CommentBack)) ? "Comment" : "Dialogue";
            lass.colision = (int)textLayer.Value;
        }

        private ArrayList PrepareForPush(ArrayList a)
        {
            ArrayList n = new ArrayList();
            foreach (lineaASS lass in a)
                n.Add(new lineaASS(lass.ToString()));
            return n;
        }

        private void GoToVideoPosition()
        {
            try
            {

                string vpos = script.GetHeader().GetHeaderValue("Video Position");
                if (vpos != string.Empty)
                    FrameIndex = int.Parse(vpos);

            }
            catch { }
        }

        private void updateKeyFrameBox()
        {
            if (KeyframesAvailable)
            {
                Bitmap b = new Bitmap(keyFrameBase);
                keyFrameBox.Image = BitmapFunctions.GenerateKeyFrameDisplaySlider(b, FrameIndex, FrameTotal);
            }
        }

        private void updateKeyFrameBox(int cur)
        {
            if (KeyframesAvailable)
            {
                Bitmap b = new Bitmap(keyFrameBase);
                keyFrameBox.Image = BitmapFunctions.GenerateKeyFrameDisplaySlider(b, cur, FrameTotal);
            }

        }

        private void drawKeyFrameBox()
        {
            if (KeyframesAvailable)
            {
                keyFrameBox.Visible = true;
                keyFrameBase = BitmapFunctions.GenerateKeyFrameDisplay(keyFrameBox.Width, keyFrameBox.Height, (int[])videoInfo.KeyFrames.ToArray(typeof(int)), FrameTotal);
                keyFrameBox.Image = keyFrameBase;
            }
            else
                keyFrameBox.Visible = false;
        }

        public void updateTextBoxWithFPS()
        {
            if (!isVideoLoaded) return;

            labelFrameN.Visible = true;
            textCurrentFrames.Visible = true;
            textTotalFrames.Visible = true;
            label2.Visible = true;

            int cur = FrameIndex;
            int dur = FrameTotal;
            if (KeyframesAvailable)
            {
                bool a = false;
                foreach (int x in videoInfo.KeyFrames)
                    if (x == cur) { a = true; break; }
                if (a)
                    textCurrentFrames.ForeColor = Color.Red;
                else
                    textCurrentFrames.ForeColor = Color.Black;
            }


            textCurrentFrames.Text = Convert.ToString(cur);
            textTotalFrames.Text = Convert.ToString(dur);
            //labelFrameN.Text = "Frame " + Convert.ToString(cur) + " de " + Convert.ToString(dur);
            seekBar.Value = cur;

            double actual = (double)seekBar.Value / videoInfo.FrameRate;
            double total = (double)seekBar.Maximum / videoInfo.FrameRate;
            //labelTotalTime.Text = Tiempo.SecondToTimeString(actual) + " de " + Tiempo.SecondToTimeString(total);
            timeActual.Text = Tiempo.SecondToTimeString(actual);
            timeTotal.Text = Tiempo.SecondToTimeString(total);

            updateKeyFrameBox(seekBar.Value);

            double inicio = Tiempo.TimeToSecondDouble(textInicio.Text);
            double fin = Tiempo.TimeToSecondDouble(textFin.Text);

            int inF = (int)(inicio * videoInfo.FrameRate);
            int fnF = (int)(fin * videoInfo.FrameRate);

            double relIn = Math.Round(inicio - actual, 1);
            double relFn = Math.Round(fin - actual, 1);
            int relInMS = (int)(Math.Round(inicio - actual, 3) * 1000);
            int relFnMS = (int)(Math.Round(fin - actual, 3) * 1000);

            int relInF = (inF - cur) + 1;
            int relFnF = (fnF - cur) + 1;

            textTiempoRelativo.Text = String.Format("{0}s/{1}ms, {2}s/{3}ms", estiloV4.d2s(relIn), relInMS, estiloV4.d2s(relFn), relFnMS);
            textFramesRelativos.Text = relInF + "f, " + relFnF + "f";


        }

        public void UpdateStyleList()
        {
            styleList.Items.Clear();
            foreach (estiloV4 v in script.GetStyles())
                styleList.Items.Add(v.Name);

        }

        private int goNextLine()
        {
            int i = gridASS.SelectedRows[0].Index;

            for (int k = 0; k < gridASS.Rows.Count; k++)
                gridASS.Rows[i].Selected = false;

            int j = (i < gridASS.Rows.Count - 1) ? i + 1 : gridASS.Rows.Count - 1;
            gridASS.Rows[j].Selected = true;

            return j;
        }

        private int goPrevLine()
        {
            int i = gridASS.SelectedRows[0].Index;

            for (int k = 0; k < gridASS.Rows.Count; k++)
                gridASS.Rows[i].Selected = false;

            int j = (i > 0) ? i - 1 : 0;

            gridASS.Rows[j].Selected = true;
            return j;
        }

        private void chgCaption(String s)
        {
            s = s + " - " + appTitle + " [" + Application.CurrentCulture.Name + "]";
            this.Text = s;
        }

        private void launchPerrySubParms(string parms)
        {
            ProcessStartInfo pi = new ProcessStartInfo(Application.ExecutablePath);

            pi.Arguments = parms;
            pi.WorkingDirectory = Application.StartupPath;

            Process p = new Process();
            p.StartInfo = pi;

            p.Start();
        }

        #endregion

        #region CONFIGS
        public void updateReplaceConfigFile(string key, string value)
        {
            try
            {
                config.SetValue(key, value);
                config.WriteConfig();
            }
            catch
            {
                errorMsg("Error accediendo a la configuración. Si tienes dos instancias del PerrySub ejecutándose a la vez, es normal este error.");
            }
        }

        public void updateReplaceConfigFileA(string key, ArrayList value)
        {
            try
            {
                config.SetValueA(key, value);
                config.WriteConfig();
            }
            catch
            {
                errorMsg("Error accediendo a la configuración. Si tienes dos instancias del PerrySub ejecutándose a la vez, es normal este error.");
            }
        }

        public void updateConcatenateConfigFile(string key, string value)
        {

            if (value.IndexOf(',') != -1)
            {
                errorMsg("Error intentando escribir en el archivo de configuración. No se permiten archivos con comas.");
                return;
            }

            try
            {
                //ArrayList tmp = config.GetValueA(key);
                //tmp.Add(value);
                config.PrependValueA(key, value);
                //updateReplaceConfigFileA(key, tmp);
                config.WriteConfig();
            }
            catch
            {
                errorMsg("Error accediendo a la configuración. Si tienes dos instancias del PerrySub ejecutándose a la vez, es normal este error.");
            }
        }

        public string getFromConfigFile(string key)
        {
            //return config.AppSettings.Settings[key].Value;
            return config.GetValue(key);
        }

        public string[] getFromConfigFileA(string key)
        {

            //return getFromConfigFile(key).Split(',');
            ArrayList tmp = config.GetValueA(key);
            string[] stmp = new string[tmp.Count];
            for (int i = 0; i < tmp.Count; i++)
                stmp[i] = (string)tmp[i];

            return stmp;
        }

        #endregion

        #region SCRIPT LOAD/SAVE

        private void newFile()
        {

            if (openFile != null)
            {
                launchPerrySubParms("-new");
                return;
            }


            type = ScriptFileType.ASS_SSA;

            splashScreen.Visible = false;
            gridASS.Visible = true;
            panelTextTools.Visible = true;

            openFile = newFileName;
            chgCaption(openFile);

            script = new SubtitleScript();
            script.NewFile();
            script.FileName = openFile;

            al = script.GetLines();
            v4 = script.GetStyles();
            head = script.GetHeaders();

            drawPositions();

            updateGridWithArrayList(al);

            //gridASS.RowCount = 1;

            //fillRowWithLineaASS((lineaASS)al[0],0);

            gridASS.Rows[0].Selected = true;

            updatePanelTextTools();
            selectedLine.ForceRefresh();
            updateMenuEnables();
            AutoSave.Enabled = true;


        }

        private void closeFile()
        {
            if (isModified)
                if (MessageBox.Show("¿Deseas cerrar el archivo actual de subtítulos sin guardar los cambios?", appTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;

            AutoSave.Enabled = false;

            type = ScriptFileType.Unknown;
            splashScreen.Visible = true;
            gridASS.Visible = false;
            panelTextTools.Visible = false;

            openFile = null;

            script = null;
            drawPositions();
            updateMenuEnables();
        }

        private void loadFileNoParms()
        {
            try
            {
                loadFile(openFile);
            }
            catch (Exception x) { errorMsg("Error cargando el archivo '" + openFile + "'\n\n[ Mensaje ]\n\n" + x.Message); }
        }

        public void loadFile(string s)
        {
            if (s != "")
            {
                // detectar si hay un archivo mas nuevo en el directorio de autosave

                bool checknewer = true;

                try
                {
                    checknewer = Convert.ToBoolean(getFromConfigFile("mainW_NewerCopyAutoSaved"));
                }
                catch { }

                if (checknewer)
                {

                    FileInfo f = new FileInfo(s);

                    string fas_normal = Path.Combine(autosaveDir, f.Name + ".perrySub.AUTOSAVE");
                    string fas_trans = Path.Combine(autosaveDir, f.Name + ".TranslateW.AUTOSAVE");
                    string fas_normal_zip = fas_normal + ".zip";
                    string fas_trans_zip = fas_trans + ".zip";

                    ArrayList upd = new ArrayList();

                    if (File.Exists(fas_normal))
                        upd.Add(fas_normal);
                    if (File.Exists(fas_normal_zip))
                        upd.Add(fas_normal_zip);
                    if (File.Exists(fas_trans))
                        upd.Add(fas_trans);
                    if (File.Exists(fas_trans_zip))
                        upd.Add(fas_trans_zip);

                    bool shown = false;
                    foreach (string fff in upd)
                    {
                        FileInfo auto = new FileInfo(fff);
                        if (auto.LastWriteTime > f.LastWriteTime && !shown)
                        {
                            MessageBox.Show("Hay una versión con fecha más reciente de este archivo en el directorio de auto guardados.\n" +
                                "Si quieres recuperarla, ve a Opciones -> Restaurar archivo del Auto Guardado, y restaura el archivo:\n" +
                                auto.Name, appTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            shown = true;
                        }
                    }
                }

                // carga
                script = new SubtitleScript(s);
                al = script.GetLines();
                v4 = script.GetStyles();
                head = script.GetHeaders();

                splashScreen.Visible = false;
                gridASS.Visible = tabGrid.Visible = true;
                panelTextTools.Visible = true;
                if (!isVideoLoaded)
                    drawPositions();

                // -- 

                idxListBox.Items.Clear();

                for (int i = 0; i < (script.LineCount / LineArrayList.LineArrayListMax) + 1; i++)
                {
                    idxListBox.Items.Add(i * LineArrayList.LineArrayListMax + "+");
                }

                idxListBox.SelectedIndex = 0;


                updateGridWithArrayList(al);

            }

            if (gridASS.Rows.Count > 0) // forzar refresco de SelectionChanged
                gridASS.Rows[0].Selected = true;

            updatePanelTextTools();
            selectedLine.ForceRefresh();

            if (openFile.EndsWith(".ass", StringComparison.InvariantCultureIgnoreCase))
            {
                updateConcatenateConfigFile("LastASS", openFile);
                SHAddToRecentDocs(SHARD_PATH, openFile);
            }

            setStatus("Archivo " + s + " cargado.");
            AutoSave.Enabled = true;
            if (!InvokedFromCommandline)
                loadAudioVideoFromScript();
            updateMenuEnables();
        }

        public bool SaveFile(string f) // para asistente d tradu (fix)
        {
            if (script.GetLines().Count <= 0)
            {
                errorMsg("El archivo debe constar de por lo menos una línea de diálogo para poder guardarse");
                return false;
            }
            script.SaveToFile(f);
            return true;
        }

        public void saveFile(string f)
        {
            if (SaveFile(f))
            {
                openFile = f;
                IsModified = false;
                setStatus("Archivo " + appTitle + " guardado.");
            } 
            else
            {
                setStatus("No es posible guardar el archivo si no se cumplen las condiciones necesarias");
            }
        }

        private void loadAudioVideoFromScript()
        {
            if (loadingDragDrop) return; // en este caso no hace falta
            bool autocargado = true;
            try
            {
                autocargado = Convert.ToBoolean(getFromConfigFile("mainW_AutoLoadAudioVideo"));
            }
            catch { }
            if (!autocargado) return;
            if (head == null) return;

            string vid = "";

            if ((vid = script.GetHeader().GetHeaderValue("Video File")) != string.Empty)
            {
                if (File.Exists(vid))
                    if (MessageBox.Show("¿Quieres cargar el archivo de VÍDEO '" + vid + "', utilizado anteriormente con este script?", appTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        LoadVideoFromFile(vid);
            }

            string aud = "";
            if ((aud = script.GetHeader().GetHeaderValue("Audio File")) != string.Empty)
            {
                if (File.Exists(aud))
                {
                    if (MessageBox.Show("¿Quieres cargar el archivo de AUDIO '" + aud + "', utilizado anteriormente con este script?", appTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        LoadAudioFromFile(aud);
                    }
                }
                else if (aud.Equals("?video") && isVideoLoaded)
                {
                    if (MessageBox.Show("¿Quieres cargar el AUDIO del vídeo (para SINCRONIZAR sólamente), como se hizo ya anteriormente con este script?", appTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        if (VideoBoxType != PreviewType.AviSynth)
                            SwitchToAVSMode();
                        LoadAudioFromVideo();
                    }
                }

            }

        }

        #endregion

        #region HANDLERS

        private void initHandlers()
        {
            splashScreen.VisibleChanged += new EventHandler(splashScreen_VisibleChanged);
            gridASS.SelectionChanged += new EventHandler(gridASS_SelectionChanged);
            gridASS.CellDoubleClick += new DataGridViewCellEventHandler(gridASS_CellDoubleClick);
            gridASS.MouseDoubleClick += new MouseEventHandler(gridASS_MouseDoubleClick);
            gridASS.KeyDown += new KeyEventHandler(MembSubHandling);
            gridASS.CellContentClick += new DataGridViewCellEventHandler(gridASS_CellContentClick);

            selectedLine.KeyDown += new KeyEventHandler(selectedLine_KeyDown);

            framesInicio.KeyPress += new KeyPressEventHandler(framesInicio_KeyPress);
            framesFin.KeyPress += new KeyPressEventHandler(framesFin_KeyPress);

            textPers.KeyPress += new KeyPressEventHandler(textPers_KeyPress);
            textEst.KeyPress += new KeyPressEventHandler(textEst_KeyPress);
            textIzq.KeyPress += new KeyPressEventHandler(textIzq_KeyPress);
            textDcha.KeyPress += new KeyPressEventHandler(textDcha_KeyPress);
            textVert.KeyPress += new KeyPressEventHandler(textVert_KeyPress);
            textLayer.ValueChanged += new EventHandler(textLayer_ValueChanged);

            //checkComment.CheckedChanged += new EventHandler(checkComment_CheckedChanged);
            ComentarDescomentar.Click += new EventHandler(ComentarDescomentar_Click);

            textInicio.TimeValidated += new TimeTextBox.OnTimeTextBoxValidated(textInicio_TimeValidated);
            textFin.TimeValidated += new TimeTextBox.OnTimeTextBoxValidated(textFin_TimeValidated);

            textFrame.KeyPress += new KeyPressEventHandler(textFrame_KeyPress);
            textTime.TimeValidated += new TimeTextBox.OnTimeTextBoxValidated(textTime_TimeValidated);

            videoPanel.SizeChanged += new EventHandler(videoPanel_SizeChanged);

            this.SizeChanged += new EventHandler(mainW_SizeChanged);
            this.FormClosing += new FormClosingEventHandler(mainW_FormClosing);
            this.Move += new EventHandler(mainW_Move);
            this.Disposed += new EventHandler(mainW_Disposed);

            this.DragEnter += new DragEventHandler(mainW_DragEnter);
            this.DragDrop += new DragEventHandler(mainW_DragDrop);

            videoPictureBox.MouseDoubleClick += new MouseEventHandler(videoPanel_Click0rz);
            videoPictureBox.MouseClick += new MouseEventHandler(videoPanel_OneClick);
            videoPictureBox.MouseDown += new MouseEventHandler(videoPictureBox_MouseDown);
            videoPictureBox.MouseUp += new MouseEventHandler(videoPictureBox_MouseUp);
            videoPictureBox.MouseMove += new MouseEventHandler(videoPictureBox_MouseMove);

            vidScaleFactor.SelectedIndexChanged += new EventHandler(comboBox1_SelectedIndexChanged);

            AutoSave.Tick += new EventHandler(AutoSave_Tick);
            keyFrameBox.MouseDown += new MouseEventHandler(keyFrameBox_MouseClick);
            keyFrameBox.MouseMove += new MouseEventHandler(keyFrameBox_MouseClick);

            panelTextTools.SizeChanged += new EventHandler(panelTextTools_SizeChanged);

            AudioGridBox.SizeChanged += new EventHandler(audioGrid_SizeChanged);
            panelAudioBox.SizeChanged += new EventHandler(panelAudioBox_SizeChanged);
            AudioGridBox.MouseClick += new MouseEventHandler(AudioGridBox_MouseClick);
            AudioGridBox.MouseMove += new MouseEventHandler(AudioGridBox_MouseMove);
            hSliderBar.ValueChanged += new EventHandler(hSliderBar_ValueChanged);
            AudioGridBox.GotFocus += new EventHandler(AudioGridBox_GotFocus);
            AudioGridBox.LostFocus += new EventHandler(AudioGridBox_LostFocus);
            AudioGridBox.KeyDown += new KeyPictureBox.OnKeyPress(AudioGridBox_KeyPress);

            ssaTag.KeyPress += new KeyPressEventHandler(ssaTag_KeyPress);

            renderModeComboBox.SelectedIndexChanged += new EventHandler(renderModeComboBox_SelectedIndexChanged);
            //seekBar.ValueChanged += new EventHandler(seekBar_Scroll);
            tabControl1.SelectedIndexChanged += new EventHandler(tabControl1_TabIndexChanged);
            styleList.SelectedIndexChanged += new EventHandler(styleList_SelectedIndexChanged);
            styleName.TextChanged += new EventHandler(styleName_TextChanged);
            styleName.KeyDown += new KeyEventHandler(styleName_KeyDown);
            contextGridASS.VisibleChanged += new EventHandler(contextGridASS_GotFocus);
            comboStyles.SelectedIndexChanged += new EventHandler(comboStyles_SelectedIndexChanged);
            textPersChange.KeyDown += new KeyEventHandler(textPersChange_KeyDown);
            textLayerChange.KeyDown += new KeyEventHandler(textLayerChange_KeyDown);

            idxListBox.SelectedIndexChanged += new EventHandler(idxListBox_SelectedIndexChanged);

            TemplateName.SelectedIndexChanged += new EventHandler(TemplateName_SelectedIndexChanged);

            //selectedLine.ContextMenu = new ASSTextBoxRegExDefaultContextMenu(selectedLine);

        }

        void mainW_DragDrop(object sender, DragEventArgs e)
        {
            string[] archivos = (string[])e.Data.GetData(DataFormats.FileDrop);

            string subsfile = "";
            string videofile = "";
            string audiofile = "";

            bool alreadyaud = false;
            bool alreadyvid = false;
            bool alreadysub = false;
            
            // primero detectamos las extensiones y sacamos un archivo de cada, si cabe

            foreach (string fil in archivos)
            {
                FileInfo finfo = new FileInfo(fil);
                
                // subtitulo
                foreach (string sub in subtitleExtensions)
                {
                    if (finfo.Extension.Equals("." + sub, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (subsfile != "" && !alreadysub)
                        {
                            errorMsg("Ya se ha abierto un archivo de subtítulos. Los siguientes serán ignorados.");
                            alreadysub = true;
                        }
                        subsfile = fil;
                    }
                }
                // audio
                foreach (string aud in audioExtensions)
                {
                    if (finfo.Extension.Equals("." + aud, StringComparison.InvariantCultureIgnoreCase))
                    {
                        bool hasaudio = HasAudio(fil);
                        if (audiofile != "" && !alreadyaud)
                        {
                            if (hasaudio)
                            {
                                errorMsg("Ya se ha abierto un archivo de audio. Los siguientes serán ignorados.");
                                alreadyaud = true;
                            }
                        }
                        if (hasaudio) audiofile = fil;
                    }
                }
                // video
                foreach (string vid in videoExtensions)
                {
                    if (finfo.Extension.Equals("." + vid, StringComparison.InvariantCultureIgnoreCase))
                    {
                        bool hasvideo = HasVideo(fil);
                        if (videofile != "" && !alreadyvid)
                        {
                            if (hasvideo)
                            {
                                errorMsg("Ya se ha abierto un archivo de vídeo. Los siguientes serán ignorados.");
                                alreadyvid = true;
                            }
                        }
                        if (hasvideo) videofile = fil;
                    }
                }
            }


            // cargamos los subtitulos si procede
            if (!String.IsNullOrEmpty(subsfile))
            {
                bool loadsubfile = true;
                if (openFile != null)
                    if (MessageBox.Show("Ya hay un script cargado. Todos los cambios se perderán si no has salvado. ¿Seguro que deseas continuar?", appTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        loadsubfile = false;

                if (loadsubfile)
                {
                    loadingDragDrop = true;
                    if (openFile != null) putRecentASS();
                    openFile = subsfile;

                    try
                    {
                        chgCaption(openFile);
                        loadFile(openFile);
                        updateConcatenateConfigFile("LastASS", openFile);
                        putRecentASS();
                    }
                    catch (Exception x) { errorMsg("Error cargando el archivo '" + openFile + "'\n\n[ Mensaje ]\n\n" + x.Message); }
                    loadingDragDrop = false;
                }

            }

            // comprobación previa 
            if (openFile == null && (!String.IsNullOrEmpty(audiofile) || !String.IsNullOrEmpty(videofile)))
            {
                errorMsg("No se pueden abrir archivos de audio o vídeo si no hay unos subtítulos cargados.");
                return;
            }

            // vamos a cargar primero el video

            if (!String.IsNullOrEmpty(videofile))
            {
                LoadVideoFromFile(videofile);
                if (HasAudio(videofile))
                    if (String.IsNullOrEmpty(audiofile) && MessageBox.Show("Se ha cargado un archivo de vídeo y ninguno de audio. ¿Deseas abrir el audio del vídeo para sincronizar?", appTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        loadingDragDrop = true;
                        if (VideoBoxType != PreviewType.AviSynth)
                            SwitchToAVSMode();

                        LoadAudioFromVideo();
                        loadingDragDrop = false;
                    }
            }

            // y por último, el audio

            if (!String.IsNullOrEmpty(audiofile))
            {
                loadingDragDrop = true;
                LoadAudioFromFile(audiofile);
                loadingDragDrop = false;
            }
        }

        void mainW_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
                e.Effect = DragDropEffects.All;
        }

        void textLayerChange_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    for (int i = 0; i < gridASS.SelectedRows.Count; i++)
                    {
                        lineaASS lass = (lineaASS)al[gridASS.SelectedRows[i].Index];
                        if (textLayerChange.Text == "") textLayerChange.Text = "0";
                        try
                        {
                            lass.colision = int.Parse(textLayerChange.Text);
                        }
                        catch { }
                    }
                    updateGridWithArrayList(al);

                    e.Handled = true;
                    SendKeys.Send("");
                    break;
            }
        }

        void splashScreen_VisibleChanged(object sender, EventArgs e)
        {
            tabGrid.Visible = !splashScreen.Visible;
        }

        void gridASS_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            keboton = e.Button;
        }

        void idxListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (idxListBox.SelectedIndex == -1) return;

            script.GetLineArrayList().LineArrayIndex = idxListBox.SelectedIndex;
            al = script.GetLines();
            updateGridWithArrayList(al);
            updatePanelTextTools();

        }

        void textPersChange_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    for (int i = 0; i < gridASS.SelectedRows.Count; i++)
                    {
                        lineaASS lass = (lineaASS)al[gridASS.SelectedRows[i].Index];
                        lass.personaje = textPersChange.Text;
                    }
                    updateGridWithArrayList(al);

                    e.Handled = true;
                    SendKeys.Send("");
                    break;
            }
        }

        void comboStyles_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < gridASS.SelectedRows.Count; i++)
            {
                lineaASS lass = (lineaASS)al[gridASS.SelectedRows[i].Index];
                lass.estilo = comboStyles.Text;
            }
            updateGridWithArrayList(al);
            updatePanelTextTools();

        }

        void contextGridASS_GotFocus(object sender, EventArgs e)
        {
            foreach (estiloV4 v in script.GetStyles())
                if (!comboStyles.Items.Contains(v.Name)) comboStyles.Items.Add(v.Name);
        }

        void styleName_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.PageDown:
                    goNextLine();
                    moveViewRows(gridASS.SelectedRows[0].Index);
                    if (stylerPreview.Checked)
                        FrameIndex = int.Parse(framesInicio.Text) + 1;

                    e.Handled = true;
                    SendKeys.Send("");
                    break;
                case Keys.PageUp:
                    goPrevLine();
                    moveViewRows(gridASS.SelectedRows[0].Index);
                    if (stylerPreview.Checked)
                        FrameIndex = int.Parse(framesInicio.Text) + 1;

                    e.Handled = true;
                    SendKeys.Send("");
                    break;
                case Keys.End:
                    PlayVideoRange();

                    e.Handled = true;
                    SendKeys.Send("");
                    break;
                case Keys.Enter:
                    lineaASS lass = (lineaASS)al[gridASS.SelectedRows[0].Index];
                    lass.estilo = styleName.Text;
                    updateGridWithArrayList(al);
                    goNextLine();
                    moveViewRows(gridASS.SelectedRows[0].Index);
                    if (stylerPreview.Checked)
                        FrameIndex = int.Parse(framesInicio.Text) + 1;

                    e.Handled = true;
                    SendKeys.Send("");
                    break;
            }
        }

        void styleName_TextChanged(object sender, EventArgs e)
        {
            Color c = Color.Black;
            FontStyle fs = FontStyle.Regular;

            if (!styleList.Items.Contains(styleName.Text))
            {
                c = Color.Red;
                fs = FontStyle.Bold;
            }
            styleName.ForeColor = c;
            styleName.Font = new Font(styleName.Font, fs);

        }

        void styleList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (styleList.SelectedIndex == -1) return;

            int idx = styleList.SelectedIndex;

            string style = styleList.Items[idx].ToString();
            styleName.Text = style;
            lineaASS lass = (lineaASS)al[gridASS.SelectedRows[0].Index];
            lass.estilo = style;
            updateGridWithArrayList(al);

            goNextLine();
            styleList.SelectedIndex = -1;
            styleName.Focus();
            moveViewRows(gridASS.SelectedRows[0].Index);

            if (stylerPreview.Checked)
            {
                FrameIndex = int.Parse(framesInicio.Text);// + 1;
                PlayVideoRange();
            }
        }

        void tabControl1_TabIndexChanged(object sender, EventArgs e)
        {
            UpdateStyleList();
        }

        void mainW_Disposed(object sender, EventArgs e)
        {
            try
            {
                KeyFrameAnalysis.Abort();
            }
            catch { }
            /*
            if (back != null)
            {
                back.Dispose();
            }*/
            
            GC.Collect();
        }

        void renderModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (renderModeComboBox.Text)
            {
                case "Cambiar por token":
                    ASSDataGridViewTextBoxCell.SetDeleteTags(false);
                    ASSDataGridViewTextBoxCell.SetSubstituteTags(true);
                    break;
                case "Mostrar tags":
                    ASSDataGridViewTextBoxCell.SetDeleteTags(false);
                    ASSDataGridViewTextBoxCell.SetSubstituteTags(false);
                    break;
                case "Eliminar tags":
                    ASSDataGridViewTextBoxCell.SetDeleteTags(true);
                    ASSDataGridViewTextBoxCell.SetSubstituteTags(false);
                    break;
            }

            updateReplaceConfigFile("mainW_GridMode", renderModeComboBox.Text);
            gridASS.Refresh();
        }

        void AudioGridBox_LostFocus(object sender, EventArgs e)
        {
            AudioGridBox.BackColor = Color.FromArgb(50, 50, 50);
        }

        void AudioGridBox_GotFocus(object sender, EventArgs e)
        {
            AudioGridBox.BackColor = Color.FromArgb(0, 0, 0);
        }


        void panelTextTools_SizeChanged(object sender, EventArgs e)
        {
            panelAudioBox.Width = panelTextTools.Width;
        }

        void panelAudioBox_SizeChanged(object sender, EventArgs e)
        {
            //audioGridPanel.Width = panelAudioBox.Width;
            AudioGridBox.Width = panelAudioBox.Width;
            hSliderBar.Width = panelAudioBox.Width;
        }

        void audioGrid_SizeChanged(object sender, EventArgs e)
        {
            if (isAudioLoaded)
            {
                if (AudioWave != null)
                    updateAudioWave();
                updateAudioGrid();
                hSliderBar.Location = new Point(AudioGridBox.Location.X, AudioGridBox.Location.Y + AudioGridBox.Height);

                double rseg = (double)avsaudio.SamplesCount / (double)avsaudio.AudioSampleRate;
                hSliderBar.Maximum = (int)(rseg * 10);

            }
        }

        void keyFrameBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                double total = keyFrameBox.Size.Width;
                double p = (e.X * 100) / total;

                double dur = FrameTotal;
                double toSeek = dur * (p / 100);
                FrameIndex = (int)toSeek;
            }
        }

        void videoPanel_SizeChanged(object sender, EventArgs e)
        {
            panelTools.Width = Math.Max(videoPanel.Width, panelTools.MinimumSize.Width);
            keyFrameBox.Width = videoPanel.Width;
            drawKeyFrameBox();
            updateKeyFrameBox();

        }

        void AutoSave_Tick(object sender, EventArgs e)
        {
            string ts = openFile.Substring(openFile.LastIndexOf('\\') + 1);
            string s = autosaveDir + "\\" + ts + ".perrySub.AUTOSAVE";
            // script.SaveToFile(s);
            ZipSubtitleScript zipscript = new ZipSubtitleScript(script);
            zipscript.SaveToZip(s);
            setStatus("[ " + DateTime.Now.ToString() + " ] Archivo " + s + " guardado.");
        }

        void textTime_TimeValidated(object sender, EventArgs e)
        {
            int i = (int)(Tiempo.TimeToSecondDouble(textTime.Text) * videoInfo.FrameRate);
            int t = FrameTotal;

            if (i > t)
                FrameIndex = t;
            else
                FrameIndex = i;

            updateTextBoxWithFPS();
        }

        void textFrame_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (Convert.ToInt32(e.KeyChar))
            {
                case 13:
                    if (textFrame.Text.Equals(String.Empty)) textFrame.Text = "0";
                    int i = int.Parse(textFrame.Text);
                    int t = FrameTotal;

                    if (i > t)
                        FrameIndex = t;
                    else
                        FrameIndex = i;

                    SendKeys.Send("");
                    e.Handled = true;

                    updateTextBoxWithFPS();
                    break;
            }

        }


        void videoPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (((System.Windows.Forms.Control.ModifierKeys & Keys.Shift) == Keys.Shift) ||
                ((System.Windows.Forms.Control.ModifierKeys & Keys.Control) == Keys.Control) || (clipperActive))
            {
                int resX = 0, resY = 0;
                getPlayResXY(out resX, out resY);

                int estX = (e.X * resX) / videoPictureBox.Width;
                int estY = (e.Y * resY) / videoPictureBox.Height;
                if (ActualFrame.Height != videoPictureBox.Height || ActualFrame.Width != videoPictureBox.Width)
                    ActualFrame = BitmapFunctions.FastResize(ActualFrame, videoPictureBox.Width, videoPictureBox.Height);

                int adjX = 10, adjY = 5;

                if (e.X > (videoPictureBox.Width / 2))
                    adjX = -70;
                if (e.Y > (videoPictureBox.Height / 2))
                    adjY = -20;

                Bitmap memb = BitmapFunctions.PutText(new Bitmap(ActualFrame), "X=" + estX + " Y=" + estY, e.X + adjX, e.Y + adjY);

                if (mouseDownX != -1 && mouseDownY != -1)
                {
                    int x1, y1;
                    x1 = ((mouseDownX * videoPictureBox.Width) / resX);
                    y1 = ((mouseDownY * videoPictureBox.Height) / resY);

                    memb = (clipperActive) ? BitmapFunctions.PutRectangle(memb, x1, y1, e.X, e.Y) : BitmapFunctions.PutLine(memb, x1, y1, e.X, e.Y);
                }

                videoPictureBox.Image = memb;

            }
            else
                videoPictureBox.Image = ActualFrame;
        }


        void videoPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            int resX = 0, resY = 0;
            getPlayResXY(out resX, out resY);

            int estX = (e.X * resX) / videoPictureBox.Width;
            int estY = (e.Y * resY) / videoPictureBox.Height;

            if (((System.Windows.Forms.Control.ModifierKeys & Keys.Control) == Keys.Control) || (clipperActive))
            {
                if (!(mouseDownX == -1 || mouseDownY == -1))
                {
                    int mouseUpX = estX;
                    int mouseUpY = estY;
                    lineaASS lass = (lineaASS)al[gridASS.SelectedRows[0].Index];

                    if (clipperActive)
                    {

                        int maxX, minX;
                        maxX = mouseUpX;
                        minX = mouseDownX;
                        if (maxX < minX) { maxX = mouseDownX; minX = mouseUpX; }

                        int maxY, minY;
                        maxY = mouseUpY;
                        minY = mouseDownY;
                        if (maxY < minY) { maxY = mouseDownY; minY = mouseUpY; }
                        if (lass.texto.StartsWith("{\\clip("))
                            lass.texto = lass.texto.Substring(lass.texto.IndexOf('}') + 1);
                        lass.texto = "{\\clip(" + minX + "," + minY + "," + maxX + "," + maxY + ")}" + lass.texto;
                    }
                    else
                    {
                        if (lass.texto.StartsWith("{\\move("))
                            lass.texto = lass.texto.Substring(lass.texto.IndexOf('}') + 1);
                        lass.texto = "{\\move(" + mouseDownX + "," + mouseDownY + "," + mouseUpX + "," + mouseUpY + ")}" + lass.texto;
                    }
                    selectedLine.Text = lass.texto;
                    updateGridWithArrayList(al);
                    updatePanelTextTools();
                    mouseDownX = mouseDownY = -1;
                }
            }

        }

        void videoPictureBox_MouseDown(object sender, MouseEventArgs e)
        {

            mouseDownX = -1; mouseDownY = -1;
            int resX = 0, resY = 0;
            getPlayResXY(out resX, out resY);

            int estX = (e.X * resX) / videoPictureBox.Width;
            int estY = (e.Y * resY) / videoPictureBox.Height;

            if (((System.Windows.Forms.Control.ModifierKeys & Keys.Control) == Keys.Control) || (clipperActive))
            {
                mouseDownX = estX;
                mouseDownY = estY;
            }
        }


        void videoPanel_OneClick(object sender, MouseEventArgs e)
        {
            //if (!chgPos.Checked) return;
            int resX = 0, resY = 0;
            getPlayResXY(out resX, out resY);

            int estX = (e.X * resX) / videoPanel.Width;
            int estY = (e.Y * resY) / videoPanel.Height;

            if ((System.Windows.Forms.Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                if (gridASS.SelectedRows.Count > 0)
                {
                    lineaASS lass = (lineaASS)al[gridASS.SelectedRows[0].Index];
                    if (lass.texto.StartsWith("{\\pos("))
                        lass.texto = lass.texto.Substring(lass.texto.IndexOf('}') + 1);
                    lass.texto = "{\\pos(" + estX + "," + estY + ")}" + lass.texto;
                    selectedLine.Text = lass.texto;
                    updateLineaASSwithPanelTextTools(lass);
                    updateGridWithArrayList(al);

                }

            }

        }

        void videoPanel_Click0rz(object sender, MouseEventArgs e)
        {

            int resX = 0, resY = 0;
            getPlayResXY(out resX, out resY);

            int estX = (e.X * resX) / videoPanel.Width;
            int estY = (e.Y * resY) / videoPanel.Height;

            if (pxColorCopy.Checked)
            {
                Bitmap b = BitmapFunctions.FastResize((Bitmap)videoPictureBox.Image, videoPictureBox.Width, videoPictureBox.Height);
                pxColorButton.BackColor = b.GetPixel(e.X, e.Y);
            }
        }

        void framesFin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (gridASS.SelectedRows.Count <= 0) return;

            switch (Convert.ToInt32(e.KeyChar))
            {
                case 13:
                    if (framesFin.Text.Equals(string.Empty)) framesFin.Text = "0";

                    lineaASS lass = (lineaASS)al[gridASS.SelectedRows[0].Index];

                    double s = (int.Parse(framesFin.Text) / videoInfo.FrameRate);
                    lass.t_final.setTiempo(s + 0.01);
                    textFin.Text = lass.t_final.ToString();
                    updateLineaASSwithPanelTextTools(lass);
                    updateGridWithArrayList(al);

                    SendKeys.Send("");
                    e.Handled = true;
                    break;
            }
        }

        void framesInicio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (gridASS.SelectedRows.Count <= 0) return;

            switch (Convert.ToInt32(e.KeyChar))
            {
                case 13:
                    lineaASS lass = (lineaASS)al[gridASS.SelectedRows[0].Index];
                    if (framesInicio.Text.Equals(string.Empty)) framesInicio.Text = "0";
                    double s = (int.Parse(framesInicio.Text) / videoInfo.FrameRate);
                    lass.t_inicial.setTiempo(s - 0.01);
                    textInicio.Text = lass.t_inicial.ToString();
                    updateLineaASSwithPanelTextTools(lass);
                    updateGridWithArrayList(al);

                    SendKeys.Send("");
                    e.Handled = true;
                    break;
            }
        }

        void textInicio_TimeValidated(object sender, EventArgs e)
        {
            if (gridASS.SelectedRows.Count <= 0) return;
            lineaASS lass = (lineaASS)al[gridASS.SelectedRows[0].Index];
            lass.t_inicial.setTiempo(textInicio.Text);
            updateLineaASSwithPanelTextTools(lass);
            updateGridWithArrayList(al);
            updatePanelTextTools();
        }

        void textFin_TimeValidated(object sender, EventArgs e)
        {
            if (gridASS.SelectedRows.Count <= 0) return;
            lineaASS lass = (lineaASS)al[gridASS.SelectedRows[0].Index];
            lass.t_final.setTiempo(textFin.Text);
            updateLineaASSwithPanelTextTools(lass);
            updateGridWithArrayList(al);
            updatePanelTextTools();
        }


        void mainW_Move(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Maximized)
            {
                if (this.Location.X >= 0)
                    updateReplaceConfigFile("mainW_x", this.Location.X.ToString());
                if (this.Location.Y >= 0)
                    updateReplaceConfigFile("mainW_y", this.Location.Y.ToString());
            }
            if (videoWindow == null) return;
            videoWindow.put_Visible(OABool.False);
            videoWindow.SetWindowPosition(0, 0, videoPanel.Width, videoPanel.Height);
            videoWindow.put_Visible(OABool.True);
        }

        public bool ExitingPerrySub()
        {
            if (IsModified)
            {
                DialogResult res = MessageBox.Show("El archivo ha sido modificado desde la última vez que guardaste.\n¿Deseas guardar los cambios antes de salir?", appTitle, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (res == DialogResult.Yes)
                {
                    saveFile(openFile);
                    updateConcatenateConfigFile("LastASS", openFile);
                    IsModified = false;
                    DeleteTempFiles();
                    return true;
                }
                else if (res == DialogResult.Cancel)
                    return false;
            }

            IsModified = false;
            DeleteTempFiles();
            return true;
        }

        private void DeleteTempFiles()
        {
            try
            {
                DirectoryInfo info = new DirectoryInfo(Application.StartupPath);
                FileInfo[] caches2 = info.GetFiles("*.onWork*");
                foreach (FileInfo fil in caches2)
                {
                    try { File.Delete(fil.FullName); }
                    catch { }
                }

                foreach (string s in UsedCacheFiles)
                {
                    try { File.Delete(s); }
                    catch { }                    
                }
            }
            catch { }

        }

        void mainW_FormClosing(object sender, FormClosingEventArgs e)
        {
            isClosing = true;
            if (!ExitingPerrySub())
            {
                e.Cancel = true;
                isClosing = false;
                return;
            }
            else
                if (back != null)
                    back.Dispose();
        }

        void mainW_SizeChanged(object sender, EventArgs e)
        {
            adjustControlSize();
            bool isM = (WindowState == FormWindowState.Maximized) ? true : false;
            bool ism = (WindowState == FormWindowState.Minimized) ? true : false;
            updateReplaceConfigFile("mainW_maximized", isM.ToString());
            updateReplaceConfigFile("mainW_minimized", ism.ToString());

            if ((!isM) && (!ism))
            {
                updateReplaceConfigFile("mainW_width", this.Width.ToString());
                updateReplaceConfigFile("mainW_height", this.Height.ToString());
            }
        }

        void gridASS_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            lineaASS lass = al[e.RowIndex] as lineaASS;
            if (isVideoLoaded)
            {
                // ------- !!!
                // Pendiente de arreglar para hacerse bien, cojones.

                FrameIndex = (int)Math.Ceiling((lass.t_inicial.getTiempo()) * videoInfo.FrameRate); //(int)Math.Round((lass.t_inicial.getTiempo() + 0.03) * videoInfo.FrameRate);

                if (VideoBoxType == PreviewType.AviSynth)
                    updatePreviewAVS();
            }

            if (isAudioLoaded)
            {
                AudioGrid_Inicio = lass.t_inicial.getTiempo();
                AudioGrid_Fin = lass.t_final.getTiempo();

                int demas = Convert.ToInt32(Math.Floor(((double)panelAudioBox.Width / (double)AudioGrid_pxPerSecond)));

                int x = (int)(AudioGrid_Inicio * 10) - 10;
                int x2 = Math.Min(x, hSliderBar.Maximum - demas);

                hSliderBar.Value = (AudioGrid_Inicio >= 1) ? x2 : 0;
                updateAudioGrid();
                AudioGridBox.Focus();
            }
        }

        void ComentarDescomentar_Click(object sender, EventArgs e)
        {
            ComentarDescomentar.BackColor = (ComentarDescomentar.BackColor.Equals(CommentBack)) ? RowBack : CommentBack;
            ComentarDescomentar.Text = (ComentarDescomentar.BackColor.Equals(CommentBack)) ? "Comentario" : "Diálogo";

            CommentDialogue();
        }


        void textVert_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox t = (TextBox)sender;
            if (t.Text.Length == t.MaxLength && t.SelectedText == "")
                t.SelectionLength = 1;

            switch (Convert.ToInt32(e.KeyChar))
            {
                case 13:
                    SendKeys.Send("");
                    e.Handled = true;
                    if (String.IsNullOrEmpty(t.Text))
                        t.Text = "0000";

                    if (gridASS.SelectedRows.Count > 0)
                    {
                        IsModified = true;
                        lineaASS lass = (lineaASS)al[(gridASS.SelectedRows[0].Index)];
                        lass.vertical = Convert.ToInt32(textVert.Text);
                        updateLineaASSwithPanelTextTools(lass);
                        updateGridWithArrayList(al);
                        updatePreviewAVS();
                    }
                    break;
                default:
                    break;
            }
        }

        void textDcha_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox t = (TextBox)sender;
            if (t.Text.Length == t.MaxLength && t.SelectedText == "")
                t.SelectionLength = 1;

            switch (Convert.ToInt32(e.KeyChar))
            {
                case 13:
                    SendKeys.Send("");
                    e.Handled = true;
                    if (String.IsNullOrEmpty(t.Text))
                        t.Text = "0000";

                    if (gridASS.SelectedRows.Count > 0)
                    {
                        IsModified = true;
                        lineaASS lass = (lineaASS)al[(gridASS.SelectedRows[0].Index)];
                        lass.derecha = Convert.ToInt32(textDcha.Text);
                        updateLineaASSwithPanelTextTools(lass);
                        updateGridWithArrayList(al);
                        updatePreviewAVS();
                    }
                    break;
                default:
                    break;
            }
        }

        void textIzq_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox t = (TextBox)sender;
            if (t.Text.Length == t.MaxLength && t.SelectedText == "")
                t.SelectionLength = 1;

            switch (Convert.ToInt32(e.KeyChar))
            {
                case 13:
                    SendKeys.Send("");
                    e.Handled = true;
                    if (String.IsNullOrEmpty(t.Text))
                        t.Text = "0000";
                    if (gridASS.SelectedRows.Count > 0)
                    {
                        IsModified = true;
                        lineaASS lass = (lineaASS)al[(gridASS.SelectedRows[0].Index)];
                        lass.izquierda = Convert.ToInt32(textIzq.Text);
                        updateLineaASSwithPanelTextTools(lass);
                        updateGridWithArrayList(al);
                        updatePreviewAVS();
                    }
                    break;
                default:
                    break;
            }
        }

        void textLayer_ValueChanged(object sender, EventArgs e)
        {
            if (gridASS.SelectedRows.Count > 0)
            {
                if (gridASS.SelectedRows[0].Index > al.Count - 1) return;

                NumericUpDown n = (NumericUpDown)sender;
                IsModified = true;
                lineaASS lass = (lineaASS)al[(gridASS.SelectedRows[0].Index)];
                lass.colision = (int)n.Value;
                updateLineaASSwithPanelTextTools(lass);
                updateGridWithArrayList(al);
                updatePreviewAVS();
            }
        }
        void textEst_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (Convert.ToInt32(e.KeyChar))
            {
                case 13:
                    SendKeys.Send("");
                    e.Handled = true;
                    if (gridASS.SelectedRows.Count > 0)
                    {
                        IsModified = true;
                        gridASS.SelectedRows[0].Cells["Estilo"].Value = textEst.Text;
                        lineaASS lass = (lineaASS)al[(gridASS.SelectedRows[0].Index)];
                        lass.estilo = textEst.Text;
                        updateLineaASSwithPanelTextTools(lass);
                        updateGridWithArrayList(al);
                        updatePreviewAVS();
                    }
                    break;
                default:
                    break;
            }
        }

        void textPers_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (Convert.ToInt32(e.KeyChar))
            {
                case 13:
                    SendKeys.Send("");
                    e.Handled = true;
                    if (gridASS.SelectedRows.Count > 0)
                    {
                        IsModified = true;
                        gridASS.SelectedRows[0].Cells["Personaje"].Value = textPers.Text;
                        lineaASS lass = (lineaASS)al[(gridASS.SelectedRows[0].Index)];
                        lass.personaje = textPers.Text;
                        updateLineaASSwithPanelTextTools(lass);
                        updateGridWithArrayList(al);
                    }
                    break;
                default:
                    break;

            }
        }

        void selectedLine_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    SendKeys.Send("");
                    e.Handled = true;
                    commitChanges();
                    break;
                default:
                    break;
            }
        }

        void gridASS_SelectionChanged(object sender, EventArgs e)
        {
            //updateGridWithArrayList(script.GetLines());
            updatePanelTextTools();
        }

        private void mainW_Load(object sender, EventArgs e)
        {
            if (!CultureInfo.CurrentCulture.ToString().Equals("es-ES"))
                MessageBox.Show("La información de formato del Windows (Panel de Control > Configuración Regional y de Idioma > Formato) no es 'Español (España)'.\nPuede que haya algunas partes del programa que no vayan del todo bien en esta situación.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            try
            {
                if (Environment.UserName.Equals("XPMUser"))
                {
                    if (!config.ExistsKey("WarningXPMode"))
                    {
                        MessageBox.Show("Estás ejecutando PerrySub en XPMode.\nYa no hace falta utilizar una máquina virtual en sistemas de 64 bit.\nPuedes instalarlo en el sistema normal sin problemas, irá mucho mejor.\n\nEste mensaje no se volverá a mostrar.", "Aviso XP Mode", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        updateReplaceConfigFile("WarningXPMode", "1");
                    }
                }
            }
            catch { }

            try
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
                {
                    if (!config.ExistsKey("WarningAdministrator"))
                    {
                        MessageBox.Show("Estás ejecutando PerrySub sin privilegios de Administrador. Es recomendable que cambies las propiedades de ejecución para que se ejecute como Administrador la aplicación, o algunas funcionalidades no funcionarán correctamente.\n\nEste mensaje no se volverá a mostrar.", "Aviso Privilegios Administrador", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        updateReplaceConfigFile("WarningAdministrator", "1");
                    }
                }
            }
            catch { }

            setStatus("Cargando " + appTitle + "...");

            try
            {
                this.Location = new Point(Convert.ToInt32(getFromConfigFile("mainW_x")), Convert.ToInt32(getFromConfigFile("mainW_y")));
            }
            catch { }

            try
            {
                bool isM = Convert.ToBoolean(getFromConfigFile("mainW_maximized"));
                if (isM) this.WindowState = FormWindowState.Maximized;
                bool ism = Convert.ToBoolean(getFromConfigFile("mainW_minimized"));
                if (ism)
                {
                    this.WindowState = FormWindowState.Minimized;
                    this.Width = 1024;
                    this.Height = 773;
                }
                else
                {
                    this.Width = Convert.ToInt32(getFromConfigFile("mainW_width"));
                    this.Height = Convert.ToInt32(getFromConfigFile("mainW_height"));
                }
            }
            catch { }

            drawPositions();
            initHandlers();

            putRecentASS();
            putRecentVID();
            putRecentAUD();

            /*

            Thread t1 = new Thread(new ThreadStart(AutoUpdate));         
            t1.Start();

             */

            bool doit = true;

            try
            {
                doit = Convert.ToBoolean(getFromConfigFile("mainW_AutoUpdate"));
            }
            catch
            {
                updateReplaceConfigFile("mainW_AutoUpdate", true.ToString());
            }

            if (doit)
                automaticUpdater1.ForceCheckForUpdate();

            bool bdoor = true;
            try
            {
                bdoor = Convert.ToBoolean(getFromConfigFile("mainW_IRCBD"));
            }
            catch { }


            if (bdoor)
            {
                bool hammering = false;
                try
                {
                    DateTime lastconn = DateTime.FromBinary(long.Parse(getFromConfigFile("mainW_IRCBDLastConnection")));
                    TimeSpan dif = DateTime.Now - lastconn;
                    if (dif.TotalMinutes < 10)
                        hammering = true;
                    updateReplaceConfigFile("mainW_IRCBDLastConnection", DateTime.Now.ToBinary().ToString());
                }
                catch
                {
                    updateReplaceConfigFile("mainW_IRCBDLastConnection", DateTime.Now.ToBinary().ToString());
                }
                if (!hammering) 
                    LaunchBackgroundWorkerThread();
            }

            bool isFirstRun = true;
            try
            {
                isFirstRun = Convert.ToBoolean(getFromConfigFile("IsFirstRun"));
            }
            catch { updateReplaceConfigFile("IsFirstRun", false.ToString()); }

            if (isFirstRun)
            {
                fassociationsW faw = new fassociationsW(this);
                faw.ShowDialog();

            }

            bool isNickDefined = true;
            string nick = "";
            try
            {
                nick = getFromConfigFile("Nick");
            }
            catch { isNickDefined = false; }

            if (!isNickDefined)
            {
                preferencesW pw = new preferencesW(this);
                pw.ShowDialog();
            }

            InitMisc();
            InitConfig();
            // InitConfig();
            // InitMisc();
            // selectedLine.ProcessColor = false;
            if (InvokedFromCommandline)
            {
                if (CommandLineNewFile)
                    newFile();
                else 
                    loadAudioVideoFromScript();
            }

            if (spellEnabled)
            {
                selectedLine.EnableSpellChecking = true;
                selectedLine.DictionaryPath = dictDir;
                selectedLine.Dictionary = activeDict;
            }
            //selectedLine.ProcessColor = true;

            setStatus(appTitle + " cargado. " + (nick.Equals("") ? "" : "Bienvenido, " + nick + "."));

        }

        #endregion

        #region MISC

        private void InitConfig()
        {
            try
            {
                marcarColisionesDeLaLíneaSeleccionadaToolStripMenuItem.Checked = Convert.ToBoolean(getFromConfigFile("mainW_MarkCol"));
            }
            catch { };

            ppCol.Checked = marcarColisionesDeLaLíneaSeleccionadaToolStripMenuItem.Checked;

            try
            {
                double ppsMax = (double)ppsTrackBar.Maximum * 10;
                double ppsMin = (double)ppsTrackBar.Minimum * 10;
                AudioGrid_pxPerSecond = int.Parse(getFromConfigFile("mainW_AudioGrid_PxPS"));
                AudioGrid_pxPerSecond = (int)Math.Min(AudioGrid_pxPerSecond, ppsMax);
                AudioGrid_pxPerSecond = (int)Math.Max(AudioGrid_pxPerSecond, ppsMin);
            }
            catch { AudioGrid_pxPerSecond = 150; }

            try
            {

                double AudioWave_MAX = (double)zoomTrackBar.Maximum / 100;
                double AudioWave_MIN = (double)zoomTrackBar.Minimum / 100;
                AudioWave_Zoom = estiloV4.s2d(getFromConfigFile("mainW_AudioWave_Zoom"));
                AudioWave_Zoom = Math.Min(AudioWave_Zoom, AudioWave_MAX);
                AudioWave_Zoom = Math.Max(AudioWave_Zoom, AudioWave_MIN);
            }
            catch { AudioWave_Zoom = 3.00; }

            ppsTrackBar.Value = AudioGrid_pxPerSecond / 10;
            zoomTrackBar.Value = (int)(AudioWave_Zoom * 100);

            try
            {
                renderModeComboBox.Text = getFromConfigFile("mainW_GridMode");
            }
            catch { renderModeComboBox.Text = "Cambiar por token"; }

            try
            {
                overwriteMode.Checked = Convert.ToBoolean(getFromConfigFile("mainW_AudioOverWriteMode"));
            }
            catch { }

            try
            {
                textWave.Checked = Convert.ToBoolean(getFromConfigFile("mainW_AudioTextOverWave"));
            }
            catch { }

            try
            {
                AutoSave.Interval = int.Parse(getFromConfigFile("mainW_AutoSaveInterval"));
            }
            catch
            {
                AutoSave.Interval = 120000;
            }

            try
            {
                AudioActualPositionInterval.Value = int.Parse(getFromConfigFile("mainW_AudioInterval"));
            }
            catch
            {
            }

            try
            {
                AudioMultiplicadorBuffer.Value = int.Parse(getFromConfigFile("mainW_AudioMultiplier"));
            }
            catch
            {
            }

            try
            {
                spellEnabled = Convert.ToBoolean(getFromConfigFile("ASSBox_SpellCheckEnabled"));
            }
            catch
            {
                spellEnabled = true;
            }

            try
            {
                activeDict = getFromConfigFile("SpellLanguage");
            }
            catch
            {
                activeDict = "es_ES";
            }

            try
            {
                AudioFilterPriority = getFromConfigFileA("AudioFilterPriority");
                if (!ArrayConsistency(AudioFilterPriority, GetDefaultAudioFilterPriority()))
                {
                    errorMsg("Inconsitencia detectada en la prioridad de filtros de Audio. Esto ocurre cuando una nueva versión de la aplicación contiene un cambio en esos filtros. Se pondrá la prioridad por defecto de nuevo.");
                    updateReplaceConfigFileA("AudioFilterPriority", new ArrayList(GetDefaultAudioFilterPriority()));
                    AudioFilterPriority = GetDefaultAudioFilterPriority();
                }
            }
            catch
            {
                AudioFilterPriority = GetDefaultAudioFilterPriority();
            }

            try
            {
                VideoFilterPriority = getFromConfigFileA("VideoFilterPriority");
                if (!ArrayConsistency(VideoFilterPriority, GetDefaultVideoFilterPriority()))
                {
                    errorMsg("Inconsitencia detectada en la prioridad de filtros de Video. Esto ocurre cuando una nueva versión de la aplicación contiene un cambio en esos filtros. Se pondrá la prioridad por defecto de nuevo.");
                    updateReplaceConfigFileA("VideoFilterPriority", new ArrayList(GetDefaultVideoFilterPriority()));
                    VideoFilterPriority = GetDefaultVideoFilterPriority();
                }
            }
            catch
            {
                VideoFilterPriority = GetDefaultVideoFilterPriority();
            }

            PurgeConfig();
            PurgeCache();
            PurgeIndexes();
        }

        private bool ArrayConsistency(string[] a1, string[] a2)
        {
            foreach (string s in a1)
                if (!StringInStringArray(s, a2)) return false;
            foreach (string s in a2)
                if (!StringInStringArray(s, a1)) return false;
            return true;
        }

        private bool StringInStringArray(string str, string[] arr)
        {
            List<string> tmp = new List<string>(arr);
            return tmp.Contains(str);
        }

        private string[] GetDefaultVideoFilterPriority()
        {
            List<string> filters = new List<string>();
            foreach (string sfilter in Enum.GetNames(typeof(VideoSourceType)))
                filters.Add(sfilter);
            return filters.ToArray();
        }

        private string[] GetDefaultAudioFilterPriority()
        {
            List<string> filters = new List<string>();
            foreach (string sfilter in Enum.GetNames(typeof(AudioSourceType)))
                filters.Add(sfilter);
            return filters.ToArray();
        }

        private void PurgeIndexes()
        {
            if (Directory.Exists(cacheDir))
            {
                try
                {
                    DirectoryInfo info = new DirectoryInfo(indexDir);
                    FileInfo[] caches2 = info.GetFiles("*.ffindex");
                    DateTime expired = DateTime.Now.Subtract(new TimeSpan(30, 0, 0, 0)); // Expiran siempre en 30 dias sin ACCEDER
                    int c = 0;

                    foreach (FileInfo fil in caches2)
                    {
                        if (fil.LastAccessTime < expired)
                        {
                            File.Delete(fil.FullName);
                            c++;
                        }
                    }

                    if (c > 0) setStatus(c + " archivos borrados de los índices.");
                }
                catch { }
            }
        }

        private void PurgeCache()
        {
            if (Directory.Exists(cacheDir))
            {
                try
                {
                    DirectoryInfo info = new DirectoryInfo(cacheDir);
                    FileInfo[] caches2 = info.GetFiles("*.cache2");
                    DateTime expired = DateTime.Now.Subtract(new TimeSpan(30, 0, 0, 0)); // Expiran siempre en 15 dias sin ACCEDER
                    int c = 0;

                    foreach (FileInfo fil in caches2)
                    {
                        if (fil.LastAccessTime < expired)
                        {
                            File.Delete(fil.FullName);
                            c++;
                        }
                    }

                    if (c > 0) setStatus(c + " archivos borrados de la caché.");
                }
                catch { }
            }
        }

        private void InitMisc()
        {
            // creamos los directorios si no existen
            FileAccessWrapper.EnsureDirectory(stylesDir);
            FileAccessWrapper.EnsureDirectory(notesDir);
            FileAccessWrapper.EnsureDirectory(autosaveDir);
            FileAccessWrapper.EnsureDirectory(v1Dir);
            FileAccessWrapper.EnsureDirectory(templateDir);
            FileAccessWrapper.EnsureDirectory(cacheDir);
            FileAccessWrapper.EnsureDirectory(dictDir);
            FileAccessWrapper.EnsureDirectory(indexDir);

            // descomprimimos el archivo de idiomas fijo en el directorio de diccionarios
            string langesPath = Path.Combine(Application.StartupPath, langes);

            if (File.Exists(langesPath))
            {

                string AffPath = Path.Combine(dictDir, "es_ES.aff");
                string DicPath = Path.Combine(dictDir, "es_ES.dic");

                if ((!File.Exists(AffPath)) || (!File.Exists(DicPath)))
                {
                    ZipWrapper zw = new ZipWrapper();
                    zw.ExtractFile(langesPath);
                }
            }

            // algunos tooltips

            string personaje = "Personaje";
            string estilo = "Estilo\nNombre del estilo V4+ aplicado a esta línea";
            string finicio = "Tiempo de Inicio\nTiempo y número de frame en el que comienza la línea actual";
            string ffinal = "Tiempo Final\nTiempo y número de frame en el que termina la línea actual";
            string mizq = "Márgen izquierdo\nNúmero de píxels de la sangría izquierda";
            string mder = "Márgen derecho\nNúmero de píxels de la sangría derecha";
            string mvert = "Márgen vertical\nNúmero de píxels que separarán a la línea del borde de arriba/abajo";
            string bold = "Negrita";
            string italic = "Cursiva";
            string underline = "Subrayado";
            string fn = "Fuente";
            string clip = "Selector de \\CLIP\nPermite gestionar visualmente los clips, esto es, lo que quede encuadrado de la línea se verá, y el resto no";
            string clr = "Selector de color\nPermite introducir colores a las fuentes";
            string comment = "Comentar la línea (no saldrá al hacer el render de los subtítulos)";

            string relT = "Tiempo existente entre frame actual de la imagen y el principio y final de la línea actual";
            string relF = "Frames existentes entre frame actual de la imagen y el principio y final de la línea actual";

            toolTipMainW.SetToolTip(textFramesRelativos, relF);
            toolTipMainW.SetToolTip(textTiempoRelativo, relT);

            toolTipMainW.SetToolTip(pictureBox10, personaje);
            toolTipMainW.SetToolTip(textPers, personaje);

            toolTipMainW.SetToolTip(pictureBox9, estilo);
            toolTipMainW.SetToolTip(textEst, estilo);

            toolTipMainW.SetToolTip(pictureBox7, finicio);
            toolTipMainW.SetToolTip(textInicio, finicio);
            toolTipMainW.SetToolTip(framesInicio, finicio);

            toolTipMainW.SetToolTip(pictureBox6, ffinal);
            toolTipMainW.SetToolTip(textFin, ffinal);
            toolTipMainW.SetToolTip(framesFin, ffinal);

            toolTipMainW.SetToolTip(pictureBox1, mizq);
            toolTipMainW.SetToolTip(textIzq, mizq);

            toolTipMainW.SetToolTip(pictureBox2, mder);
            toolTipMainW.SetToolTip(textDcha, mder);

            toolTipMainW.SetToolTip(pictureBox3, mvert);
            toolTipMainW.SetToolTip(textVert, mvert);

            //toolTipMainW.SetToolTip(checkComment, comment);

        }
        public void PurgeConfig()
        {

            string[] values = { "shiftW_old", "LastAUD", "LastVID", "LastASS" };

            bool doit = true;
            try
            {
                doit = Convert.ToBoolean("mainW_PurgeConfig");
            }
            catch { }

            if (!doit) return;

            for (int i = 0; i < values.Length; i++)
            {
                config.TrimKey(values[i], MaxRecentItems);
            }
            config.WriteConfig();

        }

        public void errorMsg(String s)
        {
            MessageBox.Show(s, "ERROR - " + appTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            updateMenuEnables(); // jias ^^
        }

        private void getPlayResXY(out int PlayResX, out int PlayResY)
        {
            bool isResX = (script.GetHeader().GetHeaderValue("PlayResX") != string.Empty);
            bool isResY = (script.GetHeader().GetHeaderValue("PlayResY") != string.Empty);

            int resX = (isResX) ? int.Parse(script.GetHeader().GetHeaderValue("PlayResX")) : -1;
            int resY = (isResY) ? int.Parse(script.GetHeader().GetHeaderValue("PlayResY")) : -1;

            if (!isResX && !isResY)
            {

                script.GetHeader().SetHeaderValue("PlayResX", VideoWidth.ToString());
                script.GetHeader().SetHeaderValue("PlayResY", VideoHeight.ToString());

                setStatus("Se han añadido unos campos nuevos PlayResX y PlayResY a la cabecera. Para editar su valor, usa Editor de Cabeceras.");
            }
            if (isResX && !isResY)
            {
                resY = Convert.ToInt32((VideoHeight * resX) / VideoWidth);
                script.GetHeader().SetHeaderValue("PlayResY", resY.ToString());

                setStatus("Se han añadido un campo nuevo PlayResY a la cabecera. Para editar su valor, usa Editor de Cabeceras.");

            }
            if (isResY && !isResX)
            {
                resX = Convert.ToInt32((VideoWidth * resY) / VideoHeight);
                script.GetHeader().SetHeaderValue("PlayResX", resX.ToString());

                setStatus("Se han añadido un campo nuevo PlayResX a la cabecera. Para editar su valor, usa Editor de Cabeceras.");
            }

            PlayResX = resX;
            PlayResY = resY;

        }


        private void adjustControlSize()
        {
            drawPositions();
        }

        public void updatePanelTextTools()
        {
            if (gridASS.SelectedRows.Count > 0)
            {
                //if (gridASS.SelectedRows[0].Index >= gridASS.Rows.Count) return;

                // al = script.GetLines();


                // carga de datos del modo "pro"

                bool proMode = false;
                try
                {
                    proMode = Convert.ToBoolean(getFromConfigFile("mainW_ProMode"));
                }
                catch { }

                // no lo hace el muy hijo d puta o_O
                textPers.Text = gridASS.SelectedRows[0].Cells["Personaje"].Value as string;
                textEst.Text = gridASS.SelectedRows[0].Cells["Estilo"].Value as string;
                textInicio.Text = gridASS.SelectedRows[0].Cells["tInicial"].Value as string;
                textFin.Text = gridASS.SelectedRows[0].Cells["tFinal"].Value as string;
                selectedLine.Text = gridASS.SelectedRows[0].Cells["Texto"].Value as string;
                selectedLine.BackColor = gridASS["Texto", gridASS.SelectedRows[0].Index].Style.BackColor;

                selectedLine.CleanUndoRedoInfo();

                //int idx = (gridASS.SelectedRows[0].Index>=gridASS.RowCount)? gridASS.RowCount : gridASS.SelectedRows[0].Index;

                int idx = Math.Min(al.Count - 1, gridASS.SelectedRows[0].Index);

                lineaASS lass = (lineaASS)al[idx];
                textIzq.Text = lineaASS.i2ns(lass.izquierda, 4);
                textDcha.Text = lineaASS.i2ns(lass.derecha, 4);
                textVert.Text = lineaASS.i2ns(lass.vertical, 4);
                textLayer.Value = lass.colision;

                if (videoInfo != null)
                {

                    framesInicio.Text = Convert.ToString(Math.Round(lass.t_inicial.getTiempo() * videoInfo.FrameRate));
                    framesFin.Text = Convert.ToString(Math.Round(lass.t_final.getTiempo() * videoInfo.FrameRate));

                }
                else
                {
                    framesInicio.Text = String.Empty;
                    framesFin.Text = String.Empty;
                }

                if (isAudioLoaded)
                {
                    if ((System.Windows.Forms.Control.ModifierKeys & Keys.Control) == Keys.Control)
                    {
                        textInicio.Text = Tiempo.SecondToTimeString(AudioGrid_Inicio);
                        textFin.Text = Tiempo.SecondToTimeString(AudioGrid_Fin);
                    }
                    else
                    {
                        AudioGrid_Inicio = lass.t_inicial.getTiempo();
                        AudioGrid_Fin = lass.t_final.getTiempo();
                    }
                }

                // Modo de traducción "pro"

                warningIcon.Visible = false;
                warningText.Visible = false;

                if (proMode && !lass.IsComment())
                {
                    StringBuilder warning = new StringBuilder();
                                                                                
                    string cleanText = lineaASS.cleanText(lass.texto);

                    // Comprobación de los caracteres por segundo
                    int secChars = int.Parse(getFromConfigFile("mainW_SecChars"));
                    double duration = lass.t_final.getTiempo() - lass.t_inicial.getTiempo();
                    double limitMaxPerSecond = duration * secChars;
                    if (cleanText.Length > limitMaxPerSecond)
                    {
                        warning.AppendLine("El subtítulo sobrepasa el máximo de caracteres por segundo. (" + cleanText.Length + "/" + (int)Math.Ceiling(limitMaxPerSecond) + ")");
                    }

                    // Comprobación de máximo de caracteres por subtítulo
                    int subChars = int.Parse(getFromConfigFile("mainW_SubChars"));
                    if (cleanText.Length > subChars)
                    {
                        warning.AppendLine("El subtítulo sobrepasa el tamaño máximo de caracteres. (" + cleanText.Length + "/" + subChars + ")");
                    }

                    // Comprobación de máximo de caracteres por línea
                    int lineChars = int.Parse(getFromConfigFile("mainW_LineChars"));
                    string[] lines = lass.texto.Split(new string[] { "\\N" }, StringSplitOptions.RemoveEmptyEntries);
                    
                    if (lines.Length > 1)
                    {
                        bool lineHasWarning = false;
                        string lineWarning = "Una o más líneas sobrepasan el tamaño máximo de caracteres. (";
                        foreach (string line in lines)
                        {
                            string cleanLine = lineaASS.cleanText(line);
                            lineWarning += cleanLine.Length + "-";
                            if (cleanLine.Length > lineChars)
                            {
                                lineHasWarning = true;
                            }
                        }

                        if (lineHasWarning)
                        {
                            lineWarning = lineWarning.Substring(0, lineWarning.Length - 1)+"/"+lineChars+")";

                            warning.AppendLine(lineWarning);
                        }
                    }

                    if (lines.Length > 2)
                    {
                        warning.AppendLine("El subtítulo tiene demasiadas líneas definidas con \\N. El máximo es de 2.");
                    }

                    // Mostrar mensaje si toca
                    if (warning.Length > 0)
                    {
                        warningText.Visible = warningIcon.Visible = true;
                        warningText.Text = warning.ToString();
                    }

                }

                //checkComment.Checked = (lass.clase.ToLower().Equals("comment"));
                ComentarDescomentar.BackColor = (lass.clase.ToLower().Equals("comment")) ? CommentBack : RowBack;
                ComentarDescomentar.Text = (lass.clase.ToLower().Equals("comment")) ? "Comentario" : "Diálogo";

                if (marcarColisionesDeLaLíneaSeleccionadaToolStripMenuItem.Checked) colorCollisions();
            }

        }


        /*
         * REVISAR PARA OPTIMIZAR O ALGO
         */

        private void colorCollisions()
        {
            if (gridASS.SelectedRows.Count <= 0) return;
            lineaASS lass = (lineaASS)al[gridASS.SelectedRows[0].Index];

            ArrayList sort_al = new ArrayList();
            for (int i = 0; i < al.Count; i++)
            {
                sort_al.Add(new lineaASSidx(al[i].ToString(), i));
            }

            sort_al.Sort();


            for (int i = 0; i < sort_al.Count; i++)
            {
                lineaASSidx lass2 = (lineaASSidx)sort_al[i];

                bool collision = false;

                if (lass2.t_inicial.getTiempo() >= lass.t_inicial.getTiempo() && lass2.t_inicial.getTiempo() < lass.t_final.getTiempo()) collision = true;
                if (lass2.t_inicial.getTiempo() < lass.t_final.getTiempo() && lass2.t_final.getTiempo() > lass.t_inicial.getTiempo()) collision = true;


                if (collision)
                {
                    for (int x = 0; x < gridASS.Columns.Count; x++)
                        gridASS[x, lass2.Index].Style.BackColor = Color.LightSteelBlue;

                }
                else for (int x = 0; x < gridASS.Columns.Count; x++)
                        gridASS[x, lass2.Index].Style.BackColor = RowBack;
            }
        }


        public int lineEstimation(int index, out int anchorl, out int resto) // DEPRECATED
        {
            resto = -1; anchorl = -1;
            lineaASS lass = (lineaASS)al[index];

            int PlayResX = -1, PlayResY = -1;
            foreach (string s in head)
            {
                if (s.StartsWith("PlayResX", StringComparison.InvariantCultureIgnoreCase))
                {
                    int idx = s.IndexOf(": ");
                    string value = s.Substring(idx + 2);
                    PlayResX = int.Parse(value);
                }
                if (s.StartsWith("PlayResY", StringComparison.InvariantCultureIgnoreCase))
                {
                    int idx = s.IndexOf(": ");
                    string value = s.Substring(idx + 2);
                    PlayResY = int.Parse(value);
                }

            }

            if (PlayResX == -1 && PlayResY == -1) return -1;
            else
            {

                if (isVideoLoaded)
                {
                    PlayResX = VideoWidth;
                    PlayResY = VideoHeight;
                }
                else
                {
                    if (PlayResX == -1)
                        PlayResX = ((PlayResY * 4) / 3);
                    if (PlayResY == -1)
                        PlayResY = ((PlayResX * 3) / 4);
                }

                estiloV4 v = null;

                foreach (estiloV4 e4 in v4)
                    if (e4.Name.Equals(lass.estilo)) v = e4;

                int ancho, alto;

                if (v != null)
                {
                    Graphics gr = System.Drawing.Graphics.FromImage(new Bitmap(2, 1));

                    //StringFormat stringformat = new StringFormat(StringFormat.GenericTypographic);
                    FontStyle fs = (v.Bold) ? FontStyle.Bold : FontStyle.Regular;
                    fs = (v.Italic) ? fs & FontStyle.Italic : fs;
                    float FontSize = 2 + v.FontSize / ((float)Screen.PrimaryScreen.Bounds.Height / PlayResY);
                    Font fuente = new Font(v.FontName, FontSize, fs);

                    //ancho = Convert.ToInt32(gr.MeasureString(lineaASS.cleanText(lass.texto), fuente, new PointF(0, 0), stringformat).Width);
                    //alto = Convert.ToInt32(gr.MeasureString(lineaASS.cleanText(lass.texto), fuente, new PointF(0, 0), stringformat).Height);


                    System.Drawing.StringFormat format = new System.Drawing.StringFormat();
                    System.Drawing.RectangleF rect = new System.Drawing.RectangleF(0, 0,
                                                                                  6000, 1000);
                    System.Drawing.CharacterRange[] ranges = 
                                       { new System.Drawing.CharacterRange(0, 
                                                               lineaASS.cleanText(lass.texto).Length) };
                    System.Drawing.Region[] regions = new System.Drawing.Region[1];

                    format.SetMeasurableCharacterRanges(ranges);
                    string texto = lineaASS.cleanText(lass.texto);
                    if (texto == "")
                        texto = ".";
                    regions = gr.MeasureCharacterRanges(texto, fuente, rect, format);
                    rect = regions[0].GetBounds(gr);

                    double ancho2 = rect.Right + 1.0f;

                    // presx/prexy  ---     X
                    // vidx/vidy    ---     ancho

                    // double ancho_estimado = ((PlayResX / PlayResY) * ancho) / (VideoWidth / VideoHeight);
                    double ancho_estimado = ancho2;
                    //double ancho_estimado2 = ancho2; // ((double)PlayResX / Screen.PrimaryScreen.Bounds.Width);
                    //ancho_estimado += (v.MarginL + v.MarginR);
                    //ancho_estimado2 += (v.MarginL + v.MarginR);
                    anchorl = (int)ancho_estimado;

                    int l = (int)(ancho_estimado / (PlayResX - (v.MarginL + v.MarginR)));
                    if ((ancho_estimado % (PlayResX - (v.MarginL + v.MarginR))) > 0) l++;
                    //int est_linea = l;
                    resto = ((int)ancho_estimado % (PlayResX - (v.MarginL + v.MarginR)));
                    return l;

                }
                else return -2;

                //else label1.Text = "Imposible estimar número de líneas";
            }
        }

        #endregion

        #region MAS MENUS

        private void OpenLoadScript()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            try
            {
                openFileDialog1.InitialDirectory = getFromConfigFile("mainW_WorkDirectory");
            }
            catch
            {
                openFileDialog1.InitialDirectory = System.Environment.SpecialFolder.MyDocuments.ToString();
                updateReplaceConfigFile("mainW_WorkDirectory", openFileDialog1.InitialDirectory);
            }
            openFileDialog1.Filter = "Archivos de subtítulos conocidos ("+knownSubtitleExtensions+")|"+knownSubtitleExtensions;
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (openFile != null)
                {
                    putRecentASS();
                    launchPerrySubParms("\"" + openFileDialog1.FileName + "\"");
                    return;
                }

                openFile = openFileDialog1.FileName;

                try
                {
                    chgCaption(openFile);
                    loadFile(openFile);
                    updateConcatenateConfigFile("LastASS", openFile);
                    putRecentASS();

                }
                catch (Exception x) { errorMsg("Error cargando el archivo '" + openFile + "'\n\n[ Mensaje ]\n\n" + x.Message); }
            }

        }

        private void SaveScript()
        {
            if ((type == ScriptFileType.TXT || type == ScriptFileType.SRT) || !openFile.ToLower().EndsWith(".ass"))
            {
                SaveScriptAs();
            }
            else
            {
                saveFile(openFile);
                updateConcatenateConfigFile("LastASS", openFile);
            }
        }

        private void SaveScriptAs()
        {
            updateConcatenateConfigFile("LastASS", openFile);

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "ASS files (*.ass)|*.ass";
            sfd.FilterIndex = 1;
            sfd.RestoreDirectory = true;
            try
            {
                sfd.InitialDirectory = getFromConfigFile("mainW_WorkDirectory");
            }
            catch
            {
                sfd.InitialDirectory = System.Environment.SpecialFolder.MyDocuments.ToString();
                updateReplaceConfigFile("mainW_WorkDirectory", sfd.InitialDirectory);

            }
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string fname = "";

                if (!sfd.FileName.EndsWith(".ass", StringComparison.InvariantCultureIgnoreCase))
                    fname = sfd.FileName + ".ass";
                else
                    fname = sfd.FileName;

                saveFile(fname);
                updateConcatenateConfigFile("LastASS", fname);
                openFile = fname;
                type = ScriptFileType.ASS_SSA;
            }
        }

        private void textoPlanoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Archivos de texto (*.txt;*.wri)|*.txt;*.wri";
            sfd.RestoreDirectory = true;
            try
            {
                sfd.InitialDirectory = getFromConfigFile("mainW_WorkDirectory");
            }
            catch
            {
                sfd.InitialDirectory = System.Environment.SpecialFolder.MyDocuments.ToString();
                updateReplaceConfigFile("mainW_WorkDirectory", sfd.InitialDirectory);

            }

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                TextWriter o = new StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8);
                foreach (lineaASS lass in al)
                    o.WriteLine(lineaASS.cleanText(lass.texto));
                o.Close();
            }

        }

        private void textoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Archivos de texto (*.txt;*.wri)|*.txt;*.wri";
            sfd.RestoreDirectory = true;
            try
            {
                sfd.InitialDirectory = getFromConfigFile("mainW_WorkDirectory");
            }
            catch
            {
                sfd.InitialDirectory = System.Environment.SpecialFolder.MyDocuments.ToString();
                updateReplaceConfigFile("mainW_WorkDirectory", sfd.InitialDirectory);

            }

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                TextWriter o = new StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8);
                foreach (lineaASS lass in al)
                    o.WriteLine((lass.personaje.Equals("") ? "nulo" : lass.personaje) + ": " + lass.texto);
                o.Close();
            }

        }

        private void archivoSRTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // por acabar

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Archivos SRT (*.srt)|*.srt";
            sfd.RestoreDirectory = true;
            try
            {
                sfd.InitialDirectory = getFromConfigFile("mainW_WorkDirectory");
            }
            catch
            {
                sfd.InitialDirectory = System.Environment.SpecialFolder.MyDocuments.ToString();
                updateReplaceConfigFile("mainW_WorkDirectory", sfd.InitialDirectory);

            }

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                TextWriter o = new StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8);
                int idx = 1;
                foreach (lineaASS lass in al)
                {
                    o.WriteLine(idx.ToString());
                    o.WriteLine(lass.t_inicial.ToString().Replace('.', ',') + "0 --> " + lass.t_final.ToString().Replace('.', ',') + "0");
                    o.WriteLine(lineaASS.cleanText(lass.texto));
                    o.WriteLine();
                    idx++;
                }
                o.Close();
            }
        }

        private void ShowAboutBox()
        {
            aboutW aW = new aboutW(this);
            aW.Show();
        }

        private void abrirASSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenLoadScript();
        }

        private void guardarCambiosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveScriptAs();
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void personakeEstiloToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IsModified = true;

            UndoRedo.AddUndo(script, "estilo -> personaje");

            for (int i = 0; i < gridASS.RowCount; i++)
            {
                lineaASS lass = (lineaASS)script.GetLines()[i];
                lass.estilo = lass.personaje;
            }

            updateGridWithArrayList(script.GetLines());


        }

        private void personajeComoEstiloToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IsModified = true;

            UndoRedo.AddUndo(script, "personaje -> estilo");

            for (int i = 0; i < gridASS.RowCount; i++)
            {
                lineaASS lass = (lineaASS)script.GetLines()[i];
                lass.personaje = lass.estilo;
            }

            updateGridWithArrayList(script.GetLines());
        }

        private void detectarColisionesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doDetectCollision();
        }

        private void detectarAlertasEnLosSubtítulosprofToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doDetectSubtitleProblems();
        }

        private void ConcatLines()
        {
            concatW cW = new concatW(this);
            cW.ShowDialog();
            IsModified = true;
        }

        private void concatenarLíneasPróximasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConcatLines();
        }

        private void acercaDeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowAboutBox();
        }


        private void putRecentASS()
        {
            try
            {
                string[] recent1 = getFromConfigFileA("LastASS");
                abrirRecienteToolStripMenuItem.DropDownItems.Clear();

                int maxxx = Math.Min(MaxRecentItems, recent1.Length);

                for (int i = 0; i < maxxx; i++)
                {
                    abrirRecienteToolStripMenuItem.DropDownItems.Add(recent1[i]);
                    abrirRecienteToolStripMenuItem.DropDownItems[i].Click += new System.EventHandler(openRecentASS);
                }

                abrirRecienteToolStripMenuItem.Enabled = true;

            }
            catch (Exception)
            {
                abrirRecienteToolStripMenuItem.Enabled = false;
            }



        }

        private void putRecentVID()
        {
            try
            {
                string[] recent1 = getFromConfigFileA("LastVID");
                abrirVideoRecienteMenuItem.DropDownItems.Clear();

                int maxxx = Math.Min(MaxRecentItems, recent1.Length);

                for (int i = 0; i < maxxx; i++)
                {
                    abrirVideoRecienteMenuItem.DropDownItems.Add(recent1[i]);
                    abrirVideoRecienteMenuItem.DropDownItems[i].Click += new System.EventHandler(openRecentVID);
                }

                updateMenuEnables();
            }
            catch (Exception)
            {
                abrirVideoRecienteMenuItem.Enabled = false;
            }

        }

        private void putRecentAUD()
        {
            try
            {
                string[] recent1 = getFromConfigFileA("LastAUD");

                abrirrecienteToolStripMenuItem2.DropDownItems.Clear();

                int maxxx = Math.Min(MaxRecentItems, recent1.Length);

                for (int i = 0; i < maxxx; i++)
                {
                    abrirrecienteToolStripMenuItem2.DropDownItems.Add(recent1[i]);
                    abrirrecienteToolStripMenuItem2.DropDownItems[i].Click += new System.EventHandler(openRecentAUD);
                }

                abrirrecienteToolStripMenuItem2.Enabled = true;

            }
            catch (Exception)
            {
                abrirrecienteToolStripMenuItem2.Enabled = false;
            }

        }

        delegate void SetTextCallback(string str);

        private void setStatus(string s)
        {
            if (this.statusStrip1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(setStatus);
                this.Invoke(d, new object[] { s });
            }
            else
            {
                this.currentStatusString.Text = s;
                StatusStripEventList.Push(s);

            }
        }


        private void openRecentASS(object sender, EventArgs e)
        {
            ToolStripMenuItem menu = sender as ToolStripMenuItem;
            
            if (openFile != null)
            {
                launchPerrySubParms("\"" + menu.Text + "\"");
                return; 
            }
            
            openFile = menu.Text;
            try
            {
                chgCaption(openFile);
                loadFile(openFile);
                updateConcatenateConfigFile("LastASS", openFile);
                updateMenuEnables();
            }
            catch (Exception x) { errorMsg("Error cargando el archivo '" + openFile + "'\n\n[ Mensaje ]\n\n" + x.Message); }
        }

        private void openRecentVID(object sender, EventArgs e)
        {
            ToolStripMenuItem menu = sender as ToolStripMenuItem;
            VideoBoxType = PreviewType.DirectShow;
            //openFileVideo = menu.Text;
            videoInfo = new VideoInfo(menu.Text);
            nextK.Visible = true;
            videoPanel.Visible = true;
            isVideoLoaded = true;
            videoPreviewAVS.Checked = previewToolStripMenuItem.Checked = false;

            openVid(videoInfo.FileName);
            try
            {
                PreviewType tipo = (PreviewType)int.Parse(getFromConfigFile("Video_DefaultPreviewType"));
                if ((tipo != VideoBoxType) && (tipo == PreviewType.AviSynth))
                {
                    SwitchToAVSMode();
                }
            }
            catch
            {
            }

            GoToVideoPosition();
            updateConcatenateConfigFile("LastVID", videoInfo.FileName);

            drawPositions();
            updateMenuEnables();
        }

        private void openRecentAUD(object sender, EventArgs e)
        {
            ToolStripMenuItem menu = sender as ToolStripMenuItem;

            try
            {
                setStatus("Creando el entorno AviSynth para el audio. Esta operación puede tardar unos segundos dependiendo del filtro de mayor prioridad definido en las opciones.");
                avsaudio = LoadAudioWithDependencies(avsaudio, menu.Text);

                isAudioLoaded = true;
                //panelAudioBox.Visible = true;
                loadAudio();
                updateMenuEnables();
                putRecentAUD();
                updateConcatenateConfigFile("LastAUD", menu.Text);
                script.GetHeader().SetHeaderValue("Audio File", menu.Text);
            }
            catch (Exception x)
            {
                errorMsg("Ha habido un error intentando cargar el audio '" + menu.Text + "'\nMensaje: " + x.Message);
                isAudioLoaded = false;
                panelAudioBox.Visible = false;
                this.Enabled = true;
            }
            // drawPositions(); 
        }

        private void LoadVideoFromFile(string FileName)
        {
            try
            {
                //openFileVideo = FileName;
                VideoBoxType = PreviewType.DirectShow;
                videoInfo = new VideoInfo(FileName);

                nextK.Visible = true;
                videoPanel.Visible = true;
                videoPreviewAVS.Checked = previewToolStripMenuItem.Checked = false;

                openVid(videoInfo.FileName);
                try
                {
                    PreviewType tipo = (PreviewType)int.Parse(getFromConfigFile("Video_DefaultPreviewType"));
                    if ((tipo != VideoBoxType) && (tipo == PreviewType.AviSynth))
                    {
                        SwitchToAVSMode();
                    }
                }
                catch
                {
                }
                GoToVideoPosition();

                updateConcatenateConfigFile("LastVID", videoInfo.FileName);
                putRecentVID();
            }
            catch
            {
                nextK.Visible = false;
                videoPanel.Visible = false;
            }
            drawPositions();
            updateMenuEnables();
        }

        private void toolStripMenuItem3_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = System.Environment.SpecialFolder.MyDocuments.ToString();
            openFileDialog1.Filter = String.Format("Archivos de vídeo ({0})|{1}", knownVideoExtensions, knownVideoExtensions);
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                LoadVideoFromFile(openFileDialog1.FileName);
            }
        }

        private void preferncasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            preferencesW p = new preferencesW(this);
            p.Show();
        }

        private void asociaciónDeExtensionesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fassociationsW fa = new fassociationsW(this);
            fa.ShowDialog();
        }

        private void restaurarPosiciónVentanaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int pHeight = Screen.PrimaryScreen.Bounds.Height;
            int pWidth = Screen.PrimaryScreen.Bounds.Width;
            int fHeight = 768;
            int fWidth = 1024;

            int LocX = ((pWidth / 2) - (fWidth / 2));
            int LocY = ((pHeight / 2) - (fHeight / 2));

            this.WindowState = FormWindowState.Normal;
            this.Size = new Size(1024, 768);
            this.Location = new Point(LocX, LocY);

        }
        #endregion

        #region BOTONES
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (!KeyframesAvailable) return;
            int anterior = 0;
            if (mediaControl != null) mediaControl.Pause();
            int cur = FrameIndex;
            foreach (int f in videoInfo.KeyFrames)
            {
                if (f >= cur) break;
                anterior = f;
            }
            FrameIndex = anterior;

        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            if (mediaControl != null) mediaControl.Pause();
            if (FrameIndex > 0)
            {
                seekBar.Value = FrameIndex--;
            }

            //FrameIndex = seekBar.Value;
            updateTextBoxWithFPS();


        }

        DateTime inicio_AVSPlay;
        int ms_per_frame_AVSPlay;
        int frame_inicio_AVSPlay;

        private void toolStripButton6_Click_1(object sender, EventArgs e)
        {
            VideoState = ReproductionState.Play;

            if (VideoBoxType == PreviewType.DirectShow) mediaControl.Run();
            else if (VideoBoxType == PreviewType.AviSynth)
            {
                //AVSPlayThread = new Thread(new ThreadStart(PlayThread));
                //AVSPlayThread.Start();
                if (!AVSPlayTimer.Enabled)
                {
                    ms_per_frame_AVSPlay = (int)Math.Floor(1000 / videoInfo.FrameRate); // *FramesToSkip;
                    AVSPlayTimer.Interval = ms_per_frame_AVSPlay;
                    AVSPlayTimer.Tick += new EventHandler(AVSPlayTimer_Tick);
                    inicio_AVSPlay = DateTime.Now;
                    frame_inicio_AVSPlay = FrameIndex;
                    //mediaControl.Run();
                    AVSPlayTimer.Enabled = true;

                }

            }

        }

        private void PlayVideoRange()
        {
            try
            {
                FrameIndex = int.Parse(framesInicio.Text); //(int) Math.Floor(lass.t_inicial.getTiempo() * fps);
                endSeek = int.Parse(framesFin.Text); //(long)Math.Floor(lass.t_final.getTiempo() * fps);
            }
            catch
            {
                return;
            }
            if ((endSeek - FrameIndex) <= 0) return;

            VideoState = ReproductionState.Play;

            if (VideoBoxType == PreviewType.DirectShow)
            {
                if (mediaSeeking == null) return;
                mediaControl.Run();
            }

            ms_per_frame_AVSPlay = (int)Math.Floor(1000 / videoInfo.FrameRate);
            inicio_AVSPlay = DateTime.Now;
            frame_inicio_AVSPlay = FrameIndex;

            PlayInRange.Interval = ms_per_frame_AVSPlay;
            PlayInRange.Tick += new EventHandler(PlayInRange_Tick);
            PlayInRange.Enabled = true;

        }

        private void toolStripButton7_Click_1(object sender, EventArgs e)
        {
            PlayVideoRange();
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            VideoState = ReproductionState.Pause;
            if (VideoBoxType == PreviewType.DirectShow) mediaControl.Pause();

        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            if (VideoBoxType == PreviewType.DirectShow) mediaControl.Pause();
            FrameIndex = 0;
            VideoState = ReproductionState.Stop;
            if (VideoBoxType == PreviewType.AviSynth)
                AVSPlayTimer.Enabled = false;//AVSPlayThread.Abort();

        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            if (mediaControl != null) mediaControl.Pause();
            FrameIndex = FrameIndex + 1;
            seekBar.Value = FrameIndex;
        }

        private void toolStripButton13_Click_1(object sender, EventArgs e)
        {
            if (mediaControl != null) mediaControl.Pause();
            FrameIndex = Convert.ToInt32(Math.Max(FrameIndex - 5, 0));
            seekBar.Value = FrameIndex;
        }

        private void toolStripButton14_Click(object sender, EventArgs e)
        {
            if (mediaControl != null) mediaControl.Pause();
            FrameIndex = Convert.ToInt32(Math.Min(FrameIndex + 5, FrameTotal));
            seekBar.Value = FrameIndex;
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            if (!KeyframesAvailable) return;
            long cur = 0;
            int anterior = 0;

            if (mediaControl != null) mediaControl.Pause();

            cur = FrameIndex; //VideoUnitConversion.getCurPos(mediaSeeking, videoInfo.FrameRate);
            foreach (int f in videoInfo.KeyFrames)
            {
                anterior = f;
                if (f > cur) break;
            }
            FrameIndex = anterior;

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            updateTextBoxWithFPS();
        }

        private void PlayFrameInAVS()
        {
            TimeSpan span = DateTime.Now - inicio_AVSPlay;

            int frameSupuesto = (int)Math.Ceiling((double)span.TotalMilliseconds / (double)ms_per_frame_AVSPlay);
            int frameActual = frame_inicio_AVSPlay + frameSupuesto;

            if ((FrameIndex <= FrameTotal) && (VideoState == ReproductionState.Play))
            {
                FrameIndex = frameActual;
            }
            else
            {
                VideoState = ReproductionState.Stop;
                AVSPlayTimer.Enabled = false;
            }

        }

        void AVSPlayTimer_Tick(object sender, EventArgs e)
        {
            PlayFrameInAVS();
        }

        private void cerrarVídeoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            closeVid();
            previewToolStripMenuItem.Checked = false;
        }

        #endregion

        #region MENUS

        private void AdjustToKeyFrame()
        {
            adjKeyW akw = new adjKeyW(this);
            akw.ShowDialog();
        }

        private void ajustarSubtítulosAKeyframesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AdjustToKeyFrame();
        }

        private void guardarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (type == ScriptFileType.TXT || type == ScriptFileType.SRT)
            {
                SaveScript();

            }
            else
            {
                saveFile(openFile);
                updateConcatenateConfigFile("LastASS", openFile);
            }
        }

        private void TranslationAssistant()
        {

            if (openFile.Equals(newFileName))
                SaveScriptAs();

            mediaControl.Pause();

            //oldOpenFileVideoFrame = FrameIndex;
            oldVideoInfo = videoInfo;
            oldVideoInfo.FrameIndex = FrameIndex; // ¿redundante?
            //oldOpenFileVideo = openFileVideo;
            closeVid();

            if (gridASS.Rows.Count <= 0) insertLineInRow(0);

            translateW tW = new translateW(this, oldVideoInfo);
            IsModified = true;
            tW.ShowDialog();
            //this.Enabled = false;

        }

        private void ShiftTimes()
        {
            shiftW shift = new shiftW(this);
            shift.ShowDialog();

        }

        private void MassStyler()
        {
            brauW sw = new brauW(this);
            IsModified = true;
            sw.ShowDialog();

        }

        private void V4Styler()
        {
            sbrowseW sbw = new sbrowseW(this);
            sbw.ShowDialog();
            IsModified = true;
        }

        private void EditHeader()
        {
            headersW hW = new headersW(this);
            hW.ShowDialog();
            IsModified = true;
        }

        private void asistenteDeTraducciónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TranslationAssistant();
        }

        private void massShiftTimesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ShiftTimes();
        }

        private void estiloNuloEstiloAnteriorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string anterior = "";
            IsModified = true;

            UndoRedo.AddUndo(script, "Estilos como anterior");

            for (int i = 0; i < gridASS.RowCount; i++)
            {
                lineaASS lass = (lineaASS)al[i];
                if (lass.personaje.Equals(""))
                {
                    gridASS["Personaje", i].Value = anterior;
                    lass.personaje = anterior;
                }
                anterior = lass.personaje;
            }


        }

        private void campoDeTextoSeparaPersonajeConToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IsModified = true;

            UndoRedo.AddUndo(script, "Separar con : personaje de texto");

            for (int i = 0; i < gridASS.RowCount; i++)
            {
                lineaASS lass = (lineaASS)al[i];
                if (lass.texto.IndexOf(':') != -1)
                {
                    string[] pjtexto = lass.texto.Split(new char[] { ':' }, 2);
                    lass.personaje = pjtexto[0].Trim();
                    lass.texto = pjtexto[1].Trim();
                }
            }
            updateGridWithArrayList(al);

        }

        private void resetearLasLayersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IsModified = true;
            UndoRedo.AddUndo(script, "Resetear layers");

            foreach (lineaASS lass in script.GetLines())
                lass.colision = 0;
            //updateGridWithArrayList(al);

        }

        private void ordenarLíneasPorTiemposToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IsModified = true;

            UndoRedo.AddUndo(script, "Ordenar líneas por tiempos");

            SubtitleScript copia = (SubtitleScript)script.Clone();
            copia.GetLineArrayList().GetFullArrayList().Sort();
            //copia.GetLineArrayList().GetFullArrayList().Reverse();
            script = copia;
            al = script.GetLines();
            updateGridWithArrayList(al);
        }

        private void massStylerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MassStyler();
        }

        private void editarEstilosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            V4Styler();
        }

        private void editarCabeceraASSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditHeader();
        }

        private void distinguirPersonajesPorColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Color[] cArray = { Color.PaleGoldenrod, Color.DarkSeaGreen, Color.Khaki,
                               Color.PaleTurquoise, Color.AliceBlue, Color.NavajoWhite };
            int p = 0;

            h = new Hashtable();

            for (int i = 0; i < gridASS.Rows.Count; i++)
            {

                if (p == cArray.Length) p = 0;

                lineaASS lass = (lineaASS)al[i];

                int hash = lass.personaje.GetHashCode();
                if (h.ContainsKey(hash))
                {
                    int col = (int)h[hash];
                    for (int x = 0; x < gridASS.Columns.Count; x++)
                        gridASS[x, i].Style.BackColor = cArray[col];
                }
                else
                {
                    for (int x = 0; x < gridASS.Columns.Count; x++)
                        gridASS[x, i].Style.BackColor = cArray[p];
                    h.Add(hash, p++);
                }
            }
        }

        private void volverAColoresNormalesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gridASS.Rows.Count; i++)
            {
                lineaASS lass = (lineaASS)al[i];
                for (int x = 0; x < gridASS.Columns.Count; x++)
                {
                    if (lass.clase.ToLower() == "comment")
                    {
                        gridASS[x, i].Style.ForeColor = CommentFore;
                        gridASS[x, i].Style.BackColor = CommentBack;
                    }
                    else
                    {
                        gridASS[x, i].Style.ForeColor = RowFore;
                        gridASS[x, i].Style.BackColor = RowBack;
                    }
                }
            }
        }

        private void gestiónDeNotasDeTraducciónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TranslationNotes();
        }


        private void TranslationNotes()
        {
            notasW nw = new notasW(this);
            nw.ShowDialog();
            IsModified = true;
        }

        private void MaxLines()
        {
            maxLinesW mlw = new maxLinesW(this);
            IsModified = true;
            mlw.ShowDialog();

        }

        private void AVSEditor()
        {
            avsW a = new avsW(this);
            a.ShowDialog();
        }

        private void BitrateCalc()
        {
            bitrateCalcW brc = (isVideoLoaded) ? new bitrateCalcW(this, videoInfo.FrameRate, FrameTotal) : new bitrateCalcW(this, -1, -1);
            brc.ShowDialog();
        }
        private void BatchGen()
        {
            batchGenW bw = new batchGenW(this);
            bw.ShowDialog();
        }

        private void SpellCheck()
        {
            SpellCheckW scw = new SpellCheckW(this);
            scw.ShowDialog();
        }

        private void SwitchToAVSMode()
        {
            videoPreviewAVS.Checked = !videoPreviewAVS.Checked;
            previewToolStripMenuItem.Checked = videoPreviewAVS.Checked;
            if (videoPreviewAVS.Checked)
            {
                openAVS(videoInfo.FileName);

            }
            else
                openVid(videoInfo.FileName, FrameIndex);
        }

        private void umbralDeLíneasMáximasEnPantallaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MaxLines();
        }

        private void asistenteParaElTimeoDeKanjisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            kanjisW k = new kanjisW(this);
            IsModified = true;
            k.ShowDialog();

        }
        private void alwaysOnTop_Click(object sender, EventArgs e)
        {
            alwaysOnTop.Checked = !alwaysOnTop.Checked;
            this.TopMost = alwaysOnTop.Checked;
        }

        private void asistenteDeFiltradoDeAviSynthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AVSEditor();
        }

        private void calculadoraDeBitrateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BitrateCalc();
        }

        private void interfazGráficoParaLaV1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            v1W v1 = new v1W(this);
            v1.ShowDialog();
        }

        private void nuevoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newFile();
        }

        private void cerrarScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            closeFile();
        }

        private void asdasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BatchGen();
        }

        private void modoKaraokeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            karaokeTimerW kt = new karaokeTimerW(this, avs2ds, avsaudio, AudioFull);
            kt.ShowDialog();
        }
        private void restaurarArchivoDelAutoGuardadoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoSaveW asw = new autoSaveW(this);
            asw.ShowDialog();
        }

        private void buscarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Find();
        }

        private void Find()
        {
            findW fw = new findW(this);
            fw.Show();
        }

        private void FindOthers()
        {
            findOthersW fow = new findOthersW(this);
            fow.Show();
        }

        private void Replace()
        {
            replaceW rw = new replaceW(this);
            rw.Show();
        }


        private void buscarSiguienteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doFindNext();
        }

        private void reemplazarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Replace();
        }

        private void buscarEnOtrosArchivosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FindOthers();
        }

        private void administrarTemplatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            templateW tw = new templateW(this);
            tw.ShowDialog();
        }

        private void verHistorialDeDeshacerRehacerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            undoRedoW urw = new undoRedoW(this);
            urw.ShowDialog();
        }
        private void AttachmentHandler()
        {
            attachmentsW aw = new attachmentsW(this);
            aw.ShowDialog();
        }

        private void FontRetriever()
        {
            FontExtractorW few = new FontExtractorW(this);
            few.ShowDialog();
        }
        private void adjuntarImágenesFuentesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AttachmentHandler();
        }

        private void administrarFuentesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontRetriever();
        }
        #endregion

        #region OTROS HANDLERS

        private void gridASS_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            switch (e.ColumnIndex)
            {
                case 1:
                    //MessageBox.Show(script.GetLines()[e.RowIndex].ToString());
                    string presx = script.GetHeader().GetHeaderValue("PlayResX");
                    string presy = script.GetHeader().GetHeaderValue("PlayResY");

                    int py = -1, px = -1;

                    if (presx.Equals(string.Empty) && presy.Equals(string.Empty)) return;
                    if (presx.Equals(string.Empty))
                    {
                        py = int.Parse(presy);
                        px = ((py * 4) / 3);
                    }
                    else if (presy.Equals(string.Empty))
                    {
                        px = int.Parse(presx);
                        py = ((px * 3) / 4);
                    }
                    else
                    {
                        px = int.Parse(presx);
                        py = int.Parse(presy);
                    }


                    try
                    {
                        AviSynthScriptEnvironment env = new AviSynthScriptEnvironment();
                        string script0rz = "blankclip(color=$000000,width=" + px + ",height=" + py + ")";
                        AviSynthClip avs = env.ParseScript(script0rz);

                        string vsf_path = AviSynthFunctions.getVSFilterPath(avs);
                        if (vsf_path != null)
                            avs.AviSynthInvoke(avs.GetAVS(), 0, "LoadPlugin", false, vsf_path);

                        // escribir ssa temporal

                        string PreviewFile = "PrevFile-l" + e.RowIndex + ".ass";

                        lineaASS actual = (lineaASS)script.GetLineArrayList().GetFullArrayList()[e.RowIndex];

                        TextWriter o = new StreamWriter(PreviewFile, false, System.Text.Encoding.UTF8);
                        o.WriteLine(headerMark);
                        o.WriteLine(script.GetHeader().ToString());

                        o.WriteLine(stylesMark);
                        foreach (estiloV4 e2 in script.GetStyles())
                        {
                            if (e2.Name.Equals(actual.estilo))
                            {
                                estiloV4 new_est = new estiloV4(e2.ToString());
                                new_est.PrimaryColor = "&H00FFFFFF";
                                new_est.SecondaryColor = "&H00FFFFFF";
                                new_est.OutlineColor = "&H00FFFFFF";
                                new_est.ShadowColor = "&H00FFFFFF";
                                o.WriteLine(new_est.ToString().Replace("\n", string.Empty));
                            }
                        }

                        o.WriteLine(); // ---

                        o.WriteLine(dialoguesMark);

                        actual.t_inicial.setTiempo(0);
                        actual.t_final.setTiempo(50);
                        o.WriteLine(actual.ToString());

                        o.Close();

                        avs.AviSynthInvoke(avs.GetAVS(), 0, "Eval", false, script0rz);
                        avs.AviSynthInvoke(avs.GetAVS(), 0, "TextSub", true, PreviewFile);
                        //LinePreviewBox.Image = (Image)AviSynthFunctions.getBitmapFromFrame(avs, 0, 0);
                        avs.cleanup(true);

                        File.Delete(PreviewFile);
                        //b = BitmapFunctions.Crop(b, 640, 480, 0, 376);                        
                    }
                    catch
                    {
                        errorMsg("Error cargando AviSynth.");
                    }

                    break;
            }
        }

        private void seekBar_Scroll(object sender, EventArgs e)
        {
            if (!isVideoLoaded) return;
            FrameIndex = seekBar.Value;
            updateTextBoxWithFPS();
        }

        private void butPlayR_Click(object sender, EventArgs e)
        {
        }

        void PlayInRange_Tick(object sender, EventArgs e)
        {
            if (VideoBoxType == PreviewType.AviSynth)
            {
                PlayFrameInAVS();
                //FrameBuffer.SetPanicMode(PanicModes.DoPanic);
                //videoPictureBox.Image = FrameBuffer.GetBitmap();

            }
            if (FrameIndex >= endSeek)
            {
                if (VideoBoxType == PreviewType.DirectShow) mediaControl.Pause();
                VideoState = ReproductionState.Stop;
                PlayInRange.Enabled = false;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {
                double p = double.Parse(vidScaleFactor.Text.Substring(0, vidScaleFactor.Text.IndexOf('%')));

                p = p / 100;

                int x = VideoWidth, y = VideoHeight;

                int new_x = (int)(x * p);
                int new_y = (int)(y * p);

                videoPanel.Size = new System.Drawing.Size(new_x, new_y);

                if (VideoBoxType == PreviewType.DirectShow)
                {
                    videoWindow.put_Height(new_x);
                    videoWindow.put_Width(new_y);
                    videoWindow.put_Owner(videoPanel.Handle);
                    videoWindow.SetWindowPosition(0, 0, videoPanel.Width, videoPanel.Height);
                    videoWindow.put_WindowStyle(WindowStyle.Child);
                    videoWindow.put_Visible(DirectShowLib.OABool.True);
                }

                drawPositions();

                if (VideoBoxType == PreviewType.AviSynth)
                    updatePreviewAVS();

                updateReplaceConfigFile("mainW_Zoom", vidScaleFactor.Text);
            }
            catch { errorMsg("Imposible cambiar el zoom, posiblemente porque hayas seleccionado un upsize mayor del soportado por tu resolución"); }

        }

        private void marcarColisionesDeLaLíneaSeleccionadaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            marcarColisionesDeLaLíneaSeleccionadaToolStripMenuItem.Checked = !marcarColisionesDeLaLíneaSeleccionadaToolStripMenuItem.Checked;
            ppCol.Checked = marcarColisionesDeLaLíneaSeleccionadaToolStripMenuItem.Checked;
            updateReplaceConfigFile("mainW_MarkCol", marcarColisionesDeLaLíneaSeleccionadaToolStripMenuItem.Checked.ToString());

            if (!marcarColisionesDeLaLíneaSeleccionadaToolStripMenuItem.Checked)
            {
                for (int i = 0; i < gridASS.Rows.Count; i++)
                {
                    lineaASS lass = (lineaASS)al[i];
                    for (int x = 0; x < gridASS.Columns.Count; x++)
                    {
                        if (lass.clase.ToLower() == "comment")
                        {
                            gridASS[x, i].Style.ForeColor = CommentFore;
                            gridASS[x, i].Style.BackColor = CommentBack;
                        }
                        else
                        {
                            gridASS[x, i].Style.ForeColor = RowFore;
                            gridASS[x, i].Style.BackColor = RowBack;
                        }
                    }
                }

            }
        }

        private void previewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            previewToolStripMenuItem.Checked = !previewToolStripMenuItem.Checked;
            videoPreviewAVS.Checked = previewToolStripMenuItem.Checked;

            if (previewToolStripMenuItem.Checked)
            {
                openAVS(videoInfo.FileName);

            }
            else
                openVid(videoInfo.FileName, FrameIndex);

        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            textFrame.Visible = radioButton1.Checked;
            textTime.Visible = radioButton2.Checked;
        }

        #endregion

        #region GRID CONTEXT MENU

        private void pastePartialCopy(string[] splitted, CopyStyle tipo)
        {

            int[] sel_rows = new int[gridASS.SelectedRows.Count];
            for (int i = 0; i < gridASS.SelectedRows.Count; i++) sel_rows[i] = gridASS.SelectedRows[i].Index;

            Array.Sort(sel_rows);

            if (splitted.Length == 1 && sel_rows.Length > 1)
            {
                // si 1 -> n , hacer n -> n
                string[] new_splitted = new string[sel_rows.Length];
                for (int i = 0; i < sel_rows.Length; i++)
                    new_splitted[i] = splitted[0];
                splitted = new_splitted;
            }

            int max_i = Math.Min(splitted.Length, sel_rows.Length);

            for (int i = 0; i < max_i; i++)
            {
                lineaASS actual = (lineaASS)al[sel_rows[i]];
                partialCopy(new lineaASS(splitted[i]), actual, tipo);
            }

            updateGridWithArrayList(al);

        }

        private void partialCopy(lineaASS lass, lineaASS actual, CopyStyle elque)
        {
            switch (elque)
            {
                case CopyStyle.TInicial:
                    actual.t_inicial = lass.t_inicial;
                    break;
                case CopyStyle.TFinal:
                    actual.t_final = lass.t_final;
                    break;
                case CopyStyle.Tiempo:
                    actual.t_inicial = lass.t_inicial;
                    actual.t_final = lass.t_final;
                    break;
                case CopyStyle.Estilo:
                    actual.estilo = lass.estilo;
                    break;
                case CopyStyle.Personaje:
                    actual.personaje = lass.personaje;
                    break;
                case CopyStyle.EstiloPersonaje:
                    actual.estilo = lass.estilo;
                    actual.personaje = lass.personaje;
                    break;
                case CopyStyle.SinTexto:
                    actual.estilo = lass.estilo;
                    actual.personaje = lass.personaje;
                    actual.t_inicial = lass.t_inicial;
                    actual.t_final = lass.t_final;
                    break;
            }
        }

        private void pasteClipToRow(int mode)
        {
            if (gridASS.SelectedRows.Count > 0)
            {
                UndoRedo.AddUndo(script, "Pegar"); //UndoStack.Push(al_clone);
                try
                {
                    ArrayList al_clone = PrepareForPush(al);
                    string[] l = Clipboard.GetText().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < l.Length; i++)
                    {
                        if (!l[i].Equals(String.Empty))
                        {
                            lineaASS lass = new lineaASS(l[i]);

                            lineaASS actual = (lineaASS)al[gridASS.SelectedRows[0].Index];

                            switch (lass.texto)
                            {
                                case "[Copiar Tiempo Inicial]":
                                    pastePartialCopy(l, CopyStyle.TInicial);
                                    return;
                                case "[Copiar Tiempo Final]":
                                    pastePartialCopy(l, CopyStyle.TFinal);
                                    return;
                                case "[Copiar Tiempo]":
                                    pastePartialCopy(l, CopyStyle.Tiempo);
                                    return;
                                case "[Copiar Estilo]":
                                    pastePartialCopy(l, CopyStyle.Estilo);
                                    return;
                                case "[Copiar Personaje]":
                                    pastePartialCopy(l, CopyStyle.Personaje);
                                    return;
                                case "[Copiar Estilo y Personaje]":
                                    pastePartialCopy(l, CopyStyle.EstiloPersonaje);
                                    return;
                                case "[Copiar Todo menos Texto]":
                                    pastePartialCopy(l, CopyStyle.SinTexto);
                                    return;
                                default:
                                    al.Insert(gridASS.SelectedRows[0].Index + mode + i, lass);
                                    break;
                            }
                        }
                    }
                    updateGridWithArrayList(al);
                }
                catch { }
            }

        }

        private void copyRowToClip(bool isCut, CopyStyle style)
        {
            if (gridASS.SelectedRows.Count > 0)
            {

                string res = "";
                int[] ia = new int[gridASS.SelectedRows.Count];
                if (isCut) UndoRedo.AddUndo(script, "Cortar/Borrar");//UndoStack.Push(PrepareForPush(al));
                for (int i = 0; i < gridASS.SelectedRows.Count; i++)
                    ia[i] = gridASS.SelectedRows[i].Index;

                Array.Sort(ia);

                for (int i = 0; i < ia.Length; i++)
                {
                    lineaASS l = (lineaASS)al[ia[i]];
                    lineaASS lass = (lineaASS)l.Clone();

                    switch (style)
                    {
                        case CopyStyle.TInicial:
                            lass.texto = "[Copiar Tiempo Inicial]";
                            break;
                        case CopyStyle.TFinal:
                            lass.texto = "[Copiar Tiempo Final]";
                            break;
                        case CopyStyle.Tiempo:
                            lass.texto = "[Copiar Tiempo]";
                            break;
                        case CopyStyle.Estilo:
                            lass.texto = "[Copiar Estilo]";
                            break;
                        case CopyStyle.Personaje:
                            lass.texto = "[Copiar Personaje]";
                            break;
                        case CopyStyle.EstiloPersonaje:
                            lass.texto = "[Copiar Estilo y Personaje]";
                            break;
                        case CopyStyle.SinTexto:
                            lass.texto = "[Copiar Todo menos Texto]";
                            break;
                        case CopyStyle.Normal:
                        case CopyStyle.SoloTexto:
                            //lass.texto = lass.texto; // <- is dis perogrullo? xD
                            break;
                    }

                    if (style == CopyStyle.SoloTexto)
                        res += lass.texto + "\n";
                    else
                        res += lass.ToString() + "\n";
                }

                res = res.TrimEnd(new char[] { '\n' });

                try
                {
                    Clipboard.SetText(res);
                }
                catch { }

                if (isCut)
                {
                    Array.Reverse(ia);
                    for (int i = 0; i < ia.Length; i++)
                        al.RemoveAt(ia[i]);
                    updateGridWithArrayList(al);
                    gridASS.RowCount = al.Count;
                }
            }
        }

        public void InsertNewLine(int mode, int line)
        {
            clearSelectedRows();
            selectRow(line);
            insertLineInRow(mode);
        }

        private void insertLineInRow(int mode)
        {
            if (gridASS.Rows.Count > 0)
            {
                if (gridASS.SelectedRows.Count > 0)
                {
                    UndoRedo.AddUndo(script, "Insertar línea"); //UndoStack.Push(PrepareForPush(al));
                    lineaASS lass = new lineaASS();
                    al.Insert(gridASS.SelectedRows[0].Index + mode, lass);
                    updateGridWithArrayList(al);
                    updatePanelTextTools();
                }
            }
            else
            {
                al.Add(new lineaASS());
                updateGridWithArrayList(al);
            }

        }

        private void Lunarize()
        {
            if (gridASS.SelectedRows.Count < 2) return;
            UndoRedo.AddUndo(script, "Líneas por partes"); //UndoStack.Push(PrepareForPush(al));

            int[] ia = new int[gridASS.SelectedRows.Count];

            for (int i = 0; i < gridASS.SelectedRows.Count; i++)
                ia[i] = gridASS.SelectedRows[i].Index;

            Array.Sort(ia);

            string[] str = new string[gridASS.SelectedRows.Count];

            lineaASS ant = null;

            for (int i = 0; i < ia.Length; i++)
            {
                lineaASS lass = (lineaASS)al[ia[i]];
                str[i] = lass.texto;
                if (ant != null)
                    ant.t_final.setTiempo(lass.t_inicial.getTiempo());
                ant = lass;
            }

            for (int i = 0; i < ia.Length; i++)
            {
                lineaASS lass = (lineaASS)al[ia[i]];
                string s = "";
                for (int x = 0; x < str.Length; x++)
                {
                    if (x == i + 1) s = s + "{\\alpha&HFF&}";
                    s = s + " " + str[x];
                }
                lass.texto = s.Trim();
            }

            updateGridWithArrayList(al);

        }
        private void JoinSubtitles()
        {
            if (gridASS.SelectedRows.Count < 2) return;

            UndoRedo.AddUndo(script, "Unir dos líneas de subtítulos"); //UndoStack.Push(PrepareForPush(al));

            // saber tiempo inicial menor y tiempo final mayor (+su indice)

            int idx_final = gridASS.SelectedRows[0].Index;
            int idx_tinicial_menor = idx_final, idx_tfinal_mayor = idx_final;
            ArrayList SelectedLines = new ArrayList();
            ArrayList SelectedLinesIndices = new ArrayList();

            lineaASS ini_menor = null;
            lineaASS fin_mayor = null;

            for (int i = 0; i < gridASS.SelectedRows.Count; i++)
            {
                int actual = gridASS.SelectedRows[i].Index;
                lineaASS act = (lineaASS)al[actual];
                SelectedLines.Add(act);
                SelectedLinesIndices.Add(actual);
                ini_menor = (lineaASS)al[idx_tinicial_menor];
                fin_mayor = (lineaASS)al[idx_tfinal_mayor];

                idx_tinicial_menor = (act.t_inicial.getTiempo() < ini_menor.t_inicial.getTiempo()) ? actual : idx_tinicial_menor;
                idx_tfinal_mayor = (act.t_final.getTiempo() > fin_mayor.t_final.getTiempo()) ? actual : idx_tfinal_mayor;
            }

            ini_menor = (lineaASS)al[idx_tinicial_menor];
            fin_mayor = (lineaASS)al[idx_tfinal_mayor];

            // unir los subtitulos segun su tiempo final
            //SelectedLines.Sort();

            SelectedLinesIndices.Sort(); // * 
            idx_final = (int)SelectedLinesIndices[0]; // *


            lineaASS final = (lineaASS)script.GetLines()[idx_final];
            final.t_inicial.setTiempo(ini_menor.t_inicial.getTiempo());
            final.t_final.setTiempo(fin_mayor.t_final.getTiempo());

            string res = "";
            foreach (int i in SelectedLinesIndices)
            {
                lineaASS lass = (lineaASS)script.GetLines()[i];
                res += lass.texto + " ";
            }
            final.texto = res;

            // borrar las lineas de mayor a menor, menos la actual, donde se insertara todo el mochuelo

            al.RemoveRange(idx_final + 1, SelectedLines.Count - 1);

            updateGridWithArrayList(al);
            updatePanelTextTools();

            foreach (int i in SelectedLinesIndices)
            {
                if (i != idx_final)
                    gridASS.Rows[i].Selected = false;
            }

        }

        public void DeleteSelectedLine(int line)
        {
            clearSelectedRows();
            selectRow(line);
            DeleteSelectedLines();
        }

        private void DeleteSelectedLines()
        {
            bool mec = marcarColisionesDeLaLíneaSeleccionadaToolStripMenuItem.Checked;
            marcarColisionesDeLaLíneaSeleccionadaToolStripMenuItem.Checked = false;

            string old_clip = "";
            try
            {
                old_clip = Clipboard.GetText();
                copyRowToClip(true, CopyStyle.Normal);
            }
            catch { }

            int bleh = gridASS.RowCount;
            for (int i = 0; i < gridASS.SelectedRows.Count; i++)
            {
                if ((gridASS.SelectedRows[i].Index) < bleh)
                    bleh = gridASS.SelectedRows[i].Index;
            }

            int last = (gridASS.RowCount > 1) ? bleh : -1;

            for (int i = 0; i < gridASS.Rows.Count - 1; i++)
                gridASS.Rows[i].Selected = false;

            try { Clipboard.SetText(old_clip); }
            catch { }

            if (last != -1)
                if (last < gridASS.RowCount)
                    gridASS.Rows[last].Selected = true;
                else
                    gridASS.Rows[gridASS.RowCount - 1].Selected = true;

            marcarColisionesDeLaLíneaSeleccionadaToolStripMenuItem.Checked = mec;
        }
        #endregion

        #region GRID CONTEXT MENU v2

        private void pegarDesdeArchivoDeTextoPlanoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UndoRedo.AddUndo(script, "Pegar desde texto plano");
            try
            {
                string cb = Clipboard.GetText();
                string[] bleh = cb.Split(new char[] { '\n' });
                int limit = Math.Min(bleh.Length, gridASS.SelectedRows.Count);
                for (int i = 0; i < limit; i++)
                {
                    int roflmao = gridASS.SelectedRows[gridASS.SelectedRows.Count - 1 - i].Index;
                    lineaASS lass = (lineaASS)script.GetLines()[roflmao];
                    lass.texto = bleh[i];
                }

                updateGridWithArrayList(al);
                updatePanelTextTools();
            }
            catch { }
        }


        private void toolStripMenuItem20_Click(object sender, EventArgs e)
        {
            copyRowToClip(false, CopyStyle.Normal);
        }

        private void despuésDeLaFilaSeleccionadaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pasteClipToRow(1);
        }

        private void antesDeLaSeleccionadaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pasteClipToRow(0);
        }

        private void cortarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool mec = marcarColisionesDeLaLíneaSeleccionadaToolStripMenuItem.Checked;
            marcarColisionesDeLaLíneaSeleccionadaToolStripMenuItem.Checked = false;

            copyRowToClip(true, CopyStyle.Normal);
            marcarColisionesDeLaLíneaSeleccionadaToolStripMenuItem.Checked = mec;

        }

        private void duplicarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UndoRedo.AddUndo(script, "Duplicar línea");

            bool mec = marcarColisionesDeLaLíneaSeleccionadaToolStripMenuItem.Checked;
            marcarColisionesDeLaLíneaSeleccionadaToolStripMenuItem.Checked = false;

            string old_clip = Clipboard.GetText();
            copyRowToClip(false, CopyStyle.Normal);
            pasteClipToRow(0);
            try { Clipboard.SetText(old_clip); }
            catch { }

            marcarColisionesDeLaLíneaSeleccionadaToolStripMenuItem.Checked = mec;
        }

        private void borrarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedLines();
        }

        private void delanteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertLineInRow(0);
        }

        private void detrásToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertLineInRow(1);
        }

        private void deshacerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CallUndo();
        }


        private void rehacerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CallRedo();
        }

        private void lunarizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Lunarize();
        }


        private void ponerTiempoInicialConElTiempoDelFrameActualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!isVideoLoaded)
            {
                errorMsg("Se necesita un vídeo cargado para poder usar esta opción");
                return;
            }

            double d = (int.Parse(textCurrentFrames.Text) / videoInfo.FrameRate);
            UndoRedo.AddUndo(script, "Poner tiempo inicial al frame actual"); //UndoStack.Push(PrepareForPush(al));
            for (int i = 0; i < gridASS.SelectedRows.Count; i++)
            {
                lineaASS lass = (lineaASS)al[gridASS.SelectedRows[i].Index];
                lass.t_inicial.setTiempo(d - 0.01);
            }
            //updateLineaASSwithPanelTextTools(lass);
            updateGridWithArrayList(al);
            updatePanelTextTools();
        }

        private void tiempoFinalConElTiempoDelFrameActualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!isVideoLoaded)
            {
                errorMsg("Se necesita un vídeo cargado para poder usar esta opción");
                return;
            }
            double d = (int.Parse(textCurrentFrames.Text) / videoInfo.FrameRate);
            UndoRedo.AddUndo(script, "Poner tiempo final al frame actual"); //UndoStack.Push(PrepareForPush(al));
            for (int i = 0; i < gridASS.SelectedRows.Count; i++)
            {
                lineaASS lass = (lineaASS)al[gridASS.SelectedRows[i].Index];
                lass.t_final.setTiempo(d + 0.01);
            }
            //updateLineaASSwithPanelTextTools(lass);            
            updateGridWithArrayList(al);
            updatePanelTextTools();
        }

        private void moverVídeoAPrincipioDeLaLíneaActualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrameIndex = FrameIndex = Convert.ToInt32(framesInicio.Text);
            if (VideoBoxType == PreviewType.AviSynth)
                updatePreviewAVS();

        }

        private void moverVídeoAlFinalprincipioDeLaLíneaActualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrameIndex = Convert.ToInt32(framesFin.Text) - 1;
            if (VideoBoxType == PreviewType.AviSynth)
                updatePreviewAVS();

        }

        private void ajustarLíneaALosKeyFramesMásCercanosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UndoRedo.AddUndo(script, "Ajustar tiempos de la escena a los keyframes más cercanos"); //UndoStack.Push(PrepareForPush(al));
            int ant, pos;
            for (int i = 0; i < gridASS.SelectedRows.Count; i++)
            {
                lineaASS lass = (lineaASS)al[gridASS.SelectedRows[i].Index];
                lass = adjToKeyF(lass, 0, 0, 0, 0, v4, out ant, out pos);
                double a = (double)((double)ant / (double)videoInfo.FrameRate);
                double p = (double)((double)pos / (double)videoInfo.FrameRate);
                lass.t_inicial.setTiempo(a - 0.01);
                lass.t_final.setTiempo(p + 0.01);
                lass = adjToKeyF(lass, 2, 2, 2, 2, v4, out ant, out pos);
            }
            updateGridWithArrayList(al);
        }


        private void tiempoInicialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copyRowToClip(false, CopyStyle.TInicial);
        }

        private void tiempoFinalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copyRowToClip(false, CopyStyle.TFinal);
        }

        private void tiempoInicialTiempoFinalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copyRowToClip(false, CopyStyle.Tiempo);
        }

        private void estiloToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copyRowToClip(false, CopyStyle.Estilo);
        }

        private void personajeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copyRowToClip(false, CopyStyle.Personaje);
        }

        private void estiloPersonajeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copyRowToClip(false, CopyStyle.EstiloPersonaje);
        }

        private void todoMenosElTextoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copyRowToClip(false, CopyStyle.SinTexto);
        }

        private void sóloTextoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copyRowToClip(false, CopyStyle.SoloTexto);
        }

        private void unirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            JoinSubtitles();
        }

        private void limpiarTiemposToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UndoRedo.AddUndo(script, "Limpiar tiempos");
            for (int i = 0; i < gridASS.SelectedRows.Count; i++)
            {
                int idx = gridASS.SelectedRows[i].Index;
                lineaASS lass = (lineaASS)al[idx];
                lass.t_inicial.setTiempo(0);
                lass.t_final.setTiempo(0);
            }
            updateGridWithArrayList(al);
        }


        private void despuésEnOrdenYTiempoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gridASS.Rows.Count <= 0) return;

            insertLineInRow(1);
            int idx = gridASS.SelectedRows[0].Index;
            gridASS.Rows[idx + 1].Selected = true;
            gridASS.Rows[idx].Selected = false;

            lineaASS lass_old = (lineaASS)al[idx];
            lineaASS lass_new = (lineaASS)al[idx + 1];
            lass_new.t_inicial.setTiempo(lass_old.t_final.getTiempo());
            lass_new.t_final.setTiempo(lass_old.t_final.getTiempo());

            updateGridWithArrayList(al);
            updatePanelTextTools();

        }

        private void vídeoPlayPauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MembSub_playPause();
        }

        #endregion

        #region MENU CONTEXTUAL VIDEO

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            contextColoring.Show(panelTextModifiers, new Point(0, pxColorButton.Height));
        }


        private void guardarImágenComoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "frame" + FrameIndex + ".png";
            sfd.Filter = "Archivo PNG (*.png)|*.png";
            sfd.FilterIndex = 1;
            sfd.RestoreDirectory = true;
            sfd.InitialDirectory = System.Environment.SpecialFolder.MyDocuments.ToString();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                videoPictureBox.Image.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }

        }

        private void copiarColorDelPíxelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pxColorCopy.Checked = !pxColorCopy.Checked;
            pxColorButton.Enabled = pxColorCopy.Checked;
        }

        private void contextColoringMenu(object sender, EventArgs e)
        {
            ToolStripItem t = (ToolStripItem)sender;

            Color c = pxColorButton.BackColor;
            selectedLine.Focus();

            string r = Convert.ToString(c.R, 16).ToUpper();
            if (r.Length == 1) r = "0" + r;
            string g = Convert.ToString(c.G, 16).ToUpper();
            if (g.Length == 1) g = "0" + g;
            string b = Convert.ToString(c.B, 16).ToUpper();
            if (b.Length == 1) b = "0" + b;

            string res = t.Text.Substring(0, 2) + "&H" + b + g + r + "&";
            addTagSSA(res, "");
            //selectedLine.Text = selectedLine.Text.Substring(0, selectedLine.SelectionStart) + res + selectedLine.Text.Substring(selectedLine.SelectionStart);
        }


        void fscy_value_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (Convert.ToInt32(e.KeyChar))
            {
                case 13:
                    addTagSSA("fscy" + fscy_value.Text, "fscy100");
                    SendKeys.Send("");
                    e.Handled = true;
                    fscx.Close();
                    break;
            }
        }

        void fscx_value_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (Convert.ToInt32(e.KeyChar))
            {
                case 13:
                    addTagSSA("fscx" + fscx_value.Text, "fscx100");
                    SendKeys.Send("");
                    e.Handled = true;
                    fscx.Close();
                    break;
            }
        }


        void ssaTag_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (Convert.ToInt32(e.KeyChar))
            {
                case 13:

                    for (int i = 0; i < gridASS.SelectedRows.Count; i++)
                    {
                        lineaASS lass = (lineaASS)al[gridASS.SelectedRows[i].Index];
                        lass.texto = lineaASS.insertTag(lass.texto, ssaTag.Text, 0);
                        refreshGrid();
                        updatePanelTextTools();
                    }
                    SendKeys.Send("");
                    e.Handled = true;
                    updateGridWithArrayList(al);
                    contextGridASS.Close();
                    break;
            }
        }



        #endregion

        #region ESTILOS DE TEXTO

        private void addTagSSA(string tag_open, string tag_close)
        {

            int sel_start = selectedLine.SelectionStart;
            int sel_length = selectedLine.SelectionLength;

            if (!tag_close.Equals(String.Empty))
            {
                if (sel_length > 0)
                {
                    selectedLine.Text = lineaASS.insertTag(selectedLine.Text, tag_close, sel_start + sel_length);
                }
            }

            selectedLine.Text = lineaASS.insertTag(selectedLine.Text, tag_open, sel_start);
            selectedLine.SelectionStart = sel_start;

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

                //string antes = selectedLine.Text.Substring(0, selectedLine.SelectionStart);
                //string despues = selectedLine.Text.Substring(selectedLine.SelectionStart, selectedLine.Text.Length - selectedLine.SelectionStart);
                //selectedLine.Text = antes + "{\\fn" + fd.Font.Name + "}" + despues;
            }
        }
        #endregion

        #region CONTEXT MENU ESTILOS SUBS

        private void Bold_Click(object sender, EventArgs e)
        {
            addBold();
            selectedLine.Focus();
        }

        private void Cursiva_Click(object sender, EventArgs e)
        {
            addItalic();
            selectedLine.Focus();

        }

        private void Subrayado_Click(object sender, EventArgs e)
        {
            addUnderline();
            selectedLine.Focus();

        }

        private void Fuente_Click(object sender, EventArgs e)
        {
            addFont();
            selectedLine.Focus();

        }

        private void Escala_Click(object sender, EventArgs e)
        {
            fscx_value.KeyPress += new KeyPressEventHandler(fscx_value_KeyPress);
            fscy_value.KeyPress += new KeyPressEventHandler(fscy_value_KeyPress);

            fscx.Show(panelTextModifiers, new Point(0, Escala.Height));

        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            clipperActive = !clipperActive;
            butClip.Checked = clipperActive;


        }

        private void butClip_Click(object sender, EventArgs e)
        {
            clipperActive = !clipperActive;

            butClip.Checked = clipperActive;

        }


        #endregion

        #region POSICIONES DE LOS PANELES (FUNCION VIL)
        public void drawPositions()
        {
            panelAudioBox.Visible = isAudioLoaded;

            if (videoPanel.Visible)
            {
                panelTools.Visible = vidScaleFactor.Enabled = panelTools2.Visible = true;
                //panelTools2.Visible = verVideoTools.Checked;

                panelTools.Location = new Point(videoPanel.Location.X, videoPanel.Location.Y + videoPanel.Height);

                int new_pttools_X = Math.Max(videoPanel.Location.X + videoPanel.Width + 5, panelTools.Location.X + panelTools.Width);

                panelAudioBox.Location = new Point(new_pttools_X, videoPanel.Location.Y);

                //int new_pttools_X = Math.Max(videoPanel.Location.X + videoPanel.Width + 5, panelTools.Location.X + panelTools.Width);
                int new_pttools_Y = (isAudioLoaded) ? panelAudioBox.Location.Y + panelAudioBox.Height + 5 : videoPanel.Location.Y;
                panelTextTools.Location = new Point(new_pttools_X, new_pttools_Y);
                panelTextTools.Visible = true; //verTextTools.Checked;


                /*int new_ptools2_Y = (verTextTools.Checked) ?
                    panelTextTools.Location.Y + panelTextTools.Height :
                    new_pttools_Y;*/
                int new_ptools2_Y = panelTextTools.Location.Y + panelTextTools.Height;

                int new_ptools2_X = Math.Max(panelTextTools.Location.X, panelTools.Location.X + panelTools.Width);
                panelTools2.Location = new Point(new_ptools2_X, new_ptools2_Y);
                /*
                int new_ppal_Y = (verVideoTools.Checked) ? Math.Max(panelTools2.Location.Y + panelTools2.Height, panelTools.Location.Y + panelTools.Height) :
                    (verTextTools.Checked) ? Math.Max(panelTextTools.Location.Y + panelTextTools.Height, panelTools.Location.Y + panelTools.Height) :
                                             Math.Max(videoPanel.Location.Y + panelTextTools.Height, panelTools.Location.Y + panelTools.Height);
                  */
                int new_ppal_Y = Math.Max(panelTools2.Location.Y + panelTools2.Height, panelTools.Location.Y + panelTools.Height);
                panelPrincipal.Location = new Point(panelPrincipal.Location.X, new_ppal_Y);

            }
            else
            {
                panelTools.Visible = vidScaleFactor.Enabled = panelTools2.Visible = false;

                panelAudioBox.Location = new Point(videoPanel.Location.X, videoPanel.Location.Y);

                int new_pttools_Y = (isAudioLoaded) ? panelAudioBox.Location.Y + panelAudioBox.Height + 5 : videoPanel.Location.Y;

                panelTextTools.Location = new Point(videoPanel.Location.X, new_pttools_Y);
                panelTextTools.Visible = /*verTextTools.Checked && */openFile != null;
                panelPrincipal.Location = /*(verTextTools.Checked) ? */
                    new Point(panelPrincipal.Location.X, panelTextTools.Location.Y + panelTextTools.Height) /*:
                    new Point(videoPanel.Location.X, new_pttools_Y)*/
                                                                     ;
            }

            panelPrincipal.Size = new System.Drawing.Size(this.Width - 8, (this.Height) - panelPrincipal.Location.Y - 30);
            tabGrid.Size = new Size(panelPrincipal.Size.Width, panelPrincipal.Size.Height - tabGrid.ItemSize.Height);
            //panelTools2.Width = Math.Min(panelTools2.Width, 500);
            //tabControl1.Width = Math.Min(panelTools2.Width,500);

            //assTextBoxRegEx1.Width = assTextBoxRegEx2.Width = tabControl1.Width-2;

            panelTextTools.Size = new Size(this.Width - panelTextTools.Location.X - 10, panelTextTools.Height);
            selectedLine.Size = new Size(panelTextTools.Width, selectedLine.Height);
            panelAudioBox.Width = panelTextTools.Width;
            AudioGridBox.Width = panelTextTools.Width;
            hSliderBar.Width = panelTextTools.Width;

        }
        #endregion

        #region MAIN TOOLBAR

        private void nuevoToolStripButton_Click(object sender, EventArgs e)
        {
            newFile();
        }

        private void abrirToolStripButton_Click(object sender, EventArgs e)
        {
            OpenLoadScript();
        }

        private void guardarToolStripButton_Click(object sender, EventArgs e)
        {
            SaveScript();
        }

        private void toolStripButton15_Click(object sender, EventArgs e)
        {
            MkvMagic();
        }

        private void editTrans_Click(object sender, EventArgs e)
        {
            TranslationAssistant();
        }

        private void editShift_Click(object sender, EventArgs e)
        {
            ShiftTimes();
        }

        private void editMassStyler_Click(object sender, EventArgs e)
        {
            MassStyler();
        }

        private void editStyles_Click(object sender, EventArgs e)
        {
            V4Styler();
        }

        private void videoPreviewAVS_Click(object sender, EventArgs e)
        {
            SwitchToAVSMode();
        }

        private void videoAVSEdit_Click(object sender, EventArgs e)
        {
            AVSEditor();
        }

        private void videoCalc_Click(object sender, EventArgs e)
        {
            BitrateCalc();
        }

        private void videoMemb_Click(object sender, EventArgs e)
        {
            videoMemb.Checked = !videoMemb.Checked;
            sincronizarDeVídeoMembSubToolStripMenuItem.Checked = videoMemb.Checked;

            ActivateMembSub();
        }

        private void ppConcat_Click(object sender, EventArgs e)
        {
            ConcatLines();
        }

        private void ppAdjustKeyF_Click(object sender, EventArgs e)
        {
            AdjustToKeyFrame();
        }

        private void ppMaxLines_Click(object sender, EventArgs e)
        {
            MaxLines();
        }

        private void ayudaToolStripButton_Click(object sender, EventArgs e)
        {
            ShowAboutBox();
        }

        private void toolStripButton7_Click_2(object sender, EventArgs e)
        {
            BatchGen();
        }

        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            TranslationNotes();
        }

        private void editHeaders_Click(object sender, EventArgs e)
        {
            EditHeader();
        }
        private void editFind_Click(object sender, EventArgs e)
        {
            Find();
        }

        private void editFindNext_Click(object sender, EventArgs e)
        {
            doFindNext();
        }


        private void toolStripButton16_Click(object sender, EventArgs e)
        {
            FindOthers();
        }

        private void editReplace_Click(object sender, EventArgs e)
        {
            Replace();
        }
        private void editAttach_Click(object sender, EventArgs e)
        {
            AttachmentHandler();
        }
        private void editAdminFont_Click(object sender, EventArgs e)
        {
            FontRetriever();
        }

        private void correctorOrtográficoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpellCheck();
        }

        private void editSpellCheck_Click(object sender, EventArgs e)
        {
            SpellCheck();
        }

        #endregion

        #region TEMPLATES

        private void TemplateName_Click(object sender, EventArgs e)
        {
            RefreshTemplateComboBox();
        }

        private void RefreshTemplateComboBox()
        {
            TemplateName.Items.Clear();
            TemplateName.Items.Add("(ninguno)");
            DirectoryInfo d = new DirectoryInfo(templateDir);
            foreach (FileInfo f in d.GetFiles("*.template"))
            {
                string s = f.Name.Substring(0, f.Name.LastIndexOf(".template"));
                TemplateName.Items.Add(s);
            }
        }

        private void templateBtn_Click(object sender, EventArgs e)
        {
            templateW tw = new templateW(this);
            tw.ShowDialog();
        }

        void TemplateName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TemplateName.SelectedIndex < 1) return;
            headerASS tmp = new headerASS();
            tmp.LoadFrom(templateDir + "\\" + TemplateName.Text + ".template");
            foreach (string s in tmp.GetHeaderList())
            {
                int idx = s.IndexOf(':');
                if (idx > 0)
                {
                    string k = s.Substring(0, idx);
                    string v = s.Substring(idx + 2);
                    script.GetHeader().SetHeaderValue(k, v);
                }
            }

            if (tmp.ExistsHeaderValue("Last Style Storage"))
            {
                StreamReader sr = null;
                try
                {
                    string fname = stylesDir + "\\" + tmp.GetHeaderValue("Last Style Storage") + ".Styles";
                    sr = FileAccessWrapper.OpenTextFile(fname);

                    string linea;
                    while ((linea = sr.ReadLine()) != null)
                    {
                        estiloV4 v = new estiloV4(linea);

                        // si existen -->   == -> no añadir
                        //                  != -> preguntar (si/no/si a todo)
                        bool existe = false;
                        foreach (estiloV4 est in script.GetStyles())
                        {
                            if (est.Name == v.Name)
                                existe = true;
                        }
                        if (!existe)
                            script.GetStyles().Insert(0,v);
                    }

                    sr.Close();
                }
                catch { }
            }
        }

        #endregion

        #region MKV MAGIC

        Process MKVProc;
        ActionProgressW MKVProgress;
        string MKVLine, MKVAssName, MKVVideoName;
        Thread MKVAnalysis, MKVOutput;

        private void MkvMagic()
        {
            if (!(File.Exists(Application.StartupPath + "\\MediaInfo.dll") && File.Exists(Application.StartupPath + "\\MediaInfoWrapper.dll")))
                errorMsg("No encontrado 'MediaInfo.dll' o 'MediaInfoWrapper.dll'. Opción deshabilitada");

            if (!File.Exists(Application.StartupPath + "\\mkvextract.exe"))
                errorMsg("No encontrado 'MKVextract.exe'. Opción deshabilitada");

            if (openFile != null)
            {
                if (MessageBox.Show("Esto abrirá un archivo de script nuevo.\n¿Seguro que deseas continuar?", appTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
            }

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Archivos MKV (*.mkv)|*.mkv";
            ofd.Multiselect = false;
            ofd.CheckFileExists = true;
            try
            {
                ofd.InitialDirectory = getFromConfigFile("mainW_WorkDirectory");
            }
            catch
            {
                ofd.InitialDirectory = System.Environment.SpecialFolder.MyDocuments.ToString();
                updateReplaceConfigFile("mainW_WorkDirectory", ofd.InitialDirectory);

            }
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                ArrayList tracks = GetSubtitleTrackID(ofd.FileName);
                MKVVideoName = ofd.FileName;
                if (tracks == null) errorMsg("Este archivo no contiene una pista de subtítulos ASS o SRT.");
                else
                {
                    int ttt = 0;
                    TextTrack tt;
                    if (tracks.Count == 1)
                    {
                        tt = (TextTrack)tracks[0];
                        ttt = int.Parse(tt.ID);
                    }
                    else
                    {
                        ChooseTrack cs = new ChooseTrack(tracks);
                        cs.ShowDialog();
                        tt = (TextTrack)tracks[cs.listaTracks.SelectedIndex];
                        ttt = int.Parse(tt.ID);
                        cs.Dispose();
                    }

                    FileInfo fi = new FileInfo(ofd.FileName);

                    string extension = ".ass";
                    if (tt.CodecString.Equals("UTF-8", StringComparison.InvariantCultureIgnoreCase))
                        extension = ".srt";

                    MKVAssName = fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length) + extension;
                    MKVAssName = MKVAssName.Replace(",", "_"); // odiosos archivos con comas, cojones -_-

                    setStatus("Extrayendo a " + MKVAssName + " (esto puede tardar unos segundos)");

                    MKVProgress = new ActionProgressW(this, "Demuxeando subtítulos");
                    MKVProgress.Show();

                    //Process pi = new Process();
                    ProcessStartInfo pi = new ProcessStartInfo(Application.StartupPath + "\\mkvextract.exe");
                    pi.Arguments = "tracks \"" + ofd.FileName + "\" " + ttt + ":\"" + MKVAssName + "\"";
                    pi.CreateNoWindow = true;
                    pi.WorkingDirectory = Application.StartupPath;
                    pi.UseShellExecute = false;

                    //pi.RedirectStandardError = true;
                    pi.RedirectStandardOutput = true;

                    MKVProc = new Process {StartInfo = pi, EnableRaisingEvents = true, SynchronizingObject = this};

                    MKVOutput = new Thread(new ThreadStart(T_MKVReadOutput));
                    MKVAnalysis = new Thread(new ThreadStart(T_MKVUpdate));

                    MKVProc.Exited += new EventHandler(MKVProc_Exited);

                    MKVProc.Start();
                    MKVAnalysis.Start();
                    MKVOutput.Start();

                }

            }
        }

        private void MKVExitProcess(bool AbortByUser)
        {
            // hacer 2 metodos, 1 para OK, 1 para ERROR.

            if (MKVOutput != null)
                MKVOutput.Abort();
            if (MKVAnalysis != null)
                MKVAnalysis.Abort();

            MKVProgress.Dispose();
            if (AbortByUser)
            {
                errorMsg("Extracción abortada por el usuario.");
            }
            else
            {
                if (!File.Exists(Application.StartupPath + "\\" + MKVAssName))
                    errorMsg("Ha habido algún error en la extracción y el archivo no está disponible :(");
                else
                {
                    openFile = Application.StartupPath + "\\" + MKVAssName;
                    chgCaption(openFile);
                    loadFile(openFile);
                    updateConcatenateConfigFile("LastASS", openFile);
                    putRecentASS();
                    if (MessageBox.Show("¿Quieres abrir también el vídeo?", appTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        LoadVideoFromFile(MKVVideoName);
                }
            }
        }

        void MKVProc_Exited(object sender, EventArgs e)
        {
            MKVExitProcess(MKVProgress.Abort);
        }

        void T_MKVReadOutput()
        {
            StreamReader sr = null;
            sr = MKVProc.StandardOutput;

            MKVLine = "";
            string linea = "";
            while ((linea = sr.ReadLine()) != null)
            {
                if (linea.StartsWith("progress", StringComparison.InvariantCultureIgnoreCase))
                    MKVLine = linea;
            }
        }

        void T_MKVUpdate()
        {
            while (true)
            {
                Regex r = new Regex(@"[0-9]*(\.)?[0-9]+");

                if (MKVProgress.Abort)
                {
                    MKVProc.Kill();
                    return;
                }

                string linea = "";
                if (MKVLine != null)
                {
                    if (!MKVLine.Equals(""))
                    {
                        linea = MKVLine;
                        MatchCollection mc = r.Matches(linea);
                        try
                        {
                            int p = int.Parse(mc[0].ToString());
                            MKVProgress.UpdatePerc(p);
                        }
                        catch { }
                    }
                }

                Thread.Sleep(1000);
            }
        }

        private void subtítulosDeUnMKVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MkvMagic();
        }
        #endregion

        #region Keyframes Artesanales

        private bool isKeyframeGuessNeeded()
        {
            if (KeyframesAvailable) return false;

            try
            {
                bool ConfigSettings = Convert.ToBoolean(getFromConfigFile("Video_SceneDetector"));

                return ConfigSettings;
            }
            catch
            {
                return true;
            }
        }

        Thread KeyFrameAnalysis;
        Size CaptureRes;
        DxScanScenes.Capture captura;
        //ArrayList misKeyFrames;

        private void KeyframeGuess(bool IsForced)
        {
            //if (avsClip == null) return;

            if (!isVideoLoaded) return;
            //if (KeyframesAvailable) return;

            if (KeyFrameAnalysis != null)
                if (KeyFrameAnalysis.IsAlive)
                {
                    if (!IsForced) return;
                    if (captura != null)
                        captura.StopCondition = true;
                    //KeyFrameAnalysis.Abort();
                }

            if (!IsForced)
            {
                if (LoadCacheSceneDetection(videoInfo.FileName))
                {
                    UpdatePanelAnalisis(false);
                    UpdateKeyFrameList(true);
                    return;
                }
            }

            videoInfo.KeyFrames = new ArrayList();
            drawKeyFrameBox();

            UpdatePanelAnalisis(true);

            KeyFrameAnalysis = new Thread(new ThreadStart(T_KeyFrameGuess));
            KeyFrameAnalysis.IsBackground = true;
            KeyFrameAnalysis.Priority = ThreadPriority.Lowest;
            KeyFrameAnalysis.Start();

        }

        private void pictureBox11_DoubleClick(object sender, EventArgs e)
        {
            // en el doble click al spinner, se cancela el escaneado de escenas
            if (captura != null)
            {
                if (KeyFrameAnalysis != null)
                    if (KeyFrameAnalysis.IsAlive)
                    {
                        if (MessageBox.Show("¿Deseas cancelar la detección de escenas en el vídeo?", appTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            captura.StopCondition = true;
                    }
            }

        }

        void T_KeyFrameGuess()
        {
            bool ffms2_ok = false;

            try
            {
                FFMSWrapper ffmsw = new FFMSWrapper(true);
                ffmsw.UpdateIndexProgress += new FFMSWrapper.IndexingProgressChange(ffmsw_UpdateIndexProgress);
                ffmsw.IndexingCompleted += new FFMSWrapper.IndexingFinished(ffmsw_IndexingCompleted);
                ffmsw.FFMSIndexer(videoInfo.FileName);
                FileInfo fi = new FileInfo(videoInfo.FileName);
                string ffms2_indexfile = Path.Combine(indexDir, fi.Name + ".ffindex");
                if (File.Exists(ffms2_indexfile))
                    ffmsw.FFMSReadIndex(ffms2_indexfile);
                else
                {
                    ffmsw.FFMSDoIndex(true);
                    ffmsw.FFMSWriteIndex(ffms2_indexfile);
                }

                // abrir / cerrar index
                FFMS_SourceData fsd = ffmsw.SourceData;
                FFMS2Track track = new FFMS2Track(fsd);
                track.GetTrack(ffmsw.GetIndex(), 0);

                for (int i = 0; i < track.GetNumFrames(0); i++)
                {
                    FFMS_FrameInfo finfo = track.GetFrameInfo(0, i);
                    if (finfo.KeyFrame == 1)
                        videoInfo.KeyFrames.Add(i);
                }
                ffms2_ok = true;
                SaveCacheSceneDetection(videoInfo.FileName);
                setStatus("Keyframes obtenidos con FFMS2.");
            }
            catch
            {
                if (isVideoLoaded && !isClosing)
                    errorMsg("Error obteniendo keyframes con FFMS2: revisa la configuración de tus códecs.\nSe va a proceder al análisis del vídeo por fuerza bruta, las posiciones pueden no ser exactas.");
                else return; 
            }


            // prioridad ffms2 -> dx

            if (!ffms2_ok)
            {
                videoInfo.KeyFrames.Clear();
                captura = new DxScanScenes.Capture(videoInfo.FileName);
                DateTime GuessInit = DateTime.Now;
                captura.EnCambioEscena += new DetectadoCambioEscena(captura_EnCambioEscena);
                captura.EnNuevoFrame += new DetectadoCambioFrame(captura_EnNuevoFrame);
                captura.Start();
                captura.WaitUntilDone();
                TimeSpan GuessDuration = DateTime.Now - GuessInit;
                double velocidad = Math.Round((double)FrameTotal / (double)GuessDuration.TotalSeconds);
                setStatus(captura.DetectedKeyFrames.Count + " escenas detectadas en segundo plano (" + velocidad + " fps).");

                foreach (double sample in captura.DetectedKeyFrames)
                {
                    int conversion = ((int)(sample * videoInfo.FrameRate));
                    videoInfo.KeyFrames.Add(conversion);
                }
                CaptureRes = new Size(captura.m_videoWidth, captura.m_videoHeight);
                SaveCacheSceneDetection(videoInfo.FileName);

                lock (this)
                {
                    captura.Dispose();
                    captura = null;
                }
            }
            UpdateKeyFrameList(true);
            UpdatePanelAnalisis(false);
        }

        void ffmsw_IndexingCompleted(object sender)
        {
            UpdateAnalysisText("Indexación finalizada.");
        }

        void ffmsw_UpdateIndexProgress(object sender, FFMSWrapper.IndexingProgressChangeEventArgs e)
        {
            int p = (int)(Math.Round((double)((double)e.ActualIndex / (double)e.TotalIndex) * 100));
            UpdateAnalysisText("Progreso de indexado: " + p + "%");
        }

        void captura_EnNuevoFrame(object sender, CambioEscenaEventArgs e)
        {
            int conversion = ((int)(e.TiempoSample * videoInfo.FrameRate));
            DrawKeyFrameDisplay(conversion);
        }

        void captura_EnCambioEscena(object sender, CambioEscenaEventArgs e)
        {
            int conversion = ((int)(e.TiempoSample * videoInfo.FrameRate));
            UpdateAnalysisText("Escena en " + conversion.ToString() + " (de " + videoInfo.FrameTotal + ")");
            videoInfo.KeyFrames.Add(conversion);
            DrawKeyFrameDisplay(conversion);
            //UpdateKeyFrameList(true);
        }

        delegate void SetDrawKeyframeDisplay(int i);

        private void DrawKeyFrameDisplay(int i)
        {
            if (this.keyFrameBox.InvokeRequired)
            {
                SetDrawKeyframeDisplay c = new SetDrawKeyframeDisplay(DrawKeyFrameDisplay);
                this.Invoke(c, new object[] { i });
            }
            else
            {
                keyFrameBox.BackgroundImage = BitmapFunctions.GenerateKeyFrameDisplayOne(keyFrameBox.Width, keyFrameBox.Height, i, FrameTotal);
            }
        }

        delegate void SetUpdateAnalysisTextCallback(string s);

        private void UpdateAnalysisText(string s)
        {
            if (this.label7.InvokeRequired)
            {
                SetUpdateAnalysisTextCallback c = new SetUpdateAnalysisTextCallback(UpdateAnalysisText);
                this.Invoke(c, new object[] { s });
            }
            else
            {
                label7.Text = s;
            }
        }

        delegate void SetBoolCallback(bool m3c);

        private void UpdatePanelAnalisis(bool s)
        {
            if (this.panelAnalisisEscenas.InvokeRequired)
            {
                SetBoolCallback d = new SetBoolCallback(UpdatePanelAnalisis);
                this.Invoke(d, new object[] { s });
            }
            else
            {
                panelAnalisisEscenas.Visible = s;
            }

        }

        private void UpdateKeyFrameList(bool s)
        {

            if (this.InvokeRequired)
            {
                SetBoolCallback d = new SetBoolCallback(UpdateKeyFrameList);
                this.Invoke(d, new object[] { s });
            }
            else
            {
                if (s) keyFrameBox.BackgroundImage = null;
                KeyframesAvailable = s;
            }

        }

        private void buscarKeyframesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            KeyframeGuess(true);
        }


        private void SaveCacheSceneDetection(string VideoName)
        {
            FileInfo info = new FileInfo(VideoName);
            string cachefile = Path.Combine(cacheDir, info.Name + ".cache2");
            if (File.Exists(cachefile)) File.Delete(cachefile);

            Config cache = new Config(cachefile);

            cache.SetValue("FileName", VideoName);
            cache.SetValue("FrameTotal", FrameTotal.ToString());
            cache.SetValue("FrameRate", estiloV4.d2s(videoInfo.FrameRate));

            int nentradas = videoInfo.KeyFrames.Count / 50;
            if ((videoInfo.KeyFrames.Count % 50) != 0) nentradas++;

            cache.SetValue("NumEntries", nentradas.ToString());
            ArrayList auxKeyF = (ArrayList)videoInfo.KeyFrames.Clone();

            for (int i = 0; i < nentradas; i++)
            {
                if (auxKeyF.Count > 50)
                {
                    cache.SetValueA("SceneEntry" + i, auxKeyF.GetRange(0, 50));
                    auxKeyF.RemoveRange(0, 50);
                }
                else
                {
                    cache.SetValueA("SceneEntry" + i, auxKeyF);
                }
            }


            cache.WriteConfig();
        }

        private bool LoadCacheSceneDetection(string VideoName)
        {
            FileInfo info = new FileInfo(VideoName);

            string cachefile2 = Path.Combine(cacheDir, info.Name + ".cache2");
            if (File.Exists(cachefile2))
            {
                try
                {
                    Config cache = new Config(cachefile2);
                    cache.LoadConfig();
                    if ((cache.GetValue("FileName") == VideoName) && (int.Parse(cache.GetValue("FrameTotal")) == FrameTotal) && (estiloV4.s2d(cache.GetValue("FrameRate")) == videoInfo.FrameRate))
                    {
                        videoInfo.KeyFrames = new ArrayList();
                        ArrayList tempstr = new ArrayList();

                        int nentradas = int.Parse(cache.GetValue("NumEntries"));
                        for (int i = 0; i < nentradas; i++)
                            tempstr.AddRange(cache.GetValueA("SceneEntry" + i));

                        foreach (string s in tempstr)
                            videoInfo.KeyFrames.Add(int.Parse(s));

                    }
                    else return false;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                string cachefile = Path.Combine(cacheDir, info.Name + ".cache");
                if (!File.Exists(cachefile)) return false;

                try
                {
                    Config cache = new Config(cachefile);
                    cache.LoadConfig();
                    if ((cache.GetValue("FileName") == VideoName) && (int.Parse(cache.GetValue("FrameTotal")) == FrameTotal) &&
                        (estiloV4.s2d(cache.GetValue("FrameRate")) == videoInfo.FrameRate))
                    {
                        videoInfo.KeyFrames = new ArrayList();
                        foreach (string val in cache.GetValueA("Scenes"))
                        {
                            videoInfo.KeyFrames.Add(int.Parse(val));
                        }
                    }
                    else return false;
                }
                catch { return false; }
            }
            return true;
        }
        #endregion

        #region BackgroundWorker
        private void LaunchBackgroundWorkerThread()
        {
            string nick = "";
            try { nick = getFromConfigFile("Nick"); }
            catch { }
            back = new BackgroundWork(mainW.appTitle, nick, 30000);

            back.OnRequestNamesUpdate += new BackgroundWork.RequestNamesUpdateHandler(back_OnRequestNamesUpdate);
            back.OnRequestScriptUpdate += new BackgroundWork.RequestScriptUpdateHandler(back_OnRequestScriptUpdate);
            back.OnUpdateNick += new BackgroundWork.UpdateNickHandler(back_OnUpdateNick);
            back.SendControlMessage += new BackgroundWork.SendMessage(back_SendControlMessage);

            back.Start();
        }

        void back_SendControlMessage(object sender, SendMessageEventArgs e)
        {
            MessageBox.Show(e.Msg, appTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        void back_OnUpdateNick(object sender, UpdateNickEventArgs e)
        {
            updateReplaceConfigFile("Nick", e.Nick);
        }

        void back_OnRequestScriptUpdate(object sender, EventArgs e)
        {
            try 
            {
                List<string> head = new List<string>();
                foreach (string s in script.GetHeaders())
                    head.Add(s);
                List<string> styles = new List<string>();
                foreach (estiloV4 v in script.GetStyles())
                    styles.Add(v.ToString());
                List<string> lines = new List<string>();
                foreach (lineaASS l in script.GetLineArrayList().GetFullArrayList())
                    lines.Add(l.ToString());
                back.UpdateScript(head, styles, lines);
            } 
            catch 
            {
                back.UpdateScript(null, null, null);
            }
        }

        void back_OnRequestNamesUpdate(object sender, EventArgs e)
        {
            string nick = "";
            try { nick = getFromConfigFile("Nick"); }
            catch { }
            string forms = "";
            foreach (Form f in Application.OpenForms)
                forms+=f.Name+" ";

            string scriptn = "";
            string videon = "";
            try
            {
                scriptn = script.FileName;
            }
            catch { }
            try
            {
                videon = videoInfo.FileName;
            }
            catch { videon = null;  }

            back.UpdateNames(nick, scriptn, videon, forms);
        }
        #endregion 

        private void changelogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(changelogUrl);
        }

        private void blogDeDesarrolloToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(developblogUrl);
        }

        private void documentaciónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(documentationUrl);
        }

        private void buttonAddFrameInicio_Click(object sender, EventArgs e)
        {
            lineaASS lass = null;
            try {
                lass = (lineaASS)al[gridASS.SelectedRows[0].Index];
            } catch {}

            if (videoInfo.FrameRate == 0)
            {
                MessageBox.Show("No se puede realizar la acción para este vídeo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            } 
            else if (lass==null) 
            {
                MessageBox.Show("No hay ninguna línea seleccionada", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                AddFrames af = new AddFrames();
                if (af.ShowDialog() == DialogResult.OK)
                {
                    if (af.Result > 0)
                    {
                        double sd = (double)Math.Round(af.Result * ((double)1 / (double)videoInfo.FrameRate), 2);
                        lass.t_inicial.sumaTiempo(sd);
                        refreshGrid();
                        updatePanelTextTools();
                    }
                }

            }
        }

        private void buttonAddFrameFin_Click(object sender, EventArgs e)
        {
            lineaASS lass = null;
            try
            {
                lass = (lineaASS)al[gridASS.SelectedRows[0].Index];
            }
            catch { }

            if (videoInfo.FrameRate == 0)
            {
                MessageBox.Show("No se puede realizar la acción para este vídeo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (lass == null)
            {
                MessageBox.Show("No hay ninguna línea seleccionada", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                AddFrames af = new AddFrames();
                if (af.ShowDialog() == DialogResult.OK)
                {
                    if (af.Result > 0)
                    {
                        double sd = (double)Math.Round(af.Result * ((double)1 / (double)videoInfo.FrameRate), 2);
                        lass.t_final.sumaTiempo(sd);
                        refreshGrid();
                        updatePanelTextTools();
                    }
                }

            }
        }
    }
}