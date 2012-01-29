using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace scriptASS
{
    class FileAccessWrapper
    {

        public static char[] InvalidCharacters = { '.', '\"', '/', '\\', '[', ']', ':', ';', '=', ',' };

        private static System.Text.Encoding getEncodingFromFile(string fileName)
        {
            System.Text.Encoding enc = null;
            System.IO.FileStream file = new System.IO.FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (file.CanSeek)
            {
                byte[] bom = new byte[4];
                file.Read(bom, 0, 4);
                if ((bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) ||
                    (bom[0] == 0xff && bom[1] == 0xfe) ||
                    (bom[0] == 0xfe && bom[1] == 0xff) ||
                    (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff))
                {
                    enc = System.Text.Encoding.Unicode;
                }
                else
                {
                    enc = System.Text.Encoding.ASCII;
                }

            }
            else
            {
                enc = System.Text.Encoding.ASCII;
            }
            file.Close();
            return enc;
        }

        public static StreamReader OpenTextFile(string fname){
            StreamReader sr;

            if (getEncodingFromFile(fname) == System.Text.Encoding.Unicode)
            {
                sr = File.OpenText(fname);
            }
            else
            {
                FileStream aFile = File.Open(fname, FileMode.Open);
                sr = new StreamReader(aFile, Encoding.GetEncoding(1252));
            }
            return sr;
        }

        public static void EnsureDirectory(System.IO.DirectoryInfo oDirInfo) 
        { 
            if (oDirInfo.Parent != null) 
 	            EnsureDirectory(oDirInfo.Parent); 
            if (!oDirInfo.Exists) 
 	        { 
 	            oDirInfo.Create(); 
 	        } 
 	    }

        public static void EnsureDirectory(string dirname)
        {
            FileAccessWrapper.EnsureDirectory(new DirectoryInfo(dirname));
        }

    }
}
