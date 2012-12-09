namespace scriptASS
{
    partial class bitrateCalcW
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.comboAudioBitrate = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.comboAudioFormat = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textSize = new scriptASS.NumericTextBox();
            this.isSize = new System.Windows.Forms.RadioButton();
            this.isBitRate = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.hasBFrames = new System.Windows.Forms.CheckBox();
            this.comboContainer = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textFrames = new scriptASS.NumericTextBox();
            this.textFPS = new scriptASS.NumericTextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBitrate = new scriptASS.NumericTextBox();
            this.textTargetSize = new scriptASS.NumericTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Tamaño deseado";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Número de Frames";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Frames por segundo";
            // 
            // comboAudioBitrate
            // 
            this.comboAudioBitrate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboAudioBitrate.FormattingEnabled = true;
            this.comboAudioBitrate.Items.AddRange(new object[] {
            "96",
            "128",
            "160",
            "192",
            "256"});
            this.comboAudioBitrate.Location = new System.Drawing.Point(81, 56);
            this.comboAudioBitrate.Name = "comboAudioBitrate";
            this.comboAudioBitrate.Size = new System.Drawing.Size(76, 21);
            this.comboAudioBitrate.TabIndex = 6;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.comboAudioFormat);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.textSize);
            this.groupBox1.Controls.Add(this.isSize);
            this.groupBox1.Controls.Add(this.isBitRate);
            this.groupBox1.Controls.Add(this.comboAudioBitrate);
            this.groupBox1.Location = new System.Drawing.Point(12, 33);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(172, 153);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Audio";
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button1.Location = new System.Drawing.Point(124, 124);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(33, 17);
            this.button1.TabIndex = 14;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboAudioFormat
            // 
            this.comboAudioFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboAudioFormat.FormattingEnabled = true;
            this.comboAudioFormat.Items.AddRange(new object[] {
            "AAC",
            "OGG",
            "MP3 CBR",
            "MP3 VBR",
            "AC3"});
            this.comboAudioFormat.Location = new System.Drawing.Point(81, 19);
            this.comboAudioFormat.Name = "comboAudioFormat";
            this.comboAudioFormat.Size = new System.Drawing.Size(76, 21);
            this.comboAudioFormat.TabIndex = 13;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(45, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Formato";
            // 
            // textSize
            // 
            this.textSize.AllowNegative = false;
            this.textSize.DoublePoint = false;
            this.textSize.Location = new System.Drawing.Point(81, 93);
            this.textSize.Name = "textSize";
            this.textSize.Point = false;
            this.textSize.Size = new System.Drawing.Size(76, 20);
            this.textSize.TabIndex = 11;
            // 
            // isSize
            // 
            this.isSize.AutoSize = true;
            this.isSize.Location = new System.Drawing.Point(6, 94);
            this.isSize.Name = "isSize";
            this.isSize.Size = new System.Drawing.Size(64, 17);
            this.isSize.TabIndex = 9;
            this.isSize.Text = "Tamaño";
            this.isSize.UseVisualStyleBackColor = true;
            // 
            // isBitRate
            // 
            this.isBitRate.AutoSize = true;
            this.isBitRate.Checked = true;
            this.isBitRate.Location = new System.Drawing.Point(6, 57);
            this.isBitRate.Name = "isBitRate";
            this.isBitRate.Size = new System.Drawing.Size(55, 17);
            this.isBitRate.TabIndex = 8;
            this.isBitRate.TabStop = true;
            this.isBitRate.Text = "Bitrate";
            this.isBitRate.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.hasBFrames);
            this.groupBox2.Controls.Add(this.comboContainer);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.textFrames);
            this.groupBox2.Controls.Add(this.textFPS);
            this.groupBox2.Location = new System.Drawing.Point(190, 33);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(199, 153);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Vídeo";
            // 
            // hasBFrames
            // 
            this.hasBFrames.AutoSize = true;
            this.hasBFrames.Checked = true;
            this.hasBFrames.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hasBFrames.Location = new System.Drawing.Point(9, 125);
            this.hasBFrames.Name = "hasBFrames";
            this.hasBFrames.Size = new System.Drawing.Size(70, 17);
            this.hasBFrames.TabIndex = 16;
            this.hasBFrames.Text = "B-Frames";
            this.hasBFrames.UseVisualStyleBackColor = true;
            // 
            // comboContainer
            // 
            this.comboContainer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboContainer.FormattingEnabled = true;
            this.comboContainer.Items.AddRange(new object[] {
            "MP4",
            "MKV",
            "AVI"});
            this.comboContainer.Location = new System.Drawing.Point(111, 19);
            this.comboContainer.Name = "comboContainer";
            this.comboContainer.Size = new System.Drawing.Size(76, 21);
            this.comboContainer.TabIndex = 15;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 22);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(62, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Contenedor";
            // 
            // textFrames
            // 
            this.textFrames.AllowNegative = false;
            this.textFrames.DoublePoint = false;
            this.textFrames.Location = new System.Drawing.Point(111, 56);
            this.textFrames.Name = "textFrames";
            this.textFrames.Point = false;
            this.textFrames.Size = new System.Drawing.Size(76, 20);
            this.textFrames.TabIndex = 3;
            // 
            // textFPS
            // 
            this.textFPS.AllowNegative = false;
            this.textFPS.DoublePoint = false;
            this.textFPS.Location = new System.Drawing.Point(111, 95);
            this.textFPS.Name = "textFPS";
            this.textFPS.Point = true;
            this.textFPS.Size = new System.Drawing.Size(76, 20);
            this.textFPS.TabIndex = 4;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(196, 9);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(92, 13);
            this.label8.TabIndex = 10;
            this.label8.Text = "Cálculo del Bitrate";
            // 
            // textBitrate
            // 
            this.textBitrate.AllowNegative = false;
            this.textBitrate.DoublePoint = false;
            this.textBitrate.Location = new System.Drawing.Point(310, 6);
            this.textBitrate.Name = "textBitrate";
            this.textBitrate.Point = false;
            this.textBitrate.ReadOnly = true;
            this.textBitrate.Size = new System.Drawing.Size(76, 20);
            this.textBitrate.TabIndex = 11;
            // 
            // textTargetSize
            // 
            this.textTargetSize.AllowNegative = false;
            this.textTargetSize.AutoCompleteCustomSource.AddRange(new string[] {
            "140",
            "170",
            "175",
            "233",
            "350",
            "700",
            "1400"});
            this.textTargetSize.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.textTargetSize.DoublePoint = false;
            this.textTargetSize.Location = new System.Drawing.Point(108, 6);
            this.textTargetSize.Name = "textTargetSize";
            this.textTargetSize.Point = true;
            this.textTargetSize.Size = new System.Drawing.Size(51, 20);
            this.textTargetSize.TabIndex = 12;
            this.textTargetSize.Text = "175";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(161, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "MB";
            // 
            // bitrateCalcW
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 197);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textTargetSize);
            this.Controls.Add(this.textBitrate);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "bitrateCalcW";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Calculadora de Bitrate";
            this.Load += new System.EventHandler(this.bitrateCalcW_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboAudioBitrate;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton isSize;
        private System.Windows.Forms.RadioButton isBitRate;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox comboAudioFormat;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox comboContainer;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private NumericTextBox textFrames;
        private NumericTextBox textFPS;
        private NumericTextBox textSize;
        private NumericTextBox textBitrate;
        private System.Windows.Forms.CheckBox hasBFrames;
        private NumericTextBox textTargetSize;
        private System.Windows.Forms.Label label4;
    }
}