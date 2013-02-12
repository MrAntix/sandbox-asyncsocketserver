namespace Sandbox.AsyncSocketServer.WindowsForms
{
    partial class MainForm
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
                _server.Dispose();
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
            this.LogTextBox = new System.Windows.Forms.TextBox();
            this.ClearLogButton = new System.Windows.Forms.Button();
            this.FooterPanel = new System.Windows.Forms.Panel();
            this.LogLevelControl = new System.Windows.Forms.ComboBox();
            this.StartStopButton = new System.Windows.Forms.Button();
            this.FooterPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // LogTextBox
            // 
            this.LogTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogTextBox.Location = new System.Drawing.Point(0, 0);
            this.LogTextBox.Multiline = true;
            this.LogTextBox.Name = "LogTextBox";
            this.LogTextBox.Size = new System.Drawing.Size(604, 298);
            this.LogTextBox.TabIndex = 0;
            // 
            // ClearLogButton
            // 
            this.ClearLogButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.ClearLogButton.Location = new System.Drawing.Point(520, 0);
            this.ClearLogButton.Name = "ClearLogButton";
            this.ClearLogButton.Size = new System.Drawing.Size(84, 23);
            this.ClearLogButton.TabIndex = 1;
            this.ClearLogButton.Text = "Clear";
            this.ClearLogButton.UseVisualStyleBackColor = true;
            this.ClearLogButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // FooterPanel
            // 
            this.FooterPanel.Controls.Add(this.StartStopButton);
            this.FooterPanel.Controls.Add(this.LogLevelControl);
            this.FooterPanel.Controls.Add(this.ClearLogButton);
            this.FooterPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.FooterPanel.Location = new System.Drawing.Point(0, 298);
            this.FooterPanel.Name = "FooterPanel";
            this.FooterPanel.Size = new System.Drawing.Size(604, 23);
            this.FooterPanel.TabIndex = 2;
            // 
            // LogLevelControl
            // 
            this.LogLevelControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogLevelControl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.LogLevelControl.FormattingEnabled = true;
            this.LogLevelControl.IntegralHeight = false;
            this.LogLevelControl.Location = new System.Drawing.Point(0, 0);
            this.LogLevelControl.Name = "LogLevelControl";
            this.LogLevelControl.Size = new System.Drawing.Size(520, 21);
            this.LogLevelControl.TabIndex = 2;
            // 
            // StartStopButton
            // 
            this.StartStopButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.StartStopButton.Location = new System.Drawing.Point(0, 0);
            this.StartStopButton.Name = "StartStopButton";
            this.StartStopButton.Size = new System.Drawing.Size(75, 23);
            this.StartStopButton.TabIndex = 3;
            this.StartStopButton.Text = "Stop";
            this.StartStopButton.UseVisualStyleBackColor = true;
            this.StartStopButton.Click += new System.EventHandler(this.StartStopButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 321);
            this.Controls.Add(this.LogTextBox);
            this.Controls.Add(this.FooterPanel);
            this.Name = "MainForm";
            this.Text = "Async Socket Server";
            this.FooterPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox LogTextBox;
        private System.Windows.Forms.Button ClearLogButton;
        private System.Windows.Forms.Panel FooterPanel;
        private System.Windows.Forms.ComboBox LogLevelControl;
        private System.Windows.Forms.Button StartStopButton;
    }
}

