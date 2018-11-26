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

namespace Framework.Test.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Framework.Dependency;
    using Framework.Repository;
    using Framework.Repository.Abstraction;

    using Microsoft.EntityFrameworkCore;

    public abstract class GetRepositoryTests<TDbContext, TEntity, TKey, TIRepository> : RepositoryBaseTests<TDbContext>
        where TEntity : class where TIRepository : IGetRepository<TEntity, TKey> where TDbContext : DbContext
    {
        protected abstract GetTestDbContext<TDbContext, TEntity, TKey, TIRepository> CreateTestDbContext();

        protected abstract TKey    GetEntityKey(TEntity  entity);
        protected abstract TEntity SetEntityKey(TEntity  entity,  TKey    key);
        protected abstract bool    CompareEntity(TEntity entity1, TEntity entity2);

        protected async Task<IEnumerable<TEntity>> GetAll()
        {
            using (var ctx = CreateTestDbContext())
            {
                IEnumerable<TEntity> entities = await ctx.Repository.GetAll();
                entities.Should().NotBeNull();
                return entities;
            }
        }

        protected async Task<TEntity> GetTrackingOK(TKey key)
        {
            using (var ctx = CreateTestDbContext())
            {
                TEntity entity = await ctx.Repository.GetTracking(key);
                entity.Should().NotBeNull();
                entity.Should().BeOfType(typeof(TEntity));
                return entity;
            }
        }

        protected async Task<TEntity> GetOK(TKey key)
        {
            using (var ctx = CreateTestDbContext())
            {
                TEntity entity = await ctx.Repository.Get(key);
                entity.Should().BeOfType(typeof(TEntity));
                entity.Should().NotBeNull();
                return entity;
            }
        }

        protected async Task GetNotExist(TKey key)
        {
            using (var ctx = CreateTestDbContext())
            {
                var entity = await ctx.Repository.Get(key);
                entity.Should().BeNull();
            }
        }
    }
}