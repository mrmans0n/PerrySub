using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data;

using Microsoft.DirectX.DirectSound;
using Buffer = Microsoft.DirectX.DirectSound.Buffer;

/// <summary>
/// Summary description for AVStoDirectSound
/// </summary>
/// 
namespace scriptASS
{
    public class WaveCustomization
    {
        public static Color WaveColor = Color.Blue; // TODO: poder cambiar por configuracion el color de la onda
    }

    public class AVStoDirectSound
    {
        Device applicationDevice;
        AviSynthClip avs;
        int max_vol, min_vol;

        public AVStoDirectSound(System.Windows.Forms.Control form, AviSynthClip avs)
        {
            PreparaAudio(form);
            this.avs = avs;
            max_vol = 0;
            min_vol = 999999999;
        }

        private void PreparaAudio(System.Windows.Forms.Control form)
        {
            try
            { 
                //Initialize the DirectSound Device
                applicationDevice = new Device();

                // Set the priority of the device with the rest of the operating system
                applicationDevice.SetCooperativeLevel(form, CooperativeLevel.Priority);
            }
            catch 
            {
                // 
                throw new Exception("Error al inicializar Direct Sound");
                //Close();
                //return;
            }
        }


        public byte[] CargaAudioAVS(int inicio, int final)
        {
            int len = final - inicio;
            byte[] buffer = new byte[len * avs.ChannelsCount * avs.BytesPerSample]; //avs.AudioSizeInBytes];
            avs.ReadAudio(buffer, inicio, len);

            return buffer;
        }

        public int[] ConvertToIntegers(byte[] buffer)
        {
            max_vol = 0;
            min_vol = 999999999;
            int len = buffer.Length;
            int[] bufferEntero = new int[len / avs.BytesPerSample];

            for (int i = 0; i < len; i += avs.BytesPerSample)//avs.AudioSizeInBytes; i += 2)
            {
                try
                {
                    int cacho = buffer[i] * 256 + buffer[i + 1];
                    bufferEntero[i / avs.BytesPerSample] = cacho;
                    if (cacho > max_vol)
                    {
                        max_vol = cacho;
                    }
                    if (cacho > 0 && cacho < min_vol)
                    {
                        min_vol = cacho;
                    }
                }
                catch
                {
                    max_vol = 1;
                }


            }

            return bufferEntero;
        }
        public byte[] CargaAudioAVS(double Sinicial, double Sfinal)
        {
            int inicio = (int)(avs.AudioSampleRate * Sinicial);
            int final = (int)(avs.AudioSampleRate * Sfinal);

            if (inicio % 2 != 0)
                inicio++;
            if (final % 2 != 0)
                final++;
            return CargaAudioAVS(inicio, final);
        }

        public byte[] AudioConvertToBytes(int[] bufferEntero)
        {
            byte[] buffer = new byte[bufferEntero.Length * 2];
            for (int i = 0; i < buffer.Length; i += 2)
            {
                buffer[i] = Convert.ToByte(Math.Abs(bufferEntero[i / 2]) % 256);
                buffer[i + 1] = Convert.ToByte(Math.Abs(bufferEntero[i / 2]) - (Math.Abs(bufferEntero[i / 2]) / 256) * 256);
            }

            return buffer;
        }

        /*
        //Dibuja la grafica del wave entre los valores [-15,15] hace falta aplicarle un zoom del orden alto/10
        //un buen valor seria asignar como zoom=zoomAux*alto/20
        public Bitmap DrawWave(int[] bufferEntero, int ancho, int alto, double zoom, int modo)
        {
            int zancada = Convert.ToInt32(bufferEntero.Length) / ancho;
            Bitmap aux = new Bitmap(ancho, alto);

            if (modo < 0)
                modo = 0;
            if (modo > 3)
                modo = 3;
            // Tomamos las dimensiones de la imagen
            int width = aux.Width;
            int height = aux.Height;

            // Bloqueamos las imágenes

            BitmapData destinoDatos = aux.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            // Cogemos el ancho de línea de imagen
            int stride = destinoDatos.Stride;
            // El offset que hay que utilizar (para el relleno)
            int offset = stride - (width * 4);


            unsafe
            {
                // Empezamos en fila 0 columna 0
                byte* dst = (byte*)destinoDatos.Scan0.ToPointer() + stride;
                try
                {
                    for (int x = 0; x < width; x++, dst += 4)
                    {
                        double valorEnI = 0;
                        for (int j = 0; j < zancada; j += zancada / 20)
                        {
                            valorEnI += bufferEntero[j + x * zancada];
                        }

                        valorEnI = 20 * valorEnI / zancada;



                        double rango;
                        if (valorEnI > 0)
                        {
                            switch (modo)
                            {
                                case 3:
                                    rango = zoom * Math.Exp(Math.Log10(zoom * valorEnI / ((min_vol + max_vol) / 40)));
                                    break;
                                case 2:
                                    rango = zoom * Math.Log(zoom * valorEnI / ((min_vol + max_vol) / 40), 2);
                                    break;
                                case 1:
                                    rango = Math.Sqrt(zoom) * Math.Pow(1 + Math.Max(0, ((valorEnI - (min_vol + max_vol) / 50)) / (max_vol / 40)), zoom);
                                    break;
                                default:
                                    rango = Math.Sqrt(zoom) * Math.Pow(1 + valorEnI / (max_vol / 20), zoom);
                                    break;
                            }

                        }
                        else
                            rango = 0;

                        int arriba = Convert.ToInt32((aux.Height / 2) - rango);
                        int abajo = Convert.ToInt32((aux.Height / 2) + rango);
                        dst = (byte*)destinoDatos.Scan0.ToPointer() + stride;
                        dst += x * 4 + stride * arriba;
                        for (int y = arriba; y <= abajo; y++, dst += stride)
                        {

                            dst[0] = 255;
                            dst[1] = 0;
                            dst[2] = 0;
                            dst[3] = 255;

                        }//fin for

                    }//fin for

                }
                catch
                { //algun puntero se acaba escapando 
                }

            } aux.UnlockBits(destinoDatos);



            return aux;
        }
        */
        private int Convierte(int a)
        {
            int numBits = 16;
            BitArray b = new BitArray(new int[] { a });
            BitArray bAux = (BitArray)b.Clone();
            for (int i = 0; i < b.Length; i++)
            {
                b[i] = bAux[b.Length - 1 - i];
            }
            int result = -(a & (1 << numBits - 1));
            for (int i = numBits - 2; i >= 0; i--)
                result |= (a & (1 << i));

            return result;
        }

        private int Proximo2N(int a)
        {
            int res = 2;

            while (res < a)
            {
                res = res * 2;
            }


            return res;
        }

        private int VB(byte[] bufferEntero, int x)
        {
            return Convierte((bufferEntero[1 + x] << 8) + bufferEntero[x]);
        }
        public Bitmap DrawWave(byte[] bufferEntero, int ancho, int alto, double zoom, int modo)
        {
            Boolean FFTActivo = false;
            Boolean WaveFFT = false;
            if (bufferEntero.Length % 2 != 0)
                MessageBox.Show("Fatal Error: El Buffer no es de tamaño par");

            if (ancho % 2 != 0)
                ancho--;

            int zancada = Convert.ToInt32(bufferEntero.Length) / (ancho);
            if (zancada % 2 != 0)
                zancada++;

            Bitmap aux = new Bitmap(ancho + 1, alto);
            if (modo >= 4)
                FFTActivo = true;
            if (modo == 5)
                WaveFFT = true;
            if (modo < 0)
                modo = 0;
            if (modo > 3)
                modo = 3;
            // Tomamos las dimensiones de la imagen
            int width = aux.Width;
            int height = aux.Height;

            // Bloqueamos las imágenes

            BitmapData destinoDatos = aux.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            // Cogemos el ancho de línea de imagen
            int stride = destinoDatos.Stride;
            // El offset que hay que utilizar (para el relleno)
            int offset = stride - (width * 4);


            int salto = Proximo2N(Convert.ToInt32(3 * bufferEntero.Length / avs.AudioSampleRate));
            unsafe
            {
                // Empezamos en fila 0 columna 0
                byte* dst = (byte*)destinoDatos.Scan0.ToPointer() + stride;
                try
                {
                    for (int x = 0; x < bufferEntero.Length; x += salto*8, dst += 8)
                    {
                        double valorEnI = 0;

                        //FFT
                        if (FFTActivo)
                        {
                            FFT f1 = new FFT(bufferEntero, 2, x, x + salto*8);
                            f1.AplicaFFT();
                            f1.CreaEspectro();
                            byte rango;
                            if (WaveFFT)
                            {
                                int auxl = 0;
                                try
                                {
                                    if (x > 0 && x + salto < bufferEntero.Length)
                                    {
                                        auxl = (VB(bufferEntero, x - salto) + VB(bufferEntero, x + salto / 4) + VB(bufferEntero, x + salto / 2) + VB(bufferEntero, x - salto / 4) + VB(bufferEntero, x - salto / 2) + VB(bufferEntero, x) + VB(bufferEntero, x + salto)) / 7;
                                    }
                                    else
                                        auxl = VB(bufferEntero, x);

                                }
                                catch
                                {
                                    MessageBox.Show("Buffer Overflow en DrawWave: Acceso al Buffer fuera de rango");
                                }

                                valorEnI = auxl;
                                rango = Convert.ToByte(255*Math.Pow(Math.Abs(valorEnI / 65769),0.5/zoom));


                            }
                            else
                                rango = 255;

                            //MessageBox.Show("Hay "+f1.numeroFrecuencias()+" frecuencias en el espectro");
                            dst = (byte*)destinoDatos.Scan0.ToPointer() + 4 * (x / zancada) + stride;
                            for (int y = 0; y+4 < height/2-1; y+=4)
                            {
                                int valor = Math.Abs(f1.EspectroPaso(y, aux.Height/2));
                                double max = f1.GetMaxReal();
                                valor=((int)Math.Floor(255*(valor)/(max+1)));
                                int frecuencia = y * avs.AudioSampleRate / (salto * 8);

                                byte azul = Convert.ToByte((valor));
                                byte verde = Convert.ToByte((Math.Min(255,Math.Max(0,1.8*valor-64))));
                                byte rojo = Convert.ToByte((Math.Min(255,Math.Max(0,2*valor-128))));



                                if (frecuencia < 82 || frecuencia > 1056)
                                {
                                    dst[0] = azul;
                                    dst[1] = verde;
                                    dst[2] = rojo;
                                    dst[3] = Convert.ToByte(3*rango/5);
                                    dst += stride;

                                    dst[0] = azul;
                                    dst[1] = verde;
                                    dst[2] = rojo;
                                    dst[3] = Convert.ToByte(rango*3/4);
                                    dst += stride;

                                    dst[0] = azul;
                                    dst[1] = verde;
                                    dst[2] = rojo;
                                    dst[3] = Convert.ToByte(rango*3/4);
                                    dst += stride;

                                    dst[0] = azul;
                                    dst[1] = verde;
                                    dst[2] = rojo;
                                    dst[3] = rango;
                                    dst += stride;

                                    dst[0] = azul;
                                    dst[1] = verde;
                                    dst[2] = rojo;
                                    dst[3] = rango;
                                    dst += stride;

                                    dst[0] = azul;
                                    dst[1] = verde;
                                    dst[2] = rojo;
                                    dst[3] = Convert.ToByte(rango*3/4);
                                    dst += stride;

                                    dst[0] = azul;
                                    dst[1] = verde;
                                    dst[2] = rojo;
                                    dst[3] = Convert.ToByte(rango*3/4);
                                    dst += stride;

                                    dst[0] = azul;
                                    dst[1] = verde;
                                    dst[2] = rojo;
                                    dst[3] = Convert.ToByte(3*rango/5);
                                    dst += stride;
                                }
                                else
                                {
                                    dst[0] = rojo;
                                    dst[1] = verde;
                                    dst[2] = azul;
                                    dst[3] = Convert.ToByte(3*rango/5);
                                    dst += stride;

                                    dst[0] = rojo;
                                    dst[1] = verde;
                                    dst[2] = azul;
                                    dst[3] = Convert.ToByte(rango * 3 / 4);
                                    dst += stride;

                                    dst[0] = rojo;
                                    dst[1] = verde;
                                    dst[2] = azul;
                                    dst[3] = Convert.ToByte(rango * 3 / 4);
                                    dst += stride;

                                    dst[0] = rojo;
                                    dst[1] = verde;
                                    dst[2] = azul;
                                    dst[3] = rango;
                                    dst += stride;

                                    dst[0] = rojo;
                                    dst[1] = verde;
                                    dst[2] = azul;
                                    dst[3] = rango;
                                    dst += stride;

                                    dst[0] = rojo;
                                    dst[1] = verde;
                                    dst[2] = azul;
                                    dst[3] = Convert.ToByte(rango * 3 / 4);
                                    dst += stride;

                                    dst[0] = rojo;
                                    dst[1] = verde;
                                    dst[2] = azul;
                                    dst[3] = Convert.ToByte(rango * 3 / 4);
                                    dst += stride;

                                    dst[0] = rojo;
                                    dst[1] = verde;
                                    dst[2] = azul;
                                    dst[3] = Convert.ToByte(3*rango/5);
                                    dst += stride;
                                }


                            }//fin for

                            
                            
                        }//fin if
                        else //Normal
                        {
                            /*
                             *        Si convierto a entero ponia esto al reves
                            int auxl=Convierte(( (bufferEntero[x] << 8) + bufferEntero[1+x]));
                             */
                            int auxl = 0;
                            try
                            {
                                if (x > 0 && x + salto < bufferEntero.Length)
                                {
                                    auxl = (VB(bufferEntero, x - salto) + VB(bufferEntero, x + salto / 4) + VB(bufferEntero, x + salto / 2) + VB(bufferEntero, x - salto / 4) + VB(bufferEntero, x - salto / 2) + VB(bufferEntero, x) + VB(bufferEntero, x + salto)) / 7;
                                }
                                else
                                    auxl = VB(bufferEntero, x);

                            }
                            catch
                            {
                                MessageBox.Show("Buffer Overflow en DrawWave: Acceso al Buffer fuera de rango");
                            }



                            valorEnI = auxl;


                            double rango = 0;
                            try
                            {
                                if (valorEnI > -32768)
                                {
                                    switch (modo)
                                    {
                                        case 3:
                                            rango = Math.Sqrt(zoom) * Math.Pow(1 + valorEnI / 35769, zoom * 2);
                                            break;
                                        case 2:
                                            rango = zoom * Math.Pow(1 + valorEnI / 65769, Math.Sqrt(zoom));
                                            break;
                                        case 1:
                                            rango = zoom * Math.Exp(1 + valorEnI / 65769);
                                            break;
                                        default:
                                            rango = zoom * Math.Pow(1 + valorEnI / 65769, zoom);
                                            break;
                                    }

                                }
                                else
                                    rango = 0;
                            }
                            catch
                            {
                                MessageBox.Show("Excepcion Matematica: El valor es Infinito");
                            }

                            int arriba = Convert.ToInt32(Math.Min(aux.Height - 1, Math.Max(1, ((double)aux.Height / 2) - (rango - zoom))));
                            int abajo = aux.Height - arriba;
                            if (arriba > abajo)
                            {
                                int auxI = arriba;
                                arriba = abajo;
                                abajo = auxI;
                            }
                            dst = (byte*)destinoDatos.Scan0.ToPointer() + stride;
                            dst += x / zancada * 4 + stride * arriba;
                            if (arriba == abajo)
                            {
                                dst[0] = 255;
                                dst[1] = 0;
                                dst[2] = 0;
                                dst[3] = 255;
                            }
                            if (FFTActivo)
                            {

                            }//fin if
                            else
                            {
                                for (int y = arriba; y < abajo && y < height; y++)
                                {


                                    dst[0] = 255;
                                    dst[1] = 0;
                                    dst[2] = 0;
                                    dst[3] = 255;

                                    dst += stride;
                                }//fin for
                            }//fin else

                        }//fin for
                    }
                }
                catch
                { //algun puntero se acaba escapando 
                    MessageBox.Show("Pointer Overflow en DrawWave: Acceso al Bitmap fuera de rango");
                }

            } aux.UnlockBits(destinoDatos);



            return aux;
        }


        public SecondaryBuffer PreparaAudio(byte[] buffer)
        {
            WaveFormat format = new WaveFormat
                                    {
                                        BitsPerSample = avs.BitsPerSample,
                                        Channels = avs.ChannelsCount,
                                        BlockAlign = Convert.ToInt16(avs.BytesPerSample*avs.ChannelsCount),
                                        FormatTag = WaveFormatTag.Pcm,
                                        SamplesPerSecond = avs.AudioSampleRate,
                                        AverageBytesPerSecond = avs.AvgBytesPerSec
                                    };

            // buffer description         
            BufferDescription desc = new BufferDescription(format)
                                         {
                                             DeferLocation = true,
                                             ControlVolume = true,
                                             BufferBytes = buffer.Length
                                         };

            // create the buffer         
            //Device ApplicationDevice = new Device();
            SecondaryBuffer secondaryBuffer = null;
            try
            {
                secondaryBuffer = new SecondaryBuffer(desc, applicationDevice);
                //load audio samples to secondary buffer
                secondaryBuffer.Write(0, buffer, LockFlag.EntireBuffer);

            }
            catch (OutOfMemoryException)
            {
                MessageBox.Show("Error en el envío de sonido por falta de memoria.\nUna posible solución sería rebajar el valor del campo Multiplicador.");
            }
            catch (Exception x)
            {
                MessageBox.Show("Error en el envío de sonido (" + x.ToString() + "): " + x.Message);
            }

            return secondaryBuffer;
        }
    }
}
