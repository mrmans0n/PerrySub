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
    public partial class animationW : Form
    {
        Queue animaciones = new Queue();
        ArrayList listam = new ArrayList();
        v2W v2;
        public animationW(v2W v2)
        {
            this.v2 = v2;
            InitializeComponent();
        }

        private void animationW_Load(object sender, EventArgs e)
        {
            label1.Hide();
            label2.Hide();
            numericTextBox1.Hide();
            numericTextBox2.Hide();
            numericTextBox1.AllowNegative = true;
            numericTextBox2.AllowNegative = true;
            ArrayList estilosDeAnimacion = new ArrayList();
            int i = 0;
            while (i < 2)
            {
                     estilosDeAnimacion.Add(ParticleAnimacion.NombreTipo[i]);
                i++;
            }
            comboBox1.DataSource = estilosDeAnimacion;
            listBox1.Text = "";


        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
          
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = comboBox1.SelectedIndex;
            int numEntradas = ParticleAnimacion.ValTipo[i];

            switch (numEntradas)
            {
                case 2:
                    label1.Show();
                    label2.Show();
                    label1.Text = ParticleAnimacion.LayersTipo[i][0];
                    label2.Text = ParticleAnimacion.LayersTipo[i][1];
                    numericTextBox1.Show();
                    numericTextBox2.Show();
                    break;
                case 1:
                    label1.Show();
                    label1.Text = ParticleAnimacion.LayersTipo[i][0];
                    numericTextBox1.Point = true;
                    numericTextBox1.Show();
                    label2.Hide();
                    numericTextBox2.Hide();
                    break;
                default:
                    label1.Hide();
                    label2.Hide();
                    numericTextBox1.Hide();
                    numericTextBox2.Hide();
                    break;
            }

            
        }

        private void add_Click(object sender, EventArgs e)
        {
            int i = comboBox1.SelectedIndex;
            int numEntradas = ParticleAnimacion.ValTipo[i];
            if ((numEntradas == 2 && label1.Text != "" && label2.Text != "") || (numEntradas == 1 && label1.Text != ""))
            {
                object[] parametros = new object[numEntradas];
                parametros[0]=Convert.ToInt32(numericTextBox1.Text);
                parametros[1] = Convert.ToInt32(numericTextBox2.Text);
                ParticleAnimacion pa =  new ParticleAnimacion((ParticleAnimacion.Tipo)comboBox1.SelectedIndex,parametros);
                animaciones.Enqueue(pa);
                listam.Add(ParticleAnimacion.NombreTipo[i]);
            }
            listBox1.DataSource = listam;
        }

        private void animationW_FormClosing(object sender, FormClosingEventArgs e)
        {
            v2.Enabled = true;
            v2.SetAnimaciones(animaciones);
            if (animaciones.Count > 0)
                v2.actGen();
        }

    }
}