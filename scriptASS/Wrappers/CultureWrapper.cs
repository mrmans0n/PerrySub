using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace scriptASS
{
    class CultureWrapper
    {

        public static string GetLanguageName(string langstring)
        {
            string[] lang_country = langstring.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            CultureInfo[] cinfo = CultureInfo.GetCultures(CultureTypes.AllCultures);

            string first = lang_country[0] + "-" + lang_country[1];

            foreach (CultureInfo ci in cinfo)
            {
                if (ci.IetfLanguageTag == first)
                {
                    return ci.DisplayName;
                }
            }
            
            string second = lang_country[0];

            foreach (CultureInfo ci in cinfo)
            {
                if (ci.IetfLanguageTag == second)
                {
                    return ci.DisplayName;
                }
            }

            return "Desconocido ("+langstring+")";
        }

    }
}
