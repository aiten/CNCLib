using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Tools;

namespace GCode.Logic.Commands
{
	[IsGCommand("Mxx")]
	class MxxCommand : Command
    {
		#region crt + factory

		public MxxCommand()
		{
			Code = "";
		}

		#endregion

		#region GCode
		new public string Code
		{
			get { return base.Code; }
			set { base.Code = value; }
		}

		#endregion

		#region Draw

		#endregion
	}
}
