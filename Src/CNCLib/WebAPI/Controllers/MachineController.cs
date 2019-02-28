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
    public class MachineController : Controller
    {
        private readonly IMachineService    _service;
        private readonly ICNCLibUserContext _userContext;

        public MachineController(IMachineService service, ICNCLibUserContext userContext)
        {
            _service     = service ?? throw new ArgumentNullException();
            _userContext = userContext ?? throw new ArgumentNullException();
            ((CNCLibUserContext)_userContext).InitFromController(this);
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
        public async Task<ActionResult<int>> GetDefaultMachine()
        {
            int id = await _service.GetDefaultMachine();
            return Ok(id);
        }

        [Route("defaultmachine")]
        [HttpPut]

        //Always explicitly state the accepted HTTP method
        public async Task<ActionResult> SetDefaultMachine(int id)
        {
            await _service.SetDefaultMachine(id);
            return StatusCode(204);
        }
    }
}