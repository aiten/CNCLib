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
using System.Threading.Tasks;
using AutoMapper;
using Framework.Contracts.Repository;
using Framework.Tools;

namespace Framework.Logic
{
    public abstract class CRUDManager<T, TId, TEntry> : ManagerBase where T : class where TEntry : class
    {
        private IMapper _mapper;
        private ICRUDRepository<TEntry, TId> _repository;
        private IUnitOfWork _unitOfWork;

        protected CRUDManager(IUnitOfWork unitOfWork, ICRUDRepository<TEntry, TId> repository, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException();
            _repository = repository ?? throw new ArgumentNullException();
            _mapper = mapper ?? throw new ArgumentNullException(); ;
        }

        protected abstract TId GetKey(TEntry entity);

        public async Task<IEnumerable<T>> GetAll()
        {
            return _mapper.Map<IEnumerable<TEntry>, IEnumerable<T>>(await _repository.GetAll());
        }

        public async Task<T> Get(TId key)
        {
            return _mapper.Map<TEntry, T>(await _repository.Get(key));
        }

        public async Task<IEnumerable<T>> Get(IEnumerable<TId> keys)
        {
            return _mapper.Map<IEnumerable<TEntry>, IEnumerable<T>>(await _repository.Get(keys));
        }

        public async Task<TId> Add(T value)
        {
            return (await Add(new List<T>() { value })).First();

            /*
                        var me = m.ToEntity(_mapper);
                        me.MachineID = 0;
                        foreach (var mc in me.MachineInitCommands) mc.MachineID = 0;
                        foreach (var mi in me.MachineInitCommands) mi.MachineID = 0;
                        _repository.Add(me);
                        await _unitOfWork.SaveChangesAsync();
                        return me.MachineID;
            */
        }

        public async Task<IEnumerable<TId>> Add(IEnumerable<T> values)
        {
            var entities = _mapper.Map<IEnumerable<T>, IEnumerable<TEntry>>(values);
            using (var trans = _unitOfWork.BeginTransaction())
            {
                _repository.AddRange(entities);
                await trans.CommitTransactionAsync();
                return entities.Select(e => GetKey(e));
            }
        }

        public async Task Delete(T value)
        {
            await Delete(new T[]{value});
        }

        public async Task Delete(IEnumerable<T> values)
        {
            using (var trans = _unitOfWork.BeginTransaction())
            {
                var entities = _mapper.Map<IEnumerable<T>, IEnumerable<TEntry>>(values);
                _repository.DeleteRange(entities);
                await trans.CommitTransactionAsync();
            }
        }
        public async Task Update(T value)
        {
            await Update(new T[] { value });
        }

        public async Task Update(IEnumerable<T> values)
        {
            using (var trans = _unitOfWork.BeginTransaction())
            {
                var entities = _mapper.Map<IEnumerable<T>, IEnumerable<TEntry>>(values);
                foreach (var entity in entities)
                {
                    _repository.Update(GetKey(entity),entity);
                }
                await trans.CommitTransactionAsync();
            }
        }
    }
}
