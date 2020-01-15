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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using CNCLib.Logic.Abstraction;
using CNCLib.Logic.Abstraction.DTO;
using CNCLib.Repository.Abstraction;
using CNCLib.Shared;

using Framework.Logic;
using Framework.Repository.Abstraction;

using UserFileEntity = CNCLib.Repository.Abstraction.Entities.UserFile;

namespace CNCLib.Logic.Manager
{
    public class UserFileManager : CrudManager<UserFile, int, UserFileEntity>, IUserFileManager
    {
        private readonly IUnitOfWork         _unitOfWork;
        private readonly IUserFileRepository _repository;
        private readonly IMapper             _mapper;
        private readonly ICNCLibUserContext  _userContext;


        public UserFileManager(IUnitOfWork unitOfWork, IUserFileRepository repository, ICNCLibUserContext userContext, IMapper mapper) : base(unitOfWork, repository, mapper)
        {
            _unitOfWork  = unitOfWork;
            _repository  = repository;
            _userContext = userContext;
            _mapper      = mapper;
        }

        protected override Task<IList<UserFileEntity>> GetAllEntities()
        {
            return _repository.GetByUser(_userContext.UserId);
        }

        public async Task<IEnumerable<string>> GetFileNames()
        {
            return await _repository.GetFileNames(_userContext.UserId);
        }

        protected override int GetKey(UserFileEntity entity)
        {
            return entity.UserFileId;
        }

        public async Task<UserFile> GetByName(string filename)
        {
            return MapToDto(await _repository.GetByName(_userContext.UserId, filename));
        }

        public async Task<int> GetFileId(string fileName)
        {
            return await _repository.GetFileId(_userContext.UserId, fileName);
        }
    }
}