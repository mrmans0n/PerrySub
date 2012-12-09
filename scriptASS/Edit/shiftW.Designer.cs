namespace scriptASS
{
    partial class shiftW
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
            this.radioResta = new System.Windows.Forms.RadioButton();
            this.radioSuma = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioPosteriores = new System.Windows.Forms.RadioButton();
            this.radioAnteriores = new System.Windows.Forms.RadioButton();
            this.radioTodas = new System.Windows.Forms.RadioButton();
            this.radioSeleccionadas = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioFrames = new System.Windows.Forms.RadioButton();
            this.radioTiempo = new System.Windows.Forms.RadioButton();
            this.timeText = new scriptASS.TimeTextBox();
            this.numericFrames = new scriptASS.NumericTextBox();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // radioResta
            // 
            this.radioResta.AutoSize = true;
            this.radioResta.Location = new System.Drawing.Point(67, 14);
            this.radioResta.Name = "radioResta";
            this.radioResta.Size = new System.Drawing.Size(56, 17);
            this.radioResta.TabIndex = 1;
            this.radioResta.Text = "Restar";
            this.radioResta.UseVisualStyleBackColor = true;
            // 
            // radioSuma
            // 
            this.radioSuma.AutoSize = true;
            this.radioSuma.Checked = true;
            this.radioSuma.Location = new System.Drawing.Point(6, 14);
            this.radioSuma.Name = "radioSuma";
            this.radioSuma.Size = new System.Drawing.Size(55, 17);
            this.radioSuma.TabIndex = 0;
            this.radioSuma.TabStop = true;
            this.radioSuma.Text = "Sumar";
            this.radioSuma.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(112, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(13, 13);
            this.label2.TabIndex = 41;
            this.label2.Text = "±";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(232, 199);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(112, 23);
            this.button2.TabIndex = 39;
            this.button2.Text = "Aplicar";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(112, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(13, 13);
            this.label1.TabIndex = 38;
            this.label1.Text = "±";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(232, 9);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(112, 186);
            this.listBox1.TabIndex = 36;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioSuma);
            this.groupBox2.Controls.Add(this.radioResta);
            this.groupBox2.Location = new System.Drawing.Point(12, 92);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(214, 36);
            this.groupBox2.TabIndex = 35;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Acción";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioPosteriores);
            this.groupBox1.Controls.Add(this.radioAnteriores);
            this.groupBox1.Controls.Add(this.radioTodas);
            this.groupBox1.Controls.Add(this.radioSeleccionadas);
            this.groupBox1.Location = new System.Drawing.Point(12, 134);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(214, 88);
            this.groupBox1.TabIndex = 34;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Aplicar a";
            // 
            // radioPosteriores
            // 
            this.radioPosteriores.AutoSize = true;
            this.radioPosteriores.Location = new System.Drawing.Point(6, 64);
            this.radioPosteriores.Name = "radioPosteriores";
            this.radioPosteriores.Size = new System.Drawing.Size(181, 17);
            this.radioPosteriores.TabIndex = 3;
            this.radioPosteriores.TabStop = true;
            this.radioPosteriores.Text = "Línea seleccionada y posteriores";
            this.radioPosteriores.UseVisualStyleBackColor = true;
            // 
            // radioAnteriores
            // 
            this.radioAnteriores.AutoSize = true;
            this.radioAnteriores.Location = new System.Drawing.Point(6, 49);
            this.radioAnteriores.Name = "radioAnteriores";
            this.radioAnteriores.Size = new System.Drawing.Size(176, 17);
            this.radioAnteriores.TabIndex = 2;
            this.radioAnteriores.TabStop = true;
            this.radioAnteriores.Text = "Línea seleccionada y anteriores";
            this.radioAnteriores.UseVisualStyleBackColor = true;
            // 
            // radioTodas
            // 
            this.radioTodas.AutoSize = true;
            this.radioTodas.Checked = true;
            this.radioTodas.Location = new System.Drawing.Point(6, 19);
            this.radioTodas.Name = "radioTodas";
            this.radioTodas.Size = new System.Drawing.Size(55, 17);
            this.radioTodas.TabIndex = 1;
            this.radioTodas.TabStop = true;
            this.radioTodas.Text = "Todas";
            this.radioTodas.UseVisualStyleBackColor = true;
            // 
            // radioSeleccionadas
            // 
            this.radioSeleccionadas.AutoSize = true;
            this.radioSeleccionadas.Location = new System.Drawing.Point(6, 34);
            this.radioSeleccionadas.Name = "radioSeleccionadas";
            this.radioSeleccionadas.Size = new System.Drawing.Size(95, 17);
            this.radioSeleccionadas.TabIndex = 0;
            this.radioSeleccionadas.Text = "Seleccionadas";
            this.radioSeleccionadas.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioFrames);
            this.groupBox3.Controls.Add(this.radioTiempo);
            this.groupBox3.Controls.Add(this.timeText);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.numericFrames);
            this.groupBox3.Location = new System.Drawing.Point(12, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(214, 83);
            this.groupBox3.TabIndex = 42;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Tipo de desplazamiento";
            // 
            // radioFrames
            // 
            this.radioFrames.AutoSize = true;
            this.radioFrames.Location = new System.Drawing.Point(17, 49);
            this.radioFrames.Name = "radioFrames";
            this.radioFrames.Size = new System.Drawing.Size(75, 17);
            this.radioFrames.TabIndex = 43;
            this.radioFrames.Text = "Por frames";
            this.radioFrames.UseVisualStyleBackColor = true;
            this.radioFrames.CheckedChanged += new System.EventHandler(this.desplazamiento_CheckedChanged);
            // 
            // radioTiempo
            // 
            this.radioTiempo.AutoSize = true;
            this.radioTiempo.Checked = true;
            this.radioTiempo.Location = new System.Drawing.Point(17, 23);
            this.radioTiempo.Name = "radioTiempo";
            this.radioTiempo.Size = new System.Drawing.Size(75, 17);
            this.radioTiempo.TabIndex = 42;
            this.radioTiempo.TabStop = true;
            this.radioTiempo.Text = "Por tiempo";
            this.radioTiempo.UseVisualStyleBackColor = true;
            // 
            // timeText
            // 
            this.timeText.Location = new System.Drawing.Point(126, 22);
            this.timeText.MinimumSize = new System.Drawing.Size(72, 20);
            this.timeText.Name = "timeText";
            this.timeText.ReadOnly = false;
            this.timeText.Size = new System.Drawing.Size(72, 20);
            this.timeText.TabIndex = 33;
            // 
            // numericFrames
            // 
            this.numericFrames.AllowNegative = false;
            this.numericFrames.DoublePoint = false;
            this.numericFrames.Location = new System.Drawing.Point(126, 48);
            this.numericFrames.Name = "numericFrames";
            this.numericFrames.Point = false;
            this.numericFrames.Size = new System.Drawing.Size(72, 20);
            this.numericFrames.TabIndex = 40;
            this.numericFrames.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericFrames.TextChanged += new System.EventHandler(this.numericFrames_TextChanged);
            // 
            // shiftW
            // 
            this.AcceptButton = this.button2;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 233);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.listBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "shiftW";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Shift Times";
            this.Load += new System.EventHandler(this.shiftW_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton radioResta;
        private System.Windows.Forms.RadioButton radioSuma;
        private TimeTextBox timeText;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioTodas;
        private System.Windows.Forms.RadioButton radioSeleccionadas;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioPosteriores;
        private System.Windows.Forms.RadioButton radioAnteriores;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label2;
        private NumericTextBox numericFrames;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton radioFrames;
        private System.Windows.Forms.RadioButton radioTiempo;
    }
}