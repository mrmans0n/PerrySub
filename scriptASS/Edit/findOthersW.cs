using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading;

namespace scriptASS
{
    public partial class findOthersW : Form
    {
        mainW perrymain;
        Hashtable found;
        Size original;
        string busqueda;
        SubtitleScript current;

        delegate void SetPercCallback(int newperc);
        delegate void AddMatchCallback(string name);

        public findOthersW(mainW mec)
        {
            InitializeComponent();
            perrymain = mec;

            this.Width = 560; // hardcoded :S
            original = this.Size;

            archivosView.KeyDown += new KeyEventHandler(archivosView_KeyDown);
            coincidenciasView.SelectedIndexChanged += new EventHandler(coincidenciasView_SelectedIndexChanged);
            lineasCoincidencias.MouseClick += new MouseEventHandler(lineasCoincidencias_MouseClick);
            lineasCoincidencias.SelectedIndexChanged += new EventHandler(lineasCoincidencias_SelectedIndexChanged);
        }


        void lineasCoincidencias_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (coincidenciasView.SelectedIndices.Count == 1)
                {
                    this.Width = 1024;
                }
            }
        }

        void lineasCoincidencias_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lineasCoincidencias.SelectedIndices.Count != 1)
            {
                this.Width = original.Width;
                return;
            }

            int i = int.Parse(lineasCoincidencias.SelectedItems[0].Text);

            // rellenar el richtextbox ( bold a la linea en si, rojo a las ocurrencias en todas ellas )
            lineaASS actual = (lineaASS)current.GetLineArrayList().GetFullArrayList()[i];

            label4.Text = "Contexto que rodea a la línea " + i + " (" + actual.t_inicial.ToString() + " -> " + actual.t_final.ToString() + ")";
            string rtbString = ""; 
            if (i - 2 >= 0) rtbString += ((lineaASS)current.GetLineArrayList().GetFullArrayList()[i - 2]).texto + "\n";
            if (i - 1 >= 0) rtbString += ((lineaASS)current.GetLineArrayList().GetFullArrayList()[i - 1]).texto + "\n";

            int inL = rtbString.Length;
            rtbString += actual.texto + "\n";
            int boldL = rtbString.Length - inL;

            if (i + 1 < current.LineCount) rtbString += ((lineaASS)current.GetLineArrayList().GetFullArrayList()[i + 1]).texto + "\n";
            if (i + 2 < current.LineCount) rtbString += ((lineaASS)current.GetLineArrayList().GetFullArrayList()[i + 2]).texto + "\n";

            richTextBox1.Text = rtbString;
            richTextBox1.SelectAll();
            richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Regular); // reseteo
            richTextBox1.Select(inL, boldL); // bold línea ocurrencia
            richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Bold);

            Regex r;
            MatchCollection mc;

            r = new Regex(@"\b" + busqueda.ToLower() + @"\b");
            mc = r.Matches(rtbString.ToLower());

            foreach (Match m in mc)
            {
                richTextBox1.Select(m.Index, m.Length);
                richTextBox1.SelectionColor = Color.Crimson;
                richTextBox1.SelectionFont = new Font(Font, FontStyle.Bold);
            }

            richTextBox1.Select(0, 0);
        }

        void coincidenciasView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (coincidenciasView.SelectedIndices.Count != 1)
                return;

            lineasCoincidencias.Enabled = true;
            lineasCoincidencias.Items.Clear();            

            foreach (SubtitleScript script in found.Keys)
            {
                if (script.FileName.Equals(coincidenciasView.SelectedItems[0].Text))
                {
                    foreach (int i in (ArrayList)found[script])
                    {
                        string[] tmp = new string[2];
                        tmp[0] = i.ToString();
                        lineaASS actual = (lineaASS)script.GetLineArrayList().GetFullArrayList()[i];
                        tmp[1] = lineaASS.cleanText(actual.texto);
                        lineasCoincidencias.Items.Add(new ListViewItem(tmp,2));
                    }
                    current = script;
                    lineasCoincidencias.Columns[1].Width = -2; // autosize
                }
            }


        }

        void archivosView_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    for (int i = archivosView.SelectedItems.Count-1; i>-1; i--)
                    {
                        archivosView.Items.RemoveAt(archivosView.SelectedIndices[i]);
                    }
                    break;
            }
        }


        private void findOthersW_Load(object sender, EventArgs e)
        {
            try
            {
                string[] historial = perrymain.getFromConfigFileA("findW_FindHistorial");
                for (int i = 0; i < historial.Length; i++)
                {
                    comboFind.Items.Add(historial[i].Replace('※', ',').Replace('卍', '"'));
                }
            }
            catch { }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            try
            {
                openFileDialog1.InitialDirectory = perrymain.getFromConfigFile("mainW_WorkDirectory");
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

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            try
            {
                fbd.SelectedPath = perrymain.getFromConfigFile("mainW_WorkDirectory");
            }
            catch
            {
                fbd.SelectedPath = System.Environment.SpecialFolder.MyDocuments.ToString();
            }

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                if (subdir.Checked)
                {
                    BusquedaNormal(fbd.SelectedPath); // mirar tb raiz
                    BusquedaRecursiva(fbd.SelectedPath);
                }
                else
                    BusquedaNormal(fbd.SelectedPath);

            }
        }

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
                archivosView.Items.Add(f, 0);
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

        public void UpdatePerc(int s)
        {

            if (this.progressBar1.InvokeRequired)
            {
                SetPercCallback d = new SetPercCallback(UpdatePerc);
                this.Invoke(d, new object[] { s });
            }
            else
            {
                this.progressBar1.Value = s;
            }
        }

        public void AddFileMatch(string s)
        {

            if (this.coincidenciasView.InvokeRequired)
            {
                AddMatchCallback d = new AddMatchCallback(AddFileMatch);
                this.Invoke(d, new object[] { s });
            }
            else
            {
                int estilo = 1; // default hoja roja (ssa/ass)
                if (s.EndsWith(".txt", StringComparison.InvariantCultureIgnoreCase))
                    estilo = 3;
                else if (s.EndsWith(".srt", StringComparison.InvariantCultureIgnoreCase))
                    estilo = 4;
                this.coincidenciasView.Items.Add(s, estilo);
            }
        }

        private void PokePoints()
        {
            if (toolStripStatusLabel2.Text == "")
                toolStripStatusLabel2.Text = ".";
            else if (toolStripStatusLabel2.Text == ".")
                toolStripStatusLabel2.Text = "..";
            else if (toolStripStatusLabel2.Text == "..")
                toolStripStatusLabel2.Text = "...";
            else if (toolStripStatusLabel2.Text == "...")
                toolStripStatusLabel2.Text = ".";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboFind.Text == "") return;
            if (comboFind.Text.Length < 3)
            {
                perrymain.errorMsg("No se pueden buscar palabras de menos de tres carácteres");
                return;
            }

            progressBar1.Visible = true;
            progressBar1.Value = 0;
            coincidenciasView.Enabled = true;
            coincidenciasView.Items.Clear();
            lineasCoincidencias.Enabled = false;
            lineasCoincidencias.Items.Clear();
            found = new Hashtable();
            busqueda = comboFind.Text;

            ArrayList listaArchivos = new ArrayList();
            foreach (ListViewItem li in archivosView.Items)
                listaArchivos.Add(li.Text);

            // threaded version - llamar al hilo
            AnalizarBusquedaArchivos analisis = new AnalizarBusquedaArchivos(comboFind.Text, caseInSensitive.Checked, regExp.Checked, listaArchivos);
            analisis.ArchivoProcesado += new AnalizarBusquedaArchivosProcesado(analisis_ArchivoProcesado);
            analisis.AnalisisFinalizado += new AnalizarBusquedaArchivosFinalizada(analisis_AnalisisFinalizado);
            progressBar1.Maximum = listaArchivos.Count;
            toolStripStatusLabel1.Text = "Iniciando la búsqueda en " + listaArchivos.Count + " archivo(s)";

            string parsed_text = comboFind.Text.Replace(',', '※').Replace('"', '卍');
            perrymain.updateConcatenateConfigFile("findW_FindHistorial", parsed_text);

            Thread t = new Thread(new ThreadStart(analisis.RealizarAnalisis));
            t.Start();
        }

        void analisis_AnalisisFinalizado(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Análisis finalizado. Encontrados " + found.Count + " archivo(s) con coincidencias.";
            toolStripStatusLabel2.Text = "";
        }

        void analisis_ArchivoProcesado(object sender, ArchivoProcesadoEventArgs e)
        {
            if ((e.LineasCoincidencias != null) && (e.ScriptTratado!=null))
            {
                found.Add(e.ScriptTratado, e.LineasCoincidencias);
                AddFileMatch(e.ScriptTratado.FileName);                
            }
            UpdatePerc(progressBar1.Value + 1);
            PokePoints();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Width = original.Width;
        }
    }
}
