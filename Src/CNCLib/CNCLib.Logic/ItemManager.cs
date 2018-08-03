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
using Framework.Logic;
using CNCLib.Repository.Contracts;
using CNCLib.Logic.Converter;
using CNCLib.Logic.Contracts;
using CNCLib.Logic.Contracts.DTO;
using Framework.Contracts.Repository;

namespace CNCLib.Logic
{
    public class ItemManager : ControllerBase, IItemManager
    {
        private IUnitOfWork _unitOfWork;
        private IItemRepository _repository;

        public ItemManager(IUnitOfWork unitOfWork, IItemRepository repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public async Task<IEnumerable<Item>> GetAll()
		{
			return (await _repository.GetAll()).ToDto();
		}

		public async Task<IEnumerable<Item>> GetByClassName(string classname)
		{
			return (await _repository.Get(classname)).ToDto();
		}

		public async Task<Item> Get(int id)
		{
			return (await _repository.Get(id)).ToDto();
		}

		public async Task Delete(Item item)
		{
			_repository.Delete(item.ToEntity());
			await _unitOfWork.SaveChangesAsync();
		}

		public async Task<int> Add(Item item)
		{
            using (var trans = _unitOfWork.BeginTransaction())
            {
                var me = item.ToEntity();
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
            throw new NotImplementedException();
/*
		    using (var trans = _unitOfWork.BeginTransaction())
		    {
				var me = item.ToEntity();
				await _repository.Store(me);
				await _unitOfWork.SaveChangesAsync();
		        await trans.CommitTransactionAsync();

		        return me.ItemID;
		    }
*/
		}
    }
}
