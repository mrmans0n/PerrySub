using System;
using System.Collections.Generic;
using System.Text;

namespace scriptASS
{
    public enum TimeFormat : int
    {
        Full = 0,
        Simple = 1
    }

    public class Tiempo : ICloneable, IComparable   
    {
        private double T;
        public double t
        {
            get { return T; }
            set { T = Math.Max(0,value); }
        }

        public Tiempo(string s)
        {
            t = str2double(s);  
        }

        public Tiempo(double d)
        {
            t = d;
        }

        public Tiempo()
        {
            t = 0.0;
        }

        public void setTiempo(string t1)
        {
            t = str2double(t1);
        }

        public void setTiempo(double t2)
        {
            t = t2;
        }

        public double getTiempo()
        {
            return t;
        }

        public void sumaTiempo(string t1)
        {
            t += str2double(t1);
            if (t<0) t=0;
        }

        public void restaTiempo(string t1)
        {
            double d = str2double(t1);
            if (d > t) t = 0;
            else t -= d;
        }

        public void sumaTiempo(double d)
        {
            t += d;
        }

        public void restaTiempo(double d)
        {
            if (d > t) t = 0;
            else t -= d;
        }


        private double str2double(String s)
        { 
            return Tiempo.TimeToSecondDouble(s);
        }

        private string double2str(double d, TimeFormat formato)
        {
            return (formato == TimeFormat.Full) ? Tiempo.SecondToTimeString(d) : Tiempo.SecondToTimeStringRedux(d);
        }

        public static double TimeToSecondDouble(string s)
        {
            int hora, minuto;
            double segundo=0;
            String[] str = s.Split(':');
            hora = Convert.ToInt32(str[0]);
            minuto = Convert.ToInt32(str[1]);
            segundo = Convert.ToDouble(str[2].Replace('.', ','));
            //segundo = Convert.ToDouble(str[2].Replace(Convert.ToChar(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator), '.'));
            

            double res = (hora * 60 * 60 + minuto * 60 + segundo);

            return res;
        }


        public static string SecondToTimeStringRedux(double d)
        {
            String h, m, s1, ms1;

            ConversionInfo(d, out h, out m, out s1, out ms1);

            return h + ":" + m + ":" + s1;
        }

        public static string SecondToTimeString(double d)
        {
            String h, m, s1, ms1;

            ConversionInfo(d, out h, out m, out s1, out ms1);

            return h + ":" + m + ":" + s1 + "." + ms1;
        }

        private static void ConversionInfo(double d, out string h, out string m, out string s1, out string ms1)
        {
            int hora, minuto, s, ms;
            //String h, m, s1, ms1;
            double segundo;

            hora = Convert.ToInt32(Math.Truncate((d / 3600)));
            minuto = Convert.ToInt32(Math.Truncate(((d - (3600 * hora)) / 60)));
            segundo = (d - (hora * 3600) - (minuto * 60));
            segundo = Math.Round(segundo, 2); //bugfix o_O

            s = Convert.ToInt32(Math.Truncate(segundo));
            ms = Convert.ToInt32((segundo - s) * 100);

            //if (hora < 10) h = "0" + hora;
            //else 
                h = Convert.ToString(hora);

            if (minuto < 10) m = "0" + minuto;
            else m = Convert.ToString(minuto);
            if (s < 10) s1 = "0" + s;
            else s1 = Convert.ToString(s);

            ms1 = Convert.ToString(ms);
            if (ms < 10) ms1 = "0" + ms1;
        }

        public override String ToString()
        {

            return double2str(t, TimeFormat.Full);
        }

        #region Miembros de ICloneable

        public object Clone()
        {
            return new Tiempo(this.ToString());
        }

        #endregion

        #region Miembros de IComparable

        public int CompareTo(object obj)
        {
            Tiempo tobj = (Tiempo)obj;
            return t.CompareTo(tobj);
        }

        #endregion
    }
}
