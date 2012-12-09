using System;
using System.Collections.Generic;
using System.Text;

namespace scriptASS
{
    public class StyleException : PerrySubException
    {
        public StyleException() : base() { }
        public StyleException(string m) : base(m) { }
    }

    class estiloV4 : IComparable
    {
        #region Variables Locales
        private string name;
        private string fontname;
        private int fontsize;

        private string prim_color;
        private string sec_color;
        private string out_color;
        private string back_color;
        
        private bool bold;
        private bool italic;
        private bool underline;
        private bool strikeout;
        
        private double scaleX;
        private double scaleY;
        private double spacing;
        private double angle;

        private bool borderstyle;
        private double outline;
        private double shadow;
        private int alignment;

        private int marginL;
        private int marginR;
        private int marginV;

        private int encoding;
        #endregion

        #region Variables globales

        public string Name
        {
            get { return name; }
            set { name = value.Replace(',', '.'); }
        }

        public string FontName
        {
            get { return fontname; }
            set { fontname = value.Replace(',', '.'); }
        }

        public int FontSize
        {
            get { return fontsize; }
            set { fontsize = value; }
        }

        public string PrimaryColor
        {
            get { return prim_color; }
            set { prim_color = value; }
        }
        public string SecondaryColor
        {
            get { return sec_color; }
            set { sec_color = value; }
        }

        public string OutlineColor
        {
            get { return out_color; }
            set { out_color = value; }
        }

        public string ShadowColor
        {
            get { return back_color; }
            set { back_color = value; }
        }

        public bool Bold
        {
            get { return bold; }
            set { bold = value; }
        }

        public bool Italic
        {
            get { return italic; }
            set { italic = value; }
        }

        public bool Underline
        {
            get { return underline; }
            set { underline = value; }
        }

        public bool Strikeout
        {
            get { return strikeout; }
            set { strikeout = value; }
        }

        public double ScaleX
        {
            get { return scaleX; }
            set { scaleX = value; }
        }

        public double ScaleY
        {
            get { return scaleY; }
            set { scaleY = value; }
        }


        public double Spacing
        {
            get { return spacing; }
            set { spacing = value; }
        }

        public double Angle
        {
            get { return angle; }
            set { angle = value; }
        }


        public bool Border
        {
            get { return borderstyle; }
            set { borderstyle = value; }
        }
        

        public double Outline
        {
            get { return outline; }
            set { outline = value; }
        }


        public double Shadow
        {
            get { return shadow; }
            set { shadow = value; }
        }

        public int Alignment
        {
            get { return alignment; }
            set { alignment = value; }
        }

        public int MarginL
        {
            get { return marginL; }
            set { marginL = value; }
        }

        public int MarginR
        {
            get { return marginR; }
            set { marginR = value; }
        }

        public int MarginV
        {
            get { return marginV; }
            set { marginV = value; }
        }

        public int Encoding
        {
            get { return encoding; }
            set { encoding = value; }
        }

        #endregion


        public estiloV4()
        {
            loadStyle("Default,Arial,20,&H00FFFFFF,&H0000FFFF,&H00000000,&H00000000,0,0,0,0,100,100,0.00,0.00,1,2.00,2.00,2,10,10,10,0");
        }

        public estiloV4(string linea)
        {
            try
            {
                loadStyle(linea);
            }
            catch { throw new StyleException("Excepción de estilos - Comprueba sintaxis en la línea:\n" + linea); }
        }

        private void loadStyle(string linea)
        {
            string l = linea.Substring(linea.IndexOf(" ") + 1);
            string[] la = l.Split(',');

            if (la.Length > 20) parseV4plus(la);
            else parseV4(la);
        }

        private void parseV4(string[] la) // SSA
        {
            name = la[0];
            fontname = la[1]; fontsize = int.Parse(la[2]);
            bold = Convert.ToBoolean(int.Parse(la[7])); italic = Convert.ToBoolean(int.Parse(la[8]));
            borderstyle = Convert.ToBoolean(int.Parse(la[9]));

            outline = s2d(la[10]); shadow = s2d(la[11]); alignment = int.Parse(la[12]);
            marginL = int.Parse(la[13]); marginR = int.Parse(la[14]); marginV = int.Parse(la[15]);
            encoding = int.Parse(la[17]);

            // no estan en los tags ssa, ponemos unos valores por defecto

            underline = false; strikeout = false;
            scaleX = 100; scaleY = 100;
            spacing = 0; angle = 0;

            // FIX

            int a = int.Parse(la[16]);            
            string alpha = a.ToString("X");
            if (alpha.Length == 1) alpha = "0" + alpha;

            prim_color = "&H" + alpha + ToHexNumber(int.Parse(la[3]),6);
            sec_color = "&H" + alpha + ToHexNumber(int.Parse(la[4]),6);
            out_color = "&H" + alpha + ToHexNumber(int.Parse(la[6]),6);
            back_color = "&H" + alpha + ToHexNumber(int.Parse(la[5]),6);

        }

        private void parseV4plus(string[] la) // ASS
        {
            name = la[0];
            fontname = la[1]; fontsize = int.Parse(la[2]);

            if (la[3].StartsWith("&H")) prim_color = la[3];
            else prim_color = "&H" + ToHexNumber(int.Parse(la[3]), 8);
            
            if (la[4].StartsWith("&H")) sec_color = la[4];
            else sec_color = "&H" + ToHexNumber(int.Parse(la[4]), 8);
            
            if (la[5].StartsWith("&H")) out_color = la[5];
            else out_color = "&H" + ToHexNumber(int.Parse(la[5]), 8);

            if (la[6].StartsWith("&H")) back_color = la[6];
            else back_color = "&H" + ToHexNumber(int.Parse(la[6]), 8);
            
            
            bold = Convert.ToBoolean(int.Parse(la[7])); italic = Convert.ToBoolean(int.Parse(la[8]));
            underline = Convert.ToBoolean(int.Parse(la[9])); strikeout = Convert.ToBoolean(int.Parse(la[10]));

            scaleX = s2d(la[11]); scaleY = s2d(la[12]);
            spacing = s2d(la[13]); angle = s2d(la[14]);

            borderstyle = Convert.ToBoolean(int.Parse(la[15]));

            outline = s2d(la[16]); shadow = s2d(la[17]); alignment = int.Parse(la[18]);

            marginL = int.Parse(la[19]); marginR = int.Parse(la[20]); marginV = int.Parse(la[21]);
            encoding = int.Parse(la[22]);
        }


        private string ToHexNumber(int bleh,int n)
        {
            string temp = bleh.ToString("X");

            for (int i = temp.Length; i < n; i++)
                temp = "0" + temp;
            return temp;

        }

        public static double s2d(string s)
        {
            
            System.Globalization.CultureInfo objCI = new System.Globalization.CultureInfo("es-ES");
            System.Threading.Thread.CurrentThread.CurrentCulture = objCI;
            System.Threading.Thread.CurrentThread.CurrentUICulture = objCI;
            
            s = s.Replace('.', Convert.ToChar(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
            return Convert.ToDouble(s);
        }

        public static string d2s(double d)
        {
            
            System.Globalization.CultureInfo objCI = new System.Globalization.CultureInfo("es-ES");
            System.Threading.Thread.CurrentThread.CurrentCulture = objCI;
            System.Threading.Thread.CurrentThread.CurrentUICulture = objCI;
            
            string res = Convert.ToString(d);
            res = res.Replace(Convert.ToChar(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator), '.');            
            return res;

        }

        public override string ToString()
        {
            string res = "Style: "+name + "," + fontname + "," + fontsize + "," + prim_color + "," + sec_color + "," + out_color + "," + back_color + ",";
            res += Convert.ToInt32(bold) + "," + Convert.ToInt32(italic) + "," + Convert.ToInt32(underline) + "," + Convert.ToInt32(strikeout) + "," + estiloV4.d2s(scaleX) + "," + estiloV4.d2s(scaleY) + "," + estiloV4.d2s(spacing) + "," + estiloV4.d2s(angle) + ",";
            res += Convert.ToInt32(borderstyle) + "," + estiloV4.d2s(outline) + "," + estiloV4.d2s(shadow) + "," + alignment + "," + marginL + "," + marginR + "," + marginV + "," + encoding;
            return res;
        }


        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is estiloV4)
            {
                estiloV4 otro = (estiloV4)obj;
                if (otro.Name == name && otro.FontName == fontname && otro.PrimaryColor == prim_color && otro.SecondaryColor == sec_color && otro.OutlineColor == out_color &&
                    otro.ShadowColor == back_color && otro.Bold == bold && otro.Italic == italic && otro.Underline == underline && otro.Strikeout == strikeout &&
                    otro.ScaleX == scaleX && otro.ScaleY == scaleY && otro.Spacing == spacing && otro.Angle == angle && otro.Border == borderstyle && otro.Outline == outline &&
                    otro.Shadow == shadow && otro.Alignment == alignment && otro.MarginL == marginL && otro.MarginR == marginR && otro.MarginV == marginV && otro.Encoding == encoding)
                {
                    return 0;
                }
                return -1; // default
            }
            else throw new ArgumentException("object is not estiloV4");
        }

        #endregion
    }
}
