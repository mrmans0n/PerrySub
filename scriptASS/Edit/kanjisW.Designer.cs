namespace scriptASS
{
    partial class kanjisW
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.kanjiGrid = new System.Windows.Forms.DataGridView();
            this.NumLinea = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.K = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Tiempo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Rom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Kanjis = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button2 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.sustituir = new System.Windows.Forms.RadioButton();
            this.añadir = new System.Windows.Forms.RadioButton();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.roomajiBox = new System.Windows.Forms.TextBox();
            this.furiganaBox = new System.Windows.Forms.TextBox();
            this.kanjisBox = new scriptASS.IMM32TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            ((System.ComponentModel.ISupportInitialize)(this.kanjiGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // kanjiGrid
            // 
            this.kanjiGrid.AllowUserToAddRows = false;
            this.kanjiGrid.AllowUserToDeleteRows = false;
            this.kanjiGrid.AllowUserToResizeColumns = false;
            this.kanjiGrid.AllowUserToResizeRows = false;
            this.kanjiGrid.BackgroundColor = System.Drawing.SystemColors.Control;
            this.kanjiGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.kanjiGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            this.kanjiGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.kanjiGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NumLinea,
            this.K,
            this.Tiempo,
            this.Rom,
            this.Kanjis});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.kanjiGrid.DefaultCellStyle = dataGridViewCellStyle3;
            this.kanjiGrid.EnableHeadersVisualStyles = false;
            this.kanjiGrid.Location = new System.Drawing.Point(0, 0);
            this.kanjiGrid.MultiSelect = false;
            this.kanjiGrid.Name = "kanjiGrid";
            this.kanjiGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.kanjiGrid.RowHeadersVisible = false;
            this.kanjiGrid.RowTemplate.Height = 17;
            this.kanjiGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.kanjiGrid.ShowCellErrors = false;
            this.kanjiGrid.ShowCellToolTips = false;
            this.kanjiGrid.ShowEditingIcon = false;
            this.kanjiGrid.ShowRowErrors = false;
            this.kanjiGrid.Size = new System.Drawing.Size(341, 505);
            this.kanjiGrid.TabIndex = 0;
            // 
            // NumLinea
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.NumLinea.DefaultCellStyle = dataGridViewCellStyle1;
            this.NumLinea.Frozen = true;
            this.NumLinea.HeaderText = "#";
            this.NumLinea.Name = "NumLinea";
            this.NumLinea.ReadOnly = true;
            this.NumLinea.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.NumLinea.Width = 40;
            // 
            // K
            // 
            this.K.Frozen = true;
            this.K.HeaderText = "K";
            this.K.Name = "K";
            this.K.ReadOnly = true;
            this.K.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.K.Width = 40;
            // 
            // Tiempo
            // 
            this.Tiempo.Frozen = true;
            this.Tiempo.HeaderText = "Tiempo";
            this.Tiempo.Name = "Tiempo";
            this.Tiempo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Tiempo.Width = 50;
            // 
            // Rom
            // 
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Rom.DefaultCellStyle = dataGridViewCellStyle2;
            this.Rom.Frozen = true;
            this.Rom.HeaderText = "Roomaji";
            this.Rom.Name = "Rom";
            this.Rom.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Kanjis
            // 
            this.Kanjis.Frozen = true;
            this.Kanjis.HeaderText = "Kanjis";
            this.Kanjis.Name = "Kanjis";
            this.Kanjis.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(592, 478);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(152, 22);
            this.button2.TabIndex = 5;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::scriptASS.Properties.Resources.icont15;
            this.pictureBox1.Location = new System.Drawing.Point(750, 400);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(101, 100);
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // button3
            // 
            this.button3.Image = global::scriptASS.Properties.Resources.Check;
            this.button3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button3.Location = new System.Drawing.Point(347, 449);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(109, 43);
            this.button3.TabIndex = 9;
            this.button3.Text = "Aplicar";
            this.button3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button1
            // 
            this.button1.Image = global::scriptASS.Properties.Resources.Open32;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.button1.Location = new System.Drawing.Point(347, 400);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(109, 43);
            this.button1.TabIndex = 2;
            this.button1.Text = "Cargar Kanjis";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(589, 400);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(154, 78);
            this.label1.TabIndex = 11;
            this.label1.Text = "[ INSERT ] Añadir kanji nuevo\r\n[ END ] Añadir kanji a existente\r\n[ DEL ] Sigue el" +
                " kanji anterior\r\n\r\n[ PGUP ] Retroceder selección\r\n[ PGDN ] Avanzar selección";
            // 
            // button4
            // 
            this.button4.Image = global::scriptASS.Properties.Resources.Delete32;
            this.button4.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button4.Location = new System.Drawing.Point(462, 449);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(109, 43);
            this.button4.TabIndex = 12;
            this.button4.Text = "Salir";
            this.button4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.sustituir);
            this.groupBox1.Controls.Add(this.añadir);
            this.groupBox1.Location = new System.Drawing.Point(462, 394);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(109, 50);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            // 
            // sustituir
            // 
            this.sustituir.AutoSize = true;
            this.sustituir.Checked = true;
            this.sustituir.Location = new System.Drawing.Point(6, 30);
            this.sustituir.Name = "sustituir";
            this.sustituir.Size = new System.Drawing.Size(98, 17);
            this.sustituir.TabIndex = 1;
            this.sustituir.TabStop = true;
            this.sustituir.Text = "Sustituir original";
            this.sustituir.UseVisualStyleBackColor = true;
            // 
            // añadir
            // 
            this.añadir.AutoSize = true;
            this.añadir.Location = new System.Drawing.Point(6, 10);
            this.añadir.Name = "añadir";
            this.añadir.Size = new System.Drawing.Size(100, 17);
            this.añadir.TabIndex = 0;
            this.añadir.Text = "Añadir a original";
            this.añadir.UseVisualStyleBackColor = true;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewTextBoxColumn1.Frozen = true;
            this.dataGridViewTextBoxColumn1.HeaderText = "#";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn1.Width = 40;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.Frozen = true;
            this.dataGridViewTextBoxColumn2.HeaderText = "K";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn2.Width = 50;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.Frozen = true;
            this.dataGridViewTextBoxColumn3.HeaderText = "Tiempo";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn3.Width = 50;
            // 
            // dataGridViewTextBoxColumn4
            // 
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewTextBoxColumn4.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewTextBoxColumn4.Frozen = true;
            this.dataGridViewTextBoxColumn4.HeaderText = "Roomaji";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.Frozen = true;
            this.dataGridViewTextBoxColumn5.HeaderText = "Kanjis";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.kanjisBox);
            this.tabPage1.Controls.Add(this.furiganaBox);
            this.tabPage1.Controls.Add(this.roomajiBox);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(507, 362);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Texto";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // roomajiBox
            // 
            this.roomajiBox.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.roomajiBox.ForeColor = System.Drawing.Color.DarkRed;
            this.roomajiBox.Location = new System.Drawing.Point(0, 335);
            this.roomajiBox.Name = "roomajiBox";
            this.roomajiBox.ReadOnly = true;
            this.roomajiBox.Size = new System.Drawing.Size(507, 21);
            this.roomajiBox.TabIndex = 8;
            // 
            // furiganaBox
            // 
            this.furiganaBox.Font = new System.Drawing.Font("MS Gothic", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.furiganaBox.Location = new System.Drawing.Point(0, 301);
            this.furiganaBox.Name = "furiganaBox";
            this.furiganaBox.ReadOnly = true;
            this.furiganaBox.Size = new System.Drawing.Size(507, 28);
            this.furiganaBox.TabIndex = 7;
            // 
            // kanjisBox
            // 
            this.kanjisBox.Font = new System.Drawing.Font("MS Mincho", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.kanjisBox.FuriganaTextBox = this.furiganaBox;
            this.kanjisBox.ImeMode = System.Windows.Forms.ImeMode.On;
            this.kanjisBox.Location = new System.Drawing.Point(0, 0);
            this.kanjisBox.Multiline = true;
            this.kanjisBox.Name = "kanjisBox";
            this.kanjisBox.RoomajiTextBox = this.roomajiBox;
            this.kanjisBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.kanjisBox.Size = new System.Drawing.Size(507, 295);
            this.kanjisBox.TabIndex = 6;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(348, 6);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(515, 388);
            this.tabControl1.TabIndex = 14;
            // 
            // kanjisW
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(863, 505);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.kanjiGrid);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "kanjisW";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Asistente para el timeo de Kanjis";
            this.Load += new System.EventHandler(this.kanjisW_Load);
            ((System.ComponentModel.ISupportInitialize)(this.kanjiGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView kanjiGrid;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton sustituir;
        private System.Windows.Forms.RadioButton añadir;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn NumLinea;
        private System.Windows.Forms.DataGridViewTextBoxColumn K;
        private System.Windows.Forms.DataGridViewTextBoxColumn Tiempo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Rom;
        private System.Windows.Forms.DataGridViewTextBoxColumn Kanjis;
        private System.Windows.Forms.TabPage tabPage1;
        private IMM32TextBox kanjisBox;
        private System.Windows.Forms.TextBox furiganaBox;
        private System.Windows.Forms.TextBox roomajiBox;
        private System.Windows.Forms.TabControl tabControl1;

    }
}