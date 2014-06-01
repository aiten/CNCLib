using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Framework.Tools;
using System.Reflection;

namespace GCode.Logic.Commands
{
	[IsGCommand]
	class G91Command : Command
    {
		#region crt + factory

		public G91Command()
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
