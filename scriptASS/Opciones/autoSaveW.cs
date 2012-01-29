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
    public partial class autoSaveW : Form
    {
        mainW mW;
        public autoSaveW(mainW mw)
        {
            InitializeComponent();
            mW = mw;

        }

        private void RellenaLista()
        {
            DirectoryInfo d = new DirectoryInfo(mW.autosaveDir);
            foreach (FileInfo f in d.GetFiles("*.AUTOSAVE.zip"))
            {
                AutoSaveList.Items.Add(f.Name);
            }            
            foreach (FileInfo f in d.GetFiles("*.AUTOSAVE"))
            {
                AutoSaveList.Items.Add(f.Name);
            }

        }

        private void autoSaveW_Load(object sender, EventArgs e)
        {
            this.AutoSaveList.SelectedIndexChanged += new EventHandler(AutoSaveList_SelectedIndexChanged);
            RellenaLista();
        }

        void AutoSaveList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AutoSaveList.SelectedIndex == -1) return;

            FileInfo info = new FileInfo(mW.autosaveDir + "\\" + AutoSaveList.Items[AutoSaveList.SelectedIndex].ToString());
            SubtitleScript script;
            if (info.Name.EndsWith(".zip", StringComparison.InvariantCultureIgnoreCase))
                script = new ZipSubtitleScript(info.FullName);
            else 
                script = new SubtitleScript(info.FullName);

            label1.Text = "Lineas: " + script.LineCount;
            label2.Text = "Estilos: " + script.StyleCount;
            label3.Text = "Fecha: " + info.LastWriteTime.Day.ToString() + "/" + info.LastWriteTime.Month.ToString()+"/"+info.LastWriteTime.Year.ToString();
            label4.Text = "Tamaño: " + info.Length/1024 +"KB";
            label5.Text = "Hora: " + info.LastWriteTime.TimeOfDay.ToString();
            label6.Text = "Adjuntos: "+( (script.HasAttachments)? "Sí ("+script.AttachmentCount+" archivo/s)" : "No");

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FileInfo info = new FileInfo(Path.Combine(mW.autosaveDir,AutoSaveList.Items[AutoSaveList.SelectedIndex].ToString()));

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = info.Name.Replace(".TranslateW.AUTOSAVE", "").Replace(".perrySub.AUTOSAVE", "").Replace(".zip", "");
            sfd.Filter = "Archivo ASS (*.ass)|*.ass";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                SubtitleScript script;
                if (info.Name.EndsWith(".zip", StringComparison.InvariantCultureIgnoreCase))
                    script = new ZipSubtitleScript(info.FullName);
                else
                    script = new SubtitleScript(info.FullName);
                script.SaveToFile(sfd.FileName);
                if (MessageBox.Show("Archivo restaurado correctamente.\n¿Deseas cargarlo en el programa principal?", mainW.appTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    mW.openFile = sfd.FileName;
                    mW.loadFile(sfd.FileName);
                }
            }
        }

    }
}