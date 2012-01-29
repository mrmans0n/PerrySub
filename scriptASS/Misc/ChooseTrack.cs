using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using MediaInfoWrapper;

namespace scriptASS
{
    public partial class ChooseTrack : Form
    {
        public bool isExec;

        public ChooseTrack(ArrayList tracks)
        {
            isExec = true;
            InitializeComponent();
            
            foreach (TextTrack tt in tracks)
            {
                listaTracks.Items.Add("[#" + tt.ID + "] Título: "+tt.Title+" - Idioma: "+tt.LanguageString+" - Tipo: " + tt.CodecString);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listaTracks.SelectedIndex == -1)
            {
                MessageBox.Show("Debes seleccionar uno de los subtítulos", mainW.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.Hide();
            isExec = false;
        }
    }
}