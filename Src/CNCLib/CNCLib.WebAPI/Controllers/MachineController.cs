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
    public class MachineController : Controller
    {
        private readonly IMachineService    _service;
        private readonly ICNCLibUserContext _userContext;

        public MachineController(IMachineService service, ICNCLibUserContext usercontext)
        {
            _service     = service ?? throw new ArgumentNullException();
            _userContext = usercontext ?? throw new ArgumentNullException();
            ((CNCLibUserContext) _userContext).InitFromController(this);
        }

        #region default REST

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Machine>>> Get()
        {
            return await this.GetAll(_service);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Machine>> Get(int id)
        {
            return await this.Get(_service, id);
        }

        [HttpPost]
        public async Task<ActionResult<Machine>> Add([FromBody] Machine value)
        {
            return await this.Add<Machine, int>(_service, value);
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] Machine value)
        {
            return await this.Update<Machine, int>(_service, id, value.MachineId, value);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await this.Delete<Machine, int>(_service, id);
        }

        #endregion

        #region bulk

        [HttpPost]
        [Route("bulk")]
        public async Task<ActionResult<IEnumerable<UriAndValue<Machine>>>> Add([FromBody] IEnumerable<Machine> values)
        {
            return await this.Add<Machine, int>(_service, values);
        }

        [HttpPut]
        [Route("bulk")]
        public async Task<ActionResult> Update([FromBody] IEnumerable<Machine> values)
        {
            return await this.Update<Machine, int>(_service, values);
        }

        [HttpDelete]
        [Route("bulk")]
        public async Task<ActionResult> Delete(int[] ids)
        {
            return await this.Delete<Machine, int>(_service, ids);
        }

        #endregion

        [Route("default")]
        [HttpGet]
        public async Task<ActionResult<Machine>> DefaultMachine()
        {
            var m = await _service.DefaultMachine();
            if (m == null)
            {
                return NotFound();
            }

            return Ok(m);
        }

        [Route("defaultmachine")]
        [HttpGet]
        //Always explicitly state the accepted HTTP method
        public async Task<ActionResult<int>> GetDetaultMachine()
        {
            int id = await _service.GetDetaultMachine();
            return Ok(id);
        }

        [Route("defaultmachine")]
        [HttpPut]
        //Always explicitly state the accepted HTTP method
        public async Task<ActionResult> SetDetaultMachine(int id)
        {
            await _service.SetDetaultMachine(id);
            return StatusCode(204);
        }
    }
}