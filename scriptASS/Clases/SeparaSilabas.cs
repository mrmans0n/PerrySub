using System;
using System.Collections.Generic;
using System.Text;

namespace scriptASS
{
    class SeparaSilabas
    {
        string[] conocidas = { 
            "a", "i", "u", "e", "o",
            "ka", "ki", "ku", "ke", "ko",
            "ta", "chi", "tsu", "te", "to",
            "da", "di", "du", "de", "do",
            "sa", "shi", "su", "se", "so",
            "ha", "hi", "fu", "he", "ho",
            "ba", "bi", "bu", "be", "bo",
            "pa", "pi", "pu", "pe", "po",
            "ga", "gi", "gu", "ge", "go",
            "za", "zi", "zu", "ze", "zo",
            "ma", "mi", "mu", "me", "mo",
            "na", "ni", "nu", "ne", "no",
            "ra", "ri", "ru", "re", "ro",
            "ya", "yu", "yo",
            "wa", "wo", "n", 
            "sha", "sho", "shu",
            "cha", "cho", "chu",
            "kya", "kyo", "kyu",
            "gya", "gyo", "gyu",
            "bya", "byo", "byu",
            "pya", "pyo", "pyu",
            "tu", "si", "dzu"
            };

        string toSplit;

        public SeparaSilabas(string s)
        {
            toSplit = s;
        }

        public string Separa()
        {
            string res = "";
            string temp = toSplit;

            while (temp.Length > 0)
            {
                int size = 1;
                if (esConocida(temp, out size))
                    res += temp.Substring(0, size) + "|";
                else if (temp.StartsWith(" "))
                    res += " ";
                else
                    res += temp.Substring(0, 1) + "|";
                temp = temp.Substring(size);
            }

            return res.Substring(0, res.Length - 1);

        }

        private bool esConocida(string s, out int size)
        {
            bool tmp = false;
            for (int i = 0; i < conocidas.Length; i++)
            {
                if (s.StartsWith(conocidas[i].ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    size = conocidas[i].ToString().Length;
                    return true;
                }
            }
            size = 1;
            return tmp;
        }

    }
}
