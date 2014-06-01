using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Framework.Tools;

namespace GCode.Logic.Commands
{
	[IsGCommand("G4,G04")]
	class G04Command : Command
    {
		#region crt + factory

		public G04Command()
		{
			Code = "G04";
		}

		#endregion

		#region GCode

		#endregion

		#region Draw

		#endregion
	}
}
