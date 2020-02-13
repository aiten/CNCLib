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

using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Framework.Logic.Abstraction;
using Framework.Pattern;
using Framework.Tools;
using Microsoft.Extensions.Configuration;

namespace CNCLib.Serial.WebAPI.Manager
{
    public class UserManager : IAuthenticationManager
    {
        private IConfiguration _configuration;

        public UserManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static string DecodePassword(string encodedPassword)
        {
            return Base64Helper.StringFromBase64(encodedPassword);
        }

        public static string EncodePassword(string password)
        {
            return Base64Helper.StringToBase64(password);
        }

        public async Task<ClaimsPrincipal> Authenticate(string userName, string password)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                var users = _configuration.GetSection("Users");

                if (users != null)
                {
                    var passwordEncrypded = users.GetValue<string>(userName);
                    if (passwordEncrypded != null)
                    {
                        var passwordInConfig = DecodePassword(passwordEncrypded);

                        // password is stored with username as prefix
                        if (ComparePassword(passwordInConfig, $"{userName}:{password}"))
                        {
                            var claims = new[]
                            {
                                new Claim(ClaimTypes.NameIdentifier, 1.ToString()),
                                new Claim(ClaimTypes.Name,           userName),
                            };
                            var identity = new ClaimsIdentity(claims, "BasicAuthentication");
                            var principal = new ClaimsPrincipal(identity);

                            return principal;
                        }
                    }
                }
            }

            return await Task.FromResult<ClaimsPrincipal>(null);
        }

        private bool ComparePassword(string pwd1, string pwd2)
        {
            return pwd1 == pwd2 || (string.IsNullOrEmpty(pwd1) && string.IsNullOrEmpty(pwd2));
        }
    }
}