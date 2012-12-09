namespace scriptASS
{
    partial class TimeTextBox
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


        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.styleTextBox = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.transparentLabel2 = new scriptASS.TransparentLabel();
            this.transparentLabel1 = new scriptASS.TransparentLabel();
            this.label1 = new scriptASS.TransparentLabel();
            this.textMS = new scriptASS.NumericTextBox();
            this.textS = new scriptASS.NumericTextBox();
            this.textM = new scriptASS.NumericTextBox();
            this.textH = new scriptASS.NumericTextBox();
            this.SuspendLayout();
            // 
            // styleTextBox
            // 
            this.styleTextBox.BackColor = System.Drawing.Color.White;
            this.styleTextBox.Location = new System.Drawing.Point(1, 1);
            this.styleTextBox.Name = "styleTextBox";
            this.styleTextBox.Size = new System.Drawing.Size(70, 18);
            this.styleTextBox.TabIndex = 8;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.MaxLength = 11;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(72, 20);
            this.textBox1.TabIndex = 9;
            // 
            // transparentLabel2
            // 
            this.transparentLabel2.Location = new System.Drawing.Point(51, 3);
            this.transparentLabel2.Name = "transparentLabel2";
            this.transparentLabel2.Size = new System.Drawing.Size(5, 13);
            this.transparentLabel2.TabIndex = 7;
            this.transparentLabel2.TabStop = false;
            this.transparentLabel2.Text = ".";
            // 
            // transparentLabel1
            // 
            this.transparentLabel1.Location = new System.Drawing.Point(34, 2);
            this.transparentLabel1.Name = "transparentLabel1";
            this.transparentLabel1.Size = new System.Drawing.Size(5, 13);
            this.transparentLabel1.TabIndex = 6;
            this.transparentLabel1.TabStop = false;
            this.transparentLabel1.Text = ":";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(5, 13);
            this.label1.TabIndex = 5;
            this.label1.TabStop = false;
            this.label1.Text = ":";
            // 
            // textMS
            // 
            this.textMS.AllowNegative = false;
            this.textMS.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textMS.DoublePoint = false;
            this.textMS.Location = new System.Drawing.Point(56, 3);
            this.textMS.MaximumSize = new System.Drawing.Size(13, 13);
            this.textMS.MaxLength = 2;
            this.textMS.Name = "textMS";
            this.textMS.Point = false;
            this.textMS.Size = new System.Drawing.Size(13, 13);
            this.textMS.TabIndex = 4;
            this.textMS.Text = "00";
            this.textMS.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textS
            // 
            this.textS.AllowNegative = false;
            this.textS.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textS.DoublePoint = false;
            this.textS.Location = new System.Drawing.Point(39, 3);
            this.textS.MaximumSize = new System.Drawing.Size(13, 13);
            this.textS.MaxLength = 2;
            this.textS.Name = "textS";
            this.textS.Point = false;
            this.textS.Size = new System.Drawing.Size(13, 13);
            this.textS.TabIndex = 3;
            this.textS.Text = "00";
            this.textS.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textM
            // 
            this.textM.AllowNegative = false;
            this.textM.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textM.DoublePoint = false;
            this.textM.Location = new System.Drawing.Point(20, 3);
            this.textM.MaximumSize = new System.Drawing.Size(14, 13);
            this.textM.MaxLength = 2;
            this.textM.Name = "textM";
            this.textM.Point = false;
            this.textM.Size = new System.Drawing.Size(14, 13);
            this.textM.TabIndex = 2;
            this.textM.Text = "00";
            this.textM.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textH
            // 
            this.textH.AllowNegative = false;
            this.textH.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textH.DoublePoint = false;
            this.textH.Location = new System.Drawing.Point(3, 3);
            this.textH.MaximumSize = new System.Drawing.Size(12, 13);
            this.textH.MaxLength = 2;
            this.textH.Name = "textH";
            this.textH.Point = false;
            this.textH.Size = new System.Drawing.Size(12, 13);
            this.textH.TabIndex = 1;
            this.textH.Text = "00";
            this.textH.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // TimeTextBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.transparentLabel2);
            this.Controls.Add(this.transparentLabel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textMS);
            this.Controls.Add(this.textS);
            this.Controls.Add(this.textM);
            this.Controls.Add(this.textH);
            this.Controls.Add(this.styleTextBox);
            this.Controls.Add(this.textBox1);
            this.Name = "TimeTextBox";
            this.Size = new System.Drawing.Size(96, 31);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private NumericTextBox textH;
        private NumericTextBox textM;
        private NumericTextBox textS;
        private NumericTextBox textMS;
        private TransparentLabel label1;
        private TransparentLabel transparentLabel1;
        private TransparentLabel transparentLabel2;
        private System.Windows.Forms.Panel styleTextBox;
        private System.Windows.Forms.TextBox textBox1;
    }
}
