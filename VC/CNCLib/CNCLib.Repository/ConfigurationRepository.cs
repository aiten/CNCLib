////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2015 Herbert Aitenbichler

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

using CNCLib.Repository.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CNCLib.Repository.Contracts;
using Framework.Tools;
using System.Data.Entity;
using Framework.EF;
using Framework.Tools.Pattern;

namespace CNCLib.Repository
{
    public class ConfigurationRepository : RepositoryBase, IConfigurationRepository
	{
		public Contracts.Entities.Configuration Get(string group, string  name)
        {
			using (var uow = UnitOfWorkFactory.CreateAndCast())
			{
				return uow.Context.Configurations.Where((c) => c.Group == group && c.Name == name).FirstOrDefault();
			}
        }

		public void Delete(Contracts.Entities.Configuration configuration)
        {
			using (var uow = UnitOfWorkFactory.CreateAndCast())
			{
				try
				{
					uow.BeginTransaction();

					uow.MarkDeleted(configuration);
					uow.Save();

					uow.CommitTransaction();
				}
				catch (Exception)
				{
					uow.RollbackTransaction();
					throw;
				}
			}
        }


		public void Save(Contracts.Entities.Configuration configuration)
		{
			// search und update machine

			using (var uow = UnitOfWorkFactory.CreateAndCast())
			{
				try
				{
					uow.BeginTransaction();

					var cInDb = uow.Context.Configurations.Where((c) => c.Group == configuration.Group && c.Name == configuration.Name).FirstOrDefault();

					if (cInDb == default(Contracts.Entities.Configuration))
					{
						// add new

						cInDb = configuration;
						uow.MarkNew(cInDb);
						uow.Save();
					}
					else
					{
						// syn with existing
						cInDb.CopyValueTypeProperties(configuration);
						uow.Save();
					}

					uow.CommitTransaction();
				}
				catch (Exception /* ex */)
				{
					uow.RollbackTransaction();
					throw;
				}
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
