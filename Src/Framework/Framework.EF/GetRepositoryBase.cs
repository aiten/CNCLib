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
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Framework.Tools.Pattern;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Unity.Policy.Mapping;

namespace Framework.EF
{
    public abstract class GetRepositoryBase<TDbContext, TEntity, TKey> : QueryRepositoryBase<TDbContext, TEntity>
        where TDbContext : DbContext
        where TEntity : class
    {
        protected GetRepositoryBase(TDbContext dbContext) : base(dbContext)
        {
        }

        protected abstract IQueryable<TEntity> AddPrimaryWhere(IQueryable<TEntity> query, TKey key);
        protected abstract IQueryable<TEntity> AddPrimaryWhereIn(IQueryable<TEntity> query, IEnumerable<TKey> key);

        protected virtual IQueryable<TEntity> AddInclude(IQueryable<TEntity> query)
        {
            return query;
        }

        protected virtual IQueryable<TEntity> AddOptionalWhere(IQueryable<TEntity> query)
        {
            return query;
        }

        protected IQueryable<TEntity> QueryWithOptional => AddOptionalWhere(Query<TEntity>());

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            return await QueryWithOptional.ToListAsync();
        }

        public async Task<TEntity> Get(TKey key)
        {
            return await AddPrimaryWhere(AddInclude(Query),key).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TEntity>> Get(IEnumerable<TKey> keys)
        {
            return await AddPrimaryWhereIn(AddInclude(Query), keys).ToListAsync();
        }

        public async Task<TEntity> GetTracking(TKey key)
        {
            return await AddPrimaryWhere(AddInclude(TrackingQuery), key).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TEntity>> GetTracking(IEnumerable<TKey> keys)
        {
            return await AddPrimaryWhereIn(AddInclude(TrackingQuery), keys).ToListAsync();
        }
    }
}
