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

using System;
using System.Windows;
using System.Windows.Input;

using CNCLib.WpfClient.Helpers;
using CNCLib.WpfClient.Models;

using Framework.Wpf.Helpers;
using Framework.Wpf.ViewModels;

namespace CNCLib.WpfClient.ViewModels
{
    public class EepromViewModel : BaseViewModel, IDisposable
    {
        private readonly Global _global;

        #region crt

        bool _validReadEeprom;

        public EepromViewModel(Global global)
        {
            _global = global ?? throw new ArgumentNullException();
        }

        #endregion

        #region dispose

        public void Dispose()
        {
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
                if (await _global.Com.Current.WriteEepromAsync(EepromValue))
                {
                    MessageRestart();
                    CloseAction();
                }
            }
        }

        public async void ReadEeprom()
        {
            var eeprom = await _global.Com.Current.ReadEepromAsync();
            if (eeprom != null)
            {
                _validReadEeprom = true;
                EepromValue      = eeprom;
            }
            else
            {
                EepromValue = Eeprom.Create(0, 0);
            }
        }

        public async void EraseEeprom()
        {
            if (MessageBox?.Invoke("Send 'Erase EEprom command' to machine?", "CNCLib", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                if (await _global.Com.Current.EraseEepromAsync())
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