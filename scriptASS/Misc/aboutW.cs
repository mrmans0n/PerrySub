using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace scriptASS
{
    public partial class aboutW : Form
    {
        mainW mW;
        public aboutW(mainW mW)
        {
            InitializeComponent();
            this.mW = mW;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void aboutW_Load(object sender, EventArgs e)
        {
            label2.Text = mainW.appTitle;
        }

    }
}