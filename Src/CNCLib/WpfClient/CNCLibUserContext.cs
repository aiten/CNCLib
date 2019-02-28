/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2019 Herbert Aitenbichler

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
using System.Threading.Tasks;

using CNCLib.Logic.Contract.DTO;
using CNCLib.Service.Contract;
using CNCLib.Shared;

using Framework.Dependency;

namespace CNCLib.WpfClient
{
    public class CNCLibUserContext : ICNCLibUserContextRW
    {
        public CNCLibUserContext()
        {
            //string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            UserName = Environment.UserName;
        }

        public string UserName { get; private set; }

        public int? UserId { get; private set; }

        public async Task InitUserContext()
        {
            try
            {
                using (var userService = Dependency.Resolve<IUserService>())
                {
                    var user = await userService.GetByName(UserName);
                    if (user == null)
                    {
                        user = new User()
                        {
                            UserName = this.UserName
                        };
                        user.UserId = await userService.Add(user);
                    }

                    UserId = user.UserId;
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