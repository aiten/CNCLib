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
using System.Linq;
using System.Threading.Tasks;

using CNCLib.Repository.Context;
using CNCLib.Repository.Contracts;
using CNCLib.Repository.Contracts.Entities;

using Framework.Repository;

using Microsoft.EntityFrameworkCore;

namespace CNCLib.Repository
{
    public class UserRepository : CRUDRepositoryBase<CNCLibContext, User, int>, IUserRepository
    {
        public UserRepository(CNCLibContext context) : base(context)
        {
        }

        protected override IQueryable<User> AddInclude(IQueryable<User> query)
        {
            return query;
        }

        protected override IQueryable<User> AddPrimaryWhere(IQueryable<User> query, int key)
        {
            return query.Where(m => m.UserId == key);
        }

        protected override IQueryable<User> AddPrimaryWhereIn(IQueryable<User> query, IEnumerable<int> key)
        {
            return query.Where(m => key.Contains(m.UserId));
        }

        public async Task<User> GetByName(string username)
        {
            return await AddInclude(Query).Where(u => u.UserName == username).FirstOrDefaultAsync();
        }

        public async Task Store(User user)
        {
            // search und update User

            int id = user.UserId;

            var userInDb = await Context.Users.Where(m => m.UserId == id).FirstOrDefaultAsync();

            if (userInDb == default(User))
            {
                // add new

                AddEntity(user);
            }
            else
            {
                // syn with existing

                SetValue(userInDb, user);

                // search und update Usercommands (add and delete)
            }
        }
    }
}