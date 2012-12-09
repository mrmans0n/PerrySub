using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Collections;

namespace scriptASS
{
    class RPC_Client
    {
        private string addr;
        private int port;

        private TcpClient cliente;
        private StreamReader input;
        private StreamWriter output;
        private bool cnn = false;

        public bool IsConnected
        {
            get { return cnn; }
        }

        public RPC_Client(string direccion, int puerto)
        {
            addr = direccion;
            port = puerto;
        }


        public bool Connect()
        {
            try
            {
                cliente = new TcpClient(addr, port);

                NetworkStream stream = cliente.GetStream();
                input = new StreamReader(stream, new UTF8Encoding(false));
                output = new StreamWriter(stream, new UTF8Encoding(false));

                output.AutoFlush = true;
                output.WriteLine("mrmr0x");
                
                bool bbb = cliente.Connected && input.ReadLine().StartsWith("Bienvenido", StringComparison.InvariantCultureIgnoreCase);
                cnn = bbb;
                return bbb;
            }
            catch {}
            return false;

        }

        private ArrayList GetListing()
        {
            ArrayList res = new ArrayList();
            string linea = "";
            while (!(linea = input.ReadLine()).Equals(".")) 
                res.Add(linea);
            return res;
        }

        public ArrayList GetDictionaries()
        {
            output.WriteLine("dicts");
            return GetListing();
        }

        public Hashtable GetAllDefinitions(string dict)
        {
            Hashtable res = new Hashtable();
            output.WriteLine("dictlist " + dict);
            foreach (string s in GetListing())
            {
                string[] ss = s.Split(new char[] { ':' }, 2);
                res.Add(ss[0].Trim(), ss[1].Trim());
            }            
            return res;
        }

        public ArrayList GetDefinitionListOnly(string dict)
        {            
            return new ArrayList(GetAllDefinitions(dict).Keys);
        }

        public bool ExistDictionary(string dict)
        {
            return GetDictionaries().Contains(dict);
        }

        public bool ExistDefinition(string dict, string key)
        {
            return GetAllDefinitions(dict).ContainsKey(key);
        }

        public bool IsDefinitionModified(string dict, string key, string value)
        {
            return (!GetDefinition(dict, key).Equals(value));
        }

        public void CreateDictionary(string dict)
        {
            output.WriteLine("dictcreate " + dict);
        }

        public void DeleteDictionary(string dict)
        {
            output.WriteLine("dictdel " + dict);
        }

        public void InsertDefinition(string dict, string key, string value)
        {
            output.WriteLine("dictinsert " + dict + "," + key + ": " + value);
        }

        public string GetDefinition(string dict, string key)
        {
            return (string)GetAllDefinitions(dict)[key];
        }

        public void EditDefinition(string dict, string key, string value)
        {
            DeleteDefinition(dict, key);
            InsertDefinition(dict, key, value);
        }

        public void DeleteDefinition(string dict, string key)
        {
            output.WriteLine("dictremove " + dict + "," + key);
        }

        public void Disconnect()
        {
            output.WriteLine("quit");
            cnn = false;
            //cliente.Client.Disconnect(true);
        }

    }
}
