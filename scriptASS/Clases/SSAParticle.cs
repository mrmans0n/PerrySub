using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace scriptASS.Clases
{
    class SSAParticle: IDisposable
    {
        //Particula sencilla forma + posicion + vida
        string[] forma;
        Point posicion;
        double life;
        double birth;
        double rate;
        //Un poco más compleja con peso y velocidad
        int alfa;
        double peso;
        Point velocidad;
        //La complicamos más con aceleracion y momento angular
        Point aceleracion;
        double dextrogiro;
        //añadimos el dispose
        bool alreadyDisposed=false;

        public SSAParticle(double b)
        {
            posicion = new Point(0, 0);
            alfa = 0;
            forma = new string[1];
            forma[0]="{\\1c&H000000&\\p1}m 0 0 l 1 0 l 1 1 l 0 1 c";
            peso=1.0;
            life = 100.0;
            birth = b;
            rate = 0.1;
        }

        public SSAParticle(Point pos, int alpha, string[] formas, double w, double l, double b, double r)
        {
            posicion = pos;
            alfa = alpha;
            forma = formas;
            peso = w;
            life = l;
            birth = b;
            rate = r;
        }

         public void Dispose(bool llamadaExplicita)
         {
          if(!this.alreadyDisposed)
          {
           if(llamadaExplicita)
           {
              
           }

          }
          alreadyDisposed = true;   
         }

         public void Dispose()
         {
          Dispose(true);
         }

        ~SSAParticle()
        {

        }

        public void SetPos(Point pos)
        {
            posicion = pos;
        }

        public Point GetPos()
        {
            return posicion;
        }

        public void SetForma(string[] forma)
        {
            this.forma = forma;
        }

        public void SetPeso(double w)
        {
            peso = w;
        }

        public double GetPeso()
        {
            return peso;
        }
        public void SetLife(double l)
        {
            life = l;
        }

        public double GetLife()
        {
            return life;
        }

        public double GetBirth()
        {
            return birth;
        }

        public string[] Show(double time)
        {
            string[] ret = new string[forma.Length];
            if (time < birth + life && time > birth && life > 0)
            {
                Random r= new Random();
                double indeterminismo = Math.Max(0.9,Math.Abs(r.NextDouble()%1.1));
                
                for (int i = 0; i < forma.Length; i++)
                {
                    string X=(double)posicion.X/100/indeterminismo + "";
                    string Y=(double)posicion.Y/100/indeterminismo + "";
                    ret[i] = "{\\pos(" + X.Replace(Convert.ToChar(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator), '.') + "," + Y.Replace(Convert.ToChar(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator), '.') + ")" + forma[i];
                }

            }

            life -= rate;
            return ret;
        }

        public int NumLineas()
        {
            return forma.Length;
        }

        
    }
}
