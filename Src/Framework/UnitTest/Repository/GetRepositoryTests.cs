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

namespace Framework.UnitTest.Repository
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

        public async Task<IList<TEntity>> GetAll()
        {
            using (var ctx = CreateTestDbContext())
            {
                var entities = await ctx.Repository.GetAll();
                entities.Should().NotBeNull();
                return entities;
            }
        }

        public async Task<TEntity> GetTrackingOK(TKey key)
        {
            using (var ctx = CreateTestDbContext())
            {
                var entity = await ctx.Repository.GetTracking(key);
                entity.Should().NotBeNull();
                entity.Should().BeOfType(typeof(TEntity));
                return entity;
            }
        }

        public async Task<TEntity> GetOK(TKey key)
        {
            using (var ctx = CreateTestDbContext())
            {
                var entity = await ctx.Repository.Get(key);
                entity.Should().BeOfType(typeof(TEntity));
                entity.Should().NotBeNull();
                return entity;
            }
        }

        public async Task<IList<TEntity>> GetOK(IEnumerable<TKey> keys)
        {
            using (var ctx = CreateTestDbContext())
            {
                var entities = await ctx.Repository.Get(keys);
                entities.Should().NotBeNull();
                return entities;
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