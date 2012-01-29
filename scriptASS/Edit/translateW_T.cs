using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace scriptASS
{
    class translateW_T
    {
        Uri u;
        WebBrowser w;
        translateW t;

        public translateW_T(string uri, WebBrowser wb, translateW tw)
        {
            u = new Uri(uri, UriKind.Absolute);
            w = wb;
            t = tw;
        }

        public void Run()
        {
            w.Url = u;
            //t.LoadUriInWebBrowser(w, u);
        }

    }
}
