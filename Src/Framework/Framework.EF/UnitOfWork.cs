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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Framework.Tools.Pattern;

namespace Framework.EF
{
    public class UnitOfWork<T> : IUnitOfWork where T : DbContext, new()
	{
		private T _context;

		public T Context
		{
			get
			{
				if (_context == null)
					_context = new T();
				return _context;
			}
		}

		public void MarkDirty(object entity)
		{
			Context.Entry(entity).State = EntityState.Modified;
		}

		public void SetValue(object entity, object values)
		{
			Context.Entry(entity).CurrentValues.SetValues(values);
		}

		public void MarkNew(object entity)
		{

			Context.Entry(entity).State = EntityState.Added;
		}

		public void MarkDeleted(object entity)
		{
			Context.Entry(entity).State = EntityState.Deleted;
		}

		public async Task Save()
		{
			await Context.SaveChangesAsync();
		}

		public async Task<int> ExecuteSqlCommand(string sql)
		{
		    return await Context.Database.ExecuteSqlCommandAsync(sql);
		}

		public async Task<int> ExecuteSqlCommand(string sql, params object[] parameters)
		{
			return await Context.Database.ExecuteSqlCommandAsync(sql, parameters);
		}

		bool _disposed; 

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			if (disposing)
			{
				if (InTransaction)
					RollbackTransaction();

				_context?.Dispose();
			}

			_disposed = true;
			_context = null;
		}


		#region Transaction

		private IDbContextTransaction _dbTran;

		public bool InTransaction => _dbTran != null;

	    public void BeginTransaction()
		{
			if (InTransaction) throw new ArgumentException();
			_dbTran = Context.Database.BeginTransaction();
		}

		public void CommitTransaction()
		{
			if (InTransaction==false) throw new ArgumentException();
			_dbTran.Commit();
			_dbTran = null;
		}

		public void RollbackTransaction()
		{
			if (InTransaction==false) throw new ArgumentException();
			_dbTran.Rollback();
			_dbTran = null;
		}


		#endregion

		public void InitializeDatabase()
		{
            throw new NotImplementedException();
			//Context.Database.Initialize(true);        
		}
	}
}
