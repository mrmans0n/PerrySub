using System;
using System.Collections.Generic;
using System.Text;

namespace scriptASS
{
    class NumericToolStripTextBox : System.Windows.Forms.ToolStripTextBox
    {

        private bool point = false;
        private bool doublepoint = false;

        public bool Point
        {
            get { return point; }
            set { point = value; }
        }        

        public bool DoublePoint
        {
            get { return doublepoint; }
            set { doublepoint = value; }
        }

        protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (!(e.KeyChar == '.')) && (!(e.KeyChar == ':')))
            {
                e.Handled = true;
            }

            if (!point && (e.KeyChar == '.'))
                e.Handled = true;

            if (!doublepoint && (e.KeyChar == ':'))
                e.Handled = true;

            base.OnKeyPress(e);
        }

    }
}
