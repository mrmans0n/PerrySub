using System;
using System.Collections.Generic;
using System.Text;

namespace scriptASS
{
    class lineaASSidx : lineaASS
    {
        private int idx;

        public int Index
        {
            get { return idx; }
            set { idx = value; }
        }

        public lineaASSidx(string linea, int idx)
        {
            base.loadLine(linea);
            this.idx = idx;
        }
    }
}
