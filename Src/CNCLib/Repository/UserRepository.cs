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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CNCLib.Repository.Context;
using CNCLib.Repository.Contract;
using CNCLib.Repository.Contract.Entities;

using Framework.Repository;

using Microsoft.EntityFrameworkCore;

namespace CNCLib.Repository
{
    public class UserRepository : CRUDRepository<CNCLibContext, User, int>, IUserRepository
    {
        #region ctr/default/overrides

        public UserRepository(CNCLibContext context) : base(context)
        {
        }

        protected override FilterBuilder<User, int> FilterBuilder =>
            new FilterBuilder<User, int>()
            {
                PrimaryWhere   = (query, key) => query.Where(item => item.UserId == key),
                PrimaryWhereIn = (query, keys) => query.Where(item => keys.Contains(item.UserId))
            };

        protected override IQueryable<User> AddInclude(IQueryable<User> query)
        {
            return query;
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

                // search und update User Commands (add and delete)
            }
        }

        #endregion

        #region extra Queries

        public async Task<User> GetByName(string username)
        {
            return await AddInclude(Query).Where(u => u.UserName == username).FirstOrDefaultAsync();
        }

        #endregion
    }
}