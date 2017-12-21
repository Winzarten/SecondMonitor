namespace SecondMonitor.WindowsControls.Controls
{
    using System.ComponentModel;
    using System.Windows.Forms;

    partial class PedalControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.pnlClutch = new System.Windows.Forms.Panel();
            this.lblClutch = new System.Windows.Forms.Label();
            this.SuspendLayout();

            // pnlThrottle
            this.pnlThrottle.Anchor =
                (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top
                                                     | System.Windows.Forms.AnchorStyles.Right));
            this.pnlThrottle.BackColor = System.Drawing.Color.FromArgb(
                (int)(((byte)(0))),
                (int)(((byte)(192))),
                (int)(((byte)(0))));
            this.pnlThrottle.Location = new System.Drawing.Point(72, 0);
            this.pnlThrottle.Name = "pnlThrottle";
            this.pnlThrottle.Size = new System.Drawing.Size(28, 118);
            this.pnlThrottle.TabIndex = 0;

            // lblThrottle
            this.lblThrottle.Anchor =
                (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top
                                                     | System.Windows.Forms.AnchorStyles.Right));
            this.lblThrottle.AutoSize = true;
            this.lblThrottle.ForeColor = System.Drawing.Color.White;
            this.lblThrottle.Location = new System.Drawing.Point(75, 124);
            this.lblThrottle.Name = "lblThrottle";
            this.lblThrottle.Size = new System.Drawing.Size(25, 13);
            this.lblThrottle.TabIndex = 1;
            this.lblThrottle.Text = "100";
            this.lblThrottle.Click += new System.EventHandler(this.lblThrottle_Click);

            // pnlBrake
            this.pnlBrake.Anchor =
                (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top
                                                     | System.Windows.Forms.AnchorStyles.Right));
            this.pnlBrake.BackColor = System.Drawing.Color.FromArgb(
                (int)(((byte)(192))),
                (int)(((byte)(0))),
                (int)(((byte)(0))));
            this.pnlBrake.Location = new System.Drawing.Point(38, 0);
            this.pnlBrake.Name = "pnlBrake";
            this.pnlBrake.Size = new System.Drawing.Size(28, 118);
            this.pnlBrake.TabIndex = 2;

            // lblBrake
            this.lblBrake.Anchor =
                (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top
                                                     | System.Windows.Forms.AnchorStyles.Right));
            this.lblBrake.AutoSize = true;
            this.lblBrake.ForeColor = System.Drawing.Color.White;
            this.lblBrake.Location = new System.Drawing.Point(41, 124);
            this.lblBrake.Name = "lblBrake";
            this.lblBrake.Size = new System.Drawing.Size(25, 13);
            this.lblBrake.TabIndex = 3;
            this.lblBrake.Text = "100";

            // pnlClutch
            this.pnlClutch.Anchor =
                (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top
                                                     | System.Windows.Forms.AnchorStyles.Right));
            this.pnlClutch.BackColor = System.Drawing.Color.Yellow;
            this.pnlClutch.Location = new System.Drawing.Point(4, 0);
            this.pnlClutch.Name = "pnlClutch";
            this.pnlClutch.Size = new System.Drawing.Size(28, 118);
            this.pnlClutch.TabIndex = 4;

            // lblClutch
            this.lblClutch.Anchor =
                (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top
                                                     | System.Windows.Forms.AnchorStyles.Right));
            this.lblClutch.AutoSize = true;
            this.lblClutch.ForeColor = System.Drawing.Color.White;
            this.lblClutch.Location = new System.Drawing.Point(7, 124);
            this.lblClutch.Name = "lblClutch";
            this.lblClutch.Size = new System.Drawing.Size(25, 13);
            this.lblClutch.TabIndex = 5;
            this.lblClutch.Text = "100";

            // PedalControl
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb((int)(((byte)(5))), (int)(((byte)(5))), (int)(((byte)(5))));
            this.Controls.Add(this.lblClutch);
            this.Controls.Add(this.pnlClutch);
            this.Controls.Add(this.lblBrake);
            this.Controls.Add(this.pnlBrake);
            this.Controls.Add(this.lblThrottle);
            this.Controls.Add(this.pnlThrottle);
            this.Name = "PedalControl";
            this.Size = new System.Drawing.Size(110, 150);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Panel pnlThrottle;
        private Label lblThrottle;
        private Panel pnlBrake;
        private Label lblBrake;
        private Panel pnlClutch;
        private Label lblClutch;
    }
}
