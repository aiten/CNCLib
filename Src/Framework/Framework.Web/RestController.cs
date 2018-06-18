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
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Framework.Web
{
    public abstract class RestController<T> : Controller
    {
        protected RestController(IRest<T> controller)
        {
            Rest = controller ?? throw new ArgumentNullException();
        }

        public IRest<T> Rest { get; }
/*
        [HttpGet]
        public async Task<IActionResult> Get()
		{
			return await this.GetAll<T>(Rest);
		}
*/
        [HttpGet("{id:int}")]
		public async Task<ActionResult<T>> Get(int id)
		{
		    return await this.Get(Rest,id);
		}

        [HttpPost]
		public async Task<ActionResult<T>> Post([FromBody]T value)
		{
            // return url to new object
		    return await this.Post(Rest, value);
		}

        [HttpPut("{id:int}")]
        public async Task<ActionResult<T>> Put(int id, [FromBody]T value)
		{
		    return await this.Put(Rest, id, value);
		}

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<T>> Delete(int id)
		{
		    return await this.Delete(Rest, id);
		}
	}
}
