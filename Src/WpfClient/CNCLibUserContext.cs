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

namespace CNCLib.WpfClient
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using CNCLib.Logic.Abstraction;
    using CNCLib.Logic.Abstraction.DTO;
    using CNCLib.Service.Abstraction;
    using CNCLib.Shared;

    using Framework.Dependency;
    using Framework.Tools;

    using Microsoft.Extensions.DependencyInjection;

    public class CNCLibUserContext : ICNCLibUserContextRW
    {
        public CNCLibUserContext(string userName = null)
        {
            if (userName == null)
            {
                userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            }

            UserId  = 1;
            User    = CreatePrincipal(userName, UserId);
            IsAdmin = true;

            UserName          = userName; // Environment.UserName;
            EncryptedPassword = Base64Helper.StringToBase64(UserName);
        }

        private ClaimsPrincipal CreatePrincipal(string userName, int userId)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name,           userName),
                new Claim(CNCLibClaims.IsAdmin,      "true"),
            };
            var identity = new ClaimsIdentity(claims, "BasicAuthentication");

            return new ClaimsPrincipal(identity);
        }

        public ClaimsPrincipal User { get; private set; }

        public string UserName { get; private set; }

        public string EncryptedPassword { get; private set; }

        public string Password => Base64Helper.StringFromBase64(EncryptedPassword);

        public int  UserId  { get; private set; }
        public bool IsAdmin { get; private set; }

        public async Task InitUserContext()
        {
            await InitUserContext(UserName);
        }

        public async Task InitUserContext(string userName)
        {
            try
            {
                UserName = userName;
                using (var scope = AppService.ServiceProvider.CreateScope())
                {
                    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                    var user        = await userService.GetByName(UserName);

                    if (user == null)
                    {
                        user        = new User();
                        user.Name   = UserName;
                        user.UserId = await userService.Add(user);
                    }

                    UserId            = user.UserId;
                    User              = CreatePrincipal(UserName, UserId);
                    EncryptedPassword = Base64Helper.StringToBase64(UserName);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }
    }
}