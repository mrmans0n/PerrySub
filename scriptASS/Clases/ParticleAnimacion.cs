using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace scriptASS.Clases
{
    class ParticleAnimacion
    {
        public enum Tipo : int 
        {
            Mover,
            Destruir,
            Fin
        };

        public static string[] NombreTipo ={ "Mover", "Destruir", "Fin" };
        public static int[] ValTipo =      {     2,        1,        0  };
        public static string[] NombreMover ={ "X", "Y" };
        public static string[] NombreDestruir ={ "Tiempo" };
        public static string[] NombreFin ={ "Fin" };
        public static string[][] LayersTipo ={ NombreMover, NombreDestruir, NombreFin };

        Tipo Atype;
        object[] parametros;

        public ParticleAnimacion(Tipo t, params object[] paramlist)
        {
            switch (t)
            {
                case Tipo.Mover:
                    if (paramlist.Length == 2)
                    {
                        Point P = new Point(0, 0);
                        parametros = new object[paramlist.Length];
                        Atype = t;
                        P.X = (int)paramlist[0];
                        P.Y = (int)paramlist[1];
                        parametros[0] = P;
                        
                    }
                    break;
                case Tipo.Destruir:
                    if (paramlist.Length == 1)
                    {

                        parametros = new object[paramlist.Length];
                        Atype = t;
                        parametros[0] = paramlist[0];

                    }
                    break;
                default:
                    //usamos una excepcion del sistema para indicar que no se ha realizado correctamente la asignacion
                    throw new Exception("ParticleAnimation: llamada al constructor no valida");
                    break;
            }
        }

        public void ApplyAnimacion(ref SSAParticle p)
        {
            switch (Atype)
            {
                case Tipo.Mover:
                    Point posicion = p.GetPos();
                    posicion.X += ((Point)parametros[0]).X;
                    posicion.Y += ((Point)parametros[0]).Y;
                    p.SetPos(posicion);
                    break;
                case Tipo.Destruir:
                    double tiempoD=((double)parametros[0]);
                    p.SetLife(p.GetBirth()+tiempoD);
                    break;
                
                default:
                    break;
            }
        }

    }
}
