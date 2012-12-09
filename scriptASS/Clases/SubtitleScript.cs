using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;

namespace scriptASS
{
    public class SubtitleScript : ICloneable
    {
        protected headerASS Header;
        protected ArrayList Styles;
        protected LineArrayList Lines;
        protected Attachments Attachments;

        private string name;

        public string FileName
        {
            get { return name; }
            set { name = value; }
        }

        public int HeaderCount
        {
            get { return Header.GetHeaderList().Count; }
        }

        public int StyleCount
        {
            get { return Styles.Count; }
        }

        public int LineCount
        {
            get { return Lines.Count; }
        }

        public bool HasAttachments
        {
            get { return Attachments.HasAttachments; }
        }

        public int AttachmentCount
        {
            get { return Attachments.GetFontsAttachmentList().Count + Attachments.GetGraphicsAttachmentList().Count; }
        }

        public bool IsMorbid
        {
            get { return (Lines.Count > LineArrayList.LineArrayListMax); }
        }

        string headerMark = "[Script Info]";
        
        string stylesMark =
            "[V4+ Styles]\r\n" +
            "Format: Name, Fontname, Fontsize, PrimaryColour, SecondaryColour, OutlineColour, BackColour, Bold, Italic, Underline, StrikeOut, ScaleX, ScaleY, Spacing, Angle, BorderStyle, Outline, Shadow, Alignment, MarginL, MarginR, MarginV, Encoding";

        string dialoguesMark =
            "[Events]\r\n" +
            "Format: Layer, Start, End, Style, Name, MarginL, MarginR, MarginV, Effect, Text";

        public SubtitleScript()
        {
            InitVariables();
        }

        public SubtitleScript(string filename) 
        {
            InitVariables();
            LoadFromFile(filename);
        }

        private void InitVariables()
        {
            Header = new headerASS();
            Styles = new ArrayList();
            Lines = new LineArrayList();
            Attachments = new Attachments();

            name = "Nuevo Archivo de PerrySub.ass";
        }

        private void ResetScript()
        {
            Header.GetHeaderList().Clear();
            Styles.Clear();
            Lines.Clear();
            Attachments.Reset();
        }

        public ArrayList GetStyles()
        {
            return Styles;
        }

        public ArrayList GetLines()
        {
            return Lines.GetLines();
        }

        public LineArrayList GetLineArrayList()
        {
            return Lines;
        }

        public ArrayList GetHeaders()
        {
            return Header.GetHeaderList();
        }

        public headerASS GetHeader()
        {
            return Header;
        }

        public Attachments GetAttachments()
        {
            return Attachments;
        }

        public void NewFile()
        {
            ResetScript();
            name = "Nuevo Archivo de PerrySub.ass";

            Header.SetHeaderDefaults();
            Styles.Add(new estiloV4());
            Lines.Add(new lineaASS());
            
        }

        public void InsertAttachment(string fn)
        {
            Attachments.InsertFromFile(fn);
        }

        public void ExtractAttachment(int idx, AttachmentType tipo, string fn)
        {
            Attachments.ExtractToFile(idx, tipo, fn);
        }

        public void LoadFromFile(string filename)
        {
            FileName = filename;
            FileInfo info = new FileInfo(FileName);
            if (!info.Exists) 
                throw new FileNotFoundException();
            ScriptFileType type = ScriptFileType.Unknown;

            switch (info.Extension.ToLower())
            {
                case ".ass":
                case ".ssa":
                case ".autosave":
                    type = ScriptFileType.ASS_SSA;
                    break;
                case ".srt":
                    type = ScriptFileType.SRT;
                    break;
                case ".txt":
                    type = ScriptFileType.TXT;
                    break;
                default:
                    type = ScriptFileType.Unknown;
                    break;
            }

            if (type == ScriptFileType.Unknown)
                throw new PerrySubException("Tipo de archivo desconocido");

            ResetScript();

            StreamReader sr = FileAccessWrapper.OpenTextFile(FileName);

            ArrayList head = Header.GetHeaderList();


            bool reachEOH = false;
            Tiempo ianterior = null;
            Tiempo fanterior = null;

            string linea = "";
            string pj = "";
            int i = 0;

            bool isGraphic = false;
            bool isFont = false;
            string gfx = ""; string gfx_name = "";
            string fnt = ""; string fnt_name = "";

            while ((linea = sr.ReadLine()) != null)
            {
                switch (type)
                {
                    case ScriptFileType.ASS_SSA:
                        if (linea.ToLower().StartsWith("[v4")) reachEOH = true;
                        if (!linea.ToLower().Equals("[script info]") && !reachEOH)
                        {
                            if (!linea.StartsWith("; "))
                                if (linea.IndexOf(": ") != -1)
                                    head.Add(linea);
                        }

                        if (linea.ToLower().StartsWith("style:"))
                            Styles.Add(new estiloV4(linea));

                        if (linea.ToLower().StartsWith("dialogue") || linea.ToLower().StartsWith("comment"))
                        {
                            try
                            {
                                Lines.Add(new lineaASS(linea));
                            }
                            catch (LineException le)
                            {
                                System.Windows.Forms.MessageBox.Show("Detectada excepción en una línea\n" + le.Message, mainW.appTitle, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                            }
                        }

                        // aqui tenemos que ver si hay attachments, y cargarlos si toca

                        if (isGraphic)
                        {
                            if (linea.Equals(""))
                            {
                                Attachments.InsertGraphic(gfx_name, gfx);
                                isGraphic = false;
                                gfx = "";
                                break;
                            }
                            gfx += linea+"\r\n";
                        }

                        if (isFont)
                        {
                            if (linea.Equals(""))
                            {
                                Attachments.InsertFont(fnt_name, fnt);
                                isFont = false;
                                fnt = "";
                                break;
                            }
                            fnt += linea+"\r\n";
                        }

                        if (linea.ToLower().StartsWith("filename:")) // [Graphics]
                        {
                            isGraphic = true;
                            gfx_name = linea.Substring(linea.IndexOf(':') + 1).Trim();
                        }

                        if (linea.ToLower().StartsWith("fontname:")) // [Fonts]
                        {
                            isFont = true;
                            fnt_name = linea.Substring(linea.IndexOf(':') + 1).Trim();
                        }

                        break;

                    case ScriptFileType.TXT:

                        // 1- nueva cabecera (luego)
                        // 2- estilo default (luego)
                        // 3- lineas

                        linea = linea.Replace("\t", "");
                        int idx = linea.IndexOf(":");
                        if (idx > 0)
                        {
                            pj = linea.Substring(0, idx).Trim();
                            linea = linea.Substring(idx + 1, linea.Length - (idx + 1)).Trim();
                        }
                        lineaASS telameto = new lineaASS();
                        telameto.texto = linea;
                        telameto.personaje = pj;
                        Lines.Add(telameto);
                        break;

                    case ScriptFileType.SRT:
                        //int moddy = i % 4;                        

                        if (i == 1)
                        {
                            string[] tiempos = linea.Split(new string[] { "-->" }, StringSplitOptions.None);
                            ianterior = new Tiempo(tiempos[0].Replace(',', '.').Trim());
                            fanterior = new Tiempo(tiempos[1].Replace(',', '.').Trim());
                        }
                        else if (i == 2)
                        {
                            Lines.Add(new lineaASS(ianterior.ToString(), fanterior.ToString(), "Default", linea));
                        }
                        else if (i > 2 && !linea.Equals(""))
                        {
                            lineaASS lass_ultima = (lineaASS)Lines.GetLines()[Lines.Count - 1];
                            lass_ultima.texto = lass_ultima.texto+"\\N"+linea;
                        }

                        if (linea.Equals("")) i = 0;
                        else i++;
                        break;
                }
            }

            Lines.Trim();
            sr.Close();

            if (isFont)
                Attachments.InsertFont(fnt_name, fnt); // por si acaso llegamos a EOF antes de \r\n
            if (isGraphic)
                Attachments.InsertFont(gfx_name, gfx);

            if (type == ScriptFileType.TXT || type == ScriptFileType.SRT)
            {
                Header.SetHeaderDefaults();
                Styles.Add(new estiloV4());
            }                        

            GC.Collect();
        }

        public void SaveFile()
        {
            SaveToFile(FileName);
        }

        public void SaveToFile(string filename)
        {
            TextWriter o = new StreamWriter(filename, false, System.Text.Encoding.UTF8);
            o.WriteLine(headerMark);
            o.WriteLine("; ---");
            o.WriteLine("; Editado usando "+mainW.appTitle);
            o.WriteLine("; Do it Perry style!! ~( º3º)~ ~(ºxº )~");
            o.WriteLine("; ---");

            Header.SetHeaderValue("PerrySubVersion", mainW.appTitle);            
            Header.SetHeaderValue("ScriptType", "v4.00+");

            o.WriteLine(Header.ToString());            

            //foreach (string s in Header.head)
            //    o.WriteLine(s);

            o.WriteLine(stylesMark);
            foreach (estiloV4 e in Styles)
                o.WriteLine(e.ToString().Replace("\n",string.Empty));

            o.WriteLine(); // ---

            o.WriteLine(dialoguesMark);
            foreach (lineaASS lass in Lines.GetFullArrayList())
                o.WriteLine(lass.ToString().Replace("\n", string.Empty));

            if (Attachments.HasAttachments)
            {
                o.WriteLine();
                o.WriteLine(Attachments.ToString());
            }

            o.Close();

        }


        #region Miembros de ICloneable

        public object Clone()
        {
            SubtitleScript nuevo = new SubtitleScript();

            nuevo.FileName = String.Copy(this.FileName);

            foreach (string s in this.Header.GetHeaderList())
            {
                string tmp = String.Copy(s);
                nuevo.GetHeaders().Add(tmp);
            }

            foreach (estiloV4 v in this.Styles)
                nuevo.GetStyles().Add(new estiloV4(v.ToString()));

            foreach (lineaASS l in this.Lines.GetFullArrayList())
                nuevo.GetLineArrayList().Add(new lineaASS(l.ToString()));

            Attachments natt = nuevo.GetAttachments();
            natt = (Attachments)Attachments.Clone();

            return nuevo;
        }

        #endregion
    }
}
