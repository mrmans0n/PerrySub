namespace scriptASS
{
    partial class findOthersW
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(findOthersW));
            this.regExp = new System.Windows.Forms.CheckBox();
            this.caseInSensitive = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.comboFind = new System.Windows.Forms.ComboBox();
            this.archivosView = new System.Windows.Forms.ListView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.coincidenciasView = new System.Windows.Forms.ListView();
            this.subdir = new System.Windows.Forms.CheckBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lineasCoincidencias = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader(2);
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.label3 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // regExp
            // 
            this.regExp.AutoSize = true;
            this.regExp.Location = new System.Drawing.Point(12, 62);
            this.regExp.Name = "regExp";
            this.regExp.Size = new System.Drawing.Size(129, 17);
            this.regExp.TabIndex = 8;
            this.regExp.Text = "Expresiones regulares";
            this.regExp.UseVisualStyleBackColor = true;
            // 
            // caseInSensitive
            // 
            this.caseInSensitive.AutoSize = true;
            this.caseInSensitive.Checked = true;
            this.caseInSensitive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.caseInSensitive.Location = new System.Drawing.Point(12, 39);
            this.caseInSensitive.Name = "caseInSensitive";
            this.caseInSensitive.Size = new System.Drawing.Size(174, 17);
            this.caseInSensitive.TabIndex = 7;
            this.caseInSensitive.Text = "Ignorar mayúsculas/minúsculas";
            this.caseInSensitive.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(449, 10);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(95, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Buscar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboFind
            // 
            this.comboFind.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.comboFind.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboFind.FormattingEnabled = true;
            this.comboFind.Location = new System.Drawing.Point(12, 12);
            this.comboFind.Name = "comboFind";
            this.comboFind.Size = new System.Drawing.Size(431, 21);
            this.comboFind.TabIndex = 5;
            // 
            // archivosView
            // 
            this.archivosView.LargeImageList = this.imageList1;
            this.archivosView.Location = new System.Drawing.Point(12, 121);
            this.archivosView.Name = "archivosView";
            this.archivosView.Size = new System.Drawing.Size(254, 254);
            this.archivosView.SmallImageList = this.imageList1;
            this.archivosView.TabIndex = 9;
            this.archivosView.UseCompatibleStateImageBehavior = false;
            this.archivosView.View = System.Windows.Forms.View.SmallIcon;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "star.gif");
            this.imageList1.Images.SetKeyName(1, "newhotfolder.gif");
            this.imageList1.Images.SetKeyName(2, "jump.gif");
            this.imageList1.Images.SetKeyName(3, "folder.gif");
            this.imageList1.Images.SetKeyName(4, "hotfolder.gif");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 97);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Archivos en los que buscar";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(287, 97);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(188, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Archivos en los que hay coincidencias";
            // 
            // coincidenciasView
            // 
            this.coincidenciasView.Enabled = false;
            this.coincidenciasView.LargeImageList = this.imageList1;
            this.coincidenciasView.Location = new System.Drawing.Point(290, 121);
            this.coincidenciasView.MultiSelect = false;
            this.coincidenciasView.Name = "coincidenciasView";
            this.coincidenciasView.Size = new System.Drawing.Size(254, 160);
            this.coincidenciasView.SmallImageList = this.imageList1;
            this.coincidenciasView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.coincidenciasView.TabIndex = 15;
            this.coincidenciasView.UseCompatibleStateImageBehavior = false;
            this.coincidenciasView.View = System.Windows.Forms.View.SmallIcon;
            // 
            // subdir
            // 
            this.subdir.AutoSize = true;
            this.subdir.Checked = true;
            this.subdir.CheckState = System.Windows.Forms.CheckState.Checked;
            this.subdir.Location = new System.Drawing.Point(142, 404);
            this.subdir.Name = "subdir";
            this.subdir.Size = new System.Drawing.Size(124, 17);
            this.subdir.TabIndex = 12;
            this.subdir.Text = "Incluir Subdirectorios";
            this.subdir.UseVisualStyleBackColor = true;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(290, 56);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(254, 23);
            this.progressBar1.TabIndex = 16;
            this.progressBar1.Visible = false;
            // 
            // lineasCoincidencias
            // 
            this.lineasCoincidencias.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lineasCoincidencias.Enabled = false;
            this.lineasCoincidencias.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lineasCoincidencias.Location = new System.Drawing.Point(290, 300);
            this.lineasCoincidencias.MultiSelect = false;
            this.lineasCoincidencias.Name = "lineasCoincidencias";
            this.lineasCoincidencias.Size = new System.Drawing.Size(254, 121);
            this.lineasCoincidencias.SmallImageList = this.imageList1;
            this.lineasCoincidencias.TabIndex = 17;
            this.lineasCoincidencias.UseCompatibleStateImageBehavior = false;
            this.lineasCoincidencias.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "";
            this.columnHeader1.Width = 74;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Texto";
            this.columnHeader2.Width = 176;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(287, 284);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(129, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "Líneas con coincidencias";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 430);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1023, 22);
            this.statusStrip1.TabIndex = 19;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(0, 17);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(562, 85);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(450, 336);
            this.richTextBox1.TabIndex = 20;
            this.richTextBox1.Text = "Tiempos brau brau - \n1\n2\n3\n4\n5";
            this.richTextBox1.WordWrap = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(559, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "Contexto";
            // 
            // button4
            // 
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button4.Location = new System.Drawing.Point(936, 56);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 22;
            this.button4.Text = "<<";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.AutoSize = true;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button3.Image = global::scriptASS.Properties.Resources.Open32;
            this.button3.Location = new System.Drawing.Point(76, 381);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(60, 40);
            this.button3.TabIndex = 11;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.AutoSize = true;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button2.Image = global::scriptASS.Properties.Resources.New32;
            this.button2.Location = new System.Drawing.Point(12, 381);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(60, 40);
            this.button2.TabIndex = 10;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // findOthersW
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1023, 452);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lineasCoincidencias);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.coincidenciasView);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.subdir);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.archivosView);
            this.Controls.Add(this.regExp);
            this.Controls.Add(this.caseInSensitive);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboFind);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "findOthersW";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Buscar en otros archivos";
            this.Load += new System.EventHandler(this.findOthersW_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox regExp;
        private System.Windows.Forms.CheckBox caseInSensitive;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox comboFind;
        private System.Windows.Forms.ListView archivosView;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListView coincidenciasView;
        private System.Windows.Forms.CheckBox subdir;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ListView lineasCoincidencias;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button4;
    }
}