////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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

using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using CNCLib.Repository.Contracts;

namespace CNCLib.Repository
{
    public class ConfigurationRepository : CNCLibRepository, IConfigurationRepository
	{
		public async Task<Contracts.Entities.Configuration> Get(string group, string  name)
        {
			return await Context.Configurations.Where((c) => c.Group == group && c.Name == name).FirstOrDefaultAsync();
        }

		public async Task Delete(Contracts.Entities.Configuration configuration)
        {
			Uow.MarkDeleted(configuration);
			await Task.FromResult(0);
        }


		public async Task Save(Contracts.Entities.Configuration configuration)
		{
			// search und update machine

			var cInDb = await Context.Configurations.Where((c) => c.Group == configuration.Group && c.Name == configuration.Name).FirstOrDefaultAsync();

			if (cInDb == default(Contracts.Entities.Configuration))
			{
				// add new

				cInDb = configuration;
				Uow.MarkNew(cInDb);
			}
			else
			{
				// syn with existing
				Uow.SetValue(cInDb,configuration);
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
		// ~ConfigurationRepository() {
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
