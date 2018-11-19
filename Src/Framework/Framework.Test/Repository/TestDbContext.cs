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

    using Framework.Contracts.Repository;

    using Microsoft.EntityFrameworkCore;

    public class TestDbContext<TDbContext, TIRepository> : IDisposable where TIRepository : IRepository where TDbContext : DbContext
    {
        public TDbContext DbContext  { get; private set; }
        public IUnitOfWork   UnitOfWork { get; private set; }
        public TIRepository  Repository { get; private set; }

        public TestDbContext(TDbContext dbContext, IUnitOfWork uow, TIRepository repository)
        {
            DbContext  = dbContext;
            UnitOfWork = uow;
            Repository = repository;
        }

        public void Dispose()
        {
            DbContext.Dispose();
            DbContext = null;
        }
    }
}