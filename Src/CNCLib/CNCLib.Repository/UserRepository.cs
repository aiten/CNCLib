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

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using CNCLib.Repository.Context;
using CNCLib.Repository.Contracts;
using Framework.Tools.Pattern;

namespace CNCLib.Repository
{
    public class UserRepository : CNCLibRepository<Contracts.Entities.User>, IUserRepository
	{
        public UserRepository(CNCLibContext context) : base(context)
        {
        }

        #region CUD

        public async Task<IEnumerable<Contracts.Entities.User>> GetAll()
		{
            return await Query.
                ToListAsync();
		}

		public async Task<Contracts.Entities.User> Get(int id)
        {
			return await Query.
				Where(m => m.UserID == id).
				FirstOrDefaultAsync();
        }

	    public async Task<Contracts.Entities.User> GetTracking(int id)
	    {
	        return await TrackingQuery.
	            Where(m => m.UserID == id).
	            FirstOrDefaultAsync();
	    }

        public void Add(Contracts.Entities.User user)
	    {
	        AddEntity(user);
	    }

        public void Delete(Contracts.Entities.User user)
        {
			DeleteEntity(user);
		}

        #endregion

	    public async Task<Contracts.Entities.User> GetUser(string username)
	    {
	        return await Query.
	            Where(m => m.UserName == username).
	            FirstOrDefaultAsync();
	    }

        public async Task Store(Contracts.Entities.User user)
		{
			// search und update User

			int id = user.UserID;

			var UserInDb = await Context.Users.
				Where(m => m.UserID == id).
				FirstOrDefaultAsync();

			if (UserInDb == default(Contracts.Entities.User))
			{
				// add new

				AddEntity(user);
			}
			else
			{
				// syn with existing

				SetValue(UserInDb,user);

				// search und update Usercommands (add and delete)
			}
		}
	}
}
