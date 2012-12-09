using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Configuration;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace scriptASS
{
    public partial class sbrowseW : Form
    {
        private string PreviewFile = Application.StartupPath + "\\PerrySub.PreviewStyle";

        private mainW mW;
        private ArrayList almacen = new ArrayList();
        private AviSynthScriptEnvironment env;
        private AviSynthClip avs;
        private string PreviewBackColor = "CCCCCC";

        public sbrowseW(mainW mw)
        {
            InitializeComponent();
            mW = mw;
            
            rellenaAlmacen();
        }

        private void rellenaAlmacen()
        {
            DirectoryInfo d = new DirectoryInfo(mW.stylesDir);
            foreach (FileInfo f in d.GetFiles("*.Styles"))
            {
                string s = f.Name.Substring(0,f.Name.LastIndexOf(".Styles"));
                comboCont.Items.Add(s);
            }
            
        }

        private void sbrowseW_Load(object sender, EventArgs e)
        {
            this.MaximumSize = this.Size;
            foreach (estiloV4 v in mW.v4)
                listBox1.Items.Add(v.Name);

            listBox1.SelectedValueChanged += new EventHandler(listBox1_SelectedValueChanged);
            //textNuevoEstilo.KeyPress += new KeyPressEventHandler(textNuevoEstilo_KeyPress);
            toolStripTextBox2.KeyPress += new KeyPressEventHandler(toolStripTextBox2_KeyPress);
            toolStripTextBox1.KeyPress += new KeyPressEventHandler(toolStripTextBox1_KeyPress);

            sName.KeyPress += new KeyPressEventHandler(defaultKeyPressHandler);
            sFont.KeyPress += new KeyPressEventHandler(defaultKeyPressHandler);
            sFontSize.KeyPress += new KeyPressEventHandler(defaultKeyPressHandler);
            sIzq.KeyPress += new KeyPressEventHandler(defaultKeyPressHandler);
            sDcha.KeyPress += new KeyPressEventHandler(defaultKeyPressHandler);
            sVert.KeyPress += new KeyPressEventHandler(defaultKeyPressHandler);
            sEncoding.KeyPress += new KeyPressEventHandler(defaultKeyPressHandler);
            sAngle.KeyPress += new KeyPressEventHandler(defaultKeyPressHandler);
            sSpacing.KeyPress += new KeyPressEventHandler(defaultKeyPressHandler);
            sScaleX.KeyPress += new KeyPressEventHandler(defaultKeyPressHandler);
            sScaleY.KeyPress += new KeyPressEventHandler(defaultKeyPressHandler);
            sOutline.KeyPress += new KeyPressEventHandler(defaultKeyPressHandler);
            sShadow.KeyPress += new KeyPressEventHandler(defaultKeyPressHandler);
            previewText.KeyPress += new KeyPressEventHandler(defaultKeyPressHandler);
            textPlayResY.KeyPress += new KeyPressEventHandler(defaultKeyPressHandler);

            this.Disposed += new EventHandler(sbrowseW_Disposed);

            sOutline.Point = true;
            sShadow.Point = true;
            sAngle.Point = true;
            sSpacing.Point = true;

            if (listBox1.Items.Count > 0)
                listBox1.SetSelected(0, true);

        }

        void sbrowseW_Disposed(object sender, EventArgs e)
        {
            if (File.Exists(PreviewFile)) File.Delete(PreviewFile);
        }


        void toolStripTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (Convert.ToInt32(e.KeyChar))
            {
                case 13:
                    SendKeys.Send("");
                    e.Handled = true;



                    string fname = mW.stylesDir + "\\" + toolStripTextBox1.Text + ".Styles";
                    if (!File.Exists(fname))
                    {
                        comboCont.Items.Add(toolStripTextBox1.Text);

                        //textNuevoCont.Visible = false;
                        //button10.Visible = true;

                        listBox2.Items.Clear();
                        almacen.Clear();
                        comboCont.SelectedIndex = comboCont.Items.Count - 1;
                        contextNuevoAlmacen.Hide();

                    }
                    else MessageBox.Show("El nombre de ese contenedor ya está en uso.\nIntroduce otro nombre.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    break;
                default:
                    break;

            }
        }

        /*
        void textNuevoCont_KeyPress(object sender, KeyPressEventArgs e)
        {

        }*/

        void defaultKeyPressHandler(object sender, KeyPressEventArgs e)
        {
            TextBox t = (TextBox)sender;
            if (t==sIzq || t==sDcha || t==sVert)
            {                
                if (t.Text.Length == t.MaxLength && t.SelectedText == "")
                    t.SelectionLength = 1;
            }

            switch (Convert.ToInt32(e.KeyChar))
            {
                case 13:
                    SendKeys.Send("");
                    e.Handled = true;
                    saveAndUpdate();
                    break;
                default:
                    break;
            }

        }

        void listBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1) return;

            estiloV4 v = (estiloV4)mW.v4[listBox1.SelectedIndex];

            //textBox1.Text = v.ToString();

            sName.Text = v.Name;
            sFont.Text = v.FontName;
            sFontSize.Text = v.FontSize.ToString();
            sBold.Checked = v.Bold;
            sItalic.Checked = v.Italic;
            sUnderline.Checked = v.Underline;
            sStrikeout.Checked = v.Strikeout;

            RadioButton r = (RadioButton)groupPosition.Controls[v.Alignment - 1];
            r.Checked = true;

            sIzq.Text = lineaASS.i2ns(v.MarginL, 4);
            sDcha.Text = lineaASS.i2ns(v.MarginR, 4);
            sVert.Text = lineaASS.i2ns(v.MarginV, 4);

            sOpaco.Checked = !v.Border;
            sEncoding.Text = v.Encoding.ToString();

            sOutline.Text = estiloV4.d2s(v.Outline);
            sShadow.Text = estiloV4.d2s(v.Shadow);

            sAngle.Text = estiloV4.d2s(v.Angle);
            sSpacing.Text = estiloV4.d2s(v.Spacing);

            sScaleX.Text = estiloV4.d2s(v.ScaleX);
            sScaleY.Text = estiloV4.d2s(v.ScaleY);

            string _1c = v.PrimaryColor;
            string _2c = v.SecondaryColor;
            string _3c = v.OutlineColor;
            string _4c = v.ShadowColor;
            int alpha;
            b1c.BackColor = str2color(_1c, out alpha);
            trackBar1c.Value = alpha;
            a1.Text = alpha.ToString();
            b2c.BackColor = str2color(_2c, out alpha);
            trackBar2c.Value = alpha;
            a2.Text = alpha.ToString();
            b3c.BackColor = str2color(_3c, out alpha);
            trackBar3c.Value = alpha;
            a3.Text = alpha.ToString();
            b4c.BackColor = str2color(_4c, out alpha);
            trackBar4c.Value = alpha;
            a4.Text = alpha.ToString();

        }

        private Color str2color(string s, out int a) // sacamos el alpha asi
        {
            s = s.Remove(0, 2);
            a = Int32.Parse(s.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            int b = Int32.Parse(s.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            int g = Int32.Parse(s.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            int r = Int32.Parse(s.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            Color c = new Color();
            c = Color.FromArgb(r, g, b);
            return c;
        }

        private string color2str(Color c, int alpha)
        {
            string res;
            string a = Convert.ToString(alpha,16).ToUpper();
            if (a.Length == 1) a = "0" + a;
            string r = Convert.ToString(c.R, 16).ToUpper();
            if (r.Length == 1) r = "0" + r;
            string g = Convert.ToString(c.G, 16).ToUpper();
            if (g.Length == 1) g = "0" + g;
            string b = Convert.ToString(c.B, 16).ToUpper();
            if (b.Length == 1) b = "0" + b;
            res = "&H" + a + b + g + r;
            return res;
        }

        private string color2strRGB(Color c)
        {
            string res;
            string r = Convert.ToString(c.R, 16).ToUpper();
            if (r.Length == 1) r = "0" + r;
            string g = Convert.ToString(c.G, 16).ToUpper();
            if (g.Length == 1) g = "0" + g;
            string b = Convert.ToString(c.B, 16).ToUpper();
            if (b.Length == 1) b = "0" + b;
            res = r + g + b;
            return res;
        }

        private void saveAndUpdate()
        {
            if (listBox1.SelectedIndex == -1) return;
            estiloV4 v = (estiloV4)mW.v4[listBox1.SelectedIndex];

            mW.UndoRedo.AddUndo(mW.script, "Cambio en estilo " + v.Name);

            v.Name = sName.Text;
            v.FontName = sFont.Text;

            v.FontSize = Convert.ToInt32(estiloV4.s2d(sFontSize.Text));

            v.Bold = sBold.Checked;
            v.Italic = sItalic.Checked;
            v.Underline = sUnderline.Checked;
            v.Strikeout = sStrikeout.Checked;
            v.MarginL = int.Parse(sIzq.Text);
            v.MarginR = int.Parse(sDcha.Text);
            v.MarginV = int.Parse(sVert.Text);
            v.Border = !sOpaco.Checked;
            v.Encoding = int.Parse(sEncoding.Text);
            v.Outline = estiloV4.s2d(sOutline.Text);
            v.Shadow = estiloV4.s2d(sShadow.Text);
            v.Angle = estiloV4.s2d(sAngle.Text);
            v.Spacing = estiloV4.s2d(sSpacing.Text);
            v.ScaleX = estiloV4.s2d(sScaleX.Text);
            v.ScaleY = estiloV4.s2d(sScaleY.Text);
            v.PrimaryColor = color2str(b1c.BackColor, trackBar1c.Value);
            v.SecondaryColor = color2str(b2c.BackColor, trackBar2c.Value);
            v.OutlineColor = color2str(b3c.BackColor, trackBar3c.Value);
            v.ShadowColor = color2str(b4c.BackColor, trackBar4c.Value);

            int n = groupPosition.Controls.Count;
            int res = 0;
            for (int i = 0; i < n; i++)
            {
                RadioButton r = (RadioButton)groupPosition.Controls[i];
                if (r.Checked) res = i + 1;
            }

            v.Alignment = res;

            // update
            int old = listBox1.SelectedIndex;

            listBox1.Items.Clear();
            foreach (estiloV4 v1 in mW.v4)
                listBox1.Items.Add(v1.Name);
            listBox1.SetSelected(old, true);

            showPreview();
            mW.UpdateStyleList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            /*
            FontDialog fd = new FontDialog();
            fd.ShowColor = false;
            fd.ShowEffects = true;
            fd.FontMustExist = true;
            */
            FontSelectionW fd = new FontSelectionW(sFont.Text, sFontSize.Text);
            
            if (fd.ShowDialog() == DialogResult.OK)
            {
                sFont.Text = fd.FontList.Text;
                sFontSize.Text = fd.FontSize.Value.ToString();
                fd.Dispose();
                /*
                sBold.Checked = fd.Font.Bold;
                sItalic.Checked = fd.Font.Italic;
                sUnderline.Checked = fd.Font.Underline;
                sStrikeout.Checked = fd.Font.Strikeout;
                 */
            }
        }

        private void trackBar1c_Scroll(object sender, EventArgs e)
        {
            a1.Text = trackBar1c.Value.ToString();
        }

        private void trackBar2c_Scroll(object sender, EventArgs e)
        {
            a2.Text = trackBar2c.Value.ToString();
        }

        private void trackBar3c_Scroll(object sender, EventArgs e)
        {
            a3.Text = trackBar3c.Value.ToString();
        }

        private void trackBar4c_Scroll(object sender, EventArgs e)
        {
            a4.Text = trackBar4c.Value.ToString();
        }

        private void button_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                Button b = (Button)sender;
                b.BackColor = cd.Color;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //button1.Visible = false;
            //textNuevoEstilo.Visible = true;
            //textNuevoEstilo.Focus();
            contextNuevoEstilo.Show(button1, new Point(button1.Width, 0));
            toolStripTextBox2.Focus();
            toolStripTextBox2.SelectAll();
        }

        void toolStripTextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (Convert.ToInt32(e.KeyChar))
            {
                case 13:
                    SendKeys.Send("");
                    e.Handled = true;

                    if (!listBox1.Items.Contains(toolStripTextBox2.Text))
                    {

                        mW.v4.Add(new estiloV4("Style: " + toolStripTextBox2.Text + ",Arial,20,&H00FFFFFF,&H0000FFFF,&H00000000,&H00000000,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,0"));
                        //textNuevoEstilo.Visible = false;
                        //button1.Visible = true;

                        listBox1.Items.Clear();
                        foreach (estiloV4 v1 in mW.v4)
                            listBox1.Items.Add(v1.Name);
                        listBox1.SetSelected(listBox1.Items.Count - 1, true);
                        contextNuevoEstilo.Hide();
                    }
                    else MessageBox.Show("El nombre de ese estilo ya está en uso.\nIntroduce otro nombre.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    break;
                default:
                    break;
            }

        }


        private bool isInStyles(string neim)
        {
            foreach (estiloV4 v in mW.v4)
            {
                if (v.Name.Equals(neim,StringComparison.InvariantCultureIgnoreCase)) return true;
            }
            return false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0) return;
            if (listBox1.SelectedIndex == -1) return;

            estiloV4 v = (estiloV4)mW.v4[listBox1.SelectedIndex];
            estiloV4 v2 = new estiloV4(v.ToString());

            v2.Name = "Copia de " + v2.Name;
            if (isInStyles(v2.Name))
            {
                for (int i=1; ;i++)
                    if (!isInStyles(v2.Name + " " + i))
                    {
                        v2.Name = v2.Name + " " + i;
                        break;
                    }
            }
            
            mW.v4.Add(v2);
            listBox1.Items.Clear();
            foreach (estiloV4 v1 in mW.v4)
                listBox1.Items.Add(v1.Name);
            
            listBox1.SetSelected(listBox1.Items.Count - 1, true);

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0) return;
            if (listBox1.SelectedIndex == -1) return;

            ArrayList toDel = new ArrayList();

            for (int i = 0; i <listBox1.SelectedItems.Count; i++)
            {
                int idx = listBox1.SelectedIndices[i];
                estiloV4 v = (estiloV4)mW.v4[idx];
                bool isUsed = false;

                foreach (lineaASS lass in mW.al)
                    if (lass.estilo.Equals(v.Name)) isUsed = true;

                if (isUsed)
                {
                    if (MessageBox.Show("El estilo '" + v.Name + "' está siendo usado en el script.\n¿Sigues queriendo eliminarlo de la lista?", "PerrySub", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        toDel.Add(idx);
                }
                else toDel.Add(idx);
            }

            toDel.Reverse();
            foreach (int i in toDel)
                mW.v4.RemoveAt(i);

            listBox1.Items.Clear();
            foreach (estiloV4 v1 in mW.v4)
                listBox1.Items.Add(v1.Name);
            
            if (listBox1.Items.Count>0) listBox1.SetSelected(0, true);

        }

        private void comboCont_SelectedIndexChanged(object sender, EventArgs e)
        {
            StreamReader sr;
            ComboBox c = (ComboBox)sender;
            listBox2.Items.Clear();
            almacen.Clear();

            try
            {
                string fname = mW.stylesDir + "\\" + c.Items[c.SelectedIndex].ToString() + ".Styles";
                sr = FileAccessWrapper.OpenTextFile(fname);

                string linea;
                while ((linea = sr.ReadLine()) != null)
                {
                    estiloV4 v = new estiloV4(linea);
                    listBox2.Items.Add(v.Name);
                    almacen.Add(v);
                }

                sr.Close();
            }
            catch { }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (listBox2.Items.Count == 0) return;
            if (listBox2.SelectedIndex == -1) return;

            for (int i = 0; i < listBox2.SelectedItems.Count; i++)
            {
                int idx = listBox2.SelectedIndices[i];
                estiloV4 t = (estiloV4)almacen[idx];
                if (!listBox1.Items.Contains(t.Name)) mW.v4.Add(almacen[idx]);
            }

            listBox1.Items.Clear();
            foreach (estiloV4 v1 in mW.v4)
                listBox1.Items.Add(v1.Name);
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0) return;
            if (listBox1.SelectedIndex == -1) return;
            if (comboCont.SelectedIndex == -1) return;

            for (int i = 0; i < listBox1.SelectedItems.Count; i++)
            {
                int idx = listBox1.SelectedIndices[i];
                estiloV4 t = (estiloV4)mW.v4[idx];
                if (!listBox2.Items.Contains(t.Name)) almacen.Add(mW.v4[idx]);
            }

            listBox2.Items.Clear();
            foreach (estiloV4 v1 in almacen)
                listBox2.Items.Add(v1.Name);


        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (listBox2.Items.Count == 0) return;
            if (comboCont.Text == String.Empty) return;
            string fil = comboCont.Items[comboCont.SelectedIndex].ToString();

            for (int i = 0; i < FileAccessWrapper.InvalidCharacters.Length; i++)
                fil = fil.Replace(FileAccessWrapper.InvalidCharacters[i], '_');

            string fname = mW.stylesDir + "\\" + fil + ".Styles";
            TextWriter o = new StreamWriter(fname, false, System.Text.Encoding.UTF8);

            foreach (estiloV4 v in almacen)
                o.WriteLine(v.ToString());

            o.Close();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (listBox2.Items.Count == 0) return;
            if (listBox2.SelectedIndex == -1) return;
            ArrayList toDel = new ArrayList();

            for (int i = 0; i < listBox2.SelectedItems.Count; i++)
            {
                int idx = listBox2.SelectedIndices[i];
                toDel.Add(idx);
            }

            toDel.Reverse();
            foreach (int i in toDel)
                almacen.RemoveAt(i);

            listBox2.Items.Clear();
            foreach (estiloV4 v in almacen)
                listBox2.Items.Add(v.Name);

        }

        private void button10_Click(object sender, EventArgs e)
        {
            contextNuevoAlmacen.Show(button10, new Point(button10.Width, 0));
            toolStripTextBox1.Focus();
            toolStripTextBox1.SelectAll();
        }

        private void makeTempFile()
        {
            TextWriter o = new StreamWriter(PreviewFile, false, System.Text.Encoding.UTF8);
            o.WriteLine(mW.headerMark);
            o.WriteLine("PlayResY: " + textPlayResY.Text);
            o.WriteLine();
            o.WriteLine(mW.stylesMark);
            estiloV4 e = (estiloV4)mW.v4[listBox1.SelectedIndex];
            estiloV4 e2 = new estiloV4(e.ToString());

            switch (e.Alignment)
            {
                case 4:
                case 7:
                    e2.Alignment = 1;
                    break;
                case 5:
                case 8:
                    e2.Alignment = 2;
                    break;
                case 6:
                case 9:
                    e2.Alignment = 3;
                    break;
                default:
                    e2.Alignment = e.Alignment;
                    break;
            }
            
            o.WriteLine(e2.ToString());
            o.WriteLine();
            o.WriteLine(mW.dialoguesMark);
            lineaASS l = new lineaASS();
            if (checkBox2.Checked)
                l.texto = "{\\be1}"+previewText.Text;
            else l.texto = previewText.Text;
            l.estilo = e.Name;
            l.t_final.setTiempo(50);
            o.WriteLine(l.ToString());
            o.Close();

        }

        private void showPreview()
        {
            if (listBox1.SelectedIndex == -1)
            {
                mW.errorMsg("No se puede hacer preview si no hay un estilo seleccionado.");
                return;
            }

            if (!checkBox1.Checked) return;

            try
            {
                env = new AviSynthScriptEnvironment();
                string script0rz = "blankclip(color=$"+PreviewBackColor+")";
                avs = env.ParseScript(script0rz);

                string vsf_path = AviSynthFunctions.getVSFilterPath(avs);
                if (vsf_path != null)
                    avs.AviSynthInvoke(avs.GetAVS(),0, "LoadPlugin", false, vsf_path);

                makeTempFile();
                avs.AviSynthInvoke(avs.GetAVS(), 0, "Eval", false, script0rz);
                avs.AviSynthInvoke(avs.GetAVS(),0, "TextSub", true, PreviewFile);
                Bitmap b = AviSynthFunctions.getBitmapFromFrame(avs,0,0);
                b = BitmapFunctions.Crop(b, 640, 480, 0, 376);
                stylePreviewBox.Image = b;
            }
            catch { 
                mW.errorMsg("Error cargando AviSynth.");
                checkBox1.Checked = false;
            }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                stylePreviewBox.Visible=true;
                showPreview();
            } else {
                stylePreviewBox.Visible=false;
                avs=null;
                env=null;
            }

        }

        private void button11_Click(object sender, EventArgs e)
        {
            saveAndUpdate();
            //showPreview();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                PreviewBackColor = color2strRGB(cd.Color);
                showPreview();
            }

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            showPreview();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            showPreview();
        }

        private void nuevoToolStripButton_Click(object sender, EventArgs e)
        {
            contextNuevoAlmacen.Show(toolStrip1,new Point(0,nuevoToolStripButton.Height));            
            toolStripTextBox1.Focus();
            toolStripTextBox1.SelectAll();
        }

        private void guardarToolStripButton_Click(object sender, EventArgs e)
        {
            if (listBox2.Items.Count == 0) return;
            if (comboCont.Text == String.Empty) return;
            string fil = comboCont.Items[comboCont.SelectedIndex].ToString();

            for (int i = 0; i < FileAccessWrapper.InvalidCharacters.Length; i++)
                fil = fil.Replace(FileAccessWrapper.InvalidCharacters[i], '_');

            string fname = mW.stylesDir + "\\" + fil + ".Styles";
            TextWriter o = new StreamWriter(fname, false, System.Text.Encoding.UTF8);

            foreach (estiloV4 v in almacen)
                o.WriteLine(v.ToString());

            o.Close();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (listBox2.Items.Count == 0) return;
            if (listBox2.SelectedIndex == -1) return;
            ArrayList toDel = new ArrayList();

            for (int i = 0; i < listBox2.SelectedItems.Count; i++)
            {
                int idx = listBox2.SelectedIndices[i];
                toDel.Add(idx);
            }

            toDel.Reverse();
            foreach (int i in toDel)
                almacen.RemoveAt(i);

            listBox2.Items.Clear();
            foreach (estiloV4 v in almacen)
                listBox2.Items.Add(v.Name);

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (listBox2.Items.Count == 0) return;
            if (listBox2.SelectedIndex == -1) return;

            for (int i = 0; i < listBox2.SelectedItems.Count; i++)
            {
                int idx = listBox2.SelectedIndices[i];
                estiloV4 t = (estiloV4)almacen[idx];
                if (!listBox1.Items.Contains(t.Name)) mW.v4.Add(almacen[idx]);
            }

            listBox1.Items.Clear();
            foreach (estiloV4 v1 in mW.v4)
                listBox1.Items.Add(v1.Name);

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            contextNuevoEstilo.Show(toolStrip2, new Point(0, toolStripButton2.Height));
            toolStripTextBox2.Focus();
            toolStripTextBox2.SelectAll();

        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0) return;
            if (listBox1.SelectedIndex == -1) return;

            estiloV4 v = (estiloV4)mW.v4[listBox1.SelectedIndex];
            estiloV4 v2 = new estiloV4(v.ToString());

            v2.Name = "Copia de " + v2.Name;
            if (isInStyles(v2.Name))
            {
                for (int i = 1; ; i++)
                    if (!isInStyles(v2.Name + " " + i))
                    {
                        v2.Name = v2.Name + " " + i;
                        break;
                    }
            }

            mW.v4.Add(v2);
            listBox1.Items.Clear();
            foreach (estiloV4 v1 in mW.v4)
                listBox1.Items.Add(v1.Name);

            listBox1.SetSelected(listBox1.Items.Count - 1, true);

        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0) return;
            if (listBox1.SelectedIndex == -1) return;

            ArrayList toDel = new ArrayList();

            for (int i = 0; i < listBox1.SelectedItems.Count; i++)
            {
                int idx = listBox1.SelectedIndices[i];
                estiloV4 v = (estiloV4)mW.v4[idx];
                bool isUsed = false;

                foreach (lineaASS lass in mW.al)
                    if (lass.estilo.Equals(v.Name)) isUsed = true;

                if (isUsed)
                {
                    if (MessageBox.Show("El estilo '" + v.Name + "' está siendo usado en el script.\n¿Sigues queriendo eliminarlo de la lista?", "PerrySub", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        toDel.Add(idx);
                }
                else toDel.Add(idx);
            }

            toDel.Reverse();
            foreach (int i in toDel)
                mW.v4.RemoveAt(i);

            listBox1.Items.Clear();
            foreach (estiloV4 v1 in mW.v4)
                listBox1.Items.Add(v1.Name);

            if (listBox1.Items.Count > 0) listBox1.SetSelected(0, true);

        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0) return;
            if (listBox1.SelectedIndex == -1) return;
            if (comboCont.SelectedIndex == -1) return;

            for (int i = 0; i < listBox1.SelectedItems.Count; i++)
            {
                int idx = listBox1.SelectedIndices[i];
                estiloV4 t = (estiloV4)mW.v4[idx];
                if (!listBox2.Items.Contains(t.Name)) almacen.Add(mW.v4[idx]);
            }

            listBox2.Items.Clear();
            foreach (estiloV4 v1 in almacen)
                listBox2.Items.Add(v1.Name);


        }

        private void button13_Click(object sender, EventArgs e)
        {
            FontSelectionW fsw = new FontSelectionW(sFont.Text, sFontSize.Text);
            if (fsw.ShowDialog() == DialogResult.OK)
            {
                sFont.Text = fsw.FontList.Text;
                sFontSize.Text = fsw.FontSize.Value.ToString();
            }
        }

        private void cargarEstilosOtroScript_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Archivos de subtítulos conocidos (*.ass; *.ssa; *.txt; *.srt)|*.ass; *.ssa; *.txt; *.srt";
            try
            {
                ofd.InitialDirectory = mW.getFromConfigFile("mainW_WorkDirectory");
            }
            catch
            {
                ofd.InitialDirectory = System.Environment.SpecialFolder.MyDocuments.ToString();
            }
            ofd.FilterIndex = 0;
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    SubtitleScript nuevo = new SubtitleScript(ofd.FileName);
                    ArrayList toDel = new ArrayList();

                    foreach (estiloV4 estilo in nuevo.GetStyles())
                    {
                        if (listBox1.Items.Contains(estilo.Name))
                        {
                            if (MessageBox.Show("El estilo '" + estilo.Name + "' ya existe en los estilos actuales.\n¿Deseas sobreescribirlo con el cargado desde el archivo?", mainW.appTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                int idx = listBox1.Items.IndexOf(estilo.Name);
                                if (idx != -1)
                                    toDel.Add(idx);
                                mW.v4.Add(new estiloV4(estilo.ToString()));
                            }                                                           
                        }
                        else
                        {
                            mW.v4.Add(new estiloV4(estilo.ToString()));
                        }
                    }

                    toDel.Reverse();
                    foreach (int i in toDel)
                        mW.v4.RemoveAt(i);


                    listBox1.Items.Clear();
                    foreach (estiloV4 v1 in mW.v4)
                        listBox1.Items.Add(v1.Name);
                }
                catch
                {
                    mW.errorMsg("Error cargando script.");
                }
            }
        }

        private void button13_Click_1(object sender, EventArgs e)
        {
            saveAndUpdate();
        }

    }
}