using System;
using System.Collections.Generic;
using System.Text;

namespace scriptASS
{
    class Karaoke_K : K
    {
        private string kanjis = "";

        public string Kanjis
        {
            get { return kanjis; }
            set { kanjis = value; }
        }

        public Karaoke_K(int n, string k, int m, string t) : base(n,k,m,t)
        {          
        }

        public override string ToString()
        {
            return "{\\" + StyleK + Milliseconds + "}" + Kanjis;
        }

    }
}
