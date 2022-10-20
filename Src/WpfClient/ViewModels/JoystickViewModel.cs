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

using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using AutoMapper;

using CNCLib.Service.Abstraction;
using CNCLib.WpfClient.Models;

using Framework.Pattern;
using Framework.Wpf.Helpers;
using Framework.Wpf.ViewModels;

public class JoystickViewModel : BaseViewModel
{
    private readonly IFactory<IJoystickService> _joystickService;
    private readonly IMapper                    _mapper;

    #region crt

    public JoystickViewModel(IFactory<IJoystickService> joystickService, IMapper mapper)
    {
        _joystickService = joystickService;
        _mapper          = mapper;
    }

    public override async Task Loaded()
    {
        await base.Loaded();
        await LoadJoystick();
    }

    #endregion

    #region Properties

    Models.Joystick _currentJoystick = new Models.Joystick();
    int             _id              = -1;

    public Models.Joystick Joystick
    {
        get => _currentJoystick;
        set { SetProperty(() => _currentJoystick == value, () => _currentJoystick = value); }
    }

    #endregion

    #region Operations

    public async Task LoadJoystick()
    {
        using (var scope = _joystickService.Create())
        {
            var joystick = _mapper.Map<Joystick>((await scope.Instance.GetAllAsync()).FirstOrDefault());
            if (joystick == null)
            {
                joystick = new Joystick() { Id = 0, BaudRate = 250000, ComPort = @"com7" };
                _id      = 0;
            }

            Joystick = joystick;
            _id      = Joystick.Id;


            RaisePropertyChanged(nameof(Joystick));
        }
    }

    public async void SaveJoystick()
    {
        using (var scope = _joystickService.Create())
        {
            var joystick = _mapper.Map<Logic.Abstraction.DTO.Joystick>(Joystick);
            if (_id == 0)
            {
                await scope.Instance.AddAsync(joystick);
            }
            else
            {
                await scope.Instance.UpdateAsync(joystick);
            }

            CloseAction();
        }
    }

    public bool CanSaveJoystick()
    {
        return true;
    }

    #endregion

    #region Commands

    public ICommand SaveJoystickCommand => new DelegateCommand(SaveJoystick, CanSaveJoystick);

    #endregion
}