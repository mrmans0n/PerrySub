using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace scriptASS
{
    public partial class bitrateCalcW : Form
    {
        mainW mW;

        private const double MP4_NoBFrames = 4.3;
        private const double MP4_BFrames = 10.4;
        private const double AVI_Fixed = 24;
        private const double AVI_MP3_CBR = 23.75;
        private const double AVI_MP3_VBR = 40;
        private const double AVI_AC3 = 23.75;
        private const double MKV_AudioHeader = 140;
        private const double MKV_VorbisHeader = 4096;
        private const double MKV_IFrame = 26;
        private const double MKV_PFrame = 13;
        private const double MKV_BFrame = 16;
        private const double MKV_AAC_Block = 1024;
        private const double MKV_AC3_Block = 1536;
        private const double MKV_MP3_Block = 1152;
        private const double MKV_OGG_Block = 1024;

        public double Fps
        {
            get { return estiloV4.s2d(textFPS.Text); }
            set { 
                textFPS.Text = estiloV4.d2s(value);
            }
        }        

        public int FrameTotal
        {
            get { return int.Parse(textFrames.Text); }
            set { 
                textFrames.Text = value.ToString();
            }
        }

        public bitrateCalcW(mainW mw, double fps, int frametotal)
        {
            InitializeComponent();
            mW = mw;
            Fps = (fps!=-1)? fps : 23.976;
            FrameTotal = (frametotal!=-1)? frametotal : 1000;
            textFrames.TextChanged += new EventHandler(InfoChanged);
            textFPS.TextChanged += new EventHandler(InfoChanged);
            comboAudioFormat.SelectedIndexChanged += new EventHandler(InfoChanged);
            comboAudioBitrate.SelectedIndexChanged += new EventHandler(InfoChanged);
            textSize.TextChanged += new EventHandler(InfoChanged);
            textSize.KeyPress += new KeyPressEventHandler(textSize_KeyPress);
            comboContainer.SelectedIndexChanged += new EventHandler(InfoChanged);
            textTargetSize.TextChanged += new EventHandler(InfoChanged);
            hasBFrames.CheckedChanged += new EventHandler(InfoChanged);            

            calculate();
        }

        void textSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (Convert.ToInt32(e.KeyChar))
            {
                case 13:
                    calculate();
                    e.Handled = true;
                    SendKeys.Send("");
                    break;
            }
        }

        void InfoChanged(object sender, EventArgs e)
        {
            calculate();
        }

        private void calculate()
        {
            try
            {
                int f = int.Parse(textFrames.Text);
                double fps = estiloV4.s2d(textFPS.Text);
                double n_seg = (double)((double)f / fps); // segundos que dura el video

                double tam_audio = (isBitRate.Checked) ? n_seg * (double.Parse(comboAudioBitrate.Text) / 8) : double.Parse(textSize.Text) / 1024;
                double tam_kb = estiloV4.s2d(textTargetSize.Text) * 1024; // kb deseados


                switch (comboContainer.Text)
                {
                    case "AVI" :
                        tam_kb -= (AVI_Fixed * f)/1024.0;
                        switch (comboAudioFormat.Text)
                        {
                            case "AAC":
                            case "MP3 VBR":
                                tam_kb -= (AVI_MP3_VBR * f) / 1024.0;
                                break;
                            case "MP3 CBR":
                                tam_kb -= (AVI_MP3_CBR * f) / 1024.0;
                                break;
                            case "AC3":
                                tam_kb -= (AVI_AC3 * f) / 1024.0;
                                break;
                        }
                        break;
                    case "MKV" :
                        double iF = f / 10;
                        double bF = (hasBFrames.Checked) ? (f - iF) / 2 : 0;
                        double pF = f - iF - bF;
                        tam_kb -= (5700 + iF * MKV_IFrame + bF * MKV_BFrame + pF * MKV_PFrame)/1024.0;
                        tam_kb -= ((n_seg/2.0) *12)/1024.0;

                        double sampling = 48000;
                        double samples = sampling * n_seg;
                        double audio_head = MKV_AudioHeader;
                        double sxblock = 0.0;

                        switch (comboAudioFormat.Text)
                        {
                            case "MP3 VBR":
                            case "MP3 CBR":
                                sxblock = MKV_MP3_Block;
                                break;
                            case "AAC":
                                sxblock = MKV_AAC_Block;
                                break;
                            case "AC3":
                                sxblock = MKV_AC3_Block;
                                break;
                            case "OGG":
                                sxblock = MKV_OGG_Block;
                                audio_head += MKV_VorbisHeader;
                                break;
                        }

                        double block_overhead = (samples / sxblock) * 22.0 / 8.0;
                        tam_kb -= (audio_head + (5 * n_seg + block_overhead)) / 1024.0;
                        break;
                    case "MP4" :
                        tam_kb -= (hasBFrames.Checked) ? (MP4_BFrames * f)/1024 : (MP4_NoBFrames * f)/1024.0 ;
                        break;
                }

                double res = Math.Round(((tam_kb - tam_audio) / n_seg) * 8);
                textBitrate.Text = (res>0) ? res.ToString() : "ERROR";
            }
            catch { textBitrate.Text = "ERROR"; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.Filter = "Formatos de Audio (*.aac;*.mp4;*.mp3;*.ogg)|*.aac;*.mp4;*.mp3;*.ogg";
            if (o.ShowDialog() == DialogResult.OK)
            {
                FileInfo f = new FileInfo(o.FileName);
                textSize.Text = f.Length.ToString();
            }
        }

        private void bitrateCalcW_Load(object sender, EventArgs e)
        {

        }
    }
}