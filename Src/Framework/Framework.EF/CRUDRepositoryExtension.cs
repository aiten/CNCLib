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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Framework.Contracts.Repository;
using Framework.Tools.Pattern;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Unity.Policy.Mapping;

namespace Framework.EF
{
    public static class CRUDRepositoryExtensions
    {
        public static async Task Store<TEntity, TKey>(this ICRUDRepository<TEntity, TKey> repository, TEntity entity, TKey key) where TEntity: class
        {
            TEntity entityinDb = await repository.GetTracking(key);
            if (entityinDb == default(TEntity))
            {
                repository.Add(entity);
            }
            else
            {
                // syn with existing
                repository.SetValue(entityinDb, entity);
            }
        }
    }
}
