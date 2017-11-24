////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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

using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using CNCLib.Repository.Contracts;
using Framework.Tools.Pattern;

namespace CNCLib.Repository
{
    public class UserRepository : CNCLibRepository, IUserRepository
	{
        public UserRepository(IUnitOfWork uow) : base(uow)
        {
        }

        public async Task<Contracts.Entities.User[]> GetUsers()
		{
            return await Context.Users.
                ToArrayAsync();
		}

		public async Task<Contracts.Entities.User> GetUser(int id)
        {
			return await Context.Users.
				Where(m => m.UserID == id).
				FirstOrDefaultAsync();
        }

        public async Task<Contracts.Entities.User> GetUser(string username)
        {
            return await Context.Users.
                Where(m => m.UserName == username).
                FirstOrDefaultAsync();
        }

        public async Task Delete(Contracts.Entities.User user)
        {
			Uow.MarkDeleted(user);
			await Task.FromResult(0);
		}

		public async Task Store(Contracts.Entities.User user)
		{
			// search und update User

			int id = user.UserID;

			var UserInDb = await Context.Users.
				Where(m => m.UserID == id).
				FirstOrDefaultAsync();

			if (UserInDb == default(Contracts.Entities.User))
			{
				// add new

				Uow.MarkNew(user);
			}
			else
			{
				// syn with existing

				Uow.SetValue(UserInDb,user);

				// search und update Usercommands (add and delete)
			}
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~UserRepository() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion
	}
}
