﻿////////////////////////////////////////////////////////
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

namespace Framework.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Abstraction;

    using AutoMapper;

    using Repository.Abstraction;

    public abstract class CRUDManager<T, TKey, TEntity> : GetManager<T, TKey, TEntity>, ICRUDManager<T, TKey> where T : class where TEntity : class
    {
        private readonly IMapper                        _mapper;
        private readonly ICRUDRepository<TEntity, TKey> _repository;
        private readonly IUnitOfWork                    _unitOfWork;

        protected CRUDManager(IUnitOfWork unitOfWork, ICRUDRepository<TEntity, TKey> repository, IMapper mapper) : base(unitOfWork, repository, mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException();
            _repository = repository ?? throw new ArgumentNullException();
            _mapper     = mapper ?? throw new ArgumentNullException();
        }

        public async Task<TKey> Add(T value)
        {
            return (await Add(new List<T>() { value })).First();
        }

        public async Task<IEnumerable<TKey>> Add(IEnumerable<T> values)
        {
            using (var trans = _unitOfWork.BeginTransaction())
            {
                await ValidateDto(values, ValidationType.AddValidation);
                var entities = MapFromDtos(values, ValidationType.AddValidation);

                foreach (var entity in entities)
                {
                    AddEntity(entity);
                }

                try
                {
                    _repository.AddRange(entities);
                    await trans.CommitTransactionAsync();
                }
                catch (Exception)
                {
                    // Console.WriteLine(e);
                    throw;
                }

                await Modified();

                return entities.Select(GetKey);
            }
        }

        public async Task Delete(T value)
        {
            await Delete(new[] { value });
        }

        public async Task Delete(IEnumerable<T> values)
        {
            using (var trans = _unitOfWork.BeginTransaction())
            {
                await ValidateDto(values, ValidationType.DeleteValidation);
                var entities = MapFromDtos(values, ValidationType.DeleteValidation);

                foreach (var entity in entities)
                {
                    DeleteEntity(entity);
                }

                try
                {
                    _repository.DeleteRange(entities);
                    await trans.CommitTransactionAsync();
                }
                catch (Exception)
                {
                    // for debugging
                    // Console.WriteLine(e);
                    throw;
                }
            }
        }

        public async Task Delete(TKey key)
        {
            await Delete(new[] { key });
        }

        public async Task Delete(IEnumerable<TKey> keys)
        {
            using (var trans = _unitOfWork.BeginTransaction())
            {
                var entities = await _repository.GetTracking(keys);

                foreach (var entity in entities)
                {
                    DeleteEntity(entity);
                }

                _repository.DeleteRange(entities);
                await trans.CommitTransactionAsync();
                await Modified();
            }
        }

        public async Task Update(T value)
        {
            await Update(new[] { value });
        }

        public async Task Update(IEnumerable<T> values)
        {
            using (var trans = _unitOfWork.BeginTransaction())
            {
                await ValidateDto(values, ValidationType.UpdateValidation);

                var entities = MapFromDtos(values, ValidationType.UpdateValidation);

                var entitiesInDb = await _repository.GetTracking(entities.Select(GetKey));

                var mergeJoin = entitiesInDb.Join(entities, GetKey, GetKey, (EntityInDb, Entity) => new { EntityInDb, Entity });

                if (entities.Count() != entitiesInDb.Count() || entities.Count() != mergeJoin.Count())
                {
                    throw new ArgumentException();
                }

                foreach (var merged in mergeJoin)
                {
                    UpdateEntity(merged.EntityInDb, merged.Entity);
                }

                try
                {
                    await trans.CommitTransactionAsync();
                    await Modified();
                }
                catch (Exception)
                {
                    // Console.WriteLine(e);
                    throw;
                }
            }
        }

        #region Validadation and Modification overrides

        protected enum ValidationType
        {
            AddValidation,
            UpdateValidation,
            DeleteValidation
        }

        protected virtual async Task ValidateDto(IEnumerable<T> values, ValidationType validation)
        {
            foreach (var dto in values)
            {
                await ValidateDto(dto, validation);
            }
        }

#pragma warning disable 1998
        protected virtual async Task ValidateDto(T dto, ValidationType validation)
        {
        }

        protected virtual void AddEntity(TEntity entityInDb)
        {
        }

        protected virtual void DeleteEntity(TEntity entityInDb)
        {
        }

        protected virtual void UpdateEntity(TEntity entityInDb, TEntity values)
        {
            _repository.SetValueGraph(entityInDb, values);
        }

        protected virtual async Task Modified()
        {
        }
#pragma warning restore 1998

        protected virtual IEnumerable<TEntity> MapFromDtos(IEnumerable<T> values, ValidationType validation)
        {
            return _mapper.Map<IEnumerable<T>, IEnumerable<TEntity>>(values);
        }

        #endregion
    }
}