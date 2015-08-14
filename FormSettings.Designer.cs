namespace ArethruTwitchNotifier
{
    partial class FormSettings
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSettings));
            this.btnAuth = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSetUserTok = new System.Windows.Forms.Button();
            this.btnSetUpFreq = new System.Windows.Forms.Button();
            this.btnSetPopTime = new System.Windows.Forms.Button();
            this.textBoxUserTok = new System.Windows.Forms.TextBox();
            this.numericUpDownUpFreq = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownPopTime = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUpFreq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPopTime)).BeginInit();
            this.SuspendLayout();
            // 
            // btnAuth
            // 
            this.btnAuth.Location = new System.Drawing.Point(282, 66);
            this.btnAuth.Name = "btnAuth";
            this.btnAuth.Size = new System.Drawing.Size(75, 23);
            this.btnAuth.TabIndex = 0;
            this.btnAuth.Text = "Authorize";
            this.btnAuth.UseVisualStyleBackColor = true;
            this.btnAuth.Click += new System.EventHandler(this.btnAuth_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(371, 39);
            this.label1.TabIndex = 1;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // btnSetUserTok
            // 
            this.btnSetUserTok.Location = new System.Drawing.Point(282, 95);
            this.btnSetUserTok.Name = "btnSetUserTok";
            this.btnSetUserTok.Size = new System.Drawing.Size(75, 23);
            this.btnSetUserTok.TabIndex = 2;
            this.btnSetUserTok.Text = "Save";
            this.btnSetUserTok.UseVisualStyleBackColor = true;
            this.btnSetUserTok.Click += new System.EventHandler(this.btnSetUserTok_Click);
            // 
            // btnSetUpFreq
            // 
            this.btnSetUpFreq.Location = new System.Drawing.Point(282, 158);
            this.btnSetUpFreq.Name = "btnSetUpFreq";
            this.btnSetUpFreq.Size = new System.Drawing.Size(75, 23);
            this.btnSetUpFreq.TabIndex = 3;
            this.btnSetUpFreq.Text = "Save";
            this.btnSetUpFreq.UseVisualStyleBackColor = true;
            this.btnSetUpFreq.Click += new System.EventHandler(this.btnSetUpFreq_Click);
            // 
            // btnSetPopTime
            // 
            this.btnSetPopTime.Location = new System.Drawing.Point(282, 205);
            this.btnSetPopTime.Name = "btnSetPopTime";
            this.btnSetPopTime.Size = new System.Drawing.Size(75, 23);
            this.btnSetPopTime.TabIndex = 4;
            this.btnSetPopTime.Text = "Save";
            this.btnSetPopTime.UseVisualStyleBackColor = true;
            this.btnSetPopTime.Click += new System.EventHandler(this.btnSetPopTime_Click);
            // 
            // textBoxUserTok
            // 
            this.textBoxUserTok.Location = new System.Drawing.Point(12, 97);
            this.textBoxUserTok.Name = "textBoxUserTok";
            this.textBoxUserTok.Size = new System.Drawing.Size(250, 20);
            this.textBoxUserTok.TabIndex = 5;
            // 
            // numericUpDownUpFreq
            // 
            this.numericUpDownUpFreq.Location = new System.Drawing.Point(161, 161);
            this.numericUpDownUpFreq.Maximum = new decimal(new int[] {
            6000,
            0,
            0,
            0});
            this.numericUpDownUpFreq.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownUpFreq.Name = "numericUpDownUpFreq";
            this.numericUpDownUpFreq.Size = new System.Drawing.Size(96, 20);
            this.numericUpDownUpFreq.TabIndex = 6;
            this.numericUpDownUpFreq.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // numericUpDownPopTime
            // 
            this.numericUpDownPopTime.Location = new System.Drawing.Point(161, 208);
            this.numericUpDownPopTime.Maximum = new decimal(new int[] {
            6000,
            0,
            0,
            0});
            this.numericUpDownPopTime.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.numericUpDownPopTime.Name = "numericUpDownPopTime";
            this.numericUpDownPopTime.Size = new System.Drawing.Size(96, 20);
            this.numericUpDownPopTime.TabIndex = 7;
            this.numericUpDownPopTime.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 161);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(138, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Network Update Frequency";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 208);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(145, 26);
            this.label3.TabIndex = 9;
            this.label3.Text = "Desktop Notification Window\r\non-screen time in seconds";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(13, 267);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(203, 17);
            this.checkBox1.TabIndex = 11;
            this.checkBox1.Text = "Run automatic update timer at startup";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(13, 290);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(117, 17);
            this.checkBox2.TabIndex = 12;
            this.checkBox2.Text = "Start with Windows";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(13, 314);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(96, 17);
            this.checkBox3.TabIndex = 13;
            this.checkBox3.Text = "Start minimized";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // FormSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(372, 343);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numericUpDownPopTime);
            this.Controls.Add(this.numericUpDownUpFreq);
            this.Controls.Add(this.textBoxUserTok);
            this.Controls.Add(this.btnSetPopTime);
            this.Controls.Add(this.btnSetUpFreq);
            this.Controls.Add(this.btnSetUserTok);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnAuth);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSettings";
            this.Text = "Settings";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSettings_FormClosing);
            this.Load += new System.EventHandler(this.FormSettings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownUpFreq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPopTime)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAuth;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSetUserTok;
        private System.Windows.Forms.Button btnSetUpFreq;
        private System.Windows.Forms.Button btnSetPopTime;
        private System.Windows.Forms.TextBox textBoxUserTok;
        private System.Windows.Forms.NumericUpDown numericUpDownUpFreq;
        private System.Windows.Forms.NumericUpDown numericUpDownPopTime;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox3;
    }
}
