using System;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace scriptASS
{

    public partial class TimeTextBox : UserControl
    {

        [DllImport("user32")]
        private static extern bool HideCaret(IntPtr hWnd);
        [DllImport("user32")]
        private static extern bool ShowCaret(IntPtr hWnd);

        #region consultores varios
        public override Color ForeColor
        {
            get
            {
                return textH.ForeColor;
            }
            set
            {
                textH.ForeColor = value;
                textM.ForeColor = value;
                textS.ForeColor = value;
                textMS.ForeColor = value;
            }
        }

        public override Color BackColor
        {
            get
            {
                return styleTextBox.BackColor;
            }
            set
            {
                styleTextBox.BackColor = value;
                textH.BackColor = value;
                textM.BackColor = value;
                textS.BackColor = value;
                textMS.BackColor = value;
            }
        }

        public override Size MinimumSize
        {
            get
            {
                return new Size(72, 20);
            }
        }


        public override string Text
        {
            get { return textH.Text + ":" + textM.Text + ":" + textS.Text + "." + textMS.Text; }
            set
            {
                try
                {
                    string s = value;
                    string[] hms = s.Split(':');
                    textH.Text = hms[0];
                    textM.Text = hms[1];
                    string[] s_ms = hms[2].Split('.');
                    textS.Text = s_ms[0];
                    textMS.Text = s_ms[1];
                }
                catch
                {
                    textH.Text = "00"; textM.Text = "00"; textS.Text = "00"; textMS.Text = "00";
                }
            }
        }

        private bool readOnly = false;

        public bool ReadOnly
        {
            get { return readOnly; }
            set
            {
                readOnly = value;
                this.BackColor = (readOnly) ? SystemColors.Control : Color.White;
            }
        }

        #endregion

        public delegate void OnTimeTextBoxValidated(object sender, EventArgs e);
        public event OnTimeTextBoxValidated TimeValidated;

        public TimeTextBox()
        {
            InitializeComponent();

            textH.EnabledChanged += new EventHandler(TimeTextBox_EnabledChanged);

            textH.KeyPress += new KeyPressEventHandler(textH_KeyPress);
            textH.KeyDown += new KeyEventHandler(textH_KeyDown);
            textM.KeyPress += new KeyPressEventHandler(textM_KeyPress);
            textM.KeyDown += new KeyEventHandler(textM_KeyDown);
            textS.KeyPress += new KeyPressEventHandler(textS_KeyPress);
            textS.KeyDown += new KeyEventHandler(textS_KeyDown);
            textMS.KeyPress += new KeyPressEventHandler(textMS_KeyPress);
            textMS.KeyDown += new KeyEventHandler(textMS_KeyDown);
            textM.LostFocus += new EventHandler(lostFocus);
            textS.LostFocus += new EventHandler(lostFocus);
            this.Size = styleTextBox.Size;
            this.GotFocus += new EventHandler(TimeTextBox_GotFocus);
            label1.Click += new EventHandler(label1_Click);
            transparentLabel1.Click += new EventHandler(transparentLabel1_Click);
            transparentLabel2.Click += new EventHandler(transparentLabel2_Click);
            transparentLabel1.ForeColor = label1.ForeColor = transparentLabel2.ForeColor = textH.ForeColor;

            // desactivar seleccion
            textH.DoubleClick += new EventHandler(text_DoubleClick);
            textM.DoubleClick += new EventHandler(text_DoubleClick);
            textS.DoubleClick += new EventHandler(text_DoubleClick);
            textMS.DoubleClick += new EventHandler(text_DoubleClick);
        }

        void text_DoubleClick(object sender, EventArgs e)
        {
            TextBox t = (TextBox)sender;
            HideCaret(t.Handle);
            t.SelectionLength = 0;
        }


        void transparentLabel2_Click(object sender, EventArgs e)
        {
            textS.Focus();
            textS.SelectionLength = 0;
            textS.SelectionStart = 2;
        }

        void transparentLabel1_Click(object sender, EventArgs e)
        {
            textM.Focus();
            textM.SelectionLength = 0;
            textM.SelectionStart = 2;
        }

        void label1_Click(object sender, EventArgs e)
        {
            textH.Focus();
            textH.SelectionLength = 0;
            textH.SelectionStart = 2;
        }



        void TimeTextBox_EnabledChanged(object sender, EventArgs e)
        {
            styleTextBox.BackColor = textH.BackColor;
            textBox1.BackColor = textH.BackColor;
            label1.Refresh();
            transparentLabel1.ForeColor = label1.ForeColor = transparentLabel2.ForeColor = textH.ForeColor;
            transparentLabel1.Refresh();
            transparentLabel2.Refresh();
        }


        void TimeTextBox_GotFocus(object sender, EventArgs e)
        {
            textH.Focus();
        }

        void lostFocus(object sender, EventArgs e)
        {
            adjustToMaxValues();
        }

        private void adjustToMaxValues()
        {
            if (textH.Text == string.Empty) textH.Text = "00";
            if (textM.Text == string.Empty) textM.Text = "00";
            if (textS.Text == string.Empty) textS.Text = "00";
            if (textMS.Text == string.Empty) textMS.Text = "00";


            if (int.Parse(textM.Text) > 59) textM.Text = "59";
            if (int.Parse(textS.Text) > 59) textS.Text = "59";

        }

        #region textH handlers

        void textH_KeyDown(object sender, KeyEventArgs e)
        {
            if (readOnly) return;            
            switch (e.KeyCode)
            {
                case Keys.Right:
                    if (textH.SelectionStart + textH.SelectionLength >= textH.MaxLength - 1)
                        textM.Focus();
                    break;
                case Keys.Back:
                case Keys.Delete:
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
            }
            //throw this.KeyDown();
        }

        void textH_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (readOnly) return;
            if (textH.Text.Length == textH.MaxLength && textH.SelectedText == "")
            {
                textH.SelectionLength = 1;
            }

            if (textH.SelectionStart == textH.MaxLength - 1)
            {
                textM.Focus();
                textM.SelectionLength = 0;
                textM.SelectionStart = 0;
            }

            switch (Convert.ToInt32(e.KeyChar))
            {
                case 13:
                    if (textH.Text == string.Empty)
                        textH.Text = "00";
                    textM.Focus();
                    SendKeys.Send("");
                    e.Handled = true;

                    EventArgs e1 = new EventArgs();
                    TimeValidated(this, e1);

                    break;
                case 32:
                    if (textH.Text == string.Empty)
                        textH.Text = "00";
                    textM.Focus();
                    break; // espacio
            }
        }
        #endregion

        #region textM handlers

        void textM_KeyDown(object sender, KeyEventArgs e)
        {
            if (readOnly) return;
            switch (e.KeyCode)
            {
                case Keys.Right:
                    if (textM.SelectionStart + textM.SelectionLength >= textM.MaxLength - 1)
                    {
                        adjustToMaxValues();
                        textS.Focus();
                    }
                    break;
                case Keys.Left:
                    if (textM.SelectionStart == 0)
                    {
                        adjustToMaxValues();
                        textH.Focus();
                        textH.SelectionStart = textH.MaxLength - 1;

                        if (textM.Text == string.Empty)
                            textM.Text = "00";

                    }
                    break;
                case Keys.Back:
                case Keys.Delete:
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;

            }
        }

        void textM_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (readOnly) return;
            if (textM.Text.Length == textM.MaxLength && textM.SelectedText == "")
            {
                textM.SelectionLength = 1;
            }

            if (textM.SelectionStart == textM.MaxLength - 1)
            {
                textS.Focus();
                textS.SelectionLength = 0;
                textS.SelectionStart = 0;
            }


            switch (Convert.ToInt32(e.KeyChar))
            {
                case 13:
                    adjustToMaxValues();
                    if (textM.Text == string.Empty)
                        textM.Text = "00";
                    textS.Focus();
                    SendKeys.Send("");
                    e.Handled = true;
                    EventArgs e1 = new EventArgs();
                    TimeValidated(this, e1);
                    break;
                case 32:
                    adjustToMaxValues();
                    if (textM.Text == string.Empty)
                        textM.Text = "00";

                    textS.Focus();
                    break;
            }
        }
        #endregion

        #region textS handlers

        void textS_KeyDown(object sender, KeyEventArgs e)
        {
            if (readOnly) return;
            switch (e.KeyCode)
            {
                case Keys.Right:
                    if (textS.SelectionStart + textS.SelectionLength >= textS.MaxLength - 1)
                    {
                        adjustToMaxValues();
                        textMS.Focus();
                    }
                    break;
                case Keys.Left:
                    if (textS.SelectionStart == 0)
                    {
                        textM.Focus();
                        textM.SelectionStart = textM.MaxLength - 1;
                        adjustToMaxValues();
                        if (textS.Text == string.Empty)
                            textS.Text = "00";

                    }
                    break;
                case Keys.Back:
                case Keys.Delete:
                    e.Handled = true;
                    e.SuppressKeyPress = true;

                    break;

            }
        }

        void textS_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (readOnly) return;
            if (textS.Text.Length == textS.MaxLength && textS.SelectedText == "")
            {
                textS.SelectionLength = 1;
            }

            if (textS.SelectionStart == textS.MaxLength - 1)
            {
                textMS.Focus();
                textMS.SelectionLength = 0;
                textMS.SelectionStart = 0;
            }


            switch (Convert.ToInt32(e.KeyChar))
            {
                case 13:
                    adjustToMaxValues();
                    if (textS.Text == string.Empty)
                        textS.Text = "00";
                    textMS.Focus();
                    SendKeys.Send("");
                    e.Handled = true;
                    EventArgs e1 = new EventArgs();
                    TimeValidated(this, e1);
                    break;
                case 32:
                    adjustToMaxValues();
                    if (textS.Text == string.Empty)
                        textS.Text = "00";

                    textMS.Focus();
                    break;
            }
        }
        #endregion

        #region textMS handlers

        void textMS_KeyDown(object sender, KeyEventArgs e)
        {
            if (readOnly) return;
            switch (e.KeyCode)
            {
                case Keys.Left:
                    if (textMS.SelectionStart == 0)
                    {
                        textS.Focus();
                        textS.SelectionStart = textS.MaxLength - 1;
                    }
                    break;
                case Keys.Back:
                case Keys.Delete:
                    e.Handled = true;
                    e.SuppressKeyPress = true;

                    break;
            }
        }

        void textMS_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (readOnly) return;
            if (textMS.Text.Length == textMS.MaxLength && textMS.SelectedText == "")
            {
                textMS.SelectionLength = 1;
            }

            switch (Convert.ToInt32(e.KeyChar))
            {
                case 13:
                    if (textMS.Text == string.Empty)
                        textMS.Text = "00";
                    SendKeys.Send("");
                    e.Handled = true;
                    EventArgs e1 = new EventArgs();
                    TimeValidated(this, e1);
                    break;
                case 32:
                    if (textMS.Text == string.Empty)
                        textMS.Text = "00";
                    break;
            }
        }
        #endregion
    }
}
