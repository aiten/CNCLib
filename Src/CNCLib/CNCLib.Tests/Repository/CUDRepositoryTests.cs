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
using CNCLib.Repository;
using CNCLib.Repository.Context;
using CNCLib.Repository.Contracts;
using CNCLib.Repository.Contracts.Entities;
using FluentAssertions;
using Framework.Contracts.Repository;
using Framework.EF;
using Framework.Tools.Dependency;
using Framework.Tools.Pattern;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CNCLib.Tests.Repository
{
    public class CUDTestContext<TEntry, TKey, TIRepository> : IDisposable where TEntry : class  where TIRepository : ICUDRepository<TEntry, TKey>
    {
        public CNCLibContext DbContext { get; private set; }
        public IUnitOfWork UnitOfWork { get; private set; }
        public TIRepository Repository { get; private set; }

        public CUDTestContext(CNCLibContext dbContext, IUnitOfWork uow, TIRepository repository)
        {
            DbContext = dbContext;
            UnitOfWork = uow;
            Repository = repository;
        }

        public void Dispose()
        {
            DbContext.Dispose();
            DbContext = null;
        }
    }

    [TestClass]
	public abstract class CUDRepositoryTests<TEntity, TKey, TIRepository> : RepositoryTests where TEntity : class where TIRepository : ICUDRepository<TEntity, TKey>
    {
        protected abstract CUDTestContext<TEntity, TKey, TIRepository> CreateCUDTestContext();
        protected abstract TKey GetEntityKey(TEntity entity);
        protected abstract bool CompareEntity(TEntity entity1, TEntity entity2);

        protected async Task<IEnumerable<TEntity>> GetAll()
        {
            using (var ctx = CreateCUDTestContext())
            {
                IEnumerable<TEntity> entities = await ctx.Repository.GetAll();
                entities.Should().NotBeNull();
                return entities;
            }
        }

        protected async Task<TEntity> GetTrackingOK(TKey key)
        {
            using (var ctx = CreateCUDTestContext())
            {
                TEntity entity = await ctx.Repository.GetTracking(key);
                entity.Should().NotBeNull();
                entity.Should().BeOfType(typeof(TEntity));
                return entity;
            }
        }

        protected async Task<TEntity> GetOK(TKey key)
        {
            using (var ctx = CreateCUDTestContext())
            {
                TEntity entity = await ctx.Repository.Get(key);
                entity.Should().BeOfType(typeof(TEntity));
                entity.Should().NotBeNull();
                return entity;
            }
        }

        protected async Task GetNotExist(TKey key)
        {
            using (var ctx = CreateCUDTestContext())
            {
                var entity = await ctx.Repository.Get(key);
                entity.Should().BeNull();
            }
        }

        public async Task AddUpdateDelete(Func<TEntity> createTestEntity, Action<TEntity> updateEntity)
        {
            var allWithoutAdd = await GetAll();
            allWithoutAdd.Should().NotBeNull();

            // first add entity

            TKey key;
            using (var ctx = CreateCUDTestContext())
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
            allWithAdd.Count().Should().Be(allWithoutAdd.Count() + 1);

            // read again and update 
            using (var ctx = CreateCUDTestContext())
            using (var trans = ctx.UnitOfWork.BeginTransaction())
            {
                TEntity entity = await ctx.Repository.GetTracking(key);
                GetEntityKey(entity).Should().Be(key);
                CompareEntity(entity, createTestEntity()).Should().BeTrue();
                updateEntity(entity);

                await ctx.UnitOfWork.SaveChangesAsync();
                await trans.CommitTransactionAsync();
            }

            // read again and delete 
            using (var ctx = CreateCUDTestContext())
            using (var trans = ctx.UnitOfWork.BeginTransaction())
            {
                TEntity entity = await ctx.Repository.GetTracking(key);
                GetEntityKey(entity).Should().Be(key);

                var compareEntity = createTestEntity();
                updateEntity(compareEntity);
                CompareEntity(entity, compareEntity).Should().BeTrue();

                ctx.Repository.Delete(entity);

                await ctx.UnitOfWork.SaveChangesAsync();
                await trans.CommitTransactionAsync();
            }

            // read again to test is not exist

            using (var ctx = CreateCUDTestContext())
            {
                TEntity entity = await ctx.Repository.GetTracking(key);
                entity.Should().BeNull();
            }
        }

        public async Task AddRollBack(Func<TEntity> createTestEntity)
        {
            // first add entity

            TKey key;
            using (var ctx = CreateCUDTestContext())
            using (var trans = ctx.UnitOfWork.BeginTransaction())
            {
                TEntity entityToAdd = createTestEntity();
                ctx.Repository.Add(entityToAdd);

                await ctx.UnitOfWork.SaveChangesAsync();
                // await trans.CommitTransactionAsync(); => no commit => Rollback

                key = GetEntityKey(entityToAdd);
            }

            // read again to test is not exist

            using (var ctx = CreateCUDTestContext())
            using (var trans = ctx.UnitOfWork.BeginTransaction())
            {
                TEntity entity = await ctx.Repository.GetTracking(key);
                entity.Should().BeNull();
            }
        }
    }
}
