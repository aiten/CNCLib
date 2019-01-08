////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2019 Herbert Aitenbichler

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

namespace Framework.Repository.Abstraction
{
    using System;
    using System.Collections.Generic;

    public interface ICRUDRepository<TEntity, TKey> : IGetRepository<TEntity, TKey>
        where TEntity : class
    {
        void Add(TEntity entity);

        void AddRange(IEnumerable<TEntity> entities);

        void Delete(TEntity entity);

        void DeleteRange(IEnumerable<TEntity> entities);

        void SetValue(TEntity trackingEntity, TEntity values);

        void SetValueGraph(TEntity trackingEntity, TEntity values);

        void SetState(TEntity entity, EntityState state);

        void Sync(ICollection<TEntity> inDb, ICollection<TEntity> toDb, Func<TEntity, TEntity, bool> predicate);
    }
}