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