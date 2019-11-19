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
    public class UserController : Controller
    {
        private readonly IUserManager       _manager;
        private readonly ICNCLibUserContext _userContext;

        public UserController(IUserManager manager, ICNCLibUserContext userContext)
        {
            _manager     = manager ?? throw new ArgumentNullException(nameof(manager));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            ((CNCLibUserContext)_userContext).InitFromController(this);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                var m = await _manager.GetByName(userName);
                if (m == null)
                {
                    return NotFound();
                }

                return Ok(new List<User>() { m });
            }

            return await this.GetAll(_manager);
        }

        [HttpGet("isValidUser")]
        public async Task<ActionResult<string>> IsValidUser(string userName, string password)
        {
            bool isValidUser = false;

            if (!string.IsNullOrEmpty(userName))
            {
                isValidUser = (await _manager.Authenticate(userName, password)).HasValue;
            }

            return Ok(isValidUser ? "true" : "false");
        }

        #region default REST

        [HttpGet("{id:int}")]
        public async Task<ActionResult<User>> Get(int id)
        {
            return await this.Get<User, int>(_manager, id);
        }

        [HttpPost]
        public async Task<ActionResult<User>> Add([FromBody] User value)
        {
            return await this.Add<User, int>(_manager, value);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] User value)
        {
            return await this.Update<User, int>(_manager, id, value.UserId, value);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await this.Delete<User, int>(_manager, id);
        }

        #endregion
    }
}