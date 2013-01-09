namespace AVICaptureTest
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
        protected override void Dispose( bool disposing )
        {
            if( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.videoFormatBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.frameRateBar = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cameraGroupBox = new System.Windows.Forms.GroupBox();
            this.aviCapture1 = new AVICapWrapper.AVICapture();
            this.GoButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.frameRateBar)).BeginInit();
            this.cameraGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // videoFormatBox
            // 
            this.videoFormatBox.FormattingEnabled = true;
            this.videoFormatBox.Location = new System.Drawing.Point(87, 6);
            this.videoFormatBox.Name = "videoFormatBox";
            this.videoFormatBox.Size = new System.Drawing.Size(121, 21);
            this.videoFormatBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Video Format";
            // 
            // frameRateBar
            // 
            this.frameRateBar.Location = new System.Drawing.Point(87, 33);
            this.frameRateBar.Maximum = 25;
            this.frameRateBar.Minimum = 1;
            this.frameRateBar.Name = "frameRateBar";
            this.frameRateBar.Size = new System.Drawing.Size(239, 45);
            this.frameRateBar.TabIndex = 3;
            this.frameRateBar.Value = 1;
            this.frameRateBar.Scroll += new System.EventHandler(this.frameRateBar_Scroll);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Frame Rate";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(332, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Current";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(379, 36);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(49, 20);
            this.textBox1.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(434, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Frames/Sec";
            // 
            // cameraGroupBox
            // 
            this.cameraGroupBox.Controls.Add(this.aviCapture1);
            this.cameraGroupBox.Location = new System.Drawing.Point(12, 84);
            this.cameraGroupBox.Name = "cameraGroupBox";
            this.cameraGroupBox.Size = new System.Drawing.Size(175, 150);
            this.cameraGroupBox.TabIndex = 9;
            this.cameraGroupBox.TabStop = false;
            this.cameraGroupBox.Text = "Frame from Camera";
            // 
            // aviCapture1
            // 
            this.aviCapture1.Location = new System.Drawing.Point(6, 20);
            this.aviCapture1.Name = "aviCapture1";
            this.aviCapture1.Size = new System.Drawing.Size(160, 120);
            this.aviCapture1.TabIndex = 2;
            // 
            // GoButton
            // 
            this.GoButton.Location = new System.Drawing.Point(379, -2);
            this.GoButton.Name = "GoButton";
            this.GoButton.Size = new System.Drawing.Size(149, 23);
            this.GoButton.TabIndex = 18;
            this.GoButton.Text = "go!";
            this.GoButton.UseVisualStyleBackColor = true;
            this.GoButton.Click += new System.EventHandler(this.GoButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(559, 244);
            this.Controls.Add(this.GoButton);
            this.Controls.Add(this.cameraGroupBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.frameRateBar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.videoFormatBox);
            this.Name = "Form1";
            this.Text = "Video Test";
            ((System.ComponentModel.ISupportInitialize)(this.frameRateBar)).EndInit();
            this.cameraGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox videoFormatBox;
        private System.Windows.Forms.Label label1;
        private AVICapWrapper.AVICapture aviCapture1;
        private System.Windows.Forms.TrackBar frameRateBar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox cameraGroupBox;
        private System.Windows.Forms.Button GoButton;

    }
}

