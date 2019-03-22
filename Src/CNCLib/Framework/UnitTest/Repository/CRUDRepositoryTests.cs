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
    using System.Linq;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Framework.Repository;
    using Framework.Repository.Abstraction;

    using Microsoft.EntityFrameworkCore;

    public class CRUDRepositoryTests<TDbContext, TEntity, TKey, TIRepository> : GetRepositoryTests<TDbContext, TEntity, TKey, TIRepository>
        where TEntity : class where TIRepository : ICRUDRepository<TEntity, TKey> where TDbContext : DbContext
    {
        public async Task AddUpdateDelete(Func<TEntity> createTestEntity, Action<TEntity> updateEntity)
        {
            var allWithoutAdd = await GetAll();
            allWithoutAdd.Should().NotBeNull();

            // first add entity

            TKey key;
            using (var ctx = CreateTestDbContext())
            using (var trans = ctx.UnitOfWork.BeginTransaction())
            {
                TEntity entityToAdd = createTestEntity();
                ctx.Repository.Add(entityToAdd);

                await ctx.UnitOfWork.SaveChangesAsync();
                await trans.CommitTransactionAsync();

                key = GetEntityKey(entityToAdd);
            }

            var allWithAdd = await GetAll();
            allWithAdd.Should().NotBeNull();
            allWithAdd.Count.Should().Be(allWithoutAdd.Count() + 1);

            // read again and update 

            using (var ctx = CreateTestDbContext())
            using (var trans = ctx.UnitOfWork.BeginTransaction())
            {
                TEntity entity = await ctx.Repository.GetTracking(key);
                GetEntityKey(entity).Should().Be(key);
                CompareEntity(createTestEntity(), entity).Should().BeTrue();
                updateEntity(entity);

                await ctx.UnitOfWork.SaveChangesAsync();
                await trans.CommitTransactionAsync();
            }

            // read again

            using (var ctx = CreateTestDbContext())
            using (var trans = ctx.UnitOfWork.BeginTransaction())
            {
                TEntity entity = await ctx.Repository.Get(key);
                GetEntityKey(entity).Should().Be(key);
            }

            // update (with method update)

            using (var ctx = CreateTestDbContext())
            using (var trans = ctx.UnitOfWork.BeginTransaction())
            {
                var entity = createTestEntity();
                SetEntityKey(entity, key);

                await ctx.Repository.Update(key, entity);

                await ctx.UnitOfWork.SaveChangesAsync();
                await trans.CommitTransactionAsync();
            }

            // read again and delete 

            using (var ctx = CreateTestDbContext())
            using (var trans = ctx.UnitOfWork.BeginTransaction())
            {
                TEntity entity = await ctx.Repository.GetTracking(key);
                GetEntityKey(entity).Should().Be(key);

                var compareEntity = createTestEntity();
                CompareEntity(compareEntity, entity).Should().BeTrue();

                ctx.Repository.Delete(entity);

                await ctx.UnitOfWork.SaveChangesAsync();
                await trans.CommitTransactionAsync();
            }

            // read again to test is not exist

            using (var ctx = CreateTestDbContext())
            {
                TEntity entity = await ctx.Repository.GetTracking(key);
                entity.Should().BeNull();
            }
        }

        public async Task AddUpdateDeleteBulk(Func<IEnumerable<TEntity>> createTestEntities, Action<IEnumerable<TEntity>> updateEntities)
        {
            var allWithoutAdd = await GetAll();
            allWithoutAdd.Should().NotBeNull();

            // first add entity

            IEnumerable<TKey> keys;
            using (var ctx = CreateTestDbContext())
            using (var trans = ctx.UnitOfWork.BeginTransaction())
            {
                IEnumerable<TEntity> entitiesToAdd = createTestEntities();
                ctx.Repository.AddRange(entitiesToAdd);

                await ctx.UnitOfWork.SaveChangesAsync();
                await trans.CommitTransactionAsync();

                keys = entitiesToAdd.Select(GetEntityKey);
            }

            var allWithAdd = await GetAll();
            allWithAdd.Should().NotBeNull();
            allWithAdd.Count().Should().Be(allWithoutAdd.Count() + keys.Count());

            // read again and update 
            using (var ctx = CreateTestDbContext())
            using (var trans = ctx.UnitOfWork.BeginTransaction())
            {
                IEnumerable<TEntity> entities        = await ctx.Repository.GetTracking(keys);
                IEnumerable<TEntity> compareEntities = createTestEntities();
                for (int i = 0; i < compareEntities.Count(); i++)
                {
                    GetEntityKey(entities.ElementAt(i)).Should().Be(keys.ElementAt(i));
                    CompareEntity(compareEntities.ElementAt(i), entities.ElementAt(i)).Should().BeTrue();
                }

                updateEntities(entities);

                await ctx.UnitOfWork.SaveChangesAsync();
                await trans.CommitTransactionAsync();
            }

            // read again
            using (var ctx = CreateTestDbContext())
            using (var trans = ctx.UnitOfWork.BeginTransaction())
            {
                IEnumerable<TEntity> entities = await ctx.Repository.Get(keys);
                for (int i = 0; i < entities.Count(); i++)
                {
                    GetEntityKey(entities.ElementAt(i)).Should().Be(keys.ElementAt(i));
                }
            }

            // read again and delete 

            using (var ctx = CreateTestDbContext())
            using (var trans = ctx.UnitOfWork.BeginTransaction())
            {
                IEnumerable<TEntity> entities = await ctx.Repository.GetTracking(keys);

                var compareEntities = createTestEntities();
                updateEntities(compareEntities);

                for (int i = 0; i < compareEntities.Count(); i++)
                {
                    GetEntityKey(entities.ElementAt(i)).Should().Be(keys.ElementAt(i));
                    CompareEntity(compareEntities.ElementAt(i), entities.ElementAt(i)).Should().BeTrue();
                }

                ctx.Repository.DeleteRange(entities);

                await ctx.UnitOfWork.SaveChangesAsync();
                await trans.CommitTransactionAsync();
            }

            // read again to test if not exist

            using (var ctx = CreateTestDbContext())
            {
                IEnumerable<TEntity> entities = await ctx.Repository.GetTracking(keys);
                entities.Count().Should().Be(0);
            }
        }

        public async Task AddRollBack(Func<TEntity> createTestEntity)
        {
            // first add entity

            TKey key;
            using (var ctx = CreateTestDbContext())
            using (var trans = ctx.UnitOfWork.BeginTransaction())
            {
                TEntity entityToAdd = createTestEntity();
                ctx.Repository.Add(entityToAdd);

                await ctx.UnitOfWork.SaveChangesAsync();

                // await trans.CommitTransactionAsync(); => no commit => Rollback

                key = GetEntityKey(entityToAdd);
            }

            // read again to test is not exist

            using (var ctx = CreateTestDbContext())
            using (var trans = ctx.UnitOfWork.BeginTransaction())
            {
                TEntity entity = await ctx.Repository.GetTracking(key);
                entity.Should().BeNull();
            }
        }

        public async Task Store(Func<TEntity> createTestEntity, Action<TEntity> updateEntity)
        {
            // test if entry not exist in DB

            TKey key = GetEntityKey(createTestEntity());

            using (var ctx = CreateTestDbContext())
            using (var trans = ctx.UnitOfWork.BeginTransaction())
            {
                TEntity entityToTest = createTestEntity();
                var     notFound     = await ctx.Repository.Get(key);
                notFound.Should().BeNull();
            }

            // first add entity
            // only useful if key is no identity

            using (var ctx = CreateTestDbContext())
            using (var trans = ctx.UnitOfWork.BeginTransaction())
            {
                TEntity entityToAdd = createTestEntity();
                await ctx.Repository.Store(entityToAdd, key);

                await ctx.UnitOfWork.SaveChangesAsync();
                await trans.CommitTransactionAsync();
            }

            // Read and Update Entity
            // only useful if key is no identity

            using (var ctx = CreateTestDbContext())
            using (var trans = ctx.UnitOfWork.BeginTransaction())
            {
                var entityInDb = await ctx.Repository.Get(key);
                entityInDb.Should().NotBeNull();
                CompareEntity(createTestEntity(), entityInDb).Should().BeTrue();
            }

            // modify existing

            using (var ctx = CreateTestDbContext())
            using (var trans = ctx.UnitOfWork.BeginTransaction())
            {
                TEntity entityToUpdate = createTestEntity();
                updateEntity(entityToUpdate);

                await ctx.Repository.Store(entityToUpdate, key);

                await ctx.UnitOfWork.SaveChangesAsync();
                await trans.CommitTransactionAsync();
            }

            // read again (modified)

            using (var ctx = CreateTestDbContext())
            using (var trans = ctx.UnitOfWork.BeginTransaction())
            {
                var entityInDb = await ctx.Repository.Get(key);
                entityInDb.Should().NotBeNull();

                TEntity entityToCompare = createTestEntity();
                updateEntity(entityToCompare);

                CompareEntity(entityToCompare, entityInDb).Should().BeTrue();

                ctx.Repository.Delete(entityInDb);

                await ctx.UnitOfWork.SaveChangesAsync();
                await trans.CommitTransactionAsync();
            }
        }
    }
}