////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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

namespace Framework.Tools.Pattern
{
	public interface IUnitOfWork : IDisposable
    {
        void MarkDirty(object entity);
		void MarkNew(object entity);
		void MarkDeleted(object entity);
		void SetValue(object entity, object values);

		Task Save();

		// SQL Commands

		Task<int> ExecuteSqlCommand(string sql);
		Task<int> ExecuteSqlCommand(string sql, params object[] parameters);

		// Transaction

		void BeginTransaction();
		void CommitTransaction();
		void RollbackTransaction();

		// Global

		void InitializeDatabase();
    }
}
