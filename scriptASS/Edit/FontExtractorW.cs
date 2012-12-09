using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Drawing.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;

namespace scriptASS
{
    public partial class FontExtractorW : Form
    {
        mainW principal;
        ArrayList FontList;
        FontUtil FontListEx;

        public FontExtractorW(mainW main)
        {
            InitializeComponent();
            principal = main;

            try
            {
                textBox1.Text = principal.getFromConfigFile("mainW_WorkDirectory");
            }
            catch
            {
                textBox1.Text = Application.StartupPath;
            }

        }

        private void InicializarListaFuentes()
        {
            NuevaLineaLog("Buscando lista de fuentes instaladas en el sistema...");

            FontList = new ArrayList();
            FontListEx = new FontUtil();
            

            foreach (FontFamily f in FontFamily.Families)
            {
                FontList.Add(f.Name);
            }
            
            NuevaLineaLog("FontListEx = " + FontListEx.FontMap.Count);
            NuevaLineaLog("FontList = " + FontList.Count);
            
            NuevaLineaLog("Búsqueda finalizada.");
        }

        delegate void SetTextCallback(string str);

        private void NuevaLineaLog(string linea)
        {
            NuevaLineaLogNoCRLF(linea + "\n");
        }

        private void NuevaLineaLogNoCRLF(string linea)
        {
            if (richTextBox1.InvokeRequired)
            {
                SetTextCallback callback = new SetTextCallback(NuevaLineaLogNoCRLF);
                this.Invoke(callback, new object[] { linea });
            }
            else
            {
                richTextBox1.AppendText(linea.Replace("\t", "  "));
            }
        }

        private void BuscarFuentesEnScript()
        {
            NuevaLineaLog("Buscando fuentes en el script.");
            // 1 - obtener las de los estilos
            ArrayList GotFonts = new ArrayList();
            NuevaLineaLog("\tBuscando en estilos...");
            foreach (estiloV4 estilo in principal.script.GetStyles())
            {
                if (!GotFonts.Contains(estilo.FontName))
                {
                    GotFonts.Add(estilo.FontName);
                    NuevaLineaLogNoCRLF("\t\tEncontrada fuente " + estilo.FontName + ". Buscándola en el sistema... ");
                    if (FontList.Contains(estilo.FontName) || FontListEx.FontList.Contains(estilo.FontName))
                    {
                        NuevaLineaLog("Encontrada.");
                        RealizarAccionFuente(estilo.FontName);
                    }
                    else NuevaLineaLog("No se ha encontrado.");
                }
            }

            /*
            if (principal.script.HasAttachments)
            {
                Attachments adjuntos = principal.script.GetAttachments();
                if (adjuntos.GetFontsAttachmentList().Count > 0)
                {
                    NuevaLineaLog("\tBuscando en adjuntos...");
                    foreach (attachmentASS adjunto in adjuntos.GetFontsAttachmentList())
                    {
                        NuevaLineaLog("\t\tEncontrada fuente " + adjunto.FileName + ".");
                        NuevaLineaLogNoCRLF("\t\t\tBuscándola en el sistema... ");
                        if (FontList.Contains(estilo.FontName))
                        {
                            NuevaLineaLog("Encontrada.");
                        }
                        else NuevaLineaLog("No se ha encontrado.");
                    }
                }
                else NuevaLineaLog("\tNo hay fuentes en los archivos adjuntos.");
            }
            else NuevaLineaLog("\tNo hay archivos adjuntos.");
            */

            // 3 - obtener las de los \fn

            NuevaLineaLog("\tBuscando en las líneas...");
            Regex r = new Regex(@"\{[^\{\\\r\n\f]*(?:\\.[^\}\r\n\f]*)*\}", RegexOptions.IgnoreCase); // regex tags
            Regex r2 = new Regex(@"\\[fF][nN]([^\\])+"); // separar tags entre si


            foreach (lineaASS linea in principal.script.GetLineArrayList().GetFullArrayList())
            {
                MatchCollection tags = r.Matches(linea.texto);
                foreach (Match m in tags)
                {
                    string entrellaves = linea.texto.Substring(m.Index + 1, m.Length - 2).Trim();
                    if (entrellaves.StartsWith(@"\")) // es tag, no comment
                    {
                        MatchCollection tags_separados = r2.Matches(entrellaves);
                        foreach (Match tag in tags_separados)
                        {
                            if (tag.Value.StartsWith(@"\fn", StringComparison.InvariantCultureIgnoreCase))
                            {
                                string linfontname = tag.Value.Substring(3);
                                GotFonts.Add(linfontname);
                                NuevaLineaLogNoCRLF("\t\tEncontrada fuente " + linfontname + ". Buscándola en el sistema... ");
                                if (FontList.Contains(linfontname) || FontListEx.FontList.Contains(linfontname))
                                {
                                    NuevaLineaLog("Encontrada.");
                                    RealizarAccionFuente(linfontname);
                                }
                                else NuevaLineaLog("No se ha encontrado.");

                            }
                        }
                    }
                }
            }
            NuevaLineaLog("Búsqueda finalizada.");
        }

        private void RealizarAccionFuente(string fontname)
        {
            NuevaLineaLogNoCRLF("\t\t\tIntentando extraer fuente... ");
            try
            {
                string path = FontListEx.FontMap[fontname];
                string nombre = path.Substring(path.LastIndexOf('\\') + 1);

                if (chkCopy.Checked)
                {
                    File.Copy(path, textBox1.Text+"\\"+nombre, true);
                    NuevaLineaLog("Archivo copiado.");
                }
                else if (chkAdd.Checked)
                {
                    bool isthere = false;
                    foreach(attachmentASS adjuntos in principal.script.GetAttachments().GetFontsAttachmentList())
                    {
                        if (adjuntos.FileName==nombre)
                            isthere= true;
                    }

                    if (isthere)
                        NuevaLineaLog("Ya estaba incluído como adjunto.");
                    else
                    {
                        principal.script.InsertAttachment(path);
                        NuevaLineaLog("Archivo añadido a adjuntos.");
                    }
                }

            }
            catch (Exception ex)
            {
                NuevaLineaLog("Error.");
            }
            
        }

        Thread WorkerThread;

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Enabled = true;

            ComenzarEnabled(false);
            SpinnerEnabled(true);

            richTextBox1.SelectAll();
            richTextBox1.Font = new Font(richTextBox1.Font, FontStyle.Regular);
            richTextBox1.Select(0, 0);

            richTextBox1.Clear();

            WorkerThread = new Thread(new ThreadStart(T_Work));
            WorkerThread.Start();

        }

        void T_Work()
        {
            InicializarListaFuentes();
            NuevaLineaLog("-----");
            BuscarFuentesEnScript();
            NuevaLineaLog("-----");

            ComenzarEnabled(true);
            SpinnerEnabled(false);
            //pictureBox1.Visible = false;
            //button1.Enabled = true;
        }

        delegate void SetBoolCallback(bool b);

        private void ComenzarEnabled(bool b)
        {
            if (button1.InvokeRequired)
            {
                SetBoolCallback callback = new SetBoolCallback(ComenzarEnabled);
                this.Invoke(callback, new object[] { b });
            }
            else
                button1.Enabled = b;
        }

        private void SpinnerEnabled(bool b)
        {
            if (pictureBox1.InvokeRequired)
            {
                SetBoolCallback callback = new SetBoolCallback(SpinnerEnabled);
                this.Invoke(callback, new object[] { b });
            }
            else
                pictureBox1.Visible = b;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = true;
            fbd.SelectedPath = textBox1.Text;

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = fbd.SelectedPath;
            }
        }

    }
}
