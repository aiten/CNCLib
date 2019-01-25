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
    using System.Linq;

    using Microsoft.EntityFrameworkCore;

    public abstract class QueryRepository<TDbContext, TEntity> : RepositoryBase<TDbContext>
        where TDbContext : DbContext where TEntity : class
    {
        protected QueryRepository(TDbContext dbContext)
            : base(dbContext)
        {
        }

        /// <summary>
        /// Gets an IQueryable of the default entity of the DA with AsNoTracking set. This should be the default.
        /// </summary>
        /// <returns>Queryable with AsNoTracking() set</returns>
        protected IQueryable<TEntity> Query => Query<TEntity>();

        /// <summary>
        /// Gets an IQueryable of the default entity of the DA that has tracking enabled.
        /// </summary>
        /// <returns>Queryable with tracking enabled</returns>
        protected IQueryable<TEntity> TrackingQuery => TrackingQuery<TEntity>();
    }
}