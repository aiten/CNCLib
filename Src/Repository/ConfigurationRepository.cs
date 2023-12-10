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

namespace CNCLib.Repository;

using System.Linq;
using System.Threading.Tasks;

using CNCLib.Repository.Abstraction;
using CNCLib.Repository.Abstraction.Entities;
using CNCLib.Repository.Context;

using Framework.Repository;

using Microsoft.EntityFrameworkCore;

public class ConfigurationRepository : CrudRepository<CNCLibContext, ConfigurationEntity, int>, IConfigurationRepository
{
    #region ctr/default/overrides

    public ConfigurationRepository(CNCLibContext dbContext) : base(dbContext)
    {
    }

    protected override FilterBuilder<ConfigurationEntity, int> FilterBuilder =>
        new()
        {
            PrimaryWhere   = (query, key) => query.Where(c => c.ConfigurationId == key),
            PrimaryWhereIn = (query, keys) => query.Where(item => keys.Contains(item.ConfigurationId))
        };

    public async Task StoreAsync(ConfigurationEntity configuration)
    {
        // search und update machine

        var cInDb = await AddOptionalWhere(TrackingQuery).Where(c => c.UserId == configuration.UserId && c.Group == configuration.Group && c.Name == configuration.Name).FirstOrDefaultAsync();

        if (cInDb == default(ConfigurationEntity))
        {
            // add new

            cInDb = configuration;
            await AddEntityAsync(cInDb);
        }
        else
        {
            // syn with existing
            configuration.ConfigurationId = cInDb.ConfigurationId;
            configuration.UserId          = cInDb.UserId;
            configuration.User            = cInDb.User;
            SetValue(cInDb, configuration);
        }
    }

    public async Task DeleteByUserAsync(int userId)
    {
        var machines = await TrackingQueryWithInclude().Where(m => m.UserId == userId).ToListAsync();
        DeleteEntities(machines);
    }

    #endregion

    #region extra queries

    public async Task<ConfigurationEntity?> GetAsync(int userId, string group, string name)
    {
        return await AddOptionalWhere(Query).Where(c => c.UserId == userId && c.Group == group && c.Name == name).FirstOrDefaultAsync();
    }

    #endregion
}