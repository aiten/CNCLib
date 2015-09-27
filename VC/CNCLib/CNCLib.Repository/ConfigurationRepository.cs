using CNCLib.Repository.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CNCLib.Repository;
using Framework.Tools;
using System.Data.Entity;
using Framework.EF;

namespace CNCLib.Repository
{
    public class ConfigurationRepository
    {
		public Entities.Configuration Get(string group, string  name)
        {
			using (IUnitOfWork uow = UnitOfWorkFactory.Create())
			{
				return uow.Query<Entities.Configuration>().Where((c) => c.Group == group && c.Name == name).FirstOrDefault();
			}
        }

		public void Delete(Entities.Configuration configuration)
        {
			using (IUnitOfWork uow = UnitOfWorkFactory.Create())
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


		public void Save(Entities.Configuration configuration)
		{
			// search und update machine

			using (IUnitOfWork uow = UnitOfWorkFactory.Create())
			{
				try
				{
					uow.BeginTransaction();

					var cInDb = uow.Query<Entities.Configuration>().Where((c) => c.Group == configuration.Group && c.Name == configuration.Name).FirstOrDefault();

					if (cInDb == default(Entities.Configuration))
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
				catch (Exception ex)
				{
					uow.RollbackTransaction();
					throw;
				}
			}
		}
    }
}
