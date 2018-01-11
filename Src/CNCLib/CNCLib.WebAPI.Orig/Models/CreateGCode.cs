using System;

namespace CNCLib.WebAPI.Models
{
    public class CreateGCode
	{
		public int LoadOptionsId { get; set; }
		public string FileName { get; set; }

		public Byte[] FileContent { get; set; }
	}
}