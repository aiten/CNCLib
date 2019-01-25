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

namespace Framework.Repository
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    public abstract class GetRepository<TDbContext, TEntity, TKey> : QueryRepository<TDbContext, TEntity>
        where TDbContext : DbContext where TEntity : class
    {
        protected GetRepository(TDbContext dbContext)
            : base(dbContext)
        {
        }

        protected IQueryable<TEntity> QueryWithInclude => AddInclude(Query);

        protected IQueryable<TEntity> TrackingQueryWithInclude => AddInclude(TrackingQuery);

        protected IQueryable<TEntity> QueryWithOptional => AddOptionalWhere(Query);

        public async Task<IList<TEntity>> GetAll()
        {
            return await AddInclude(QueryWithOptional).ToListAsync();
        }

        public async Task<TEntity> Get(TKey key)
        {
            return await AddPrimaryWhere(QueryWithInclude, key).FirstOrDefaultAsync();
        }

        public async Task<IList<TEntity>> Get(IEnumerable<TKey> keys)
        {
            return await AddPrimaryWhereIn(QueryWithInclude, keys).ToListAsync();
        }

        public async Task<TEntity> GetTracking(TKey key)
        {
            return await AddPrimaryWhere(TrackingQueryWithInclude, key).FirstOrDefaultAsync();
        }

        public async Task<IList<TEntity>> GetTracking(IEnumerable<TKey> keys)
        {
            return await AddPrimaryWhereIn(TrackingQueryWithInclude, keys).ToListAsync();
        }

        protected virtual IQueryable<TEntity> AddOptionalWhere(IQueryable<TEntity> query) => query;

        protected abstract IQueryable<TEntity> AddInclude(IQueryable<TEntity> query);

        protected abstract IQueryable<TEntity> AddPrimaryWhere(IQueryable<TEntity> query, TKey key);

        protected abstract IQueryable<TEntity> AddPrimaryWhereIn(IQueryable<TEntity> query, IEnumerable<TKey> key);
    }
}