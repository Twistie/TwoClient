namespace Two_XNA_Client
{
    partial class FindServer
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
            this.serverBox = new System.Windows.Forms.ListBox();
            this.joinButton = new System.Windows.Forms.Button();
            this.NameBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // serverBox
            // 
            this.serverBox.FormattingEnabled = true;
            this.serverBox.Location = new System.Drawing.Point(11, 41);
            this.serverBox.Name = "serverBox";
            this.serverBox.Size = new System.Drawing.Size(313, 329);
            this.serverBox.TabIndex = 0;
            // 
            // joinButton
            // 
            this.joinButton.Location = new System.Drawing.Point(330, 41);
            this.joinButton.Name = "joinButton";
            this.joinButton.Size = new System.Drawing.Size(108, 50);
            this.joinButton.TabIndex = 1;
            this.joinButton.Text = "Join Server";
            this.joinButton.UseVisualStyleBackColor = true;
            this.joinButton.Click += new System.EventHandler(this.JoinButtonClick);
            // 
            // NameBox
            // 
            this.NameBox.Location = new System.Drawing.Point(133, 12);
            this.NameBox.Name = "NameBox";
            this.NameBox.Size = new System.Drawing.Size(191, 20);
            this.NameBox.TabIndex = 2;
            this.NameBox.Text = "Noname";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(89, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Name:";
            // 
            // FindServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(453, 393);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.NameBox);
            this.Controls.Add(this.joinButton);
            this.Controls.Add(this.serverBox);
            this.Name = "FindServer";
            this.Text = "TWO!";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox serverBox;
        private System.Windows.Forms.Button joinButton;
        private System.Windows.Forms.TextBox NameBox;
        private System.Windows.Forms.Label label1;
    }
}