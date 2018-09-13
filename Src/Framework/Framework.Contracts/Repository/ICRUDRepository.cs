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

    public interface ICRUDRepository<TEntity, TKey> : IGetRepository<TEntity, TKey> where TEntity : class
    {
        void Add(TEntity                   entity);
        void AddRange(IEnumerable<TEntity> entities);

        void Delete(TEntity                   entity);
        void DeleteRange(IEnumerable<TEntity> entities);

        void SetValue(TEntity      trackingentity, TEntity values);
        void SetValueGraph(TEntity trackingentity, TEntity values);

        void SetState(TEntity entity, EntityState state);
    }
}