namespace PROJEKT_RV
{
    partial class Form1
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
            this.btnOdpriDatoteko = new System.Windows.Forms.Button();
            this.btnCompress = new System.Windows.Forms.Button();
            this.btnDecompress = new System.Windows.Forms.Button();
            this.btnOpenBinFile = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // btnOdpriDatoteko
            // 
            this.btnOdpriDatoteko.Location = new System.Drawing.Point(12, 12);
            this.btnOdpriDatoteko.Name = "btnOdpriDatoteko";
            this.btnOdpriDatoteko.Size = new System.Drawing.Size(141, 58);
            this.btnOdpriDatoteko.TabIndex = 0;
            this.btnOdpriDatoteko.Text = "Open .txt file";
            this.btnOdpriDatoteko.UseVisualStyleBackColor = true;
            this.btnOdpriDatoteko.Click += new System.EventHandler(this.btnOdpriDatoteko_Click);
            // 
            // btnCompress
            // 
            this.btnCompress.Location = new System.Drawing.Point(159, 12);
            this.btnCompress.Name = "btnCompress";
            this.btnCompress.Size = new System.Drawing.Size(141, 58);
            this.btnCompress.TabIndex = 1;
            this.btnCompress.Text = "Compress";
            this.btnCompress.UseVisualStyleBackColor = true;
            this.btnCompress.Click += new System.EventHandler(this.btnCompress_Click);
            // 
            // btnDecompress
            // 
            this.btnDecompress.Location = new System.Drawing.Point(159, 76);
            this.btnDecompress.Name = "btnDecompress";
            this.btnDecompress.Size = new System.Drawing.Size(141, 58);
            this.btnDecompress.TabIndex = 2;
            this.btnDecompress.Text = "Decompress";
            this.btnDecompress.UseVisualStyleBackColor = true;
            this.btnDecompress.Click += new System.EventHandler(this.btnDecompress_Click);
            // 
            // btnOpenBinFile
            // 
            this.btnOpenBinFile.Location = new System.Drawing.Point(12, 76);
            this.btnOpenBinFile.Name = "btnOpenBinFile";
            this.btnOpenBinFile.Size = new System.Drawing.Size(141, 58);
            this.btnOpenBinFile.TabIndex = 3;
            this.btnOpenBinFile.Text = "Open .bin file";
            this.btnOpenBinFile.UseVisualStyleBackColor = true;
            this.btnOpenBinFile.Click += new System.EventHandler(this.btnOpenBinFile_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(12, 159);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(288, 96);
            this.richTextBox1.TabIndex = 4;
            this.richTextBox1.Text = "";
            // 
            // richTextBox2
            // 
            this.richTextBox2.Location = new System.Drawing.Point(306, 159);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(288, 96);
            this.richTextBox2.TabIndex = 5;
            this.richTextBox2.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.richTextBox2);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.btnOpenBinFile);
            this.Controls.Add(this.btnDecompress);
            this.Controls.Add(this.btnCompress);
            this.Controls.Add(this.btnOdpriDatoteko);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOdpriDatoteko;
        private System.Windows.Forms.Button btnCompress;
        private System.Windows.Forms.Button btnDecompress;
        private System.Windows.Forms.Button btnOpenBinFile;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.RichTextBox richTextBox2;
    }
}

