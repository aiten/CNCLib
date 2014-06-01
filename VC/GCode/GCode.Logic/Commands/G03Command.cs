using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Framework.Tools;

namespace GCode.Logic.Commands
{
	[IsGCommand("G3,G03")]
	class G03Command : Command
    {
		#region crt + factory

		public G03Command()
		{
			PositionValid = true;
			Movetype = MoveType.Normal;
			Code = "G3";
		}

		#endregion

		#region GCode

		#endregion

		#region Draw

		public override void Draw(IOutputCommand output, object param)
		{
			base.Draw(output, param);
			/*

						Rectangle rect;
						Point start = DrawStartPosition;
						rect = new Rectangle(start, new Size(DrawEndPosition.X - start.X, DrawEndPosition.Y - start.Y));
						e.Graphics.DrawEllipse(GetForgroundPen(paintstate), rect);
			 */
		}

		#endregion
	}
}
