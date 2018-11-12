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

using CNCLib.Logic.Contracts.DTO;
using CNCLib.Service.Contracts;
using CNCLib.Shared;

using Framework.Web.Controllers;

using Microsoft.AspNetCore.Mvc;

namespace CNCLib.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class ItemController : Controller
    {
        private readonly IItemService       _service;
        private readonly ICNCLibUserContext _userContext;

        public ItemController(IItemService service, ICNCLibUserContext userContext)
        {
            _service     = service ?? throw new ArgumentNullException();
            _userContext = userContext ?? throw new ArgumentNullException();
            ((CNCLibUserContext) _userContext).InitFromController(this);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> Get(string classname)
        {
            if (classname == null)
            {
                return await this.GetAll(_service);
            }

            IEnumerable<Item> m = await _service.GetByClassName(classname);
            return await this.NotFoundOrOk(m);
        }

        #region default REST

/*
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> Get()
        {
            return await this.GetAll(_service);
        }
*/
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Item>> Get(int id)
        {
            return await this.Get<Item, int>(_service, id);
        }

        [HttpPost]
        public async Task<ActionResult<Item>> Add([FromBody] Item value)
        {
            return await this.Add<Item, int>(_service, value);
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] Item value)
        {
            return await this.Update<Item, int>(_service, id, value.ItemId, value);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await this.Delete<Item, int>(_service, id);
        }

        #endregion
    }
}