using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace scriptASS
{
    public partial class cropW : Form
    {

        private enum DragType : int
        {
            SW = 1,
            S = 2,
            SE = 3,
            W = 4,
            None = 5,
            E = 6,
            NW = 7,
            N = 8,
            NE = 9
        }

        DragType tipo = DragType.None;
        AviSynthClip clip;
        avsW aW;
        Bitmap CleanImage;

        bool IsPushed = false;

        public cropW(avsW aW, AviSynthClip clip)
        {
            InitializeComponent();
            this.aW = aW;
            this.clip = clip;

            this.Disposed += new EventHandler(cropW_Disposed);

            VideoBox.MouseDown += new MouseEventHandler(VideoBox_MouseDown);
            VideoBox.MouseUp += new MouseEventHandler(VideoBox_MouseUp);
            VideoBox.MouseMove += new MouseEventHandler(VideoBox_MouseMove);

            Izquierda.TextChanged += new EventHandler(TxtChange);
            Derecha.TextChanged += new EventHandler(TxtChange);
            Arriba.TextChanged += new EventHandler(TxtChange);
            Abajo.TextChanged += new EventHandler(TxtChange);

            FrameActual.KeyPress += new KeyPressEventHandler(FrameActual_KeyPress);
        }

        void FrameActual_KeyPress(object sender, KeyPressEventArgs e)
        {
            int k = Convert.ToInt32(e.KeyChar);

            if (k == 13)
            {
                int frame = int.Parse(FrameActual.Text);
                int final = int.Parse(FrameFinal.Text);

                if (frame > final)
                {
                    frame = final;
                    FrameActual.Text = FrameFinal.Text;
                }

                CleanImage = AviSynthFunctions.getBitmapFromFrame(clip, 0, frame);
                DrawLines();
                
                SendKeys.Send("");
                e.Handled = true;                
            }


        }

        void TxtChange(object sender, EventArgs e)
        {
            try
            {
                DrawLines();
            }
            catch { } 
        }

        void VideoBox_MouseMove(object sender, MouseEventArgs e)
        {
            int arriba = int.Parse(Arriba.Text);
            int abajo = VideoBox.Height - int.Parse(Abajo.Text) - 1;
            int izquierda = int.Parse(Izquierda.Text);
            int derecha = VideoBox.Width - int.Parse(Derecha.Text) - 1;

            if (!IsPushed)
            {
                VideoBox.Cursor = Cursors.Default;

                if (e.X == izquierda)
                {
                    tipo = DragType.W;
                    VideoBox.Cursor = Cursors.SizeWE;

                    if (e.Y == arriba)
                    {
                        VideoBox.Cursor = Cursors.SizeNWSE;
                        tipo = DragType.NW;
                    }
                    if (e.Y == abajo)
                    {
                        tipo = DragType.SW;
                        VideoBox.Cursor = Cursors.SizeNESW;
                    }

                }

                if (e.X == derecha)
                {
                    tipo = DragType.E;
                    VideoBox.Cursor = Cursors.SizeWE;

                    if (e.Y == arriba)
                    {
                        VideoBox.Cursor = Cursors.SizeNESW;
                        tipo = DragType.NE;
                    }
                    if (e.Y == abajo)
                    {
                        VideoBox.Cursor = Cursors.SizeNWSE;
                        tipo = DragType.SE;
                    }

                }

                if ((VideoBox.Cursor == Cursors.Default) && ((e.Y == arriba) || (e.Y == abajo)))
                {
                    VideoBox.Cursor = Cursors.SizeNS;
                    if (e.Y == arriba) tipo = DragType.N;
                    else tipo = DragType.S;
                }

                    
            }
            else
            {
                switch (tipo)
                {
                    case DragType.W:
                        Izquierda.Text = e.X.ToString();
                        break;
                    case DragType.NW:
                        Izquierda.Text = e.X.ToString();
                        Arriba.Text = e.Y.ToString();
                        break;
                    case DragType.SW:
                        Izquierda.Text = e.X.ToString();
                        Abajo.Text = Convert.ToString(VideoBox.Height - e.Y);
                        break;
                    case DragType.N:
                        Arriba.Text = e.Y.ToString();
                        break;
                    case DragType.S:
                        Abajo.Text = Convert.ToString(VideoBox.Height - e.Y);
                        break;
                    case DragType.E:
                        Derecha.Text = Convert.ToString(VideoBox.Width - e.X);
                        break;
                    case DragType.NE:
                        Arriba.Text = e.Y.ToString();
                        Derecha.Text = Convert.ToString(VideoBox.Width - e.X);
                        break;
                    case DragType.SE:
                        Abajo.Text = Convert.ToString(VideoBox.Height - e.Y);
                        Derecha.Text = Convert.ToString(VideoBox.Width - e.X);
                        break;
                }
                
                if ((e.X < 0 || e.X >VideoBox.Width) || (e.Y < 0 || e.Y > VideoBox.Height))
                    FixValues();
                DrawLines();

            }

        }

        void VideoBox_MouseUp(object sender, MouseEventArgs e)
        {
            IsPushed = false;
            VideoBox.Cursor = Cursors.Default;
            tipo = DragType.None;
        }

        void VideoBox_MouseDown(object sender, MouseEventArgs e)
        {
            IsPushed = true;
        }

        private void FixValues()
        {
            int iz = int.Parse(Izquierda.Text);
            int de = int.Parse(Derecha.Text);
            int ar = int.Parse(Arriba.Text);
            int ab = int.Parse(Abajo.Text);

            if (iz < 0) Izquierda.Text = "0";
            if (de < 0) Derecha.Text = "0";
            if (ar < 0) Arriba.Text = "0";
            if (ab < 0) Abajo.Text = "0";

        }

        private void DrawLines()
        {
            int arriba = int.Parse(Arriba.Text);
            int ab = int.Parse(Abajo.Text);
            int abajo = VideoBox.Height - ab - 1;
            int izquierda = int.Parse(Izquierda.Text);
            int de = int.Parse(Derecha.Text);
            int derecha = VideoBox.Width - de - 1;

            Bitmap b = new Bitmap(CleanImage);
            using (Graphics g = Graphics.FromImage((Image)b))
            {
                if (arriba > 0)
                    g.FillRectangle(new SolidBrush(Color.Crimson), 0, 0, VideoBox.Width, arriba);
                if (ab > 0)
                    g.FillRectangle(new SolidBrush(Color.Crimson), 0, abajo, VideoBox.Width, VideoBox.Height);
                if (izquierda > 0)
                    g.FillRectangle(new SolidBrush(Color.Crimson), 0, 0, izquierda, VideoBox.Height);
                if (de > 0)
                    g.FillRectangle(new SolidBrush(Color.Crimson), derecha, 0, VideoBox.Width, VideoBox.Height);
            }

            VideoBox.Image = b;
        }

        void cropW_Disposed(object sender, EventArgs e)
        {
            aW.Enabled = true;
            aW.Focus();
        }

        private void LoadVideo()
        {
            VideoBox.Location = new Point(1, 1);
            VideoBox.Size = new Size(clip.VideoWidth, clip.VideoHeight);
            VideoBox.Image = AviSynthFunctions.getBitmapFromFrame(clip, 0, 0);
            CleanImage = new Bitmap(VideoBox.Image);
            FrameActual.Text = "0";
            FrameFinal.Text = Convert.ToString(clip.num_frames-1);
            Arriba.Text = Abajo.Text = Izquierda.Text = Derecha.Text = "0";
        }

        private void AdjustSize()
        {            
            ControlBox.Width = clip.VideoWidth;
            ControlBox.Location = new Point(VideoBox.Location.X, VideoBox.Location.Y + VideoBox.Height);
            this.Size = new Size(VideoBox.Location.X + VideoBox.Width + 7, ControlBox.Location.Y + ControlBox.Height+25);
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;

            int frm_width = this.Width;
            int frm_height = this.Height;

            System.Windows.Forms.Screen src = System.Windows.Forms.Screen.PrimaryScreen;
            int src_height = src.Bounds.Height;
            int src_width = src.Bounds.Width;

            this.Left = (src_width - frm_width) / 2;
            this.Top = (src_height - frm_height) / 2;

        }

        private void cropW_Load(object sender, EventArgs e)
        {
            try
            {
                LoadVideo();
                AdjustSize();
            }
            catch
            {
                aW.mW.errorMsg("Debes tener cargado el vídeo para hacer esto.\nPulsa el botón de Preview, o la tecla F5 para cargarlo.");
                this.Dispose();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            aW.InsertAVSCode("Crop(" + Izquierda.Text + "," + Arriba.Text + ",-" + Derecha.Text + ",-" + Abajo.Text + ")");
            this.Dispose();
        }
    }
}