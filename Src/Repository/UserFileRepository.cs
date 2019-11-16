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
using Framework.Repository.Linq;

using Microsoft.EntityFrameworkCore;

namespace CNCLib.Repository
{
    public class UserFileRepository : CRUDRepository<CNCLibContext, UserFile, UserFileKey>, IUserFileRepository
    {
        #region ctr/default/overrides

        public UserFileRepository(CNCLibContext context) : base(context)
        {
        }

        protected override FilterBuilder<UserFile, UserFileKey> FilterBuilder =>
            new FilterBuilder<UserFile, UserFileKey>()
            {
                PrimaryWhere = (query, key) => query.Where(item => item.UserId == key.UserId && item.FileName == key.FileName),
                PrimaryWhereIn = (query, keys) =>
                {
                    var predicate = PredicateBuilder.New<UserFile>();

                    foreach (var key in keys)
                    {
                        predicate = predicate.Or(c => c.UserId == key.UserId && c.FileName == key.FileName);
                    }

                    return query.Where(predicate);
                }
            };

        protected override IQueryable<UserFile> AddInclude(IQueryable<UserFile> query)
        {
            return query;
        }

        #endregion

        #region extra Queries

        #endregion
    }
}