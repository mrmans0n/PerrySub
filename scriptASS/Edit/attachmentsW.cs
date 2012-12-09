using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Text;
using System.IO;
using System.Collections;

namespace scriptASS
{
    public partial class attachmentsW : Form
    {
        mainW mw;
        public attachmentsW(mainW m)
        {
            InitializeComponent();
            mw = m;

            listBox1.SelectedIndexChanged += new EventHandler(SelectedIndexChanged);
            listBox2.SelectedIndexChanged += new EventHandler(SelectedIndexChanged);
        }

        void SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                infoFont.Text = "Tamaño: Ø";
                button5.Enabled = button3.Enabled = previewFont.Visible = false;
            }
            else
            {
                attachmentASS a = mw.script.GetAttachments().GetAttachment(listBox1.SelectedIndex, AttachmentType.Font);
                infoFont.Text = "Tamaño: " + ((a.DecodedDataSize) / 1024) + "KB (encoded " + ((a.EncodedDataSize) / 1024) + "KB)";
                button5.Enabled = button3.Enabled = previewFont.Visible = true;

                try
                {
                    PrivateFontCollection pfc = new PrivateFontCollection();
                    string newname = Application.StartupPath + "\\tmp_" + a.FileName;
                    
                    byte[] fontdata = a.Decode();
                    unsafe
                    {
                        fixed (byte* pFontData = fontdata)
                        {
                            pfc.AddMemoryFont((System.IntPtr)pFontData, fontdata.Length); // pq salta excepcion de file not found ?!?!!? sin sentido.com
                        }
                    }
                    
                    Font f = new Font(pfc.Families[0], 25, FontStyle.Regular);
                    StringFormat sf = new StringFormat(StringFormat.GenericTypographic);
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;

                    Bitmap b = new Bitmap(previewFont.Width, previewFont.Height);

                    using (Graphics g = Graphics.FromImage((Image)b))
                    {
                        g.DrawString("abcDEF123", f, new SolidBrush(Color.Black), new Rectangle(0, 0, previewFont.Width, previewFont.Height), sf);
                    }

                    previewFont.Image = b;

                    File.Delete(newname);
                }
                catch { previewFont.Visible = false; }

            }

            if (listBox2.SelectedIndex == -1)
            {
                infoImg.Text = "Tamaño: Ø";
                button1.Enabled = button6.Enabled = previewImage.Visible = false;
            }
            else
            {
                attachmentASS a = mw.script.GetAttachments().GetAttachment(listBox2.SelectedIndex, AttachmentType.Graphic);
                infoImg.Text = "Tamaño: " + ((a.DecodedDataSize) / 1024) + "KB (encoded " + ((a.EncodedDataSize) / 1024) + "KB)";
                button1.Enabled = button6.Enabled = previewImage.Visible = true;

                if (!a.FileName.EndsWith("ico"))
                { // no le gustan los iconos a esto

                    string newname = Application.StartupPath + "\\tmp_" + a.FileName;
                    
                    if (!File.Exists(newname))
                        mw.script.ExtractAttachment(listBox2.SelectedIndex, AttachmentType.Graphic, newname);

                    previewImage.Load(newname);

                    double ratio1 = (double)previewImage.Image.Width / (double)previewImage.Image.Height;
                    double ratio2 = (double)previewImage.Image.Height / (double)previewImage.Image.Width;

                    int b_height = 115;
                    int b_width = 215;
                    b_width = Math.Min((int)Math.Round(ratio1 * b_height), b_width);
                    b_height = Math.Min((int)Math.Round(ratio2 * b_width), b_height);

                    previewImage.Width = b_width;
                    previewImage.Height = b_height;
                    File.Delete(newname);
                }
            }

        }

        private void UpdateListViews()
        {
            listBox1.DataSource = null;
            listBox2.DataSource = null;

            listBox1.DataSource = mw.script.GetAttachments().GetFontsAttachmentList();
            listBox1.DisplayMember = "FileName";

            listBox2.DataSource = mw.script.GetAttachments().GetGraphicsAttachmentList();
            listBox2.DisplayMember = "FileName";

            mw.updateGridWithArrayList(mw.al);

        }

        private void attachmentsW_Load(object sender, EventArgs e)
        {
            UpdateListViews();            
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Fuentes (*.ttf)|*.ttf";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                mw.UndoRedo.AddUndo(mw.script, "Añadir fuente " + ofd.FileName);
                mw.script.InsertAttachment(ofd.FileName);                
            }
            UpdateListViews();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Archivos de Imágenes (*.bmp; *.jpg; *.ico; *.wmf; *.gif)|*.bmp; *.jpg; *.ico; *.wmf; *.gif";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                mw.UndoRedo.AddUndo(mw.script, "Añadir imagen " + ofd.FileName);
                mw.script.InsertAttachment(ofd.FileName);                
            }
            UpdateListViews();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1) return;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = mw.script.GetAttachments().GetFileName(listBox1.SelectedIndex, AttachmentType.Font);
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                mw.script.ExtractAttachment(listBox1.SelectedIndex, AttachmentType.Font, sfd.FileName);
            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex == -1) return;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = mw.script.GetAttachments().GetFileName(listBox2.SelectedIndex, AttachmentType.Graphic);
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                mw.script.ExtractAttachment(listBox2.SelectedIndex, AttachmentType.Graphic, sfd.FileName);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1) return;

            mw.script.GetAttachments().Delete(listBox1.SelectedIndex,AttachmentType.Font);
            UpdateListViews();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex == -1) return;
            mw.script.GetAttachments().Delete(listBox2.SelectedIndex, AttachmentType.Graphic);
            UpdateListViews();
        }


    }
}