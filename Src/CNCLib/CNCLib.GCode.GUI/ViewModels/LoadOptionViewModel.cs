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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;
using AutoMapper;
using CNCLib.GCode.GUI.Models;
using CNCLib.Service.Contracts;
using Framework.Tools.Pattern;
using Framework.Wpf.Helpers;
using Framework.Wpf.ViewModels;

namespace CNCLib.GCode.GUI.ViewModels
{
    public class LoadOptionViewModel : BaseViewModel
    {
        #region crt

        public LoadOptionViewModel(IFactory<ILoadOptionsService> loadOptionsService, IMapper mapper)
        {
            _loadOptionsService = loadOptionsService ?? throw new ArgumentNullException();
            _mapper             = mapper ?? throw new ArgumentNullException();
        }

        readonly IFactory<ILoadOptionsService> _loadOptionsService;
        readonly IMapper                       _mapper;

        public override async Task Loaded()
        {
            await base.Loaded();
            RaisePropertyChanged(nameof(LoadOptionsValue));
            using (var scope = _loadOptionsService.Create())
            {
                await LoadAllSettings(LoadOptionsValue.Id, scope);
            }
        }

        private bool _allSettingsLoaded = false;

        #endregion

        #region private operations

        #endregion

        #region GUI-forward

        #endregion

        #region Properties

        private LoadOptions _loadOptions = new LoadOptions();

        public LoadOptions LoadOptionsValue { get => _loadOptions; set { SetProperty(() => _loadOptions == value, () => _loadOptions = value); } }

        private ObservableCollection<LoadOptions> _allLoadOptions = new ObservableCollection<LoadOptions>();

        public ObservableCollection<LoadOptions> AllLoadOptions { get => _allLoadOptions; set => SetProperty(ref _allLoadOptions, value); }

        private LoadOptions _selectedloadOptions = null;

        public LoadOptions SelectedLoadOption
        {
            get => _selectedloadOptions;
            set
            {
                SetProperty(() => _selectedloadOptions == value, () => _selectedloadOptions = value);
                if (value != null && _allSettingsLoaded)
                {
                    LoadOptionsValue = _mapper.Map<LoadOptions>(value);
                }
            }
        }

        private bool _useAzure = false;

        public bool UseAzure { get => _useAzure; set { SetProperty(() => _useAzure == value, () => _useAzure = value); } }

        private bool _busy = false;

        public bool Busy { get => _busy; set { SetProperty(() => _busy == value, () => _busy = value); } }

        #endregion

        #region Operations

        private async Task LoadAllSettings(int? setselectedid, IScope<ILoadOptionsService> scope)
        {
            _allLoadOptions.Clear();

            var items = await scope.Instance.GetAll();
            if (items != null)
            {
                foreach (var s in items.OrderBy(o => o.SettingName))
                {
                    var option = _mapper.Map<LoadOptions>(s);
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
            return Busy == false;
        }

        public bool CanSave()
        {
            List<String> ignorelist = new List<string>();
            if (LoadOptionsValue.AutoScale)
            {
                ignorelist.AddRange(new string[]
                {
                    "ScaleX", "ScaleY", "OffsetX", "OffsetY"
                });
            }

            if (Can() && SelectedLoadOption != null && !Framework.Tools.Helpers.CompareProperties.AreObjectsPropertiesEqual(SelectedLoadOption, LoadOptionsValue, ignorelist.ToArray()))
            {
                return true;
            }

            return false;
        }

        void BrowseFileName()
        {
            string filename = BrowseFileNameFunc?.Invoke(LoadOptionsValue.FileName, false);
            if (filename != null)
            {
                LoadOptionsValue.FileName = filename;
                RaisePropertyChanged(nameof(LoadOptionsValue));
            }
        }

        void BrowseGCodeFileName()
        {
            string filename = BrowseFileNameFunc?.Invoke(LoadOptionsValue.GCodeWriteToFileName, false);
            if (filename != null)
            {
                LoadOptionsValue.GCodeWriteToFileName = filename;
                RaisePropertyChanged(nameof(LoadOptionsValue));
            }
        }

        void BrowseImageFileName()
        {
            string filename = BrowseFileNameFunc?.Invoke(LoadOptionsValue.ImageWriteToFileName, false);
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
                Busy = true;
                var opt = _mapper.Map<Logic.Contracts.DTO.LoadOptions>(LoadOptionsValue);
                using (var scope = _loadOptionsService.Create())
                {
                    await scope.Instance.Update(opt);
                    await LoadAllSettings(LoadOptionsValue.Id, scope);
                }
            }
            catch (Exception ex)
            {
                MessageBox?.Invoke("Save Options failed: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Busy = false;
            }
        }

        async Task SaveAsSettings()
        {
            try
            {
                Busy = true;
                var opt = _mapper.Map<Logic.Contracts.DTO.LoadOptions>(LoadOptionsValue);
                using (var scope = _loadOptionsService.Create())
                {
                    int id = await scope.Instance.Add(opt);
                    await LoadAllSettings(id, scope);
                }
            }
            catch (Exception ex)
            {
                MessageBox?.Invoke("SaveAs Options failed: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Busy = false;
            }
        }

        async Task DeleteSettings()
        {
            try
            {
                Busy = true;
                var opt = _mapper.Map<Logic.Contracts.DTO.LoadOptions>(SelectedLoadOption);
                using (var scope = _loadOptionsService.Create())
                {
                    await scope.Instance.Delete(opt);
                    await LoadAllSettings(AllLoadOptions?.FirstOrDefault(o => o.Id != opt.Id)?.Id, scope);
                }
            }
            catch (Exception ex)
            {
                MessageBox?.Invoke("Delete Options failed: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Busy = false;
            }
        }

        async Task ImportSettings()
        {
            string filename = BrowseFileNameFunc?.Invoke(@"*.xml", false);
            if (filename != null)
            {
                Busy = true;
                try
                {
                    using (var sr = new StreamReader(filename))
                    {
                        var serializer = new XmlSerializer(typeof(Logic.Contracts.DTO.LoadOptions));
                        var opt        = (Logic.Contracts.DTO.LoadOptions) serializer.Deserialize(sr);
                        sr.Close();

                        if (_allLoadOptions.FirstOrDefault(o => o.SettingName == opt.SettingName) != null)
                        {
                            opt.SettingName = $"{opt.SettingName}#imported#{DateTime.Now}";
                        }

                        using (var scope = _loadOptionsService.Create())
                        {
                            int id = await scope.Instance.Add(opt);
                            await LoadAllSettings(id, scope);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox?.Invoke("ImportSettings failed: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    Busy = false;
                }
            }
        }

        void ExportSettings()
        {
            string filename = BrowseFileNameFunc?.Invoke(LoadOptionsValue.SettingName + @".xml", true);
            if (filename != null)
            {
                var opt = _mapper.Map<Logic.Contracts.DTO.LoadOptions>(SelectedLoadOption);

                using (var sw = new StreamWriter(filename))
                {
                    var serializer = new XmlSerializer(typeof(Logic.Contracts.DTO.LoadOptions));
                    serializer.Serialize(sw, opt);
                    sw.Close();
                }
            }
        }

        #endregion

        #endregion

        #region overrides

        protected override bool CanDialogOK()
        {
            return base.CanDialogOK() && !Busy;
        }

        #endregion

        #region Commands

        public ICommand BrowseFileNameCommand      => new DelegateCommand(BrowseFileName,      Can);
        public ICommand BrowseGCodeFileNameCommand => new DelegateCommand(BrowseGCodeFileName, Can);
        public ICommand BrowseImageFileNameCommand => new DelegateCommand(BrowseImageFileName, Can);

        public ICommand SetSameDPICommand => new DelegateCommand(() =>
        {
            LoadOptionsValue.ImageDPIY = LoadOptionsValue.ImageDPIX;
            RaiseLoadOptionsChanged();
        }, Can);

        public ICommand SetSameScaleToCommand => new DelegateCommand(() =>
        {
            LoadOptionsValue.ScaleY = LoadOptionsValue.ScaleX;
            RaiseLoadOptionsChanged();
        }, Can);

        public ICommand SetSameOfsCommand => new DelegateCommand(() =>
        {
            LoadOptionsValue.OfsY = LoadOptionsValue.OfsX;
            RaiseLoadOptionsChanged();
        }, Can);

        public ICommand SetSameDotSizeCommand => new DelegateCommand(() =>
        {
            LoadOptionsValue.DotSizeY = LoadOptionsValue.DotSizeX;
            RaiseLoadOptionsChanged();
        }, Can);

        public ICommand SetSameDotDistCommand => new DelegateCommand(() =>
        {
            LoadOptionsValue.DotDistY = LoadOptionsValue.DotDistX;
            RaiseLoadOptionsChanged();
        }, Can);

        public ICommand SaveSettingCommand => new DelegateCommand(async () => await SaveSettings(), CanSave).ObservesProperty(() => Busy);

        public ICommand SaveAsSettingCommand =>
            new DelegateCommand(async () => await SaveAsSettings(), () => Can() && !string.IsNullOrEmpty(LoadOptionsValue.SettingName)).ObservesProperty(() => Busy);

        public ICommand DeleteSettingCommand =>
            new DelegateCommand(async () => await DeleteSettings(), () => Can() && SelectedLoadOption != null).ObservesProperty(() => Busy);

        public ICommand ImportSettingCommand => new DelegateCommand(async () => await ImportSettings(), Can).ObservesProperty(() => Busy);

        public ICommand ExportSettingCommand =>
            new DelegateCommand(ExportSettings, () => Can() && SelectedLoadOption != null).ObservesProperty(() => Busy);

        #endregion
    }
}