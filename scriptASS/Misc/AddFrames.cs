using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace scriptASS.Misc
{
    public partial class AddFrames : Form
    {
        private int result;

        public int Result
        {
            get { return result; }
            set { result = value; }
        }

        public AddFrames()
        {
            InitializeComponent();
            this.DialogResult = DialogResult.Cancel;
            result = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            result = (int)numericUpDown1.Value;
        }
    }
}
