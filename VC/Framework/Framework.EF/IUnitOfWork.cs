////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2015 Herbert Aitenbichler

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

namespace Framework.EF
{
	public interface IUnitOfWork : IDisposable
    {
        void MarkDirty(object entity);
		void MarkNew(object entity);
		void MarkDeleted(object entity);

		IQueryBuilder<T> Query<T>() where T : class;

		void Save();

		// SQL Commands

		int ExecuteSqlCommand(string sql);
		int ExecuteSqlCommand(string sql, params object[] parameters);

		// Transaction

		void BeginTransaction();
		void CommitTransaction();
		void RollbackTransaction();

		// Global

		void InitializeDatabase();
    }
}
