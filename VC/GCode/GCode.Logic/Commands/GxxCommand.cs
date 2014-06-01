using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Framework.Tools;

namespace GCode.Logic.Commands
{
	[IsGCommand("GXX")]
	class GxxCommand : Command
    {
		#region crt + factory

		public GxxCommand()
		{
			Code = "";
		}

		#endregion

		new public string Code
		{
			get { return base.Code;  }
			set { base.Code = value;  }
		}

		#region GCode

		#endregion

		#region Draw

		#endregion
	}
}
