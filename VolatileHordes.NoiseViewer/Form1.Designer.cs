namespace VolatileHordes.NoiseViewer
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.DisplayBox = new System.Windows.Forms.PictureBox();
            this.ResetButton = new System.Windows.Forms.Button();
            this.NoiseButton = new System.Windows.Forms.Button();
            this.NoiseBar = new System.Windows.Forms.ProgressBar();
            this.NoisePerClickSlider = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.NoisePerClickDisplay = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.DisplayBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NoisePerClickSlider)).BeginInit();
            this.SuspendLayout();
            // 
            // DisplayBox
            // 
            this.DisplayBox.Location = new System.Drawing.Point(12, 87);
            this.DisplayBox.Name = "DisplayBox";
            this.DisplayBox.Size = new System.Drawing.Size(583, 422);
            this.DisplayBox.TabIndex = 0;
            this.DisplayBox.TabStop = false;
            // 
            // ResetButton
            // 
            this.ResetButton.Location = new System.Drawing.Point(12, 12);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(75, 23);
            this.ResetButton.TabIndex = 1;
            this.ResetButton.Text = "Reset";
            this.ResetButton.UseVisualStyleBackColor = true;
            // 
            // NoiseButton
            // 
            this.NoiseButton.Location = new System.Drawing.Point(108, 12);
            this.NoiseButton.Name = "NoiseButton";
            this.NoiseButton.Size = new System.Drawing.Size(75, 23);
            this.NoiseButton.TabIndex = 2;
            this.NoiseButton.Text = "Noise";
            this.NoiseButton.UseVisualStyleBackColor = true;
            // 
            // NoiseBar
            // 
            this.NoiseBar.Location = new System.Drawing.Point(12, 63);
            this.NoiseBar.Name = "NoiseBar";
            this.NoiseBar.Size = new System.Drawing.Size(491, 23);
            this.NoiseBar.TabIndex = 3;
            // 
            // NoisePerClickSlider
            // 
            this.NoisePerClickSlider.Location = new System.Drawing.Point(189, 12);
            this.NoisePerClickSlider.Maximum = 1000000;
            this.NoisePerClickSlider.Name = "NoisePerClickSlider";
            this.NoisePerClickSlider.Size = new System.Drawing.Size(262, 45);
            this.NoisePerClickSlider.TabIndex = 4;
            this.NoisePerClickSlider.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(274, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "Noise Per Click";
            // 
            // NoisePerClickDisplay
            // 
            this.NoisePerClickDisplay.AutoSize = true;
            this.NoisePerClickDisplay.Location = new System.Drawing.Point(467, 19);
            this.NoisePerClickDisplay.Name = "NoisePerClickDisplay";
            this.NoisePerClickDisplay.Size = new System.Drawing.Size(37, 15);
            this.NoisePerClickDisplay.TabIndex = 6;
            this.NoisePerClickDisplay.Text = "Noise";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(545, 608);
            this.Controls.Add(this.NoisePerClickDisplay);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.NoisePerClickSlider);
            this.Controls.Add(this.NoiseBar);
            this.Controls.Add(this.NoiseButton);
            this.Controls.Add(this.ResetButton);
            this.Controls.Add(this.DisplayBox);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.DisplayBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NoisePerClickSlider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox DisplayBox;
        private System.Windows.Forms.Button ResetButton;
        private System.Windows.Forms.Button NoiseButton;
        private System.Windows.Forms.ProgressBar NoiseBar;
        private System.Windows.Forms.TrackBar NoisePerClickSlider;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label NoisePerClickDisplay;
    }
}