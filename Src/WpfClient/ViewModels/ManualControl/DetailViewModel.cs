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

namespace CNCLib.WpfClient.ViewModels.ManualControl;

using System;

using CNCLib.Logic.Abstraction.DTO;

using Framework.Wpf.ViewModels;

public class DetailViewModel : BaseViewModel
{
    private readonly Global                  _global;
    protected        IManualControlViewModel Vm { get; set; }

    public DetailViewModel(IManualControlViewModel vm, Global global)
    {
        _global = global;
        Vm      = vm;
    }

    public bool Connected => _global.Com.Current.IsConnected;

    protected void RunInNewTask(Action todo)
    {
        Vm.RunInNewTask(todo);
    }

    protected void RunAndUpdate(Action todo)
    {
        Vm.RunAndUpdate(todo);
    }

    protected void SetPositions(decimal[] positions, int positionIdx)
    {
        Vm.SetPositions(positions, positionIdx);
    }

    #region Command/CanCommand

    public bool CanSend()
    {
        return Connected;
    }

    public bool CanSendGCode()
    {
        return CanSend() && _global.Machine?.CommandSyntax != CommandSyntax.Hpgl;
    }

    #endregion
}