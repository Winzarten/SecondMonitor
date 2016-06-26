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
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblBreakTemp
            // 
            this.lblBreakTemp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblBreakTemp.BackGround = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblBreakTemp.BorderColor = System.Drawing.Color.BlanchedAlmond;
            this.lblBreakTemp.BorderSpace = 3;
            this.lblBreakTemp.CharSpacing = 2;
            this.lblBreakTemp.DotMatrix = LCDLabel.DotMatrix.mat5x7;
            this.lblBreakTemp.ForeColor = System.Drawing.Color.Maroon;
            this.lblBreakTemp.LineSpacing = 2;
            this.lblBreakTemp.Location = new System.Drawing.Point(3, 98);
            this.lblBreakTemp.Name = "lblBreakTemp";
            this.lblBreakTemp.NumberOfCharacters = 3;
            this.lblBreakTemp.PixelHeight = 5;
            this.lblBreakTemp.PixelOff = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblBreakTemp.PixelOn = System.Drawing.Color.Green;
            this.lblBreakTemp.PixelShape = LCDLabel.PixelShape.Shaped;
            this.lblBreakTemp.PixelSize = LCDLabel.PixelSize.pix5x5;
            this.lblBreakTemp.PixelSpacing = 1;
            this.lblBreakTemp.PixelWidth = 5;
            this.lblBreakTemp.Size = new System.Drawing.Size(99, 49);
            this.lblBreakTemp.TabIndex = 0;
            this.lblBreakTemp.Text = "300";
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
            this.wheelTempLeft.PixelSpacing = 1;
            this.wheelTempLeft.PixelWidth = 3;
            this.wheelTempLeft.Size = new System.Drawing.Size(69, 35);
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
            this.wheelTempCenter.Location = new System.Drawing.Point(78, 3);
            this.wheelTempCenter.Name = "wheelTempCenter";
            this.wheelTempCenter.NumberOfCharacters = 3;
            this.wheelTempCenter.PixelHeight = 3;
            this.wheelTempCenter.PixelOff = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.wheelTempCenter.PixelOn = System.Drawing.Color.Navy;
            this.wheelTempCenter.PixelShape = LCDLabel.PixelShape.Shaped;
            this.wheelTempCenter.PixelSize = LCDLabel.PixelSize.pix3x3;
            this.wheelTempCenter.PixelSpacing = 1;
            this.wheelTempCenter.PixelWidth = 3;
            this.wheelTempCenter.Size = new System.Drawing.Size(69, 35);
            this.wheelTempCenter.TabIndex = 2;
            this.wheelTempCenter.Text = "300";
            this.wheelTempCenter.TextLines = 1;
            // 
            // wheelTempRight
            // 
            this.wheelTempRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.wheelTempRight.BackGround = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.wheelTempRight.BorderColor = System.Drawing.Color.BlanchedAlmond;
            this.wheelTempRight.BorderSpace = 3;
            this.wheelTempRight.CharSpacing = 2;
            this.wheelTempRight.DotMatrix = LCDLabel.DotMatrix.dos5x7;
            this.wheelTempRight.ForeColor = System.Drawing.Color.Maroon;
            this.wheelTempRight.LineSpacing = 2;
            this.wheelTempRight.Location = new System.Drawing.Point(153, 3);
            this.wheelTempRight.Name = "wheelTempRight";
            this.wheelTempRight.NumberOfCharacters = 3;
            this.wheelTempRight.PixelHeight = 3;
            this.wheelTempRight.PixelOff = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.wheelTempRight.PixelOn = System.Drawing.Color.Navy;
            this.wheelTempRight.PixelShape = LCDLabel.PixelShape.Shaped;
            this.wheelTempRight.PixelSize = LCDLabel.PixelSize.pix3x3;
            this.wheelTempRight.PixelSpacing = 1;
            this.wheelTempRight.PixelWidth = 3;
            this.wheelTempRight.Size = new System.Drawing.Size(69, 35);
            this.wheelTempRight.TabIndex = 3;
            this.wheelTempRight.Text = "300";
            this.wheelTempRight.TextLines = 1;
            // 
            // lblTyrePressure
            // 
            this.lblTyrePressure.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblTyrePressure.BackGround = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblTyrePressure.BorderColor = System.Drawing.Color.BlanchedAlmond;
            this.lblTyrePressure.BorderSpace = 3;
            this.lblTyrePressure.CharSpacing = 2;
            this.lblTyrePressure.DotMatrix = LCDLabel.DotMatrix.mat5x7;
            this.lblTyrePressure.ForeColor = System.Drawing.Color.Maroon;
            this.lblTyrePressure.LineSpacing = 2;
            this.lblTyrePressure.Location = new System.Drawing.Point(123, 98);
            this.lblTyrePressure.Name = "lblTyrePressure";
            this.lblTyrePressure.NumberOfCharacters = 3;
            this.lblTyrePressure.PixelHeight = 5;
            this.lblTyrePressure.PixelOff = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblTyrePressure.PixelOn = System.Drawing.Color.Yellow;
            this.lblTyrePressure.PixelShape = LCDLabel.PixelShape.Shaped;
            this.lblTyrePressure.PixelSize = LCDLabel.PixelSize.pix5x5;
            this.lblTyrePressure.PixelSpacing = 1;
            this.lblTyrePressure.PixelWidth = 5;
            this.lblTyrePressure.Size = new System.Drawing.Size(99, 49);
            this.lblTyrePressure.TabIndex = 4;
            this.lblTyrePressure.Text = "300";
            this.lblTyrePressure.TextLines = 1;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::SecondMonitor.Properties.Resources.tirepressure300;
            this.pictureBox2.Location = new System.Drawing.Point(123, 74);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(25, 18);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 6;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::SecondMonitor.Properties.Resources.brake;
            this.pictureBox1.Location = new System.Drawing.Point(3, 76);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(18, 18);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // WheelStatusControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblTyrePressure);
            this.Controls.Add(this.wheelTempRight);
            this.Controls.Add(this.wheelTempCenter);
            this.Controls.Add(this.wheelTempLeft);
            this.Controls.Add(this.lblBreakTemp);
            this.Name = "WheelStatusControl";
            this.Size = new System.Drawing.Size(225, 150);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private LCDLabel.LcdLabel lblBreakTemp;
        private LCDLabel.LcdLabel wheelTempLeft;
        private LCDLabel.LcdLabel wheelTempCenter;
        private LCDLabel.LcdLabel wheelTempRight;
        private LCDLabel.LcdLabel lblTyrePressure;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}
