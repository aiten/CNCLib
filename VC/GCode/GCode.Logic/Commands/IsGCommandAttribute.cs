using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCode.Logic.Commands
{
	[System.AttributeUsage(System.AttributeTargets.Class)]
	public class IsGCommandAttribute : Attribute
	{
		public bool IsGComamnd { get; set; }
		public String RegisterAs { get; set; }
		public IsGCommandAttribute()
		{
			IsGComamnd = true;
		}
		public IsGCommandAttribute(string registeras)
		{
			RegisterAs = registeras;
		}
	}
}
