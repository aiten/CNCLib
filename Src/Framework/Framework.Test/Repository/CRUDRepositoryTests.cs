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
using System.Threading.Tasks;

using FluentAssertions;

using Framework.Contracts.Repository;
using Framework.Repository;

using Microsoft.EntityFrameworkCore;

namespace Framework.Test.Repository
{
    public abstract class CRUDRepositoryTests<TDbContext, TEntity, TKey, TIRepository> : GetRepositoryTests<TDbContext, TEntity, TKey, TIRepository> where TEntity : class where TIRepository : ICRUDRepository<TEntity, TKey> where TDbContext : DbContext
    {
        public async Task AddUpdateDelete(Func<TEntity> createTestEntity, Action<TEntity> updateEntity)
        {
            var allWithoutAdd = (await GetAll()).ToList();
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

            var allWithAdd = (await GetAll()).ToList();
            allWithAdd.Should().NotBeNull();
            allWithAdd.Count().Should().Be(allWithoutAdd.Count() + 1);

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

            // update (with methode update)
            using (var ctx = CreateTestDbContext())
            using (var trans = ctx.UnitOfWork.BeginTransaction())
            {
                await ctx.Repository.Update(key, SetEntityKey(createTestEntity(), key));

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
            var allWithoutAdd = (await GetAll()).ToList();
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

            var allWithAdd = (await GetAll()).ToList();
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

            // read again to test is not exist

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