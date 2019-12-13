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

using System;
using System.Collections.Generic;
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
    public class UserFileRepository : CRUDRepository<CNCLibContext, UserFile, int>, IUserFileRepository
    {
        #region ctr/default/overrides

        public UserFileRepository(CNCLibContext context) : base(context)
        {
        }

        protected override FilterBuilder<UserFile, int> FilterBuilder =>
            new FilterBuilder<UserFile, int>()
            {
                PrimaryWhere = (query, key) => query.Where(item => item.UserFileId == key),
                PrimaryWhereIn = (query, keys) => query.Where(item => keys.Contains(item.UserFileId))
            };

        protected override IQueryable<UserFile> AddInclude(IQueryable<UserFile> query)
        {
            return query;
        }

        #endregion

        #region extra Queries

        public async Task<IList<UserFile>> GetByUser(int userId)
        {
            return await AddOptionalWhere(Query).Where(m => m.UserId == userId).ToListAsync();
        }

        public async Task<IList<string>> GetFileNames(int userId)
        {
            return await QueryWithOptional.Where(f => f.UserId == userId).Select(f => f.FileName).ToListAsync();
        }

        public async Task<int> GetFileId(int userId, string fileName)
        {
            return await QueryWithOptional.Where(f => f.UserId == userId && f.FileName == fileName).Select(f => f.UserFileId).FirstOrDefaultAsync();
        } 

        public async Task<UserFile> GetByName(int userId, string fileName)
        {
            return await QueryWithOptional.FirstOrDefaultAsync(f => f.UserId == userId && f.FileName == fileName);
        }

        #endregion
    }
}