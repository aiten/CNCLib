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

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Framework.Contracts.Repository
{
    public enum EntityState
    {
        Detached,
        Unchanged,
        Deleted,
        Modified,
        Added,
    }

    public interface ICRUDRepository<TEntry, TKey>: IBaseRepository where TEntry : class
    {
        Task<IEnumerable<TEntry>> GetAll();
        Task<TEntry> Get(TKey key);
        Task<TEntry> GetTracking(TKey key);
        Task Update(TKey key, TEntry values);               // shortcut to GetTracking and SetValue


        void Add(TEntry entity);
        void Delete(TEntry entity);
        void SetValue(TEntry entity, object values);

        void SetState(TEntry entity, EntityState state);
    }
}
