using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace GCode.GUI.Shapes
{
    class PolyLine : Command
    {
        public class LinePoint
        {
            public Point DrawPos { get; set; }
            public Point Mm1000Pos { get; set; }
            public String Mm1000Command { get { return Command.GCodeHelper("PD",Mm1000Pos); } }
        };

        public List<LinePoint> Points { get { return _points; } }

        private List<LinePoint> _points = new List<LinePoint>();

        public override Command CreateCommand()
        {
            return new PolyLine();
        }

        public Point LastDrawPos()
        {
            return _points.Count == 0 ? DrawStart : _points.Last().DrawPos;
        }

        public override void AdjustDrawPos(ToClient toDrawPos)
        {
            base.AdjustDrawPos(toDrawPos);
            foreach (LinePoint pt in _points)
            {
                pt.DrawPos = toDrawPos(pt.Mm1000Pos);
            }
        }

        public override string[] GetMm1000Commands()
        {
            List<String> cmds = new List<string>();
            cmds.Add(GCodeHelper("PU",Mm1000Start));

            foreach (LinePoint pt in _points)
            {
                cmds.Add(pt.Mm1000Command);
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
