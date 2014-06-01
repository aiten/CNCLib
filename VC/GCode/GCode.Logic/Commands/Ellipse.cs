using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Framework.Tools;

namespace GCode.Logic.Commands
{
    class Ellipse : Command
    {
		#region crt + factory

		public Ellipse()
		{
			DrawLineSize = 1;
		}

		#endregion

		#region GCode

		public override string[] GetGCodeCommands(SpaceCoordinate startfrom)
        {
			if (startfrom == null) startfrom = new SpaceCoordinate(0, 0, 70);
            List<string> gcode = new List<string>();
			decimal width  = Math.Abs(startfrom.X.Value - CalculatedEndPosition.X.Value);
			decimal height = Math.Abs(startfrom.Y.Value - CalculatedEndPosition.Y.Value);

			SpaceCoordinate center = new SpaceCoordinate(startfrom.X.Value + width / 2m, startfrom.Y.Value + height / 2m,0m);

            Circle(center, width/2m, gcode);

            return gcode.ToArray();
        }

		#endregion

		#region Draw

		public override void Draw(PaintEventArgs e, PaintState paintstate)
        {
			Rectangle rect;
			Point start = DrawStartPosition;
			rect = new Rectangle(start, new Size(DrawEndPosition.X - start.X, DrawEndPosition.Y - start.Y));
            e.Graphics.DrawEllipse(GetForgroundPen(paintstate), rect);
        }

		void Rotate(SpaceCoordinate p, ref SpaceCoordinate dp, double rad)
        {
	        double mysin = Math.Sin(rad);
            double mycos = Math.Cos(rad);

            dp.X = (decimal) ((double) p.X.Value * mycos + (double) p.Y.Value * mysin);
			dp.Y = (decimal) ((double) p.Y.Value * mycos - (double) p.X.Value * mysin);
        }

		void Circle(SpaceCoordinate center, decimal r, List<string> gcode)
        {
          Polygon(center,r,(int) (20 + r*100m),0,gcode);
        }

		void Polygon(SpaceCoordinate center, decimal radius, int n, int grad, List<string> gcode)
        {
          // mittelpunkt!
          if (n>1)
          {
            int i;
			SpaceCoordinate dp = new SpaceCoordinate();
			SpaceCoordinate pt = new SpaceCoordinate(radius, 0, 0);

            for (i=0;i<=n;i++)
            {
                double rad = ((360.0/n*i)+grad) / 180.0 * Math.PI;
                Rotate(pt, ref dp, rad);
                dp.Offset(center);

				//gcode.Add(GCodeHelper(i == 0 ? "g00" : "g01", dp, dp));
            }
          }
		}

		#endregion
	}
}
