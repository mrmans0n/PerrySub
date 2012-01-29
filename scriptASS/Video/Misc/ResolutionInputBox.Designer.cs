namespace scriptASS
{
    partial class ResolutionInputBox
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.ancho = new scriptASS.NumericTextBox();
            this.alto = new scriptASS.NumericTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.aspect = new System.Windows.Forms.ComboBox();
            this.mod16 = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::scriptASS.Properties.Resources.Help;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(35, 34);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(90, 68);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Aceptar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(171, 68);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Cancelar";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // ancho
            // 
            this.ancho.AllowNegative = false;
            this.ancho.DoublePoint = false;
            this.ancho.Location = new System.Drawing.Point(121, 12);
            this.ancho.MaxLength = 4;
            this.ancho.Name = "ancho";
            this.ancho.Point = false;
            this.ancho.Size = new System.Drawing.Size(44, 20);
            this.ancho.TabIndex = 3;
            this.ancho.Text = "0";
            // 
            // alto
            // 
            this.alto.AllowNegative = false;
            this.alto.DoublePoint = false;
            this.alto.Location = new System.Drawing.Point(121, 38);
            this.alto.MaxLength = 4;
            this.alto.Name = "alto";
            this.alto.Point = false;
            this.alto.Size = new System.Drawing.Size(44, 20);
            this.alto.TabIndex = 4;
            this.alto.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(63, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Ancho";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(76, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Alto";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(171, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Aspect Ratio";
            // 
            // aspect
            // 
            this.aspect.FormattingEnabled = true;
            this.aspect.Items.AddRange(new object[] {
            "16:9",
            "4:3",
            "Otro"});
            this.aspect.Location = new System.Drawing.Point(245, 11);
            this.aspect.Name = "aspect";
            this.aspect.Size = new System.Drawing.Size(56, 21);
            this.aspect.TabIndex = 8;
            this.aspect.Text = "16:9";
            // 
            // mod16
            // 
            this.mod16.AutoSize = true;
            this.mod16.Location = new System.Drawing.Point(199, 40);
            this.mod16.Name = "mod16";
            this.mod16.Size = new System.Drawing.Size(62, 17);
            this.mod16.TabIndex = 9;
            this.mod16.Text = "Mod 16";
            this.mod16.UseVisualStyleBackColor = true;
            // 
            // ResolutionInputBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(325, 99);
            this.Controls.Add(this.mod16);
            this.Controls.Add(this.aspect);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.alto);
            this.Controls.Add(this.ancho);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ResolutionInputBox";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ResolutionInputBox";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private NumericTextBox ancho;
        private NumericTextBox alto;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox aspect;
        private System.Windows.Forms.CheckBox mod16;
    }
}