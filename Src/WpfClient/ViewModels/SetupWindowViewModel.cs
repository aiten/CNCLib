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

namespace CNCLib.WpfClient.ViewModels;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using AutoMapper;

using CNCLib.Service.Abstraction;
using CNCLib.Shared;
using CNCLib.WpfClient.Models;

using Framework.Pattern;
using Framework.Tools;
using Framework.Wpf.Helpers;
using Framework.Wpf.ViewModels;

public class SetupWindowViewModel : BaseViewModel
{
    #region crt

    public SetupWindowViewModel(IFactory<IMachineService> machineService, IFactory<IJoystickService> joystickService, IMapper mapper, Global global, ICNCLibUserContext userContext)
    {
        _machineService  = machineService;
        _joystickService = joystickService;
        _mapper          = mapper;
        _global          = global;
        _userContext     = userContext;

        ResetOnConnect = false;
    }

    private readonly IFactory<IMachineService>  _machineService;
    private readonly IFactory<IJoystickService> _joystickService;
    private readonly IMapper                    _mapper;
    private readonly Global                     _global;
    private readonly ICNCLibUserContext         _userContext;

    public override async Task Loaded()
    {
        await base.Loaded();
        if (_machines.Count == 0)
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

            foreach (var m in await service.GetAllAsync())
            {
                machines.Add(Converter.Convert(m, _mapper));
            }

            defaultM = await service.GetDefault();
        }

        Machines = machines;

        var defaultMachine = machines.FirstOrDefault(m => m.MachineId == defaultMachineId) ?? machines.FirstOrDefault(m => m.MachineId == defaultM);

        if (defaultMachine == null && machines.Count > 0)
        {
            defaultMachine = machines[0];
        }

        Machine = defaultMachine!;
    }

    private async Task LoadJoystick()
    {
        using (var scope = _joystickService.Create())
        {
            Joystick = _mapper.Map<Joystick>((await scope.Instance.GetAllAsync()).FirstOrDefault());

            _global.Joystick = Joystick;
        }
    }

    #endregion

    #region GUI-forward

    public Action<int>? EditMachine  { get; set; }
    public Action?      EditJoystick { get; set; }
    public Action?      ShowEeprom   { get; set; }

    public Func<Tuple<string, string>?>? Login { get; set; }

    #endregion

    #region Properties

    #region Current Machine

    public Machine? Machine
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

    public Joystick? Joystick { get; set; }

    Machine? _selectedMachine;

    private ObservableCollection<Machine> _machines = new ObservableCollection<Machine>();

    public ObservableCollection<Machine> Machines
    {
        get => _machines;
        set
        {
            _machines.Clear();
            foreach (var m in value)
            {
                _machines.Add(m);
            }
        }
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

    public string CNCLibVersion => Assembly.GetExecutingAssembly().GetName().Version!.ToString();

    public string? UserName => _userContext.UserName;

    #endregion

    #region Operations

    public async Task<bool> Connect(CancellationToken ctk)
    {
        try
        {
            _global.Com.SetCurrent(Machine!.SerialServer!);

            _global.Com.Current.DtrIsReset     = Machine.DtrIsReset;
            _global.Com.Current.ResetOnConnect = ResetOnConnect;

            _global.Com.Current.CommandToUpper = Machine.CommandToUpper;
            _global.Com.Current.BaudRate       = Machine.BaudRate;
            await _global.Com.Current.ConnectAsync(Machine.ComPort!, Machine.SerialServer, Machine.SerialServerUser, Machine.SerialServerPassword);
            await SetGlobal();

            if (SendInitCommands && Machine != null)
            {
                var initCommands = Machine.MachineInitCommands!;

                if (initCommands.Any())
                {
                    // wait (do not check if reset - arduino may reset even the "reset" is not specified)
                    await _global.Com.Current.WaitUntilResponseAsync(3000);
                    await _global.Com.Current.QueueCommandsAsync(initCommands.OrderBy(cmd => cmd.SeqNo).Select(e => e.CommandString!));
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
            _global.ComJoystick.BaudRate       = Joystick!.BaudRate;
            await _global.ComJoystick.ConnectAsync(Joystick.ComPort!, null, null, null);
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
        _global.SizeX                         = Machine!.SizeX;
        _global.SizeY                         = Machine.SizeY;
        _global.SizeZ                         = Machine.SizeZ;
        _global.Com.Current.ArduinoBufferSize = Machine.BufferSize;

        using (var scope = _machineService.Create())
        {
            _global.Machine = await scope.Instance.GetAsync(Machine.MachineId);
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
        EditMachine?.Invoke(mId);
        await LoadMachines(mId);
    }

    public async void SetupJoystick()
    {
        EditJoystick?.Invoke();
        await LoadJoystick();
    }

    public async void SetDefaultMachine()
    {
        if (Machine != null)
        {
            using (var scope = _machineService.Create())
            {
                await scope.Instance.SetDefault(Machine.MachineId);
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

    public async Task<bool> LoginUser(CancellationToken tx)
    {
        var newUser = Login?.Invoke();
        if (newUser != null && !string.IsNullOrEmpty(newUser.Item1))
        {
            var userContextRW = (_userContext as CNCLibUserContext)!;
            await userContextRW.InitUserContext(newUser.Item1, newUser.Item2);
            await LoadMachines(-1);
            RaisePropertyChanged(nameof(UserName));
        }

        return true;
    }

    public bool CanLoginUser()
    {
        return !Connected;
    }

    #endregion

    #region Commands

    public ICommand SetupMachineCommand       => new DelegateCommand(SetupMachine, CanSetupMachine);
    public ICommand ConnectCommand            => new DelegateCommandAsync<bool>(Connect,    CanConnect);
    public ICommand DisConnectCommand         => new DelegateCommandAsync<bool>(DisConnect, CanDisConnect);
    public ICommand EepromCommand             => new DelegateCommand(SetEeprom, CanDisConnect);
    public ICommand LoginCommand              => new DelegateCommandAsync<bool>(LoginUser, CanLoginUser);
    public ICommand SetDefaultMachineCommand  => new DelegateCommand(SetDefaultMachine, CanSetupMachine);
    public ICommand ConnectJoystickCommand    => new DelegateCommandAsync<bool>(ConnectJoystick,    CanConnectJoystick);
    public ICommand DisConnectJoystickCommand => new DelegateCommandAsync<bool>(DisConnectJoystick, CanDisConnectJoystick);
    public ICommand SetupJoystickCommand      => new DelegateCommand(SetupJoystick, CanSetupJoystick);

    #endregion
}