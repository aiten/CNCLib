using CNCLib.GCode.Commands;
using CNCLib.GCode.Load;
using CNCLib.Logic.Contracts;
using CNCLib.Logic.Contracts.DTO;
using Framework.Tools.Dependency;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CNCLib.WebAPI.Controllers
{
	public class GCodeController : ApiController
	{
		public IEnumerable<string> Post([FromBody] LoadInfo input)
		{
			string filename = Path.GetFileName(input.FileName);
			string tmpfile = Path.GetTempPath() + filename;
			input.FileName = tmpfile;
			input.ImageWriteToFileName = null;

			try
			{
				File.WriteAllBytes(tmpfile, input.FileContent);

				LoadBase load=LoadBase.Create(input);

				if (load == null)
					return null;
				
				load.Load();
				return load.Commands.ToStringList();
			}
			finally
			{
				File.Delete(tmpfile);
			}
		}
	}
}
