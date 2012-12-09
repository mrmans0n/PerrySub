using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace scriptASS
{
    class T_LoadFullAudio
    {

        IntPtr puntero;
        AviSynthClip avs;
        AVStoDirectSound avs2ds;
        mainW mw;

        public T_LoadFullAudio(AviSynthClip a, AVStoDirectSound a2, IntPtr p, mainW m)
        {
            avs = a;
            avs2ds = a2;
            puntero = p;
            mw = m;
        }

        /*
        public void LoadAudio()
        {
            mainW.idx0rz = 0;
            mainW.AudioLoadFinished = false;
            int segs = Convert.ToInt32(Math.Ceiling((double)avs.SamplesCount / (double)avs.AudioSampleRate));
            Thread.Sleep(1000);
            DateTime inicio = DateTime.Now;
            for (int i = 0; i < segs; i++)
            {
                byte[] b = avs2ds.CargaAudioAVS((double)i, (double)i + 1);
                unsafe
                {
                    for (int x = 0; x < b.Length; x++)
                    {
                        try
                        {
                            byte* bptr = (byte*)puntero.ToPointer();
                            bptr = bptr + mainW.idx0rz;
                            *bptr = b[x];
                            mainW.idx0rz++;                            
                        }
                        catch { 
                            mainW.AudioLoadFinished = true; 
                            goto hemosterminado;
                        }
                        
                    }
                }             
            }
        hemosterminado:
            DateTime fin = DateTime.Now;
            TimeSpan tiempo = fin - inicio;
            MessageBox.Show(tiempo.ToString());
            ;
        }
 
        */

        /*
        public void LoadAudio()
        {
            mainW.idx0rz = 0;
            mainW.AudioLoadFinished = false;
            int segs = Convert.ToInt32(Math.Ceiling((double)avs.SamplesCount / (double)avs.AudioSampleRate));
            //Thread.Sleep(1000);

            DateTime inicio = DateTime.Now;

            for (int i = 0; i < segs; i++)
            {
                unsafe
                {
                    try
                    {
                        byte[] b = avs2ds.CargaAudioAVS((double)i, (double)i + 1);
                        byte* bdest = (byte*)puntero.ToPointer();
                        bdest += mainW.idx0rz;

                        fixed (byte* tmpb = b)
                        {
                            byte* borig = tmpb;                            

                            for (int x = 0; x < b.Length / 4; x++)
                            {
                                *((int*)bdest) = *((int*)borig);
                                bdest += 4;
                                borig += 4;
                                mainW.idx0rz = mainW.idx0rz + 4;
                            }

                            for (int x = 0; x < b.Length % 4; x++)
                            {
                                *bdest = *borig;
                                bdest++;
                                borig++;
                                mainW.idx0rz++;
                            }
                        }

                    }
                    catch
                    {
                        mainW.AudioLoadFinished = true;
                        goto hemosterminado;
                    }
                
                }
            }
        hemosterminado:
            DateTime fin = DateTime.Now;
            TimeSpan duracion = fin-inicio;
            MessageBox.Show(duracion.ToString());
            ;
        }
         */

        public void LoadAudio()
        {
            mainW.idx0rz = 0;
            mainW.AudioLoadFinished = false;
            int segs = Convert.ToInt32(Math.Ceiling((double)avs.SamplesCount / (double)avs.AudioSampleRate));
            //Thread.Sleep(1000);

            //DateTime inicio = DateTime.Now;

            for (int i = 0; i < segs; i++)
            {
                unsafe
                {
                    try
                    {
                        byte[] b = avs2ds.CargaAudioAVS((double)i, (double)i + 1);
                        byte* bdest = (byte*)puntero.ToPointer();
                        bdest += mainW.idx0rz;

                        fixed (byte* tmpb = b)
                        {
                            byte* borig = tmpb;

                            for (int x = 0; x < b.Length / 8; x++)
                            {
                                *((Int64*)bdest) = *((Int64*)borig);
                                bdest += 8;
                                borig += 8;
                                mainW.idx0rz = mainW.idx0rz + 8;
                            }

                            for (int x = 0; x < b.Length % 8; x++)
                            {
                                *bdest = *borig;
                                bdest++;
                                borig++;
                                mainW.idx0rz++;
                            }
                        }

                    }
                    catch
                    {
                        mainW.AudioLoadFinished = true;
                        goto hemosterminado;
                    }

                }
            }
        hemosterminado:
            //DateTime fin = DateTime.Now;
            //TimeSpan duracion = fin - inicio;
            //MessageBox.Show(duracion.ToString());
            ;
        }
    }
}
