using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace scriptASS
{
    public partial class avsW : Form
    {
        public mainW mW;
        string avsFile1 = null;
        ArrayList tempFileNames = new ArrayList();

        Hashtable AVSEnvironments = new Hashtable();
        Hashtable AVSClips = new Hashtable();
        Hashtable AVSActualFrames = new Hashtable();
        Hashtable fileDirectories = new Hashtable();
        string cur_dir;
        int tabpage = 0;

        public avsW(mainW mw)
        {
            mW = mw;
            InitializeComponent();
            InitForm();
        }

        public avsW(mainW mw, string fname)
        {
            mW = mw;
            InitializeComponent();
            InitForm();

            LoadFileAVS(fname);
        }


        private void InitForm()
        {
            tabControl.Controls.Clear();
            AddTabPage();
            cur_dir = Directory.GetCurrentDirectory();
            this.Disposed += new EventHandler(avsW_Disposed);
            tabControl.Size = new Size(this.Width - 10, this.Height - 100);
            this.SizeChanged += new EventHandler(avsW_SizeChanged);
            tabControl.Selected += new TabControlEventHandler(tabControl_Selected);
            ActualFrame.KeyPress += new KeyPressEventHandler(ActualFrame_KeyPress);
            trackBar.ValueChanged += new EventHandler(trackBar_Scroll);
        }

        void ActualFrame_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (Convert.ToInt32(e.KeyChar))
            {
                case 13:
                    try
                    {
                        int toFrame = int.Parse(ActualFrame.Text);
                        if (toFrame > trackBar.Maximum) toFrame = trackBar.Maximum;

                        trackBar.Value = toFrame;
                    }
                    catch { }

                    e.Handled = true;
                    SendKeys.Send("");
                    break;
            }
        }


        void tabControl_Selected(object sender, TabControlEventArgs e)
        {
            TabPage t = (TabPage)tabControl.Controls[tabControl.SelectedIndex];
            int k = t.Handle.ToInt32();

            try
            {
                AviSynthClip clip = (AviSynthClip)AVSClips[k];
                int frame = (int)AVSActualFrames[k];
                //PictureBox p = GetPictureBox(tabControl.SelectedIndex); // no hace falta y perdemos velocidad
                //p.Image = AviSynthFunctions.getBitmapFromFrame(clip, 0, frame);
                updateFromAviSynthClip(clip);
                ActualFrame.Text = frame.ToString();
                trackBar.Value = frame;
            }
            catch
            {
                trackBar.Visible = false;
                label3.Visible = false;
                label4.Visible = false;
                ActualFrame.Visible = false;
                TotalFrames.Visible = false;
                mas1.Visible = false;
                menos1.Visible = false;
            }

        }

        private void updateFromAviSynthClip(AviSynthClip avs)
        {
            trackBar.Visible = true;
            label3.Visible = true;
            label4.Visible = true;
            ActualFrame.Visible = true;
            TotalFrames.Visible = true;
            mas1.Visible = true;
            menos1.Visible = true;


            ActualFrame.Text = "0";
            TotalFrames.Text = avs.num_frames.ToString();
            trackBar.Value = 0;
            trackBar.Maximum = avs.num_frames;

        }

        void avsW_SizeChanged(object sender, EventArgs e)
        {
            tabControl.Size = new Size(this.Width - 10, this.Height - 100);
        }

        void avsW_Disposed(object sender, EventArgs e)
        {
            foreach (string s in tempFileNames)
            {
                try
                {
                    File.SetAttributes(s, FileAttributes.Normal);
                    File.Delete(s);
                }
                catch { }
            }

            ICollection clips = AVSClips.Values;            

            foreach (AviSynthClip avs in clips)
            {
                avs.cleanup(false);
            }

            AVSClips.Clear();
            AVSEnvironments.Clear();

            Directory.SetCurrentDirectory(cur_dir);

            GC.Collect();
        }

        private string GetTempFileName()
        {
            int i = tabControl.SelectedIndex;

            string dir = (string)fileDirectories[tabControl.SelectedIndex];
            string b = dir + "\\AVSEditor.TempFile.";
            string s = b + i+".";
            int i2 = 0;
            string res = "";
            while (true)
            {
                if (!File.Exists(s + i2 + ".avs"))
                {
                    res = s + i2 + ".avs";
                    break;
                }
                i2++;
            }
            
            tempFileNames.Add(res);
            return res;
        }

        public void InsertAVSCode(string code)
        {
            AVSTextBox a = GetAVSTextBox(tabControl.SelectedIndex);
            string pre_string = a.Text.Substring(0, a.SelectionStart);
            string post_string = a.Text.Substring(a.SelectionStart+a.SelectionLength);
            a.Text = pre_string + code + post_string;
            a.SelectionStart = pre_string.Length + code.Length;
            a.SelectionLength = 0;
        }

        private AviSynthClip GetAVSClip(int tab_index)
        {
            TabPage t = (TabPage)tabControl.Controls[tab_index];
            int k = t.Handle.ToInt32();
            return (AviSynthClip)AVSClips[k];
        }

        private AVSTextBox GetAVSTextBox(int tab_index)
        {
            TabPage t = (TabPage)tabControl.Controls[tab_index];
            SplitContainer s = (SplitContainer)t.Controls[0];
            return (AVSTextBox)s.Panel1.Controls[0];
        }

        private PictureBox GetPictureBox(int tab_index)
        {
            TabPage t = (TabPage)tabControl.Controls[tab_index];
            SplitContainer s = (SplitContainer)t.Controls[0];
            Panel p = (Panel)s.Panel2.Controls[0];
            return (PictureBox)p.Controls[0];
        }

        private SplitContainer GetSplitContainer(int tab_index)
        {
            TabPage t = (TabPage)tabControl.Controls[tab_index];
            return (SplitContainer)t.Controls[0];
        }

        private void LoadFileAVS(string fname)
        {
            AVSTextBox a = GetAVSTextBox(tabControl.SelectedIndex);
            a.LoadFile(fname, RichTextBoxStreamType.PlainText);
            avsFile1 = fname;
            if (fileDirectories.ContainsKey(tabControl.SelectedIndex))
                fileDirectories.Remove(tabControl.SelectedIndex);
            fileDirectories.Add(tabControl.SelectedIndex, avsFile1.Substring(0, avsFile1.LastIndexOf('\\')));
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Archivos de AviSynth (*.avs)|*.avs|Archivos de Funciones de AviSynth (*.avsi)|*.avsi";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                LoadFileAVS(ofd.FileName);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (avsFile1 != null)
            {
                AVSTextBox a = GetAVSTextBox(tabControl.SelectedIndex);
                a.SaveFile(avsFile1, RichTextBoxStreamType.PlainText);
            }
            else
            {
                mW.errorMsg("Si es la primera vez que guardas el archivo, usa la opción 'Guardar cómo'.");
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Archivos de AviSynth (*.avs)|*.avs|Archivos de Funciones de AviSynth (*.avsi)|*.avsi";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                AVSTextBox a = GetAVSTextBox(tabControl.SelectedIndex);
                a.SaveFile(sfd.FileName, RichTextBoxStreamType.PlainText);

                if (fileDirectories.ContainsKey(tabControl.SelectedIndex))
                    fileDirectories.Remove(tabControl.SelectedIndex);
                fileDirectories.Add(tabControl.SelectedIndex, sfd.FileName.Substring(0, sfd.FileName.LastIndexOf('\\')));

            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }


        private void AddTabPage()
        {
            int new_number = tabpage++;
            string new_name = "AVS " + new_number;
            TabPage new_tab = new TabPage(new_name);
            new_tab.Text = new_name;
            //new_tab.GotFocus += new EventHandler(TabGotLove);

            tabControl.Controls.Add(new_tab);

            SplitContainer new_split = new SplitContainer();

            new_split.Dock = System.Windows.Forms.DockStyle.Fill;
            new_split.Location = new System.Drawing.Point(3, 3);
            new_split.Name = new_name;
            new_split.Orientation = System.Windows.Forms.Orientation.Horizontal;
            new_split.Panel2Collapsed = true;
            new_split.BorderStyle = System.Windows.Forms.BorderStyle.None;

            AVSTextBox new_text = new AVSTextBox();

            new_text.Dock = System.Windows.Forms.DockStyle.Fill;
            new_text.Font = new System.Drawing.Font("Courier New", 10, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            new_text.Location = new System.Drawing.Point(0, 0);
            new_text.Size = new System.Drawing.Size(817, 546);
            new_text.Text = "";
            new_text.ScrollBars = RichTextBoxScrollBars.Both;
            new_text.WordWrap = false;
            new_text.AcceptsTab = true;
            new_text.KeyDown += new KeyEventHandler(new_text_KeyDown);
            new_text.ContextMenuStrip = menuCustom;

            Panel new_panel = new Panel();
            new_panel.Dock = System.Windows.Forms.DockStyle.Fill;
            new_panel.Location = new System.Drawing.Point(0, 0);
            new_panel.Size = new System.Drawing.Size(817, 25);
            new_panel.TabIndex = 0;
            new_panel.TabStop = false;
            new_panel.BorderStyle = BorderStyle.Fixed3D;
            new_panel.AutoScroll = true;

            PictureBox new_pict = new PictureBox();
            new_pict.Dock = DockStyle.None;
            new_pict.Location = new System.Drawing.Point(0, 0);
            new_pict.Size = new System.Drawing.Size(0,0);
            new_pict.TabIndex = 0;
            new_pict.TabStop = false;
            new_pict.BorderStyle = BorderStyle.None;

            new_panel.Controls.Add(new_pict);

            new_split.Panel1.Controls.Add(new_text);
            new_split.Panel2.Controls.Add(new_panel);

            new_tab.Controls.Add(new_split);

            fileDirectories.Add(new_number, Application.StartupPath);
        }

        void new_text_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    if (e.Alt)
                    {
                        if (tabControl.SelectedIndex > 0 && tabControl.TabCount > 1)
                            tabControl.SelectedIndex--;
                    }
                    break;
                case Keys.Right:
                    if (e.Alt)
                    {
                        if (tabControl.SelectedIndex < tabControl.TabCount - 1 && tabControl.TabCount > 1)
                            tabControl.SelectedIndex++;
                    }
                    break;
                case Keys.F5:
                    RefreshImageAVS();
                    break;
                case Keys.F2:
                    AddTabPage();
                    break;
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            AddTabPage();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex == -1) return;

            if (MessageBox.Show("Se van a perder todos los datos del tab, ¿seguro que deseas quitarlo?", "Eliminar Tab", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {

                if (tabControl.Controls.Count == 1)
                {
                    mW.errorMsg("No se puede quitar el tab cuando sólamente queda uno");
                    return;
                }

                TabPage t = (TabPage)tabControl.Controls[tabControl.SelectedIndex];

                AVSTextBox a = GetAVSTextBox(tabControl.SelectedIndex);
                a.Dispose();

                PictureBox p = GetPictureBox(tabControl.SelectedIndex);
                p.Dispose();
                
                SplitContainer s = GetSplitContainer(tabControl.SelectedIndex);
                s.Dispose();

                int k = t.Handle.ToInt32();

                AVSClips.Remove(k);
                AVSEnvironments.Remove(k);

                tabControl.Controls.RemoveAt(tabControl.SelectedIndex);
                fileDirectories.Remove(tabControl.SelectedIndex);

            }
        }

        private void RefreshImageAVS()
        {
            string tmp = GetTempFileName();

            AVSTextBox a = GetAVSTextBox(tabControl.SelectedIndex);
            a.SaveFile(tmp, RichTextBoxStreamType.PlainText);
            try
            {
                File.SetAttributes(tmp, FileAttributes.Hidden);
            }
            catch { }

            try
            {

                TabPage t = (TabPage)tabControl.Controls[tabControl.SelectedIndex];
                int k = t.Handle.ToInt32();

                AviSynthScriptEnvironment env = new AviSynthScriptEnvironment();
                AviSynthClip clip = env.OpenScriptFile(tmp);


                if (AVSEnvironments.ContainsKey(k))
                    AVSEnvironments.Remove(k);
                AVSEnvironments.Add(k, env);

                if (AVSClips.ContainsKey(k))
                {
                    AviSynthClip old_clip = (AviSynthClip)AVSClips[k];
                    old_clip.cleanup(false);
                    AVSClips.Remove(k);
                }
                AVSClips.Add(k, clip);

                if (AVSActualFrames.ContainsKey(k))
                    AVSActualFrames.Remove(k);
                AVSActualFrames.Add(k, 0);

                DateTime inicio = DateTime.Now;
                PictureBox p = GetPictureBox(tabControl.SelectedIndex);
                Bitmap frame = AviSynthFunctions.getBitmapFromFrame(clip, 0, 0);
                p.Size = frame.Size;
                p.Image = frame;
                DateTime fin = DateTime.Now;
                TimeSpan tiempo = fin - inicio;
                int t_millis = tiempo.Milliseconds;
                label1.Text = "Tiempo de carga: " + t_millis.ToString() + "ms";

                updateFromAviSynthClip(clip);

                /*
                int tiempo_est = (t_millis * clip.num_frames);
                long ticks = tiempo_est * 10000000;
                TimeSpan t_est = new TimeSpan(ticks);                
                label2.Text = "Tiempo estimado: " + t_est.ToString();
                */
            }
            catch (AviSynthException ase)
            {
                mW.errorMsg(ase.Message);
            }
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            RefreshImageAVS();
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            SplitContainer actual = GetSplitContainer(tabControl.SelectedIndex);

            if (actual.Panel1Collapsed) return;
            if (actual.Panel2Collapsed) 
                actual.Panel2Collapsed = false;
            else
                actual.Panel1Collapsed = true;
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            SplitContainer actual = GetSplitContainer(tabControl.SelectedIndex);

            if (actual.Panel2Collapsed) return;
            if (actual.Panel1Collapsed)
                actual.Panel1Collapsed = false;
            else
                actual.Panel2Collapsed = true;

        }

        private void trackBar_Scroll(object sender, EventArgs e)
        {
            try {
                AviSynthClip clip = GetAVSClip(tabControl.SelectedIndex);
                TabPage t = (TabPage)tabControl.Controls[tabControl.SelectedIndex];
                int k = t.Handle.ToInt32();

                if (AVSActualFrames.Contains(k))
                    AVSActualFrames.Remove(k);
                AVSActualFrames.Add(k, trackBar.Value);
                ActualFrame.Text = trackBar.Value.ToString();
                PictureBox p = GetPictureBox(tabControl.SelectedIndex);
                p.Image = null;
                GC.Collect();
                p.Image = AviSynthFunctions.getBitmapFromFrame(clip, 0, trackBar.Value);
            } catch { } 
        }

        private void menos1_Click(object sender, EventArgs e)
        {
            trackBar.Value = (trackBar.Value > 0) ? trackBar.Value-1 : 0;
        }

        private void mas1_Click(object sender, EventArgs e)
        {
            trackBar.Value = (trackBar.Value < trackBar.Maximum) ? trackBar.Value+1 : trackBar.Maximum;
        }

        // --- funciones

        #region ABRIR VIDEOS

        private void aviSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.Filter = "Archivos de vídeo AVI (*.avi)|*.avi";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                InsertAVSCode("AviSource(\"" + ofd.FileName + "\",false)");
            }
        }

        private void directShowSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.Filter = "Archivos de vídeo (*.avi;*.wmv;*.ogm;*.mkv;*.mp4)|*.avi;*.wmv;*.ogm;*.mkv;*.mp4";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                InsertAVSCode("DirectShowSource(\"" + ofd.FileName + "\",audio=false)");
            }
        }

        private void mPEG2SourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.Filter = "Archivos D2V (*.d2v)|*.d2v";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                InsertAVSCode("MPEG2Source(\"" + ofd.FileName + "\")");
            }
        }


        #endregion

        #region COLOR
        private void yV12ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertAVSCode("ConvertToYV12()");
        }       

        private void rGB32ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertAVSCode("ConvertToRGB32()");
        }

        private void yUY2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertAVSCode("ConvertToYUY2()");
        }

        private void y8ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertAVSCode("ConvertToY8()");
        }
        #endregion

        #region ALIASING
        private void aAAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertAVSCode("BicubicResize(width*2,height*2)\nSangNom(AA=10)\nTurnRight()\nSangNom(AA=10)\nTurnLeft()\nBicubicResize(width/2,height/2)");
        }
        private void sangnomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertAVSCode("SangNom(AA=10)");
        }

        private void tIsophoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertAVSCode("TIsophote()");
        }
        #endregion

        #region DENOISERS
        private void vaguedenoiserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertAVSCode("vaguedenoiser(nsteps=8,chromaT=0,Wiener=true)");
        }

        private void deenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertAVSCode("deen(\"a2d\",2,10,10)");
        }

        private void fft3dgpuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertAVSCode("FFT3DGPU(sigma=3)");
        }

        private void fft3dfilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertAVSCode("FFT3DFilter(sigma=3)");
        }
        private void unDotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertAVSCode("UnDot()");
        }


        #endregion

        #region SHARPENERS

        private void aSharpaWarpSharpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertAVSCode("asharp(1,0,0).awarpsharp(5,1,bm=3,cm=0,thresh=0.99)");
        }

        private void limitedSharpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertAVSCode("LimitedSharpenFaster(strength=150,smode=3,ss_x=3,ss_y=3)");
        }

        #endregion

        #region PERFILADO LINEAS
        private void fastLineDarkenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertAVSCode("FastLineDarken(thinning=0)");
        }

        private void mfToonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertAVSCode("mfToon()");
        }

        #endregion

        #region RESIZERS
        private void lanczosResizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResolutionInputBox res = new ResolutionInputBox(this, "LanczosResize");
            this.Enabled = false;
            res.Show();
        }

        private void bicubicResizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResolutionInputBox res = new ResolutionInputBox(this, "BicubicResize");
            this.Enabled = false;
            res.Show();

        }

        private void lanczos4ResizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResolutionInputBox res = new ResolutionInputBox(this, "Lanczos4Resize");
            this.Enabled = false;
            res.Show();

        }

        private void simpleResizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResolutionInputBox res = new ResolutionInputBox(this, "SimpleResize");
            this.Enabled = false;
            res.Show();

        }
        #endregion

        #region MISC
        private void textSubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.Filter = "Archivos de Subtítulos (*.ssa;*.ass)|*.ssa;*.ass";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                InsertAVSCode("TextSub(\"" + ofd.FileName + "\")");
            }

        }
        #endregion

        private void importarAVSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.Filter = "Archivos de AviSynth (*.avs;*.avsi)|*.avs;*.avsi";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                InsertAVSCode("Import(\"" + ofd.FileName + "\")");
            }

        }

        private void cargarPluginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.Filter = "Archivos de Filtros (*.dll;*.vdf)|*.dll;*.vdf";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                InsertAVSCode("LoadPlugin(\"" + ofd.FileName + "\")");
            }

        }

        private void croppingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cropW cw = null;

            try
            {
                AviSynthClip clip = GetAVSClip(tabControl.SelectedIndex);
                cw = new cropW(this, clip);
                this.Enabled = false;
                cw.Show();
            }
            catch
            {
                if (cw!=null) cw.Dispose();
                this.Enabled = true;
                mW.errorMsg("Debes tener cargado el vídeo para hacer esto.\nPulsa el botón de Preview, o la tecla F5 para cargarlo.");
            }
        }

    }
}