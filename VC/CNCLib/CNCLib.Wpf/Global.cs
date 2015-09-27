using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Tools;

namespace CNCLib.Wpf
{
	public class Global : Singleton<Global>
	{
		public CNCLib.Logic.DTO.Machine Machine { get; set; }
	}
}
