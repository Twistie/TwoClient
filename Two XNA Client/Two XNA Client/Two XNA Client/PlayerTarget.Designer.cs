namespace Two_XNA_Client
{
    partial class PlayerTarget
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
            this.PlayerBox = new System.Windows.Forms.ListBox();
            this.TargetButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // PlayerBox
            // 
            this.PlayerBox.FormattingEnabled = true;
            this.PlayerBox.Location = new System.Drawing.Point(12, 12);
            this.PlayerBox.Name = "PlayerBox";
            this.PlayerBox.Size = new System.Drawing.Size(196, 134);
            this.PlayerBox.TabIndex = 0;
            // 
            // TargetButton
            // 
            this.TargetButton.Location = new System.Drawing.Point(214, 12);
            this.TargetButton.Name = "TargetButton";
            this.TargetButton.Size = new System.Drawing.Size(97, 52);
            this.TargetButton.TabIndex = 1;
            this.TargetButton.Text = "Target Player";
            this.TargetButton.UseVisualStyleBackColor = true;
            this.TargetButton.Click += new System.EventHandler(this.TargetButtonClick);
            // 
            // PlayerTarget
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(323, 154);
            this.Controls.Add(this.TargetButton);
            this.Controls.Add(this.PlayerBox);
            this.Name = "PlayerTarget";
            this.Text = "Target a Player";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox PlayerBox;
        private System.Windows.Forms.Button TargetButton;
    }
}