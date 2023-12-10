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

namespace CNCLib.Service.Logic;

using System.Threading.Tasks;

using CNCLib.Logic.Abstraction;
using CNCLib.Logic.Abstraction.DTO;
using CNCLib.Service.Abstraction;
using CNCLib.Shared;

using Framework.Service.Logic;

public class UserService : CrudService<User, int>, IUserService
{
    readonly         IUserManager       _manager;
    private readonly ICNCLibUserContext _userContext;

    public UserService(IUserManager manager, ICNCLibUserContext userContext) : base(manager)
    {
        _manager     = manager;
        _userContext = userContext;
    }

    public async Task<User?> GetByName(string username)
    {
        return await _manager.GetByNameAsync(username);
    }

    public async Task<User?> GetCurrentUser()
    {
        return await _manager.GetAsync(_userContext.UserId);
    }

    public async Task<bool> IsValidUser(string username, string password)
    {
        var claimsPrincipal = await _manager.AuthenticateAsync(username, password);
        return claimsPrincipal != null;
    }

    public async Task<string> Register(string username, string password)
    {
        var id = await _manager.RegisterAsync(username, password);
        return id;
    }

    public async Task Leave()
    {
        await _manager.LeaveAsync();
    }

    public async Task InitData()
    {
        await _manager.InitDataAsync();
    }

    public async Task Cleanup()
    {
        await _manager.CleanupAsync();
    }
}