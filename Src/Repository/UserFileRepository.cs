﻿/*
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

namespace CNCLib.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CNCLib.Repository.Abstraction;
    using CNCLib.Repository.Abstraction.Entities;
    using CNCLib.Repository.Abstraction.QueryResult;
    using CNCLib.Repository.Context;

    using Framework.Repository;

    using Microsoft.EntityFrameworkCore;

    public class UserFileRepository : CrudRepository<CNCLibContext, UserFile, int>, IUserFileRepository
    {
        #region ctr/default/overrides

        public UserFileRepository(CNCLibContext context) : base(context)
        {
        }

        protected override FilterBuilder<UserFile, int> FilterBuilder =>
            new FilterBuilder<UserFile, int>()
            {
                PrimaryWhere   = (query, key) => query.Where(item => item.UserFileId == key),
                PrimaryWhereIn = (query, keys) => query.Where(item => keys.Contains(item.UserFileId))
            };

        protected override IQueryable<UserFile> AddInclude(IQueryable<UserFile> query)
        {
            return query;
        }

        #endregion

        #region extra Queries

        public async Task<IList<UserFile>> GetByUser(int userId)
        {
            return await QueryWithInclude.Where(m => m.UserId == userId).ToListAsync();
        }

        public async Task DeleteByUser(int userId)
        {
            var userFiles = await TrackingQueryWithInclude.Where(m => m.UserId == userId).ToListAsync();
            DeleteEntities(userFiles);
        }

        private static UserFileInfo WithNoImage(UserFile userFile)
        {
            return new UserFileInfo()
            {
                FileName   = userFile.FileName,
                UserFileId = userFile.UserFileId,
                IsSystem   = userFile.IsSystem,
                UploadTime = userFile.UploadTime,
                FileSize   = userFile.Content.Length
            };
        }

        public async Task<IList<UserFileInfo>> GetFileInfos(int userId)
        {
            return await Query
                .Where(f => f.UserId == userId)
                .Select(f => WithNoImage(f)).ToListAsync();
        }

        public async Task<UserFileInfo> GetFileInfo(int userFileId)
        {
            return await Query
                .Where(f => f.UserFileId == userFileId)
                .Select(f => WithNoImage(f))
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetFileId(int userId, string fileName)
        {
            return await Query.Where(f => f.UserId == userId && f.FileName == fileName).Select(f => f.UserFileId).FirstOrDefaultAsync();
        }

        public async Task<UserFile> GetByName(int userId, string fileName)
        {
            return await QueryWithInclude.FirstOrDefaultAsync(f => f.UserId == userId && f.FileName == fileName);
        }

        public async Task<long> GetTotalUserFileSize(int userId)
        {
            return await Query
                .Where(f => f.UserId == userId)
                .GroupBy(f => f.UserId, g => g.Content.Length, (_, fileSizes) => fileSizes.Sum())
                .FirstAsync();
        }

        public async Task<long> GetUserFileSize(int userFileId)
        {
            return await Query
                .Where(f => f.UserFileId == userFileId)
                .Select(f => f.Content.Length)
                .FirstAsync();
        }

        #endregion
    }
}