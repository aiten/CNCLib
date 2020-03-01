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
using System.Net.Http;
using System.Threading.Tasks;

using CNCLib.Logic.Abstraction.DTO;
using CNCLib.Service.Abstraction;

using Framework.Pattern;
using Framework.Service.WebAPI;
using Framework.Service.WebAPI.Uri;

namespace CNCLib.Service.WebAPI
{
    public class UserService : CrudServiceBase<User, int>, IUserService
    {
        private HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            BaseApi = @"api/user";
            _httpClient = httpClient;
        }
        protected override IScope<HttpClient> CreateScope()
        {
            return new ScopeInstance<HttpClient>(_httpClient);
        }

        protected override int GetKey(User u) => u.UserId;

        public async Task<User> GetByName(string username)
        {
            return (await ReadList<User>(
                    CreatePathBuilder()
                        .AddQuery(new UriQueryBuilder().Add("username", username))))
                .FirstOrDefault();
        }

        public async Task<bool> IsValidUser(string username, string password)
        {
            using (var scope = CreateScope())
            {
                var builder = CreatePathBuilder()
                    .AddPath("isValidUser")
                    .AddQuery(new UriQueryBuilder()
                        .Add("username", username)
                        .Add("password", password));

                var response = await scope.Instance.GetAsync(builder.Build());
                return response.IsSuccessStatusCode;
            }
        }
    }
}