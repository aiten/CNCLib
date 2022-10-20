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

using System.Linq;
using System.Windows.Input;

using CNCLib.Logic.Abstraction.DTO;
using CNCLib.WpfClient.Helpers;

using Framework.Wpf.Helpers;

public class CustomViewModel : DetailViewModel
{
    private readonly Global _global;

    public CustomViewModel(IManualControlViewModel vm, Global global) : base(vm, global)
    {
        _global = global;

        _global.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == "Machine")
            {
                MachineChanged();
            }
        };
    }

    #region Properties

    public string Desc00 => GetDesc(0, 0);
    public string Desc01 => GetDesc(0, 1);
    public string Desc02 => GetDesc(0, 2);
    public string Desc03 => GetDesc(0, 3);
    public string Desc04 => GetDesc(0, 4);
    public string Desc10 => GetDesc(1, 0);
    public string Desc11 => GetDesc(1, 1);
    public string Desc12 => GetDesc(1, 2);
    public string Desc13 => GetDesc(1, 3);
    public string Desc14 => GetDesc(1, 4);
    public string Desc20 => GetDesc(2, 0);
    public string Desc21 => GetDesc(2, 1);
    public string Desc22 => GetDesc(2, 2);
    public string Desc23 => GetDesc(2, 3);
    public string Desc24 => GetDesc(2, 4);
    public string Desc30 => GetDesc(3, 0);
    public string Desc31 => GetDesc(3, 1);
    public string Desc32 => GetDesc(3, 2);
    public string Desc33 => GetDesc(3, 3);
    public string Desc34 => GetDesc(3, 4);
    public string Desc40 => GetDesc(4, 0);
    public string Desc41 => GetDesc(4, 1);
    public string Desc42 => GetDesc(4, 2);
    public string Desc43 => GetDesc(4, 3);
    public string Desc44 => GetDesc(4, 4);
    public string Desc50 => GetDesc(5, 0);
    public string Desc51 => GetDesc(5, 1);
    public string Desc52 => GetDesc(5, 2);
    public string Desc53 => GetDesc(5, 3);
    public string Desc54 => GetDesc(5, 4);
    public string Desc60 => GetDesc(6, 0);
    public string Desc61 => GetDesc(6, 1);
    public string Desc62 => GetDesc(6, 2);
    public string Desc63 => GetDesc(6, 3);
    public string Desc64 => GetDesc(6, 4);

    #endregion

    private void MachineChanged()
    {
        RaisePropertyChanged(nameof(Desc00));
        RaisePropertyChanged(nameof(Desc01));
        RaisePropertyChanged(nameof(Desc02));
        RaisePropertyChanged(nameof(Desc03));
        RaisePropertyChanged(nameof(Desc04));

        RaisePropertyChanged(nameof(Desc10));
        RaisePropertyChanged(nameof(Desc11));
        RaisePropertyChanged(nameof(Desc12));
        RaisePropertyChanged(nameof(Desc13));
        RaisePropertyChanged(nameof(Desc14));

        RaisePropertyChanged(nameof(Desc20));
        RaisePropertyChanged(nameof(Desc21));
        RaisePropertyChanged(nameof(Desc22));
        RaisePropertyChanged(nameof(Desc23));
        RaisePropertyChanged(nameof(Desc24));

        RaisePropertyChanged(nameof(Desc30));
        RaisePropertyChanged(nameof(Desc31));
        RaisePropertyChanged(nameof(Desc32));
        RaisePropertyChanged(nameof(Desc33));
        RaisePropertyChanged(nameof(Desc34));

        RaisePropertyChanged(nameof(Desc40));
        RaisePropertyChanged(nameof(Desc41));
        RaisePropertyChanged(nameof(Desc42));
        RaisePropertyChanged(nameof(Desc43));
        RaisePropertyChanged(nameof(Desc44));

        RaisePropertyChanged(nameof(Desc50));
        RaisePropertyChanged(nameof(Desc51));
        RaisePropertyChanged(nameof(Desc52));
        RaisePropertyChanged(nameof(Desc53));
        RaisePropertyChanged(nameof(Desc54));

        RaisePropertyChanged(nameof(Desc60));
        RaisePropertyChanged(nameof(Desc61));
        RaisePropertyChanged(nameof(Desc62));
        RaisePropertyChanged(nameof(Desc63));
        RaisePropertyChanged(nameof(Desc64));
    }

    private MachineCommand GetCmd(int x, int y)
    {
        if (_global.Machine == null || _global.Machine.MachineCommands == null)
        {
            return null;
        }

        return _global.Machine.MachineCommands.FirstOrDefault(m => m.PosX == x && m.PosY == y);
    }

    private string GetDesc(int x, int y)
    {
        MachineCommand cmd = GetCmd(x, y);
        return cmd == null ? "" : cmd.CommandName;
    }

    #region Commands / CanCommands

    public bool CanSendXY(int x, int y)
    {
        var cmd = GetCmd(x, y);
        return cmd != null && string.IsNullOrEmpty(cmd.CommandString) == false && CanSend();
    }

    public void SendXY(int x, int y)
    {
        RunAndUpdate(
            async () =>
            {
                var cmd = GetCmd(x, y);
                if (cmd != null)
                {
                    await _global.Com.Current.SendMacroCommandAsync(_global.Machine, cmd.CommandString);
                }
            });
    }

    #endregion

    #region ICommand

    public ICommand Send00Command => new DelegateCommand(() => SendXY(0, 0), () => CanSendXY(0, 0));
    public ICommand Send01Command => new DelegateCommand(() => SendXY(0, 1), () => CanSendXY(0, 1));
    public ICommand Send02Command => new DelegateCommand(() => SendXY(0, 2), () => CanSendXY(0, 2));
    public ICommand Send03Command => new DelegateCommand(() => SendXY(0, 3), () => CanSendXY(0, 3));
    public ICommand Send04Command => new DelegateCommand(() => SendXY(0, 4), () => CanSendXY(0, 4));
    public ICommand Send10Command => new DelegateCommand(() => SendXY(1, 0), () => CanSendXY(1, 0));
    public ICommand Send11Command => new DelegateCommand(() => SendXY(1, 1), () => CanSendXY(1, 1));
    public ICommand Send12Command => new DelegateCommand(() => SendXY(1, 2), () => CanSendXY(1, 2));
    public ICommand Send13Command => new DelegateCommand(() => SendXY(1, 3), () => CanSendXY(1, 3));
    public ICommand Send14Command => new DelegateCommand(() => SendXY(1, 4), () => CanSendXY(1, 4));
    public ICommand Send20Command => new DelegateCommand(() => SendXY(2, 0), () => CanSendXY(2, 0));
    public ICommand Send21Command => new DelegateCommand(() => SendXY(2, 1), () => CanSendXY(2, 1));
    public ICommand Send22Command => new DelegateCommand(() => SendXY(2, 2), () => CanSendXY(2, 2));
    public ICommand Send23Command => new DelegateCommand(() => SendXY(2, 3), () => CanSendXY(2, 3));
    public ICommand Send24Command => new DelegateCommand(() => SendXY(2, 4), () => CanSendXY(2, 4));
    public ICommand Send30Command => new DelegateCommand(() => SendXY(3, 0), () => CanSendXY(3, 0));
    public ICommand Send31Command => new DelegateCommand(() => SendXY(3, 1), () => CanSendXY(3, 1));
    public ICommand Send32Command => new DelegateCommand(() => SendXY(3, 2), () => CanSendXY(3, 2));
    public ICommand Send33Command => new DelegateCommand(() => SendXY(3, 3), () => CanSendXY(3, 3));
    public ICommand Send34Command => new DelegateCommand(() => SendXY(3, 4), () => CanSendXY(3, 4));
    public ICommand Send40Command => new DelegateCommand(() => SendXY(4, 0), () => CanSendXY(4, 0));
    public ICommand Send41Command => new DelegateCommand(() => SendXY(4, 1), () => CanSendXY(4, 1));
    public ICommand Send42Command => new DelegateCommand(() => SendXY(4, 2), () => CanSendXY(4, 2));
    public ICommand Send43Command => new DelegateCommand(() => SendXY(4, 3), () => CanSendXY(4, 3));
    public ICommand Send44Command => new DelegateCommand(() => SendXY(4, 4), () => CanSendXY(4, 4));
    public ICommand Send50Command => new DelegateCommand(() => SendXY(5, 0), () => CanSendXY(5, 0));
    public ICommand Send51Command => new DelegateCommand(() => SendXY(5, 1), () => CanSendXY(5, 1));
    public ICommand Send52Command => new DelegateCommand(() => SendXY(5, 2), () => CanSendXY(5, 2));
    public ICommand Send53Command => new DelegateCommand(() => SendXY(5, 3), () => CanSendXY(5, 3));
    public ICommand Send54Command => new DelegateCommand(() => SendXY(5, 4), () => CanSendXY(5, 4));
    public ICommand Send60Command => new DelegateCommand(() => SendXY(6, 0), () => CanSendXY(6, 0));
    public ICommand Send61Command => new DelegateCommand(() => SendXY(6, 1), () => CanSendXY(6, 1));
    public ICommand Send62Command => new DelegateCommand(() => SendXY(6, 2), () => CanSendXY(6, 2));
    public ICommand Send63Command => new DelegateCommand(() => SendXY(6, 3), () => CanSendXY(6, 3));
    public ICommand Send64Command => new DelegateCommand(() => SendXY(6, 4), () => CanSendXY(6, 4));

    #endregion
}