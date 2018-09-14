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
using Microsoft.EntityFrameworkCore;

namespace Framework.Repository
{
    public abstract class RepositoryBase<TDbContext> where TDbContext : DbContext
    {
        protected RepositoryBase(TDbContext dbContext)
        {
            Context = dbContext;
        }

        /// <summary>
        /// The DbContext. Should be used rarely, instead use <see cref="Query{T}"/> and <see cref="TrackingQuery{T}"/>.
        /// </summary>
        protected TDbContext Context { get; private set; }

        /// <summary>
        /// Returns an IQueryable of the Entity with AsNoTracking set. This should be the default.
        /// </summary>
        /// <typeparam name="T">Entity for which to return the IQueryable.</typeparam>
        /// <returns>Queryable with AsNoTracking() set</returns>
        protected IQueryable<T> Query<T>() where T : class
        {
            return Context.Set<T>().AsNoTracking();
        }

        /// <summary>
        /// Returns an IQueryable that has tracking enabled.
        /// </summary>
        /// <typeparam name="T">Entity for which to return the IQueryable</typeparam>
        /// <returns>Queryable with tracking enabled</returns>
        protected IQueryable<T> TrackingQuery<T>() where T : class
        {
            return Context.Set<T>();
        }

        protected void SetEntityState<TEntity>(TEntity entity, EntityState state) where TEntity : class
        {
            Context.Entry(entity).State = state;
        }

        protected void SetValue<TEntity>(TEntity entity, object values) where TEntity : class
        {
            Context.Entry(entity).CurrentValues.SetValues(values);
        }

        protected void SetModified<TEntity>(TEntity entity) where TEntity : class
        {
            SetEntityState(entity, EntityState.Modified);
        }

        protected void AddEntity<TEntity>(TEntity entity) where TEntity : class
        {
            Context.Add(entity);
        }

        protected void AddEntities<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            Context.AddRange(entities);
        }

        protected void DeleteEntity<TEntity>(TEntity entity) where TEntity : class
        {
            SetEntityState(entity, EntityState.Deleted);
        }

        protected void DeleteEntities<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            foreach (var entity in entities)
            {
                SetEntityState(entity, EntityState.Deleted);
            }
        }

        public void Sync<TEntity>(ICollection<TEntity> inDb, ICollection<TEntity> toDb, Func<TEntity, TEntity, bool> predicate) where TEntity : class
        {
            // 1. DeleteEntity from DB (in DB) and update
            var delete = new List<TEntity>();

            foreach (TEntity entityInDb in inDb)
            {
                TEntity entityToDb = toDb.FirstOrDefault(x => predicate(x, entityInDb));
                if (entityToDb != null && predicate(entityToDb, entityInDb))
                {
                    SetValue(entityInDb, entityToDb);
                }
                else
                {
                    delete.Add(entityInDb);
                }
            }

            foreach (var del in delete)
            {
                DeleteEntity(del);
            }

            // 2. AddEntity To DB

            foreach (TEntity entityToDb in toDb)
            {
                var entityInDb = inDb.FirstOrDefault(x => predicate(x, entityToDb));
                if (entityInDb == null || predicate(entityToDb, entityInDb) == false)
                {
                    AddEntity(entityToDb);
                }
            }
        }
    }
}