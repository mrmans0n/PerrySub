namespace scriptASS.Edit
{
    partial class v2W
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
            this.particulaL = new System.Windows.Forms.Label();
            this.birthrate = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.generar = new System.Windows.Forms.Button();
            this.animation = new System.Windows.Forms.Button();
            this.fril = new System.Windows.Forms.Label();
            this.frfl = new System.Windows.Forms.Label();
            this.frf = new scriptASS.NumericTextBox();
            this.fri = new scriptASS.NumericTextBox();
            this.numericTextBox1 = new scriptASS.NumericTextBox();
            this.particulaText = new scriptASS.ASSTextBoxRegEx();
            this.SuspendLayout();
            // 
            // particulaL
            // 
            this.particulaL.AutoSize = true;
            this.particulaL.Location = new System.Drawing.Point(15, 8);
            this.particulaL.Name = "particulaL";
            this.particulaL.Size = new System.Drawing.Size(51, 13);
            this.particulaL.TabIndex = 1;
            this.particulaL.Text = "Particula:";
            // 
            // birthrate
            // 
            this.birthrate.AutoSize = true;
            this.birthrate.Location = new System.Drawing.Point(15, 112);
            this.birthrate.Name = "birthrate";
            this.birthrate.Size = new System.Drawing.Size(126, 13);
            this.birthrate.TabIndex = 3;
            this.birthrate.Text = "Frecuencia de particulas:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(253, 112);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "por segundo";
            // 
            // generar
            // 
            this.generar.Location = new System.Drawing.Point(147, 202);
            this.generar.Name = "generar";
            this.generar.Size = new System.Drawing.Size(75, 23);
            this.generar.TabIndex = 6;
            this.generar.Text = "Generar";
            this.generar.UseVisualStyleBackColor = true;
            this.generar.Click += new System.EventHandler(this.generar_Click);
            // 
            // animation
            // 
            this.animation.Location = new System.Drawing.Point(18, 202);
            this.animation.Name = "animation";
            this.animation.Size = new System.Drawing.Size(95, 23);
            this.animation.TabIndex = 7;
            this.animation.Text = "Crear Animación";
            this.animation.UseVisualStyleBackColor = true;
            this.animation.Click += new System.EventHandler(this.animation_Click);
            // 
            // fril
            // 
            this.fril.AutoSize = true;
            this.fril.Location = new System.Drawing.Point(15, 147);
            this.fril.Name = "fril";
            this.fril.Size = new System.Drawing.Size(67, 13);
            this.fril.TabIndex = 8;
            this.fril.Text = "Frame Inicio:";
            // 
            // frfl
            // 
            this.frfl.AutoSize = true;
            this.frfl.Location = new System.Drawing.Point(166, 147);
            this.frfl.Name = "frfl";
            this.frfl.Size = new System.Drawing.Size(56, 13);
            this.frfl.TabIndex = 8;
            this.frfl.Text = "Frame Fin:";
            // 
            // frf
            // 
            this.frf.AllowNegative = false;
            this.frf.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.frf.DoublePoint = false;
            this.frf.Location = new System.Drawing.Point(228, 145);
            this.frf.Name = "frf";
            this.frf.Point = false;
            this.frf.Size = new System.Drawing.Size(60, 20);
            this.frf.TabIndex = 9;
            // 
            // fri
            // 
            this.fri.AllowNegative = false;
            this.fri.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fri.DoublePoint = false;
            this.fri.Location = new System.Drawing.Point(88, 145);
            this.fri.Name = "fri";
            this.fri.Point = false;
            this.fri.Size = new System.Drawing.Size(60, 20);
            this.fri.TabIndex = 9;
            // 
            // numericTextBox1
            // 
            this.numericTextBox1.AllowNegative = false;
            this.numericTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericTextBox1.DoublePoint = false;
            this.numericTextBox1.Location = new System.Drawing.Point(147, 109);
            this.numericTextBox1.Name = "numericTextBox1";
            this.numericTextBox1.Point = false;
            this.numericTextBox1.Size = new System.Drawing.Size(100, 20);
            this.numericTextBox1.TabIndex = 4;
            // 
            // particulaText
            // 
            this.particulaText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.particulaText.Location = new System.Drawing.Point(18, 24);
            this.particulaText.Name = "particulaText";
            this.particulaText.Size = new System.Drawing.Size(636, 74);
            this.particulaText.TabIndex = 0;
            this.particulaText.Text = "";
            this.particulaText.TextChanged += new System.EventHandler(this.particulaText_TextChanged);
            // 
            // v2W
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(758, 250);
            this.Controls.Add(this.frf);
            this.Controls.Add(this.fri);
            this.Controls.Add(this.frfl);
            this.Controls.Add(this.fril);
            this.Controls.Add(this.animation);
            this.Controls.Add(this.generar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericTextBox1);
            this.Controls.Add(this.birthrate);
            this.Controls.Add(this.particulaL);
            this.Controls.Add(this.particulaText);
            this.Name = "v2W";
            this.Text = "v2W";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.v2W_FormClosing);
            this.Load += new System.EventHandler(this.v2W_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ASSTextBoxRegEx particulaText;
        private System.Windows.Forms.Label particulaL;
        private System.Windows.Forms.Label birthrate;
        private NumericTextBox numericTextBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button generar;
        private System.Windows.Forms.Button animation;
        private System.Windows.Forms.Label fril;
        private System.Windows.Forms.Label frfl;
        private NumericTextBox fri;
        private NumericTextBox frf;
    }
}