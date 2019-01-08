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

namespace Framework.Test.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Framework.Repository.Abstraction;

    using Microsoft.EntityFrameworkCore;

    public class GetRepositoryTests<TDbContext, TEntity, TKey, TIRepository> : UnitTestBase
        where TEntity : class where TIRepository : IGetRepository<TEntity, TKey> where TDbContext : DbContext
    {
        public Func<GetTestDbContext<TDbContext, TEntity, TKey, TIRepository>> CreateTestDbContext;

        public Func<TEntity, TKey>   GetEntityKey;
        public Action<TEntity, TKey> SetEntityKey;

        public Func<TEntity, TEntity, bool> CompareEntity;

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            using (var ctx = CreateTestDbContext())
            {
                IEnumerable<TEntity> entities = await ctx.Repository.GetAll();
                entities.Should().NotBeNull();
                return entities;
            }
        }

        public async Task<TEntity> GetTrackingOK(TKey key)
        {
            using (var ctx = CreateTestDbContext())
            {
                TEntity entity = await ctx.Repository.GetTracking(key);
                entity.Should().NotBeNull();
                entity.Should().BeOfType(typeof(TEntity));
                return entity;
            }
        }

        public async Task<TEntity> GetOK(TKey key)
        {
            using (var ctx = CreateTestDbContext())
            {
                TEntity entity = await ctx.Repository.Get(key);
                entity.Should().BeOfType(typeof(TEntity));
                entity.Should().NotBeNull();
                return entity;
            }
        }

        public async Task GetNotExist(TKey key)
        {
            using (var ctx = CreateTestDbContext())
            {
                var entity = await ctx.Repository.Get(key);
                entity.Should().BeNull();
            }
        }
    }
}