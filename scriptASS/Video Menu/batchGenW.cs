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
    public partial class batchGenW : Form
    {
        mainW mW;

        public batchGenW(mainW mw)
        {
            InitializeComponent();

            this.mW = mw;
        }

        private void batchGenW_Load(object sender, EventArgs e)
        {
            this.MaximumSize = this.MinimumSize = this.Size;
            try { x264_path.Text = mW.getFromConfigFile("batchGenW_x264"); } catch {}
            try { besweet_path.Text = mW.getFromConfigFile("batchGenW_BeSweet"); } catch {} 
            try { mplayer_path.Text = mW.getFromConfigFile("batchGenW_MPlayer"); } catch {}
            try { mp4box_path.Text = mW.getFromConfigFile("batchGenW_MP4Box"); } catch { }
            try { x264_FirstPass.Text = mW.getFromConfigFile("batchGenW_x264_1stpass"); } catch { }
            try { x264_SecondPass.Text = mW.getFromConfigFile("batchGenW_x264_2ndpass"); } catch { } 
            try { mp4box_path.Text = mW.getFromConfigFile("batchGenW_MP4Box"); } catch { } 

            

            AviSynthTemplate.Text = "AviSource(\"%filename\",false)\nConvertToYV12";
            AviSynthTemplate.ForceRefresh();
        }

        private void btn3_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "Archivos de vídeo (*.avi;*.mkv;*.mp4;*.wmv)|*.avi;*.mkv;*.mp4;*.wmv";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string s in ofd.FileNames)
                    FileQueue.Items.Add(s);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < FileQueue.SelectedItems.Count; i++)
                FileQueue.Items.RemoveAt(FileQueue.SelectedIndices[i]);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "x264.exe (x264.exe)|x264.exe";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                mW.updateConcatenateConfigFile("batchGenW_x264", ofd.FileName);
                x264_path.Text = ofd.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "BeSweet.exe (BeSweet.exe)|BeSweet.exe";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                mW.updateConcatenateConfigFile("batchGenW_BeSweet", ofd.FileName);
                besweet_path.Text = ofd.FileName;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "MPlayer.exe (MPlayer.exe)|MPlayer.exe";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                mW.updateConcatenateConfigFile("batchGenW_MPlayer", ofd.FileName);
                mplayer_path.Text = ofd.FileName;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "MP4Box.exe (MP4Box.exe)|MP4Box.exe";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                mW.updateConcatenateConfigFile("batchGenW_MP4Box", ofd.FileName);
                mp4box_path.Text = ofd.FileName;
            }        

        }

        private void button8_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Archivos BAT (*.bat)|*.bat";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                TargetFile.Text = sfd.FileName;
            }
        }

        private void GenerateAVSCode(string filename)
        {
            FileInfo fi = new FileInfo(filename);
            FileInfo newfile = new FileInfo(TargetFile.Text);
            string avsfile = fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length)+".avs";
            string directorio = newfile.DirectoryName;

            TextWriter o = new StreamWriter(directorio+"\\"+avsfile, false, System.Text.Encoding.UTF8);
            o.WriteLine(AviSynthTemplate.Text.Replace("%filename", filename));
            o.Close();
        }

        private void GeneratePartialCode(int idx, string filename,TextWriter o)
        {
            idx++;
            FileInfo fi = new FileInfo(filename);
            FileInfo newfile = new FileInfo(TargetFile.Text);

            //string archivosinextension = fi.FullName.Substring(0, fi.FullName.Length - fi.Extension.Length);
            string nombresinextension = fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);

            string montaje = newfile.DirectoryName + "\\" + nombresinextension;

            o.WriteLine("@title PerrySub Generated Batch File [AUDIO ENCODING] - Queue: " + idx + "/" + FileQueue.Items.Count);
            o.WriteLine("@echo Converting audio to PCM WAV");
            o.WriteLine("@\"" + mplayer_path.Text + "\" -vo null -vc dump -ao pcm \"" + filename+"\"");
            //o.WriteLine("@\"" + mplayer_path.Text + "\" -vo null -vc null -ao pcm:fast \"" + filename + "\"");
            o.WriteLine("@echo Converting audio WAV to LC_AAC");
            o.WriteLine("@\"" + besweet_path.Text + "\" -core( -input \"audiodump.wav\" -output \"" + montaje + "[audio].mp4\"  -logfile \"" + montaje + "[audio].log\" ) -azid( -s stereo -c normal -L -3db ) -bsn( -2ch -cbr " + AudioBitRate.Text + " -codecquality_high -aacprofile_lc ) -ota( -g max )");
            o.WriteLine("@del AudioDump.wav");            
            o.WriteLine("@title PerrySub Generated Batch File [VIDEO ENCODING (1st Pass)] - Queue: " + idx + "/" + FileQueue.Items.Count + "");
            o.WriteLine("@\"" + x264_path.Text + "\" --pass 1 --bitrate " + VideoBitRate.Text + " --stats \"" + montaje + ".log\" " + x264_FirstPass.Text + " --output NUL \"" + montaje + ".avs\"");
            o.WriteLine("@title PerrySub Generated Batch File [VIDEO ENCODING (2nd Pass)] - Queue: " + idx + "/" + FileQueue.Items.Count + "");
            o.WriteLine("@\"" + x264_path.Text + "\" --pass 3 --bitrate " + VideoBitRate.Text + " --stats \"" + montaje + ".log\" " + x264_SecondPass.Text + " --output \"" + montaje + ".264\" \"" + montaje + ".avs\"");
            o.WriteLine("@title PerrySub Generated Batch File [MUXING] - Queue: " + idx + "/" + FileQueue.Items.Count);
            o.WriteLine("@\"" + mp4box_path.Text + "\" -add \"" + montaje + ".264\" -add \"" + montaje + "[audio].mp4:lang=jap\" -fps " + FramesPerSecond.Text + " -new \"" + montaje + ".mp4\"\n");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string bat_file = "";

            // un poquito de error checking no vendria mal aqui
            if (!AviSynthTemplate.Text.Contains("%filename"))
            {
                mW.errorMsg("El archivo avisynth debe contener %filename para funcionar.");
                return;
            }

            if (FileQueue.Items.Count < 1)
            {
                mW.errorMsg("Debes añadir por lo menos 1 archivo a la lista.");
                return;
            }

            TextWriter o = new StreamWriter(TargetFile.Text, false, System.Text.Encoding.ASCII);

            for (int i = 0; i < FileQueue.Items.Count; i++)
            {
                string vid_name = FileQueue.Items[i].ToString();
                GenerateAVSCode(vid_name);
                GeneratePartialCode(i, vid_name, o);
            }
            
            o.WriteLine(bat_file);
            o.WriteLine("@pause");
            o.Close();

            mW.updateReplaceConfigFile("batchGenW_x264_1stpass", x264_FirstPass.Text);
            mW.updateReplaceConfigFile("batchGenW_x264_2ndpass", x264_SecondPass.Text);

            MessageBox.Show("Archivo .BAT creado con éxito.", "PerrySub", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void button9_Click(object sender, EventArgs e)
        {
            AviSynthTemplate.Text = "AviSource(\"%filename\",false)\nConvertToYV12";
        }

        private void button10_Click(object sender, EventArgs e)
        {
            AviSynthTemplate.Text = "DirectShowSource(\"%filename\",audio=false)\nConvertToYV12";
        }

        private void button11_Click(object sender, EventArgs e)
        {
            AviSynthTemplate.Text = "MPEG2Source(\"%filename\")\nConvertToYV12";
        }

        private void button12_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Archivo de AviSynth (*.avs)|*.avs";
            if (ofd.ShowDialog() == DialogResult.OK)
                AviSynthTemplate.LoadFile(ofd.FileName,RichTextBoxStreamType.PlainText);
        }
    }
}