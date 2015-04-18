using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Tools;

namespace Proxxon.Wpf
{
	public class Global : Singleton<Global>
	{
		public Proxxon.Logic.DTO.Machine Machine { get; set; }
	}
}
