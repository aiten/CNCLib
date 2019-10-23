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

using CNCLib.Logic.Abstraction;
using CNCLib.Logic.Abstraction.DTO;
using CNCLib.Shared;

using Framework.WebAPI.Controller;

using Microsoft.AspNetCore.Mvc;

namespace CNCLib.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class LoadOptionsController : Controller
    {
        private readonly ILoadOptionsManager _manager;
        private readonly ICNCLibUserContext  _userContext;

        public LoadOptionsController(ILoadOptionsManager manager, ICNCLibUserContext userContext)
        {
            _manager     = manager ?? throw new ArgumentNullException(nameof(manager));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            ((CNCLibUserContext)_userContext).InitFromController(this);
        }

        #region default REST

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoadOptions>>> Get()
        {
            return await this.GetAll(_manager);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LoadOptions>> Get(int id)
        {
            return await this.Get<LoadOptions, int>(_manager, id);
        }

        [HttpPost]
        public async Task<ActionResult<LoadOptions>> Add([FromBody] LoadOptions value)
        {
            return await this.Add<LoadOptions, int>(_manager, value);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] LoadOptions value)
        {
            return await this.Update<LoadOptions, int>(_manager, id, value.Id, value);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await this.Delete<LoadOptions, int>(_manager, id);
        }

        #endregion
    }
}