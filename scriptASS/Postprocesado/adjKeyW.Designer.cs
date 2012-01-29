namespace scriptASS
{
    partial class adjKeyW
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupManual = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.textBox4 = new scriptASS.NumericTextBox();
            this.textBox3 = new scriptASS.NumericTextBox();
            this.textBox2 = new scriptASS.NumericTextBox();
            this.textBox1 = new scriptASS.NumericTextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.radioAuto = new System.Windows.Forms.RadioButton();
            this.radioManual = new System.Windows.Forms.RadioButton();
            this.groupAuto = new System.Windows.Forms.GroupBox();
            this.trackFrames = new System.Windows.Forms.TrackBar();
            this.label5 = new System.Windows.Forms.Label();
            this.labelFrames = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupManual.SuspendLayout();
            this.groupAuto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackFrames)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioManual);
            this.groupBox1.Controls.Add(this.radioAuto);
            this.groupBox1.Location = new System.Drawing.Point(135, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(386, 44);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Estilo de configuración";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(42, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(135, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Frames Posteriores al Inicio";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Frames Anteriores al Inicio";
            // 
            // groupManual
            // 
            this.groupManual.Controls.Add(this.label2);
            this.groupManual.Controls.Add(this.label4);
            this.groupManual.Controls.Add(this.label1);
            this.groupManual.Controls.Add(this.label3);
            this.groupManual.Controls.Add(this.textBox2);
            this.groupManual.Controls.Add(this.textBox4);
            this.groupManual.Controls.Add(this.textBox1);
            this.groupManual.Controls.Add(this.textBox3);
            this.groupManual.Enabled = false;
            this.groupManual.Location = new System.Drawing.Point(135, 134);
            this.groupManual.Name = "groupManual";
            this.groupManual.Size = new System.Drawing.Size(386, 78);
            this.groupManual.TabIndex = 1;
            this.groupManual.TabStop = false;
            this.groupManual.Text = "Especificación Manual";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(231, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(132, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Frames Posteriores al Final";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(231, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(127, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Frames Anteriores al Final";
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.CheckOnClick = true;
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(5, 12);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(124, 229);
            this.checkedListBox1.TabIndex = 13;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(135, 218);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(224, 23);
            this.progressBar1.TabIndex = 16;
            // 
            // textBox4
            // 
            this.textBox4.AllowNegative = false;
            this.textBox4.DoublePoint = false;
            this.textBox4.Location = new System.Drawing.Point(195, 45);
            this.textBox4.MaxLength = 2;
            this.textBox4.Name = "textBox4";
            this.textBox4.Point = false;
            this.textBox4.Size = new System.Drawing.Size(30, 20);
            this.textBox4.TabIndex = 1;
            this.textBox4.Text = "0";
            this.textBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox3
            // 
            this.textBox3.AllowNegative = false;
            this.textBox3.DoublePoint = false;
            this.textBox3.Location = new System.Drawing.Point(195, 19);
            this.textBox3.MaxLength = 2;
            this.textBox3.Name = "textBox3";
            this.textBox3.Point = false;
            this.textBox3.Size = new System.Drawing.Size(30, 20);
            this.textBox3.TabIndex = 0;
            this.textBox3.Text = "0";
            this.textBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox2
            // 
            this.textBox2.AllowNegative = false;
            this.textBox2.DoublePoint = false;
            this.textBox2.Location = new System.Drawing.Point(6, 45);
            this.textBox2.MaxLength = 2;
            this.textBox2.Name = "textBox2";
            this.textBox2.Point = false;
            this.textBox2.Size = new System.Drawing.Size(30, 20);
            this.textBox2.TabIndex = 1;
            this.textBox2.Text = "0";
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox1
            // 
            this.textBox1.AllowNegative = false;
            this.textBox1.DoublePoint = false;
            this.textBox1.Location = new System.Drawing.Point(6, 19);
            this.textBox1.MaxLength = 2;
            this.textBox1.Name = "textBox1";
            this.textBox1.Point = false;
            this.textBox1.Size = new System.Drawing.Size(30, 20);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = "0";
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(365, 218);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 17;
            this.button2.Text = "Aplicar";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(446, 218);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 18;
            this.button3.Text = "Salir";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // radioAuto
            // 
            this.radioAuto.AutoSize = true;
            this.radioAuto.Checked = true;
            this.radioAuto.Location = new System.Drawing.Point(6, 19);
            this.radioAuto.Name = "radioAuto";
            this.radioAuto.Size = new System.Drawing.Size(152, 17);
            this.radioAuto.TabIndex = 0;
            this.radioAuto.TabStop = true;
            this.radioAuto.Text = "Automática (recomendado)";
            this.radioAuto.UseVisualStyleBackColor = true;
            this.radioAuto.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioManual
            // 
            this.radioManual.AutoSize = true;
            this.radioManual.Location = new System.Drawing.Point(164, 19);
            this.radioManual.Name = "radioManual";
            this.radioManual.Size = new System.Drawing.Size(60, 17);
            this.radioManual.TabIndex = 1;
            this.radioManual.Text = "Manual";
            this.radioManual.UseVisualStyleBackColor = true;
            this.radioManual.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // groupAuto
            // 
            this.groupAuto.Controls.Add(this.labelFrames);
            this.groupAuto.Controls.Add(this.label5);
            this.groupAuto.Controls.Add(this.trackFrames);
            this.groupAuto.Location = new System.Drawing.Point(135, 62);
            this.groupAuto.Name = "groupAuto";
            this.groupAuto.Size = new System.Drawing.Size(386, 66);
            this.groupAuto.TabIndex = 19;
            this.groupAuto.TabStop = false;
            this.groupAuto.Text = "Especificación Automática";
            // 
            // trackFrames
            // 
            this.trackFrames.Location = new System.Drawing.Point(6, 15);
            this.trackFrames.Maximum = 20;
            this.trackFrames.Minimum = 1;
            this.trackFrames.Name = "trackFrames";
            this.trackFrames.Size = new System.Drawing.Size(240, 45);
            this.trackFrames.TabIndex = 0;
            this.trackFrames.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackFrames.Value = 6;
            this.trackFrames.Scroll += new System.EventHandler(this.trackFrames_Scroll);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(252, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 26);
            this.label5.TabIndex = 1;
            this.label5.Text = "Frames\r\nseleccionados";
            // 
            // labelFrames
            // 
            this.labelFrames.AutoSize = true;
            this.labelFrames.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFrames.Location = new System.Drawing.Point(338, 27);
            this.labelFrames.Name = "labelFrames";
            this.labelFrames.Size = new System.Drawing.Size(20, 24);
            this.labelFrames.TabIndex = 2;
            this.labelFrames.Text = "6";
            // 
            // adjKeyW
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 254);
            this.Controls.Add(this.groupAuto);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.checkedListBox1);
            this.Controls.Add(this.groupManual);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "adjKeyW";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ajustar subtítulos a KeyFrames";
            this.Load += new System.EventHandler(this.adjKeyW_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupManual.ResumeLayout(false);
            this.groupManual.PerformLayout();
            this.groupAuto.ResumeLayout(false);
            this.groupAuto.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackFrames)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupManual;
        private NumericTextBox textBox2;
        private NumericTextBox textBox1;
        private NumericTextBox textBox4;
        private NumericTextBox textBox3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.RadioButton radioManual;
        private System.Windows.Forms.RadioButton radioAuto;
        private System.Windows.Forms.GroupBox groupAuto;
        private System.Windows.Forms.Label labelFrames;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar trackFrames;
    }
}