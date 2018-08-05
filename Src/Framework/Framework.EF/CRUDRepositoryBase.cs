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
    public abstract class CRUDRepositoryBase<TDbContext, TEntity, TKey> : RepositoryBase<TDbContext, TEntity>
        where TDbContext : DbContext
        where TEntity : class
    {
        protected CRUDRepositoryBase(TDbContext dbContext) : base(dbContext)
        {
        }

        #region CRUD

        protected abstract IQueryable<TEntity> AddInclude(IQueryable<TEntity> query);
        protected abstract IQueryable<TEntity> AddPrimaryWhere(IQueryable<TEntity> query, TKey key);

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            return await Query.ToListAsync();
        }

        public async Task<TEntity> Get(TKey key)
        {
            return await AddPrimaryWhere(AddInclude(Query),key).FirstOrDefaultAsync();
        }

        public async Task<TEntity> GetTracking(TKey key)
        {
            return await AddPrimaryWhere(AddInclude(TrackingQuery), key).FirstOrDefaultAsync();
        }

        public async Task Update(TKey key, TEntity values)
        {
            var entityInDB = await GetTracking(key);
            if (entityInDB == default(TEntity))
            {
                throw new DBConcurrencyException();
            }

            await UpdateGraph(entityInDB, values);
        }

        protected virtual async Task UpdateGraph(TEntity trackingentity, TEntity values)
        {
            SetValue(trackingentity, values);
        }

        public void SetState(TEntity entity, Framework.Contracts.Repository.EntityState state)
        {
            SetEntityState(entity,(EntityState) state);
        }

        public void Add(TEntity entity)
        {
            AddEntity(entity);
        }

        public void Delete(TEntity entity)
        {
            DeleteEntity(entity);
        }

        public void SetValue(TEntity entity, object values)
        {
            base.SetValue(entity,values);
        }

        #endregion
    }
}
