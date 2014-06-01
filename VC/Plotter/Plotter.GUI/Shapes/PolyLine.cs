using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Plotter.GUI.Shapes
{
    public class PolyLine : Shape
    {
        public class LinePoint
        {
            public Point DrawPos { get; set; }
            public Point HPGLPos { get; set; }
            public String HPGLCommand { get { return Shape.HPGLHelper("PD",HPGLPos); } }
        };

        public List<LinePoint> Points { get { return _points; } }

        private List<LinePoint> _points = new List<LinePoint>();

        public override Shape CreateShape()
        {
            return new PolyLine();
        }

        public Point LastDrawPos()
        {
            return _points.Count == 0 ? DrawStart : _points.Last().DrawPos;
        }

        public override void AdjustDrawPos(HPGLToClient toDrawPos)
        {
            base.AdjustDrawPos(toDrawPos);
            foreach (LinePoint pt in _points)
            {
                pt.DrawPos = toDrawPos(pt.HPGLPos);
            }
        }

        public override string[] GetHPGLCommands()
        {
            List<String> cmds = new List<string>();
            cmds.Add(HPGLHelper("PU",HPGLStart));

            foreach (LinePoint pt in _points)
            {
                cmds.Add(pt.HPGLCommand);
            }
            return cmds.ToArray();
        }

        public override void Draw(PaintEventArgs e, PaintState paintstate)
        {
            Point old = DrawStart;
            Pen pen = GetForgroundPen(paintstate);
            foreach (LinePoint pt in _points)
            {
                MyDrawLine(e, pen, old, pt.DrawPos);
                old = pt.DrawPos;
            }
        }
    }
}
