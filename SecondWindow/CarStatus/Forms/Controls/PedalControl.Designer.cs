namespace SecondWindow.CarStatus.Forms.Controls
{
    partial class PedalControl
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
            this.pnlThrottle = new System.Windows.Forms.Panel();
            this.lblThrottle = new System.Windows.Forms.Label();
            this.pnlBrake = new System.Windows.Forms.Panel();
            this.lblBrake = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pnlThrottle
            // 
            this.pnlThrottle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.pnlThrottle.Location = new System.Drawing.Point(122, 3);
            this.pnlThrottle.Name = "pnlThrottle";
            this.pnlThrottle.Size = new System.Drawing.Size(28, 118);
            this.pnlThrottle.TabIndex = 0;
            // 
            // lblThrottle
            // 
            this.lblThrottle.AutoSize = true;
            this.lblThrottle.ForeColor = System.Drawing.Color.White;
            this.lblThrottle.Location = new System.Drawing.Point(122, 124);
            this.lblThrottle.Name = "lblThrottle";
            this.lblThrottle.Size = new System.Drawing.Size(25, 13);
            this.lblThrottle.TabIndex = 1;
            this.lblThrottle.Text = "100";
            this.lblThrottle.Click += new System.EventHandler(this.lblThrottle_Click);
            // 
            // pnlBrake
            // 
            this.pnlBrake.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.pnlBrake.Location = new System.Drawing.Point(67, 3);
            this.pnlBrake.Name = "pnlBrake";
            this.pnlBrake.Size = new System.Drawing.Size(28, 118);
            this.pnlBrake.TabIndex = 2;
            // 
            // lblBrake
            // 
            this.lblBrake.AutoSize = true;
            this.lblBrake.ForeColor = System.Drawing.Color.White;
            this.lblBrake.Location = new System.Drawing.Point(70, 124);
            this.lblBrake.Name = "lblBrake";
            this.lblBrake.Size = new System.Drawing.Size(25, 13);
            this.lblBrake.TabIndex = 3;
            this.lblBrake.Text = "100";
            // 
            // PedalControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(5)))), ((int)(((byte)(5)))));
            this.Controls.Add(this.lblBrake);
            this.Controls.Add(this.pnlBrake);
            this.Controls.Add(this.lblThrottle);
            this.Controls.Add(this.pnlThrottle);
            this.Name = "PedalControl";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlThrottle;
        private System.Windows.Forms.Label lblThrottle;
        private System.Windows.Forms.Panel pnlBrake;
        private System.Windows.Forms.Label lblBrake;
    }
}
