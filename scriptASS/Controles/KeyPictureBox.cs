using System;
using System.Windows.Forms;

namespace scriptASS
{
    class KeyPictureBox : PictureBox
    {
        private const short WM_KEYDOWN = 0x100;
        private const short WM_KEYUP = 0x101;

        public delegate void OnKeyPress(object sender, KeyEventArgs e);
        public event OnKeyPress KeyDown;
        public event OnKeyPress KeyUp;

        // PictureBox con KeyDown / KeyUp :)

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == WM_KEYDOWN && KeyDown!=null)
            {
                Keys keyCode = (Keys)(int)m.WParam & Keys.KeyCode;
                KeyEventArgs e1 = new KeyEventArgs(keyCode);
                KeyDown(this, e1);                
            }
            else if (m.Msg == WM_KEYUP && KeyUp != null)
            {
                Keys keyCode = (Keys)(int)m.WParam & Keys.KeyCode;
                KeyEventArgs e1 = new KeyEventArgs(keyCode);
                KeyUp(this, e1);                
            }
            else
                base.WndProc(ref m);
        }


    }
}
