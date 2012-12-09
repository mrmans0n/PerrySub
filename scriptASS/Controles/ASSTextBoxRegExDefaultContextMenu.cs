using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace scriptASS
{
    class ASSTextBoxRegExDefaultContextMenu : ContextMenu
    {
        private ASSTextBoxRegEx myText;
        private int StartWord = 0;
        private string TheWord;

        public ASSTextBoxRegExDefaultContextMenu(ASSTextBoxRegEx text)
        {
            myText = text;
            InicializaMenu();
        }

        private void InicializaMenu()
        {
            this.MenuItems.Clear();
            this.MenuItems.Add("Deshacer", Undo);                        //0
            this.MenuItems.Add("Rehacer", Redo);
            this.MenuItems.Add("-");
            this.MenuItems.Add("Copiar", Copy);                          //3
            this.MenuItems.Add("Cortar", Cut);                           //4
            this.MenuItems.Add("Pegar", Paste);                          //5
            this.MenuItems.Add("Borrar", Delete);                        //6
            this.MenuItems.Add("-");
            this.MenuItems.Add("Seleccionar todo", SelectAll);           //8
            this.MenuItems.Add("-");
            this.MenuItems.Add("Añadir palabra a diccionario", AddNewWord); //10
            //this.MenuItems.Add("Sugerencias");                          //11

        }

        protected override void OnPopup(EventArgs e)
        {
            InicializaMenu();
            this.MenuItems[0].Enabled = myText.CanUndo;
            this.MenuItems[1].Enabled = myText.CanRedo;
            this.MenuItems[3].Enabled = MenuItems[4].Enabled = MenuItems[6].Enabled = (myText.SelectionLength > 0);
            this.MenuItems[5].Enabled = Clipboard.ContainsText();
            this.MenuItems[8].Enabled = (myText.Text.Length > 0);

            if (myText.EnableSpellChecking)
            {

                bool EsValida;
                TheWord = myText.ActualWord(out EsValida, out StartWord);


                this.MenuItems[10].Enabled = this.MenuItems[10].Visible = !EsValida;

                if (!String.IsNullOrEmpty(TheWord))
                {
                    this.MenuItems[10].Text = "Añadir [" + TheWord + "] a diccionario";

                    List<MenuItem> Suggestions = new List<MenuItem>();
                    foreach (string s in myText.SuggestedWords)
                    {
                        Suggestions.Add(new MenuItem(s, SubstituteWord));
                    }

                    if (Suggestions.Count != 0)
                    {
                        this.MenuItems.Add("Sugerencias", Suggestions.ToArray());
                    }

                }
            }
            else // ocultamos todos los menus
            {
                this.MenuItems[10].Visible = this.MenuItems[9].Visible = false;
            }
            base.OnPopup(e);
        }

        private void AddNewWord(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(TheWord))
            {
                myText.AddWordToDic(TheWord);
            }
        }

        private void SubstituteWord(object sender, EventArgs e)
        {
            int OldSelectStart = myText.SelectionStart;
            int OldSelectLength = myText.SelectionLength;

            MenuItem mi = (MenuItem)sender;
            myText.Select(StartWord, TheWord.Length);
            myText.SelectedText = mi.Text;

            myText.Select(OldSelectStart, OldSelectLength);
        }

        private void Redo(object sender, EventArgs e)
        {
            myText.Redo();
        }

        private void Undo(object sender, EventArgs e)
        {
            myText.Undo();
        }
        private void Copy(object sender, EventArgs e)
        {
            myText.Copy();
        }
        private void Cut(object sender, EventArgs e)
        {
            myText.Cut();
        }
        private void Paste(object sender, EventArgs e)
        {
            myText.Paste();
        }

        private void Delete(object sender, EventArgs e)
        {
            int old_pos = myText.SelectionStart;
            myText.Text = myText.Text.Substring(0, old_pos) + myText.Text.Substring(old_pos + myText.SelectionLength);
            myText.SelectionStart = old_pos;
            myText.SelectionLength = 0;
        }

        private void SelectAll(object sender, EventArgs e)
        {
            myText.SelectAll();
        }

    }
}
