using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.Threading;

namespace scriptASS
{
    public class AjustarKeyframeLineaProcesadaEventArgs : EventArgs
    {
        public readonly int NumeroLinea;

        public AjustarKeyframeLineaProcesadaEventArgs(int n)
        {
            NumeroLinea = n;
        }
    }

    public class AdjustToKeyframe
    {
        SubtitleScript script;
        int iPre, iPost, fPre, fPost;
        double fps;
        ArrayList estilos;
        ArrayList lineasOrdenadas;
        int[] keyFrame;
        public Hashtable Resultado;
        bool seguridadColisiones;

        public int ProcesarLineas {
            get {
                return lineasOrdenadas.Count;
            }
        }

        public delegate void AjustarKeyframeLineaProcesada(object sender, AjustarKeyframeLineaProcesadaEventArgs lineaproc);
        public delegate void AjustarKeyframeProcesamientoFinalizado(object sender);

        public event AjustarKeyframeLineaProcesada LineaProcesada;
        public event AjustarKeyframeProcesamientoFinalizado ProcesoFinalizado;

        public AdjustToKeyframe(SubtitleScript script, int iPre, int iPost, int fPre, int fPost, double fps, ArrayList est, ArrayList keyFrame)
        {
            this.script = script;
            this.iPre = iPre;
            this.iPost = iPost;
            this.fPre = fPre;
            this.fPost = fPost;
            this.fps = fps;

            seguridadColisiones = true; //((iPre != fPre) || (iPost != fPost));

            Resultado = new Hashtable();

            estilos = new ArrayList();
            foreach (estiloV4 v in est)
                estilos.Add(v.Name);
            
            
            this.keyFrame = (int[])keyFrame.ToArray(typeof(int));

            lineasOrdenadas = new ArrayList();
            for (int i = 0; i < script.GetLineArrayList().GetFullArrayList().Count; i++)
            {
                lineaASS lass = (lineaASS)script.GetLineArrayList().GetFullArrayList()[i];
                if (estilos.Contains(lass.estilo))
                    lineasOrdenadas.Add(new lineaASSidx(lass.ToString(), i));
            }
            lineasOrdenadas.Sort();
        }

        public void ProcesarAjuste()
        {
            ArrayList keysInRangeInit = new ArrayList();
            ArrayList keysInRangeFinal = new ArrayList();
            bool change;

            for (int count=0; count<lineasOrdenadas.Count; count++)
            {
                lineaASSidx lassidx = (lineaASSidx)lineasOrdenadas[count];
                change = false;
                lineaASS lass = new lineaASS(lassidx.ToString());
                lineaASS lass_original = new lineaASS(lassidx.ToString());

                int idx = lassidx.Index;

                int anterior = 0, posterior = 0;

                int inFrame = Convert.ToInt32(Math.Round(lass.t_inicial.getTiempo() * fps));
                int outFrame = Convert.ToInt32(Math.Round(lass.t_final.getTiempo() * fps));

                int inRange = Math.Max(inFrame - iPre, 0);  // evitando negatiffos
                int inRangeEnd = inFrame + iPost;
                int outRange = Math.Max(outFrame - fPre,0); // caso raro
                int outRangeEnd = outFrame + fPost;

                // lista de los keyframes que cumplen los requisitos
                keysInRangeInit.Clear();
                keysInRangeFinal.Clear();

                for (int i = 0; i < keyFrame.Length; i++)
                {
                    int k = (int)keyFrame[i];
                    if ((k >= inRange) && (k <= inRangeEnd)) keysInRangeInit.Add(k);
                    if ((k <= outRangeEnd) && (k >= outRange)) keysInRangeFinal.Add(k);

                    if (k <= inFrame)
                        anterior = k;

                    if (k >= outFrame)
                        if (posterior == 0)
                            posterior = k;
                }

                double r=0.0, s=0.0;

                if (keysInRangeFinal.Count > 0)
                {
                    r = (Convert.ToDouble((int)keysInRangeFinal[keysInRangeFinal.Count - 1]) / (double)fps);
                    r -= 0.01;
                    if (Math.Round(r,2)!=lassidx.t_inicial.getTiempo())
                        lass.t_final.setTiempo(r); // si el final coincide con el principio
                    change = true;
                }

                r = lass.t_final.getTiempo();

                if (keysInRangeInit.Count > 0)
                {                    
                    s = (Convert.ToDouble((int)keysInRangeInit[0]) / (double)fps);
                    s -= 0.01;
                    if (r != Math.Round(s, 2) && r > Math.Round(s, 2)) // primamos fin a inicio
                        lass.t_inicial.setTiempo(s);
                    change = true;
                }

                double duracion = lass.t_final.getTiempo() - lass.t_inicial.getTiempo();
                if (duracion <= 0.0)
                    lass.t_inicial = lass_original.t_inicial;

                // lass tratada

                // aquí código de seguridad para evitar colisiones si los valores no están igualados si se ha cambiado algo

                if (seguridadColisiones)
                {
                    foreach (int key in Resultado.Keys)
                    {
                        lineaASS otra = (lineaASS)Resultado[key];   // una de las anteriores
                        if (otra.t_final.getTiempo() > lass.t_inicial.getTiempo())
                        {
                            // sera con fpre supongo
                            double pre_s = (Convert.ToDouble(fPre) / (double)fps);
                            if (!((otra.t_final.getTiempo() - pre_s) > lass.t_inicial.getTiempo())) {
                                lass.t_inicial = otra.t_final; // si hay más de 1 supongo que todos estarán al mismo keyframe, ¿no?
                                change = true;
                            }
                        }
                    }
                }

                // s'acabo
                
                if (change)
                    Resultado.Add(idx, lass);

                if (LineaProcesada != null)
                    LineaProcesada(this, new AjustarKeyframeLineaProcesadaEventArgs(count));

            }

            if (ProcesoFinalizado != null)
                ProcesoFinalizado(this);
        }

    }
}
