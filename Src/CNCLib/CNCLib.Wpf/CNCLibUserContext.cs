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

using CNCLib.Logic.Contracts.DTO;
using CNCLib.Service.Contracts;
using CNCLib.Shared;
using Framework.Tools.Dependency;
using System;
using System.Threading.Tasks;

namespace CNCLib.Wpf
{
    public class CNCLibUserContext : ICNCLibUserContextRW
    {
        public CNCLibUserContext()
        {
            //string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            UserName = Environment.UserName;
        }

        public string UserName { get; private set; }

        public int? UserID { get; private set; }

        public async Task InitUserContext()
        {
            try
            {
                using (var userservice = Dependency.Resolve<IUserService>())
                {
                    var user = await userservice.GetByName(UserName);
                    if (user == null)
                    {
                        user = new User()
                        {
                            UserName = this.UserName
                        };
                        user.UserID = await userservice.Add(user);
                    }

                    UserID = user.UserID;
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