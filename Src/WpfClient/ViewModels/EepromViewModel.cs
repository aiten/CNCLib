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

namespace CNCLib.WpfClient.ViewModels
{
    using System.Windows;
    using System.Windows.Input;

    using CNCLib.GCode.Machine;
    using CNCLib.GCode.Serial;

    using Framework.Wpf.Helpers;
    using Framework.Wpf.ViewModels;

    public class EepromViewModel : BaseViewModel
    {
        private readonly Global _global;

        #region crt

        bool _validReadEeprom;

        public EepromViewModel(Global global)
        {
            _global = global;
        }

        #endregion

        #region Properties

        private Eeprom _eeprom = new Eeprom();

        public Eeprom EepromValue
        {
            get => _eeprom;
            set { SetProperty(() => _eeprom == value, () => _eeprom = value); }
        }

        #endregion

        #region Operations

        private void MessageRestart()
        {
            MessageBox?.Invoke("Done! Please restart machine!", "CNCLib", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public async void WriteEeprom()
        {
            if (MessageBox?.Invoke("Send 'Write EEprom commands' to machine?", "CNCLib", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                if (await _global.Com.Current.WriteEeprom(EepromValue))
                {
                    MessageRestart();
                    CloseAction();
                }
            }
        }

        public async void ReadEeprom()
        {
            var eeprom = await _global.Com.Current.ReadEeprom();
            if (eeprom != null)
            {
                _validReadEeprom = true;
                EepromValue      = eeprom;
            }
            else
            {
                EepromValue = new Eeprom();
            }
        }

        public async void EraseEeprom()
        {
            if (MessageBox?.Invoke("Send 'Erase EEprom command' to machine?", "CNCLib", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                if (await _global.Com.Current.EraseEeprom())
                {
                    MessageRestart();
                    CloseAction();
                }
            }
        }

        public bool CanReadEeprom()
        {
            return _global.Com.Current.IsConnected;
        }

        public bool CanWriteEeprom()
        {
            return _global.Com.Current.IsConnected && _validReadEeprom;
        }

        public bool CanEraseEeprom()
        {
            return _global.Com.Current.IsConnected;
        }

        #endregion

        #region Commands

        public ICommand ReadEepromCommand  => new DelegateCommand(ReadEeprom,  CanReadEeprom);
        public ICommand WriteEepromCommand => new DelegateCommand(WriteEeprom, CanWriteEeprom);
        public ICommand EraseEepromCommand => new DelegateCommand(EraseEeprom, CanEraseEeprom);

        #endregion
    }
}