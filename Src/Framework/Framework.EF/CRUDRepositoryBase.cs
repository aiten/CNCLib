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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Framework.Tools.Pattern;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Framework.EF
{
    public abstract class CRUDRepositoryBase<TDbContext, TEntity, TId> : RepositoryBase<TDbContext, TEntity>
        where TDbContext : DbContext
        where TEntity : class
    {
        protected CRUDRepositoryBase(TDbContext dbContext) : base(dbContext)
        {
        }

        #region CRUD

        protected Func<TEntity, TId, bool> IsPrimary { get; set; }
        protected Func<IQueryable<TEntity>, IQueryable<TEntity>> AddInclude { get; set; } = m => m;

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            return await Query.ToListAsync();
        }

        public async Task<TEntity> Get(TId id)
        {
            return await Get(id, IsPrimary);
        }

        public async Task<TEntity> Get<TPk>(TPk key, Func<TEntity, TPk, bool> isKey)
        {
            return await AddInclude(Query).
                Where(m => isKey(m,key)).
                FirstOrDefaultAsync();
        }

        public async Task<TEntity> GetTracking(TId id)
        {
            return await GetTracking(id, IsPrimary);
        }

        public async Task<TEntity> GetTracking<TPk>(TPk key, Func<TEntity, TPk, bool> isKey)
        {
            return await AddInclude(TrackingQuery).
                Where(m => isKey(m, key)).
                FirstOrDefaultAsync();
        }

        public void Add(TEntity entity)
        {
            AddEntity(entity);
        }

        public void Delete(TEntity entity)
        {
            DeleteEntity(entity);
        }

        #endregion


    }
}
