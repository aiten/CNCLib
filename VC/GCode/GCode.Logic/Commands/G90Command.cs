using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Tools;

namespace GCode.Logic.Commands
{
	[IsGCommand]
	class G90Command : Command
    {
		#region crt + factory

		public G90Command()
		{
			Code = GetType().Name.Substring(0, 3);
		}

		#endregion

		#region GCode

		#endregion

		#region Draw

		#endregion
	}
}
