using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace scriptASS
{
    public partial class templateW : Form
    {
        mainW mW;

        headerASS actual = new headerASS();

        public templateW(mainW mw)
        {
            mW = mw;
            InitializeComponent();

            toolStripTextBox1.KeyPress += new KeyPressEventHandler(toolStripTextBox1_KeyPress);

            RefreshList();
        }

        void toolStripTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (Convert.ToInt32(e.KeyChar))
            {
                case 13:

                    if (listBox1.Items.Contains(toolStripTextBox1.Text))
                        MessageBox.Show("Ya existe un template con ese nombre", mainW.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {
                        headerASS bleh = new headerASS();
                        bleh.SetHeaderDefaults();
                        bleh.SaveTo(mW.templateDir+"\\"+toolStripTextBox1.Text + ".template");
                        contextNuevo.Hide();

                        RefreshList();
                        SendKeys.Send("");
                        e.Handled = true;
                    }
                    break;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // actualizar campos
            groupBox1.Enabled = (listBox1.SelectedIndex != -1);
            actual.LoadFrom(mW.templateDir + "\\" + listBox1.Text + ".template");
            RefreshFields();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // add
            contextNuevo.Show(button1, new Point(0, button1.Height));
            toolStripTextBox1.Text = "Nuevo template";
            toolStripTextBox1.SelectAll();
            toolStripTextBox1.Focus();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // del
            File.Delete(Path.Combine(mW.templateDir,listBox1.Text + ".template"));
            RefreshList();
            ClearFields();
            groupBox1.Enabled = false;
        }

        private void ClearFields()
        {
            headTitulo.Text = headOriginal.Text = headTradu.Text = headEdicion.Text = headTiempos.Text = headRevision.Text = headX.Text = headY.Text = "";
            headEst.Text = "(ninguno)";

        }

        private void RefreshFields()
        {
            headTitulo.Text = actual.GetHeaderValue("Title");
            headOriginal.Text = actual.GetHeaderValue("Original Script");
            headTradu.Text = actual.GetHeaderValue("Original Translation");
            headEdicion.Text = actual.GetHeaderValue("Original Editing");
            headTiempos.Text = actual.GetHeaderValue("Original Timing");
            headRevision.Text = actual.GetHeaderValue("Original Script Checking");
            headX.Text = actual.GetHeaderValue("PlayResX");
            headY.Text = actual.GetHeaderValue("PlayResY");
            headEst.Text = actual.ExistsHeaderValue("Last Style Storage") ? actual.GetHeaderValue("Last Style Storage") : "(ninguno)";
        }

        private void RefreshList()
        {
            DirectoryInfo d = new DirectoryInfo(mW.templateDir);
            listBox1.Items.Clear();
            foreach (FileInfo f in d.GetFiles("*.template"))
                listBox1.Items.Add(f.Name.Substring(0, f.Name.LastIndexOf(".template")));

            headEst.Items.Clear();
            headEst.Items.Add("(ninguno)");
            d = new DirectoryInfo(mW.stylesDir);
            foreach (FileInfo f in d.GetFiles("*.Styles"))
                headEst.Items.Add(f.Name.Substring(0, f.Name.LastIndexOf(".Styles")));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!headTitulo.Text.Equals(string.Empty)) actual.SetHeaderValue("Title", headTitulo.Text);
            if (!headOriginal.Text.Equals(string.Empty)) actual.SetHeaderValue("Original Script", headOriginal.Text);
            if (!headTradu.Text.Equals(string.Empty)) actual.SetHeaderValue("Original Translation", headTradu.Text);
            if (!headEdicion.Text.Equals(string.Empty)) actual.SetHeaderValue("Original Editing", headEdicion.Text);
            if (!headTiempos.Text.Equals(string.Empty)) actual.SetHeaderValue("Original Timing", headTiempos.Text);
            if (!headRevision.Text.Equals(string.Empty)) actual.SetHeaderValue("Original Script Checking", headRevision.Text);
            if (!headX.Text.Equals(string.Empty)) actual.SetHeaderValue("PlayResX", headX.Text);
            if (!headY.Text.Equals(string.Empty)) actual.SetHeaderValue("PlayResY", headY.Text);
            if (!headEst.Text.Equals("(ninguno)")) actual.SetHeaderValue("Last Style Storage", headEst.Text);
            actual.SaveTo(mW.templateDir + "\\" + listBox1.Text + ".template");
        }

    }
}