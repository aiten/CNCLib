using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Tools;

namespace GCode.Logic.Commands
{
	[IsGCommand("G0,G00")]
    class G00Command : Command
    {

		#region crt + factory

		public G00Command()
		{
			PositionValid = true;
			Movetype = MoveType.Fast;
			Code = "G0";
		}

		#endregion

		#region GCode

		#endregion

		#region Draw

		#endregion
	}
}
