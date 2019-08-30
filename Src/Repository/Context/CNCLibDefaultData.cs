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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using CNCLib.Repository.Abstraction.Entities;

using Framework.Repository.Tools;

namespace CNCLib.Repository.Context
{
    public class CNCLibDefaultData : DbImporter
    {
        private Dictionary<int, User>                            _userMap;
        private Dictionary<int, Machine>                         _machineMap;
        private Dictionary<int, MachineCommand>                  _machineCommandMap;
        private Dictionary<int, MachineInitCommand>              _machineInitMap;
        private Dictionary<int, Item>                            _itemMap;
        private Dictionary<Tuple<int, string>, ItemProperty>     _itemPropertyMap;
        private Dictionary<Tuple<string, string>, Configuration> _configurationMap;

        public CNCLibDefaultData(CNCLibContext context) : base(context)
        {
            CsvDir = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\DefaultData";
        }

        public void CNCSeed(bool isTest)
        {
            _userMap = SeedCsvData<int, User>("User.csv", u => u.UserId, (u, key) => u.UserId = key);

            _machineMap = SeedCsvData<int, Machine>("Machine.csv", m => m.MachineId, (m, key) =>
            {
                m.MachineId = key;
                m.User      = m.UserId.HasValue ? _userMap[m.UserId.Value] : null;
                m.UserId    = null;
            });
            _machineCommandMap = SeedCsvData<int, MachineCommand>("MachineCommand.csv", mc => mc.MachineCommandId, (mc, key) =>
            {
                mc.MachineCommandId = key;
                mc.Machine          = _machineMap[mc.MachineId];
                mc.MachineId        = 0;
            });
            _machineInitMap = SeedCsvData<int, MachineInitCommand>("MachineInitCommand.csv", mic => mic.MachineInitCommandId, (mic, key) =>
            {
                mic.MachineInitCommandId = key;
                mic.Machine              = _machineMap[mic.MachineId];
                mic.MachineId            = 0;
            });

            _itemMap = SeedCsvData<int, Item>("Item.csv", i => i.ItemId, (i, key) =>
            {
                i.ItemId = key;
                i.User   = i.UserId.HasValue ? _userMap[i.UserId.Value] : null;
                i.UserId = null;
            });
            _itemPropertyMap = SeedCsvData<Tuple<int, string>, ItemProperty>("ItemProperty.csv", ip => new Tuple<int, string>(ip.ItemId, ip.Name), (ip, key) =>
            {
                ip.Item   = _itemMap[ip.ItemId];
                ip.ItemId = 0;
            });

            _configurationMap = SeedCsvData< Tuple<string, string>, Configuration >(isTest ? "ConfigurationTest.csv" : "Configuration.csv", c => new Tuple<string, string>(c.Group,c.Name), (c, key) =>
            {
                c.User   = c.UserId.HasValue ? _userMap[c.UserId.Value] : null;
                c.UserId = null;
            });
        }
    }
}
