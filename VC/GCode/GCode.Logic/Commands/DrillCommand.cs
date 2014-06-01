using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Tools;

namespace GCode.Logic.Commands
{
    class DrillCommand : Command
    {
		#region crt + factory

		public DrillCommand()
		{
			PositionValid = true;
			Movetype = MoveType.Fast;
		}

		#endregion

		#region GCode

		#endregion

		#region Draw

		public override void Draw(IOutputCommand output, object param)
		{
			base.Draw(output, param);
			output.DrawEllipse(this, param, MoveType.Normal, CalculatedEndPosition, 10, 10);
        }

		#endregion
	}
}
