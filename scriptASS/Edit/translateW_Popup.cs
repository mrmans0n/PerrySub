using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace scriptASS
{
    public enum TranslationStyle : int
    {
        FromScriptWithActors = 0,
        FromScriptWithoutActors = 1,
        FromScratch = 2,
        FromScratchAudio = 3
    }

    public partial class translateW_Popup : Form
    {

        private TranslationStyle style;

        public TranslationStyle Style
        {
            get { return style; }
            set { style = value; }
        }

        private mainW mw;

        public translateW_Popup(mainW mw)
        {
            InitializeComponent();

            this.mw = mw;

            radioButton1.CheckedChanged += new EventHandler(SelectionChanged);
            radioButton2.CheckedChanged += new EventHandler(SelectionChanged);
            radioButton3.CheckedChanged += new EventHandler(SelectionChanged);
            radioButton4.CheckedChanged += new EventHandler(SelectionChanged);
            ShowPopup.CheckedChanged += new EventHandler(ShowPopup_CheckedChanged);

            this.Disposed += new EventHandler(translateW_Popup_Disposed);

        }

        void ShowPopup_CheckedChanged(object sender, EventArgs e)
        {
            mw.updateReplaceConfigFile("translateW_ShowPopup", (!ShowPopup.Checked).ToString());
        }

        void translateW_Popup_Disposed(object sender, EventArgs e)
        {
            // script con actores = yes , sin actores = no
            // no script = cancel, de audio = ignore

            switch (style)
            {
                case TranslationStyle.FromScriptWithActors:
                    this.DialogResult = DialogResult.Yes;
                    break;
                case TranslationStyle.FromScriptWithoutActors:
                    this.DialogResult = DialogResult.No;
                    break;
                case TranslationStyle.FromScratch:
                    this.DialogResult = DialogResult.Cancel;
                    break;
                case TranslationStyle.FromScratchAudio:
                    this.DialogResult = DialogResult.Ignore;
                    break;
            }
        }

        void SelectionChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
                style = TranslationStyle.FromScriptWithActors;
            else if (radioButton2.Checked)
                style = TranslationStyle.FromScriptWithoutActors;
            else if (radioButton3.Checked)
                style = TranslationStyle.FromScratch;
            else if (radioButton4.Checked)
                style = TranslationStyle.FromScratchAudio;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

    }
}
