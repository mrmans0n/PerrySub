using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using scriptASS.Clases;

namespace scriptASS.Edit
{
    public partial class v2W : Form
    {
        bool creadaAlgunaAnimacion = false;
        Queue animaciones;
        animationW aw;
        ParticleEngine pe;
        mainW mw;
        
        public v2W(mainW mw1)
        {
            mw = mw1;
            InitializeComponent();
            numericTextBox1.Point = true;
        }

        
        private void v2W_Load(object sender, EventArgs e)
        {
            if (!creadaAlgunaAnimacion)
            {
                generar.Enabled = false;
            }
            aw=new animationW(this);
        }

        private void animation_Click(object sender, EventArgs e)
        {
            aw.Show();
            this.Enabled = false;
        }

        public void SetAnimaciones(Queue a)
        {
            animaciones = a;
        }

        private void generar_Click(object sender, EventArgs e)
        {
            if (particulaText.Text != "")
            {
                string [] formas = particulaText.Text.Split('\n');
                pe = new ParticleEngine(((double)mw.FrameIndex/100) * mw.videoInfo.FrameRate, mw.videoInfo.FrameRate/100, Convert.ToDouble(numericTextBox1.Text),formas);
            }
            else {
                pe = new ParticleEngine(((double)mw.FrameIndex/100)*mw.videoInfo.FrameRate, mw.videoInfo.FrameRate/100, Convert.ToDouble(numericTextBox1.Text));
            }

            int inicio = Convert.ToInt32(fri.Text);
            int fin = Convert.ToInt32(frf.Text);
            while (animaciones.Count>0)
            {
                pe.AddAnimacion((ParticleAnimacion)animaciones.Dequeue());
            }

            for (int i = inicio; i < fin; i++)
            {
                pe.Itera();
            }

            int ind=0;
            while (pe.lineasASS.Count > 0)
            {
                mw.al.Add((lineaASS)pe.lineasASS.Dequeue());
                ind++;
            }
        }

        private void v2W_FormClosing(object sender, FormClosingEventArgs e)
        {
            //mw.updateGridWithArrayList(mw.al);
            mw.Enabled = true;
                        
        }

        public void actGen()
        {
            generar.Enabled = true;
        }

        private void particulaText_TextChanged(object sender, EventArgs e)
        {

        }
    }
}