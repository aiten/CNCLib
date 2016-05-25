////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

  CNCLib is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  CNCLib is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.
  http://www.gnu.org/licenses/
*/

using System.Collections.Generic;
using System.IO;
using System.Web.Http;
using CNCLib.GCode.Load;
using CNCLib.Logic.Contracts;
using CNCLib.Logic.Contracts.DTO;
using CNCLib.ServiceProxy;
using CNCLib.WebAPI.Models;
using Framework.Tools.Dependency;

namespace CNCLib.WebAPI.Controllers
{
	public class GCodeController : ApiController
	{
//		[ActionName("")]
		public IEnumerable<string> Post([FromBody] LoadOptions input)
		{
			string filename = Path.GetFileName(input.FileName);
			string tmpfile = Path.GetTempPath() + filename;
			input.FileName = tmpfile;
			input.ImageWriteToFileName = null;

			try
			{
				File.WriteAllBytes(tmpfile, input.FileContent);

				LoadBase load = LoadBase.Create(input);

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

//		[ActionName("CreateGCode")]
		public IEnumerable<string> Put([FromBody] CreateGCode input)
		{
			using (var service = Dependency.Resolve<ILoadOptionsService>())
			{
				LoadOptions opt = service.Get(input.LoadOptionsId);
				string filename = Path.GetFileName(input.FileName);
				string tmpfile = Path.GetTempPath() + filename;

				opt.FileName = tmpfile;
				opt.ImageWriteToFileName = null;

				try
				{
					File.WriteAllBytes(tmpfile, input.FileContent);

					LoadBase load = LoadBase.Create(opt);

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
}
