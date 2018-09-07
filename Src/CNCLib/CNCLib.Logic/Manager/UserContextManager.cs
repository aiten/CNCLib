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

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using CNCLib.Logic.Contracts;
using Framework.Logic;

namespace CNCLib.Logic
{
    public class UserContextManager : ManagerBase, IUserContextManager
    {
/*
        private IEmployeeManager _employeeManager;

        public UserContextManager(IStoreManager storeManager, IEmployeeManager employeeManager)
        {
            _storeManager = storeManager;
            _employeeManager = employeeManager;
        }

        private const int MaxCacheAgeInMin = 5;
        private static IDictionary<string, UserContextData> _cache = new Dictionary<string, UserContextData>();

        public static void ResetCache()
        {
            _cache.Clear();
        }

        public async Task<UserContextData> GetUserContext(string userName)
        {
            UserContextData existingentry;
            if (_cache.TryGetValue(userName, out existingentry))
            {
                TimeSpan age = DateTime.Now - existingentry.CacheTime;
                if (age.TotalMinutes < MaxCacheAgeInMin)
                    return existingentry;
            }

            var newentry = await InitUserContext(userName);
            _cache[userName] = newentry;
            return newentry;
        }

        private async Task<UserContextData> InitUserContext(string userName)
        {
            var entry = new UserContextData();
            entry.UserName = userName;
            entry.CacheTime = DateTime.Now;

            using (var context = new PrincipalContext(ContextType.Domain))
            {
                var usr = UserPrincipal.FindByIdentity(context, userName);
                if (usr != null)
                {
                    var login = SplitUserName(userName);

                    int adsEmployeeId;
                    if (!string.IsNullOrEmpty(usr.EmployeeId) && int.TryParse(usr.EmployeeId, out adsEmployeeId))
                    {
                        entry.Employee = await _employeeManager.GetUserByADSEmployeeId(adsEmployeeId);

                        if (entry.Employee != null)
                        {
                            //Check for IsRVL or IsLVK

                            var rvlDivNo = await _storeManager.GetRvlDiv(entry.Employee.Id);
                            if (rvlDivNo.Count() > 0)
                                entry.IsRvl = true;

                            var lvkDivNo = await _storeManager.GetLvkDiv(entry.Employee.Id);
                            if (lvkDivNo.Count() > 0)
                                entry.IsLvk = true;
                        }
                    }

                    // Check for GF, Global, Admind => with ADS
                    GroupPrincipal group = GroupPrincipal.FindByIdentity(context, "Test");

                }
            }

            return entry;
        }
*/
    }
}
