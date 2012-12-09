namespace scriptASS
{
    partial class cropW
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
            this.VideoBox = new System.Windows.Forms.PictureBox();
            this.ControlBox = new System.Windows.Forms.Panel();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.posY = new System.Windows.Forms.Label();
            this.posX = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Abajo = new scriptASS.NumericTextBox();
            this.Izquierda = new scriptASS.NumericTextBox();
            this.Derecha = new scriptASS.NumericTextBox();
            this.Arriba = new scriptASS.NumericTextBox();
            this.FrameFinal = new scriptASS.NumericTextBox();
            this.FrameActual = new scriptASS.NumericTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.VideoBox)).BeginInit();
            this.ControlBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // VideoBox
            // 
            this.VideoBox.Location = new System.Drawing.Point(-1, 0);
            this.VideoBox.Name = "VideoBox";
            this.VideoBox.Size = new System.Drawing.Size(163, 111);
            this.VideoBox.TabIndex = 0;
            this.VideoBox.TabStop = false;
            // 
            // ControlBox
            // 
            this.ControlBox.Controls.Add(this.button4);
            this.ControlBox.Controls.Add(this.button3);
            this.ControlBox.Controls.Add(this.posY);
            this.ControlBox.Controls.Add(this.posX);
            this.ControlBox.Controls.Add(this.label2);
            this.ControlBox.Controls.Add(this.label1);
            this.ControlBox.Controls.Add(this.Abajo);
            this.ControlBox.Controls.Add(this.Izquierda);
            this.ControlBox.Controls.Add(this.Derecha);
            this.ControlBox.Controls.Add(this.Arriba);
            this.ControlBox.Controls.Add(this.FrameFinal);
            this.ControlBox.Controls.Add(this.FrameActual);
            this.ControlBox.Location = new System.Drawing.Point(-1, 378);
            this.ControlBox.Name = "ControlBox";
            this.ControlBox.Size = new System.Drawing.Size(618, 50);
            this.ControlBox.TabIndex = 1;
            // 
            // button4
            // 
            this.button4.Image = global::scriptASS.Properties.Resources.Delete32;
            this.button4.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button4.Location = new System.Drawing.Point(484, 3);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(109, 43);
            this.button4.TabIndex = 13;
            this.button4.Text = "Salir";
            this.button4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Image = global::scriptASS.Properties.Resources.Check;
            this.button3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button3.Location = new System.Drawing.Point(369, 3);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(109, 43);
            this.button3.TabIndex = 10;
            this.button3.Text = "Aplicar";
            this.button3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // posY
            // 
            this.posY.AutoSize = true;
            this.posY.Location = new System.Drawing.Point(409, 31);
            this.posY.Name = "posY";
            this.posY.Size = new System.Drawing.Size(20, 13);
            this.posY.TabIndex = 9;
            this.posY.Text = "Y=";
            this.posY.Visible = false;
            // 
            // posX
            // 
            this.posX.AutoSize = true;
            this.posX.Location = new System.Drawing.Point(409, 5);
            this.posX.Name = "posX";
            this.posX.Size = new System.Drawing.Size(20, 13);
            this.posX.TabIndex = 8;
            this.posX.Text = "X=";
            this.posX.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(105, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "de";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Frame";
            // 
            // Abajo
            // 
            this.Abajo.AllowNegative = false;
            this.Abajo.DoublePoint = false;
            this.Abajo.Location = new System.Drawing.Point(279, 28);
            this.Abajo.Name = "Abajo";
            this.Abajo.Point = false;
            this.Abajo.Size = new System.Drawing.Size(31, 20);
            this.Abajo.TabIndex = 5;
            // 
            // Izquierda
            // 
            this.Izquierda.AllowNegative = false;
            this.Izquierda.DoublePoint = false;
            this.Izquierda.Location = new System.Drawing.Point(242, 13);
            this.Izquierda.Name = "Izquierda";
            this.Izquierda.Point = false;
            this.Izquierda.Size = new System.Drawing.Size(31, 20);
            this.Izquierda.TabIndex = 4;
            // 
            // Derecha
            // 
            this.Derecha.AllowNegative = false;
            this.Derecha.DoublePoint = false;
            this.Derecha.Location = new System.Drawing.Point(316, 13);
            this.Derecha.Name = "Derecha";
            this.Derecha.Point = false;
            this.Derecha.Size = new System.Drawing.Size(31, 20);
            this.Derecha.TabIndex = 3;
            // 
            // Arriba
            // 
            this.Arriba.AllowNegative = false;
            this.Arriba.DoublePoint = false;
            this.Arriba.Location = new System.Drawing.Point(279, 2);
            this.Arriba.Name = "Arriba";
            this.Arriba.Point = false;
            this.Arriba.Size = new System.Drawing.Size(31, 20);
            this.Arriba.TabIndex = 2;
            // 
            // FrameFinal
            // 
            this.FrameFinal.AllowNegative = false;
            this.FrameFinal.DoublePoint = false;
            this.FrameFinal.Location = new System.Drawing.Point(129, 14);
            this.FrameFinal.Name = "FrameFinal";
            this.FrameFinal.Point = false;
            this.FrameFinal.ReadOnly = true;
            this.FrameFinal.Size = new System.Drawing.Size(58, 20);
            this.FrameFinal.TabIndex = 1;
            // 
            // FrameActual
            // 
            this.FrameActual.AllowNegative = false;
            this.FrameActual.DoublePoint = false;
            this.FrameActual.Location = new System.Drawing.Point(41, 14);
            this.FrameActual.Name = "FrameActual";
            this.FrameActual.Point = false;
            this.FrameActual.Size = new System.Drawing.Size(58, 20);
            this.FrameActual.TabIndex = 0;
            // 
            // cropW
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(616, 429);
            this.Controls.Add(this.VideoBox);
            this.Controls.Add(this.ControlBox);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "cropW";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cropper";
            this.Load += new System.EventHandler(this.cropW_Load);
            ((System.ComponentModel.ISupportInitialize)(this.VideoBox)).EndInit();
            this.ControlBox.ResumeLayout(false);
            this.ControlBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox VideoBox;
        private System.Windows.Forms.Panel ControlBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private NumericTextBox Abajo;
        private NumericTextBox Izquierda;
        private NumericTextBox Derecha;
        private NumericTextBox Arriba;
        private NumericTextBox FrameFinal;
        private NumericTextBox FrameActual;
        private System.Windows.Forms.Label posY;
        private System.Windows.Forms.Label posX;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;

    }
}