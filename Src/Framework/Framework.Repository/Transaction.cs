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
using System.Threading.Tasks;
using Framework.Contracts.Repository;
using Microsoft.EntityFrameworkCore.Storage;

namespace Framework.Repository
{
    public class Transaction : ITransaction
    {
        public IUnitOfWork UnitOfWork { get; private set; }

        public Transaction(IUnitOfWork unitOfWork, IDbContextTransaction dbTran)
        {
            UnitOfWork = unitOfWork;
            _dbTran = dbTran;
        }

        #region Transaction

        private IDbContextTransaction _dbTran;

        public bool InTransaction
        {
            get { return _dbTran != null; }
        }

        public async Task CommitTransactionAsync()
        {
            if (InTransaction == false) throw new ArgumentException();
            await UnitOfWork.SaveChangesAsync();
            _dbTran.Commit();
            _dbTran = null;
        }

        public void RollbackTransaction()
        {
            if (InTransaction == false) throw new ArgumentException();
            _dbTran.Rollback();
            _dbTran = null;
        }

        #endregion

        #region dispose

        public void Dispose()
        {
            if (InTransaction)
            {
                // if commit is not called, rollback transaction now
                RollbackTransaction();
            }
        }

        #endregion
    }
}
