using System;
using System.Collections.Generic;
using System.Text;

namespace scriptASS
{
    class K
    {
        private int nlinea;
        private string tipoK;
        private int milis;
        private string texto;
        
        public int LineIndex
        {
            get { return nlinea; }
            set { nlinea = value; }
        }

        public string StyleK
        {
            get { return tipoK; }
            set { tipoK = value; }
        }

        public int Milliseconds
        {
            get { return milis; }
            set { milis = value; }
        }

        public string Text
        {
            get { return texto; }
            set { texto = value; }
        }

        public K(int n, string k, int m, string t)
        {
            nlinea = n;
            tipoK = k;
            milis = m;
            texto = t;
        }

        public override string ToString()
        {
            return "{\\" + StyleK + Milliseconds + "}" + texto;
        }

    }
}
