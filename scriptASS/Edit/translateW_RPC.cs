using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Threading;

namespace scriptASS
{
    partial class translateW
    {
        ArrayList dic;
        bool LegitUpdate = false;

        private void InitRPC()
        {
            string servidor = "";
            try
            {
                servidor = mW.getFromConfigFile("DictionaryServer");
            }
            catch
            {
                servidor = mainW.defaultDictionaries;
                mW.updateReplaceConfigFile("DictionaryServer", servidor); // 1st exec ... maybe xD
            }
            dictAddr.Text = servidor;

            cliente = new RPC_Client(servidor, 26108);
            if (cliente.Connect())
            {
                dic = cliente.GetDictionaries();
                cliente.Disconnect();
                foreach (string s in dic)
                    if (!s.Equals("")) listBox4.Items.Add(s);
            }
            listBox4.SelectedIndexChanged += new EventHandler(listBox4_SelectedIndexChanged);
            dataGridView2.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(dataGridView2_CellValueChanged);
            addDictText.KeyPress += new System.Windows.Forms.KeyPressEventHandler(addDictText_KeyPress);
            estadoConexion.Tick += new EventHandler(estadoConexion_Tick);
            estadoConexion.Interval = 1000;
            estadoConexion.Enabled = true;

        }

        void estadoConexion_Tick(object sender, EventArgs e)
        {
            pictureBox1.BackColor = (cliente.IsConnected) ? Color.Green : Color.Red;
        }

        void addDictText_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            switch (Convert.ToInt32(e.KeyChar))
            {
                case 13:
                    if (!cliente.Connect())
                    {
                        mW.errorMsg("Error accediendo a la BBDD.");
                        return;
                    }
                    if (!cliente.ExistDictionary(addDictText.Text))
                    {
                        cliente.CreateDictionary(addDictText.Text);
                        LegitUpdate = true;
                        ArrayList dic = cliente.GetDictionaries();
                        listBox4.Items.Clear();
                        foreach (string s in dic)
                            if (!s.Equals("")) listBox4.Items.Add(s);
                        LegitUpdate = false;
                    }
                    else
                    {
                        mW.errorMsg("El nombre de diccionario especificado ya existe en la base de datos.\nTendrás que usar un nombre distinto :)");
                        cliente.Disconnect();
                        return;
                    }
                    cliente.Disconnect();
                    SendKeys.Send("");
                    e.Handled = true;
                    addDict.Close();
                    break;
            }
        }

        void dataGridView2_CellValueChanged(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            // comprobar si es columna de terminos, si termino existe, y to eso
            if (LegitUpdate) return;

            string valor = (dataGridView2[e.ColumnIndex, e.RowIndex].Value == null) ? null : dataGridView2[e.ColumnIndex, e.RowIndex].Value.ToString();

            if (e.ColumnIndex == 0)
                if (IsValueThere(valor, e.RowIndex))
                {
                    mW.errorMsg("Ya existe una definicion para el término " + valor + ", así que deberás cambiar la que existe en vez de añadir una nueva");
                    dataGridView2[e.ColumnIndex, e.RowIndex].Value = null;
                    return;
                }                

        }

        void listBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReloadValues();
        }

        ActionProgressW SaveProgress;
        Thread SaveInDictionary;
        string WorkDict;
        Hashtable KeyDefin;

        private void button14_Click(object sender, EventArgs e)
        {
            // guardar cambios
            
            if (listBox4.SelectedIndex == -1) return;
            if (!cliente.Connect())
            {
                mW.errorMsg("No se ha podido establecer la conexión con el servidor");
                return;
            }

            WorkDict = listBox4.SelectedItem.ToString();

            KeyDefin = new Hashtable();
            for (int i = 0; i < dataGridView2.Rows.Count; i++)
            {
                if (dataGridView2[0, i].Value != null && dataGridView2[1, i].Value != null)
                {
                    string key = dataGridView2[0, i].Value.ToString();
                    string valor = dataGridView2[1, i].Value.ToString();

                    if (!key.Equals("")) KeyDefin.Add(key, valor);

                }
            }

            SaveProgress = new ActionProgressW(mW, "Guardando datos en el diccionario de términos...");
            SaveProgress.Show();
            SaveProgress.CancelButton.Enabled = false;

            SaveInDictionary = new Thread(new ThreadStart(T_UpdateDictionary));
            SaveInDictionary.Start();

        }

        void T_UpdateDictionary()
        {
            ArrayList deleted = cliente.GetDefinitionListOnly(WorkDict);
            int maxxx = dataGridView2.Rows.Count;
            int count = 0;

            foreach (string key in KeyDefin.Keys)
            {
                //if (SaveProgress.Abort) break;
                SaveProgress.UpdatePerc((int)Math.Round((double)(count++*100) / (double)maxxx));

                string valor = KeyDefin[key].ToString();

                if (cliente.ExistDefinition(WorkDict, key))
                {
                    if (cliente.IsDefinitionModified(WorkDict, key, valor))
                        cliente.EditDefinition(WorkDict, key, valor); // existen pero han cambiado
                    deleted.Remove(key);
                }
                else cliente.InsertDefinition(WorkDict, key, valor); // no existian         
            }
            foreach (string s in deleted)
                cliente.DeleteDefinition(WorkDict, s);

            cliente.Disconnect();
            bool bleeeh = SaveProgress.Abort;

            SaveProgress.GoHide();
            if (!bleeeh) MessageBox.Show("Datos actualizados con éxito", "PerrySubRPC", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            // recargar valores
            ReloadValues();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            // añadir diccionario
            addDict.Show(button16, new Point(button16.Width, 0));
            addDictText.Focus();
            addDictText.SelectAll();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            // borrar diccionario
            if (!cliente.Connect()) return;
            if (MessageBox.Show("¿Seguro que deseas borrar el diccionario " + listBox4.SelectedItem.ToString() + "?\nLos datos no podrán recuperarse si lo haces, y es probable que alguien se cague en tu puta madre.", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                cliente.DeleteDictionary(listBox4.SelectedItem.ToString());
                dataGridView2.Rows.Clear();
                LegitUpdate = true;

                ArrayList dic = cliente.GetDictionaries();
                listBox4.Items.Clear();
                foreach (string s in dic)
                    if (!s.Equals("")) listBox4.Items.Add(s);

                LegitUpdate = false;

            }
            cliente.Disconnect();
        }


        private bool IsValueThere(string valor, int idx)
        {
            for (int i = 0; i < dataGridView2.Rows.Count; i++)
                if (idx != i && dataGridView2[0, i].Value != null && valor!=null)
                    if (valor.Equals(dataGridView2[0, i].Value.ToString()))
                        return true;
            return false;
        }

        private void ReloadValues()
        {
            if (listBox4.SelectedIndex == -1) return;
            if (!cliente.Connect()) return;

            dataGridView2.Enabled = true;

            Hashtable bleh = cliente.GetAllDefinitions(listBox4.Text);
            LegitUpdate = true;

            dataGridView2.Rows.Clear();
            foreach (string key in bleh.Keys)
                dataGridView2.Rows.Add(new string[] { key, (string)bleh[key] });

            LegitUpdate = false;
            cliente.Disconnect();
        }

        private void button18_Click(object sender, EventArgs e)
        {
            // añadir a autocompletar
            for (int idx = 0; idx < dataGridView2.SelectedRows.Count; idx++)
            {
                int i = dataGridView2.SelectedRows[idx].Index;
                if (dataGridView2[0, i].Value != null)
                    if (!dataGridView2[0, i].Value.ToString().Equals(""))
                        if (!autoComplete.Contains(dataGridView2[0, i].Value.ToString()))
                            autoComplete.Add(dataGridView2[0, i].Value.ToString());
            }

            updateAutoCompleteList();
        }

        private void button19_Click(object sender, EventArgs e)
        {
            // autocompletar a dict.
            foreach (string s in autoComplete)
                if (!IsValueThere(s, -1))
                    dataGridView2.Rows.Add(new string[] { s, "Importado del AutoCompletar" });

        }

    }
}
