namespace scriptASS
{
    partial class replaceW
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(replaceW));
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rangoSeleccionadas = new System.Windows.Forms.RadioButton();
            this.rangoTodas = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buscarPersonajes = new System.Windows.Forms.RadioButton();
            this.buscarEstilos = new System.Windows.Forms.RadioButton();
            this.buscarTexto = new System.Windows.Forms.RadioButton();
            this.regExp = new System.Windows.Forms.CheckBox();
            this.caseInSensitive = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.comboReplace = new System.Windows.Forms.ComboBox();
            this.btn3 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.comboReplaceTo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(340, 37);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(137, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "Avanzar a siguiente";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rangoSeleccionadas);
            this.groupBox2.Controls.Add(this.rangoTodas);
            this.groupBox2.Location = new System.Drawing.Point(138, 97);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(196, 46);
            this.groupBox2.TabIndex = 48;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Rango de búsqueda";
            // 
            // rangoSeleccionadas
            // 
            this.rangoSeleccionadas.AutoSize = true;
            this.rangoSeleccionadas.Location = new System.Drawing.Point(89, 19);
            this.rangoSeleccionadas.Name = "rangoSeleccionadas";
            this.rangoSeleccionadas.Size = new System.Drawing.Size(95, 17);
            this.rangoSeleccionadas.TabIndex = 1;
            this.rangoSeleccionadas.Text = "Seleccionadas";
            this.rangoSeleccionadas.UseVisualStyleBackColor = true;
            // 
            // rangoTodas
            // 
            this.rangoTodas.AutoSize = true;
            this.rangoTodas.Checked = true;
            this.rangoTodas.Location = new System.Drawing.Point(6, 19);
            this.rangoTodas.Name = "rangoTodas";
            this.rangoTodas.Size = new System.Drawing.Size(55, 17);
            this.rangoTodas.TabIndex = 0;
            this.rangoTodas.TabStop = true;
            this.rangoTodas.Text = "Todas";
            this.rangoTodas.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buscarPersonajes);
            this.groupBox1.Controls.Add(this.buscarEstilos);
            this.groupBox1.Controls.Add(this.buscarTexto);
            this.groupBox1.Location = new System.Drawing.Point(12, 97);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(120, 92);
            this.groupBox1.TabIndex = 47;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "¿Dónde buscar?";
            // 
            // buscarPersonajes
            // 
            this.buscarPersonajes.AutoSize = true;
            this.buscarPersonajes.Location = new System.Drawing.Point(6, 64);
            this.buscarPersonajes.Name = "buscarPersonajes";
            this.buscarPersonajes.Size = new System.Drawing.Size(77, 17);
            this.buscarPersonajes.TabIndex = 2;
            this.buscarPersonajes.TabStop = true;
            this.buscarPersonajes.Text = "Personajes";
            this.buscarPersonajes.UseVisualStyleBackColor = true;
            // 
            // buscarEstilos
            // 
            this.buscarEstilos.AutoSize = true;
            this.buscarEstilos.Location = new System.Drawing.Point(6, 41);
            this.buscarEstilos.Name = "buscarEstilos";
            this.buscarEstilos.Size = new System.Drawing.Size(55, 17);
            this.buscarEstilos.TabIndex = 1;
            this.buscarEstilos.TabStop = true;
            this.buscarEstilos.Text = "Estilos";
            this.buscarEstilos.UseVisualStyleBackColor = true;
            // 
            // buscarTexto
            // 
            this.buscarTexto.AutoSize = true;
            this.buscarTexto.Checked = true;
            this.buscarTexto.Location = new System.Drawing.Point(6, 20);
            this.buscarTexto.Name = "buscarTexto";
            this.buscarTexto.Size = new System.Drawing.Size(52, 17);
            this.buscarTexto.TabIndex = 0;
            this.buscarTexto.TabStop = true;
            this.buscarTexto.Text = "Texto";
            this.buscarTexto.UseVisualStyleBackColor = true;
            // 
            // regExp
            // 
            this.regExp.AutoSize = true;
            this.regExp.Location = new System.Drawing.Point(138, 172);
            this.regExp.Name = "regExp";
            this.regExp.Size = new System.Drawing.Size(129, 17);
            this.regExp.TabIndex = 45;
            this.regExp.Text = "Expresiones regulares";
            this.regExp.UseVisualStyleBackColor = true;
            // 
            // caseInSensitive
            // 
            this.caseInSensitive.AutoSize = true;
            this.caseInSensitive.Checked = true;
            this.caseInSensitive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.caseInSensitive.Location = new System.Drawing.Point(138, 149);
            this.caseInSensitive.Name = "caseInSensitive";
            this.caseInSensitive.Size = new System.Drawing.Size(174, 17);
            this.caseInSensitive.TabIndex = 44;
            this.caseInSensitive.Text = "Ignorar mayúsculas/minúsculas";
            this.caseInSensitive.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(340, 9);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(137, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Buscar coincidencias";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboReplace
            // 
            this.comboReplace.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.comboReplace.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboReplace.FormattingEnabled = true;
            this.comboReplace.Location = new System.Drawing.Point(12, 26);
            this.comboReplace.Name = "comboReplace";
            this.comboReplace.Size = new System.Drawing.Size(322, 21);
            this.comboReplace.TabIndex = 0;
            // 
            // btn3
            // 
            this.btn3.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn3.Image = ((System.Drawing.Image)(resources.GetObject("btn3.Image")));
            this.btn3.Location = new System.Drawing.Point(340, 132);
            this.btn3.Name = "btn3";
            this.btn3.Size = new System.Drawing.Size(137, 57);
            this.btn3.TabIndex = 6;
            this.btn3.Text = "Cancelar";
            this.btn3.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn3.UseVisualStyleBackColor = true;
            this.btn3.Click += new System.EventHandler(this.btn3_Click);
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(340, 95);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(137, 23);
            this.button3.TabIndex = 5;
            this.button3.Text = "Reemplazar todos";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // comboReplaceTo
            // 
            this.comboReplaceTo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.comboReplaceTo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboReplaceTo.FormattingEnabled = true;
            this.comboReplaceTo.Location = new System.Drawing.Point(12, 68);
            this.comboReplaceTo.Name = "comboReplaceTo";
            this.comboReplaceTo.Size = new System.Drawing.Size(322, 21);
            this.comboReplaceTo.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 52;
            this.label1.Text = "Reemplazar...";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 53;
            this.label2.Text = "por...";
            // 
            // button4
            // 
            this.button4.Enabled = false;
            this.button4.Location = new System.Drawing.Point(340, 66);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(137, 23);
            this.button4.TabIndex = 4;
            this.button4.Text = "Reemplazar";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // replaceW
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btn3;
            this.ClientSize = new System.Drawing.Size(483, 197);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboReplaceTo);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btn3);
            this.Controls.Add(this.regExp);
            this.Controls.Add(this.caseInSensitive);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboReplace);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "replaceW";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reemplazar";
            this.Load += new System.EventHandler(this.replaceW_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rangoSeleccionadas;
        private System.Windows.Forms.RadioButton rangoTodas;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton buscarPersonajes;
        private System.Windows.Forms.RadioButton buscarEstilos;
        private System.Windows.Forms.RadioButton buscarTexto;
        private System.Windows.Forms.Button btn3;
        private System.Windows.Forms.CheckBox regExp;
        private System.Windows.Forms.CheckBox caseInSensitive;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox comboReplace;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ComboBox comboReplaceTo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button4;
    }
}