using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace scriptASS
{
    public partial class headersW : Form
    {
        private mainW mW;
        public headersW(mainW mW)
        {
            InitializeComponent();
            this.mW = mW;
        }

        private void headersW_Load(object sender, EventArgs e)
        {
            updateHeaderGrid();
        }

        private void updateHeaderGrid()
        {
            int i = 0;
            gridHeader.RowCount = mW.head.Count;
            if (mW.head.Count == 0) return;
            foreach (string s in mW.head)
            {
                int idx = s.IndexOf(": ");
                if (idx != -1)
                {
                    string key = s.Substring(0, idx);
                    string value = s.Substring(idx + 2);
                    gridHeader["Clave", i].Value = key;
                    gridHeader["Valor", i].Value = value;
                    i++;
                }

            }
            headTitulo.Text = mW.script.GetHeader().GetHeaderValue("Title");
            headOriginal.Text = mW.script.GetHeader().GetHeaderValue("Original Script");
            headTradu.Text = mW.script.GetHeader().GetHeaderValue("Original Translation");
            headEdicion.Text = mW.script.GetHeader().GetHeaderValue("Original Editing");
            headTiempos.Text = mW.script.GetHeader().GetHeaderValue("Original Timing");
            headRevision.Text = mW.script.GetHeader().GetHeaderValue("Original Script Checking");
            headX.Text = mW.script.GetHeader().GetHeaderValue("PlayResX");
            headY.Text = mW.script.GetHeader().GetHeaderValue("PlayResY");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            mW.UndoRedo.AddUndo(mW.script, "Modificar cabecera");
            mW.head.Clear();
            for (int i = 0; i < gridHeader.RowCount; i++)
                mW.script.GetHeader().SetHeaderValue(gridHeader["Clave", i].Value.ToString(), gridHeader["Valor", i].Value.ToString());

            if (!headTitulo.Text.Equals(string.Empty)) mW.script.GetHeader().SetHeaderValue("Title", headTitulo.Text);
            if (!headOriginal.Text.Equals(string.Empty)) mW.script.GetHeader().SetHeaderValue("Original Script", headOriginal.Text);
            if (!headTradu.Text.Equals(string.Empty)) mW.script.GetHeader().SetHeaderValue("Original Translation", headTradu.Text);
            if (!headEdicion.Text.Equals(string.Empty)) mW.script.GetHeader().SetHeaderValue("Original Editing", headEdicion.Text);
            if (!headTiempos.Text.Equals(string.Empty)) mW.script.GetHeader().SetHeaderValue("Original Timing", headTiempos.Text);
            if (!headRevision.Text.Equals(string.Empty)) mW.script.GetHeader().SetHeaderValue("Original Script Checking", headRevision.Text);
            if (!headX.Text.Equals(string.Empty)) mW.script.GetHeader().SetHeaderValue("PlayResX", headX.Text);
            if (!headY.Text.Equals(string.Empty)) mW.script.GetHeader().SetHeaderValue("PlayResY", headY.Text);

            this.Dispose();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            mW.script.GetHeader().SetHeaderValue("Nombre de Clave", "Valor");            
            updateHeaderGrid();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (gridHeader.SelectedRows.Count<=0) return;
            try
            {
                for (int i = gridHeader.SelectedRows.Count; i != 0; i--)
                {
                    int idx = gridHeader.SelectedRows[i - 1].Index;
                    mW.head.RemoveAt(idx);
                }
            }
            catch { mW.errorMsg("Ha habido un error procesando el borrado."); }
            updateHeaderGrid();
        }
    }
}