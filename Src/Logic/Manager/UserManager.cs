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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;

using AutoMapper;

using CNCLib.Logic.Abstraction;
using CNCLib.Logic.Abstraction.DTO;
using CNCLib.Logic.Statistics;
using CNCLib.Repository.Abstraction;
using CNCLib.Repository.Abstraction.Entities;
using CNCLib.Shared;

using Framework.Logic;
using Framework.Repository.Abstraction;
using Framework.Tools.Abstraction;
using Framework.Tools.Password;

public class UserManager : CrudManager<User, int, UserEntity>, IUserManager
{
    private readonly IUnitOfWork              _unitOfWork;
    private readonly IUserRepository          _repository;
    private readonly IMachineRepository       _machineRepository;
    private readonly IItemRepository          _itemRepository;
    private readonly IUserFileRepository      _userFileRepository;
    private readonly IInitRepository          _initRepository;
    private readonly IConfigurationRepository _configRepository;

    private readonly ICurrentDateTime   _currentDate;
    private readonly IMapper            _mapper;
    private readonly ICNCLibUserContext _userContext;

    private readonly IOneWayPasswordProvider _passwordProvider;
    private readonly CallStatisticCache      _callStatisticCache;

    public const int GlobalUserId = 1;

    public UserManager(IUnitOfWork unitOfWork,
        IUserRepository            repository,
        IMachineRepository         machineRepository,
        IItemRepository            itemRepository,
        IUserFileRepository        userFileRepository,
        IConfigurationRepository   configRepository,
        IInitRepository            initRepository,
        ICNCLibUserContext         userContext,
        ICurrentDateTime           currentDate,
        IMapper                    mapper,
        IOneWayPasswordProvider    passwordProvider,
        CallStatisticCache         callStatisticCache) : base(unitOfWork, repository, mapper)
    {
        _unitOfWork         = unitOfWork;
        _repository         = repository;
        _machineRepository  = machineRepository;
        _itemRepository     = itemRepository;
        _configRepository   = configRepository;
        _userFileRepository = userFileRepository;
        _initRepository     = initRepository;
        _userContext        = userContext;
        _currentDate        = currentDate;
        _mapper             = mapper;
        _passwordProvider   = passwordProvider;
        _callStatisticCache = callStatisticCache;
    }

    protected override int GetKey(UserEntity entity)
    {
        return entity.UserId;
    }

    public async Task<User> GetByNameAsync(string username)
    {
        return await MapToDtoAsync(await _repository.GetByNameAsync(username));
    }

    public async Task<ClaimsPrincipal> AuthenticateAsync(string userName, string password)
    {
        var userEntity = await _repository.GetByNameAsync(userName);

        if (!string.IsNullOrEmpty(password) && userEntity != null && _passwordProvider.ValidatePassword(password, userEntity.Password))
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, userEntity.UserId.ToString()),
                new Claim(ClaimTypes.Name,           userName),
            };

            if (userName == CNCLibConst.AdminUser)
            {
                claims.Add(new Claim(CNCLibClaimTypes.IsAdmin, "true"));
            }

            var identity  = new ClaimsIdentity(claims, "BasicAuthentication");
            var principal = new ClaimsPrincipal(identity);

            _callStatisticCache.AddCall(new CallStatistic()
            {
                CallTime = _currentDate.Now,
                UserId   = userEntity.UserId
            });

            return principal;
        }

        return null;
    }

    public async Task<string> RegisterAsync(string userName, string password)
    {
        using (var trans = _unitOfWork.BeginTransaction())
        {
            var userEntity = await _repository.GetByNameAsync(userName);

            if (userEntity != null)
            {
                throw new Exception("user already exists");
            }

            userEntity = new UserEntity()
            {
                Name     = userName,
                Password = _passwordProvider.GetPasswordHash(password),
                Created  = _currentDate.Now
            };

            await _repository.AddAsync(userEntity);

            await trans.SaveChangesAsync();

            await _initRepository.InitializeAsync(userEntity.UserId);

            await CommitTransactionAsync(trans);

            return userEntity.UserId.ToString();
        }
    }

    public async Task ChangePasswordAsync(string userName, string passwordOld, string passwordNew)
    {
        using (var trans = _unitOfWork.BeginTransaction())
        {
            var authUser = await AuthenticateAsync(userName, passwordOld);

            if (authUser == null)
            {
                throw new AuthenticationException();
            }

            var userEntity = await _repository.GetByNameTrackingAsync(userName);
            userEntity.Password = _passwordProvider.GetPasswordHash(passwordNew);

            await CommitTransactionAsync(trans);
        }
    }

    public async Task<string> CreatePasswordHashAsync(string password)
    {
        await Task.Delay(100);
        return await Task.FromResult(_passwordProvider.GetPasswordHash(password));
    }

    public async Task InitDataAsync()
    {
        using (var trans = _unitOfWork.BeginTransaction())
        {
            await _initRepository.InitializeAsync(_userContext.UserId);

            await CommitTransactionAsync(trans);
        }
    }

    public async Task CleanupAsync()
    {
        using (var trans = _unitOfWork.BeginTransaction())
        {
            await DeleteData(_userContext.UserId);

            await CommitTransactionAsync(trans);
        }
    }

    private async Task DeleteData(int userId)
    {
        await _machineRepository.DeleteByUserAsync(userId);
        await _itemRepository.DeleteByUserAsync(userId);
        await _userFileRepository.DeleteByUserAsync(userId);
        await _configRepository.DeleteByUserAsync(userId);
    }

    public async Task LeaveAsync()
    {
        using (var trans = _unitOfWork.BeginTransaction())
        {
            await DeleteData(_userContext.UserId);

            await _repository.DeleteAsync(await _repository.GetTrackingAsync(_userContext.UserId));

            await CommitTransactionAsync(trans);
        }
    }

    public async Task LeaveAsync(string userName)
    {
        // only as admin

        using (var trans = _unitOfWork.BeginTransaction())
        {
            var userEntity = await _repository.GetByNameAsync(userName);

            if (userEntity != null && userName != CNCLibConst.AdminUser && _userContext.IsAdmin)
            {
                await DeleteData(userEntity.UserId);

                await _repository.DeleteAsync(await _repository.GetTrackingAsync(userEntity.UserId));
            }

            await CommitTransactionAsync(trans);
        }
    }

    public async Task InitMachinesAsync()
    {
        var defaultMachines = _initRepository.GetDefaultMachines();

        using (var trans = _unitOfWork.BeginTransaction())
        {
            var userMachines = await _machineRepository.GetTrackingAsync(await _machineRepository.GetIdByUserAsync(_userContext.UserId));

            var sameMachines = userMachines.Join(defaultMachines,
                m => m.Name.ToLower(),
                m => m.Name.ToLower(),
                (m, _) => m);

            await _machineRepository.DeleteRangeAsync(sameMachines);

            await _unitOfWork.SaveChangesAsync();
            await _initRepository.AddDefaultMachinesAsync(_userContext.UserId);
            await CommitTransactionAsync(trans);
        }
    }

    public async Task InitItemsAsync()
    {
        var defaultItems = _initRepository.GetDefaultItems();
        var defaultFiles = _initRepository.GetDefaultFiles();

        using (var trans = _unitOfWork.BeginTransaction())
        {
            var userItems = await _itemRepository.GetTrackingAsync(await _itemRepository.GetIdByUserAsync(_userContext.UserId));

            var sameItems = userItems.Join(defaultItems,
                item => item.Name.ToLower(),
                item => item.Name.ToLower(),
                (item, _) => item);

            await _itemRepository.DeleteRangeAsync(sameItems);

            var userFiles = await _userFileRepository.GetTrackingAsync(await _userFileRepository.GetIdByUserAsync(_userContext.UserId));

            var sameFiles = userFiles.Join(defaultFiles,
                f => f.FileName.ToLower(),
                f => f.FileName.ToLower(),
                (r, _) => r);

            await _userFileRepository.DeleteRangeAsync(sameFiles);

            await _unitOfWork.SaveChangesAsync();
            await _initRepository.AddDefaultItemsAsync(_userContext.UserId);
            await _initRepository.AddDefaultFilesAsync(_userContext.UserId);
            await CommitTransactionAsync(trans);
        }
    }
}