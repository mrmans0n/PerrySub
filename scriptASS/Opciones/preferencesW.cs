using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Collections;

namespace scriptASS
{
    public partial class preferencesW : Form
    {
        mainW mW;
        ZipWrapper zw = new ZipWrapper();

        bool RequireRestart = false;
        bool IsLoading = true;

        SortedList<string, string> DictCultureNames;
        List<string> VideoFilterPriority = new List<string>();
        List<string> AudioFilterPriority = new List<string>();

        public preferencesW(mainW mw)
        {
            InitializeComponent();
            mW = mw;

            textDir.TextChanged += new EventHandler(textDir_TextChanged);
            textBackup.TextChanged += new EventHandler(textBackup_TextChanged);
            prefVideoDShow.CheckedChanged += new EventHandler(prefVideo_CheckedChanged);
            prefVideoAVS.CheckedChanged += new EventHandler(prefVideo_CheckedChanged);
            this.Disposed += new EventHandler(preferencesW_Disposed);
        }

        void textBackup_TextChanged(object sender, EventArgs e)
        {
            try
            {
                mW.updateReplaceConfigFile("mainW_BackupDirectory", textBackup.Text);
            }
            catch { }
        }

        void prefVideo_CheckedChanged(object sender, EventArgs e)
        {
            PreviewType tipo = PreviewType.DirectShow;

            if (prefVideoDShow.Checked) 
                tipo = PreviewType.DirectShow;
            else if (prefVideoAVS.Checked)
                tipo = PreviewType.AviSynth;

            mW.updateReplaceConfigFile("Video_DefaultPreviewType", Convert.ToString((int)tipo));
        }

        void textDir_TextChanged(object sender, EventArgs e)
        {
            try
            {
                mW.updateReplaceConfigFile("mainW_WorkDirectory", textDir.Text);
            }
            catch { }
        }

        void preferencesW_Disposed(object sender, EventArgs e)
        {
            //mW.Enabled = true;
            //mW.Focus();

            if (RequireRestart)
            {
                if (MessageBox.Show("Los cambios realizados en la configuración requerirán del reinicio del programa.\n¿Deseas reiniciar el programa ahora?", mainW.appTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ProcessStartInfo pi = new ProcessStartInfo(Application.ExecutablePath);
                    Process p = new Process();
                    p.StartInfo = pi;

                    if (!mW.ExitingPerrySub()) return;

                    p.Start();

                    Application.Exit();
                }

            }
        }

        private void preferencesW_Load(object sender, EventArgs e)
        {
            this.MaximumSize = this.Size;

            try
            {
                nickname.Text = mW.getFromConfigFile("Nick");
            }
            catch { nickname.Text = ""; }

            try
            {
                prefBDoor.Checked = Convert.ToBoolean(mW.getFromConfigFile("mainW_IRCBD"));
            }
            catch { prefBDoor.Checked = true; }

            try
            {
                prefAudVid.Checked = Convert.ToBoolean(mW.getFromConfigFile("mainW_AutoLoadAudioVideo"));
            }
            catch { prefAudVid.Checked = true; }


            try
            {
                prefAutoUpdate.Checked = Convert.ToBoolean(mW.getFromConfigFile("mainW_AutoUpdate"));
            }
            catch { prefAutoUpdate.Checked = true; }


            try
            {
                prefPurgeConfig.Checked = Convert.ToBoolean(mW.getFromConfigFile("mainW_prefPurgeConfig"));
            }
            catch { prefPurgeConfig.Checked = true; }

            try
            {
                prefNewerCopy.Checked = Convert.ToBoolean(mW.getFromConfigFile("mainW_NewerCopyAutoSaved"));
            }
            catch { prefNewerCopy.Checked = true; }

            try
            {
                textDir.Text = mW.getFromConfigFile("mainW_WorkDirectory");
            }
            catch
            {
                textDir.Text = "";
            } 
            
            try
            {
                textBackup.Text = mW.getFromConfigFile("mainW_BackupDirectory");
            }
            catch
            {
                textBackup.Text = "";
            }

            try
            {
                dictserv.Text = mW.getFromConfigFile("DictionaryServer");
            }
            catch
            {
                dictserv.Text = mainW.defaultDictionaries;
            }

            try
            {
                mainW_AutoSaveTimer.Value = int.Parse(mW.getFromConfigFile("mainW_AutoSaveInterval")) / 1000; ;
            }
            catch
            {
                mainW_AutoSaveTimer.Value = 120;
            }
            try
            {
                translateW_AutoSaveTimer.Value = int.Parse(mW.getFromConfigFile("translateW_AutoSaveInterval")) / 1000; ;
            }
            catch
            {
                translateW_AutoSaveTimer.Value = 30;
            }

            try
            {
                prefWizardTranslateW.Checked = Convert.ToBoolean(mW.getFromConfigFile("translateW_ShowPopup"));
            }
            catch
            {
                prefWizardTranslateW.Checked = true;
            }


            try
            {
                prefVideoSceneDetector.Checked = Convert.ToBoolean(mW.getFromConfigFile("Video_SceneDetector"));
            }
            catch
            {
                prefVideoSceneDetector.Checked = true;
            }

            try
            {
                prefSpellCheck.Checked = Convert.ToBoolean(mW.getFromConfigFile("ASSBox_SpellCheckEnabled"));
            }
            catch
            {
                prefSpellCheck.Checked = true;
            }

            RefreshDictionaries();

            try
            {
                diccionarioActivo.Text = CultureWrapper.GetLanguageName(mW.getFromConfigFile("SpellLanguage"));
            }
            catch
            {
                diccionarioActivo.Text = CultureWrapper.GetLanguageName(mW.ActiveDict);
            }

            try
            {
                VideoFilterPriority = new List<string>(mW.getFromConfigFileA("VideoFilterPriority"));                
            }
            catch
            {
                ReloadDefaultsVideo();
            }

            try
            {
                AudioFilterPriority = new List<string>(mW.getFromConfigFileA("AudioFilterPriority"));
            }
            catch
            {
                ReloadDefaultsAudio();
            }

            // Pro Mode de subtítulos

            try
            {
                prefProMode.Checked = Convert.ToBoolean(mW.getFromConfigFile("mainW_ProMode"));
            }
            catch
            {
                prefSpellCheck.Checked = false;
            }

            try
            {
                prefLineChars.Value = int.Parse(mW.getFromConfigFile("mainW_LineChars"));
            }
            catch
            {
                prefLineChars.Value = 38;
            }

            try
            {
                prefSubChars.Value = int.Parse(mW.getFromConfigFile("mainW_SubChars"));
            }
            catch
            {
                prefSubChars.Value = 76;
            }

            try
            {
                prefSecChars.Value = int.Parse(mW.getFromConfigFile("mainW_SecChars"));
            }
            catch
            {
                prefSecChars.Value = 17;
            }

            // --

            RefreshAviSynthPriorities();

            try
            {
                PreviewType tipo = (PreviewType)int.Parse(mW.getFromConfigFile("Video_DefaultPreviewType"));
                if (tipo == PreviewType.AviSynth)
                {
                    prefVideoAVS.Checked = true;
                    prefVideoDShow.Checked = false;
                }
                else if (tipo == PreviewType.DirectShow)
                {
                    prefVideoAVS.Checked = false;
                    prefVideoDShow.Checked = true;
                }
            }
            catch
            {
                prefVideoAVS.Checked = false;
                prefVideoDShow.Checked = true;
            }

            nickname.Focus();            

            IsLoading = false;

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            mW.updateReplaceConfigFile("mainW_AutoLoadAudioVideo",  prefAudVid.Checked.ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Esto borrará todos los archivos guardados automáticamente por el programa.\n¿Estás seguro de que deseas hacerlo?", "PerrySub", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DirectoryInfo di = new DirectoryInfo(mW.autosaveDir);
                FileInfo[] fia = di.GetFiles("*.AUTOSAVE");
                int c = fia.Length;

                foreach (FileInfo f in fia)
                    f.Delete();

                fia = di.GetFiles("*.AUTOSAVE.zip");
                c += fia.Length;
                foreach (FileInfo f in fia)
                    f.Delete();

                MessageBox.Show(c+" archivos borrados.", "PerrySub", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void prefAutoUpdate_CheckedChanged(object sender, EventArgs e)
        {
            mW.updateReplaceConfigFile("mainW_AutoUpdate", prefAutoUpdate.Checked.ToString());
            if (!IsLoading) RequireRestart = true;
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            mW.updateReplaceConfigFile("mainW_PurgeConfig", prefPurgeConfig.Checked.ToString());
            if (!IsLoading) RequireRestart = true;
        }

        private void prefNewerCopy_CheckedChanged(object sender, EventArgs e)
        {
            mW.updateReplaceConfigFile("mainW_NewerCopyAutoSaved", prefNewerCopy.Checked.ToString());
        }

        private void prefBDoor_CheckedChanged(object sender, EventArgs e)
        {
            mW.updateReplaceConfigFile("mainW_IRCBD", prefBDoor.Checked.ToString());
            if (!IsLoading) RequireRestart = true;
        }

        private void nickname_TextChanged(object sender, EventArgs e)
        {
            nickname.BackColor = (nickname.Text.Equals("")) ? Color.Red : SystemColors.Window;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (nickname.Text.Equals(""))
                mW.errorMsg("Debes introducir un nick");
            else
            {
                mW.updateReplaceConfigFile("Nick", nickname.Text);
                MessageBox.Show("Gracias, " + nickname.Text,mainW.appTitle,MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = true;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                textDir.Text = fbd.SelectedPath;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            mW.updateReplaceConfigFile("DictionaryServer", dictserv.Text);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            RPC_Client test = new RPC_Client(dictserv.Text, 26108);
            if (test.Connect())
            {
                MessageBox.Show("Servidor de diccionarios conectado con éxito.", mainW.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                test.Disconnect();
            }
            else
                MessageBox.Show("No se ha podido establecer la conexión con el servidor de diccionarios.", mainW.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void button7_Click(object sender, EventArgs e)
        {

            DirectoryInfo di = new DirectoryInfo(mW.autosaveDir);
            FileInfo[] fia = di.GetFiles("*.AUTOSAVE");

            foreach (FileInfo f in fia)
            {
                ZipSubtitleScript zipscript = new ZipSubtitleScript();
                zipscript.LoadFromFile(f.FullName);
                zipscript.SaveToZip(f.FullName);

                File.Delete(f.FullName);
            }

            MessageBox.Show(fia.Length + " archivos comprimidos.", "PerrySub", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }        

        private void button8_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            string time = DateTime.Now.Day+"."+DateTime.Now.Month+"."+DateTime.Now.Year;
            sfd.FileName = "Backup de Estilos [" + nickname.Text + "] (" + time + ").zip";
            sfd.InitialDirectory = textBackup.Text;
            sfd.Filter = "Archivo ZIP (*.zip)|*.zip";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                zw.CompressDirectory(mW.stylesDir, "*.Styles", sfd.FileName, false);
                MessageBox.Show("Backup de Estilos realizado.", "PerrySub", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            string time = DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year;
            sfd.FileName = "Backup de Notas de Traducción [" + nickname.Text + "] (" + time + ").zip";
            sfd.InitialDirectory = textBackup.Text;
            sfd.Filter = "Archivo ZIP (*.zip)|*.zip";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                zw.CompressDirectory(mW.notesDir, "*.Notes", sfd.FileName, false);
                MessageBox.Show("Backup de Notas de Traducción realizado.", "PerrySub", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            string time = DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year;
            sfd.FileName = "Backup de Efectos de Karaokes v1 [" + nickname.Text + "] (" + time + ").zip";
            sfd.InitialDirectory = textBackup.Text;
            sfd.Filter = "Archivo ZIP (*.zip)|*.zip";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                zw.CompressDirectory(mW.v1Dir, "*.v1", sfd.FileName, false);
                MessageBox.Show("Backup de Efectos de Karaokes v1 realizado.", "PerrySub", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            string time = DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year;
            sfd.FileName = "Backup de Templates [" + nickname.Text + "] (" + time + ").zip";
            sfd.InitialDirectory = textBackup.Text;
            sfd.Filter = "Archivo ZIP (*.zip)|*.zip";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                zw.CompressDirectory(mW.templateDir, "*.template", sfd.FileName, false);
                MessageBox.Show("Backup de Templates realizado.", "PerrySub", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            string time = DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year;
            sfd.FileName = "Backup General PerrySub [" + nickname.Text + "] (" + time + ").zip";
            sfd.InitialDirectory = textBackup.Text;
            sfd.Filter = "Archivo ZIP (*.zip)|*.zip";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                zw.CompressDirectory(Application.StartupPath, "*.template;*.Styles;*.v1;*.Notes", sfd.FileName, true);
                mW.updateReplaceConfigFile("Backup_General", DateTime.Now.Ticks.ToString());
                MessageBox.Show("Backup de Estilos, Notas de Traducción, Efectos de Karaokes v1 y Templates realizado.", "PerrySub", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        ActionProgressW TodosProgreso;
        ZipWrapper zw2;

        private void button13_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = textDir.Text;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                string time = DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year;
                sfd.FileName = "Backup Scripts [" + nickname.Text + "] (" + time + ").zip";
                sfd.InitialDirectory = textBackup.Text;
                sfd.Filter = "Archivo ZIP (*.zip)|*.zip";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    // este es el que tardará una animalada = threaded
                    zw2 = new ZipWrapper();

                    TodosProgreso = new ActionProgressW(mW, "Obteniendo la lista de archivos a comprimir...");
                    TodosProgreso.CancelButton.Enabled = false;

                    TodosProgreso.Show();

                    zw2.ArchivoComprimido += new ZipWrapperArchivoComprimido(zw2_ArchivoComprimido);
                    zw2.CompresionFinalizada += new ZipWrapperCompresionFinalizada(zw2_CompresionFinalizada);

                    zw2.Mask = "*.ass;*.ssa;*.txt";
                    zw2.WorkDir = fbd.SelectedPath;
                    zw2.ZipFile = sfd.FileName;

                    Thread t = new Thread(new ThreadStart(ComprimirTodosLosScripts));
                    t.Start();
                }

            }
        }

        void zw2_CompresionFinalizada(object sender, EventArgs e)
        {
            TodosProgreso.GoHide();
        }

        void zw2_ArchivoComprimido(object sender, ArchivoComprimidoEventArgs e)
        {
            TodosProgreso.UpdateText("Comprimiendo archivo " + e.Indice + " de " + e.Total + "...");
            TodosProgreso.UpdatePerc((int)(Math.Ceiling(((double)e.Indice / (double)e.Total)*100)));
        }

        void ComprimirTodosLosScripts()
        {
            int c = zw2.CompressDirectory();
            mW.updateReplaceConfigFile("Backup_Scripts", DateTime.Now.Ticks.ToString());
            MessageBox.Show("Backup de " + c + " scripts realizado.", "PerrySub", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void mainW_AutoSaveTimer_ValueChanged(object sender, EventArgs e)
        {
            int real_value = (int)mainW_AutoSaveTimer.Value * 1000;
            mW.updateReplaceConfigFile("mainW_AutoSaveInterval", real_value.ToString());
            if (!IsLoading) RequireRestart = true;
        }

        private void translateW_AutoSaveTimer_ValueChanged(object sender, EventArgs e)
        {
            int real_value = (int)translateW_AutoSaveTimer.Value * 1000;
            mW.updateReplaceConfigFile("translateW_AutoSaveInterval", real_value.ToString());
            if (!IsLoading) RequireRestart = true;
        }

        private void prefWizardTranslateW_CheckedChanged(object sender, EventArgs e)
        {
            mW.updateReplaceConfigFile("translateW_ShowPopup", prefWizardTranslateW.Checked.ToString());
        }

        private void prefVideoSceneDetector_CheckedChanged(object sender, EventArgs e)
        {
            mW.updateReplaceConfigFile("Video_SceneDetector", prefVideoSceneDetector.Checked.ToString());
        }

        private void prefProMode_CheckedChanged(object sender, EventArgs e)
        {
            mW.updateReplaceConfigFile("mainW_ProMode", prefProMode.Checked.ToString());

            // Guardamos de paso todos los valores
            int real_value = (int)prefLineChars.Value;
            mW.updateReplaceConfigFile("mainW_LineChars", real_value.ToString());
            real_value = (int)prefSubChars.Value;
            mW.updateReplaceConfigFile("mainW_SubChars", real_value.ToString());
            real_value = (int)prefSecChars.Value;
            mW.updateReplaceConfigFile("mainW_SecChars", real_value.ToString());
        }

        private void prefLineChars_ValueChanged(object sender, EventArgs e)
        {
            int real_value = (int)prefLineChars.Value;
            mW.updateReplaceConfigFile("mainW_LineChars", real_value.ToString());
        }

        private void prefSubChars_ValueChanged(object sender, EventArgs e)
        {
            int real_value = (int)prefSubChars.Value;
            mW.updateReplaceConfigFile("mainW_SubChars", real_value.ToString());
        }

        private void prefSecChars_ValueChanged(object sender, EventArgs e)
        {
            int real_value = (int)prefSecChars.Value;
            mW.updateReplaceConfigFile("mainW_SecChars", real_value.ToString());
        }

        private void button14_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = true;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                textBackup.Text = fbd.SelectedPath;
            }
        }

        private void prefSpellCheck_CheckedChanged(object sender, EventArgs e)
        {
            mW.updateReplaceConfigFile("ASSBox_SpellCheckEnabled", prefSpellCheck.Checked.ToString());
            if (!IsLoading) RequireRestart = true;
        }

        private void RefreshDictionaries()
        {
            DictCultureNames = new SortedList<string, string>();
            DirectoryInfo info = new DirectoryInfo(mW.dictDir);
            FileInfo[] dics = info.GetFiles("*.dic");

            listaDiccionarios.Items.Clear();
            diccionarioActivo.Items.Clear();

            foreach (FileInfo fi in dics)
            {
                string nom = Path.GetFileNameWithoutExtension(fi.FullName);
                string nomext =  CultureWrapper.GetLanguageName(nom);
                DictCultureNames.Add(nomext,nom); // deberia ser al reves pero weno
                listaDiccionarios.Items.Add(nomext);
                diccionarioActivo.Items.Add(nomext);
            }
            

        }

        private void diccionarioActivo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string lang = DictCultureNames[diccionarioActivo.Text];
            mW.updateReplaceConfigFile("SpellLanguage", lang);
            mW.ActiveDict = lang;
            //if (!IsLoading) RequireRestart = true;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://wiki.services.openoffice.org/wiki/Dictionaries");
        }



        private void listaDiccionarios_SelectedIndexChanged(object sender, EventArgs e)
        {
            string langsel = listaDiccionarios.SelectedItem.ToString();
            label9.Text = DictCultureNames[langsel];
        }


        private void RefreshAviSynthPriorities()
        {
            listAudioPriority.Items.Clear();
            listVideoPriority.Items.Clear();

            foreach (string s in VideoFilterPriority)
                listVideoPriority.Items.Add(s);

            foreach (string s in AudioFilterPriority)
                listAudioPriority.Items.Add(s);
        }

        private void ReloadDefaultsVideo()
        {
            VideoFilterPriority.Clear();
            foreach (string sfilter in Enum.GetNames(typeof(VideoSourceType)))
            {
                VideoFilterPriority.Add(sfilter);
            }

            RefreshAviSynthPriorities();
        }

        private void ReloadDefaultsAudio()
        {
            AudioFilterPriority.Clear();
            foreach (string sfilter in Enum.GetNames(typeof(AudioSourceType)))
            {
                AudioFilterPriority.Add(sfilter);
            }
            RefreshAviSynthPriorities();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ReloadDefaultsVideo();
            if (!IsLoading) RequireRestart = true;
        }

        private void button19_Click(object sender, EventArgs e)
        {
            ReloadDefaultsAudio();
            if (!IsLoading) RequireRestart = true;
        }

        private void button16_Click(object sender, EventArgs e)
        {
            // subir prioridad (video)
            if (listVideoPriority.SelectedIndex == -1) return;
            if (listVideoPriority.SelectedIndex == 0) return; // no podemos subir el primero

            int idx = listVideoPriority.SelectedIndex;

            string dastring = VideoFilterPriority[idx].ToString();

            VideoFilterPriority.RemoveAt(idx);
            VideoFilterPriority.Insert(idx - 1, dastring);

            RefreshAviSynthPriorities();

            listVideoPriority.SelectedIndex = idx - 1;

            mW.updateReplaceConfigFileA("VideoFilterPriority", new ArrayList(VideoFilterPriority));
            if (!IsLoading) RequireRestart = true;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            // bajar prioridad (video)
            if (listVideoPriority.SelectedIndex == -1) return;
            if (listVideoPriority.SelectedIndex == listVideoPriority.Items.Count - 1) return; // no podemos bajar el último

            int idx = listVideoPriority.SelectedIndex;

            string dastring = VideoFilterPriority[idx].ToString();

            VideoFilterPriority.RemoveAt(idx);
            VideoFilterPriority.Insert(idx + 1, dastring);

            RefreshAviSynthPriorities();

            listVideoPriority.SelectedIndex = idx + 1;
            mW.updateReplaceConfigFileA("VideoFilterPriority", new ArrayList(VideoFilterPriority));
            if (!IsLoading) RequireRestart = true;

        }

        private void button17_Click(object sender, EventArgs e)
        {
            // subir prioridad (audio)
            if (listAudioPriority.SelectedIndex == -1) return;
            if (listAudioPriority.SelectedIndex == 0) return; // no podemos subir el primero

            int idx = listAudioPriority.SelectedIndex;

            string dastring = AudioFilterPriority[idx].ToString();

            AudioFilterPriority.RemoveAt(idx);
            AudioFilterPriority.Insert(idx - 1, dastring);

            RefreshAviSynthPriorities();

            listAudioPriority.SelectedIndex = idx - 1;
            mW.updateReplaceConfigFileA("AudioFilterPriority", new ArrayList(AudioFilterPriority));
            if (!IsLoading) RequireRestart = true;

        }

        private void button18_Click(object sender, EventArgs e)
        {
            // bajar prioridad (audio)
            if (listAudioPriority.SelectedIndex == -1) return;
            if (listAudioPriority.SelectedIndex == listAudioPriority.Items.Count - 1) return; // no podemos bajar el último

            int idx = listAudioPriority.SelectedIndex;

            string dastring = AudioFilterPriority[idx].ToString();

            AudioFilterPriority.RemoveAt(idx);
            AudioFilterPriority.Insert(idx + 1, dastring);

            RefreshAviSynthPriorities();

            listAudioPriority.SelectedIndex = idx + 1;
            mW.updateReplaceConfigFileA("AudioFilterPriority", new ArrayList(AudioFilterPriority));
            if (!IsLoading) RequireRestart = true;

        }

    }
}