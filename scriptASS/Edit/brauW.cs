using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace scriptASS
{
    public partial class brauW : Form
    {
        private mainW mW;
        private ArrayList persList, al;
        private const string noStyle = "(sin cambios)";
        private const string ninguno = "(ninguno)";

        public brauW(mainW mW)
        {
            InitializeComponent();
            this.mW = mW;
            this.Load += new EventHandler(stylerW_Load);
        }

        private void stylerW_Load(object sender, EventArgs e)
        {

            persList = new ArrayList();
            al = mW.al;

            foreach (lineaASS lass in al)
                if (!persList.Contains(lass.personaje))
                    persList.Add(lass.personaje);

            ArrayList c = new ArrayList();
            ArrayList d = new ArrayList();
            c.Add(noStyle);
            d.Add(ninguno);
            foreach (estiloV4 e4 in mW.v4)
            {
                if (!c.Contains(e4.Name))
                    c.Add(e4.Name);
                if (!d.Contains(e4.Name))
                    d.Add(e4.Name);
            }
            Estilo.DataSource = c;
            comboDefaultStyle.DataSource = d;
            dataGridView1.RowCount = persList.Count;
            for (int i = 0; i < persList.Count; i++)
            {
                dataGridView1["Personaje", i].Value = persList[i];
                dataGridView1["Estilo", i].Value = noStyle;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {            
            foreach (lineaASS lass in al)
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    if (lass.personaje.Equals(dataGridView1["Personaje", i].Value))
                    {
                        string est = (string)dataGridView1["Estilo", i].Value;
                        bool be1 = Convert.ToBoolean(dataGridView1["be1", i].Value);
                        if (!est.Equals(noStyle))
                            lass.estilo = est;
                        if (be1) lass.texto = lineaASS.insertTag(lass.texto, "be1", 0);

                        string customtag = (string)dataGridView1["extra", i].Value;
                        if (customtag!=null)
                        {
                            lass.texto = lineaASS.insertTag(lass.texto, customtag, 0);
                        }

                    }
                }
            }
            mW.updateGridWithArrayList(al);
            //mW.Enabled = false;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1["be1", i].Value = true;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboDefaultStyle.SelectedIndex > 0)
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    dataGridView1["Estilo", i].Value = comboDefaultStyle.Text;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1["Estilo", i].Value = noStyle;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1["be1", i].Value = false;
            }
        }

    }
}