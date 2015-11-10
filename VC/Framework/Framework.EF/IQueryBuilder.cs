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
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Framework.EF
{
	public interface IQueryBuilder<T> where T : class
    {
        IQueryBuilder<T> Where(Expression<Func<T, bool>> predicate);
        IQueryBuilder<T> Include(Expression<Func<T, object>> path);
        IQueryBuilder<T> OrderBy(Expression<Func<T, object>> path);
        IQueryBuilder<T> OrderByDescending(Expression<Func<T, object>> path);
        IQueryBuilder<T> Page(int page, int pageSize);

        T FirstOrDefault();
        Task<T> FirstOrDefaultAsync();

        List<T> ToList();
        Task<List<T>> ToListAsync();

        int Count();
        Task<int> CountAsync();
    }
}
