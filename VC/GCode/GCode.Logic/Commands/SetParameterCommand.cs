using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Tools;

namespace GCode.Logic.Commands
{
	[IsGCommand("#")]
	class SetParameterCommand : Command
    {
		#region crt + factory

		public SetParameterCommand()
		{
			Code = "#";
		}

		#endregion

		#region GCode
		public override string[] GetGCodeCommands(SpaceCoordinate startfrom)
		{
			string[] ret = new string[] 
            {
                GCodeAdd
            };
			return ret;
		}

		#endregion

		#region Draw

		#endregion
	}
}
