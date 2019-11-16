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
using System.Threading.Tasks;

using CNCLib.Repository.Abstraction;
using CNCLib.Repository.Abstraction.Entities;
using CNCLib.Repository.Context;

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

            var userInDb = await Query.Where(m => m.UserId == id).FirstOrDefaultAsync();

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
            return await AddInclude(Query).Where(u => u.Name == username).FirstOrDefaultAsync();
        }

        #endregion
    }
}