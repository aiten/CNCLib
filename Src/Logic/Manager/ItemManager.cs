/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) Herbert Aitenbichler

  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
  to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
  and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
*/

namespace CNCLib.Logic.Manager;

using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using CNCLib.Logic.Abstraction;
using CNCLib.Logic.Abstraction.DTO;
using CNCLib.Repository.Abstraction;
using CNCLib.Repository.Abstraction.Entities;
using CNCLib.Shared;

using Framework.Logic;
using Framework.Repository.Abstraction;

public class ItemManager : CrudManager<Item, int, ItemEntity>, IItemManager
{
    private readonly IUnitOfWork        _unitOfWork;
    private readonly IItemRepository    _repository;
    private readonly ICNCLibUserContext _userContext;

    public ItemManager(IUnitOfWork unitOfWork, IItemRepository repository, ICNCLibUserContext userContext, IMapper mapper) : base(unitOfWork, repository, mapper)
    {
        _unitOfWork  = unitOfWork;
        _repository  = repository;
        _userContext = userContext;
    }

    protected override int GetKey(ItemEntity entity)
    {
        return entity.ItemId;
    }

    protected override Task<IList<ItemEntity>> GetAllEntitiesAsync()
    {
        return _repository.GetByUserAsync(_userContext.UserId);
    }

    protected override void AddEntity(ItemEntity entityInDb)
    {
        entityInDb.UserId = _userContext.UserId;
        base.AddEntity(entityInDb);
    }

    protected override async Task UpdateEntityAsync(ItemEntity entityInDb, ItemEntity values)
    {
        // do not overwrite user!

        values.UserId = entityInDb.UserId;
        values.User   = entityInDb.User;

        await base.UpdateEntityAsync(entityInDb, values);
    }

    public async Task<IEnumerable<Item>> GetByClassNameAsync(string classname)
    {
        return await MapToDtoAsync(await _repository.GetAsync(_userContext.UserId, classname));
    }
}