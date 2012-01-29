using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace scriptASS
{
    public partial class undoRedoW : Form
    {
        mainW mw;
        public undoRedoW(mainW m)
        {
            InitializeComponent();

            mw = m;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void UpdateUndoRedo()
        {
            UndoRedoSubtitleScript[] undo = mw.UndoRedo.GetUndoArray();
            UndoRedoSubtitleScript[] redo = mw.UndoRedo.GetRedoArray();

            listBox1.DataSource = undo;
            listBox1.DisplayMember = "ActionName";
            listBox2.DataSource = redo;
            listBox2.DisplayMember = "ActionName";
        }

        private void undoRedoW_Load(object sender, EventArgs e)
        {
            UpdateUndoRedo();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex == -1) return;

            int iteraciones = listBox2.SelectedIndex + 1;

            for (int i = 0; i < iteraciones; i++)
                mw.script = mw.UndoRedo.GetRedo(mw.script);

            mw.al = mw.script.GetLines();
            mw.v4 = mw.script.GetStyles();
            mw.head = mw.script.GetHeaders();

            mw.updateGridWithArrayList(mw.al);
            UpdateUndoRedo();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1) return;

            int iteraciones = listBox1.SelectedIndex + 1;

            for (int i = 0; i < iteraciones; i++)
                mw.script = mw.UndoRedo.GetUndo(mw.script);

            mw.al = mw.script.GetLines();
            mw.v4 = mw.script.GetStyles();
            mw.head = mw.script.GetHeaders();

            mw.updateGridWithArrayList(mw.al);
            UpdateUndoRedo();

        }
    }
}