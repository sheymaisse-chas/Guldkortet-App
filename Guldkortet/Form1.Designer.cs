namespace Guldkortet
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
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lstRewards = new System.Windows.Forms.ListBox();
            this.txtKommun = new System.Windows.Forms.TextBox();
            this.txtAnvandarNr = new System.Windows.Forms.TextBox();
            this.txtNamn = new System.Windows.Forms.TextBox();
            this.btnAddKund = new System.Windows.Forms.Button();
            this.btnUpdateKund = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(12, 654);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(228, 76);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Starta Server";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(12, 752);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(228, 76);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Stoppa Server";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(257, 9);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(194, 32);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Status: Offline";
            this.lblStatus.Click += new System.EventHandler(this.lblStatus_Click);
            // 
            // lstRewards
            // 
            this.lstRewards.FormattingEnabled = true;
            this.lstRewards.ItemHeight = 31;
            this.lstRewards.Location = new System.Drawing.Point(286, 116);
            this.lstRewards.Name = "lstRewards";
            this.lstRewards.Size = new System.Drawing.Size(1585, 500);
            this.lstRewards.TabIndex = 3;
            // 
            // txtKommun
            // 
            this.txtKommun.Location = new System.Drawing.Point(12, 315);
            this.txtKommun.Name = "txtKommun";
            this.txtKommun.Size = new System.Drawing.Size(228, 38);
            this.txtKommun.TabIndex = 4;
            this.txtKommun.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // txtAnvandarNr
            // 
            this.txtAnvandarNr.Location = new System.Drawing.Point(12, 116);
            this.txtAnvandarNr.Name = "txtAnvandarNr";
            this.txtAnvandarNr.Size = new System.Drawing.Size(228, 38);
            this.txtAnvandarNr.TabIndex = 5;
            this.txtAnvandarNr.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // txtNamn
            // 
            this.txtNamn.Location = new System.Drawing.Point(12, 214);
            this.txtNamn.Name = "txtNamn";
            this.txtNamn.Size = new System.Drawing.Size(228, 38);
            this.txtNamn.TabIndex = 6;
            // 
            // btnAddKund
            // 
            this.btnAddKund.Location = new System.Drawing.Point(12, 378);
            this.btnAddKund.Name = "btnAddKund";
            this.btnAddKund.Size = new System.Drawing.Size(228, 76);
            this.btnAddKund.TabIndex = 7;
            this.btnAddKund.Text = "Lägg till kund";
            this.btnAddKund.UseVisualStyleBackColor = true;
            // 
            // btnUpdateKund
            // 
            this.btnUpdateKund.Location = new System.Drawing.Point(12, 460);
            this.btnUpdateKund.Name = "btnUpdateKund";
            this.btnUpdateKund.Size = new System.Drawing.Size(228, 117);
            this.btnUpdateKund.TabIndex = 8;
            this.btnUpdateKund.Text = "Uppdatera uppgifter";
            this.btnUpdateKund.UseVisualStyleBackColor = true;
            this.btnUpdateKund.Click += new System.EventHandler(this.btnUpdateKund_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(118, 32);
            this.label2.TabIndex = 10;
            this.label2.Text = "KundNr:";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 179);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 32);
            this.label3.TabIndex = 11;
            this.label3.Text = "Namn:";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 280);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(127, 32);
            this.label4.TabIndex = 12;
            this.label4.Text = "Kommun";
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(286, 689);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(1585, 139);
            this.txtLog.TabIndex = 13;
            this.txtLog.TextChanged += new System.EventHandler(this.textBox1_TextChanged_1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(280, 81);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(228, 32);
            this.label1.TabIndex = 14;
            this.label1.Text = "Utdelade vinster:";
            this.label1.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(280, 654);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(266, 32);
            this.label5.TabIndex = 15;
            this.label5.Text = "Serverlogg / Debug:";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1883, 859);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnUpdateKund);
            this.Controls.Add(this.btnAddKund);
            this.Controls.Add(this.txtNamn);
            this.Controls.Add(this.txtAnvandarNr);
            this.Controls.Add(this.txtKommun);
            this.Controls.Add(this.lstRewards);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ListBox lstRewards;
        private System.Windows.Forms.TextBox txtKommun;
        private System.Windows.Forms.TextBox txtAnvandarNr;
        private System.Windows.Forms.TextBox txtNamn;
        private System.Windows.Forms.Button btnAddKund;
        private System.Windows.Forms.Button btnUpdateKund;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
    }
}

