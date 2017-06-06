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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;
using AutoMapper;
using CNCLib.GCode.GUI.Models;
using CNCLib.ServiceProxy;
using Framework.Tools.Dependency;
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

        public override async Task Loaded()
        {
            await base.Loaded();
            await LoadAllSettings(null);
        }

        private bool _allSettingsLoaded = false;

        #endregion

        #region private operations

        #endregion

        #region GUI-forward

        public Func<string,bool,string> BrowseFileNameFunc { get; set; }

        #endregion

        #region Properties

        private LoadOptions _loadOptions = new LoadOptions();
        public LoadOptions LoadOptionsValue
        {
            get { return _loadOptions; }
            set { SetProperty(() => _loadOptions == value, () => _loadOptions = value);  }
        }

        private ObservableCollection<LoadOptions> _allLoadOptions = new ObservableCollection<LoadOptions>();
        public ObservableCollection<LoadOptions> AllLoadOptions
        {
            get { return _allLoadOptions; }
            set { SetProperty(ref _allLoadOptions, value); }
        }

        private LoadOptions _selectedloadOptions = null;
        public LoadOptions SelectedLoadOption
        {
            get { return _selectedloadOptions; }
            set {
                    SetProperty(() => _selectedloadOptions == value, () => _selectedloadOptions = value);
                    if (value != null && _allSettingsLoaded)
                        LoadOptionsValue = value;
                }
        }

        private bool _useAzure = false;
        public bool UseAzure
        {
            get { return _useAzure; }
            set { SetProperty(() => _useAzure == value, () => _useAzure = value); }
        }


        #endregion

        #region Operations

        private async Task LoadAllSettings(int? setselectedid)
        {
            _allLoadOptions.Clear();

            using (var controller = Dependency.Resolve<ILoadOptionsService>())
            {
                var map = Dependency.Resolve<IMapper>();
                var items = await controller.GetAll();
                foreach (var s in items.OrderBy((o) => o.SettingName))
                {
                    var option = map.Map<LoadOptions>(s);
                    _allLoadOptions.Add(option);
                    if (setselectedid.HasValue && option.Id == setselectedid.Value)
                    {
                        SelectedLoadOption = option;
                    }
                }
            }
            _allSettingsLoaded = true;
        }

        public bool Can()
        {
            return true;
        }

        void BrowseFileName()
        {
            string filename = BrowseFileNameFunc?.Invoke(LoadOptionsValue.FileName,false);
            if (filename != null)
            {
                LoadOptionsValue.FileName = filename;
                RaisePropertyChanged(nameof(LoadOptionsValue));
            }
        }
        void BrowseGCodeFileName()
        {
            string filename = BrowseFileNameFunc?.Invoke(LoadOptionsValue.GCodeWriteToFileName,false);
            if (filename != null)
            {
                LoadOptionsValue.GCodeWriteToFileName = filename;
                RaisePropertyChanged(nameof(LoadOptionsValue));
            }
        }
        void BrowseImageFileName()
        {
            string filename = BrowseFileNameFunc?.Invoke(LoadOptionsValue.ImageWriteToFileName,false);
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

        #region Settings

        async Task SaveSettings()
        {
            try
            {
                using (var controller = Dependency.Resolve<ILoadOptionsService>())
                {
                    var map = Dependency.Resolve<IMapper>();

                    var opt = map.Map<CNCLib.Logic.Contracts.DTO.LoadOptions>(SelectedLoadOption);
                    await controller.Update(opt);
                }
            }
            catch (Exception ex)
            {
                MessageBox?.Invoke("Save Options failed: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        async Task SaveAsSettings()
        {
            try
            {
                using (var controller = Dependency.Resolve<ILoadOptionsService>())
                {
                    var map = Dependency.Resolve<IMapper>();

                    var opt = map.Map<CNCLib.Logic.Contracts.DTO.LoadOptions>(LoadOptionsValue);
                    int id = await controller.Add(opt);
                    await LoadAllSettings(id);
                }
            }
            catch (Exception ex)
            {
                MessageBox?.Invoke("SaveAs Options failed: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        async Task DeleteSettings()
        {
            try
            {
                using (var controller = Dependency.Resolve<ILoadOptionsService>())
                {
                    var map = Dependency.Resolve<IMapper>();
                    var opt = map.Map<CNCLib.Logic.Contracts.DTO.LoadOptions>(SelectedLoadOption);
                    await controller.Delete(opt);
                    await LoadAllSettings(AllLoadOptions?.FirstOrDefault((o) => o.Id != opt.Id)?.Id);
                }
            }
            catch (Exception ex)
            {
                MessageBox?.Invoke("Delete Options failed: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        async Task ImportSettings()
        {
            string filename = BrowseFileNameFunc?.Invoke(@"*.xml", false);
            if (filename != null)
            {
                try
                {
                    using (StreamReader sr = new StreamReader(filename))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(CNCLib.Logic.Contracts.DTO.LoadOptions));
                        var opt = (CNCLib.Logic.Contracts.DTO.LoadOptions)serializer.Deserialize(sr);
                        sr.Close();

                        if (_allLoadOptions.FirstOrDefault((o) => o.SettingName == opt.SettingName)!=null)
                        {
                            opt.SettingName = $"{opt.SettingName}#imported#{DateTime.Now}"; 
                        }

                        using (var controller = Dependency.Resolve<ILoadOptionsService>())
                        {
                            int id = await controller.Add(opt);
                            await LoadAllSettings(id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox?.Invoke("ImportSettings failed: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        void ExportSettings()
        {
            string filename = BrowseFileNameFunc?.Invoke(SelectedLoadOption.SettingName + @".xml", true);
            if (filename != null)
            {
                var map = Dependency.Resolve<IMapper>();
                var opt = map.Map<CNCLib.Logic.Contracts.DTO.LoadOptions>(SelectedLoadOption);

                using (StreamWriter sw = new StreamWriter(filename))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(CNCLib.Logic.Contracts.DTO.LoadOptions));
                    serializer.Serialize(sw, opt);
                    sw.Close();
                }
            }
        }

        #endregion

        #endregion

        #region Commands

        public ICommand BrowseFileNameCommand => new DelegateCommand(BrowseFileName, Can);
        public ICommand BrowseGCodeFileNameCommand => new DelegateCommand(BrowseGCodeFileName, Can);
        public ICommand BrowseImageFileNameCommand => new DelegateCommand(BrowseImageFileName, Can);
        public ICommand SetSameDPICommand => new DelegateCommand(() => { LoadOptionsValue.ImageDPIY = LoadOptionsValue.ImageDPIX; RaiseLoadOptionsChanged(); }, Can);
        public ICommand SetSameScaleToCommand => new DelegateCommand(() => { LoadOptionsValue.ScaleY = LoadOptionsValue.ScaleX; RaiseLoadOptionsChanged(); }, Can);
        public ICommand SetSameOfsCommand => new DelegateCommand(() => { LoadOptionsValue.OfsY = LoadOptionsValue.OfsX; RaiseLoadOptionsChanged(); }, Can);
        public ICommand SetSameDotSizeCommand => new DelegateCommand(() => { LoadOptionsValue.DotSizeY = LoadOptionsValue.DotSizeX; RaiseLoadOptionsChanged(); }, Can);
        public ICommand SetSameDotDistCommand => new DelegateCommand(() => { LoadOptionsValue.DotDistY = LoadOptionsValue.DotDistX; RaiseLoadOptionsChanged(); }, Can);
        public ICommand SaveSettingCommand => new DelegateCommand(async () => await SaveSettings(), () => Can() && SelectedLoadOption != null);
        public ICommand SaveAsSettingCommand => new DelegateCommand(async () => await SaveAsSettings(), () => Can() && !string.IsNullOrEmpty(LoadOptionsValue.SettingName));
        public ICommand DeleteSettingCommand => new DelegateCommand(async () => await DeleteSettings(), () => Can() && SelectedLoadOption != null);
        public ICommand ImportSettingCommand => new DelegateCommand(async () => await ImportSettings(), Can);
        public ICommand ExportSettingCommand => new DelegateCommand(ExportSettings, () => Can() && SelectedLoadOption != null);

        #endregion
    }
}
