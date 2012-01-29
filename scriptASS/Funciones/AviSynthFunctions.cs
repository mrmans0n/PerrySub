using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Collections;
using System.Drawing;
using Microsoft.Win32;
using System.IO;
using System.Windows.Forms;

namespace scriptASS
{
    class AviSynthFunctions
    {

        public static void LoadAviSynthPlugin(AviSynthClip theclip,string funcname, string dllname)
        {
            if (theclip.FuncExists(theclip.GetAVS(), funcname)) return;
            if (File.Exists(dllname))
                theclip.AviSynthInvoke(theclip.GetAVS(), 0, "LoadPlugin", false, dllname);
        }

        public static string getVSFilterPath(AviSynthClip avs)
        {
            // opcion 1: está ya cargado

            if (avs.FuncExists(avs.GetAVS(), "TextSub")) return null;

            // opción 2: cargamos el que está en el registro
            try
            {
                RegistryKey r = RegistryKey.OpenRemoteBaseKey(RegistryHive.ClassesRoot, "");
                RegistryKey vsf = r.OpenSubKey(@"CLSID\{9852A670-F845-491B-9BE6-EBD841B8A613}\InprocServer32");
                string res = (string)vsf.GetValue("");
                if (res.ToLower().EndsWith("vsfilter.dll")) return res;
            }
            catch { }

            // opción 3: cargamos en que tenemos en nuestro directorio

            if (File.Exists(Path.Combine(Application.StartupPath,"VSFilter.dll")))
                return Path.Combine(Application.StartupPath, "VSFilter.dll");

            throw new AviSynthException("No se ha podido cargar el VSFilter.dll");
            //return null;
        }

        public static Bitmap getBitmapFromFrame(AviSynthClip avs, int clp, int frame)
        {
            Bitmap bmp;
            if (avs.PixelType == AviSynthColorspace.RGB24) bmp = new Bitmap(avs.VideoWidth, avs.VideoHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            else if (avs.PixelType == AviSynthColorspace.RGB32) bmp = new Bitmap(avs.VideoWidth, avs.VideoHeight, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            else
                bmp = new Bitmap(avs.VideoWidth, avs.VideoHeight, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

            try
            {
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                System.Drawing.Imaging.BitmapData bmpData =
                    bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    bmp.PixelFormat);
                try
                {
                    IntPtr ptr = bmpData.Scan0;
                    avs.ReadFrame(ptr, clp, bmpData.Stride, frame);
                }
                finally
                {
                    bmp.UnlockBits(bmpData);
                }
            }
            catch
            {
            }
            bmp.RotateFlip(RotateFlipType.Rotate180FlipX);
            return bmp;
        }
    }
}