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
using System.Threading.Tasks;
using AutoMapper;
using Framework.Logic;
using CNCLib.Repository.Contracts;
using CNCLib.Logic.Converter;
using CNCLib.Logic.Contracts;
using CNCLib.Logic.Contracts.DTO;
using Framework.Contracts.Repository;

namespace CNCLib.Logic
{
    public class ItemManager : ManagerBase, IItemManager
    {
        private IUnitOfWork _unitOfWork;
        private IItemRepository _repository;
        private IMapper _mapper;

        public ItemManager(IUnitOfWork unitOfWork, IItemRepository repository, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException();
            _repository = repository ?? throw new ArgumentNullException();
            _mapper = mapper ?? throw new ArgumentNullException();
        }

        public async Task<IEnumerable<Item>> GetAll()
		{
			return (await _repository.GetAll()).ToDto(_mapper);
		}

		public async Task<IEnumerable<Item>> GetByClassName(string classname)
		{
			return (await _repository.Get(classname)).ToDto(_mapper);
		}

		public async Task<Item> Get(int id)
		{
			return (await _repository.Get(id)).ToDto(_mapper);
		}

		public async Task Delete(Item item)
		{
			_repository.Delete(item.ToEntity(_mapper));
			await _unitOfWork.SaveChangesAsync();
		}

		public async Task<int> Add(Item item)
		{
            using (var trans = _unitOfWork.BeginTransaction())
            {
                var me = item.ToEntity(_mapper);
                me.ItemID = 0;
                foreach (var mc in me.ItemProperties) mc.ItemID = 0;
                _repository.Add(me);
                await _unitOfWork.SaveChangesAsync();
                await trans.CommitTransactionAsync();

                return me.ItemID;
            }
        }

		public async Task<int> Update(Item item)
		{
		    using (var trans = _unitOfWork.BeginTransaction())
		    {
				var me = item.ToEntity(_mapper);
				await _repository.Update(me.ItemID,me);
				await _unitOfWork.SaveChangesAsync();
		        await trans.CommitTransactionAsync();

		        return me.ItemID;
		    }
		}
    }
}
