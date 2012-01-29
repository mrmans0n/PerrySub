using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Threading;

namespace scriptASS // riau riau
{
    public partial class maxLinesW : Form
    {
        mainW mW;
        delegate void SetPercCallback(int sux0r);
        delegate void SetVoidCallback();

        Hashtable LineasTratables;
        Thread t;
        AnalizarLineasMax an;

        public maxLinesW(mainW mw)
        {
            InitializeComponent();
            mW = mw;
            this.Disposed += new EventHandler(maxLinesW_Disposed);
            this.FormClosing += new FormClosingEventHandler(maxLinesW_FormClosing);
            this.MaximumSize = this.Size;            
        }

        void maxLinesW_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (t != null)
                if (t.IsAlive)
                {
                    if (an != null)
                        an.StopCondition();
                    t.Join();
                }
        }

        void maxLinesW_Disposed(object sender, EventArgs e)
        {

            mW.updateGridWithArrayList(mW.al);
        }

        private void maxLinesW_Load(object sender, EventArgs e)
        {
            checkedListBox1.Items.Clear();
            foreach (estiloV4 v in mW.v4)
                checkedListBox1.Items.Add(v.Name, true);

            try
            {
                textBox1.Text = mW.getFromConfigFile("maxLinesW_lines");
                trackBar1.Value = int.Parse(textBox1.Text);

            }
            catch
            {
                textBox1.Text = "2";
                trackBar1.Value = 2;
            }

            try
            {
                textBox2.Text = mW.getFromConfigFile("maxLinesW_percent");
                trackBar2.Value = int.Parse(textBox2.Text);

            }
            catch
            {
                textBox2.Text = "80";
                trackBar2.Value = 80;
            }

            try
            {
                numericUpDown1.Value = int.Parse(mW.getFromConfigFile("maxLinesW_lowlimit"));                

            }
            catch
            {

                numericUpDown1.Value = 0;
            }

            textBox1.Text = trackBar1.Value.ToString();

            prev_X.Text = mW.VideoWidth.ToString();
            prev_Y.Text = mW.VideoHeight.ToString();

            //MessageBox.Show("Esta característica no es del todo exacta con según que scripts todavía.\nUsadlo bajo vuestra propia discreción.", "ADVERTENCIA", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            textBox1.Text = trackBar1.Value.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mW.updateReplaceConfigFile("maxLinesW_lines", textBox1.Text);
            mW.updateReplaceConfigFile("maxLinesW_percent", textBox2.Text);
            mW.updateReplaceConfigFile("maxLinesW_lowlimit", numericUpDown1.Value.ToString());

            int idx=0;

            ArrayList estilos = new ArrayList();
            for (int i = 0; i < checkedListBox1.CheckedIndices.Count; i++)
                estilos.Add(mW.v4[checkedListBox1.CheckedIndices[i]]);

            an = new AnalizarLineasMax(mW.script, estilos, int.Parse(textBox1.Text), int.Parse(prev_X.Text), int.Parse(prev_Y.Text),(int)numericUpDown1.Value);
            t = new Thread(new ThreadStart(an.ProcesarLineas));

            an.LineaProcesada += new AnalizarLineasMaxLineaProcesada(an_LineaProcesada);
            an.AnalisisCompletado += new AnalizarLineasMaxAnalisisCompletado(an_AnalisisCompletado);

            progressBar1.Maximum = mW.script.LineCount;
            toolStripStatusLabel1.Text = "Analizando líneas...";
            button1.Enabled = false;
            LineasTratables = new Hashtable();
            t.Start();

        }

        void an_AnalisisCompletado(object sender)
        {
            //MessageBox.Show("Análisis procesado con éxito. " + LineasTratables.Count + " líneas necesitan ser tratadas", mainW.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //button1.Enabled = true;
            toolStripStatusLabel1.Text = "Análisis procesado con éxito. " + LineasTratables.Count + " líneas necesitan ser tratadas";

            if (LineasTratables.Count > 0)
            {
                UpdatePerc(0);
                UpdateMax(LineasTratables.Count);
                // toolStripStatusLabel1.Text="Realizando Modificaciones...";
                an = new AnalizarLineasMax(mW.script, null, int.Parse(textBox1.Text), int.Parse(prev_X.Text), int.Parse(prev_Y.Text), (int)numericUpDown1.Value);
                an.CargarModificaciones(LineasTratables, int.Parse(textBox2.Text));
                an.LineaModificada += new AnalizarLineasMaxLineaModificada(an_LineaModificada);
                an.ModificacionesCompletadas += new AnalizarLineasMaxModificacionesCompletadas(an_ModificacionesCompletadas);

                t = new Thread(new ThreadStart(an.RealizarModificaciones));
                t.Start();
            }
            else
                EnableButton();
        }

        void an_LineaModificada(object sender, AnalizarLineasMaxLineaModificadaEventArgs e)
        {
            UpdatePerc(e.Linea);
        }

        void an_ModificacionesCompletadas(object sender)
        {
            //MessageBox.Show("Modificaciones realizadas. Ya puedes cerrar y volver al programa principal.", mainW.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
            toolStripStatusLabel1.Text = "Modificaciones realizadas.";
            EnableButton();
        }

        void an_LineaProcesada(object sender, AnalizarLineasMaxLineaProcesadaEventArgs e)
        {
            UpdatePerc(e.Contador);
            //progressBar1.Value = n;
            if (e.Cumple)
                LineasTratables.Add(e.Contador, e.Lineas);
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            textBox2.Text = trackBar2.Value.ToString();
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

        public void UpdateMax(int s)
        {

            if (this.progressBar1.InvokeRequired)
            {
                SetPercCallback d = new SetPercCallback(UpdateMax);
                this.Invoke(d, new object[] { s });
            }
            else
            {
                this.progressBar1.Maximum = s;
            }
        }

        public void EnableButton()
        {

            if (this.progressBar1.InvokeRequired)
            {
                SetVoidCallback d = new SetVoidCallback(EnableButton);
                this.Invoke(d, new object[] { });
            }
            else
            {
                this.button1.Enabled = true;
                mW.updateGridWithArrayList(mW.al);
            }
        }
    }
}