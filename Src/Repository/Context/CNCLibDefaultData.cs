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

namespace CNCLib.Repository.Context
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    using CNCLib.Repository.Abstraction.Entities;

    public class CNCLibDefaultData : CNCLibDbImporter
    {
        public CNCLibDefaultData(CNCLibContext context) : base(context)
        {
            CsvDir = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\DefaultData";
        }

        public IList<MachineEntity> GetDefaultMachines()
        {
            return Read<MachineEntity>("Machine.csv");
        }

        public IList<ItemEntity> GetDefaultItems()
        {
            return Read<ItemEntity>("Item.csv");
        }

        public IList<UserFileEntity> GetDefaultFiles()
        {
            return Read<UserFileEntity>("UserFile.csv");
        }

        public void ImportForUserMachine(int userId)
        {
            _machineMap = ImportCsv<int, MachineEntity>("Machine.csv", m => m.MachineId, (m, key) =>
            {
                m.MachineId = key;
                m.User      = null;
                m.UserId    = userId;
            });
            _machineCommandMap = ImportCsv<int, MachineCommandEntity>("MachineCommand.csv", mc => mc.MachineCommandId, (mc, key) =>
            {
                mc.MachineCommandId = key;
                mc.Machine          = _machineMap[mc.MachineId];
                mc.MachineId        = 0;
            });
            _machineInitMap = ImportCsv<int, MachineInitCommandEntity>("MachineInitCommand.csv", mic => mic.MachineInitCommandId, (mic, key) =>
            {
                mic.MachineInitCommandId = key;
                mic.Machine              = _machineMap[mic.MachineId];
                mic.MachineId            = 0;
            });
        }

        public void ImportForUserItem(int userId)
        {
            _itemMap = ImportCsv<int, ItemEntity>("Item.csv", i => i.ItemId, (i, key) =>
            {
                i.ItemId = key;
                i.User   = null;
                i.UserId = userId;
            });
            _itemPropertyMap = ImportCsv<Tuple<int, string>, ItemPropertyEntity>("ItemProperty.csv", ip => new Tuple<int, string>(ip.ItemId, ip.Name), (ip, key) =>
            {
                ip.Item   = _itemMap[ip.ItemId];
                ip.ItemId = 0;
            });
        }

        public void ImportForUserFile(int userId)
        {
            _userFileMap = ImportCsv<int, UserFileEntity>("UserFile.csv", uf => uf.UserFileId, (uf, key) =>
            {
                uf.UserFileId = key;
                uf.User       = null;
                uf.UserId     = userId;
            });
        }

        public void ImportForUser(int userId)
        {
            ImportForUserMachine(userId);
            ImportForUserItem(userId);
            ImportForUserFile(userId);
        }

        public void Import()
        {
            _userMap = ImportCsv<int, UserEntity>("User.csv", u => u.UserId, (u, key) => u.UserId = key);

            _machineMap = ImportCsv<int, MachineEntity>("Machine.csv", m => m.MachineId, (m, key) =>
            {
                m.MachineId = key;
                m.User      = _userMap[m.UserId];
                m.UserId    = 0;
            });
            _machineCommandMap = ImportCsv<int, MachineCommandEntity>("MachineCommand.csv", mc => mc.MachineCommandId, (mc, key) =>
            {
                mc.MachineCommandId = key;
                mc.Machine          = _machineMap[mc.MachineId];
                mc.MachineId        = 0;
            });
            _machineInitMap = ImportCsv<int, MachineInitCommandEntity>("MachineInitCommand.csv", mic => mic.MachineInitCommandId, (mic, key) =>
            {
                mic.MachineInitCommandId = key;
                mic.Machine              = _machineMap[mic.MachineId];
                mic.MachineId            = 0;
            });

            _itemMap = ImportCsv<int, ItemEntity>("Item.csv", i => i.ItemId, (i, key) =>
            {
                i.ItemId = key;
                i.User   = _userMap[i.UserId];
                i.UserId = 0;
            });
            _itemPropertyMap = ImportCsv<Tuple<int, string>, ItemPropertyEntity>("ItemProperty.csv", ip => new Tuple<int, string>(ip.ItemId, ip.Name), (ip, key) =>
            {
                ip.Item   = _itemMap[ip.ItemId];
                ip.ItemId = 0;
            });

            _configurationMap = ImportCsv<int, ConfigurationEntity>("Configuration.csv", c => c.ConfigurationId, (c, key) =>
            {
                c.ConfigurationId = key;
                c.User            = _userMap[c.UserId];
                c.UserId          = 0;
            });

            _userFileMap = ImportCsv<int, UserFileEntity>("UserFile.csv", uf => uf.UserFileId, (uf, key) =>
            {
                uf.UserFileId = key;
                uf.User       = _userMap[uf.UserId];
                uf.UserId     = 0;
            });
        }
    }
}