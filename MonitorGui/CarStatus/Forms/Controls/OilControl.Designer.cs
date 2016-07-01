namespace SecondMonitor.CarStatus.Forms.Controls
{
    partial class OilControl
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
            System.Windows.Forms.AGaugeLabel aGaugeLabel4 = new System.Windows.Forms.AGaugeLabel();
            System.Windows.Forms.AGaugeRange aGaugeRange4 = new System.Windows.Forms.AGaugeRange();
            this.gTemperature = new System.Windows.Forms.AGauge();
            this.gPressure = new System.Windows.Forms.AGauge();
            this.lblPressure = new LCDLabel.LcdLabel();
            this.SuspendLayout();
            // 
            // gTemperature
            // 
            this.gTemperature.BaseArcColor = System.Drawing.Color.SteelBlue;
            this.gTemperature.BaseArcRadius = 80;
            this.gTemperature.BaseArcStart = 150;
            this.gTemperature.BaseArcSweep = 230;
            this.gTemperature.BaseArcWidth = 2;
            this.gTemperature.Center = new System.Drawing.Point(100, 100);
            aGaugeLabel4.Color = System.Drawing.Color.DarkRed;
            aGaugeLabel4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            aGaugeLabel4.Name = "GaugeLabel1";
            aGaugeLabel4.Position = new System.Drawing.Point(70, 50);
            aGaugeLabel4.Text = "Oil Temp";
            this.gTemperature.GaugeLabels.Add(aGaugeLabel4);
            aGaugeRange4.Color = System.Drawing.Color.Red;
            aGaugeRange4.EndValue = 200F;
            aGaugeRange4.InnerRadius = 70;
            aGaugeRange4.InRange = false;
            aGaugeRange4.Name = "GaugeRange1";
            aGaugeRange4.OuterRadius = 80;
            aGaugeRange4.StartValue = 150F;
            this.gTemperature.GaugeRanges.Add(aGaugeRange4);
            this.gTemperature.Location = new System.Drawing.Point(3, 3);
            this.gTemperature.MaxValue = 200F;
            this.gTemperature.MinValue = 0F;
            this.gTemperature.Name = "gTemperature";
            this.gTemperature.NeedleColor1 = System.Windows.Forms.AGaugeNeedleColor.Gray;
            this.gTemperature.NeedleColor2 = System.Drawing.Color.DimGray;
            this.gTemperature.NeedleRadius = 80;
            this.gTemperature.NeedleType = System.Windows.Forms.NeedleType.Advance;
            this.gTemperature.NeedleWidth = 2;
            this.gTemperature.ScaleLinesInterColor = System.Drawing.Color.Black;
            this.gTemperature.ScaleLinesInterInnerRadius = 73;
            this.gTemperature.ScaleLinesInterOuterRadius = 80;
            this.gTemperature.ScaleLinesInterWidth = 1;
            this.gTemperature.ScaleLinesMajorColor = System.Drawing.Color.Black;
            this.gTemperature.ScaleLinesMajorInnerRadius = 70;
            this.gTemperature.ScaleLinesMajorOuterRadius = 80;
            this.gTemperature.ScaleLinesMajorStepValue = 20F;
            this.gTemperature.ScaleLinesMajorWidth = 2;
            this.gTemperature.ScaleLinesMinorColor = System.Drawing.Color.Gray;
            this.gTemperature.ScaleLinesMinorInnerRadius = 75;
            this.gTemperature.ScaleLinesMinorOuterRadius = 80;
            this.gTemperature.ScaleLinesMinorTicks = 9;
            this.gTemperature.ScaleLinesMinorWidth = 1;
            this.gTemperature.ScaleNumbersColor = System.Drawing.Color.Black;
            this.gTemperature.ScaleNumbersFormat = null;
            this.gTemperature.ScaleNumbersRadius = 95;
            this.gTemperature.ScaleNumbersRotation = 0;
            this.gTemperature.ScaleNumbersStartScaleLine = 0;
            this.gTemperature.ScaleNumbersStepScaleLines = 1;
            this.gTemperature.Size = new System.Drawing.Size(214, 179);
            this.gTemperature.TabIndex = 0;
            this.gTemperature.Text = "aGauge1";
            this.gTemperature.Value = 0F;
            // 
            // gPressure
            // 
            this.gPressure.BaseArcColor = System.Drawing.Color.Green;
            this.gPressure.BaseArcRadius = 70;
            this.gPressure.BaseArcStart = 180;
            this.gPressure.BaseArcSweep = 180;
            this.gPressure.BaseArcWidth = 2;
            this.gPressure.Center = new System.Drawing.Point(100, 100);
            this.gPressure.Location = new System.Drawing.Point(12, 144);
            this.gPressure.MaxValue = 10F;
            this.gPressure.MinValue = 0F;
            this.gPressure.Name = "gPressure";
            this.gPressure.NeedleColor1 = System.Windows.Forms.AGaugeNeedleColor.Gray;
            this.gPressure.NeedleColor2 = System.Drawing.Color.DimGray;
            this.gPressure.NeedleRadius = 80;
            this.gPressure.NeedleType = System.Windows.Forms.NeedleType.Advance;
            this.gPressure.NeedleWidth = 2;
            this.gPressure.ScaleLinesInterColor = System.Drawing.Color.Black;
            this.gPressure.ScaleLinesInterInnerRadius = 73;
            this.gPressure.ScaleLinesInterOuterRadius = 80;
            this.gPressure.ScaleLinesInterWidth = 1;
            this.gPressure.ScaleLinesMajorColor = System.Drawing.Color.Black;
            this.gPressure.ScaleLinesMajorInnerRadius = 70;
            this.gPressure.ScaleLinesMajorOuterRadius = 80;
            this.gPressure.ScaleLinesMajorStepValue = 1F;
            this.gPressure.ScaleLinesMajorWidth = 2;
            this.gPressure.ScaleLinesMinorColor = System.Drawing.Color.Gray;
            this.gPressure.ScaleLinesMinorInnerRadius = 75;
            this.gPressure.ScaleLinesMinorOuterRadius = 80;
            this.gPressure.ScaleLinesMinorTicks = 2;
            this.gPressure.ScaleLinesMinorWidth = 1;
            this.gPressure.ScaleNumbersColor = System.Drawing.Color.Black;
            this.gPressure.ScaleNumbersFormat = null;
            this.gPressure.ScaleNumbersRadius = 95;
            this.gPressure.ScaleNumbersRotation = 0;
            this.gPressure.ScaleNumbersStartScaleLine = 0;
            this.gPressure.ScaleNumbersStepScaleLines = 1;
            this.gPressure.Size = new System.Drawing.Size(205, 170);
            this.gPressure.TabIndex = 1;
            this.gPressure.Text = "aGauge1";
            this.gPressure.Value = 0F;
            this.gPressure.ValueInRangeChanged += new System.EventHandler<System.Windows.Forms.ValueInRangeChangedEventArgs>(this.gPressure_ValueInRangeChanged);
            // 
            // lblPressure
            // 
            this.lblPressure.BackGround = System.Drawing.Color.Silver;
            this.lblPressure.BorderColor = System.Drawing.Color.Black;
            this.lblPressure.BorderSpace = 0;
            this.lblPressure.CharSpacing = 0;
            this.lblPressure.DotMatrix = LCDLabel.DotMatrix.mat5x7;
            this.lblPressure.ForeColor = System.Drawing.Color.White;
            this.lblPressure.LineSpacing = 1;
            this.lblPressure.Location = new System.Drawing.Point(73, 252);
            this.lblPressure.Name = "lblPressure";
            this.lblPressure.NumberOfCharacters = 4;
            this.lblPressure.PixelHeight = 4;
            this.lblPressure.PixelOff = System.Drawing.Color.White;
            this.lblPressure.PixelOn = System.Drawing.Color.Red;
            this.lblPressure.PixelShape = LCDLabel.PixelShape.Round;
            this.lblPressure.PixelSize = LCDLabel.PixelSize.pix4x4;
            this.lblPressure.PixelSpacing = 0;
            this.lblPressure.PixelWidth = 4;
            this.lblPressure.Size = new System.Drawing.Size(82, 30);
            this.lblPressure.TabIndex = 2;
            this.lblPressure.Text = "4.5";
            this.lblPressure.TextLines = 1;
            // 
            // OilControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.lblPressure);
            this.Controls.Add(this.gPressure);
            this.Controls.Add(this.gTemperature);
            this.Name = "OilControl";
            this.Size = new System.Drawing.Size(221, 318);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.AGauge gTemperature;
        private System.Windows.Forms.AGauge gPressure;
        private LCDLabel.LcdLabel lblPressure;
    }
}
