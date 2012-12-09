using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;

namespace scriptASS
{

    public enum AttachmentType : int
    {
        Graphic = 0,
        Font = 1
    }

    public class Attachments : ICloneable
    {

        private ArrayList Graphics;
        private ArrayList Fonts;
        private bool hasAttachments;

        public bool HasAttachments
        {
            get { return (Graphics.Count > 0 || Fonts.Count > 0); }
        }


        public Attachments()
        {
            Graphics = new ArrayList();
            Fonts = new ArrayList();
        }

        public void Reset()
        {
            Graphics.Clear();
            Fonts.Clear();
        }

        public void InsertGraphic(string name, string encoded)
        {
            Graphics.Add(new attachmentASS(name, encoded));
        }

        public void InsertFont(string name, string encoded)
        {
            Fonts.Add(new attachmentASS(name, encoded));
        }

        public void InsertFromFile(string filename)
        {
            FileInfo fi = new FileInfo(filename);

            FileStream open = new FileStream(filename, FileMode.Open);
            byte[] b = new byte[open.Length];
            int written = open.Read(b, 0, b.Length);
            open.Close();

            switch (fi.Extension.ToLower())
            {
                case ".jpg":
                case ".bmp":
                case ".ico":
                case ".wmf":
                case ".gif":
                    Graphics.Add(new attachmentASS(fi.Name, b));
                    break;
                case ".ttf":
                    Fonts.Add(new attachmentASS(fi.Name, b));
                    break;
                default:
                    throw new Exception("Tipo de archivo desconocido");
                    break;
            }

        }

        public string GetFileName(int idx, AttachmentType tipo)
        {
            attachmentASS a = (tipo == AttachmentType.Font) ? (attachmentASS)Fonts[idx] : (attachmentASS)Graphics[idx];
            return a.FileName;
        }

        public attachmentASS GetAttachment(int idx, AttachmentType tipo)
        {
            attachmentASS a = (tipo == AttachmentType.Font) ? (attachmentASS)Fonts[idx] : (attachmentASS)Graphics[idx];
            return a;
        }

        public void ExtractToFile(int idx, AttachmentType tipo, string filename)
        {
            attachmentASS a = (tipo == AttachmentType.Font) ? (attachmentASS)Fonts[idx] : (attachmentASS)Graphics[idx];
            byte[] bdest = a.Decode();

            FileStream fs = new FileStream(filename, FileMode.Create);
            fs.Write(bdest, 0, bdest.Length);
            fs.Close();
            
        }

        public void Delete(int idx, AttachmentType tipo)
        {
            if (tipo == AttachmentType.Font)
                Fonts.RemoveAt(idx);
            else if (tipo == AttachmentType.Graphic)
                Graphics.RemoveAt(idx);

        }

        public override string ToString()
        {
            string res = "";

            if (Graphics.Count > 0)
            {
                res += "[Graphics]\r\n";

                foreach (attachmentASS at in Graphics)
                {
                    res += "filename: " + at.FileName + "\r\n";
                    res += at.EncodedData + "\r\n\r\n";
                }
            }

            if (Fonts.Count > 0)
            {
                res += "[Fonts]\r\n";

                foreach (attachmentASS at in Fonts)
                {
                    res += "fontname: " + at.FileName + "\r\n";
                    res += at.EncodedData + "\r\n\r\n";
                }
            }

            return res;
        }

        public ArrayList GetGraphicsAttachmentList()
        {
            return Graphics;
        }

        public ArrayList GetFontsAttachmentList()
        {
            return Fonts;
        }

        #region Miembros de ICloneable

        public object Clone()
        {
            Attachments at = new Attachments();
            foreach (attachmentASS a in Graphics)
                at.GetGraphicsAttachmentList().Add(a.Clone());

            foreach (attachmentASS a in Fonts)
                at.GetFontsAttachmentList().Add(a.Clone());

            return at;
        }

        #endregion
    }
}
