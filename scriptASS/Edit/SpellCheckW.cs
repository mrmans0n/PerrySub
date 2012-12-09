using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
//using NHunspell;
using System.Text.RegularExpressions;

namespace scriptASS
{
    public partial class SpellCheckW : Form
    {

        SortedList<string, string> DictCultureNames;
        mainW mW;
        PerryHunspell hun;
        int posicionActual = -1;
        
        internal int PosicionActual
        {
            get { return posicionActual; }
            set { 
                posicionActual = value;
                if ((posicionActual == -1) || (ErrorsFound.Count ==0))
                    labelErrorActual.Text = "Ninguno";
                else
                    labelErrorActual.Text = (posicionActual+1) + "/" + ErrorsFound.Count;
            }
        }

        string diccionario;
        List<SpellCheckError> ErrorsFound;

        public SpellCheckW(mainW mw)
        {
            InitializeComponent();
            this.mW = mw;
            diccionario = mW.ActiveDict;
            try
            {
                LoadDictionaries();
            }
            catch { }

            if (hun == null)
            {
                mW.errorMsg("No se ha podido cargar el diccionario");
                this.Dispose();
            }


        }

        private void SpellCheckW_Load(object sender, EventArgs e)
        {
            ErrorsFound = FullSpellCheck();
            if (ErrorsFound.Count == 0)
            {
                MessageBox.Show("No se han encontrado errores ortográficos", mainW.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Dispose();
            }
            else
            {
                PosicionActual = 0;
                ShowActualError();
            }
        }

        private void LoadDictionaries()
        {
            DictCultureNames = new SortedList<string, string>();
            DirectoryInfo info = new DirectoryInfo(mW.dictDir);
            FileInfo[] dics = info.GetFiles("*.dic");

            diccionarioActivo.Items.Clear();

            foreach (FileInfo fi in dics)
            {
                string nom = Path.GetFileNameWithoutExtension(fi.FullName);
                string nomext = CultureWrapper.GetLanguageName(nom);
                DictCultureNames.Add(nomext, nom); // deberia ser al reves pero weno
                diccionarioActivo.Items.Add(nomext);
            }

            diccionarioActivo.Text = CultureWrapper.GetLanguageName(diccionario);
            string AffFile = Path.Combine(mW.dictDir, DictCultureNames[diccionarioActivo.Text] + ".aff");
            string DicFile = Path.Combine(mW.dictDir, DictCultureNames[diccionarioActivo.Text] + ".dic");

            if (hun != null) hun.Dispose();
            hun = new PerryHunspell(AffFile, DicFile);
            PreviewAssBox.EnableSpellChecking = true;
            PreviewAssBox.DictionaryPath = mW.dictDir;
            PreviewAssBox.Dictionary = DictCultureNames[diccionarioActivo.Text];

            string old = PreviewAssBox.Text; // reset
            PreviewAssBox.Text = "";
            PreviewAssBox.Text = old;
        }

        private List<SpellCheckError> FullSpellCheck()
        {
            List<SpellCheckError> misErrores = new List<SpellCheckError>();

            for (int i = 0; i < mW.script.LineCount; i++)
            {
                misErrores.AddRange(LineSpellCheck(i));
            }

            return misErrores;
        }

        private List<SpellCheckError> LineSpellCheck(int idx)
        {
            lineaASS miLinea = (lineaASS)mW.script.GetLines()[idx];
            List<Point> posLlaves = new List<Point>();
            List<SpellCheckError> erroresEncontrados = new List<SpellCheckError>();

            string temp = miLinea.texto; // obtenemos la lista de llaves y sus posiciones
            do
            {
                int idxOpn = temp.LastIndexOf('{');
                int idxCls = temp.LastIndexOf('}');
                if ((idxOpn > idxCls) || (idxOpn == -1)) break;
                temp = temp.Remove(idxOpn, (idxCls - idxOpn) + 1);
                posLlaves.Add(new Point(idxOpn, idxCls));
            } while (temp.IndexOf('{') != -1);

            Regex r = new Regex(@"\w+"); // separamos las palabras
            Regex r2 = new Regex(@"{\\(p|pbo)(\d)+}(\s|\d|m|n|l|b|s|p|c)*({\\(p|pbo)0})?");
            MatchCollection mc2 = r2.Matches(miLinea.texto); // no nos gustan las lineas de dibujo
            if (mc2.Count == 0)
            {
                MatchCollection mc = r.Matches(miLinea.texto);
                foreach (Match m in mc)
                {
                    if (!EntreLlaves(m.Index, m.Length, posLlaves))
                    {
                        if (!hun.Spell(m.Value))
                            erroresEncontrados.Add(new SpellCheckError(m.Value, miLinea, idx, m.Index));

                    }
                }
            }
            return erroresEncontrados;
        }

        private bool EntreLlaves(int posInicio, int tamaño, List<Point> posLlaves)
        {
            int posFinal = posInicio + tamaño;

            foreach (Point p in posLlaves) // p.X = inicio, p.Y = fin
            {
                if (posInicio > p.X && posInicio < p.Y) return true;
                if (posFinal > p.X && posFinal < p.Y) return true;
            }
            return false;
        }

        internal class SpellCheckError
        {
            public string ErrorWord
            {
                get { return badWord; }
            }

            public lineaASS Line
            {
                get { return (lineaASS)linea; }
            }

            public int LineNumberInScript
            {
                get { return linea.Index; }
            }

            public int IndexInWord
            {
                get { return posicion; }
            }

            lineaASSidx linea;
            int posicion;
            string badWord;

            internal SpellCheckError(string badWordFound, lineaASS line, int lineNumber, int pos)
            {
                linea = new lineaASSidx(line.ToString(), lineNumber);
                posicion = pos;
                badWord = badWordFound;
            }
            
            internal void ApplyOffsetToIndex(int offset)
            {
                posicion += offset;
            }
        }

        private void ShowActualError()
        {
            if (ErrorsFound.Count == 0)
            {
                textOriginal.Text = textSugerencia.Text = PreviewAssBox.Text = "";
                listBox1.DataSource = null;
                listBox1.Items.Clear();
                PosicionActual = PosicionActual;
                mW.updateGridWithArrayList(mW.al);
                return;
            }
            SpellCheckError error = ErrorsFound[PosicionActual];
            PreviewAssBox.Text = ((lineaASS)mW.script.GetLines()[error.LineNumberInScript]).texto;
            PreviewAssBox.ForceRefresh();

            PreviewAssBox.Select(error.IndexInWord, error.ErrorWord.Length);

            textOriginal.Text = error.ErrorWord;
            List<string> sugiere = hun.Suggest(error.ErrorWord);
            listBox1.DataSource = sugiere;
            if (sugiere.Count == 0)
                textSugerencia.Text = textOriginal.Text;
        }

        private void ShowNextError()
        {
            if (PosicionActual == ErrorsFound.Count-1)
            {
                MessageBox.Show("Se ha llegao al final de los errores detectados.", mainW.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                PosicionActual = 0;
            } else 
                PosicionActual++;

            ShowActualError();
        }

        private void ReplaceError(SpellCheckError error, string texto)
        {
            // generamos nueva string
            lineaASS real = (lineaASS)mW.script.GetLines()[error.LineNumberInScript];
            string antes = real.texto.Substring(0, error.IndexInWord);
            string despues = real.texto.Substring(error.IndexInWord + error.ErrorWord.Length);
            string nueva = antes + texto + despues;
            // tenemos q actualizar el resto de entradas de esa linea si cambiamos una
            int offset = nueva.Length - real.texto.Length;
            if (offset != 0)
            {
                List<SpellCheckError> ParaActualizar = new List<SpellCheckError>();
                foreach (SpellCheckError err in ErrorsFound)
                    if (err.LineNumberInScript == error.LineNumberInScript)
                        if (err.IndexInWord > error.IndexInWord)
                            ParaActualizar.Add(err); // Jackpot                
                for (int i = 0; i < ParaActualizar.Count; i++)
                {
                    SpellCheckError err = ParaActualizar[i];
                    err.ApplyOffsetToIndex(offset);
                }

            }
            // y updateamos
            real.texto = nueva;            
            mW.updateGridWithArrayList(mW.al);
            //mW.moveViewRows(error.LineNumberInScript);
            
        }

        private void diccionarioActivo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DictCultureNames[diccionarioActivo.Text] != diccionario)
            {
                diccionario = DictCultureNames[diccionarioActivo.Text];
                // recalcular errores ortograficos
                LoadDictionaries();
                ErrorsFound = FullSpellCheck();
                PosicionActual = 0;
                ShowActualError();

                if (ErrorsFound.Count == 0)
                    MessageBox.Show("No se han encontrado errores ortográficos", mainW.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem!=null)
                textSugerencia.Text = listBox1.SelectedItem.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // reemplazar
            if (ErrorsFound.Count == 0) return;
            SpellCheckError error = ErrorsFound[PosicionActual];
            ReplaceError(error, textSugerencia.Text);
            ErrorsFound.RemoveAt(PosicionActual);
            PosicionActual = PosicionActual;
            if (PosicionActual >= ErrorsFound.Count)
                PosicionActual = 0;
            else
                PosicionActual = PosicionActual;
            ShowActualError();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // reemplazar todos
            if (ErrorsFound.Count == 0) return;
            for (int j = ErrorsFound.Count - 1; j >= 0; j--)
            {
                SpellCheckError error = ErrorsFound[j];
                if (error.ErrorWord.Equals(textOriginal.Text, StringComparison.InvariantCultureIgnoreCase))
                {
                    ReplaceError(error, textSugerencia.Text);
                    ErrorsFound.RemoveAt(j);
                }
            }
            if (PosicionActual >= ErrorsFound.Count)
                PosicionActual = 0;
            else
                PosicionActual = PosicionActual;
            ShowActualError();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // ignorar
            if (ErrorsFound.Count == 0) return;
            //ErrorsFound.RemoveAt(PosicionActual);
            //PosicionActual = PosicionActual;
            //ShowActualError();
            ShowNextError();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // ignorar todas las ocurrencias de algo
            if (ErrorsFound.Count == 0) return;
            for (int j = ErrorsFound.Count - 1; j >= 0; j--)
            {
                SpellCheckError error = ErrorsFound[j];
                if (error.ErrorWord.Equals(textOriginal.Text, StringComparison.InvariantCultureIgnoreCase))
                    ErrorsFound.RemoveAt(j);
            }
            if (PosicionActual >= ErrorsFound.Count)
                PosicionActual = 0;
            else 
                PosicionActual = PosicionActual;
            ShowActualError();

        }

        private void button5_Click(object sender, EventArgs e)
        {
            // añadir a diccionario
            if (String.IsNullOrEmpty(textOriginal.Text)) return;
            if (hun.Add(textOriginal.Text))
            {
                MessageBox.Show("Término '" + textOriginal.Text + "' añadido al diccionario con éxito.", mainW.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                for (int j = ErrorsFound.Count - 1; j >= 0; j--)
                {
                    SpellCheckError error = ErrorsFound[j];
                    if (error.ErrorWord.Equals(textOriginal.Text, StringComparison.InvariantCultureIgnoreCase))
                        ErrorsFound.RemoveAt(j);
                }
                if (PosicionActual >= ErrorsFound.Count)
                    PosicionActual = 0;
                else
                    PosicionActual = PosicionActual;
                ShowActualError();
            }
            else
                mW.errorMsg("No se ha podido añadir el término al diccionario");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // reanalisis
            PosicionActual = -1;
            ErrorsFound = FullSpellCheck();
            if (ErrorsFound.Count > 0)
            {
                PosicionActual = 0;
                ShowActualError();
            }
            else
                MessageBox.Show("No se han encontrado errores ortográficos", mainW.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}
