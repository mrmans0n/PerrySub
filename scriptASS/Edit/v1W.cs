using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Xml;
using System.Collections;
using System.Text.RegularExpressions;

namespace scriptASS
{
    public partial class v1W : Form
    {
        private string v1efectos = @"http://www.aunder.org/basedeseries/getEffects.php";
        mainW mW;
        ArrayList efv1;
        ArrayList Ks;
        ArrayList persList;

        public v1W(mainW mw)
        {
            InitializeComponent();
            mW = mw;
            Ks = new ArrayList();
            inv.CheckedChanged += new EventHandler(GlobalCheckedChanged);
            ebase.CheckedChanged += new EventHandler(GlobalCheckedChanged);
            ef1.CheckedChanged += new EventHandler(GlobalCheckedChanged);
            ef2.CheckedChanged += new EventHandler(GlobalCheckedChanged);
            ef3.CheckedChanged += new EventHandler(GlobalCheckedChanged);
            ef4.CheckedChanged += new EventHandler(GlobalCheckedChanged);
            ef5.CheckedChanged += new EventHandler(GlobalCheckedChanged);

            Efecto1_Perc.TextChanged += new EventHandler(PercChanged);
            Efecto2_Perc.TextChanged += new EventHandler(PercChanged);
            Efecto3_Perc.TextChanged += new EventHandler(PercChanged);
            Efecto4_Perc.TextChanged += new EventHandler(PercChanged);
            Efecto5_Perc.TextChanged += new EventHandler(PercChanged);

            Efecto1.TextChanged += new EventHandler(EffChanged);
            Efecto2.TextChanged += new EventHandler(EffChanged);
            Efecto3.TextChanged += new EventHandler(EffChanged);
            Efecto4.TextChanged += new EventHandler(EffChanged);
            Efecto5.TextChanged += new EventHandler(EffChanged);
            Invariante.TextChanged += new EventHandler(EffChanged);
            EfBase.TextChanged += new EventHandler(EffChanged);
            TextoAñadir.TextChanged += new EventHandler(EffChanged);

            Efecto1_TEI.AllowNegative = true;
            Efecto1_TEF.AllowNegative = true;
            Efecto2_TEI.AllowNegative = true;
            Efecto2_TEF.AllowNegative = true;
            Efecto3_TEI.AllowNegative = true;
            Efecto3_TEF.AllowNegative = true;
            Efecto4_TEI.AllowNegative = true;
            Efecto4_TEF.AllowNegative = true;
            Efecto5_TEI.AllowNegative = true;
            Efecto5_TEF.AllowNegative = true;

            dbOnline.SelectedIndexChanged += new EventHandler(dbOnline_SelectedIndexChanged);
            dbLocal.SelectedIndexChanged += new EventHandler(dbLocal_SelectedIndexChanged);

        }

        void EffChanged(object sender, EventArgs e)
        {
            TextBox Efecto = (TextBox)sender;
            if (Efecto.Text.Equals("0")) Efecto.Text = "";
            ManageCheckBoxes();
        }

        void dbLocal_SelectedIndexChanged(object sender, EventArgs e)
        {
            string fil = mW.v1Dir + "\\" + dbLocal.Text + ".v1";
            try
            {
                StreamReader sr = FileAccessWrapper.OpenTextFile(fil);
                EfectosV1 ef = ParseEfectosV1(sr);
                sr.Close();

                FillDataFromEffect(ef);
            }
            catch { } 

        }

        void dbOnline_SelectedIndexChanged(object sender, EventArgs e)
        {
            EfectosV1 ouref = null;

            foreach (EfectosV1 ef in efv1)
            {
                if (ef.nombre.Equals(dbOnline.Text))
                    ouref = ef;
            }

            if (ouref != null)
                FillDataFromEffect(ouref);

            ManageCheckBoxes();
        }

        private void ManageCheckBoxes()
        {
            inv.Checked = !(Invariante.Text.Equals("") || Invariante.Text.Equals("0"));
            ebase.Checked = !(EfBase.Text.Equals("") || EfBase.Text.Equals("0"));

            ef1.Checked = !(Efecto1.Text.Equals("") || Efecto1.Text.Equals("0"));
            ef2.Checked = !(Efecto2.Text.Equals("") || Efecto2.Text.Equals("0"));
            ef3.Checked = !(Efecto3.Text.Equals("") || Efecto3.Text.Equals("0"));
            ef4.Checked = !(Efecto4.Text.Equals("") || Efecto4.Text.Equals("0"));
            ef5.Checked = !(Efecto5.Text.Equals("") || Efecto5.Text.Equals("0"));
        }

        private void FillDataFromEffect(EfectosV1 ef)
        {

            NombreEfecto.Text = ef.nombre;

            Invariante.Text = ef.invariante;
            EfBase.Text = ef.ebase;

            Efecto1.Text = ef.efecto1;
            Efecto1_Perc.Text = Convert.ToString(ef.tefecto1);
            Efecto1_TEI.Text = Convert.ToString(ef.tade1);
            Efecto1_TEF.Text = Convert.ToString(ef.tatr1);

            Efecto2.Text = ef.efecto2;
            Efecto2_Perc.Text = Convert.ToString(ef.tefecto2);
            Efecto2_TEI.Text = Convert.ToString(ef.tade2);
            Efecto2_TEF.Text = Convert.ToString(ef.tatr2);

            Efecto3.Text = ef.efecto3;
            Efecto3_Perc.Text = Convert.ToString(ef.tefecto3);
            Efecto3_TEI.Text = Convert.ToString(ef.tade3);
            Efecto3_TEF.Text = Convert.ToString(ef.tatr3);

            Efecto4.Text = ef.efecto4;
            Efecto4_Perc.Text = Convert.ToString(ef.tefecto4);
            Efecto4_TEI.Text = Convert.ToString(ef.tade4);
            Efecto4_TEF.Text = Convert.ToString(ef.tatr4);

            Efecto5.Text = ef.efecto5;
            Efecto5_Perc.Text = Convert.ToString(ef.tefecto5);
            Efecto5_TEI.Text = Convert.ToString(ef.tade5);
            Efecto5_TEF.Text = Convert.ToString(ef.tatr5);

            TextoAñadir.Text = ef.textra;

        }

        void PercChanged(object sender, EventArgs e)
        {
            NumericTextBox t = (NumericTextBox)sender;
            if (t.Text.Equals("")) t.Text = "0";

            try
            {
                double e1 = double.Parse(Efecto1_Perc.Text);
                double e2 = double.Parse(Efecto2_Perc.Text);
                double e3 = double.Parse(Efecto3_Perc.Text);
                double e4 = double.Parse(Efecto4_Perc.Text);
                double e5 = double.Parse(Efecto5_Perc.Text);

                double sum = e1 + e2 + e3 + e4 + e5;

                TotalPerc.Text = sum.ToString();
            }
            catch { TotalPerc.Text = "ERROR"; }
        }

        void GlobalCheckedChanged(object sender, EventArgs e)
        {
            Invariante.Enabled = inv.Checked;
            EfBase.Enabled = ebase.Checked;

            Efecto1.Enabled = ef1.Checked;
            Efecto1_Perc.Enabled = ef1.Checked;
            Efecto1_TEI.Enabled = ef1.Checked;
            Efecto1_TEF.Enabled = ef1.Checked;

            Efecto2.Enabled = ef2.Checked;
            Efecto2_Perc.Enabled = ef2.Checked;
            Efecto2_TEI.Enabled = ef2.Checked;
            Efecto2_TEF.Enabled = ef2.Checked;

            Efecto3.Enabled = ef3.Checked;
            Efecto3_Perc.Enabled = ef3.Checked;
            Efecto3_TEI.Enabled = ef3.Checked;
            Efecto3_TEF.Enabled = ef3.Checked;

            Efecto4.Enabled = ef4.Checked;
            Efecto4_Perc.Enabled = ef4.Checked;
            Efecto4_TEI.Enabled = ef4.Checked;
            Efecto4_TEF.Enabled = ef4.Checked;

            Efecto5.Enabled = ef5.Checked;
            Efecto5_Perc.Enabled = ef5.Checked;
            Efecto5_TEI.Enabled = ef5.Checked;
            Efecto5_TEF.Enabled = ef5.Checked;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }


        private string substringindexof(string s)
        {
            String ret=s.Substring(s.IndexOf(':') + 1);
            double a;
            int b;
            if (ret.Contains(".") && !ret.Contains("\\"))
            {
                a = Convert.ToDouble(ret);
                b = Convert.ToInt32(a * 100);
                ret = Convert.ToString(b);
            }
            if (ret==String.Empty)
                ret="0";
            return ret;
        }

        private EfectosV1 ParseEfectosV1(StreamReader sr)
        {
            EfectosV1 v = new EfectosV1();
            v.nombre = substringindexof(sr.ReadLine());
            v.invariante = substringindexof(sr.ReadLine());
            v.ebase = substringindexof(sr.ReadLine());
            v.textra = substringindexof(sr.ReadLine());

            v.efecto1 = substringindexof(sr.ReadLine());
            v.tefecto1 = Convert.ToInt32(substringindexof(sr.ReadLine()));
            v.tade1 = Convert.ToInt32(substringindexof(sr.ReadLine()));
            v.tatr1 = Convert.ToInt32(substringindexof(sr.ReadLine()));

            v.efecto2 = substringindexof(sr.ReadLine());
            v.tefecto2 = Convert.ToInt32(substringindexof(sr.ReadLine()));
            v.tade2 = Convert.ToInt32(substringindexof(sr.ReadLine()));
            v.tatr2 = Convert.ToInt32(substringindexof(sr.ReadLine()));

            v.efecto3 = substringindexof(sr.ReadLine());
            v.tefecto3 = Convert.ToInt32(substringindexof(sr.ReadLine()));
            v.tade3 = Convert.ToInt32(substringindexof(sr.ReadLine()));
            v.tatr3 = Convert.ToInt32(substringindexof(sr.ReadLine()));

            v.efecto4 = substringindexof(sr.ReadLine());
            v.tefecto4 = Convert.ToInt32(substringindexof(sr.ReadLine()));
            v.tade4 = Convert.ToInt32(substringindexof(sr.ReadLine()));
            v.tatr4 = Convert.ToInt32(substringindexof(sr.ReadLine()));

            v.efecto5 = substringindexof(sr.ReadLine());
            v.tefecto5 = Convert.ToInt32(substringindexof(sr.ReadLine()));
            v.tade5 = Convert.ToInt32(substringindexof(sr.ReadLine()));
            v.tatr5 = Convert.ToInt32(substringindexof(sr.ReadLine()));

            return v;
        }

        private void v1W_Load(object sender, EventArgs e)
        {
            try
            {
                WebClient client = new WebClient();
                StreamReader sr = new StreamReader(client.OpenRead(v1efectos));

                string linea = "";
                efv1 = new ArrayList();
                while ((linea = sr.ReadLine()) != null)
                {
                    EfectosV1 v = new EfectosV1();
                    v.nombre = substringindexof(linea);
                    v.invariante = substringindexof(sr.ReadLine());
                    v.ebase = substringindexof(sr.ReadLine());
                    v.textra = substringindexof(sr.ReadLine());

                    v.efecto1 = substringindexof(sr.ReadLine());
                    v.tefecto1 = Convert.ToInt32(substringindexof(sr.ReadLine()));
                    v.tade1 = Convert.ToInt32(substringindexof(sr.ReadLine()));
                    v.tatr1 = Convert.ToInt32(substringindexof(sr.ReadLine()));

                    v.efecto2 = substringindexof(sr.ReadLine());
                    v.tefecto2 = Convert.ToInt32(substringindexof(sr.ReadLine()));
                    v.tade2 = Convert.ToInt32(substringindexof(sr.ReadLine()));
                    v.tatr2 = Convert.ToInt32(substringindexof(sr.ReadLine()));

                    v.efecto3 = substringindexof(sr.ReadLine());
                    v.tefecto3 = Convert.ToInt32(substringindexof(sr.ReadLine()));
                    v.tade3 = Convert.ToInt32(substringindexof(sr.ReadLine()));
                    v.tatr3 = Convert.ToInt32(substringindexof(sr.ReadLine()));

                    v.efecto4 = substringindexof(sr.ReadLine());
                    v.tefecto4 = Convert.ToInt32(substringindexof(sr.ReadLine()));
                    v.tade4 = Convert.ToInt32(substringindexof(sr.ReadLine()));
                    v.tatr4 = Convert.ToInt32(substringindexof(sr.ReadLine()));

                    v.efecto5 = substringindexof(sr.ReadLine());
                    v.tefecto5 = Convert.ToInt32(substringindexof(sr.ReadLine()));
                    v.tade5 = Convert.ToInt32(substringindexof(sr.ReadLine()));
                    v.tatr5 = Convert.ToInt32(substringindexof(sr.ReadLine()));
                    
                    efv1.Add(v);
                    dbOnline.Items.Add(v.nombre);
                }                

                sr.Close();
            }
            catch { mW.errorMsg("Error accediendo a la base de datos on-line de efectos de la v1.\nSólo se podrá actuar localmente en esta sesión.");}

            FillWithLocalEffects();

            // rellenar datagrid estilos

            persList = new ArrayList();

            foreach (lineaASS lass in mW.al)
                if (!persList.Contains(lass.personaje))
                    persList.Add(lass.personaje);


            dataGridView1.RowCount = persList.Count;
            for (int i = 0; i < persList.Count; i++)
            {
                dataGridView1["Estilos", i].Value = persList[i];
                dataGridView1["Check", i].Value = true;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {

            string newf = mW.v1Dir+"\\"+NombreEfecto.Text;
            string newfn = newf;
            int i = 0 ;
            while (true)
            {
                if (File.Exists(newfn + ".v1"))
                    newfn = newf + (i++);
                else break;
            }

            string newfext = newfn + ".v1";
            TextWriter o = new StreamWriter(newfext, false, System.Text.Encoding.UTF8);
            o.WriteLine("Nombre:" + NombreEfecto.Text);
            o.WriteLine("Invariante:" + Invariante.Text);
            o.WriteLine("Ebase:" + EfBase.Text);
            o.WriteLine("Textra:" + TextoAñadir.Text);

            o.WriteLine("Efecto1:" + Efecto1.Text);
            o.WriteLine("Tefecto1:" + Efecto1_Perc.Text);
            o.WriteLine("Tade1:" + Efecto1_TEI.Text);
            o.WriteLine("Tatr1:" + Efecto1_TEF.Text);
            
            o.WriteLine("Efecto2:" + Efecto2.Text);
            o.WriteLine("Tefecto2:" + Efecto2_Perc.Text);
            o.WriteLine("Tade2:" + Efecto2_TEI.Text);
            o.WriteLine("Tatr2:" + Efecto2_TEF.Text);

            o.WriteLine("Efecto3:" + Efecto3.Text);
            o.WriteLine("Tefecto3:" + Efecto3_Perc.Text);
            o.WriteLine("Tade3:" + Efecto3_TEI.Text);
            o.WriteLine("Tatr3:" + Efecto3_TEF.Text);

            o.WriteLine("Efecto4:" + Efecto4.Text);
            o.WriteLine("Tefecto4:" + Efecto4_Perc.Text);
            o.WriteLine("Tade4:" + Efecto4_TEI.Text);
            o.WriteLine("Tatr4:" + Efecto4_TEF.Text);

            o.WriteLine("Efecto5:" + Efecto5.Text);
            o.WriteLine("Tefecto5:" + Efecto5_Perc.Text);
            o.WriteLine("Tade5:" + Efecto5_TEI.Text);
            o.WriteLine("Tatr5:" + Efecto5_TEF.Text);
            o.Close();
        }


        private void FillWithLocalEffects()
        {
            dbLocal.Items.Clear();
            DirectoryInfo d = new DirectoryInfo(mW.v1Dir);
            foreach (FileInfo f in d.GetFiles("*.v1"))
            {
                string s = f.Name.Substring(0, f.Name.LastIndexOf(".v1"));
                dbLocal.Items.Add(s);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {            
            FillWithLocalEffects();
        }

        private int LineaASS_to_Karaoke_K(lineaASS lass, int i)
        {
            Ks.Clear();
            string s = lass.texto;
            Regex r = new Regex(@"\{\\[kK]f?\d+\}");
            MatchCollection mc = r.Matches(s);

            for (int x = 0; x < mc.Count; x++)
            {
                Match m = mc[x];
                int fin_actual = m.Index + m.Length;
                string texto = "";

                // obtener texto :)
                if (x < mc.Count - 1)
                {
                    Match siguiente = mc[x + 1];
                    int principio_siguiente = siguiente.Index;
                    texto = s.Substring(fin_actual, principio_siguiente - fin_actual);
                }
                else
                    texto = s.Substring(fin_actual);


                // estilo K
                Regex styleh = new Regex("kf|K|k");
                Match sss = styleh.Match(m.ToString());
                string estilok = sss.ToString();

                // nº milisegundos
                Regex milis = new Regex(@"\d+");
                Match mmm = milis.Match(m.ToString());
                int milisec;
                if (mmm.ToString()=="")
                    milisec = 0;
                else 
                    milisec = int.Parse(mmm.ToString());

                Ks.Add(new Karaoke_K(i, estilok, milisec, texto));
            }

            return Ks.Count;
        }

        private bool GetCheckedState(string pers)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1["Estilos", i].Value.Equals(pers))
                    return Convert.ToBoolean(dataGridView1["Check", i].Value);
            }
            return false;
        }

        private void button3_Click(object sender, EventArgs e)
        {

            mW.UndoRedo.AddUndo(mW.script, "Efecto de Karaoke");

            EfectosV1 karaoke = new EfectosV1();

            karaoke.ebase = EfBase.Text;
            karaoke.efecto1 = Efecto1.Text;
            karaoke.efecto2 = Efecto2.Text;
            karaoke.efecto3 = Efecto3.Text;
            karaoke.efecto4 = Efecto4.Text;
            karaoke.efecto5 = Efecto5.Text;
            karaoke.invariante = Invariante.Text;
            karaoke.nombre = NombreEfecto.Text;

            karaoke.tade1 = Convert.ToInt32(Efecto1_TEI.Text);
            karaoke.tade2 = Convert.ToInt32(Efecto2_TEI.Text);
            karaoke.tade3 = Convert.ToInt32(Efecto3_TEI.Text);
            karaoke.tade4 = Convert.ToInt32(Efecto4_TEI.Text);
            karaoke.tade5 = Convert.ToInt32(Efecto5_TEI.Text);

            karaoke.tatr1 = Convert.ToInt32(Efecto1_TEF.Text);
            karaoke.tatr2 = Convert.ToInt32(Efecto2_TEF.Text);
            karaoke.tatr3 = Convert.ToInt32(Efecto3_TEF.Text);
            karaoke.tatr4 = Convert.ToInt32(Efecto4_TEF.Text);
            karaoke.tatr5 = Convert.ToInt32(Efecto5_TEF.Text);

            karaoke.tefecto1 = Convert.ToInt32(Efecto1_Perc.Text);
            karaoke.tefecto2 = Convert.ToInt32(Efecto2_Perc.Text);
            karaoke.tefecto3 = Convert.ToInt32(Efecto3_Perc.Text);
            karaoke.tefecto4 = Convert.ToInt32(Efecto4_Perc.Text);
            karaoke.tefecto5 = Convert.ToInt32(Efecto5_Perc.Text);


            for (int i = 0; i < mW.al.Count; i++)
            {
                lineaASS linea = (lineaASS)mW.al[i];

                if (GetCheckedState(linea.personaje))
                {

                    String TextoLinea = "";
                    //Creamos Ks
                    int len = LineaASS_to_Karaoke_K(linea, i); //Que gustirrining el copy paste.

                    for (int j = 0; j < len; j++)
                    {
                        //aplicamos
                        TextoLinea += karaoke.AplicaASilaba((Karaoke_K)Ks[j]);

                    }
                    ((lineaASS)mW.al[i]).texto = "{"+karaoke.ebase+"}"+TextoLinea;
                    if (karaoke.tade1 < 0)
                    {
                        ((lineaASS)mW.al[i]).t_inicial.sumaTiempo(Convert.ToDouble(karaoke.tade1)/1000);
                    }
                    if (karaoke.tatr5 > 0)
                    {
                        ((lineaASS)mW.al[i]).t_final.sumaTiempo(Convert.ToDouble(karaoke.tatr5)/1000);
                    }
                }
            }

            mW.updateGridWithArrayList(mW.al);
            mW.refreshGrid();
        }

    }
}