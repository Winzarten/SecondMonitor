namespace SecondWindow.CarStatus.Forms
{
    partial class CarStatusForm
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
            System.Windows.Forms.AGaugeLabel aGaugeLabel1 = new System.Windows.Forms.AGaugeLabel();
            System.Windows.Forms.AGaugeRange aGaugeRange1 = new System.Windows.Forms.AGaugeRange();
            this.gWaterTemp = new System.Windows.Forms.AGauge();
            this.pedalControl1 = new SecondWindow.CarStatus.Forms.Controls.PedalControl();
            this.oilControl2 = new SecondWindow.CarStatus.Forms.Controls.OilControl();
            this.SuspendLayout();
            // 
            // gWaterTemp
            // 
            this.gWaterTemp.BackColor = System.Drawing.Color.White;
            this.gWaterTemp.BaseArcColor = System.Drawing.Color.Navy;
            this.gWaterTemp.BaseArcRadius = 80;
            this.gWaterTemp.BaseArcStart = 135;
            this.gWaterTemp.BaseArcSweep = 270;
            this.gWaterTemp.BaseArcWidth = 2;
            this.gWaterTemp.Center = new System.Drawing.Point(100, 100);
            aGaugeLabel1.Color = System.Drawing.Color.DarkRed;
            aGaugeLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            aGaugeLabel1.Name = "GaugeLabel1";
            aGaugeLabel1.Position = new System.Drawing.Point(65, 60);
            aGaugeLabel1.Text = "Water Temp";
            this.gWaterTemp.GaugeLabels.Add(aGaugeLabel1);
            aGaugeRange1.Color = System.Drawing.Color.Red;
            aGaugeRange1.EndValue = 120F;
            aGaugeRange1.InnerRadius = 70;
            aGaugeRange1.InRange = false;
            aGaugeRange1.Name = "GaugeRange1";
            aGaugeRange1.OuterRadius = 90;
            aGaugeRange1.StartValue = 100F;
            this.gWaterTemp.GaugeRanges.Add(aGaugeRange1);
            this.gWaterTemp.Location = new System.Drawing.Point(382, 12);
            this.gWaterTemp.MaxValue = 120F;
            this.gWaterTemp.MinValue = 0F;
            this.gWaterTemp.Name = "gWaterTemp";
            this.gWaterTemp.NeedleColor1 = System.Windows.Forms.AGaugeNeedleColor.Green;
            this.gWaterTemp.NeedleColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.gWaterTemp.NeedleRadius = 80;
            this.gWaterTemp.NeedleType = System.Windows.Forms.NeedleType.Advance;
            this.gWaterTemp.NeedleWidth = 2;
            this.gWaterTemp.ScaleLinesInterColor = System.Drawing.Color.Black;
            this.gWaterTemp.ScaleLinesInterInnerRadius = 73;
            this.gWaterTemp.ScaleLinesInterOuterRadius = 80;
            this.gWaterTemp.ScaleLinesInterWidth = 1;
            this.gWaterTemp.ScaleLinesMajorColor = System.Drawing.Color.Black;
            this.gWaterTemp.ScaleLinesMajorInnerRadius = 70;
            this.gWaterTemp.ScaleLinesMajorOuterRadius = 80;
            this.gWaterTemp.ScaleLinesMajorStepValue = 20F;
            this.gWaterTemp.ScaleLinesMajorWidth = 2;
            this.gWaterTemp.ScaleLinesMinorColor = System.Drawing.Color.Gray;
            this.gWaterTemp.ScaleLinesMinorInnerRadius = 75;
            this.gWaterTemp.ScaleLinesMinorOuterRadius = 80;
            this.gWaterTemp.ScaleLinesMinorTicks = 9;
            this.gWaterTemp.ScaleLinesMinorWidth = 1;
            this.gWaterTemp.ScaleNumbersColor = System.Drawing.Color.Black;
            this.gWaterTemp.ScaleNumbersFormat = null;
            this.gWaterTemp.ScaleNumbersRadius = 60;
            this.gWaterTemp.ScaleNumbersRotation = 90;
            this.gWaterTemp.ScaleNumbersStartScaleLine = 2;
            this.gWaterTemp.ScaleNumbersStepScaleLines = 1;
            this.gWaterTemp.Size = new System.Drawing.Size(205, 187);
            this.gWaterTemp.TabIndex = 0;
            this.gWaterTemp.Text = "gWaterTemp";
            this.gWaterTemp.Value = 0F;
            // 
            // pedalControl1
            // 
            this.pedalControl1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(5)))), ((int)(((byte)(5)))));
            this.pedalControl1.Location = new System.Drawing.Point(640, 262);
            this.pedalControl1.Name = "pedalControl1";
            this.pedalControl1.Size = new System.Drawing.Size(150, 150);
            this.pedalControl1.TabIndex = 1;
            // 
            // oilControl2
            // 
            this.oilControl2.BackColor = System.Drawing.Color.White;
            this.oilControl2.Location = new System.Drawing.Point(139, 12);
            this.oilControl2.Name = "oilControl2";
            this.oilControl2.Size = new System.Drawing.Size(237, 290);
            this.oilControl2.TabIndex = 2;
            // 
            // CarStatusForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(791, 424);
            this.Controls.Add(this.oilControl2);
            this.Controls.Add(this.pedalControl1);
            this.Controls.Add(this.gWaterTemp);
            this.Name = "CarStatusForm";
            this.ShowIcon = false;
            this.Text = "Car Status";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CarStatusForm_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.AGauge gWaterTemp;
        private Controls.PedalControl pedalControl1;        
        private Controls.OilControl oilControl2;
    }
}