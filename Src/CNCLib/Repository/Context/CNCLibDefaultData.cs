/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2019 Herbert Aitenbichler

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

using System.IO;
using System.Linq;
using System.Reflection;

using CNCLib.Repository.Contract.Entities;

using Framework.Tools.Tools;

namespace CNCLib.Repository.Context
{
    public class CNCLibDefaultData
    {
        private string DefaultDataDir => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public void CNCSeed(CNCLibContext context, bool isTest)
        {
            var users = UserSeed(context);
            MachineSeed(context, users);
            ItemSeed(context);
            ConfigurationSeed(context, isTest);

            context.Users.AddRange(users);
        }

        private User[] UserSeed(CNCLibContext context)
        {
            var userImport = new CSVImport<User>();
            var users      = userImport.Read(DefaultDataDir + @"\DefaultData\User.csv").ToArray();
            return users;
        }

        private void MachineSeed(CNCLibContext context, User[] users)
        {
            var machineImport = new CSVImport<Machine>();
            var machines      = machineImport.Read(DefaultDataDir + @"\DefaultData\Machine.csv").ToArray();

            var machineCommandImport = new CSVImport<MachineCommand>();
            var machineCommands      = machineCommandImport.Read(DefaultDataDir + @"\DefaultData\MachineCommand.csv").ToArray();

            var machineInitCommandImport = new CSVImport<MachineInitCommand>();
            var machineInitCommands      = machineInitCommandImport.Read(DefaultDataDir + @"\DefaultData\MachineInitCommand.csv").ToArray();

            foreach (var machineInitCommand in machineInitCommands)
            {
                machineInitCommand.Machine              = machines.First(m => m.MachineId == machineInitCommand.MachineId);
                machineInitCommand.MachineId            = 0;
                machineInitCommand.MachineInitCommandId = 0;
            }

            foreach (var machineCommand in machineCommands)
            {
                machineCommand.Machine          = machines.First(m => m.MachineId == machineCommand.MachineId);
                machineCommand.MachineId        = 0;
                machineCommand.MachineCommandId = 0;
            }

            foreach (var machine in machines)
            {
                machine.User      = users.FirstOrDefault(u => u.UserId == machine.UserId);
                machine.UserId    = null;
                machine.MachineId = 0;
            }

            foreach (var user in users)
            {
                user.UserId = 0;
            }

            context.Machines.AddRange(machines);
            context.MachineCommands.AddRange(machineCommands);
            context.MachineInitCommands.AddRange(machineInitCommands);
        }

        private void ItemSeed(CNCLibContext context)
        {
            var itemImport = new CSVImport<Item>();
            var items      = itemImport.Read(DefaultDataDir + @"\DefaultData\Item.csv").ToArray();

            var itemPropertyImport = new CSVImport<ItemProperty>();
            var itemProperties     = itemPropertyImport.Read(DefaultDataDir + @"\DefaultData\ItemProperty.csv").ToArray();

            foreach (var itemProperty in itemProperties)
            {
                itemProperty.Item   = items.First(i => i.ItemId == itemProperty.ItemId);
                itemProperty.ItemId = 0;
            }

            foreach (var item in items)
            {
                item.ItemId = 0;
            }

            context.Items.AddRange(items);
            context.ItemProperties.AddRange(itemProperties);
        }

        private void ConfigurationSeed(CNCLibContext context, bool isTest)
        {
            if (isTest)
            {
                var configurationImport = new CSVImport<Configuration>();
                var configurations      = configurationImport.Read(DefaultDataDir + @"\DefaultData\Configuration.csv").ToArray();

                context.Configurations.AddRange(configurations);
            }
        }
    }
}