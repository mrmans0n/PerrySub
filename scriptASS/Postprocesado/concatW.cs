using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace scriptASS
{
    public partial class concatW : Form
    {
        private mainW mW;
        public concatW(mainW mW)
        {
            InitializeComponent();
            this.mW = mW;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ArrayList estilos = new ArrayList();
            for (int i = 0; i < checkedListBox1.CheckedIndices.Count; i++)
                estilos.Add(mW.v4[checkedListBox1.CheckedIndices[i]]);
            
            mW.Enabled = true;

            if (checkBox1.Checked)
            {
                mW.doConcat(Int32.Parse(textBox1.Text),estilos);
                mW.updateReplaceConfigFile("concatW_c", textBox1.Text);
            }
            if (checkBox2.Checked)
            {
                mW.doOverlap(Int32.Parse(textBox2.Text), estilos);
                mW.updateReplaceConfigFile("concatW_o", textBox2.Text);
            }
            this.Dispose();
            
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Enabled = checkBox1.Checked;

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.Enabled = checkBox2.Checked;
        }

        private void btn3_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void concatW_Load(object sender, EventArgs e)
        {

            foreach (estiloV4 v in mW.v4)
                checkedListBox1.Items.Add(v.Name, true);
                        
            try
            {
                textBox1.Text = mW.getFromConfigFile("concatW_c");
                textBox2.Text = mW.getFromConfigFile("concatW_o");
            }
            catch
            {
                textBox1.Text = "200";
                textBox2.Text = "200";
            }

        }

    }
}