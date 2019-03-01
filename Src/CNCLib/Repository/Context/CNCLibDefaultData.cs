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
            var userImport = new CsvImport<User>();
            var users      = userImport.Read(DefaultDataDir + @"\DefaultData\User.csv").ToArray();
            return users;
        }

        private void MachineSeed(CNCLibContext context, User[] users)
        {
            var machineImport = new CsvImport<Machine>();
            var machines      = machineImport.Read(DefaultDataDir + @"\DefaultData\Machine.csv").ToArray();

            var machineCommandImport = new CsvImport<MachineCommand>();
            var machineCommands      = machineCommandImport.Read(DefaultDataDir + @"\DefaultData\MachineCommand.csv").ToArray();

            var machineInitCommandImport = new CsvImport<MachineInitCommand>();
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
            var itemImport = new CsvImport<Item>();
            var items      = itemImport.Read(DefaultDataDir + @"\DefaultData\Item.csv").ToArray();

            var itemPropertyImport = new CsvImport<ItemProperty>();
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
                var configurationImport = new CsvImport<Configuration>();
                var configurations      = configurationImport.Read(DefaultDataDir + @"\DefaultData\Configuration.csv").ToArray();

                context.Configurations.AddRange(configurations);
            }
        }
    }
}