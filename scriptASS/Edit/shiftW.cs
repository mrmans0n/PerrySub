using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace scriptASS
{
    public partial class shiftW : Form
    {
        private string tOps="+";
        mainW mW = null; 

        public shiftW(mainW mW)
        {
            InitializeComponent();
            this.mW = mW;
            if (mW.videoInfo == null)
            {
                radioFrames.Enabled = numericFrames.Enabled = label2.Enabled = false;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (listBox1.SelectedIndex == 0)
                {
                    timeText.Text = "00:00:00.00";
                }
                else
                {
                    string op = listBox1.Text.Substring(0, 1);
                    radioSuma.Checked = op.Equals("+");
                    radioResta.Checked = op.Equals("-");
                    timeText.Text = listBox1.Text.Substring(1);
                }
            }
            catch { }
        }

        private void shiftW_Load(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox1.Items.Add("(nuevo)");

            try
            {
                string[] s = mW.getFromConfigFileA("shiftW_old");
                for (int i = 0; i < s.Length; i++)
                    listBox1.Items.Add(s[i]);
            }
            catch { }

            try
            {
                string s = mW.getFromConfigFile("shiftW_style");
                switch (s)
                {
                    case "sel":
                        radioSeleccionadas.Checked = true;
                        break;
                    case "all":
                        radioTodas.Checked = true;
                        break;
                    case "aft":
                        radioPosteriores.Checked = true;
                        break;
                    case "bef":
                        radioAnteriores.Checked = true;
                        break;
                }
            }
            catch
            {
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (radioSuma.Checked) tOps = "+";
            else tOps = "-";

            string s = timeText.Text;

            if (radioFrames.Checked)
            {
                int f;
                try
                {
                    f = int.Parse(numericFrames.Text);
                    if (mW.videoInfo.FrameRate == 0) throw new Exception();
                }
                catch
                {
                    mW.errorMsg("Introduce un valor de frames válido y vuélvelo a intentar.");
                    return;
                }
                double sd = (double)Math.Round(f * ((double)1/(double)mW.videoInfo.FrameRate),2);
                Tiempo t = new Tiempo(sd);
                s = t.ToString();
            }

            if (radioSeleccionadas.Checked)
            {
                mW.doShift(tOps + s, ShiftStyle.Selected);
                mW.updateReplaceConfigFile("shiftW_style", "sel");
            }
            else if (radioTodas.Checked)
            {
                mW.doShift(tOps + s, ShiftStyle.Normal);
                mW.updateReplaceConfigFile("shiftW_style", "all");
            }
            else if (radioAnteriores.Checked)
            {
                mW.doShift(tOps + s, ShiftStyle.BeforeSelected);
                mW.updateReplaceConfigFile("shiftW_style", "bef");
            }
            else if (radioPosteriores.Checked)
            {
                mW.doShift(tOps + s, ShiftStyle.AfterSelected);
                mW.updateReplaceConfigFile("shiftW_style", "aft");
            }

            mW.updateConcatenateConfigFile("shiftW_old", tOps + s);

            this.Dispose();
        }

        private void desplazamiento_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void numericFrames_TextChanged(object sender, EventArgs e)
        {
            radioFrames.Checked = true;
        }

    }
}