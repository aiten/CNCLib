/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) Herbert Aitenbichler

  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
  to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
  and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CNCLib.Logic.Contract.DTO;
using CNCLib.Service.Contract;
using CNCLib.Shared;

using Framework.WebAPI.Controller;

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
            ((CNCLibUserContext)_userContext).InitFromController(this);
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