using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

namespace scriptASS
{
    public delegate void AnalizarBusquedaArchivosFinalizada(object sender, EventArgs e); // analisis finalizado
    public delegate void AnalizarBusquedaArchivosProcesado(object sender, ArchivoProcesadoEventArgs e); // se ha procesado un archivo (solo +1, invariablemente)

    public class ArchivoProcesadoEventArgs : EventArgs
    {
        public readonly SubtitleScript ScriptTratado;
        public readonly ArrayList LineasCoincidencias;

        public ArchivoProcesadoEventArgs(SubtitleScript script, ArrayList lineas)
        {
            ScriptTratado = script;
            LineasCoincidencias = lineas;
        }
    }

    public class AnalizarBusquedaArchivos
    {
        string buscar;
        bool casesens, isregex;
        ArrayList lista;

        public event AnalizarBusquedaArchivosFinalizada AnalisisFinalizado;
        public event AnalizarBusquedaArchivosProcesado ArchivoProcesado;


        // poner un limite de 50 scripts o asi

        public AnalizarBusquedaArchivos(string SearchString, bool CaseSensitive, bool isRegex, ArrayList FileList)
        {
            lista = FileList;
            casesens = CaseSensitive;
            isregex = isRegex;
            buscar = SearchString;
        }


        public void RealizarAnalisis()
        {

            bool totalmatch = false;
            for (int j = 0; j < lista.Count; j++)
            {
                bool filematch = false;
                SubtitleScript script = null;
                ArrayList lineasConCoincidencia = new ArrayList();
                try
                {
                    // primero generamos SubtitleScript
                    script = new SubtitleScript(lista[j].ToString());
                    // hay coincidencias?
                    for (int i = 0; i < script.LineCount; i++)
                    {
                        bool ismatch = false;
                        lineaASS actual = (lineaASS)script.GetLineArrayList().GetFullArrayList()[i];

                        if (isregex)
                        {
                            try
                            {
                                Regex r = new Regex(buscar);
                                ismatch = r.IsMatch(actual.texto);
                            }
                            catch
                            {
                                throw new Exception("Fallo al compilar la expresión regular");
                            }
                        }
                        else
                        {
                            ismatch = (casesens) ?
                                (actual.texto.ToLower().Contains(buscar.ToLower())) :
                                (actual.texto.Contains(buscar));
                        }

                        if (ismatch)
                        {
                            filematch = true;
                            lineasConCoincidencia.Add(i);
                        }
                    }

                }
                catch (Exception x) { Console.WriteLine("Excepción " + x.ToString() + " : " + x.Message); }
                    if (filematch)
                    {
                        // tenemos : script (todo el script), lineasConCoincidencia (lineas donde hay coincidencias)

                        if (ArchivoProcesado != null)
                            ArchivoProcesado(this, new ArchivoProcesadoEventArgs(script, lineasConCoincidencia));

                        totalmatch = true;
                    }
                    else
                    {

                        if (ArchivoProcesado != null)
                            ArchivoProcesado(this, new ArchivoProcesadoEventArgs(script, null));

                    }
                
                // ------ archivo procesado

            }

            // --- todos los archivos procesados

            if (AnalisisFinalizado!=null)
                AnalisisFinalizado(this, new EventArgs());
        }

    }
}
