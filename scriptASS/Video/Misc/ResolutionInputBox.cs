using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace scriptASS
{
    public partial class ResolutionInputBox : Form
    {
        avsW padre;
        string resizer;
        
        double widescreen = 16.0 / 9.0;
        double normal = 4.0 / 3.0;

        public ResolutionInputBox(avsW p,string r)
        {
            InitializeComponent();
            padre = p;
            resizer = r;

            this.Text = r;
            this.Disposed += new EventHandler(ResolutionInputBox_Disposed);
            
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;

            ancho.TextChanged += new EventHandler(ancho_TextChanged);
            alto.TextChanged += new EventHandler(alto_TextChanged);
            aspect.SelectedIndexChanged += new EventHandler(aspect_SelectedIndexChanged);
            mod16.CheckedChanged += new EventHandler(mod16_CheckedChanged);
        }

        void ForceMOD16()
        {
            if (!mod16.Checked) return;

            int an = int.Parse(ancho.Text);
            int al = int.Parse(alto.Text);

            if ((an % 16) != 0)
                an = ((an / 16) + 1) * 16;
            if ((al % 16) != 0)
                al = ((al / 16) + 1) * 16;

            ancho.Text = an.ToString();
            alto.Text = al.ToString();

        }

        void mod16_CheckedChanged(object sender, EventArgs e)
        {
            ForceMOD16();
        }

        void aspect_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int al = int.Parse(alto.Text);

                switch (aspect.Text)
                {
                    case "16:9":
                        int bleh = Convert.ToInt32((double)al * widescreen);
                        ancho.Text = bleh.ToString();
                        break;
                    case "4:3":
                        int bleh2 = Convert.ToInt32((double)al * normal);
                        ancho.Text = bleh2.ToString();
                        break;
                }
            }
            catch { }
        }

        void alto_TextChanged(object sender, EventArgs e)
        {

            if (!alto.Focused) return;

            try
            {
                int al = int.Parse(alto.Text);

                switch (aspect.Text)
                {
                    case "16:9":
                        int bleh = Convert.ToInt32((double)al * widescreen);
                        ancho.Text = bleh.ToString();
                        break;
                    case "4:3":
                        int bleh2 = Convert.ToInt32((double)al * normal);
                        ancho.Text = bleh2.ToString();
                        break;
                }
            }
            catch { }
        }

        void ancho_TextChanged(object sender, EventArgs e)
        {

            if (!ancho.Focused) return;

            try
            {
                int an = int.Parse(ancho.Text);

                switch (aspect.Text)
                {
                    case "16:9":
                        int bleh = Convert.ToInt32((double)an / widescreen);
                        alto.Text = bleh.ToString();
                        break;
                    case "4:3":
                        int bleh2 = Convert.ToInt32((double)an / normal);
                        alto.Text = bleh2.ToString();
                        break;
                }
            }
            catch { }
        }

        void ResolutionInputBox_Disposed(object sender, EventArgs e)
        {
            padre.Enabled = true;
            padre.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ForceMOD16();
            padre.InsertAVSCode(resizer + "(" +ancho.Text+ "," +alto.Text+ ")");
            this.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }        


    }
}