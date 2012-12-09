using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace scriptASS
{
    public partial class ActionProgressW : Form
    {
        mainW mw;
        delegate void SetPercCallback(int sux0r);
        delegate void SetTextCallback(string m3c);
        delegate void SetHide();
        private bool abort;

        public bool Abort
        {
            get { return abort; }
            set { abort = value; }
        }

        public ActionProgressW(mainW m, string operacion)
        {
            InitializeComponent();
            mw = m;
            this.Load += new EventHandler(progressW_Load);
            this.Disposed += new EventHandler(progressW_Disposed);
            abort = false;
            accionProgreso.Text = operacion;
        }

        public void GoHide()
        {
            if (this.InvokeRequired)
            {
                SetHide d = new SetHide(GoHide);
                this.Invoke(d, new object[] { });
            }
            else
            {
                this.Dispose();
            }
        }

        public void UpdatePerc(int s)
        {

            if (this.progressBar.InvokeRequired)
            {
                SetPercCallback d = new SetPercCallback(UpdatePerc);
                this.Invoke(d, new object[] { s });
            }
            else
            {
                try
                {
                    this.progressBar.Maximum = 100;
                    this.progressBar.Value = s;
                }
                catch { }
                progressBar.Refresh();
            }

        }


        public void UpdateText(string s)
        {

            if (this.accionProgreso.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(UpdateText);
                this.Invoke(d, new object[] { s });
            }
            else
            {
                this.accionProgreso.Text = s;
            }

        }

        void progressW_Disposed(object sender, EventArgs e)
        {
            mw.Enabled = true;
            mw.Focus();
        }

        void progressW_Load(object sender, EventArgs e)
        {
            mw.Enabled = false;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (MessageBox.Show("Esto cancelará la operación, ¿estás seguro de que deseas continuar?", "PerrySub", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                abort = true;                
 
        }

    }
}