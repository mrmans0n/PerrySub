using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace scriptASS
{
    public partial class progress2W : Form
    {
        mainW mw;

        public progress2W(mainW m)
        {
            InitializeComponent();
            mw = m;
            //this.Location = mw.Location;
            this.Load += new EventHandler(progressW_Load);
            this.Disposed += new EventHandler(progressW_Disposed);
        }

        void progressW_Disposed(object sender, EventArgs e)
        {
            mw.Enabled = true;
            mw.Focus();
            mw.updateAudioWave();
            mw.updateAudioGrid();
        }

        void progressW_Load(object sender, EventArgs e)
        {
            mw.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Esto cancelará la copia del audio en memoria, ¿estás seguro de que deseas continuar?", "PerrySub", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                mainW.AudioLoad_isCancelled = true;                
            
        }

    }
}