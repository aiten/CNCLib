////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2018 Herbert Aitenbichler

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
using System.Threading.Tasks;
using CNCLib.Repository.Context;
using CNCLib.Repository.Contracts;
using CNCLib.Repository.Contracts.Entities;
using Framework.EF;
using Microsoft.EntityFrameworkCore;

namespace CNCLib.Repository
{
    public class ConfigurationRepository : CRUDRepositoryBase<CNCLibContext, Configuration, ConfigurationPrimary>, IConfigurationRepository
	{
        public ConfigurationRepository(CNCLibContext dbcontext) : base(dbcontext)
        {
        }

	    protected override IQueryable<Configuration> AddInclude(IQueryable<Configuration> query)
	    {
	        return query;
	    }

	    protected override IQueryable<Configuration> AddPrimaryWhere(IQueryable<Configuration> query, ConfigurationPrimary key)
	    {
	        return query.Where(c => c.Group == key.Group && c.Name == key.Name);
	    }

        public async Task<Configuration> Get(string group, string  name)
        {
			return await Query.Where(c => c.Group == group && c.Name == name).FirstOrDefaultAsync();
        }

        public async Task Store(Configuration configuration)
		{
			// search und update machine

			var cInDb = await TrackingQuery.Where(c => c.Group == configuration.Group && c.Name == configuration.Name).FirstOrDefaultAsync();

			if (cInDb == default(Configuration))
			{
				// add new

				cInDb = configuration;
				AddEntity(cInDb);
			}
			else
			{
				// syn with existing
				SetValue(cInDb,configuration);
			}
		}
	}
}

