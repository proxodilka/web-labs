
namespace ChatClientWindows
{
    partial class LogginForm
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
            this.serverAddressLabel = new System.Windows.Forms.Label();
            this.serverAddresTextBox = new System.Windows.Forms.TextBox();
            this.nicknameTextBox = new System.Windows.Forms.TextBox();
            this.nicknameLabel = new System.Windows.Forms.Label();
            this.connectButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // serverAddressLabel
            // 
            this.serverAddressLabel.AutoSize = true;
            this.serverAddressLabel.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.serverAddressLabel.Location = new System.Drawing.Point(13, 13);
            this.serverAddressLabel.Name = "serverAddressLabel";
            this.serverAddressLabel.Size = new System.Drawing.Size(139, 25);
            this.serverAddressLabel.TabIndex = 0;
            this.serverAddressLabel.Text = "Server address:";
            // 
            // serverAddresTextBox
            // 
            this.serverAddresTextBox.Location = new System.Drawing.Point(158, 15);
            this.serverAddresTextBox.Name = "serverAddresTextBox";
            this.serverAddresTextBox.PlaceholderText = "127.0.0.1:8080";
            this.serverAddresTextBox.Size = new System.Drawing.Size(163, 23);
            this.serverAddresTextBox.TabIndex = 1;
            // 
            // nicknameTextBox
            // 
            this.nicknameTextBox.Location = new System.Drawing.Point(158, 40);
            this.nicknameTextBox.Name = "nicknameTextBox";
            this.nicknameTextBox.Size = new System.Drawing.Size(163, 23);
            this.nicknameTextBox.TabIndex = 3;
            // 
            // nicknameLabel
            // 
            this.nicknameLabel.AutoSize = true;
            this.nicknameLabel.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.nicknameLabel.Location = new System.Drawing.Point(52, 38);
            this.nicknameLabel.Name = "nicknameLabel";
            this.nicknameLabel.Size = new System.Drawing.Size(100, 25);
            this.nicknameLabel.TabIndex = 2;
            this.nicknameLabel.Text = "Nickname:";
            // 
            // connectButton
            // 
            this.connectButton.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.connectButton.Location = new System.Drawing.Point(13, 69);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(308, 44);
            this.connectButton.TabIndex = 4;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // LogginForm
            // 
            this.AcceptButton = this.connectButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(341, 125);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.nicknameTextBox);
            this.Controls.Add(this.nicknameLabel);
            this.Controls.Add(this.serverAddresTextBox);
            this.Controls.Add(this.serverAddressLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LogginForm";
            this.Text = "Connect to the server";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label serverAddressLabel;
        private System.Windows.Forms.TextBox serverAddresTextBox;
        private System.Windows.Forms.TextBox nicknameTextBox;
        private System.Windows.Forms.Label nicknameLabel;
        private System.Windows.Forms.Button connectButton;
    }
}