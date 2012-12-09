using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Collections;
using System.Runtime.InteropServices;
using HWND = System.IntPtr;
//using NHunspell;

namespace scriptASS
{
    class ASSTextBoxRegEx : RichTextBoxWithSpecialUnderlines
    {
        [DefaultValue(false)]
        public override bool Multiline
        {
            get { return base.Multiline; }
            set { base.Multiline = value; }
        }

        private Stack UndoStack;
        private Stack RedoStack;

        private Color syntaxColor = Color.Indigo;

        public Color SyntaxColor
        {
            get { return syntaxColor; }
            set { syntaxColor = value; }
        }
        private Color reservedWordColor = Color.Blue;

        public Color ReservedWordColor
        {
            get { return reservedWordColor; }
            set { reservedWordColor = value; }
        }
        private Color defaultColor = Color.Black;

        public Color DefaultColor
        {
            get { return defaultColor; }
            set { defaultColor = value; }
        }
        private Color bracketColor = Color.Red;

        public Color BracketColor
        {
            get { return bracketColor; }
            set { bracketColor = value; }
        }
        private Color slashColor = Color.DarkKhaki;

        public Color SlashColor
        {
            get { return slashColor; }
            set { slashColor = value; }
        }
        private Color commentColor = Color.Green;

        public Color CommentColor
        {
            get { return commentColor; }
            set { commentColor = value; }
        }
        private Color integerColor = Color.Brown;

        public Color IntegerColor
        {
            get { return integerColor; }
            set { integerColor = value; }
        }

        private bool enableSpellChecking = false;

        [DefaultValue(false)]
        public bool EnableSpellChecking
        {
            get { return enableSpellChecking; }
            set { enableSpellChecking = value; }
        }

        private string dictionaryLocal = "es_ES";
        private string dictionaryPath = "";

        public string DictionaryPath
        {
            get { return dictionaryPath; }
            set { dictionaryPath = value; }
        }

        [DefaultValue("es_ES")]
        public string Dictionary
        {
            get
            {
                return dictionaryLocal;
            }
            set
            {
                string aff = System.IO.Path.Combine(dictionaryPath, value + ".aff");
                string dic = System.IO.Path.Combine(dictionaryPath, value + ".dic");

                if (!System.IO.File.Exists(aff))
                    throw new System.IO.FileNotFoundException("No se ha encontrado el archivo AFF de diccionarios " + aff);

                if (!System.IO.File.Exists(dic))
                    throw new System.IO.FileNotFoundException("No se ha encontrado el archivo DIC de diccionarios " + dic);

                LoadDictionary(aff, dic);
                dictionaryLocal = value;
            }
        }


        private ArrayList toPaint = new ArrayList();
        private ArrayList isWord = new ArrayList();

        private string ReservedWordsRegEx = "";

        #region WIN32
        public const int WM_USER = 0x400;
        public const int WM_PAINT = 0xF;
        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public const int WM_CHAR = 0x102;

        public const int EM_GETSCROLLPOS = (WM_USER + 221);
        public const int EM_SETSCROLLPOS = (WM_USER + 222);

        public const int EM_FINDWORDBREAK = (WM_USER + 76);
        public const int EM_SETSEL = 0xB1;
        public const int WB_MOVEWORDLEFT = 4;
        public const int WB_MOVEWORDRIGHT = 5;
        public const int WB_RIGHTBREAK = 7;
        public const int WB_CLASSIFY = 3;

        public const int VK_CONTROL = 0x11;
        public const int VK_UP = 0x26;
        public const int VK_DOWN = 0x28;
        public const int VK_NUMLOCK = 0x90;

        public const short KS_ON = 0x01;
        public const short KS_KEYDOWN = 0x80;

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [DllImport("user32")]
        public static extern int SendMessage(HWND hwnd, int wMsg, int wParam, IntPtr lParam);
        [DllImport("user32")]
        public static extern int PostMessage(HWND hwnd, int wMsg, int wParam, int lParam);
        [DllImport("user32")]
        public static extern short GetKeyState(int nVirtKey);
        [DllImport("user32")]
        public static extern int LockWindowUpdate(HWND hwnd);
        #endregion

        private static bool _Paint = true;
        private bool isUndoRedo = false;
        private UndoRedoInfo LastInfo = new UndoRedoInfo("", new POINT(), 0);
        PerryHunspell hun = null;

        private string[] ReservedWords = {"a","b","i","u","s","bord","shad","be","fn","fs",
                                          "fsc","fsp","fr","fe","c","alpha","1c","2c","3c","4c",
                                          "1a","2a","3a","4a","k","q","r","t","move","pos","org","an",
                                          "fade","fad","clip","p","pbo","frx","fry","frz","fscx","fscy",
                                          // vsf 2.39
                                          "xbord","ybord","xshad","yshad","blur","fax","fay","iclip"
                                         };


        public ASSTextBoxRegEx()
            : base()
        {
            UndoStack = new Stack();
            RedoStack = new Stack();

            Array.Sort(ReservedWords);
            Array.Reverse(ReservedWords);

            for (int i = 0; i < ReservedWords.Length; i++)
                ReservedWordsRegEx += (i == ReservedWords.Length - 1) ? ReservedWords[i] : ReservedWords[i] + "|";

            this.ContextMenu = new ASSTextBoxRegExDefaultContextMenu(this);

            if (enableSpellChecking)
                Dictionary = Dictionary;

        }

        // hack para evitar flicking y captura de teclado
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {

            switch (m.Msg)
            {
                case WM_PAINT:
                    if (!_Paint)
                    {
                        m.Result = IntPtr.Zero;
                        return;
                    }
                    break;
                case WM_KEYDOWN:
                    if (((GetKeyState(VK_CONTROL)) & KS_KEYDOWN) != 0)
                    {
                        if ((Keys)(int)m.WParam == Keys.Z)
                        {
                            Undo();
                            return;
                        }
                        else if ((Keys)(int)m.WParam == Keys.Y)
                        {
                            Redo();
                            return;
                        }
                    }
                    break;
            }
            base.WndProc(ref m);

        }

        private void ProcesarTags()
        {
            toPaint = new ArrayList();
            string texto_actual = this.Text;

            Regex r = new Regex(@"\{[^\{\\\r\n\f]*(?:\\.[^\}\r\n\f]*)*\}", RegexOptions.IgnoreCase);
            MatchCollection mc = r.Matches(texto_actual);

            if (hun != null && enableSpellChecking)
            {

                Regex rx = new Regex(@"\w+");
                MatchCollection mcx = rx.Matches(texto_actual);

                foreach (Match m in mcx)
                {
                    if (!hun.Spell(m.Value))
                        toPaint.Add(new ASSTextBoxRegExPaint(m.Index, m.Length, ForeColor, Font, UnderlineStyle.Wave));
                }
            }

            foreach (Match m in mc)
            {
                toPaint.Add(new ASSTextBoxRegExPaint(m.Index, 1, BracketColor, new Font(Font, FontStyle.Bold)));

                string entrellaves = this.Text.Substring(m.Index + 1, m.Length - 2).Trim();
                bool istag = (entrellaves.StartsWith(@"\"));

                toPaint.Add(new ASSTextBoxRegExPaint(m.Index + 1, m.Length - 2, ((istag) ? SyntaxColor : CommentColor), Font));

                //if (istag && !CargaExcesiva && ProcessColor)
                if (istag)
                {
                    r = new Regex(@"\\(\w+|\W+|\(+|\)+)");
                    MatchCollection mc2 = r.Matches(this.Text.Substring(m.Index + 1, m.Length - 2));

                    foreach (Match m2 in mc2)
                    {
                        int real_index = m.Index + 1 + m2.Index;

                        string tag = this.Text.Substring(real_index, m2.Length);

                        r = new Regex(ReservedWordsRegEx, RegexOptions.IgnoreCase);
                        Match m3 = r.Match(tag);

                        toPaint.Add(new ASSTextBoxRegExPaint(real_index, 1, SlashColor, Font));
                        toPaint.Add(new ASSTextBoxRegExPaint(real_index + m3.Index, m3.Length, ReservedWordColor, Font));
                    }

                    r = new Regex("\\b(?:[0-9]*\\.)?[0-9]+\\b");
                    MatchCollection mc3 = r.Matches(this.Text.Substring(m.Index + 1, m.Length - 2));

                    foreach (Match m4 in mc3)
                        toPaint.Add(new ASSTextBoxRegExPaint(m.Index + 1 + m4.Index, m4.Length, IntegerColor, Font));

                }

                toPaint.Add(new ASSTextBoxRegExPaint(m.Index + m.Length - 1, 1, BracketColor, new Font(Font, FontStyle.Bold)));
            }

        }

        public void LoadDictionary(string AffFile, string DicFile)
        {
            if (!enableSpellChecking) return;
            hun = new PerryHunspell(AffFile, DicFile);
        }

        private void PintarSintaxis()
        {
            foreach (ASSTextBoxRegExPaint a in toPaint)
            {
                this.Select(a.SelectFrom, a.SelectLength);
                this.SelectionColor = a.SelectColor;
                this.SelectionFont = a.SelectFont;
                this.SelectionUnderlineStyle = a.UnderlineStyle;
                this.SelectionUnderlineColor = a.UnderlineColor;
            }
        }

        private void ActualizarTextBox()
        {
            int oldSelectStart = SelectionStart;
            int oldSelectLength = SelectionLength;

            // reset a defaults

            SelectAll();
            SelectionColor = DefaultColor;
            SelectionFont = new Font(Font, FontStyle.Regular);

            ProcesarTags();
            PintarSintaxis();

            Select(oldSelectStart, oldSelectLength);
        }

        public void CleanUndoRedoInfo()
        {
            UndoStack.Clear();
            RedoStack.Clear();
        }

        public void ForceRefresh()
        {
            _Paint = false;
            ActualizarTextBox();
            _Paint = true;

        }

        protected override void OnTextChanged(EventArgs e)
        {
            if (!_Paint) return;
            _Paint = false;
            LockWindowUpdate(Handle);
            base.OnTextChanged(e);

            if (!isUndoRedo)
            {
                RedoStack.Clear();
                UndoStack.Push(LastInfo);
                LastInfo = new UndoRedoInfo(this.Text, GetScrollPos(), this.SelectionStart);
            }

            ActualizarTextBox();

            LockWindowUpdate((IntPtr)0);
            this.Invalidate();
            _Paint = true;
        }

        private class UndoRedoInfo
        {
            public UndoRedoInfo(string text, POINT scrollPos, int cursorLoc)
            {
                Text = text;
                ScrollPos = scrollPos;
                CursorLocation = cursorLoc;
            }
            public readonly POINT ScrollPos;
            public readonly int CursorLocation;
            public readonly string Text;
        }

        public new bool CanUndo
        {
            get
            {
                return UndoStack.Count > 0;
            }
        }

        public new bool CanRedo
        {
            get
            {
                return RedoStack.Count > 0;
            }
        }

        public new void Undo()
        {
            if (!CanUndo) return;
            isUndoRedo = true;

            RedoStack.Push(new UndoRedoInfo(this.Text, GetScrollPos(), this.SelectionStart));
            UndoRedoInfo info = (UndoRedoInfo)UndoStack.Pop();
            this.Text = info.Text;
            this.SelectionStart = info.CursorLocation;
            SetScrollPos(info.ScrollPos);

            LastInfo = info;
            isUndoRedo = false;
        }

        public new void Redo()
        {
            if (!CanRedo) return;
            isUndoRedo = true;
            UndoStack.Push(new UndoRedoInfo(this.Text, GetScrollPos(), this.SelectionStart));
            UndoRedoInfo info = (UndoRedoInfo)RedoStack.Pop();
            this.Text = info.Text;
            this.SelectionStart = info.CursorLocation;
            SetScrollPos(info.ScrollPos);
            isUndoRedo = false;
        }

        public string ActualWord(out bool IsValid, out int StartWord)
        {
            // si algo esta seleccionado que tenga mas peso

            if (this.SelectionLength > 0)
            {
                StartWord = this.SelectionStart;
                string mitexto = this.SelectedText.Trim();

                if (enableSpellChecking)
                    IsValid = hun.Spell(mitexto);
                else
                    IsValid = true;

                return mitexto;
            }

            // si no hay nada seleccionado, devolvemos la palabra mas cercana

            int posicionactual = this.SelectionStart;
            string eltexto = this.Text;
            IsValid = false;
            StartWord = 0;

            if (eltexto.Length == 0) return null;

            char[] separadores = new char[] { ' ', '\t', '\n', ',', '.', '¡', ';', ':', '!', '(', ')', '_', '[', '{', ']', '}', '+', '¿', '?', '<', '>', '|', '@', '"', '%', '$', '/', '\\', '=' };
            int derecha = eltexto.IndexOfAny(separadores, posicionactual);
            if (derecha == -1) derecha = Text.Length;
            string textocortado = eltexto.Substring(0, posicionactual);
            int izquierda = textocortado.LastIndexOfAny(separadores);
            if (izquierda == -1) izquierda = 0;

            string textofinal = eltexto.Substring(izquierda, derecha - izquierda).Trim();
            if (izquierda > 0) izquierda++;
            StartWord = izquierda;
            if (enableSpellChecking)
                IsValid = hun.Spell(textofinal);
            else
                IsValid = true;

            return textofinal;
        }

        public List<string> SuggestedWords
        {
            get
            {
                if (!enableSpellChecking)
                    return new List<string>();
                bool tmp; int start;
                return hun.Suggest(ActualWord(out tmp, out start));
            }
        }

        public bool AddWordToDic(string word)
        {
            if (!enableSpellChecking)
                return false;
            bool res = hun.Add(word);
            if (res)
                ActualizarTextBox();
            return res;
        }

        private unsafe POINT GetScrollPos()
        {
            POINT res = new POINT();
            IntPtr ptr = new IntPtr(&res);
            SendMessage(Handle, EM_GETSCROLLPOS, 0, ptr);
            return res;

        }

        private unsafe void SetScrollPos(POINT point)
        {
            IntPtr ptr = new IntPtr(&point);
            SendMessage(Handle, EM_SETSCROLLPOS, 0, ptr);

        }
    }
    class ASSTextBoxRegExPaint
    {
        private int selectFrom;

        public int SelectFrom
        {
            get { return selectFrom; }
        }
        private int selectLength;

        public int SelectLength
        {
            get { return selectLength; }
        }
        private Color selectColor;

        public Color SelectColor
        {
            get { return selectColor; }
        }

        private Font selectFont;

        public Font SelectFont
        {
            get { return selectFont; }
        }

        private RichTextBoxWithSpecialUnderlines.UnderlineStyle underlineStyle;

        public RichTextBoxWithSpecialUnderlines.UnderlineStyle UnderlineStyle
        {
            get { return underlineStyle; }
        }

        private RichTextBoxWithSpecialUnderlines.UnderlineColor underlineColor;

        public RichTextBoxWithSpecialUnderlines.UnderlineColor UnderlineColor
        {
            get { return underlineColor; }
        }

        public ASSTextBoxRegExPaint(int f, int l, Color c, Font fn)
        {
            selectFrom = f;
            selectLength = l;
            selectColor = c;
            selectFont = fn;
            underlineStyle = RichTextBoxWithSpecialUnderlines.UnderlineStyle.None;
            underlineColor = RichTextBoxWithSpecialUnderlines.UnderlineColor.Black;
        }

        public ASSTextBoxRegExPaint(int f, int l, Color c, Font fn, RichTextBoxWithSpecialUnderlines.UnderlineStyle s)
            : this(f, l, c, fn)
        {
            underlineStyle = s;
            underlineColor = RichTextBoxWithSpecialUnderlines.UnderlineColor.Red;
        }

    }
}
