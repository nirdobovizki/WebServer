namespace Net4xWinFormsExample
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
            this.SendText = new System.Windows.Forms.TextBox();
            this.SendBtn = new System.Windows.Forms.Button();
            this.RecText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.OpenWebBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SendText
            // 
            this.SendText.Location = new System.Drawing.Point(77, 110);
            this.SendText.Name = "SendText";
            this.SendText.Size = new System.Drawing.Size(562, 38);
            this.SendText.TabIndex = 0;
            // 
            // SendBtn
            // 
            this.SendBtn.Location = new System.Drawing.Point(77, 195);
            this.SendBtn.Name = "SendBtn";
            this.SendBtn.Size = new System.Drawing.Size(562, 105);
            this.SendBtn.TabIndex = 1;
            this.SendBtn.Text = "Send";
            this.SendBtn.UseVisualStyleBackColor = true;
            this.SendBtn.Click += new System.EventHandler(this.SendBtn_Click);
            // 
            // RecText
            // 
            this.RecText.Location = new System.Drawing.Point(812, 110);
            this.RecText.Multiline = true;
            this.RecText.Name = "RecText";
            this.RecText.ReadOnly = true;
            this.RecText.Size = new System.Drawing.Size(615, 625);
            this.RecText.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(71, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(230, 32);
            this.label1.TabIndex = 3;
            this.label1.Text = "Type a message:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(812, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(183, 32);
            this.label2.TabIndex = 4;
            this.label2.Text = "All messages";
            // 
            // OpenWebBtn
            // 
            this.OpenWebBtn.Location = new System.Drawing.Point(77, 346);
            this.OpenWebBtn.Name = "OpenWebBtn";
            this.OpenWebBtn.Size = new System.Drawing.Size(562, 120);
            this.OpenWebBtn.TabIndex = 5;
            this.OpenWebBtn.Text = "Open Web UI";
            this.OpenWebBtn.UseVisualStyleBackColor = true;
            this.OpenWebBtn.Click += new System.EventHandler(this.OpenWebBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1552, 824);
            this.Controls.Add(this.OpenWebBtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.RecText);
            this.Controls.Add(this.SendBtn);
            this.Controls.Add(this.SendText);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox SendText;
        private System.Windows.Forms.Button SendBtn;
        private System.Windows.Forms.TextBox RecText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button OpenWebBtn;
    }
}

