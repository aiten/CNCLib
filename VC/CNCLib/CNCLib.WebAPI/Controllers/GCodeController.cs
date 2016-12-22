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
using System.Threading.Tasks;
using System.Web.Http;
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
			return GCodeLoadHelper.CallLoad(input.FileName, input.FileContent, input).Commands.ToStringList();
		}

//		[ActionName("CreateGCode")]
		public async Task<IEnumerable<string>> Put([FromBody] CreateGCode input)
		{
			using (var service = Dependency.Resolve<ILoadOptionsService>())
			{
				LoadOptions opt = await service.Get(input.LoadOptionsId);
				return GCodeLoadHelper.CallLoad(input.FileName, input.FileContent, opt).Commands.ToStringList();
			}
		}
	}
}
