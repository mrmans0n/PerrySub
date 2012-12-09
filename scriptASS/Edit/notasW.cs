using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace scriptASS
{
    public partial class notasW : Form
    {
        private ArrayList estList;
        private mainW mW;

        public notasW(mainW mW)
        {
            InitializeComponent();
            this.mW = mW;
            //textNewNota.KeyPress += new KeyPressEventHandler(textNewNota_KeyPress);
            //textNota.KeyDown += new KeyEventHandler(textNota_KeyDown);
            this.MaximumSize = this.MinimumSize = this.Size;
            toolStripTextBox1.KeyPress += new KeyPressEventHandler(toolStripTextBox1_KeyPress);
        }

        void toolStripTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (Convert.ToInt32(e.KeyChar))
            {
                case 13:
                    string fil = toolStripTextBox1.Text;
                    for (int i = 0; i < FileAccessWrapper.InvalidCharacters.Length; i++)
                        fil = fil.Replace(FileAccessWrapper.InvalidCharacters[i], '_');

                    string newf = mW.notesDir + "\\" + fil + ".Notes";
                    if (File.Exists(newf))
                        MessageBox.Show("Ya existe una nota guardada con el mismo nombre", "perrySub", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {

                        TextWriter o = new StreamWriter(newf, false, System.Text.Encoding.UTF8);
                        o.WriteLine("{ Introduce aquí el código de la nota. }");
                        o.WriteLine();
                        o.WriteLine("{ Sintaxis ... }");
                        o.WriteLine("{ \t%text\t donde quieras que vaya el texto }");
                        o.WriteLine("{ \t%height\t donde necesites el valor de la altura }");
                        o.WriteLine("{ \t%width\t donde necesites el valor de la anchura }");
                        o.Close();

                        rellenaNotas();
                        listBox1.Focus();
                        if (listBox1.Items.Count > 1) listBox1.SelectedIndex = 0;
                        SendKeys.Send("");
                        e.Handled = true;
                        contextMenuStrip1.Hide();
                    }

                    break;
            }
        }

        /*
        void textNota_KeyDown(object sender, KeyEventArgs e)
        {
            string newf = "";
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if (e.Control)
                    {
                        newf = mW.notesDir + "\\" + textNewNota.Text + ".Notes";
                        TextWriter o = new StreamWriter(newf, false, System.Text.Encoding.UTF8);
                        for (int i = 0; i < textNota.Lines.Length; i++)
                            o.WriteLine(textNota.Lines[i].ToString());
                        o.Close();
                        e.Handled = true;
                        SendKeys.Send("");
                    }
                    break;
            }

        }

        void textNewNota_KeyPress(object sender, KeyPressEventArgs e)
        {
            string newf="";
            switch (Convert.ToInt32(e.KeyChar))
            {
                case 13:

                    string fil = textNewNota.Text;
                    for (int i = 0; i < FileAccessWrapper.InvalidCharacters.Length; i++)
                        fil = fil.Replace(FileAccessWrapper.InvalidCharacters[i], '_');

                    newf = mW.notesDir + "\\" + fil + ".Notes";
                    if (File.Exists(newf))
                        MessageBox.Show("Ya existe una nota guardada con el mismo nombre", "perrySub", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {                       

                        TextWriter o = new StreamWriter(newf, false, System.Text.Encoding.UTF8);
                        o.WriteLine("{ Introduce aquí el código de la nota. }");
                        o.WriteLine();
                        o.WriteLine("{ Sintaxis ... }");
                        o.WriteLine("{ \t%text\t donde quieras que vaya el texto }");
                        o.WriteLine("{ \t%height\t donde necesites el valor de la altura }");
                        o.WriteLine("{ \t%width\t donde necesites el valor de la anchura }");
                        o.Close();

                        rellenaNotas();
                        listBox1.Focus();                        
                        newNote.Visible = false;
                        if (listBox1.Items.Count > 1) listBox1.SelectedIndex = 0;
                    }
                    SendKeys.Send("");
                    e.Handled = true;
                    break;
                default:
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void btn3_Click(object sender, EventArgs e)
        {
            mW.Enabled = true;
            mW.Focus();
            this.Dispose();
        }
        */
        private void rellenaNotas()
        {
            listBox1.Items.Clear();

            DirectoryInfo d = new DirectoryInfo(mW.notesDir);
            foreach (FileInfo f in d.GetFiles("*.Notes"))
            {
                string s = f.Name.Substring(0, f.Name.LastIndexOf(".Notes"));
                listBox1.Items.Add(s);
            }
        }

        private void notasW_Load(object sender, EventArgs e)
        {
            foreach (estiloV4 v in mW.script.GetStyles())
                if (!comboEstilos2.Items.Contains(v.Name))
                    comboEstilos2.Items.Add(v.Name);
            
            rellenaNotas();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1) return;

            StreamReader sr = FileAccessWrapper.OpenTextFile(mW.notesDir+"\\"+listBox1.Items[listBox1.SelectedIndex] + ".Notes");
            textNota.Text = sr.ReadToEnd();
            sr.Close();

        }
        /*
        private void button2_Click(object sender, EventArgs e)
        {
            newNote.Visible = true;
            textNewNota.Focus();
        }
        */
        // nuevos

        private void nuevoToolStripButton_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.Show(toolStrip1, new Point(0, toolStrip1.Height));
            toolStripTextBox1.Text = "Nueva nota";
            toolStripTextBox1.Focus();
            toolStripTextBox1.SelectAll();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1) return;
            string fname = mW.notesDir + "\\" + listBox1.Text + ".Notes";
            if (MessageBox.Show("¿Seguro que deseas borrar '" + listBox1.Text + "'?", mainW.appTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                File.Delete(fname);
                rellenaNotas();
                if (listBox1.Items.Count > 1) listBox1.SelectedIndex = 0;
            }

        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            mW.Enabled = true;
            if ((comboEstilos2.SelectedIndex != -1) && (listBox1.SelectedIndex != -1))
            {
                mW.UndoRedo.AddUndo(mW.script, "Notas de traducción");

                int g = mW.script.LineCount;
                for (int i = 0; i < g; i++)
                {
                    lineaASS lass = (lineaASS)mW.script.GetLines()[i];
                    if (lass.estilo.Equals(comboEstilos2.Text))
                    {
                        lass.clase = "Comment";
                        estiloV4 v = null;
                        int ancho = 0, alto = 0;

                        foreach (estiloV4 e4 in mW.v4)
                            if (e4.Name.Equals(lass.estilo)) v = e4;

                        if (v != null)
                        {
                            Graphics gr = System.Drawing.Graphics.FromImage(new Bitmap(1, 1));

                            StringFormat stringformat = new StringFormat(StringFormat.GenericTypographic);
                            FontStyle fs = (v.Bold) ? FontStyle.Bold : FontStyle.Regular;
                            fs = (v.Italic) ? fs & FontStyle.Italic : fs;
                            Font fuente = new Font(v.FontName, v.FontSize, fs);

                            ancho = Convert.ToInt32(gr.MeasureString(lass.texto, fuente, new PointF(0, 0), stringformat).Width);
                            alto = Convert.ToInt32(gr.MeasureString(lass.texto, fuente, new PointF(0, 0), stringformat).Height);
                        }

                        //al.Capacity += textNota.Lines.Length;

                        for (int x = 0; x < textNota.Lines.Length; x++)
                        {
                            string m = textNota.Lines[x].ToString().Replace("%text", lass.texto).Replace("%height", alto.ToString()).Replace("%width", ancho.ToString());
                            if (!m.Equals(string.Empty))
                                mW.script.GetLines().Add(new lineaASS(lass.t_inicial.ToString(), lass.t_final.ToString(), comboEstilos2.Text, m));
                        }
                    }
                }

                mW.updateGridWithArrayList(mW.script.GetLines());
            }
            mW.Focus();
            this.Dispose();
        }

        private void guardarToolStripButton_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1) return;
            TextWriter tw = new StreamWriter(mW.notesDir + "\\" + listBox1.Text + ".Notes", false, Encoding.UTF8);
            foreach (string s in textNota.Lines)
                tw.WriteLine(s);
            tw.Close();
        }

        
    }
}