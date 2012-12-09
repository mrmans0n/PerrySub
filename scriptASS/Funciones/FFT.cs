using System;
using System.Collections.Generic;
using System.Text;

namespace scriptASS
{
    class FFT 
    {
        private double[] Vimag;
        private double[] Vreal;
        private int[] Espectro;
        private int Espectromax;
        private double FFTRmax=0;
        private double FFTRmin=0;
        private bool isFFT;
        private bool EspectroCreado=false;


        public FFT()
        {
            Vimag = new double[1];
            Vreal = new double[1];
            FFTRmax=0;
            isFFT = false;
        }

    
        public FFT(int length)
        {
            Vimag = new double[length];
            Vreal = new double[length];
            FFTRmax=0;
            isFFT = false;
        }

        public FFT(int[] vectorEntero)
        {
            int tamañoV = SizeV2N(vectorEntero.Length);
            Vreal = new double[tamañoV];
            Vimag = new double[tamañoV];
            FFTRmax=0;
            isFFT = false;

            for (int i = 0; i < vectorEntero.Length; i++)
            {
                Vreal[i] = Convert.ToDouble(vectorEntero[i]);
                Vimag[i] = 0;
            }
            for (int i = vectorEntero.Length; i < Vreal.Length; i++)
            {
                Vreal[i] = 0;
                Vimag[i] = 0;
            }
        }

        public FFT(byte[] vectorBytes, int BpS)
        {
            int tamañoV = SizeV2N(vectorBytes.Length);
            Vreal = new double[tamañoV / BpS];
            Vimag = new double[tamañoV / BpS];
            FFTRmax=0;
            isFFT = false;

            for (int i = 0; i < Vreal.Length; i += BpS)
            {
                for (int j = 0; j < BpS; j++)
                {
                    if (i + j < vectorBytes.Length)
                        Vreal[i] = Convert.ToDouble((vectorBytes[i+j+1] << 8) + vectorBytes[i+j]);
                    else
                        Vreal[i] = 0;
                    Vimag[i] = 0;
                }
            }
        }

        public FFT(byte[] vectorBytes, int BpS, int inicio, int final)
        {
            int final2=final;

            if (inicio % 2 != 0)
                inicio++;
            if (final % 2 != 0)
                final++;

            //Evito desbordamiento ajustando los valores a los extremos. 
            if (final > vectorBytes.Length)
                final2 = vectorBytes.Length;
                        
            if (inicio < 0)
                inicio = 0;

            int tamañoV = (final-inicio);
            Vreal = new double[tamañoV / BpS];
            Vimag = new double[tamañoV / BpS];
            FFTRmax=0;
            isFFT = false;

            int ind = 0;
            for (int i = inicio; i < final2; i += BpS,ind++)
            {

                Vreal[ind] = Convert.ToDouble((vectorBytes[i + 1] << 8) + (vectorBytes[i]));
                Vimag[ind] = 0;

            }
        }

        public FFT(long[] vectorLong)
        {
            int tamañoV = SizeV2N(vectorLong.Length);
            Vreal = new double[tamañoV];
            Vimag = new double[tamañoV];
            FFTRmax=0;
            isFFT = false;

            for (int i = 0; i < vectorLong.Length; i++)
            {
                Vreal[i] = Convert.ToDouble(vectorLong[i]);
                Vimag[i] = 0;
            }
            for (int i = vectorLong.Length; i < Vreal.Length; i++)
            {
                Vreal[i] = 0;
                Vimag[i] = 0;
            }

        }

        public FFT(double[] vectorD)
        {
            int tamañoV = SizeV2N(vectorD.Length);
            Vreal = new double[tamañoV];
            Vimag = new double[tamañoV];
            FFTRmax=0;
            isFFT = false;

            for (int i = 0; i < vectorD.Length; i++)
            {
                Vreal[i] = vectorD[i];
                Vimag[i] = 0;
            }
            for (int i = vectorD.Length; i < Vreal.Length; i++)
            {
                Vreal[i] = 0;
                Vimag[i] = 0;
            }
        }

        public FFT(int[] VIR, int[] VII)
        {
            if (VIR.Length != VII.Length)
                throw new Exception("Tamño de los vectores no coinciden");

            int tamañoV = SizeV2N(VIR.Length);
            Vreal = new double[tamañoV];
            Vimag = new double[tamañoV];
            FFTRmax=0;
            isFFT = false;

            for (int i = 0; i < Vreal.Length; i++)
            {
                Vreal[i] = Convert.ToDouble(VIR[i]);
                Vimag[i] = Convert.ToDouble(VII[i]);
            }
        }

        public FFT(long[] VIR, long[] VII)
        {
            if (VIR.Length != VII.Length)
                throw new Exception("Tamño de los vectores no coinciden");

            FFTRmax=0;
            int tamañoV = SizeV2N(VIR.Length);
            Vreal = new double[tamañoV];
            Vimag = new double[tamañoV];
            isFFT = false;

            for (int i = 0; i < Vreal.Length; i++)
            {
                Vreal[i] = Convert.ToDouble(VIR[i]);
                Vimag[i] = Convert.ToDouble(VII[i]);
            }
        }
        public FFT(double[] VIR, double[] VII)
        {
            if (VIR.Length != VII.Length)
                throw new Exception("Tamño de los vectores no coinciden");

            int tamañoV = SizeV2N(VIR.Length);
            Vreal = new double[tamañoV];
            Vimag = new double[tamañoV];
            FFTRmax=0;
            isFFT = false;

            for (int i = 0; i < Vreal.Length; i++)
            {
                Vreal[i] = VIR[i];
                Vimag[i] = VII[i];
            }
        }

        public double[] GetVreal()
        {
            return Vreal;
        }

        public int[] GetVrealAsIntegers()
        {
            int[] aux = new int[Vreal.Length];
            for (int i = 0; i < Vreal.Length; i++)
                aux[i] = Convert.ToInt32(Vreal[i]);
            return aux;
        }

        public int[] GetValueAsIntegers()
        {
            int[] aux = new int[Vreal.Length];
            for (int i = 0; i < Vreal.Length; i++)
                aux[i] = Convert.ToInt32(Math.Sqrt(Vreal[i] * Vreal[i] + Vimag[i] * Vimag[i]));
            return aux;
        }

        public byte[] GetVrealAsBytes(int BpS)
        {
            byte[] aux = new byte[(Vreal.Length + 1) * BpS];
            for (int i = 0; i < Vreal.Length; i++)
            {
                for (int j = BpS; j > 0; j--)
                {
                    try
                    {
                        double valor = ToByte(Math.Abs(Vreal[i]), BpS, j);

                        aux[i * BpS + j] = Convert.ToByte(Math.Round(valor));
                    }
                    catch
                    {
                        throw new Exception("Unknow error");
                    }
                }
            }
            return aux;
        }

        public double[] GetImag()
        {
            return Vimag;
        }

        public double[,] AplicaYConservaFFT()
        {
            double[,] res = new double[Vreal.Length, 2];
            double[] AuxR = (double[])Vreal.Clone();
            double[] AuxI = (double[])Vimag.Clone();
            fft(ref AuxR, ref  AuxI, 1);

            for (int j = 0; j < Vreal.Length; j++)
            {
                res[j, 0] = AuxR[j];
                res[j, 1] = AuxI[j];
            }

            return res;
        }


        public double[,] AplicaYConservaIFFT()
        {
            double[,] res = new double[Vreal.Length, 2];
            double[] AuxR = (double[])Vreal.Clone();
            double[] AuxI = (double[])Vimag.Clone();
            fft(ref AuxR, ref AuxI, -1);

            for (int j = 0; j < Vreal.Length; j++)
            {
                res[j, 0] = AuxR[j];
                res[j, 1] = AuxI[j];
            }

            return res;
        }

        public void AplicaFFT()
        {
            fft(ref Vreal, ref Vimag, 1);
            isFFT = true;

        }

        public void AplicaIFFT()
        {
            fft(ref Vreal, ref Vimag, -1);
            isFFT = false;
        }

        public double GetMaxReal()
        {
            return Espectromax;
        }
        //Ya me gustaria a mi que coincidiera con la frecuencia xDDD
        //Esto no es un filtro de paso bajo esto es una burrada como un camello pero 
        //necesito comprobar cosas.
        public void AplicaFiltroPasoBajo(int frecuencia)
        {
            if (isFFT)
            {
                for (int i = 0; i < Vreal.Length; i++)
                {
                    Vreal[i] = Vreal[i] / ((1 + Modulo(i)) / frecuencia);
                    Vimag[i] = Vimag[i] / ((1 + Modulo(i)) / frecuencia);
                }

            }
        }

        public void AplicaFiltroPasoAlto(int frecuencia)
        {
            if (isFFT)
            {
                for (int i = Vreal.Length / 2 - frecuencia; i < Vreal.Length / 2 + frecuencia; i++)
                {

                    Vreal[i] = 0;
                    Vimag[i] = 0;
                }

            }
        }

        private double Modulo(int i)
        {
            return Math.Sqrt(Vreal[i] * Vreal[i] + Vimag[i] * Vimag[i]);
        }

        public void CreaEspectro()
        {
            if (isFFT)
            {
                
                Espectro = new int[Vreal.Length];
                for (int i = 0; i < Vreal.Length; i++)
                {
                    int indice=(int)Vreal[i];
                    if (indice>0)
                        Espectro[i] = (int)Modulo(i);
                }//fin for

            }//fin if
            EspectroCreado = true;
        }

        public int EspectroPaso(int paso, int ancho)
        {
            int dev=0;
            if (EspectroCreado)
            {
                double zancada = (double)Espectro.Length / (double)ancho;
                for (int i = Convert.ToInt32(Math.Floor(paso * zancada)); i < Espectro.Length && i <= Convert.ToInt32(Math.Floor((paso + 1) * zancada)); i++)
                {
                    if (i != 0)
                    {
                        dev += Espectro[i];
                        if (Espectromax < dev)
                            Espectromax = dev;
                    }
                    else
                        dev += 0; 
                }

            }
            return dev;
        }

        public int FrecuenciaMaxima()
        {
            int ret=0;
            int ind = 0;
            for (int i = 0; i < Vreal.Length; i++)
                if (Vreal[i] > ret)
                {
                    ret = Convert.ToInt32(Vreal[i]);
                    ind = i;
                }

            return ind;
        }

        public int numeroFrecuencias()
        {
            int ret = 0;
            int ind = 0;
            for (int i = 0; i < Espectro.Length; i++)
                if (Espectro[i] > 0)
                {
                    ind++;
                }

            return ind;
        }

        private double ToByte(double numero, int etapa, int max)
        {
            double res = numero;
            int paso = max;



            if (res > 255)
            {
                while (paso > etapa)
                {
                    res = res - (numero % Exp256(paso)) * Exp256(paso);
                    paso--;
                }
                res = res / Exp256(paso);


                if (res > 255)
                    res = 255;

            }




            return res;
        }

        private int SizeV2N(int tamaño)
        {
            int res = 0;

            while (Exp2(res) < tamaño)
                res++;


            return Exp2(res);
        }


        private int Exp256(int exponente)
        {
            int res = 1;
            for (int i = 0; i < exponente; i++)
            {
                res = 256 * res;
            }
            return res;
        }

        private int Exp2(int exponente)
        {
            int res = 1;
            for (int i = 0; i < exponente; i++)
            {
                res = 2 * res;
            }
            return res;
        }


        private void fft(ref double[] x, ref double[] y,int dir)
            {
            long m = Convert.ToInt32(Math.Log(x.Length, 2)); ;
            long n,i,i1,j,k,i2,l,l1,l2;
            double c1,c2,tx,ty,t1,t2,u1,u2,z;

            /* Calculate the number of points */
            n = x.Length;

            /* Do the bit reversal */
            i2 = n >> 1;
            j = 0;
            for (i=0;i<n-1;i++) {
                if (i < j) {
                    tx = x[i];
                    ty = y[i];
                    x[i] = x[j];
                    y[i] = y[j];
                    x[j] = tx;
                    y[j] = ty;
                }
                k = i2;
                while (k <= j && !(k==0 && j==0)) {
                    j -= k;
                    k >>= 1;
                }
                j += k;
            }

            /* Compute the FFT */
            c1 = -1.0; 
            c2 = 0.0;
            l2 = 1;
            for (l=0;l<m;l++) {
                l1 = l2;
                l2 <<= 1;
                u1 = 1.0; 
                u2 = 0.0;
                for (j=0;j<l1;j++) {
                    for (i=j;i<n;i+=l2) {
                        i1 = i + l1;
                        if (i1 >= x.Length || i1 < 0)
                            i1 = 0;
                        t1 = u1 * x[i1] - u2 * y[i1];
                        t2 = u1 * y[i1] + u2 * x[i1];
                        x[i1] = x[i] - t1; 
                        y[i1] = y[i] - t2;
                        x[i] += t1;
                        y[i] += t2;
                    }
                    z =  u1 * c1 - u2 * c2;
                    u2 = u1 * c2 + u2 * c1;
                    u1 = z;
                }
                c2 = Math.Sqrt((1.0 - c1) / 2.0);
                if (dir == 1) 
                    c2 = -c2;
                c1 = Math.Sqrt((1.0 + c1) / 2.0);
            }

            /* Scaling for forward transform */
            if (dir == 1) {
                for (i=0;i<n;i++) {
                    x[i] /= n;
                    y[i] /= n;
                    if ((x[i]) > FFTRmax)
                        FFTRmax = x[i];
                    if ((x[i]) < FFTRmin)
                        FFTRmin = x[i];
                    
                }
            }
        }




        private void fft2(ref double[] real, ref double[] imag, int sign)
        {

            int n, m, m2, i, j, k, l;
            double c1, c2, norm, norm2, cphi, sphi;

            int log2n = Convert.ToInt32(Math.Log(real.Length, 2));

            n = 1 << log2n;

            /* Inversion de los bits */
            for (i = 0; i < n; i++)
            {

                for (j = log2n - 1, m = 0, k = i; j >= 0; j--, k >>= 1) m += (k & 1) << j;

                if (m > i)
                {
                    double aux = real[i];
                    real[i] = real[m];
                    real[m] = aux;

                    aux = imag[i];
                    imag[i] = imag[m];
                    imag[m] = aux;


                }
            }

            /* normalizacion ¿es necesario? */
            norm = 1.0 / Math.Sqrt((double)n);
            for (i = 0; i < n; i++)
            {
                real[i] *= norm;
                imag[i] *= norm;
            }

            /* calculo de la FFT */
            for (j = 0; j < log2n; j++)
            {
                m = 1 << j;
                m2 = 2 * m;
                c1 = 1.0;
                c2 = 0.0;
                cphi = Math.Cos(sign * 2.0 * Math.PI / ((double)m2));
                sphi = Math.Sin(sign * 2.0 * Math.PI / ((double)m2));
                for (k = 0; k < m; k++)
                {
                    for (i = k; i < n; i += m2)
                    {
                        l = i + m;
                        norm = c1 * real[l] - c2 * imag[l];
                        norm2 = c1 * imag[l] + c2 * real[l];
                        real[l] = real[i] - norm;
                        imag[l] = imag[i] - norm2;
                        real[i] += norm;
                        if ((real[i]) > FFTRmax)
                            FFTRmax = real[i];
                        if ((real[i]) < FFTRmin)
                            FFTRmin = real[i];
                        if ((real[l]) > FFTRmax)
                            FFTRmax = real[l];
                        if ((real[l]) < FFTRmin)
                            FFTRmin = real[l];

                        imag[i] += norm2;
                    }
                    
                    norm = c1 * cphi - c2 * sphi;
                    norm2 = c1 * sphi + c2 * cphi;
                    c1 = norm; c2 = norm2;
                    
                }
            }

        }
    }
}
