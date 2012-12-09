using System;
using System.Collections.Generic;
using System.Text;

namespace scriptASS
{
    class EfectosV1
    {

        public string nombre; 
        public string invariante;
        public string ebase;
        public string textra;

        public string efecto1;
        public int tefecto1;
        public int tade1;
        public int tatr1;

        public string efecto2;
        public int tefecto2;
        public int tade2;
        public int tatr2;

        public string efecto3;
        public int tefecto3;
        public int tade3;
        public int tatr3;

        public string efecto4;
        public int tefecto4;
        public int tade4;
        public int tatr4;

        public string efecto5;
        public int tefecto5;
        public int tade5;
        public int tatr5;

        private double tiempoEant;
        private int lineaActual;

        public EfectosV1()
        {
            tiempoEant=0;
            lineaActual = 0;
        }


        public String AplicaASilaba(Karaoke_K silaba)
        {
            //Copiando tal cual el php de la V1 espero no meter la pata...

            String ret = "";
            int tiempo = silaba.Milliseconds;
            String elemento = silaba.Text;
            if (elemento=="0")
            {
            	elemento="";
            }
            //Ya que esta lo uso separo \k de \K
            int FormatoK;
            if (silaba.StyleK == "k") //espero que esto sea asi.
                FormatoK = 1;
            else
                FormatoK = 2;


            //Esto son los tiempos.
            double EEE1=0, EEE2=0, EEE3=0, EEE4=0, EEE5 = 0;
            double EE1=0, EE2=0, EE3=0, EE4=0, EE5 = 0;

            //Empezamos.
            //Comrpuebo si la linea actual de la silaba es la misma que la que he estado tratando anteriormente.
            //Si no lo es tiempoEAnt=0

            if (lineaActual != silaba.LineIndex)
            {
                lineaActual = silaba.LineIndex;
                tiempoEant = 0;
            }

            //porcentajes


            double tiempoE1 = Convert.ToDouble(tiempo * tefecto1 / 10.0 + tiempoEant);
            double tiempoE2 = Convert.ToDouble(tiempo * tefecto2 / 10.0 + tiempoE1);
            double tiempoE3 = Convert.ToDouble(tiempo * tefecto3 / 10.0 + tiempoE2);
            double tiempoE4 = Convert.ToDouble(tiempo * tefecto4 / 10.0 + tiempoE3);
            double tiempoE5 = Convert.ToDouble(tiempo * tefecto5 / 10.0 + tiempoE4);
            int tiempoMitad = tiempo / 2;

            //Ahora que dios coja confesado al padre. Al hijo y al espiritu santo.
            //Copiado tal cual de el php lo que implica que la puedo estar cagando cosa mala

            // Posible error confusion tade con tatr


            EEE1 = tiempoEant + tade1;
            EEE2 = tiempoE1 + tade2;
            EEE3 = tiempoE2 + tade3;
            EEE4 = tiempoE3 + tade4;
            EEE5 = tiempoE4 + tade5;

            EE1 = tiempoE1 + tatr1;
            EE2 = tiempoE2 + tatr2;
            EE3 = tiempoE3 + tatr3;
            EE4 = tiempoE4 + tatr4;
            EE5 = tiempoE5 + tatr5;


            if (EE1 < EEE1)
                EE1 = EEE1 + tiempoE1;
            if (EE2 < EEE2)
                EE2 = EEE2 + tiempoE2;
            if (EE3 < EEE3)
                EE3 = EEE3 + tiempoE3;
            if (EE4 < EEE4)
                EE4 = EEE4 + tiempoE4;
            if (EE5 < EEE5)
                EE5 = EEE5 + tiempoE5;

            //Ya estan los tiempos ahora se aplican a la silaba veras que es facil ,Manué
            if (tade1 < 0)
            {
                EEE1 += Math.Abs(tade1);
                EEE2 += Math.Abs(tade1);
                EEE3 += Math.Abs(tade1);
                EEE4 += Math.Abs(tade1);
                EEE5 += Math.Abs(tade1);
            }

            String NEWtexto = "{\\r" + invariante;
            if (efecto1 != "")
            {
                NEWtexto = NEWtexto + "\\t(" + estiloV4.d2s(EEE1) + "," + estiloV4.d2s(EE1) + "," + efecto1 + ")";
            }
            if (efecto2 != "")
            {
                NEWtexto = NEWtexto + "\\t(" + estiloV4.d2s(EEE2) + "," + estiloV4.d2s(EE2) + "," + efecto2 + ")";
            }
            if (efecto3 != "")
            {
                NEWtexto = NEWtexto + "\\t(" + estiloV4.d2s(EEE3) + "," + estiloV4.d2s(EE3) + "," + efecto3 + ")";
            }
            if (efecto4 != "")
            {
                NEWtexto = NEWtexto + "\\t(" + estiloV4.d2s(EEE4) + "," + estiloV4.d2s(EE4) + "," + efecto4 + ")";
            }
            if (efecto5 != "")
            {
                NEWtexto = NEWtexto + "\\t(" + estiloV4.d2s(EEE5) + "," + estiloV4.d2s(EE5) + "," + efecto5 + ")";
            }
            if (FormatoK == 1)
            {
                NEWtexto = NEWtexto + "\\k" + tiempo + "}" + elemento;
            }
            if (FormatoK == 2)
            {
                NEWtexto = NEWtexto + "\\K" + tiempo + "}" + elemento;
            }
            tiempoEant = tiempoE5;

            // y devolvemos el string
            ret = NEWtexto;

            return (ret);
        }

        public String AplicaCasoBase()
        {
            return (ebase);
        }

        public override string ToString()
        {
            string res = "Nombre:" + nombre + "\n";
            res += "Invariante:" + invariante + "\n";
            res += "Ebase:" + ebase + "\n";
            res += "Textra:" + textra + "\n";

            res += "Efecto1:" + efecto1 + "\n";
            res += "Tefecto1:" + tefecto1 + "\n";
            res += "Tade1:" + tade1 + "\n";
            res += "Tatr1:" + tatr1 + "\n";

            res += "Efecto2:" + efecto2 + "\n";
            res += "Tefecto2:" + tefecto2 + "\n";
            res += "Tade2:" + tade2 + "\n";
            res += "Tatr2:" + tatr2 + "\n";
            
            res += "Efecto3:" + efecto3 + "\n";
            res += "Tefecto3:" + tefecto3 + "\n";
            res += "Tade3:" + tade3 + "\n";
            res += "Tatr3:" + tatr3 + "\n";
            
            res += "Efecto4:" + efecto4 + "\n";
            res += "Tefecto4:" + tefecto4 + "\n";
            res += "Tade4:" + tade4 + "\n";
            res += "Tatr4:" + tatr4 + "\n";
            
            res += "Efecto5:" + efecto5 + "\n";
            res += "Tefecto5:" + tefecto5 + "\n";
            res += "Tade5:" + tade5 + "\n";
            res += "Tatr5:" + tatr5 + "\n";

            return res;
        }

    }
}
