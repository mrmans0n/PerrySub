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
    public partial class FontSelectionW : Form
    {
        ArrayList fonts = new ArrayList();
        string old;
        public FontSelectionW(string oldf,string oldsize)
        {
            InitializeComponent();
            old = oldf;
            FontSize.Value = decimal.Parse(oldsize);
            this.DialogResult = DialogResult.Cancel;
        }

        private void FontSelectionW_Load(object sender, EventArgs e)
        {
            int found = -1;
            int c = 0;
            foreach (System.Drawing.FontFamily f in System.Drawing.FontFamily.Families)
            {
                /*
                ToolStripMenuItem _TSI = new ToolStripMenuItem();                               

                if (f.IsStyleAvailable(FontStyle.Regular))
                {
                    _f = new Font(f, 12, FontStyle.Regular);
                }
                else if (f.IsStyleAvailable(FontStyle.Bold))
                {
                    _f = new Font(f, 12, FontStyle.Bold);
                }
                else if (f.IsStyleAvailable(FontStyle.Italic))
                {
                    _f = new Font(f, 12, FontStyle.Italic);
                }
                else if (f.IsStyleAvailable(FontStyle.Underline))
                {
                    _f = new Font(f, 12, FontStyle.Underline);
                }
                else if (f.IsStyleAvailable(FontStyle.Strikeout))
                {
                    _f = new Font(f, 12, FontStyle.Strikeout);
                }
                */
                //_TSI.Font = _f;
                //_TSI.Text = f.Name;
                
                FontList.Items.Add(f.Name);
                if (f.Name.Equals(old,StringComparison.InvariantCultureIgnoreCase))
                    found = c;
                fonts.Add(f);
                c++;
                //fontToolStripDropDownButton.DropDownItems.Add(_TSI);
            }

            if (found != -1)
            {
                FontList.SelectedIndex = found;
                FontList.TopIndex = found;
                UpdateFontBox();
            }
        }

        private void UpdateFontBox()
        {
            if (FontList.SelectedIndex == -1) return;
            FontFamily f = (FontFamily)fonts[FontList.SelectedIndex];
            Font _f = null;

            if (f.IsStyleAvailable(FontStyle.Regular))
            {
                label2.Text = "Fuente mostrada en estilo Regular";
                _f = new Font(f, (float)FontSize.Value, FontStyle.Regular);
            }
            else if (f.IsStyleAvailable(FontStyle.Bold))
            {
                label2.Text = "Fuente mostrada en estilo Bold";
                _f = new Font(f, (float)FontSize.Value, FontStyle.Bold);
            }
            else if (f.IsStyleAvailable(FontStyle.Italic))
            {
                label2.Text = "Fuente mostrada en estilo Italic";
                _f = new Font(f, (float)FontSize.Value, FontStyle.Italic);
            }
            else if (f.IsStyleAvailable(FontStyle.Underline))
            {
                label2.Text = "Fuente mostrada en estilo Underline";
                _f = new Font(f, (float)FontSize.Value, FontStyle.Underline);
            }
            else if (f.IsStyleAvailable(FontStyle.Strikeout))
            {
                label2.Text = "Fuente mostrada en estilo Strikeout";
                _f = new Font(f, (float)FontSize.Value, FontStyle.Strikeout);
            }

            //richTextBox1.Font.S = FontSize.Value;
            if (_f == null)
            {
            }
            else
            {
                richTextBox1.Font = _f;

                if (richTextBox1.Text.Equals(""))
                    richTextBox1.Text = "¡¿Pablito clavó un clavito?!";
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {            

            UpdateFontBox();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        private void FontSize_ValueChanged(object sender, EventArgs e)
        {
            UpdateFontBox();
        }
    }
}