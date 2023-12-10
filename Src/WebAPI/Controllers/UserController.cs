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

namespace CNCLib.WebAPI.Controllers;

using System.Collections.Generic;
using System.Threading.Tasks;

using CNCLib.Logic.Abstraction;
using CNCLib.Logic.Abstraction.DTO;
using CNCLib.Shared;

using Framework.WebAPI.Controller;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[Route("api/[controller]")]
public class UserController : Controller
{
    private readonly IUserManager       _manager;
    private readonly ICNCLibUserContext _userContext;

    public UserController(IUserManager manager, ICNCLibUserContext userContext)
    {
        _manager     = manager;
        _userContext = userContext;
    }

    [Authorize(Policy = Policies.IsAdmin)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> Get(string userName)
    {
        if (!string.IsNullOrEmpty(userName))
        {
            var user = await _manager.GetByNameAsync(userName);
            if (user == null)
            {
                return Ok(new List<User>());
            }

            return Ok(new List<User>() { user });
        }

        return await this.GetAll(_manager);
    }

    [HttpGet("currentUser")]
    public async Task<ActionResult<User>> CurrentUser()
    {
        return Ok(await _manager.GetByNameAsync(_userContext.UserName!));
    }

    [AllowAnonymous]
    [HttpGet("isValidUser")]
    public async Task<ActionResult> IsValidUser(string userName, string password)
    {
        if ((await _manager.AuthenticateAsync(userName, password)) == null)
        {
            return Forbid();
        }

        return Ok();
    }

    [HttpPut("changePassword")]
    public async Task<ActionResult> ChangePassword(string userName, string passwordOld, string passwordNew)
    {
        if (!_userContext.IsAdmin && _userContext.UserName != userName)
        {
            return Forbid();
        }

        await _manager.ChangePasswordAsync(userName, passwordOld, passwordNew);
        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<string>> Register(string userName, string password)
    {
        var result = await _manager.RegisterAsync(userName, password);
        if (string.IsNullOrEmpty(result))
        {
            return Forbid();
        }

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("passwordHash")]
    public async Task<ActionResult<string>> PasswordHash(string password)
    {
        return Ok(await _manager.CreatePasswordHashAsync(password));
    }

    [HttpPut("init")]
    public async Task<ActionResult> InitUser()
    {
        await _manager.InitDataAsync();
        return Ok();
    }

    [HttpPut("initMachines")]
    public async Task<ActionResult> InitMachines()
    {
        await _manager.InitMachinesAsync();
        return Ok();
    }

    [HttpPut("initItems")]
    public async Task<ActionResult> InitItems()
    {
        await _manager.InitItemsAsync();
        return Ok();
    }

    [HttpDelete("cleanup")]
    public async Task<ActionResult> Cleanup()
    {
        await _manager.CleanupAsync();
        return Ok();
    }

    [HttpDelete("leave")]
    public async Task<ActionResult> Leave(string userName)
    {
        if (string.IsNullOrEmpty(userName) || _userContext.UserName == userName)
        {
            await _manager.LeaveAsync();
            return Ok();
        }

        if (!_userContext.IsAdmin)
        {
            return Forbid();
        }

        await _manager.LeaveAsync(userName);
        return Ok();
    }

    #region default REST

    [Authorize(Policy = Policies.IsAdmin)]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<User>> Get(int id)
    {
        return await this.Get<User, int>(_manager, id);
    }

/*
        [HttpPost]
        public async Task<ActionResult<User>> AddAsync([FromBody] User value)
        {
            return await this.AddAsync<User, int>(_manager, value);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateAsync(int id, [FromBody] User value)
        {
            return await this.UpdateAsync<User, int>(_manager, id, value.UserId, value);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            return await this.DeleteAsync<User, int>(_manager, id);
        }
*/

    #endregion
}