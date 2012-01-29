using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace scriptASS
{
    public class LineException : PerrySubException
    {
        public LineException() : base() { }
        public LineException(string m) : base(m) { }
    }

    public class lineaASS : IComparable,ICloneable
    {
        public String clase;
        public int colision;
        public Tiempo t_inicial;
        public Tiempo t_final;
        private string Estilo;

        public string estilo
        {
            get { return Estilo; }
            set { Estilo = value.Replace(',','.'); }
        }
        private string Personaje; //

        public string personaje
        {
            get { return Personaje; }
            set { Personaje = value.Replace(',', '.'); }
        }
        
        public int izquierda;
        public int derecha;
        public int vertical;
        public String tipo;
        public String texto;

        /*
        public lineaASS(String c, int col, String t_i, String t_f, String est, String pj, int iz, int dc, int vt, String tipo, String text)
        {
            this.clase = c; this.colision = col; this.t_inicial = new Tiempo(t_i); this.t_final = new Tiempo(t_f);
            this.estilo = est; this.personaje = pj; this.izquierda = iz; this.derecha = dc;
            this.vertical = vt; this.tipo = tipo; this.texto = text;
        }
        */

        public lineaASS()
        {
            this.clase = "Dialogue"; this.colision = 0; this.t_inicial = new Tiempo(); this.t_final = new Tiempo();
            this.estilo = "Default"; this.personaje = ""; this.izquierda = 0; this.derecha = 0;
            this.vertical = 0; this.tipo = ""; this.texto = "";
        }

        public lineaASS(String t_i, String t_f, String est, String text)
        {
            this.clase = "Dialogue"; this.colision = 0; this.t_inicial = new Tiempo(t_i); this.t_final = new Tiempo(t_f);
            this.estilo = est; this.personaje = est; this.izquierda = 0; this.derecha = 0;
            this.vertical = 0; this.tipo = ""; this.texto = text;
        }

        public lineaASS(String s)
        {
            loadLine(s);
        }
        
        /* ASS
         * Dialogue: 0,00:00:12.48,00:00:13.38,Default,,0000,0000,0000,,meeeemb
         * SSA
         * Dialogue: Marked=0,0:00:12.99,0:00:13.51,T_Varios,o,0000,0000,0000,,Azul...
         */

        public void loadLine(String linea)
        {
            try
            {
                int posIn = linea.IndexOf(": ") + 2;
                String brau = linea.Substring(posIn);
                String cl = linea.Substring(0, (linea.IndexOf(": ")));
                String[] la = brau.Split(new Char[] { ',' }, 10);

                int col = 0;
                if (la[0].StartsWith("Marked=")) col = Convert.ToInt32(la[0].Remove(0, 7));
                else col = Convert.ToInt32(la[0]);

                int iz_ = Convert.ToInt32(la[5]);
                int dc_ = Convert.ToInt32(la[6]);
                int vt_ = Convert.ToInt32(la[7]);
                clase = cl; colision = col;

                t_inicial = new Tiempo(la[1]); t_final = new Tiempo(la[2]);

                estilo = la[3]; personaje = la[4];
                izquierda = iz_; derecha = dc_; vertical = vt_; tipo = la[8]; texto = la[9];
            }
            catch { throw new LineException("Excepción de SSA/ASS - Comprueba sintaxis en la línea:\n" + linea); }
        }

        public int CompareTo(object obj)
        {
            if (obj is lineaASS)
            {
                lineaASS tmp = (lineaASS)obj;
                return t_final.getTiempo().CompareTo(tmp.t_final.getTiempo());
            }
            else throw new ArgumentException("object is not lineaASS");
        }

        public static string i2ns(int i, int n)
        {
            string res = "";
            for (int x = n; x > 0; x--)
            {
                int t = (int)(i / Math.Pow(10, x - 1));
                i = i - (int)(t * Math.Pow(10, x - 1));
                res += t.ToString();

            }
            return res;
        }

        public static string cleanText(string orig)
        {
            string temp = orig;

                do
                {
                    int idxOpn = temp.IndexOf('{');
                    int idxCls = temp.IndexOf('}');
                    if ((idxOpn > idxCls) || (idxOpn == -1)) break;
                    temp = temp.Remove(idxOpn, (idxCls - idxOpn) + 1);
                } while (temp.IndexOf('{') != -1);


            return temp;
        }


        public static string insertTag(string texto, string tag, int index)
        {
            string res = "";
            Regex r = new Regex(@"\{\S+?\}");
            MatchCollection col = r.Matches(texto);
            bool isInOrNear = false;

            foreach (Match m in col)
            {
                int inicio = m.Index;
                int fin = inicio + m.Length;

                if (index >= inicio && index <= fin)
                {
                    string old_tag = texto.Substring(inicio + 1, m.Length - 2);
                    if (!old_tag.StartsWith("\\" + tag, StringComparison.InvariantCultureIgnoreCase))
                    {
                        string new_tag = "{\\" + tag + old_tag + "}";
                        texto = texto.Substring(0, inicio) + new_tag + texto.Substring(fin);
                    }
                    isInOrNear = true;
                    break;
                }

            }

            if (!isInOrNear)
            {
                res = texto.Substring(0, index) + "{\\" + tag + "}" + texto.Substring(index);
            }
            else res = texto;

            return res;
        }

        public bool IsComment()
        {
            return (clase.Equals("Comment", StringComparison.InvariantCultureIgnoreCase));
        }

        public object Clone()
        {
            return new lineaASS(this.ToString());
        }

        public override String ToString()
        {
            return clase + ": " + colision + "," + t_inicial + "," + t_final + "," + estilo + "," + personaje + "," + lineaASS.i2ns(izquierda, 4) + "," + lineaASS.i2ns(derecha, 4) + "," + lineaASS.i2ns(vertical, 4) + "," + tipo + "," + texto;
        }        
    }
}
