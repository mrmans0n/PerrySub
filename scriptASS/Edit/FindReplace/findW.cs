using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Text.RegularExpressions;

namespace scriptASS
{
    public partial class findW : Form
    {
        mainW mw;
        ArrayList found = new ArrayList();

        public findW(mainW m)
        {
            InitializeComponent();
            mw = m;

            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;

            comboFind.TextChanged += new EventHandler(comboFind_TextChanged);
        }

        void comboFind_TextChanged(object sender, EventArgs e)
        {
            button2.Enabled = false;
            found.Clear();
        }

        private void btn3_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            found.Clear();

            for (int i = 0; i < mw.script.LineCount; i++)
            {
                bool ismatch = false;
                lineaASS actual = (lineaASS)mw.script.GetLines()[i];

                if (buscarTexto.Checked)
                {
                    if (regExp.Checked)
                    {
                        try 
                        {
                            Regex r = new Regex(comboFind.Text);
                            ismatch = r.IsMatch(actual.texto);
                        } 
                        catch 
                        {
                            mw.errorMsg("Fallo al compilar la expresión regular");
                            return;
                        }
                    }
                    else
                    {
                        ismatch = (caseInSensitive.Checked) ?
                            (actual.texto.ToLower().Contains(comboFind.Text.ToLower())) :
                            (actual.texto.Contains(comboFind.Text));
                    }
                }
                else if (buscarEstilos.Checked)
                {
                    if (regExp.Checked)
                    {
                        try
                        {
                            Regex r = new Regex(comboFind.Text);
                            ismatch = r.IsMatch(actual.estilo);
                        }
                        catch
                        {
                            mw.errorMsg("Fallo al compilar la expresión regular");
                            return;
                        }
                    }
                    else
                    {
                        ismatch = (caseInSensitive.Checked) ?
                            (actual.estilo.ToLower().Contains(comboFind.Text.ToLower())) :
                            (actual.estilo.Contains(comboFind.Text));
                    }
                }
                else if (buscarPersonajes.Checked)
                {
                    if (regExp.Checked)
                    {
                        try
                        {
                            Regex r = new Regex(comboFind.Text);
                            ismatch = r.IsMatch(actual.personaje);
                        }
                        catch
                        {
                            mw.errorMsg("Fallo al compilar la expresión regular");
                            return;
                        }
                    }
                    else
                    {
                        ismatch = (caseInSensitive.Checked) ?
                            (actual.personaje.ToLower().Contains(comboFind.Text.ToLower())) :
                            (actual.personaje.Contains(comboFind.Text));
                    }
                }

                if (ismatch) found.Add(i);
            }
            mw.doFind(found, rangoSeleccionadas.Checked);
            button2.Enabled = (found.Count>0);

            // ※ - comas
            // 卍 - comillas
            string parsed_text = comboFind.Text.Replace(',','※').Replace('"','卍');
            mw.updateConcatenateConfigFile("findW_FindHistorial", parsed_text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            mw.doFindNext();
        }

        private void findW_Load(object sender, EventArgs e)
        {
            try
            {
                string[] historial = mw.getFromConfigFileA("findW_FindHistorial");
                Array.Reverse(historial);
                for (int i = 0; i < historial.Length; i++)
                {
                    comboFind.Items.Add(historial[i].Replace('※',',').Replace('卍','"'));
                }
            }
            catch { }
        }
    }
}