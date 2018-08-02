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
    public class TestContext<TEntry, TKey, TIRepository> : IDisposable where TEntry : class  where TIRepository : ICUDRepository<TEntry, TKey>
    {
        public DbContext DbContext { get; private set; }
        public IUnitOfWork UnitOfWork { get; private set; }
        public TIRepository Repository { get; private set; }

        public TestContext(DbContext dbContext, IUnitOfWork uow, TIRepository repository)
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
	public class CUDRepositoryTests<TEntry, TKey, TIRepository> : RepositoryTests where TEntry : class where TIRepository : ICUDRepository<TEntry, TKey>
    {
	    protected async Task<TEntry> GetById<TKey>(ICUDRepository<TEntry, TKey> repository, TKey key) 
	    {
	        var entity = await repository.Get(key);

	        entity.Should().BeOfType(typeof(TEntry));
	        return entity;
	    }

	    public async Task<TEntry> GetId(ICUDRepository<TEntry, TKey> repository, TKey key, Action<TEntry> addAsserts)
	    {
	        TEntry entity = await GetById(repository, key);
	        addAsserts(entity);
	        return entity;
	    }

	    public async Task GetIdNotExist(ICUDRepository<TEntry, TKey> repository, TKey key)
	    {
	        var entity = await repository.Get(key);
	        entity.Should().BeNull();
	    }
    }
}
