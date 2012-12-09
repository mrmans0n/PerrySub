using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace scriptASS
{
    public delegate void AnalizarLineasMaxLineaProcesada(object sender, AnalizarLineasMaxLineaProcesadaEventArgs e);
    public delegate void AnalizarLineasMaxAnalisisCompletado(object sender);
    public delegate void AnalizarLineasMaxLineaModificada(object sender, AnalizarLineasMaxLineaModificadaEventArgs e);
    public delegate void AnalizarLineasMaxModificacionesCompletadas(object sender);

    public class AnalizarLineasMaxLineaProcesadaEventArgs : EventArgs
    {
        public readonly int Contador;
        public readonly int Lineas;
        public readonly bool Cumple;

        public AnalizarLineasMaxLineaProcesadaEventArgs(int n, int lineas, bool cumple)
        {
            Contador = n;
            Lineas = lineas;
            Cumple = cumple;
        }

    }

    public class AnalizarLineasMaxLineaModificadaEventArgs : EventArgs
    {
        public readonly int Linea;

        public AnalizarLineasMaxLineaModificadaEventArgs(int n)
        {
            Linea = n;
        }
    }

    public class AnalizarLineasMax
    {
        SubtitleScript script;
        ArrayList styles;
        int maxLines, width, height,minPerc;
        Hashtable modificar;
        int lowLimit;
        bool stop;
        string headerMark = "[Script Info]";

        string stylesMark =
            "[V4+ Styles]\n" +
            "Format: Name, Fontname, Fontsize, PrimaryColour, SecondaryColour, OutlineColour, BackColour, Bold, Italic, Underline, StrikeOut, ScaleX, ScaleY, Spacing, Angle, BorderStyle, Outline, Shadow, Alignment, MarginL, MarginR, MarginV, Encoding";

        string dialoguesMark =
            "[Events]\n" +
            "Format: Layer, Start, End, Style, Name, MarginL, MarginR, MarginV, Effect, Text";



        public event AnalizarLineasMaxLineaProcesada LineaProcesada;
        public event AnalizarLineasMaxAnalisisCompletado AnalisisCompletado;
        public event AnalizarLineasMaxLineaModificada LineaModificada;
        public event AnalizarLineasMaxModificacionesCompletadas ModificacionesCompletadas;

        public AnalizarLineasMax(SubtitleScript script, ArrayList styles, int maxLines, int width, int height, int lowlimit)
        {
            this.script = script;
            this.styles = styles;
            this.maxLines = maxLines;
            this.width = width;
            this.height = height;
            this.lowLimit = lowLimit;
            stop = false;
        }

        public void StopCondition()
        {
            stop = true;
        }

        public void CargarModificaciones(Hashtable h, int minPerc)
        {
            this.minPerc = minPerc;
            modificar = h;
        }

        private int CalcularLineas(AviSynthClip avs, string PreviewFile, lineaASS actual)
        {
            int nLineas;

            //lineaASS actual = (lineaASS)lass.Clone();

            TextWriter o = new StreamWriter(PreviewFile, false, System.Text.Encoding.UTF8);
            o.WriteLine(headerMark);
            o.WriteLine(script.GetHeader().ToString());

            o.WriteLine(stylesMark);
            foreach (estiloV4 e2 in script.GetStyles())
            {
                if (e2.Name.Equals(actual.estilo))
                {
                    estiloV4 new_est = new estiloV4(e2.ToString());
                    new_est.PrimaryColor = "&H00FFFFFF";
                    new_est.SecondaryColor = "&H00FFFFFF";
                    new_est.OutlineColor = "&H00FFFFFF";
                    new_est.ShadowColor = "&H00FFFFFF";
                    o.WriteLine(new_est.ToString().Replace("\n", string.Empty));
                }
            }

            o.WriteLine(); // ---

            o.WriteLine(dialoguesMark);

            actual.t_inicial.setTiempo(0);
            actual.t_final.setTiempo(50);
            o.WriteLine(actual.ToString());

            o.Close();

            avs.AviSynthInvoke(avs.GetAVS(), 0, "Eval", false, "blankclip(color=$000000,width=" + width + ",height=" + height + ")");
            avs.AviSynthInvoke(avs.GetAVS(), 0, "TextSub", true, PreviewFile);

            Image iimg = (Image)AviSynthFunctions.getBitmapFromFrame(avs, 0, 0);

            int x,y,x2,y2;

            nLineas = ContarLineas(iimg, out x, out y, out x2, out y2);

            iimg.Dispose();

            File.Delete(PreviewFile);
            return nLineas;
        }

        private int ContarLineas(Image iimg, out int x1, out int y1, out int x2, out int y2)
        {
            int nLineas = 0;
            x1 = y1 = x2 = y2 = -1;
            // procesamiento de la imagen

            Bitmap img = new Bitmap(iimg);
            int max_Y = height;
            int max_X = width;
            int[] histograma = new int[max_Y];
            BitmapData data = img.LockBits(new Rectangle(0, 0, max_X, max_Y), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int ptr = 0;
            unsafe
            {
                byte* imgPtr = (byte*)(data.Scan0);
                for (int i = 0; i < data.Height; i++)
                {
                    for (int j = 0; j < data.Width; j++)
                    {
                        if (stop) return -1;
                        int r = imgPtr[ptr];
                        int g = imgPtr[ptr + 1];
                        int b = imgPtr[ptr + 2];

                        if ((r + g + b) > 0)
                        {
                            if (x1 == -1) x1 = j;
                            if (y1 == -1) y1 = i;
                            x2 = j; y2 = i;
                            histograma[i]++;
                        }
                        ptr += 3;
                    }
                    histograma[i] = (int)Math.Round((double)histograma[i] * 100 / (double)max_Y);
                    ptr += data.Stride - data.Width * 3;
                }
            }

            img.UnlockBits(data);
            nLineas = 0;
            int anterior = 0;
            for (int i = 0; i < max_Y; i++)
            {
                int ac = histograma[i];
                int perc = (int)Math.Round(((double)anterior / (double)max_X)*100);
                if (ac > 0 && anterior == 0) 
                    nLineas++;
                anterior = ac;
            }

            //if (nLineas>maxLines) MessageBox.Show("Linea que incumple los requisitos = "+idx, mainW.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            img.Dispose();
            return nLineas;
        }

        private void InitAviSynth(out AviSynthScriptEnvironment env, out AviSynthClip avs)
        {
            env = new AviSynthScriptEnvironment();
            avs = env.ParseScript("blankclip(color=$000000,width=" + width + ",height=" + height + ")");
            string vsf_path = AviSynthFunctions.getVSFilterPath(avs);
            if (vsf_path != null)
                avs.AviSynthInvoke(avs.GetAVS(), 0, "LoadPlugin", false, vsf_path);

        }

        public void ProcesarLineas()
        {
            DateTime inicio = DateTime.Now;
            int idx = 0, nLineas = 0;
            AviSynthScriptEnvironment env;
            AviSynthClip avs;
            try
            {
                InitAviSynth(out env, out avs);
            }
            catch
            {
                throw new AviSynthException("Error cargando AVS");
            }

            // blankclip(color=$000000, fps= 10, length=n_fps, width=1024, height= 576)


            SubtitleScript new_script = (SubtitleScript)script.Clone();

            for (int i = 0; i < new_script.GetStyles().Count; i++) // estilos a blanco
            {
                estiloV4 vvv = (estiloV4)new_script.GetStyles()[i];
                vvv.PrimaryColor = "&H00FFFFFF";
                vvv.SecondaryColor = "&H00FFFFFF";
                vvv.OutlineColor = "&H00FFFFFF";
                vvv.ShadowColor = "&H00FFFFFF";
            }

            for (int i = 0; i < new_script.LineCount; i++) // tiempos a 0.1++
            {
                double init_time = (double)i / 10;
                double end_time = init_time + 0.1;

                lineaASS lass = (lineaASS)new_script.GetLineArrayList().GetFullArrayList()[i];
                lass.t_inicial.setTiempo(init_time);
                lass.t_final.setTiempo(end_time);
            }
            new_script.FileName = "PrevFile-full.ass";
            new_script.SaveFile();

            avs.AviSynthInvoke(avs.GetAVS(), 0, "Eval", false, "blankclip(color=$000000,fps=10,length=" + (10 * new_script.LineCount) + ",width=" + width + ",height=" + height + ")");
            avs.AviSynthInvoke(avs.GetAVS(), 0, "TextSub", true, new_script.FileName);

            foreach (lineaASS lass in new_script.GetLineArrayList().GetFullArrayList())
            {
                if (stop) return;
                bool bleh = false;
                foreach (estiloV4 v in styles)
                    if (v.Name.Equals(lass.estilo))
                        bleh = true;

                if (bleh)
                {
                    Image iimg = (Image)AviSynthFunctions.getBitmapFromFrame(avs, 0, idx);
                    int x, y, x2, y2;
                    nLineas = ContarLineas(iimg, out x, out y, out x2, out y2);
                    iimg.Dispose();
                }
                if (LineaProcesada!=null)
                    LineaProcesada(this, new AnalizarLineasMaxLineaProcesadaEventArgs(idx, nLineas, (nLineas > maxLines))); 
                idx++;
            }
            avs.cleanup(true);
            File.Delete(new_script.FileName);         
             
            TimeSpan duracion = DateTime.Now - inicio;
            //MessageBox.Show("Duracion = " + duracion.ToString());
            if (AnalisisCompletado != null) 
                AnalisisCompletado(this);
        }

        public void RealizarModificaciones()
        {
            AviSynthScriptEnvironment env;
            string script0rz ="";
            AviSynthClip avs;
            try
            {
                InitAviSynth(out env,out avs);
            }
            catch
            {
                throw new AviSynthException("Error cargando AVS");
            }

            int nLines = 0;
            int iiddxx = 0;
            foreach (int idx in modificar.Keys)
            {
                if (stop) return;
                int lineas = (int)modificar[idx];
                lineaASS lass_orig = (lineaASS)script.GetLineArrayList().GetFullArrayList()[idx];
                lineaASS lass = (lineaASS)lass_orig.Clone();

                lass.texto = "{\\fscx"+minPerc+"}" + lass.texto;

                if ((nLines = CalcularLineas(avs, "PrevFile-l" + idx + "-fscxMIN.ass", lass)) > maxLines)
                    MessageBox.Show("Linea que incumple los requisitos : " + (idx+1) + "\nNi con el porcentaje mínimo ("+minPerc+"%) llega para hacer que tenga " + maxLines + " líneas.\nPrueba rebajando el % mínimo, o partiendo la línea en varios tiempos", mainW.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                {
                    //con el minimo se puede
                    int lastTest = minPerc;
                    for (int i = minPerc+1; i < 101; i++)
                    {
                        lass = (lineaASS)lass_orig.Clone();
                        lass.texto = "{\\fscx" + i + "}" + lass.texto;

                        if (CalcularLineas(avs, "PrevFile-l" + idx + "-fscx" + i + ".ass", lass) > maxLines)
                            break;
                        lastTest = i;
                    }

                    // MessageBox.Show("La línea "+(idx+1)+" necesita un fscx de "+lastTest+" para ocupar "+maxLines+" líneas", mainW.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lass_orig.texto = "{\\fscx" + lastTest + "}" + lass_orig.texto;
                }
                iiddxx++;
                if (LineaModificada != null)
                    LineaModificada(this, new AnalizarLineasMaxLineaModificadaEventArgs(iiddxx)); 
            }
            if (ModificacionesCompletadas != null)
                ModificacionesCompletadas(this);
        }

    }
}
