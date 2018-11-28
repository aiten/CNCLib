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

namespace Framework.Repository
{
    using System;
    using System.Threading.Tasks;

    using Abstraction;

    using Microsoft.EntityFrameworkCore;

    public class UnitOfWork<T> : IUnitOfWork
        where T : DbContext
    {
        public T Context { get; private set; }

        public UnitOfWork(T context)
        {
            Context = context;
        }

        public int SaveChanges()
        {
            return Context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await Context.SaveChangesAsync();
        }

        public async Task<int> ExecuteSqlCommand(string sql)
        {
            return await Context.Database.ExecuteSqlCommandAsync(sql);
        }

        public async Task<int> ExecuteSqlCommand(string sql, params object[] parameters)
        {
            return await Context.Database.ExecuteSqlCommandAsync(sql, parameters);
        }

        #region Transaction

        public bool IsInTransaction { get; private set; }

        public ITransaction BeginTransaction()
        {
            if (IsInTransaction)
            {
                throw new ArgumentException();
            }

            IsInTransaction = true;
            return new Transaction(this, Context.Database.BeginTransaction());
        }

        public void FinishTransaction(ITransaction trans)
        {
            IsInTransaction = false;
        }

        #endregion
    }
}