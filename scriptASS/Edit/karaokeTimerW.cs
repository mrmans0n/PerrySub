using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections;
using System.Text.RegularExpressions;
using Microsoft.DirectX.DirectSound;

namespace scriptASS
{
    public partial class karaokeTimerW : Form
    {
        mainW mW;
        int ActualLine = 0;
        private int selectedSyl = 0;
        char Separador = '|';
        bool SplitterOn = false;
        int SplitterLeft = 0;
        int SplitterRight = 0;
        ArrayList OurK = new ArrayList();
        ArrayList SensitivePoints = new ArrayList();

        Color Selected = Color.Beige;
        Color UnSelected = Color.Brown;

        Bitmap AudioWave;
        AVStoDirectSound avs2ds;
        AviSynthClip avsaudio;
        byte[] bufferEntero;
        WaveType WaveStyle = WaveType.Normal;
        ReproductionState PlayStatus = ReproductionState.Stop;
        PlayMode Mode;
        int LastPosition = -1;
        PlayOptions LastOptions = PlayOptions.Normal;
        double PosRel = 0.0;
        double LastEnd = 0.0;

        private IntPtr AudioFull = IntPtr.Zero;

        int px10ms = 0;

        public int SelectedSyl
        {
            get { return selectedSyl; }
            set { 
                selectedSyl = value;
                int cosa = value + 1;

                infoSilabaSeleccionada.Text = (cosa == 0) ? "Ø" : cosa.ToString() + " (" + GetKKKtext(value)+")";
            }
        }

        public karaokeTimerW(mainW mW,AVStoDirectSound avs2ds,AviSynthClip avsaudio, IntPtr af)
        {
            InitializeComponent();
            this.mW = mW;
            this.avs2ds = avs2ds;
            this.avsaudio = avsaudio;
            this.AudioFull = af;

            //this.MaximumSize = new Size(0,this.Height);

            this.Resize += new EventHandler(karaokeTimerW_Resize);            
            sliceTextBox.KeyPress += new KeyPressEventHandler(sliceTextBox_KeyPress);
            sliceTextBox.KeyDown += new KeyEventHandler(sliceTextBox_KeyDown);
            zoomKaraokeTrackBar.ValueChanged += new EventHandler(zoomKaraokeTrackBar_ValueChanged);
            KaraokeAudioBox.MouseMove += new MouseEventHandler(KaraokeAudioBox_MouseMove);
            KaraokeAudioBox.MouseDown += new MouseEventHandler(KaraokeAudioBox_MouseDown);
            KaraokeAudioBox.MouseUp += new MouseEventHandler(KaraokeAudioBox_MouseUp);
            KaraokeAudioBox.MouseClick += new MouseEventHandler(KaraokeAudioBox_MouseClick);
            KaraokeAudioBox.KeyDown += new KeyPictureBox.OnKeyPress(KaraokeAudioBox_KeyDown);
            KaraokeAudioBox.LostFocus += new EventHandler(KaraokeAudioBox_LostFocus);

            AudioActualPosition_Refresh.Tick += new EventHandler(AudioActualPosition_Refresh_Tick);

            //this.Width = 800;
        }

        void sliceTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F1:
                    int old_sel = sliceTextBox.SelectionStart;
                    int old_sel_len = sliceTextBox.SelectionLength;
                    sliceTextBox.Text = sliceTextBox.Text.Insert(sliceTextBox.SelectionStart, "|");
                    sliceTextBox.Select(old_sel + 1, old_sel_len);
                    break;
                case Keys.F2:
                    SliceSlice();
                    break;
            }
        }

        void AudioActualPosition_Refresh_Tick(object sender, EventArgs e)
        {
            double pos_relativa = (double)((double)mW.audio.PlayPosition) / ((double)avsaudio.AudioSampleRate * (double)avsaudio.BytesPerSample);

            if (PlayStatus == ReproductionState.Stop || mW.audio.Status.Terminated)
            {                
                PlayStatus = ReproductionState.Stop;
                mW.audio.Stop();

                DrawAudioWave();

                AudioActualPosition_Refresh.Enabled = false;
                LastPosition = -1;
                return;
            } 
            
            double principio = Tiempo.TimeToSecondDouble(infoTiempoInicial.Text);
            double fin = Tiempo.TimeToSecondDouble(infoTiempoFinal.Text);
            double duracion = fin - principio;            

            if (Mode == PlayMode.Other)
            {
                switch (LastOptions)
                {
                    case PlayOptions.BeforeStart:
                        PosRel = Math.Max(0, PosRel - .5);
                        break;
                    case PlayOptions.BeforeEnd:
                        PosRel = Math.Max(0, LastEnd - .5);
                        break;
                    case PlayOptions.AfterEnd:
                        PosRel = LastEnd;
                        break;
                }
            }

            if (Mode == PlayMode.Selection || Mode == PlayMode.Other)
                pos_relativa += PosRel;

            double perc = (pos_relativa / duracion) * 100;
            int iPX = Convert.ToInt32(Math.Round((PanelAudioBox.Width * perc) / 100));
                Bitmap temp = (Bitmap)AudioWave.Clone();
                using (Graphics g = Graphics.FromImage((Image)temp))
                {
                    g.DrawLine(new Pen(new SolidBrush(Color.White), 3), new Point(iPX, 0), new Point(iPX, PanelAudioBox.Height));
                }

                if (iPX <= LastPosition)
                {
                    PlayStatus = ReproductionState.Stop;
                    mW.audio.Stop();

                    DrawAudioWave();

                    AudioActualPosition_Refresh.Enabled = false;
                    LastPosition = -1;
                    return;
                }
                KaraokeAudioBox.Image = temp;
            LastPosition = iPX;
        }

        #region HANDLERS
        
        void KaraokeAudioBox_LostFocus(object sender, EventArgs e)
        {
            DrawAudioWave();

        }
        void KaraokeAudioBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Q:
                    PlayAllEx(PlayOptions.BeforeStart);
                    break;
                case Keys.W:
                    PlayAllEx(PlayOptions.AfterEnd);
                    break;
                case Keys.E:
                    PlayAllEx(PlayOptions.AfterStart);
                    break;
                case Keys.D:
                    PlayAllEx(PlayOptions.BeforeEnd);
                    break;
                case Keys.S:
                case Keys.Space:
                    PlaySelected();
                    break;
                case Keys.G:
                case Keys.Enter:
                    CommitChanges();
                    SelectNext();
                    break;
                case Keys.PageDown:
                    SelectNext();
                    break;
                case Keys.PageUp:
                    SelectPrev();
                    break;
                case Keys.R:
                case Keys.P:
                case Keys.T:
                    PlayAll();
                    break;
                case Keys.F2:
                    SliceSlice();
                    break;
                case Keys.F3:
                    SliceWords();
                    break;
                case Keys.F4:
                    UnionSelected();
                    break;
                case Keys.F5:
                    SepararSilabas();
                    break;
            }
        }

        
        void KaraokeAudioBox_MouseClick(object sender, MouseEventArgs e)
        {
            KaraokeAudioBox.Focus();

            if (!SplitterOn)
            {


                double principio = Tiempo.TimeToSecondDouble(infoTiempoInicial.Text);
                double fin = Tiempo.TimeToSecondDouble(infoTiempoFinal.Text);
                double duracion = fin - principio;

                int num_ticks = (int)Math.Round(duracion / 0.10);
                double bleh = (double)PanelAudioBox.Width / (double)num_ticks;
                int nuevo_in = (int)Math.Round((double)e.X * 10 / bleh);                

                int r = GetMSRegion(nuevo_in);
                SelectedSyl = r;

                if (r >= OurK.Count) return;
                ChangeButton((Button)SliceHandler.Controls[r]);                
                K mumb = (K)OurK[r];
                sliceTextBox.Text = mumb.Text;
                DrawAudioWave();
            }
        }

        private void UpdateMoveMouse(int X)
        {
            K prevK = (K)OurK[SplitterLeft];
            K nextK = (SplitterRight == OurK.Count) ? null : (K)OurK[SplitterRight];

            double principio = Tiempo.TimeToSecondDouble(infoTiempoInicial.Text);
            double fin = Tiempo.TimeToSecondDouble(infoTiempoFinal.Text);
            double duracion = fin - principio;

            int num_ticks = (int)Math.Round(duracion / 0.10);
            double bleh = (double)PanelAudioBox.Width / (double)num_ticks;
            int nuevo_in = (int)Math.Round((double)X * 10 / bleh);

            int StartLeft = GetRegionStartPoint(SplitterLeft);
            int StartRight = StartLeft + prevK.Milliseconds;


            int referencia = nuevo_in - StartRight;
            prevK.Milliseconds += referencia;
            if (nextK != null)
                nextK.Milliseconds -= referencia;
            
            DrawAudioWave();
        }

        void KaraokeAudioBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (!SplitterOn) return;
            UpdateMoveMouse(e.X); 
            SplitterOn = false;

        }

        void KaraokeAudioBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X == 0) return; //no queremos mover el 1º

            if (SensitivePoints.Contains(e.X))
            {                    
                double principio = Tiempo.TimeToSecondDouble(infoTiempoInicial.Text);
                double fin = Tiempo.TimeToSecondDouble(infoTiempoFinal.Text);
                double duracion = fin - principio;

                int num_ticks = (int)Math.Round(duracion / 0.10);
                double bleh = (double)PanelAudioBox.Width / (double)num_ticks;
                int nuevo_in = (int)Math.Round((double)e.X * 10 / bleh);

                SplitterLeft = GetMSRegion(nuevo_in-2);
                SplitterRight = SplitterLeft + 1;
                SplitterOn = true;
            }
        }

        void KaraokeAudioBox_MouseMove(object sender, MouseEventArgs e)
        {
            KaraokeAudioBox.Cursor = (SplitterOn || SensitivePoints.Contains(e.X)) ? Cursors.VSplit : Cursors.Default;

            if (SplitterOn && !SensitivePoints.Contains(e.X))
            {

                if (e.X < 0 || e.X > PanelAudioBox.Width)
                    SplitterOn = false; // nos salimos de rango                
                else
                {
                    //movemos y cambian cosas 
                    double principio = Tiempo.TimeToSecondDouble(infoTiempoInicial.Text);
                    double fin = Tiempo.TimeToSecondDouble(infoTiempoFinal.Text);
                    double duracion = fin - principio;

                    int num_ticks = (int)Math.Round(duracion / 0.10);
                    double bleh = (double)PanelAudioBox.Width / (double)num_ticks;
                    int nuevo_in = (int)Math.Round((double)e.X * 10 / bleh);

                    if (GetMSRegion(nuevo_in) != SplitterLeft && GetMSRegion(nuevo_in) != SplitterRight)
                        SplitterOn = false;
                    else
                        UpdateMoveMouse(e.X);
                    // else label7.Text = "X=" + e.X + " - " + nuevo_in + " reg=" + GetMSRegion(nuevo_in)+" Left:"+SplitterLeft+" Right:"+SplitterRight;
                }
            }

        }


        void sliceTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (Convert.ToInt32(e.KeyChar))
            {
                case 13:

                    SliceSelected();
                    sliceTextBox.Enabled = false;
                    sliceTextBox.Text = "";

                    SendKeys.Send("");
                    e.Handled = true;
                    break;
            }
        }


        void zoomKaraokeTrackBar_ValueChanged(object sender, EventArgs e)
        {
            DrawAudioWave();
        }

        void karaokeTimerW_Resize(object sender, EventArgs e)
        {
            bool[] old_values = new bool[SliceHandler.Controls.Count];
            int idx = 0;
            foreach (Button b in SliceHandler.Controls)
                old_values[idx++] = CheckButton(b);

            PanelAudioBox.Width = this.Width - 12;
            selectedKaraokeLine.Width = PanelAudioBox.Width;
            SliceHandler.Width = PanelAudioBox.Width;
            controlesPanel.Width = Math.Max(0, (this.Width - 18) - controlesPanel.Location.X);
            DrawAudioWave();
            FillPanelButtons();

            idx = 0;
            foreach (Button b in SliceHandler.Controls)
                SetButton(b, old_values[idx++]);

        }

        private void karaokeTimerW_Load(object sender, EventArgs e)
        {
            if (mW.al.Count > 0)
                DisplayLine(0);

            zoomKaraokeTrackBar.Maximum = mW.zoomTrackBar.Maximum;
            zoomKaraokeTrackBar.Minimum = mW.zoomTrackBar.Minimum;
            zoomKaraokeTrackBar.LargeChange = mW.zoomTrackBar.LargeChange;
            zoomKaraokeTrackBar.SmallChange = mW.zoomTrackBar.SmallChange;
            zoomKaraokeTrackBar.Value = mW.zoomTrackBar.Value;
            this.Width = 800;
            WaveStyle = mW.AudioWaveType;
            FillPanelButtons();
        }

        void K_ButtonClicked(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            ChangeButton(b);
            sliceTextBox.Text = b.Text;
            SelectedSyl = SliceHandler.Controls.GetChildIndex(b, false);
            DrawAudioWave();
        }

        #endregion

        #region DRAW

        private void DrawAudioGrid()
        {
            double inicio = Tiempo.TimeToSecondDouble(infoTiempoInicial.Text);
            double fin = Tiempo.TimeToSecondDouble(infoTiempoFinal.Text);

            double duracion = fin - inicio;
            if (duracion <= 0) return; // ya hemos avisado antes

            int num_ticks = (int)Math.Round(duracion / 0.10);
            int pxbticks = (int)Math.Round((double)PanelAudioBox.Width / (double)num_ticks);

            using (Graphics g = Graphics.FromImage((Image)AudioWave))
            {
                Point new_p = new Point(0, AudioWave.Height - 10);

                for (int i = 0; i < num_ticks+10; i++)
                {
                    if ((i%10)!=0)
                        g.DrawLine(new Pen(new SolidBrush(Color.Red)), new_p, new Point(new_p.X, AudioWave.Height));
                    else
                        g.DrawLine(new Pen(new SolidBrush(Color.Yellow)), new Point(new_p.X,new_p.Y-5), new Point(new_p.X, AudioWave.Height));
                    new_p.X = new_p.X + pxbticks;
                    px10ms = pxbticks;
                }
            }
        }

        private void DrawAudioWave()
        {
            AudioWave = new Bitmap(PanelAudioBox.Width, KaraokeAudioBox.Height, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);
            double begin = Tiempo.TimeToSecondDouble(infoTiempoInicial.Text);
            double off = Tiempo.TimeToSecondDouble(infoTiempoFinal.Text); // no es un puto offset, porke cojones lo habre llamado asi ? ¬¬

            if (off-begin <= 0)
            {
                mW.errorMsg("La línea tiene tiempo negativo o nulo y no se puede mostrar");
                return;
            }

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

                if (size > 0)
                {

                    bufferEntero = new byte[size];
                    IntPtr buff = new IntPtr(buf);

                    Marshal.Copy(buff, bufferEntero, 0, size);
                    AudioWave = avs2ds.DrawWave(bufferEntero, PanelAudioBox.Width, PanelAudioBox.Height, (double)zoomKaraokeTrackBar.Value / 100, (int)WaveStyle);

                    
                    DrawAudioGrid();
                    DrawK();

                    if (KaraokeAudioBox.Focused)
                    {
                        using (Graphics g = Graphics.FromImage((Image)AudioWave))
                        {
                            g.DrawRectangle(new Pen(new SolidBrush(Color.Fuchsia),2),0,0,PanelAudioBox.Width-5,PanelAudioBox.Height-5);
                        }
                    }

                    KaraokeAudioBox.Image = AudioWave;
                }

            }
        }


        private void DrawK()
        {            
            double principio = Tiempo.TimeToSecondDouble(infoTiempoInicial.Text);
            double fin = Tiempo.TimeToSecondDouble(infoTiempoFinal.Text);
            double duracion = fin - principio;

            int num_ticks = (int)Math.Round(duracion / 0.10);
            double bleh = (double)PanelAudioBox.Width / (double)num_ticks;
            
            int anterior = 0;
            int idx = 0;

            SensitivePoints.Clear();

            using (Graphics g = Graphics.FromImage((Image)AudioWave))
            {
                foreach (K kkk in OurK)
                {
                    int nuevo_in = (int)Math.Round((double)anterior/10 * bleh);
                    int nuevo_fin = nuevo_in + (int)Math.Round((((double)kkk.Milliseconds/10) *bleh));

                    nuevo_fin = (nuevo_fin > PanelAudioBox.Width-4) ? PanelAudioBox.Width-4 : nuevo_fin;

                    //rectangulo
                    Color FillColor;
                    try
                    {
                        FillColor = CheckButtonIndex(idx) ? Color.FromArgb(125, Color.Red) : Color.FromArgb(125, Color.Gray);
                    }
                    catch { FillColor = Color.Gray; }

                    g.FillRectangle(new SolidBrush(Color.FromArgb(120, FillColor)), nuevo_in, 0, nuevo_fin - nuevo_in, PanelAudioBox.Height);

                    //bordes
                    g.DrawLine(new Pen(new SolidBrush(Color.Red), 2), new Point(nuevo_in, 0), new Point(nuevo_in, AudioWave.Height));
                    SensitivePoints.Add(nuevo_in);
                    SensitivePoints.Add(nuevo_in-1);

                    if (idx == OurK.Count - 1) //solo a la ultima
                    { 
                        g.DrawLine(new Pen(new SolidBrush(Color.Green), 2), new Point(nuevo_fin, 0), new Point(nuevo_fin, AudioWave.Height));
                        SensitivePoints.Add(nuevo_fin);
                        SensitivePoints.Add(nuevo_fin-1);
                    }


                    //textos
                    Font f = new Font("Tahoma", 9 ,FontStyle.Bold);
                    string texto = kkk.Milliseconds.ToString();

                    Rectangle r = new Rectangle(nuevo_in, 0, nuevo_fin - nuevo_in, Convert.ToInt32((double)KaraokeAudioBox.Height*0.75));
                    StringFormat sf = new StringFormat(StringFormat.GenericTypographic);
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Far;

                    g.DrawString(texto, f, new SolidBrush(Color.White), r, sf);

                    r.Height = KaraokeAudioBox.Height;
                    f = new Font("Arial", 10, FontStyle.Regular);
                    texto = kkk.Text.ToString();
                    sf.LineAlignment = StringAlignment.Center; 

                    g.DrawString(texto, f, new SolidBrush(Color.YellowGreen), r, sf);

                    anterior += kkk.Milliseconds;
                    idx++;
                }
            }

        }

        #endregion

        #region FUNCIONES

        private void SepararSilabas()
        {
            if (SelectedSyl == -1) return;
            K mec = (K)OurK[SelectedSyl];
            sliceTextBox.Text = new SeparaSilabas(mec.Text).Separa();
            SliceSelected();
        }


        private string GetKKKtext(int idx)
        {
            if (idx > OurK.Count) return "";
            K tmp = (K)OurK[idx];
            return tmp.Text;
        }

        private int GetMSRegion(int x)
        {
            int anterior = 0; 
            for (int i = 0; i < OurK.Count; i++)
            {
                K tmpK = (K)OurK[i];

                if (x >= anterior && x < anterior + tmpK.Milliseconds)
                    return i;

                anterior += tmpK.Milliseconds;
            }
            return OurK.Count; //a sabiendas de ke es mas
        }

        private int GetRegionStartPoint(int reg)
        {
            int guarda = 0;
            for (int i = 0; i < reg; i++)
            {
                K tmp = (K)OurK[i];
                guarda += tmp.Milliseconds;
            }
            return guarda;
        }


        private bool CheckButton(Button b)
        {
            return (b.BackColor == Selected);
        }

        private bool CheckButtonIndex(int i)
        {
            if (i > SliceHandler.Controls.Count - 1) throw new Exception("Nooope!");
            Button b = (Button)SliceHandler.Controls[i];
            return CheckButton(b);
        }

        private void SetButton(Button b, bool check)
        {
            b.BackColor = (check)? Selected : UnSelected;
        }

        private void ChangeButton(Button b)
        {
            if (b.BackColor == Selected)
                b.BackColor = UnSelected;
            else b.BackColor = Selected;
        }

        private void UpdateToolTips()
        {
            for (int i = 0; i < SliceHandler.Controls.Count; i++)
            {
                K kaaa = (K)OurK[i];
                toolTip1.SetToolTip(SliceHandler.Controls[i], "Duración : "+kaaa.Milliseconds);
            }
        }

        private ArrayList GetK(string s) // funcion repetida ya en 3 sitios, quiza habria k meterla en una clase de forma estatica
        {
            ArrayList Ks = new ArrayList();
            Regex r = new Regex(@"\{\\[kK]f?\d+\}");
            MatchCollection mc = r.Matches(s);

            for (int x = 0; x < mc.Count; x++)
            {
                Match m = mc[x];
                int fin_actual = m.Index + m.Length;
                string texto = "";

                // obtener texto :)
                if (x < mc.Count - 1)
                {
                    Match siguiente = mc[x + 1];
                    int principio_siguiente = siguiente.Index;
                    texto = s.Substring(fin_actual, principio_siguiente - fin_actual);
                }
                else
                    texto = s.Substring(fin_actual);


                // estilo K
                Regex styleh = new Regex("[a-zA-Z]+");
                Match sss = styleh.Match(m.ToString());
                string estilok = sss.ToString();

                // nº milisegundos
                Regex milis = new Regex(@"\d+");
                Match mmm = milis.Match(m.ToString());
                int milisec = int.Parse(mmm.ToString());

                Ks.Add(new K(0, estilok, milisec, texto));
            }
            return Ks;
        }

        private void FillPanelButtons()
        {
            SliceHandler.Controls.Clear();
            int n = OurK.Count; 
            int ideal_size = (int)Math.Round(((double)SliceHandler.Width / (double)n));

            int resto = SliceHandler.Width - (ideal_size * n);
            for (int i = 0; i < n; i++)
            {
                K theK = (K)OurK[i];
                Button b = new Button();
                b.BackColor = System.Drawing.Color.Brown;
                b.Font = new System.Drawing.Font("Tahoma", 9, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                b.Location = new System.Drawing.Point(3, 0);
                b.Name = "button"+i;
                b.Size = new System.Drawing.Size(ideal_size, SliceHandler.Height - 4);
                b.TabIndex = 0;
                b.Text = theK.Text;
                b.UseVisualStyleBackColor = false;
                b.Location = new Point(b.Width * i, b.Location.Y);
                b.Click += new EventHandler(K_ButtonClicked);
                b.FlatStyle = FlatStyle.Popup;

                SliceHandler.Controls.Add(b);
            }
            if (SliceHandler.Controls.Count > 0)
            {
                Button b = (Button)SliceHandler.Controls[SliceHandler.Controls.Count - 1];
                b.Width += resto;
                b.Width = b.Width - 4;
            }
            UpdateToolTips();
        }

        private void DisplayLine(int n)
        {
            if ((n<0)||(n>mW.al.Count-1)) return;

            lineaASS lass = (lineaASS)mW.al[n];
            infoLineaSeleccionada.Text = (n + 1) + " de " + mW.al.Count;
            infoTiempoInicial.Text = lass.t_inicial.ToString();
            infoTiempoFinal.Text = lass.t_final.ToString();
            selectedKaraokeLine.Text = lineaASS.cleanText(lass.texto);
            infoSeparador.Text = Separador.ToString();
            infoDuracionMS.Text = Convert.ToInt32((lass.t_final.getTiempo() - lass.t_inicial.getTiempo()) * 100).ToString();

            if (ActualLine != n)
            {
                sliceTextBox.Enabled = false;
                sliceTextBox.Text  = "";
            }

            ActualLine = n;                       

            OurK = GetK(lass.texto);
            if (OurK.Count == 0)
            {
                double inicial = lass.t_inicial.getTiempo();
                double final = lass.t_final.getTiempo();

                double duracion = (final-inicial)*100;
                if (duracion>0) // si no hay Ks, hacemos una gorda :)
                    OurK.Add(new K(0,"k",(int)duracion,lineaASS.cleanText(lass.texto)));
            }

            DrawAudioWave();
            FillPanelButtons();
            SelectedSyl = -1;
        }

        private void UnionSelected()
        {
            int x = -1;
            for (int i = 0; i < SliceHandler.Controls.Count; i++)
            {
                Button b = (Button)SliceHandler.Controls[i];
                if (CheckButton(b))
                {
                    x = i;
                    break;
                }
            }
            if (x == -1)
            {
                mW.errorMsg("No hay ninguna sílaba seleccionada");
                return;
            }

            int y = -1;
            for (int j = SliceHandler.Controls.Count - 1; j >= 0; j--)
            {
                Button b = (Button)SliceHandler.Controls[j];
                if (CheckButton(b))
                {
                    y = j;
                    break;
                }
            }

            if (y > x)
            {
                int new_ms = 0;
                string new_text = "";

                ArrayList newOurK = new ArrayList();
                for (int k = 0; k < OurK.Count; k++)
                {
                    K temp = (K)OurK[k];
                    if (k < x) newOurK.Add(temp); // estamos antes
                    else if ((k>=x) && (k<=y)) 
                    { // estamos ahi
                        new_ms += temp.Milliseconds;
                        new_text += temp.Text;

                        if (k == y) // .. y ademas es la ultima iteracion
                        {
                            newOurK.Add(new K(0,"k",new_ms,new_text));
                        }
                    }
                    else if (k>y) newOurK.Add(temp); // despues

                }
                OurK = newOurK;
                FillPanelButtons();
                DrawAudioWave();
                
            }
            else mW.errorMsg("¿Cómo piensas unir una sílaba a sí misma? ¿Llamo a Pepe Gotera y Otilio?");

        }

        private void SliceSelected()
        {
            string[] result = sliceTextBox.Text.Split(Separador);

            if (result.Length == 1) return;

            K slicedK = (K)OurK[SelectedSyl];
            int new_ms = slicedK.Milliseconds / result.Length;
            int resto = slicedK.Milliseconds % result.Length;

            ArrayList newKs = new ArrayList();
            for (int i=0; i<result.Length; i++)
            {
                newKs.Add(new K(0,"k",new_ms,result[i]));
            }
            if (result.Length>0 && resto>0) 
            {
                K ultimoK = (K)newKs[result.Length-1];
                ultimoK.Milliseconds += resto;
            }
            OurK.RemoveAt(SelectedSyl);
            OurK.InsertRange(SelectedSyl,newKs);

            FillPanelButtons();
            DrawAudioWave();            
        }

        private void PlayAllEx(PlayOptions options)
        {
            if (SelectedSyl == -1) return;

            if (PlayStatus == ReproductionState.Play)
            {
                mW.audio.Stop();
                PlayStatus = ReproductionState.Stop;
                return;
            }

            int ms_iniciales = 0, ms_finales = 0;
            for (int i = 0; i < SelectedSyl; i++)
            {
                K tmp = (K)OurK[i];
                ms_iniciales += tmp.Milliseconds;
            }
            K sel = (K)OurK[SelectedSyl];
            ms_finales = ms_iniciales + sel.Milliseconds;

            double begin_off = (double)ms_iniciales / 100;
            double end_off = (double)ms_finales / 100;

            Mode = PlayMode.Other;
            PlayStatus = ReproductionState.Play;

            double begin = Tiempo.TimeToSecondDouble(infoTiempoInicial.Text) + begin_off;
            double off = Tiempo.TimeToSecondDouble(infoTiempoInicial.Text) + end_off; //Tiempo.TimeToSecondDouble(infoTiempoFinal.Text); 

            switch (options)
            {
                case PlayOptions.BeforeStart:
                    off = begin;
                    begin = Math.Max(0, off - .5);
                    break;

                case PlayOptions.BeforeEnd:
                    begin = Math.Max(0, off - .5);
                    break;

                case PlayOptions.AfterStart:
                    off = begin + .5;
                    break;

                case PlayOptions.AfterEnd:
                    begin = off;
                    off = begin + .5;
                    break;
            }
            PosRel = begin - Tiempo.TimeToSecondDouble(infoTiempoInicial.Text);
            LastEnd = off-Tiempo.TimeToSecondDouble(infoTiempoInicial.Text);
            unsafe
            {
                byte* buf = (byte*)AudioFull.ToPointer();
                byte* buf_final = buf;

                int principio = (int)(begin * avsaudio.AudioSampleRate * avsaudio.BytesPerSample);
                if (principio % 2 == 1) principio++;
                buf += principio;

                buf_final += (int)(off * avsaudio.AudioSampleRate * avsaudio.BytesPerSample);

                int size = (int)(buf_final - buf);
                if (size % 2 == 1) size++;

                if (size > 0)
                {

                    byte[] rangeplay = new byte[size];
                    IntPtr buff = new IntPtr(buf);

                    Marshal.Copy(buff, rangeplay, 0, size);

                    mW.audio = avs2ds.PreparaAudio(rangeplay);
                    AudioActualPosition_Refresh.Enabled = true;
                    mW.audio.Play(0, BufferPlayFlags.Default);

                }

            }



        }

        private void PlayAll()
        {

            if (PlayStatus == ReproductionState.Play)
            {
                mW.audio.Stop();
                PlayStatus = ReproductionState.Stop;
                return;
            }

            LastPosition = -1;

            PlayStatus = ReproductionState.Play;
            Mode = PlayMode.Full;
            AudioActualPosition_Refresh.Enabled = true;
            double principio = Tiempo.TimeToSecondDouble(infoTiempoInicial.Text);
            double fin = Tiempo.TimeToSecondDouble(infoTiempoFinal.Text);
            double duracion = fin - principio;

            mW.audio = avs2ds.PreparaAudio(bufferEntero);
            mW.audio.Play(0, BufferPlayFlags.Default);
        }

        private void PlaySelected()
        {
            if (SelectedSyl == -1) return;

            if (PlayStatus == ReproductionState.Play)
            {
                mW.audio.Stop();
                PlayStatus = ReproductionState.Stop;
                return;
            }

            int ms_iniciales = 0, ms_finales = 0;
            for (int i = 0; i < SelectedSyl; i++)
            {
                K tmp = (K)OurK[i];
                ms_iniciales += tmp.Milliseconds;
            }
            K sel = (K)OurK[SelectedSyl];
            ms_finales = ms_iniciales + sel.Milliseconds;

            double begin_off = (double)ms_iniciales / 100;
            double end_off = (double)ms_finales / 100;
            
            PosRel = begin_off;

            Mode = PlayMode.Selection;
            PlayStatus = ReproductionState.Play;
            //AudioActualPosition_Refresh.Enabled = true;

            // y aki va el play -_-U
            double begin = Tiempo.TimeToSecondDouble(infoTiempoInicial.Text) + begin_off;
            double off = Tiempo.TimeToSecondDouble(infoTiempoInicial.Text) + end_off; //Tiempo.TimeToSecondDouble(infoTiempoFinal.Text); 

            unsafe
            {
                byte* buf = (byte*)AudioFull.ToPointer();
                byte* buf_final = buf;

                int principio = (int)(begin * avsaudio.AudioSampleRate * avsaudio.BytesPerSample);
                if (principio % 2 == 1) principio++;
                buf += principio;

                buf_final += (int)(off * avsaudio.AudioSampleRate * avsaudio.BytesPerSample);

                int size = (int)(buf_final - buf);
                if (size % 2 == 1) size++;

                if (size > 0)
                {

                    byte[] rangeplay = new byte[size];
                    IntPtr buff = new IntPtr(buf);

                    Marshal.Copy(buff, rangeplay, 0, size);

                    mW.audio = avs2ds.PreparaAudio(rangeplay);
                    AudioActualPosition_Refresh.Enabled = true;
                    mW.audio.Play(0, BufferPlayFlags.Default);

                }

            }

        }

        private void SliceSlice()
        {
            if (sliceTextBox.Enabled)
            {
                SliceSelected();
                sliceTextBox.Text = "";
                sliceTextBox.Enabled = false;
            }
            else
            {
                sliceTextBox.Enabled = true;
                sliceTextBox.Focus();
                sliceTextBox.Select(0, 0);
            }

        }

        private void SliceWords()
        {
            sliceTextBox.Enabled = true;
            sliceTextBox.Text = sliceTextBox.Text.Replace(" ", " |");
            SliceSelected();
            sliceTextBox.Text = "";
            sliceTextBox.Enabled = false;

        }

        private void SelectNext()
        {
            DisplayLine(ActualLine + 1);
        }

        private void SelectPrev()
        {
            DisplayLine(ActualLine - 1);
        }


        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            CommitChanges();
            SelectPrev();            
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            SelectNext();            
        }


        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            CommitChanges();
            SelectNext();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            FillPanelButtons();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            PlayAll();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            SliceSlice();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            UnionSelected();
        }


        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            PlaySelected();
        }

        private void CommitChanges()
        {
            lineaASS lass = (lineaASS)mW.al[ActualLine];

            mW.UndoRedo.AddUndo(mW.script, "Cambios en línea de karaoke " + ActualLine);

            string res = "";
            foreach (K kkk in OurK)
                res += kkk.ToString();
            lass.texto = res;
            mW.updateGridWithArrayList(mW.al);
            
        }
        private void btn1_Click(object sender, EventArgs e)
        {
            CommitChanges();
        }

        private void btn3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Estás seguro de querer salir?", "perrySub", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                this.Dispose();
        }

        private void btn2_Click(object sender, EventArgs e)
        {
            CommitChanges();
            mW.saveFile(mW.openFile);

        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            SliceWords();
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            switch (WaveStyle)
            {
                case WaveType.Normal:
                    WaveStyle = WaveType.FFT;
                    break;
                case WaveType.FFT:
                    WaveStyle = WaveType.FFTWave;
                    break;
                case WaveType.FFTWave:
                    WaveStyle = WaveType.Normal;
                    break;
            }
            DrawAudioWave();
        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            if (SelectedSyl == -1) return;
            sliceTextBox.Enabled = true;
            sliceTextBox.Text = "|" + sliceTextBox.Text;
            SliceSelected();
            sliceTextBox.Text = "";
            sliceTextBox.Enabled = false;

        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            if (SelectedSyl == -1) return;
            sliceTextBox.Enabled = true;
            sliceTextBox.Text = sliceTextBox.Text + "|";
            SliceSelected();
            sliceTextBox.Text = "";
            sliceTextBox.Enabled = false;

        }
        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            SepararSilabas();
        }

    #endregion

    }
}