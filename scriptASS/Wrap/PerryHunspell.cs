using System;
using System.Collections.Generic;
using System.Text;
using NHunspell;
using System.IO;

namespace scriptASS
{
    public class PerryHunspell : Hunspell
    {

        string AffFile, DictFile;
        string UserDictionary;
        Dictionary<string,string> UserTerms = new Dictionary<string, string>();
        bool ExistsUserDictionary = false;

        public PerryHunspell(string AffFile, string DictFile)
            : base(AffFile, DictFile)
        {
            this.AffFile = AffFile;
            this.DictFile = DictFile;
            Initialize();
        }

        public PerryHunspell(string AffFile, string DictFile, string key)
            : base(AffFile, DictFile, key)
        {
            this.AffFile = AffFile;
            this.DictFile = DictFile;
            Initialize();
        }

        private void Initialize()
        {
            UserDictionary = GetUserDictionaryPath(DictFile);
            ExistsUserDictionary = LoadUserDictionary();
        }

        private string GetUserDictionaryPath(string DictFilePath)
        {
            return Path.ChangeExtension(DictFilePath,".userdic");
        }

        private bool LoadUserDictionary()
        {
            try
            {
                UserTerms = new Dictionary<string, string>();

                StreamReader sr2 = new StreamReader(File.OpenRead(UserDictionary));
                string[] terminos = sr2.ReadToEnd().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                sr2.Close();

                foreach (string termino in terminos)
                {
                    UserTerms.Add(termino, termino);
                }

            }
            catch
            {
                return false;
            }
            return true;
        }

        private bool SaveUserDictionary()
        {
            try
            {
                TextWriter o = new StreamWriter(UserDictionary, false, System.Text.Encoding.UTF8);
                foreach (string termino in UserTerms.Values)
                    o.WriteLine(termino);
                o.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }

        private bool AddToUserDictionary(string Word)
        {
            if (String.IsNullOrEmpty(Word)) return false;
            UserTerms.Add(Word, Word);
            return SaveUserDictionary();
        }

        public new bool Spell(string Word)
        {
            if (UserTerms.ContainsKey(Word))
                return true;

            return base.Spell(Word);
        }

        public new bool Add(string Word)
        {
            bool res = AddToUserDictionary(Word);
            base.Add(Word);
            return res;
        }

        public new bool AddWithAffix(string Word, string Affix)
        {
            bool res = AddToUserDictionary(Word);
            base.AddWithAffix(Word, Affix);
            return res;
        }

    }
}
