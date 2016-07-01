using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SecondMonitor.MonitorGui.CarStatus.Forms.Controls
{
    public partial class GMeter : UserControl
    {
        public GMeter()
        {
            GridColor = Color.Red;
            GBallColor = Color.Red;
            MaxG = 3;            
            GScale = 1;
            GBallDiameter = 2;
            HorizG = 1;
            DoubleBuffered = true;
            InitializeComponent();            

        }

        [Description("Color of the Grid"),
        Category("Design"),        
        Browsable(true)]
        public Color GridColor
        {
            get;
            set;
        }

        [Description("Color of the GBall"),
        Category("Design"),
        Browsable(true)]
        public Color GBallColor
        {
            get;
            set;
        }

        [Description("Maximum G"),
        Category("Design"),
        Browsable(true)]
        public int MaxG
        {
            get;
            set;
        }


        [Description("Scale of the meter"),
        Category("Design"),
        Browsable(true)]
        public int GScale
        {
            get;
            set;
        }

        [Description("Diameter of tHe Gball"),
        Category("Design"),
        Browsable(true)]
        public int GBallDiameter
        {
            get;
            set;
        }

        public double HorizG
        {
            get;
            set;
        }

        public double VertG
        {
            get;
            set;
        }


        private void DrawGrid(Graphics g)
        {
            if (this.Width == 0 || this.Height == 0)
                return;
            Point center = new Point(this.Width / 2, this.Height / 2);
            Pen pen = new Pen(GridColor);

            g.DrawLine(pen, this.Width / 2, 0, this.Width / 2, this.Height);
            g.DrawLine(pen, 0, this.Height/2, this.Width, this.Height/2);

            int currentG = 0 + GScale;
            int xScalePerG = this.Width / (MaxG * 2);
            int yScalePerG = this.Height / (MaxG * 2);
            while (currentG <= MaxG)
            {
                int currentXScalePerG = xScalePerG * currentG;
                int currentYScalePerG = yScalePerG * currentG;

                Rectangle rec = new Rectangle(center.X - currentXScalePerG, center.Y - currentYScalePerG, currentXScalePerG * 2, currentYScalePerG * 2);
                g.DrawEllipse(pen, rec);

                currentG += GScale;
            }

        }

        private void DrawGBall(Graphics g)
        {

            if (this.Width == 0 || this.Height == 0)
                return;
           
            Point center = new Point(this.Width / 2, this.Height / 2);
            int xScalePerG = this.Width / (MaxG * 2);
            int yScalePerG = this.Height / (MaxG * 2);

            Point ballLocation = new Point(center.X + (int)(xScalePerG * HorizG), center.Y + (int)(yScalePerG * VertG));

            Pen pen = new Pen(GBallColor);
            Brush brush = new SolidBrush(GBallColor);

            g.FillEllipse(brush, ballLocation.X - GBallDiameter, ballLocation.Y - GBallDiameter, GBallDiameter * 2, GBallDiameter * 2);
            
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            // Call the OnPaint method of the base class.
            base.OnPaint(pe);

            DrawGrid(pe.Graphics);
            DrawGBall(pe.Graphics);
        }

    }
}
