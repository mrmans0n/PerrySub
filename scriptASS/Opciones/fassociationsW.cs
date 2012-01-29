using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.IO;

namespace scriptASS
{
    public partial class fassociationsW : Form
    {
        //SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, 0, 0); 
        [DllImport("Shell32.dll")] 
        static extern void SHChangeNotify(int wEventId, int uFlags, IntPtr dwItem1, IntPtr dwItem2);
        const int SHCNE_ASSOCCHANGED = 0x08000000;
        const int SHCNF_IDLIST = 0;
        

        mainW mW;
        public fassociationsW(mainW m)
        {
            InitializeComponent();
            mW = m;
        }

        private void AplicaCambiosRegistro()
        {
            /*
            
            RegistryKey regkey;
            RegistryKey bleh, bleh2, bleh3, bleh4, bleh0;
            string actual, iconpath, filedescription;

            for (int i = 0; i < checkedListBox1.CheckedIndices.Count; i++)
            {
                regkey = RegistryKey.OpenRemoteBaseKey(RegistryHive.ClassesRoot, "");
                int idx = checkedListBox1.CheckedIndices[i];
                string ext = checkedListBox1.Items[idx].ToString();
                bleh = regkey.CreateSubKey(ext);
                string new_entry = "PerrySub" + ext;
                bleh.SetValue("", new_entry);


                actual = Application.ExecutablePath;
                iconpath = Application.StartupPath + "\\perrySub.Icons.dll";
                filedescription = "";
                switch (ext)
                {
                    case ".ssa":
                    case ".ass":
                    case ".srt":
                        iconpath = actual + ",0"; // perrysub.exe
                        filedescription = "Archivo de Subtítulos";
                        break;
                    case ".avs":
                    case ".avsi":
                        iconpath = iconpath + ",1";
                        filedescription = "Archivo de AviSynth";
                        break;
                    case ".txt":
                        iconpath = iconpath + ",2";
                        filedescription = "Archivo de texto";
                        break;
                }

                iconpath = "\"" + iconpath + "\"";

                regkey = RegistryKey.OpenRemoteBaseKey(RegistryHive.ClassesRoot, "");
                bleh = regkey.CreateSubKey(new_entry);
                bleh.SetValue("", filedescription);
                bleh2 = bleh.CreateSubKey("DefaultIcon");
                bleh2.SetValue("", iconpath);
                bleh2 = bleh.CreateSubKey("Shell");
                bleh2.SetValue("", "open");
                bleh3 = bleh2.CreateSubKey("Open");
                bleh3.SetValue("", "&Abrir con PerrySub");
                bleh4 = bleh3.CreateSubKey("Command");
                bleh4.SetValue("", "\"" + actual + "\" \"%1\"");

                regkey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "");
                bleh0 = regkey.OpenSubKey(@"SOFTWARE\Classes", true);
                bleh = bleh0.CreateSubKey(new_entry);
                bleh.SetValue("", filedescription);
                bleh2 = bleh.CreateSubKey("DefaultIcon");
                bleh2.SetValue("", iconpath);
                bleh2 = bleh.CreateSubKey("Shell");
                bleh2.SetValue("", "open");
                bleh3 = bleh2.CreateSubKey("Open");
                bleh3.SetValue("", "&Abrir con PerrySub");
                bleh4 = bleh3.CreateSubKey("Command");
                bleh4.SetValue("", "\"" + actual + "\" \"%1\"");

            }

            actual = Application.ExecutablePath;
            iconpath = actual;

            regkey = RegistryKey.OpenRemoteBaseKey(RegistryHive.ClassesRoot, "");
            bleh = regkey.CreateSubKey("PerrySub.exe");
            bleh.SetValue("", "Archivo de PerrySub");
            bleh2 = bleh.CreateSubKey("DefaultIcon");
            bleh2.SetValue("", iconpath);
            bleh2 = bleh.CreateSubKey("Shell");
            bleh2.SetValue("", "open");
            bleh3 = bleh2.CreateSubKey("Open");
            bleh3.SetValue("", "&Abrir con PerrySub");
            bleh4 = bleh3.CreateSubKey("Command");
            bleh4.SetValue("", "\"" + actual + "\" \"%1\"");

            regkey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "");
            bleh0 = regkey.OpenSubKey(@"SOFTWARE\Classes", true);
            bleh = bleh0.CreateSubKey("PerrySub.exe");
            bleh.SetValue("", "Archivo de PerrySub");
            bleh2 = bleh.CreateSubKey("DefaultIcon");
            bleh2.SetValue("", iconpath);
            bleh2 = bleh.CreateSubKey("Shell");
            bleh2.SetValue("", "open");
            bleh3 = bleh2.CreateSubKey("Open");
            bleh3.SetValue("", "&Abrir con PerrySub");
            bleh4 = bleh3.CreateSubKey("Command");
            bleh4.SetValue("", "\"" + actual + "\" \"%1\"");
            */

            for (int i = 0; i < checkedListBox1.CheckedIndices.Count; i++)
            {
                int idx = checkedListBox1.CheckedIndices[i];
                string ext = checkedListBox1.Items[idx].ToString();
                string actual, iconpath, filedescription;

                string new_entry = "PerrySub" + ext;

                actual = Application.ExecutablePath;
                iconpath = Path.Combine(Application.StartupPath, "Icons.dll");
                int iconorder = 0;
                filedescription = "Archivo de Subtítulos";
                switch (ext)
                {
                    case ".ssa":
                        iconorder = 3;
                        break;
                    case ".ass":
                        iconorder = 5;
                        break;
                    case ".srt":
                        iconorder = 4;
                        break;
                    case ".avs":
                    case ".avsi":
                        iconorder = 1;
                        filedescription = "Archivo de AviSynth";
                        break;
                    case ".txt":
                        iconorder = 2;
                        filedescription = "Archivo de texto";
                        break;
                }

                FileAssociation.Associate(ext, new_entry, filedescription, iconpath, iconorder, actual);

            }

            SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // HKEY_CLASSES_ROOT\.ssa - .ass - .txt ---> PerrySub
            // HKEY_CLASSES_ROOT\PerrySub\ (shell\open\command)

            // HKEY_LOCAL_MACHINE -> lo mismo
            // HKEY_LOCAL_MACHINE\SOFTWARE\Classes\Applications\perrysub.exe\shell\open\command -_-

            try
            {
                AplicaCambiosRegistro();
            }
            catch
            {
                mW.errorMsg("Si utilizas Windows Vista/Windows 7, debes ejecutar este programa como administrador para poder utilizar esta función.");
            }
            this.Dispose();
        }

        private void fassociationsW_Load(object sender, EventArgs e)
        {
            RegistryKey regkey = RegistryKey.OpenRemoteBaseKey(RegistryHive.ClassesRoot,"");
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                try
                {
                    RegistryKey temp = regkey.OpenSubKey(checkedListBox1.Items[i].ToString());
                    if (temp.GetValue("").ToString().StartsWith("PerrySub"))
                    {
                        checkedListBox1.SetItemChecked(i, true);
                    }
                }
                catch { }

            }
        }
    }
}