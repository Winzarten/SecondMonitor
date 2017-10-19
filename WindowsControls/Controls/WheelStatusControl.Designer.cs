namespace SecondMonitor.CarStatus.Forms.Controls
{
    partial class WheelStatusControl
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
            this.lblBreakTemp = new LCDLabel.LcdLabel();
            this.wheelTempLeft = new LCDLabel.LcdLabel();
            this.wheelTempCenter = new LCDLabel.LcdLabel();
            this.wheelTempRight = new LCDLabel.LcdLabel();
            this.lblTyrePressure = new LCDLabel.LcdLabel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pnlWear = new System.Windows.Forms.Panel();
            this.lbWear = new System.Windows.Forms.Label();
            this.lblTyreType = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.pnlWear.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblBreakTemp
            // 
            this.lblBreakTemp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblBreakTemp.BackGround = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblBreakTemp.BorderColor = System.Drawing.Color.BlanchedAlmond;
            this.lblBreakTemp.BorderSpace = 0;
            this.lblBreakTemp.CharSpacing = 2;
            this.lblBreakTemp.DotMatrix = LCDLabel.DotMatrix.mat5x7;
            this.lblBreakTemp.ForeColor = System.Drawing.Color.Maroon;
            this.lblBreakTemp.LineSpacing = 2;
            this.lblBreakTemp.Location = new System.Drawing.Point(27, 67);
            this.lblBreakTemp.Name = "lblBreakTemp";
            this.lblBreakTemp.NumberOfCharacters = 4;
            this.lblBreakTemp.PixelHeight = 2;
            this.lblBreakTemp.PixelOff = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblBreakTemp.PixelOn = System.Drawing.Color.Green;
            this.lblBreakTemp.PixelShape = LCDLabel.PixelShape.Shaped;
            this.lblBreakTemp.PixelSize = LCDLabel.PixelSize.pixCustom;
            this.lblBreakTemp.PixelSpacing = 0;
            this.lblBreakTemp.PixelWidth = 2;
            this.lblBreakTemp.Size = new System.Drawing.Size(48, 16);
            this.lblBreakTemp.TabIndex = 0;
            this.lblBreakTemp.Text = "3000";
            this.lblBreakTemp.TextLines = 1;
            // 
            // wheelTempLeft
            // 
            this.wheelTempLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.wheelTempLeft.BackGround = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.wheelTempLeft.BorderColor = System.Drawing.Color.BlanchedAlmond;
            this.wheelTempLeft.BorderSpace = 3;
            this.wheelTempLeft.CharSpacing = 2;
            this.wheelTempLeft.DotMatrix = LCDLabel.DotMatrix.dos5x7;
            this.wheelTempLeft.ForeColor = System.Drawing.Color.Maroon;
            this.wheelTempLeft.LineSpacing = 2;
            this.wheelTempLeft.Location = new System.Drawing.Point(3, 3);
            this.wheelTempLeft.Name = "wheelTempLeft";
            this.wheelTempLeft.NumberOfCharacters = 3;
            this.wheelTempLeft.PixelHeight = 3;
            this.wheelTempLeft.PixelOff = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.wheelTempLeft.PixelOn = System.Drawing.Color.Navy;
            this.wheelTempLeft.PixelShape = LCDLabel.PixelShape.Shaped;
            this.wheelTempLeft.PixelSize = LCDLabel.PixelSize.pix3x3;
            this.wheelTempLeft.PixelSpacing = 0;
            this.wheelTempLeft.PixelWidth = 3;
            this.wheelTempLeft.Size = new System.Drawing.Size(57, 29);
            this.wheelTempLeft.TabIndex = 1;
            this.wheelTempLeft.Text = "300";
            this.wheelTempLeft.TextLines = 1;
            // 
            // wheelTempCenter
            // 
            this.wheelTempCenter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.wheelTempCenter.BackGround = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.wheelTempCenter.BorderColor = System.Drawing.Color.BlanchedAlmond;
            this.wheelTempCenter.BorderSpace = 3;
            this.wheelTempCenter.CharSpacing = 2;
            this.wheelTempCenter.DotMatrix = LCDLabel.DotMatrix.dos5x7;
            this.wheelTempCenter.ForeColor = System.Drawing.Color.Maroon;
            this.wheelTempCenter.LineSpacing = 2;
            this.wheelTempCenter.Location = new System.Drawing.Point(66, 3);
            this.wheelTempCenter.Name = "wheelTempCenter";
            this.wheelTempCenter.NumberOfCharacters = 3;
            this.wheelTempCenter.PixelHeight = 3;
            this.wheelTempCenter.PixelOff = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.wheelTempCenter.PixelOn = System.Drawing.Color.Navy;
            this.wheelTempCenter.PixelShape = LCDLabel.PixelShape.Shaped;
            this.wheelTempCenter.PixelSize = LCDLabel.PixelSize.pix3x3;
            this.wheelTempCenter.PixelSpacing = 0;
            this.wheelTempCenter.PixelWidth = 3;
            this.wheelTempCenter.Size = new System.Drawing.Size(57, 29);
            this.wheelTempCenter.TabIndex = 2;
            this.wheelTempCenter.Text = "300";
            this.wheelTempCenter.TextLines = 1;
            // 
            // wheelTempRight
            // 
            this.wheelTempRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.wheelTempRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.wheelTempRight.BackGround = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.wheelTempRight.BorderColor = System.Drawing.Color.BlanchedAlmond;
            this.wheelTempRight.BorderSpace = 3;
            this.wheelTempRight.CharSpacing = 2;
            this.wheelTempRight.DotMatrix = LCDLabel.DotMatrix.dos5x7;
            this.wheelTempRight.ForeColor = System.Drawing.Color.Maroon;
            this.wheelTempRight.LineSpacing = 2;
            this.wheelTempRight.Location = new System.Drawing.Point(149, 3);
            this.wheelTempRight.Name = "wheelTempRight";
            this.wheelTempRight.NumberOfCharacters = 3;
            this.wheelTempRight.PixelHeight = 3;
            this.wheelTempRight.PixelOff = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.wheelTempRight.PixelOn = System.Drawing.Color.Navy;
            this.wheelTempRight.PixelShape = LCDLabel.PixelShape.Shaped;
            this.wheelTempRight.PixelSize = LCDLabel.PixelSize.pix3x3;
            this.wheelTempRight.PixelSpacing = 0;
            this.wheelTempRight.PixelWidth = 3;
            this.wheelTempRight.Size = new System.Drawing.Size(57, 29);
            this.wheelTempRight.TabIndex = 3;
            this.wheelTempRight.Text = "300";
            this.wheelTempRight.TextLines = 1;
            // 
            // lblTyrePressure
            // 
            this.lblTyrePressure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTyrePressure.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblTyrePressure.BackGround = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblTyrePressure.BorderColor = System.Drawing.Color.BlanchedAlmond;
            this.lblTyrePressure.BorderSpace = 0;
            this.lblTyrePressure.CharSpacing = 1;
            this.lblTyrePressure.DotMatrix = LCDLabel.DotMatrix.mat5x7;
            this.lblTyrePressure.ForeColor = System.Drawing.Color.Maroon;
            this.lblTyrePressure.LineSpacing = 2;
            this.lblTyrePressure.Location = new System.Drawing.Point(132, 65);
            this.lblTyrePressure.Name = "lblTyrePressure";
            this.lblTyrePressure.NumberOfCharacters = 3;
            this.lblTyrePressure.PixelHeight = 2;
            this.lblTyrePressure.PixelOff = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblTyrePressure.PixelOn = System.Drawing.Color.Black;
            this.lblTyrePressure.PixelShape = LCDLabel.PixelShape.Shaped;
            this.lblTyrePressure.PixelSize = LCDLabel.PixelSize.pixCustom;
            this.lblTyrePressure.PixelSpacing = 0;
            this.lblTyrePressure.PixelWidth = 2;
            this.lblTyrePressure.Size = new System.Drawing.Size(34, 16);
            this.lblTyrePressure.TabIndex = 4;
            this.lblTyrePressure.Text = "300";
            this.lblTyrePressure.TextLines = 1;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox2.Image = global::SecondMonitor.WindowsControls.Properties.Resources.tirepressure300;
            this.pictureBox2.Location = new System.Drawing.Point(172, 62);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(34, 31);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 6;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::SecondMonitor.WindowsControls.Properties.Resources.brake;
            this.pictureBox1.Location = new System.Drawing.Point(3, 65);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(18, 18);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // pnlWear
            // 
            this.pnlWear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlWear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.pnlWear.Controls.Add(this.lblTyreType);
            this.pnlWear.Location = new System.Drawing.Point(3, 31);
            this.pnlWear.Name = "pnlWear";
            this.pnlWear.Size = new System.Drawing.Size(203, 28);
            this.pnlWear.TabIndex = 7;
            // 
            // lbWear
            // 
            this.lbWear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbWear.AutoSize = true;
            this.lbWear.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbWear.Location = new System.Drawing.Point(78, 67);
            this.lbWear.Name = "lbWear";
            this.lbWear.Size = new System.Drawing.Size(45, 16);
            this.lbWear.TabIndex = 0;
            this.lbWear.Text = "100%";
            // 
            // lblTyreType
            // 
            this.lblTyreType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTyreType.AutoSize = true;
            this.lblTyreType.BackColor = System.Drawing.Color.Transparent;
            this.lblTyreType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblTyreType.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTyreType.Location = new System.Drawing.Point(3, 4);
            this.lblTyreType.Name = "lblTyreType";
            this.lblTyreType.Size = new System.Drawing.Size(82, 16);
            this.lblTyreType.TabIndex = 8;
            this.lblTyreType.Text = "Compound";
            // 
            // WheelStatusControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(177)))), ((int)(((byte)(177)))), ((int)(((byte)(177)))));
            this.Controls.Add(this.lbWear);
            this.Controls.Add(this.pnlWear);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblTyrePressure);
            this.Controls.Add(this.wheelTempRight);
            this.Controls.Add(this.wheelTempCenter);
            this.Controls.Add(this.wheelTempLeft);
            this.Controls.Add(this.lblBreakTemp);
            this.Name = "WheelStatusControl";
            this.Size = new System.Drawing.Size(209, 96);
            this.Load += new System.EventHandler(this.WheelStatusControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.pnlWear.ResumeLayout(false);
            this.pnlWear.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private LCDLabel.LcdLabel lblBreakTemp;
        private LCDLabel.LcdLabel wheelTempLeft;
        private LCDLabel.LcdLabel wheelTempCenter;
        private LCDLabel.LcdLabel wheelTempRight;
        private LCDLabel.LcdLabel lblTyrePressure;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Panel pnlWear;
        private System.Windows.Forms.Label lbWear;
        private System.Windows.Forms.Label lblTyreType;
    }
}
