using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace scriptASS
{
    public class LineArrayList
    {
        private ArrayList Lines;
        public static int LineArrayListMax = 50000;
        public int LineArrayIndex = 0;

        public int Count
        {
            get { return Lines.Count; }
        }

        public LineArrayList()
        {
            Lines = new ArrayList();
        }

        public ArrayList GetFullArrayList()
        {
            return Lines;
        }

        public ArrayList GetLines()
        {
            int idx = LineArrayIndex * LineArrayListMax;
            return Lines.GetRange(idx, Math.Min(LineArrayListMax, Lines.Count - idx));
        }

        public void Trim()
        {
            Lines.TrimToSize();
        }

        public void Add(lineaASS o)
        {
            Lines.Add(o);
        }

        public void Clear()
        {
            Lines.Clear();
        }

    }
}
