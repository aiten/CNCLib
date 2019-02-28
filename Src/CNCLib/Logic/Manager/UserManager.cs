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
using System.Threading.Tasks;

using AutoMapper;

using CNCLib.Logic.Contract;
using CNCLib.Logic.Contract.DTO;
using CNCLib.Repository.Contract;

using Framework.Logic;
using Framework.Repository.Abstraction;

using UserEntity = CNCLib.Repository.Contract.Entities.User;

namespace CNCLib.Logic.Manager
{
    public class UserManager : CRUDManager<User, int, UserEntity>, IUserManager
    {
        private readonly IUnitOfWork     _unitOfWork;
        private readonly IUserRepository _repository;
        private readonly IMapper         _mapper;

        public UserManager(IUnitOfWork unitOfWork, IUserRepository repository, IMapper mapper) : base(unitOfWork, repository, mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException();
            _repository = repository ?? throw new ArgumentNullException();
            _mapper     = mapper ?? throw new ArgumentNullException();
        }

        protected override int GetKey(UserEntity entity)
        {
            return entity.UserId;
        }

        public async Task<User> GetByName(string username)
        {
            return MapToDto(await _repository.GetByName(username));
        }
    }
}