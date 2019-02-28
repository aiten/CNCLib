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

        #region QueryProperties

        protected IQueryable<TEntity> QueryWithInclude => AddInclude(Query);

        protected IQueryable<TEntity> TrackingQueryWithInclude => AddInclude(TrackingQuery);

        protected IQueryable<TEntity> QueryWithOptional => AddOptionalWhere(Query);

        protected IQueryable<TEntity> TrackingQueryWithOptional => AddOptionalWhere(TrackingQuery);

        protected virtual FilterBuilder<TEntity, TKey> FilterBuilder { get; } = null;

        #endregion

        #region Get

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

        #endregion

        #region overrides

        protected virtual IQueryable<TEntity> AddOptionalWhere(IQueryable<TEntity> query) => query;

        protected abstract IQueryable<TEntity> AddInclude(IQueryable<TEntity> query);

        protected virtual IQueryable<TEntity> AddPrimaryWhere(IQueryable<TEntity> query, TKey key)
        {
            return FilterBuilder.PrimaryWhere(query, key);
        }

        protected virtual IQueryable<TEntity> AddPrimaryWhereIn(IQueryable<TEntity> query, IEnumerable<TKey> keys)
        {
            return FilterBuilder.PrimaryWhereIn(query, keys);
        }

        #endregion
    }
}