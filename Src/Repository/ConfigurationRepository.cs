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

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using CNCLib.Repository.Abstraction;
using CNCLib.Repository.Abstraction.Entities;
using CNCLib.Repository.Context;
using CNCLib.Shared;

using Framework.Repository;
using Framework.Repository.Linq;

using Microsoft.EntityFrameworkCore;

namespace CNCLib.Repository
{
    public class ConfigurationRepository : CRUDRepository<CNCLibContext, Configuration, ConfigurationPrimary>, IConfigurationRepository
    {
        private readonly ICNCLibUserContext _userContext;

        #region ctr/default/overrides

        public ConfigurationRepository(CNCLibContext dbContext, ICNCLibUserContext userContext) : base(dbContext)
        {
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        }

        protected override FilterBuilder<Configuration, ConfigurationPrimary> FilterBuilder =>
            new FilterBuilder<Configuration, ConfigurationPrimary>()
            {
                PrimaryWhere = (query, key) => query.Where(c => c.Group == key.Group && c.Name == key.Name),
                PrimaryWhereIn = (query, keys) =>
                {
                    var predicate = PredicateBuilder.New<Configuration>();

                    foreach (var key in keys)
                    {
                        predicate = predicate.Or(c => c.Group == key.Group && c.Name == key.Name);
                    }

                    return query.Where(predicate);
                }
            };

        protected override IQueryable<Configuration> AddInclude(IQueryable<Configuration> query)
        {
            return query;
        }

        protected override IQueryable<Configuration> AddOptionalWhere(IQueryable<Configuration> query)
        {
            if (_userContext.UserId.HasValue)
            {
                return query.Where(x => x.UserId.HasValue == false || x.UserId.Value == _userContext.UserId.Value);
            }

            return base.AddOptionalWhere(query);
        }

        public async Task Store(Configuration configuration)
        {
            // search und update machine

            var cInDb = await AddOptionalWhere(TrackingQuery).Where(c => c.Group == configuration.Group && c.Name == configuration.Name).FirstOrDefaultAsync();

            if (cInDb == default(Configuration))
            {
                // add new

                cInDb        = configuration;
                cInDb.UserId = _userContext.UserId;
                AddEntity(cInDb);
            }
            else
            {
                // syn with existing
                configuration.UserId = cInDb.UserId;
                configuration.User   = cInDb.User;
                SetValue(cInDb, configuration);
            }
        }

        #endregion

        #region extra queries

        public async Task<Configuration> Get(string group, string name)
        {
            return await AddOptionalWhere(Query).Where(c => c.Group == group && c.Name == name).FirstOrDefaultAsync();
        }

        #endregion
    }
}