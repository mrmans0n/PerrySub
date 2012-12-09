using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace scriptASS
{
    class ZipSubtitleScript : SubtitleScript
    {
        public ZipSubtitleScript()
            : base() { }
        public ZipSubtitleScript(string filename)        
        {            
            LoadFromZip(filename);
        }


        public ZipSubtitleScript(SubtitleScript script)
        {
            LoadFromScript(script);
        }

        public void LoadFromScript(SubtitleScript script)
        {
            this.GetHeaders().Clear();
            this.GetHeaders().AddRange(script.GetHeaders());
            this.GetStyles().Clear();
            this.GetStyles().AddRange(script.GetStyles());
            this.GetLineArrayList().GetFullArrayList().Clear();
            this.GetLineArrayList().GetFullArrayList().AddRange(script.GetLineArrayList().GetFullArrayList());
            this.Attachments = script.GetAttachments();
            this.FileName = script.FileName;
        }

        public void LoadFromZip(string filename)
        {
            FileStream fzip = File.OpenRead(filename);

            using (ZipInputStream s = new ZipInputStream(fzip))
            {
                ZipEntry entrada;
                while ((entrada = s.GetNextEntry()) != null)
                {
                    string fn = Path.GetFileName(entrada.Name) + ".autosave";
                    using (FileStream writer = File.Create(fn))
                    {
                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                                writer.Write(data, 0, size);
                            else
                                break;
                        }
                    }

                    LoadFromFile(fn);

                    File.Delete(fn);
                }
            }
        }

        public void SaveToZip(string filename)
        {
            string temp = filename + ".temp";
            base.SaveToFile(temp);
            // tendremos que proceder a comprimirlo o algo

            FileStream fzip = File.Create(filename + ".zip");
            FileInfo fi = new FileInfo(filename);

            using (ZipOutputStream s = new ZipOutputStream(fzip))
            {
                ZipEntry ze = new ZipEntry(fi.Name);
                FileStream fass = File.OpenRead(temp);
                byte[] buffer = new byte[Convert.ToInt32(fass.Length)];
                fass.Read(buffer, 0, (int)fass.Length);
                ze.DateTime = fi.LastWriteTime;
                ze.Size = fass.Length;
                fass.Close();
                s.PutNextEntry(ze);
                s.Write(buffer, 0, buffer.Length);
                s.Finish();
                s.Close();
            }

            File.Delete(temp);
        }
    }
}
