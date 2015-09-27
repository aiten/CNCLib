using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNCLib.Repository.Entities
{
	public class Configuration
	{
		public string Group { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public string Value { get; set; }

		public void SetValue(string group, string name, object value)
		{
			Name = name;
			Group = group;
			Value = value.ToString();

			Type = value.GetType().ToString();
		}
	}
}
