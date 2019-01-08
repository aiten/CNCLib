////////////////////////////////////////////////////////
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
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.EntityFrameworkCore;

    using EntityState = Abstraction.EntityState;

    public abstract class CRUDRepository<TDbContext, TEntity, TKey> : GetRepository<TDbContext, TEntity, TKey>
        where TDbContext : DbContext where TEntity : class
    {
        protected CRUDRepository(TDbContext dbContext)
            : base(dbContext)
        {
        }

        #region CRUD

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

        public void SetState(TEntity entity, EntityState state)
        {
            SetEntityState(entity, (Microsoft.EntityFrameworkCore.EntityState)state);
        }

        public void SetValue(TEntity entity, TEntity values)
        {
            AssignValues(entity, values);
            base.SetValue(entity, values);
        }

        public void SetValueGraph(TEntity trackingEntity, TEntity values)
        {
            AssignValuesGraph(trackingEntity, values);
        }

        protected virtual void AssignValues(TEntity trackingEntity, TEntity values)
        {
        }

        protected virtual void AssignValuesGraph(TEntity trackingEntity, TEntity values)
        {
            SetValue(trackingEntity, values);
        }

        #endregion

        public void Sync(ICollection<TEntity> inDb, ICollection<TEntity> toDb, Func<TEntity, TEntity, bool> predicate)
        {
            Sync<TEntity>(inDb, toDb, predicate);
        }
    }
}