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
using System.Threading.Tasks;

using AutoMapper;

using CNCLib.Logic.Abstraction;
using CNCLib.Logic.Abstraction.DTO;
using CNCLib.Repository.Abstraction;
using CNCLib.Repository.Abstraction.Entities;
using CNCLib.Shared;

using Framework.Localization;
using Framework.Logic;
using Framework.Repository.Abstraction;
using Framework.Tools.Abstraction;

using ErrorMessagesLogic = CNCLib.Logic.ErrorMessages;

public class UserFileManager : CrudManager<UserFile, int, UserFileEntity>, IUserFileManager
{
    private readonly IUnitOfWork         _unitOfWork;
    private readonly IUserFileRepository _repository;
    private readonly IMapper             _mapper;
    private readonly ICNCLibUserContext  _userContext;
    private readonly ICurrentDateTime    _currentDateTime;

    public UserFileManager(IUnitOfWork unitOfWork, IUserFileRepository repository, ICNCLibUserContext userContext, IMapper mapper, ICurrentDateTime currentDateTime) : base(unitOfWork, repository, mapper)
    {
        _unitOfWork      = unitOfWork;
        _repository      = repository;
        _userContext     = userContext;
        _mapper          = mapper;
        _currentDateTime = currentDateTime;
    }

    #region file size check

    protected override async Task ValidateDtoAsync(UserFile dto, ValidationType validation)
    {
        await base.ValidateDtoAsync(dto, validation);

        if (validation == ValidationType.AddValidation || validation == ValidationType.UpdateValidation)
        {
            const int MAXFILESIZE = 1024 * 1024 * 32;
            if (dto.Content.Length > MAXFILESIZE)
            {
                throw new ArgumentException(
                    ErrorMessagesLogic.ResourceManager.ToLocalizable(nameof(ErrorMessagesLogic.CNCLib_Logic_FileToBig), new object[] { MAXFILESIZE, dto.Content.Length }).Message());
            }

            var currentDbSize = await _repository.GetTotalUserFileSizeAsync(_userContext.UserId);

            if (validation == ValidationType.UpdateValidation)
            {
                currentDbSize -= await _repository.GetUserFileSizeAsync(dto.UserFileId);
            }

            currentDbSize += dto.Content.LongLength;

            const int MAXTOTALFILESIZE = 1024 * 1024 * 100;
            if (currentDbSize > MAXTOTALFILESIZE)
            {
                throw new ArgumentException(
                    ErrorMessagesLogic.ResourceManager.ToLocalizable(nameof(ErrorMessagesLogic.CNCLib_Logic_TotalFilesToBig), new object[] { MAXTOTALFILESIZE, currentDbSize }).Message());
            }
        }
    }

    #endregion

    protected override void AddEntity(UserFileEntity entity)
    {
        base.AddEntity(entity);
        entity.UploadTime = _currentDateTime.Now;
    }

    protected override void UpdateEntity(UserFileEntity entityInDb, UserFileEntity values)
    {
        values.UploadTime = _currentDateTime.Now;
        base.UpdateEntity(entityInDb, values);
    }

    protected override Task<IList<UserFileEntity>> GetAllEntitiesAsync()
    {
        return _repository.GetByUserAsync(_userContext.UserId);
    }

    public async Task<IEnumerable<UserFileInfo>> GetFileInfosAsync()
    {
        return _mapper.Map<IEnumerable<UserFileInfo>>(await _repository.GetFileInfosAsync(_userContext.UserId));
    }

    public async Task<UserFileInfo> GetFileInfoAsync(UserFile userFile)
    {
        return _mapper.Map<UserFileInfo>(await _repository.GetFileInfoAsync(userFile.UserFileId));
    }

    protected override int GetKey(UserFileEntity entity)
    {
        return entity.UserFileId;
    }

    public async Task<UserFile> GetByNameAsync(string filename)
    {
        return await MapToDtoAsync(await _repository.GetByNameAsync(_userContext.UserId, filename));
    }

    public async Task<int> GetFileIdAsync(string fileName)
    {
        return await _repository.GetFileIdAsync(_userContext.UserId, fileName);
    }
}