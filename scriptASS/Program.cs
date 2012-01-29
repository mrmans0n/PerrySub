using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;

namespace scriptASS
{
    static class Program
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(uint dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern bool TerminateThread(IntPtr hThread, uint dwExitCode);

        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.CurrentCulture = new System.Globalization.CultureInfo("es-ES");

            if (args.Length>0) Application.Run(new mainW(args));
            else Application.Run(new mainW());

            try
            {
                Application.Exit();
                // si no salimos aquí, malamente.
                /*
                Process thisProc = Process.GetCurrentProcess();
                ProcessThreadCollection myThreads = thisProc.Threads;

                foreach (ProcessThread pt in myThreads)
                {
                    DateTime startTime = pt.StartTime;
                    TimeSpan cpuTime = pt.TotalProcessorTime;
                    int priority = pt.BasePriority;
                    ThreadState ts = pt.ThreadState;

                    Console.WriteLine("thread:  {0}", pt.Id);
                    Console.WriteLine("\tstarted: {0}", startTime.ToString());
                    Console.WriteLine("\tCPU time: {0}", cpuTime);
                    Console.WriteLine("\tpriority: {0}", priority);
                    Console.WriteLine("\tthread state: {0}", ts.ToString());
                    // kill it with fire!
                    if (ts == ThreadState.Running)
                    {
                        Console.WriteLine("-- trying to kill it");
                        IntPtr ptr_t = OpenThread(1, false, (uint)pt.Id);
                        if (AppDomain.GetCurrentThreadId() == pt.Id)
                            Console.WriteLine("\tIt's current thread!");
                        else
                        {
                            TerminateThread(ptr_t, 1);
                            Console.WriteLine("\tThread killed.");
                        }
                    }
                }
                Console.WriteLine("-- end of threads");*/
            }
            catch { }
        }
    }
}