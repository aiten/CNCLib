////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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
using System.Threading.Tasks;
using System.Windows.Input;
using CNCLib.GCode.GUI.Models;
using Framework.Wpf.Helpers;
using Framework.Wpf.ViewModels;

namespace CNCLib.GCode.GUI.ViewModels
{
    public class LoadOptionViewModel : BaseViewModel
    {
		#region crt

		public LoadOptionViewModel()
		{
		}

        #endregion

        #region private operations

        #endregion

        #region GUI-forward

        public Func<string,string> BrowseFileNameFunc { get; set; }

        #endregion

        #region Properties

        private LoadOptions _loadOptions = new LoadOptions();
        public LoadOptions LoadOptionsValue
        {
            get { return _loadOptions; }
            set { SetProperty(() => _loadOptions == value, () => _loadOptions = value); }
        }

        #endregion

        #region Operations

        public bool Can()
        {
            return true;
        }

        public async Task Connect()
        {
		}


		public bool CanConnect()
        {
            return true;
            //return !Connected && Machine != null;
        }

        void BrowseFileName()
        {
            string filename = BrowseFileNameFunc?.Invoke(LoadOptionsValue.FileName);
            if (filename != null)
            {
                LoadOptionsValue.FileName = filename;
                RaisePropertyChanged(nameof(LoadOptionsValue));
            }
        }
        void BrowseGCodeFileName()
        {
            string filename = BrowseFileNameFunc?.Invoke(LoadOptionsValue.GCodeWriteToFileName);
            if (filename != null)
            {
                LoadOptionsValue.GCodeWriteToFileName = filename;
                RaisePropertyChanged(nameof(LoadOptionsValue));
            }
        }
        void BrowseImageFileName()
        {
            string filename = BrowseFileNameFunc?.Invoke(LoadOptionsValue.ImageWriteToFileName);
            if (filename != null)
            {
                LoadOptionsValue.ImageWriteToFileName = filename;
                RaisePropertyChanged(nameof(LoadOptionsValue));
            }
        }

        void RaiseLoadOptionsChanged()
        {
            RaisePropertyChanged(nameof(LoadOptionsValue));
        }

        #endregion

        #region Commands

        public ICommand ConnectCommand => new DelegateCommand(async () => await Connect(), CanConnect);
        public ICommand BrowseFileNameCommand => new DelegateCommand(BrowseFileName, Can);
        public ICommand BrowseGCodeFileNameCommand => new DelegateCommand(BrowseGCodeFileName, Can);
        public ICommand BrowseImageFileNameCommand => new DelegateCommand(BrowseImageFileName, Can);
        public ICommand SetSameDPICommand => new DelegateCommand(() => { LoadOptionsValue.ImageDPIY = LoadOptionsValue.ImageDPIX; RaiseLoadOptionsChanged(); }, Can);
        public ICommand SetSameScaleToCommand => new DelegateCommand(() => { LoadOptionsValue.ScaleY = LoadOptionsValue.ScaleX; RaiseLoadOptionsChanged(); }, Can);
        public ICommand SetSameOfsCommand => new DelegateCommand(() => { LoadOptionsValue.OfsY = LoadOptionsValue.OfsX; RaiseLoadOptionsChanged(); }, Can);
        public ICommand SetSameDotSizeCommand => new DelegateCommand(() => { LoadOptionsValue.DotSizeY = LoadOptionsValue.DotSizeX; RaiseLoadOptionsChanged(); }, Can);
        public ICommand SetSameDotDistCommand => new DelegateCommand(() => { LoadOptionsValue.DotDistY = LoadOptionsValue.DotDistX; RaiseLoadOptionsChanged(); }, Can);

        #endregion
    }
}
