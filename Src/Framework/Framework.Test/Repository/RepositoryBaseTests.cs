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

using Framework.Contract.Repository;

namespace Framework.Test.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Framework.Dependency;
    using Framework.Repository;

    using Microsoft.EntityFrameworkCore;

    public abstract class RepositoryBaseTests<TDbContext> : UnitTestBase where TDbContext : DbContext
    {
        protected override void InitializeDependencies()
        {
            base.InitializeDependencies();

            Dependency.Container.RegisterTypeScoped<TDbContext, TDbContext>();
            Dependency.Container.RegisterTypeScoped<IUnitOfWork, UnitOfWork<TDbContext>>();

            Dependency.Container.RegisterType(typeof(TestDbContext<,>), typeof(TestDbContext<,>));
        }
    }
}