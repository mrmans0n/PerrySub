using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.Windows.Forms;

namespace scriptASS
{

    public delegate void ZipWrapperCompresionFinalizada(object sender, EventArgs e);
    public delegate void ZipWrapperListaArchivosObtenida(object sender, EventArgs e);
    public delegate void ZipWrapperArchivoComprimido(object sender, ArchivoComprimidoEventArgs e);

    public class ArchivoComprimidoEventArgs : EventArgs
    {
        public readonly string NombreArchivo;
        public readonly int Indice;
        public readonly int Total;

        public ArchivoComprimidoEventArgs(string NombreArchivo, int Indice, int Total)
        {
            this.NombreArchivo = NombreArchivo;
            this.Indice = Indice;
            this.Total = Total;
        }
    }

    class ZipWrapper
    {
        public event ZipWrapperListaArchivosObtenida ListaArchivosObtenida;
        public event ZipWrapperArchivoComprimido ArchivoComprimido;
        public event ZipWrapperCompresionFinalizada CompresionFinalizada;

        private string workDir;

        public string WorkDir
        {
            get { return workDir; }
            set { workDir = value; }
        }

        private string mask;

        public string Mask
        {
            get { return mask; }
            set { mask = value; }
        }
        private string zipFile;

        public string ZipFile
        {
            get { return zipFile; }
            set { zipFile = value; }
        }

        public ZipWrapper()
        {
        }

        public void ExtractFile(string zipfile)
        {
            FileStream fstr = File.OpenRead(zipfile);
            using (ZipInputStream s = new ZipInputStream(fstr))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string dirname = Path.GetDirectoryName(theEntry.Name);
                    string filename = Path.GetFileName(theEntry.Name);

                    string nuevodir = Path.Combine(Application.StartupPath, dirname);

                    if (dirname.Length > 0)
                        Directory.CreateDirectory(dirname);

                    if (!String.IsNullOrEmpty(filename))
                    {
                        using (FileStream streamwriter = File.Create(Path.Combine(nuevodir, filename)))
                        {
                            int size = 2048;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                    streamwriter.Write(data, 0, size);
                                else
                                    break;
                            }

                        }
                    }
                }
            }
        }

        public int CompressDirectory()
        {
            return CompressDirectory(workDir, mask, zipFile, true);
        }

        public int CompressDirectory(string directory, string mask, string zipfile, bool recursive)
        {
            string[] allmask = mask.Split(new char[] { ';' });

            ArrayList archive = new ArrayList();
            foreach (string onemask in allmask)
                archive.AddRange(Directory.GetFiles(directory, onemask, (recursive)? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));

            if (ListaArchivosObtenida != null)
                ListaArchivosObtenida(this, new EventArgs());

            int contador = 0;

            using (ZipOutputStream str = new ZipOutputStream(File.Create(zipfile)))
            {
                foreach (string file in archive)
                {
                    ZipEntry entrada = new ZipEntry(file);
                    FileStream origen = File.OpenRead(file);
                    FileInfo fi = new FileInfo(file);
                    byte[] buffer = new byte[Convert.ToInt32(origen.Length)];
                    origen.Read(buffer, 0, (int)origen.Length);
                    entrada.DateTime = fi.LastWriteTime;
                    entrada.Size = origen.Length;
                    origen.Close();
                    str.PutNextEntry(entrada);
                    str.Write(buffer, 0, buffer.Length);

                    if (ArchivoComprimido!= null)
                        ArchivoComprimido(this, new ArchivoComprimidoEventArgs(file, contador, archive.Count));

                    contador++;
                }
                str.Finish();
                str.Close();
            }

            if (CompresionFinalizada != null)
                CompresionFinalizada(this, new EventArgs());

            return archive.Count;
        }

    }
}
