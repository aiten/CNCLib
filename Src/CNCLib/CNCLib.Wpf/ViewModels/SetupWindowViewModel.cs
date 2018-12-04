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
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using AutoMapper;

using CNCLib.Service.Contract;
using CNCLib.Wpf.Helpers;
using CNCLib.Wpf.Models;
using CNCLib.Wpf.Services;

using Framework.Tools;
using Framework.Pattern;
using Framework.Wpf.Helpers;
using Framework.Wpf.ViewModels;

namespace CNCLib.Wpf.ViewModels
{
    public class SetupWindowViewModel : BaseViewModel
    {
        #region crt

        public SetupWindowViewModel(IFactory<IMachineService> machineService, IFactory<IJoystickService> joystickService, IMapper mapper, Global global)
        {
            _machineService = machineService ?? throw new ArgumentNullException();
            _joystickService = joystickService ?? throw new ArgumentNullException();
            _mapper         = mapper ?? throw new ArgumentNullException();
            _global         = global ?? throw new ArgumentNullException(); ;
            ResetOnConnect  = false;
        }

        readonly IFactory<IMachineService> _machineService;
        private readonly IFactory<IJoystickService> _joystickService;
        private readonly IMapper _mapper;
        private readonly Global _global;

        public override async Task Loaded()
        {
            await base.Loaded();
            if (_machines == null)
            {
                await LoadMachines(-1);
                await LoadJoystick();
            }
        }

        #endregion

        #region private operations

        private async Task LoadMachines(int defaultMachineId)
        {
            var machines = new ObservableCollection<Machine>();
            int defaultM;

            using (var scope = _machineService.Create())
            {
                var service = scope.Instance;

                foreach (var m in await service.GetAll())
                {
                    machines.Add(Converter.Convert(m, _mapper));
                }

                defaultM = await service.GetDefaultMachine();
            }

            Machines = machines;

            var defaultMachine = machines.FirstOrDefault(m => m.MachineId == defaultMachineId) ?? machines.FirstOrDefault(m => m.MachineId == defaultM);

            if (defaultMachine == null && machines.Count > 0)
            {
                defaultMachine = machines[0];
            }

            Machine = defaultMachine;
        }

        private async Task LoadJoystick()
        {
            using (var scope = _joystickService.Create())
            {

                Joystick = (await scope.Instance.Load()).Item1;

                _global.Joystick = Joystick;
            }
        }

        #endregion

        #region GUI-forward

        public Action<int> EditMachine  { get; set; }
        public Action      EditJoystick { get; set; }
        public Action      ShowEeprom   { get; set; }

        #endregion

        #region Properties

        #region Current Machine

        public Machine Machine
        {
            get => _selectedMachine;
            set
            {
                SetProperty(ref _selectedMachine, value);
                if (value != null)
                {
                    RaisePropertyChanged(nameof(DtrIsReset));
                    SetGlobal().Ignore();
                }
            }
        }

        public Joystick Joystick { get; set; }

        Machine _selectedMachine;

        private ObservableCollection<Machine> _machines;

        public ObservableCollection<Machine> Machines
        {
            get => _machines;
            set => SetProperty(ref _machines, value);
        }

        public bool Connected => _global.Com.Current.IsConnected;

        public bool ConnectedJoystick => _global.ComJoystick.IsConnected;

        #endregion

        private bool _resetOnConnect = true;

        public bool ResetOnConnect
        {
            get => _resetOnConnect;
            set
            {
                SetProperty(ref _resetOnConnect, value);
                _global.ResetOnConnect = value;
            }
        }

        private bool _sendInitCommands = true;

        public bool SendInitCommands
        {
            get => _sendInitCommands;
            set => SetProperty(ref _sendInitCommands, value);
        }

        public bool DtrIsReset => Machine != null && Machine.DtrIsReset;

        public string CNCLibVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        #endregion

        #region Operations

        public async Task<bool> Connect(CancellationToken ctk)
        {
            try
            {
                string comport = Machine.GetComPort();

                _global.Com.SetCurrent(comport);

                _global.Com.Current.DtrIsReset     = Machine.DtrIsReset;
                _global.Com.Current.ResetOnConnect = ResetOnConnect;

                _global.Com.Current.CommandToUpper = Machine.CommandToUpper;
                _global.Com.Current.BaudRate       = Machine.BaudRate;
                await _global.Com.Current.ConnectAsync(comport);
                await SetGlobal();

                if (SendInitCommands && Machine != null)
                {
                    var initCommands = Machine.MachineInitCommands;

                    if (initCommands.Any())
                    {
                        // wait (do not check if reset - arduino may reset even the "reset" is not specified)
                        await _global.Com.Current.WaitUntilResponseAsync(3000);
                        await _global.Com.Current.QueueCommandsAsync(initCommands.OrderBy(cmd => cmd.SeqNo).Select(e => e.CommandString));
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox?.Invoke("Open serial port failed? " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            RaisePropertyChanged(nameof(Connected));
            CommandManager.InvalidateRequerySuggested();
            return true;
        }

        public async Task<bool> ConnectJoystick(CancellationToken ctk)
        {
            try
            {
                _global.ComJoystick.DtrIsReset     = true;
                _global.ComJoystick.ResetOnConnect = true;
                _global.ComJoystick.CommandToUpper = false;
                _global.ComJoystick.BaudRate       = Joystick.BaudRate;
                await _global.ComJoystick.ConnectAsync(Joystick.ComPort);
            }
            catch (Exception e)
            {
                MessageBox?.Invoke("Open serial port failed? " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            RaisePropertyChanged(nameof(ConnectedJoystick));
            return true;
        }

        private async Task SetGlobal()
        {
            _global.SizeX                         = Machine.SizeX;
            _global.SizeY                         = Machine.SizeY;
            _global.SizeZ                         = Machine.SizeZ;
            _global.Com.Current.ArduinoBufferSize = Machine.BufferSize;

            using (var scope = _machineService.Create())
            {
                _global.Machine = await scope.Instance.Get(Machine.MachineId);
            }
        }

        public bool CanConnect()
        {
            return !Connected && Machine != null;
        }

        public async Task<bool> DisConnect(CancellationToken ctk)
        {
            await _global.Com.Current.DisconnectAsync();
            RaisePropertyChanged(nameof(Connected));
            return true;
        }

        public bool CanDisConnect()
        {
            return Connected;
        }

        public bool CanConnectJoystick()
        {
            return !ConnectedJoystick;
        }

        public async Task<bool> DisConnectJoystick(CancellationToken ctk)
        {
            await _global.ComJoystick.DisconnectAsync();
            RaisePropertyChanged(nameof(ConnectedJoystick));
            return true;
        }

        public bool CanDisConnectJoystick()
        {
            return ConnectedJoystick;
        }

        public async void SetupMachine()
        {
            int mId = Machine?.MachineId ?? -1;

            //EditMachine?.Invoke(mId);
            EditMachine(mId);
            await LoadMachines(mId);
        }

        public async void SetupJoystick()
        {
            EditJoystick?.Invoke();
            await LoadJoystick();
        }

        public void SetDefaultMachine()
        {
            if (Machine != null)
            {
                using (var scope = _machineService.Create())
                {
                    scope.Instance.SetDefaultMachine(Machine.MachineId);
                }
            }
        }

        public bool CanSetupMachine()
        {
            return !Connected;
        }

        public bool CanShowPaint()
        {
            return Connected;
        }

        public bool CanSetupJoystick()
        {
            return !ConnectedJoystick;
        }

        public void SetEeprom()
        {
            ShowEeprom?.Invoke();
        }

        #endregion

        #region Commands

        public ICommand SetupMachineCommand      => new DelegateCommand(SetupMachine, CanSetupMachine);
        public ICommand ConnectCommand           => new DelegateCommandAsync<bool>(Connect,    CanConnect);
        public ICommand DisConnectCommand        => new DelegateCommandAsync<bool>(DisConnect, CanDisConnect);
        public ICommand EepromCommand            => new DelegateCommand(SetEeprom,         CanDisConnect);
        public ICommand SetDefaultMachineCommand => new DelegateCommand(SetDefaultMachine, CanSetupMachine);
        public ICommand ConnectJoystickCommand   => new DelegateCommandAsync<bool>(ConnectJoystick, CanConnectJoystick);

        public ICommand DisConnectJoystickCommand =>
            new DelegateCommandAsync<bool>(DisConnectJoystick, CanDisConnectJoystick);

        public ICommand SetupJoystickCommand => new DelegateCommand(SetupJoystick, CanSetupJoystick);

        #endregion
    }
}