////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2014 Herbert Aitenbichler

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
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using AutoMapper;

using CNCLib.Service.Contract;
using CNCLib.Wpf.Helpers;
using CNCLib.Wpf.Models;

using Framework.Wpf.Helpers;
using Framework.Wpf.ViewModels;
using Framework.Arduino.SerialCommunication;
using Framework.Pattern;

using MachineDto = CNCLib.Logic.Contract.DTO.Machine;

namespace CNCLib.Wpf.ViewModels
{
    public class MachineViewModel : BaseViewModel, IDisposable
    {
        #region crt

        public MachineViewModel(IFactory<IMachineService> machineService, IMapper mapper)
        {
            _machineService = machineService ?? throw new ArgumentNullException();
            _mapper = mapper ?? throw new ArgumentNullException();
            AddNewMachine   = false;
        }

        readonly IFactory<IMachineService> _machineService;
        private readonly IMapper _mapper;

        #endregion

        #region dispose

        public void Dispose()
        {
        }

        #endregion

        #region Properties

        Machine _currentMachine = new Machine();

        public Machine Machine
        {
            get => _currentMachine;
            set { SetProperty(() => _currentMachine == value, () => _currentMachine = value); }
        }

        public ObservableCollection<MachineCommand> MachineCommands => _currentMachine.MachineCommands;

        public ObservableCollection<MachineInitCommand> MachineInitCommands => _currentMachine.MachineInitCommands;

        public bool AddNewMachine { get; set; }

        #endregion

        #region Operations

        public async Task LoadMachine(int machineId)
        {
            using (var scope = _machineService.Create())
            {
                await MyLoadMachine(machineId, scope);
            }
        }

        private async Task MyLoadMachine(int machineId, IScope<IMachineService> scope)
        {
            MachineDto dto;
            AddNewMachine = machineId <= 0;
            if (AddNewMachine)
            {
                dto = await scope.Instance.DefaultMachine();
            }
            else
            {
                dto = await scope.Instance.Get(machineId);
            }

            Machine = dto.Convert(_mapper);

            RaisePropertyChanged(nameof(Machine));

            RaisePropertyChanged(nameof(MachineCommands));
            RaisePropertyChanged(nameof(MachineInitCommands));
        }

        public async void SaveMachine()
        {
            var m = _currentMachine.Convert(_mapper);

            using (var scope = _machineService.Create())
            {
                int id = m.MachineId;
                if (id == default(int))
                {
                    id = await scope.Instance.Add(m);
                }
                else
                {
                    await scope.Instance.Update(m);
                }

                await MyLoadMachine(id, scope);
            }

            CloseAction();
        }

        public bool CanSaveMachine()
        {
            return true;
        }

        public async void DeleteMachine()
        {
            using (var scope = _machineService.Create())
            {
                await scope.Instance.Delete(_currentMachine.Convert(_mapper));
            }

            CloseAction();
        }

        public bool CanDeleteMachine()
        {
            return !AddNewMachine;
        }

        public async void AddMachine()
        {
            using (var scope = _machineService.Create())
            {
                await MyLoadMachine(-1, scope);
            }
        }

        public bool CanAddMachine()
        {
            return !AddNewMachine;
        }

        public async void ReadFromMachine()
        {
            if (MessageBox?.Invoke("Read configuration from machine?", "CNCLib", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                try
                {
                    string comport = Machine.GetComPort();

                    Global.Instance.Com.SetCurrent(comport);
                    Global.Instance.Com.Current.DtrIsReset     = Machine.DtrIsReset;
                    Global.Instance.Com.Current.ResetOnConnect = Global.Instance.ResetOnConnect;
                    Global.Instance.Com.Current.CommandToUpper = Machine.CommandToUpper;
                    Global.Instance.Com.Current.BaudRate       = Machine.BaudRate;
                    await Global.Instance.Com.Current.ConnectAsync(comport);

                    await Global.Instance.Com.Current.SendCommandAsync("?", 3000);
                    await Task.Delay(100);

                    var eeprom = await new EepromHelper().ReadEepromAsync();
                    if (eeprom != null)
                    {
                        Machine.Coolant   = eeprom.HasCoolant;
                        Machine.Rotate    = eeprom.CanRotate;
                        Machine.Spindle   = eeprom.HasSpindle;
                        Machine.SDSupport = eeprom.HasSD;
                        Machine.Rotate    = eeprom.CanRotate;
                        Machine.Coolant   = eeprom.HasCoolant;
                        Machine.Laser     = eeprom.IsLaser;
                        Machine.Axis      = (int)eeprom.UseAxis;

                        Machine.SizeX = eeprom.GetAxis(0).Size / 1000m;
                        Machine.SizeY = eeprom.GetAxis(1).Size / 1000m;
                        Machine.SizeZ = eeprom.GetAxis(2).Size / 1000m;
                        Machine.SizeA = eeprom.GetAxis(3).Size / 1000m;

                        Machine.CommandSyntax = eeprom.CommandSyntax;

                        var orig = Machine;
                        Machine = null;
                        Machine = orig;
                    }
                }
                catch (Exception e)
                {
                    MessageBox?.Invoke("Open serial port failed? " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    await Global.Instance.Com.Current.DisconnectAsync();
                }
            }
        }

        public bool CanReadFromMachine()
        {
            return true;
        }

        #endregion

        #region Commands

        public ICommand SaveMachineCommand   => new DelegateCommand(SaveMachine,   CanSaveMachine);
        public ICommand DeleteMachineCommand => new DelegateCommand(DeleteMachine, CanDeleteMachine);
        public ICommand AddMachineCommand    => new DelegateCommand(AddMachine,    CanAddMachine);

        public ICommand ReadFromMachineCommand => new DelegateCommand(ReadFromMachine, CanReadFromMachine);

        #endregion
    }
}