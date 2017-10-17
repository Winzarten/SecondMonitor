namespace SecondMonitor.CarStatus.Forms
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.AGaugeLabel aGaugeLabel1 = new System.Windows.Forms.AGaugeLabel();
            System.Windows.Forms.AGaugeRange aGaugeRange1 = new System.Windows.Forms.AGaugeRange();
            System.Windows.Forms.AGaugeLabel aGaugeLabel2 = new System.Windows.Forms.AGaugeLabel();
            System.Windows.Forms.AGaugeLabel aGaugeLabel3 = new System.Windows.Forms.AGaugeLabel();
            System.Windows.Forms.AGaugeRange aGaugeRange2 = new System.Windows.Forms.AGaugeRange();
            this.gWaterTemp = new System.Windows.Forms.AGauge();
            this.gFuel = new System.Windows.Forms.AGauge();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.stayOnTopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gMeter1 = new SecondMonitor.MonitorGui.CarStatus.Forms.Controls.GMeter();
            this.blbFuelPressure = new SecondMonitor.CarStatus.Forms.Controls.LedBulb();
            this.ctlRearRight = new SecondMonitor.CarStatus.Forms.Controls.WheelStatusControl();
            this.ctlFrontRight = new SecondMonitor.CarStatus.Forms.Controls.WheelStatusControl();
            this.ctlFrontLeft = new SecondMonitor.CarStatus.Forms.Controls.WheelStatusControl();
            this.ctlRearLeft = new SecondMonitor.CarStatus.Forms.Controls.WheelStatusControl();
            this.oilControl2 = new SecondMonitor.CarStatus.Forms.Controls.OilControl();
            this.pedalControl1 = new SecondMonitor.CarStatus.Forms.Controls.PedalControl();
            this.label1 = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
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
            this.gWaterTemp.Location = new System.Drawing.Point(255, 12);
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
            // gFuel
            // 
            this.gFuel.BackColor = System.Drawing.Color.White;
            this.gFuel.BaseArcColor = System.Drawing.Color.Gray;
            this.gFuel.BaseArcRadius = 50;
            this.gFuel.BaseArcStart = 225;
            this.gFuel.BaseArcSweep = 90;
            this.gFuel.BaseArcWidth = 2;
            this.gFuel.Center = new System.Drawing.Point(40, 55);
            aGaugeLabel2.Color = System.Drawing.SystemColors.WindowText;
            aGaugeLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            aGaugeLabel2.Name = "GaugeLabel1";
            aGaugeLabel2.Position = new System.Drawing.Point(70, 35);
            aGaugeLabel2.Text = "F";
            aGaugeLabel3.Color = System.Drawing.SystemColors.WindowText;
            aGaugeLabel3.Name = "GaugeLabel2";
            aGaugeLabel3.Position = new System.Drawing.Point(7, 35);
            aGaugeLabel3.Text = "E";
            this.gFuel.GaugeLabels.Add(aGaugeLabel2);
            this.gFuel.GaugeLabels.Add(aGaugeLabel3);
            aGaugeRange2.Color = System.Drawing.Color.Red;
            aGaugeRange2.EndValue = 25F;
            aGaugeRange2.InnerRadius = 40;
            aGaugeRange2.InRange = false;
            aGaugeRange2.Name = "GaugeRange1";
            aGaugeRange2.OuterRadius = 50;
            aGaugeRange2.StartValue = 0F;
            this.gFuel.GaugeRanges.Add(aGaugeRange2);
            this.gFuel.Location = new System.Drawing.Point(309, 176);
            this.gFuel.MaxValue = 100F;
            this.gFuel.MinValue = 0F;
            this.gFuel.Name = "gFuel";
            this.gFuel.NeedleColor1 = System.Windows.Forms.AGaugeNeedleColor.Red;
            this.gFuel.NeedleColor2 = System.Drawing.Color.Black;
            this.gFuel.NeedleRadius = 40;
            this.gFuel.NeedleType = System.Windows.Forms.NeedleType.Advance;
            this.gFuel.NeedleWidth = 2;
            this.gFuel.ScaleLinesInterColor = System.Drawing.Color.Black;
            this.gFuel.ScaleLinesInterInnerRadius = 45;
            this.gFuel.ScaleLinesInterOuterRadius = 50;
            this.gFuel.ScaleLinesInterWidth = 2;
            this.gFuel.ScaleLinesMajorColor = System.Drawing.Color.Black;
            this.gFuel.ScaleLinesMajorInnerRadius = 40;
            this.gFuel.ScaleLinesMajorOuterRadius = 50;
            this.gFuel.ScaleLinesMajorStepValue = 50F;
            this.gFuel.ScaleLinesMajorWidth = 3;
            this.gFuel.ScaleLinesMinorColor = System.Drawing.Color.Gray;
            this.gFuel.ScaleLinesMinorInnerRadius = 40;
            this.gFuel.ScaleLinesMinorOuterRadius = 60;
            this.gFuel.ScaleLinesMinorTicks = 1;
            this.gFuel.ScaleLinesMinorWidth = 1;
            this.gFuel.ScaleNumbersColor = System.Drawing.Color.Black;
            this.gFuel.ScaleNumbersFormat = null;
            this.gFuel.ScaleNumbersRadius = 95;
            this.gFuel.ScaleNumbersRotation = 0;
            this.gFuel.ScaleNumbersStartScaleLine = 4;
            this.gFuel.ScaleNumbersStepScaleLines = 3;
            this.gFuel.Size = new System.Drawing.Size(82, 73);
            this.gFuel.TabIndex = 3;
            this.gFuel.Text = "aGauge1";
            this.gFuel.Value = 0F;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stayOnTopToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowCheckMargin = true;
            this.contextMenuStrip1.Size = new System.Drawing.Size(162, 26);
            // 
            // stayOnTopToolStripMenuItem
            // 
            this.stayOnTopToolStripMenuItem.Name = "stayOnTopToolStripMenuItem";
            this.stayOnTopToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.stayOnTopToolStripMenuItem.Text = "Stay On Top";
            this.stayOnTopToolStripMenuItem.Click += new System.EventHandler(this.stayOnTopToolStripMenuItem_Click);
            // 
            // gMeter1
            // 
            this.gMeter1.BackColor = System.Drawing.Color.Transparent;
            this.gMeter1.GBallColor = System.Drawing.Color.Green;
            this.gMeter1.GBallDiameter = 4;
            this.gMeter1.GridColor = System.Drawing.Color.Red;
            this.gMeter1.GScale = 1;
            this.gMeter1.HorizG = 2D;
            this.gMeter1.Location = new System.Drawing.Point(264, 255);
            this.gMeter1.MaxG = 4;
            this.gMeter1.Name = "gMeter1";
            this.gMeter1.Size = new System.Drawing.Size(213, 190);
            this.gMeter1.TabIndex = 10;
            this.gMeter1.VertG = 1.5D;
            // 
            // blbFuelPressure
            // 
            this.blbFuelPressure.BackColor = System.Drawing.Color.White;
            this.blbFuelPressure.Color = System.Drawing.Color.Red;
            this.blbFuelPressure.Location = new System.Drawing.Point(309, 229);
            this.blbFuelPressure.Name = "blbFuelPressure";
            this.blbFuelPressure.On = true;
            this.blbFuelPressure.Size = new System.Drawing.Size(20, 20);
            this.blbFuelPressure.TabIndex = 9;
            // 
            // ctlRearRight
            // 
            this.ctlRearRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.ctlRearRight.Location = new System.Drawing.Point(243, 553);
            this.ctlRearRight.MinimumSize = new System.Drawing.Size(218, 96);
            this.ctlRearRight.Name = "ctlRearRight";
            this.ctlRearRight.Size = new System.Drawing.Size(218, 96);
            this.ctlRearRight.TabIndex = 7;
            this.ctlRearRight.WheelPostion = SecondMonitor.CarStatus.Forms.Controls.WheelStatusControl.WheelPostionEnum.RearRight;
            // 
            // ctlFrontRight
            // 
            this.ctlFrontRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.ctlFrontRight.Location = new System.Drawing.Point(243, 451);
            this.ctlFrontRight.MinimumSize = new System.Drawing.Size(218, 96);
            this.ctlFrontRight.Name = "ctlFrontRight";
            this.ctlFrontRight.Size = new System.Drawing.Size(218, 96);
            this.ctlFrontRight.TabIndex = 6;
            this.ctlFrontRight.WheelPostion = SecondMonitor.CarStatus.Forms.Controls.WheelStatusControl.WheelPostionEnum.FrontRight;
            // 
            // ctlFrontLeft
            // 
            this.ctlFrontLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.ctlFrontLeft.Location = new System.Drawing.Point(12, 451);
            this.ctlFrontLeft.MinimumSize = new System.Drawing.Size(218, 96);
            this.ctlFrontLeft.Name = "ctlFrontLeft";
            this.ctlFrontLeft.Size = new System.Drawing.Size(218, 96);
            this.ctlFrontLeft.TabIndex = 5;
            this.ctlFrontLeft.WheelPostion = SecondMonitor.CarStatus.Forms.Controls.WheelStatusControl.WheelPostionEnum.FrontLeft;
            // 
            // ctlRearLeft
            // 
            this.ctlRearLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.ctlRearLeft.Location = new System.Drawing.Point(12, 553);
            this.ctlRearLeft.MinimumSize = new System.Drawing.Size(218, 96);
            this.ctlRearLeft.Name = "ctlRearLeft";
            this.ctlRearLeft.Size = new System.Drawing.Size(218, 96);
            this.ctlRearLeft.TabIndex = 4;
            this.ctlRearLeft.WheelPostion = SecondMonitor.CarStatus.Forms.Controls.WheelStatusControl.WheelPostionEnum.RearLeft;
            // 
            // oilControl2
            // 
            this.oilControl2.BackColor = System.Drawing.Color.White;
            this.oilControl2.Location = new System.Drawing.Point(12, 12);
            this.oilControl2.Name = "oilControl2";
            this.oilControl2.Size = new System.Drawing.Size(237, 290);
            this.oilControl2.TabIndex = 2;
            // 
            // pedalControl1
            // 
            this.pedalControl1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(5)))), ((int)(((byte)(5)))));
            this.pedalControl1.Location = new System.Drawing.Point(466, 12);
            this.pedalControl1.Name = "pedalControl1";
            this.pedalControl1.Size = new System.Drawing.Size(82, 150);
            this.pedalControl1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 328);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 24);
            this.label1.TabIndex = 11;
            this.label1.Text = "lblSessionTime";
            // 
            // CarStatusForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(597, 769);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.gMeter1);
            this.Controls.Add(this.blbFuelPressure);
            this.Controls.Add(this.ctlRearRight);
            this.Controls.Add(this.ctlFrontRight);
            this.Controls.Add(this.ctlFrontLeft);
            this.Controls.Add(this.ctlRearLeft);
            this.Controls.Add(this.gFuel);
            this.Controls.Add(this.oilControl2);
            this.Controls.Add(this.pedalControl1);
            this.Controls.Add(this.gWaterTemp);
            this.Location = new System.Drawing.Point(1930, 30);
            this.Name = "CarStatusForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Car Status";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CarStatusForm_FormClosed);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.AGauge gWaterTemp;
        private Controls.PedalControl pedalControl1;        
        private Controls.OilControl oilControl2;
        private System.Windows.Forms.AGauge gFuel;
        private Controls.WheelStatusControl ctlRearLeft;
        private Controls.WheelStatusControl ctlFrontLeft;
        private Controls.WheelStatusControl ctlFrontRight;
        private Controls.WheelStatusControl ctlRearRight;
        private Controls.LedBulb blbFuelPressure;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem stayOnTopToolStripMenuItem;
        private MonitorGui.CarStatus.Forms.Controls.GMeter gMeter1;
        private System.Windows.Forms.Label label1;
    }
}