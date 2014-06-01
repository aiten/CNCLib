using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Framework.Tools;

namespace GCode.Logic.Commands
{
	[IsGCommand("G1,G01")]
	class G01Command : Command
    {
		#region crt + factory
		public G01Command()
		{
			PositionValid = true;
			Movetype = MoveType.Normal;
			Code = "G1";
		}

		#endregion

		#region GCode

		#endregion

		#region Draw

		#endregion
    }
}
