using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Threading;

namespace scriptASS
{
    public partial class adjKeyW : Form
    {
        mainW mW;
        Thread t;
        AdjustToKeyframe atkf;
        delegate void SetPercCallback(int sux0r);
        delegate void FusilarVentana(bool kk);
        
        public adjKeyW(mainW mW)
        {
            InitializeComponent();
            this.mW = mW;
        }

        private void button1_Click(object sender, EventArgs e)
        {
                                 
             

            //t.Join();
            //this.Dispose();

        }

        void atkf_ProcesoFinalizado(object sender)
        {
            UpdatePerc(progressBar1.Maximum);

            foreach (int key in atkf.Resultado.Keys)
            {
                lineaASS lass = (lineaASS)atkf.Resultado[key];
                mW.al[key] = lass;
            }
            mW.updateGridWithArrayList(mW.al);
            MessageBox.Show("Ajuste finalizado. " + atkf.Resultado.Count + " cambios realizados.", mainW.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
            Fusila(true);
        }

        void atkf_LineaProcesada(object sender, AjustarKeyframeLineaProcesadaEventArgs lineaproc)
        {
            UpdatePerc(lineaproc.NumeroLinea);
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
        
        public void Fusila(bool kk)
        {
            if (this.InvokeRequired)
            {
                FusilarVentana d = new FusilarVentana(Fusila); // y esto no va
                this.Invoke(d, new object[] { kk });
            }
            else
            {
                this.Dispose();
            }
        }

        private void btn3_Click(object sender, EventArgs e)
        {
           
        }

        private void adjKeyW_Load(object sender, EventArgs e)
        {
            this.MaximumSize = this.Size;
            
            try
            {
                textBox1.Text = mW.getFromConfigFile("adjKeyW_iPre");
                textBox2.Text = mW.getFromConfigFile("adjKeyW_iPost");
                textBox3.Text = mW.getFromConfigFile("adjKeyW_fPre");
                textBox4.Text = mW.getFromConfigFile("adjKeyW_fPost");
            }
            catch
            {
                textBox1.Text = "0"; textBox2.Text = "0"; textBox3.Text = "0"; textBox4.Text = "0";
            }

            foreach (estiloV4 v in mW.v4)
                checkedListBox1.Items.Add(v.Name,true);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = button3.Enabled = false;

            if (textBox1.Text == "") textBox1.Text = "0";
            if (textBox2.Text == "") textBox2.Text = "0";
            if (textBox3.Text == "") textBox3.Text = "0";
            if (textBox4.Text == "") textBox4.Text = "0";
            int iPre = Int32.Parse(textBox1.Text);
            int iPost = Int32.Parse(textBox2.Text);
            int fPre = Int32.Parse(textBox3.Text);
            int fPost = Int32.Parse(textBox4.Text);

            mW.updateReplaceConfigFile("adjKeyW_iPre", iPre.ToString());
            mW.updateReplaceConfigFile("adjKeyW_iPost", iPost.ToString());
            mW.updateReplaceConfigFile("adjKeyW_fPre", fPre.ToString());
            mW.updateReplaceConfigFile("adjKeyW_fPost", fPost.ToString());

            ArrayList estilos = new ArrayList();
            for (int i = 0; i < checkedListBox1.CheckedIndices.Count; i++)
                estilos.Add(mW.v4[checkedListBox1.CheckedIndices[i]]);

            mW.Enabled = true;

            //mW.adjToKeyF(iPre, iPost, fPre, fPost, estilos);
            //this.Dispose();

            if (radioAuto.Checked)
            {
                iPre = iPost = fPre = fPost = trackFrames.Value;
            }

            atkf = new AdjustToKeyframe(mW.script, iPre, iPost, fPre, fPost, mW.videoInfo.FrameRate, estilos, mW.videoInfo.KeyFrames);

            t = new Thread(new ThreadStart(atkf.ProcesarAjuste));
            atkf.LineaProcesada += new AdjustToKeyframe.AjustarKeyframeLineaProcesada(atkf_LineaProcesada);
            atkf.ProcesoFinalizado += new AdjustToKeyframe.AjustarKeyframeProcesamientoFinalizado(atkf_ProcesoFinalizado);
            progressBar1.Maximum = atkf.ProcesarLineas;

            t.Start();    
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            groupAuto.Enabled = radioAuto.Checked;
            groupManual.Enabled = radioManual.Checked;
        }

        private void trackFrames_Scroll(object sender, EventArgs e)
        {
            labelFrames.Text = trackFrames.Value.ToString();
        }
    }
}