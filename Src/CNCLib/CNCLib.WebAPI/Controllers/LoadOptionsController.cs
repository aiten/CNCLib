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
    public class LoadOptionsController : Controller
    {
        private readonly ILoadOptionsService _service;
        private readonly ICNCLibUserContext  _userContext;

        public LoadOptionsController(ILoadOptionsService service, ICNCLibUserContext userContext)
        {
            _service     = service ?? throw new ArgumentNullException();
            _userContext = userContext ?? throw new ArgumentNullException();
            ((CNCLibUserContext) _userContext).InitFromController(this);
        }

        #region default REST

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoadOptions>>> Get()
        {
            return await this.GetAll(_service);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LoadOptions>> Get(int id)
        {
            return await this.Get<LoadOptions, int>(_service, id);
        }

        [HttpPost]
        public async Task<ActionResult<LoadOptions>> Add([FromBody] LoadOptions value)
        {
            return await this.Add<LoadOptions, int>(_service, value);
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] LoadOptions value)
        {
            return await this.Update<LoadOptions, int>(_service, id, value.Id, value);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await this.Delete<LoadOptions, int>(_service, id);
        }

        #endregion
    }
}