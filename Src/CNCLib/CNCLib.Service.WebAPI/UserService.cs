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
using System.Net.Http;
using System.Threading.Tasks;

using CNCLib.Logic.Contract.DTO;
using CNCLib.Service.Contract;

namespace CNCLib.Service.WebAPI
{
    public class UserService : CRUDServiceBase<User, int>, IUserService
    {
        protected override string Api            => @"api/user";
        protected override int    GetKey(User u) => u.UserId;

        public Task<User> GetByName(string username)
        {
            throw new NotImplementedException();
            /*
            using (HttpClient client = CreateHttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(Api + "/" + username);
                if (response.IsSuccessStatusCode)
                {
                    User value = await response.Content.ReadAsAsync<User>();
                    return value;
                }
            }

            return null;
            */
        }

        #region IDisposable Support

        #endregion
    }
}