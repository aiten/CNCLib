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

namespace CNCLib.Repository.Abstraction
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CNCLib.Repository.Abstraction.Entities;
    using CNCLib.Repository.Abstraction.QueryResult;

    using Framework.Repository.Abstraction;

    public interface IUserFileRepository : ICrudRepository<UserFileEntity, int>
    {
        Task<IList<UserFileEntity>> GetByUser(int userId);

        Task<IList<int>> GetIdByUser(int userId);

        Task DeleteByUser(int userId);

        Task<IList<UserFileInfoQuery>> GetFileInfos(int userId);

        Task<UserFileInfoQuery> GetFileInfo(int userFileId);

        Task<int> GetFileId(int userId, string fileName);

        Task<UserFileEntity> GetByName(int userId, string fileName);

        Task<long> GetTotalUserFileSize(int userId);

        Task<long> GetUserFileSize(int userFileId);
    }
}