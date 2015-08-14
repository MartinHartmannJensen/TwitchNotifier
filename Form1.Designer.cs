namespace ArethruTwitchNotifier
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.Clear = new System.Windows.Forms.Button();
            this.btnGetResponseString = new System.Windows.Forms.Button();
            this.btnXamlWindow = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.richTextBox1.Location = new System.Drawing.Point(12, 41);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(399, 405);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            this.richTextBox1.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.richTextBox1_LinkClicked);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(12, 12);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // Clear
            // 
            this.Clear.Location = new System.Drawing.Point(93, 12);
            this.Clear.Name = "Clear";
            this.Clear.Size = new System.Drawing.Size(75, 23);
            this.Clear.TabIndex = 2;
            this.Clear.Text = "Clear";
            this.Clear.UseVisualStyleBackColor = true;
            this.Clear.Click += new System.EventHandler(this.Clear_Click);
            // 
            // btnGetResponseString
            // 
            this.btnGetResponseString.Location = new System.Drawing.Point(174, 12);
            this.btnGetResponseString.Name = "btnGetResponseString";
            this.btnGetResponseString.Size = new System.Drawing.Size(75, 23);
            this.btnGetResponseString.TabIndex = 3;
            this.btnGetResponseString.Text = "GetFullString";
            this.btnGetResponseString.UseVisualStyleBackColor = true;
            this.btnGetResponseString.Click += new System.EventHandler(this.btnGetResponseString_Click);
            // 
            // btnXamlWindow
            // 
            this.btnXamlWindow.Location = new System.Drawing.Point(255, 12);
            this.btnXamlWindow.Name = "btnXamlWindow";
            this.btnXamlWindow.Size = new System.Drawing.Size(75, 23);
            this.btnXamlWindow.TabIndex = 4;
            this.btnXamlWindow.Text = "ForcePopup";
            this.btnXamlWindow.UseVisualStyleBackColor = true;
            this.btnXamlWindow.Click += new System.EventHandler(this.btnXamlWindow_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Location = new System.Drawing.Point(336, 12);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(75, 23);
            this.btnSettings.TabIndex = 5;
            this.btnSettings.Text = "Settings";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 458);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.btnXamlWindow);
            this.Controls.Add(this.btnGetResponseString);
            this.Controls.Add(this.Clear);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.richTextBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "TwitchNotification";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button Clear;
        private System.Windows.Forms.Button btnGetResponseString;
        private System.Windows.Forms.Button btnXamlWindow;
        private System.Windows.Forms.Button btnSettings;
    }
}

