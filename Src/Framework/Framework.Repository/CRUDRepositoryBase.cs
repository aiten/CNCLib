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
using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Framework.Repository
{
    public abstract class CRUDRepositoryBase<TDbContext, TEntity, TKey> : GetRepositoryBase<TDbContext, TEntity, TKey>
        where TDbContext : DbContext
        where TEntity : class
    {
        protected CRUDRepositoryBase(TDbContext dbContext) : base(dbContext)
        {
        }

        #region CRUD

        protected virtual void AssignValuesGraph(TEntity trackingentity, TEntity values)
        {
            SetValue(trackingentity,values);
        }

        public void Add(TEntity entity)
        {
            AddEntity(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            AddEntities(entities);
        }

        public void Delete(TEntity entity)
        {
            DeleteEntity(entity);
        }

        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            DeleteEntities(entities);
        }

        public void SetState(TEntity entity, Framework.Contracts.Repository.EntityState state)
        {
            SetEntityState(entity, (EntityState)state);
        }

        public void SetValue(TEntity entity, TEntity values)
        {
            base.SetValue(entity,(object) values);
        }

        public async Task Update(TKey key, TEntity values)
        {
            var entityInDB = await GetTracking(key);
            if (entityInDB == default(TEntity))
            {
                throw new DBConcurrencyException();
            }

            SetValueGraph(entityInDB, values);
        }

        public void SetValueGraph(TEntity trackingentity, TEntity values)
        {
            AssignValuesGraph(trackingentity, values);
        }


        #endregion
    }
}
