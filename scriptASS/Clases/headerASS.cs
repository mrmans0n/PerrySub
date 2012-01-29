using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;

namespace scriptASS
{
    public class HeaderException : PerrySubException
    {
        public HeaderException() : base() { }
        public HeaderException(string m) : base(m) { }
    }

    public class headerASS
    {
        public ArrayList head;

        public headerASS()
        {
             head = new ArrayList();
        }

        public void LoadFrom(string file)
        {
            head.Clear();
            try
            {
                StreamReader sr = FileAccessWrapper.OpenTextFile(file);
                string linea = "";
                while ((linea = sr.ReadLine()) != null)
                    if (!linea.Equals("")) head.Add(linea);
                sr.Close();
            }
            catch
            {
                throw new HeaderException("Error leyendo de archivo");
            }
        }

        public void SaveTo(string file)
        {
            try
            {
                TextWriter tw = new StreamWriter(file, false, System.Text.Encoding.UTF8);
                tw.Write(ToString());
                tw.Close();
            }
            catch
            {
                throw new HeaderException("Error escribiendo a archivo");
            }
        }

        public void SetHeaderDefaults()
        {
            SetHeaderValue("Title", "Archivo de Subtítulos de "+mainW.appTitle);
            SetHeaderValue("ScriptType", "v4.00+");
            SetHeaderValue("PlayResX", "640");
            SetHeaderValue("PlayResY", "480");
        }

        public ArrayList GetHeaderList()
        {
            return head;
        }

        public bool ExistsHeaderValue(string key)
        {
            return GetHeaderValue(key) != string.Empty;
        }

        public string GetHeaderValue(string key)
        {
            try
            {
                foreach (string s in head)
                {
                    int idx = s.IndexOf(':');
                    if (idx > 0)
                    {
                        string k = s.Substring(0, idx);
                        if (k.Equals(key, StringComparison.InvariantCultureIgnoreCase))
                            return s.Substring(idx + 1, s.Length - (idx + 1)).Trim();

                    }
                }
            }
            catch
            {
                throw new HeaderException("Error leyendo campo de cabecera"); 
            }
            return string.Empty;
        }

        public void SetHeaderValue(string key, string value)
        {
            try
            {
                if (!ExistsHeaderValue(key))
                    head.Add(key + ": " + value);
                else
                {
                    for (int i = 0; i < head.Count; i++)
                    {
                        string s = (string)head[i];
                        int idx = s.IndexOf(':');
                        if (idx > 0)
                        {
                            string k = s.Substring(0, idx);
                            if (k.Equals(key, StringComparison.InvariantCultureIgnoreCase))
                            {
                                head[i] = key + ": " + value;
                                return;
                            }

                        }
                    }

                }
            }
            catch
            {
                throw new HeaderException("Error escribiendo campo de cabecera");
            }
        }

        public override string ToString()
        {
            string res="";
            foreach (string s in head)
            {
                res += s + "\r\n";
            }

            return res;
        }

    }
}
