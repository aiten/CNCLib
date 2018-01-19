////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2018 Herbert Aitenbichler

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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Framework.Web;
using CNCLib.Logic.Contracts.DTO;
using CNCLib.ServiceProxy;

namespace CNCLib.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class ItemController : RestController<Item>
    {
        public ItemController(IItemService service, IRest<Item> rest) : base(rest)
        {
            _service = service ?? throw new ArgumentNullException();
        }

	    readonly IItemService _service;

        [HttpGet]
	    public async Task<IActionResult> Get(string classname)
	    {
	        if (classname == null)
	        {
	            return await this.GetAll<Item>(Rest);
	        }

	        IEnumerable<Item> m = await _service.GetByClassName(classname);
	        return await this.NotFoundOrOk(m);
	    }
	}
}
