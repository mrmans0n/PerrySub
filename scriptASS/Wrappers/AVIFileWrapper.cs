using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace scriptASS
{
    class AVIFileWrapper
    {               
        [DllImport("avifil32.dll")]
        public static extern void AVIFileInit();
        [DllImport("avifil32.dll", PreserveSig = true)]
        public static extern int AVIFileOpen(
                ref int ppfile,
                String szFile,
                int uMode,
                int pclsidHandler);
        [DllImport("avifil32.dll")]
        public static extern int AVIFileGetStream(
                int pfile,
                out IntPtr ppavi,
                int fccType,
                int lParam);
        [DllImport("avifil32.dll")]
        public static extern int AVIStreamFindSample(int pfile, int lPos, int lFlags);
        [DllImport("avifil32.dll")]
        public static extern int AVIStreamRelease(IntPtr aviStream);
        [DllImport("avifil32.dll")]
        public static extern int AVIFileRelease(int pfile);
        [DllImport("avifil32.dll")]
        public static extern void AVIFileExit();

    }
}
