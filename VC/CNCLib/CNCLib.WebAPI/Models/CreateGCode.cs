using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNCLib.WebAPI.Models
{
	public class CreateGCode
	{
		public int LoadOptionsId { get; set; }
		public String FileName { get; set; }

		public Byte[] FileContent { get; set; }
	}
}