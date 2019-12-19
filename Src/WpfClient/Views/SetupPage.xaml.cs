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

using System.Threading.Tasks;
using System.Windows.Controls;

using CNCLib.WpfClient.ViewModels;

using Framework.Dependency;
using Framework.Wpf.Views;

using Microsoft.Extensions.DependencyInjection;

namespace CNCLib.WpfClient.Views
{
    /// <summary>
    /// Interaction logic for SetupPage.xaml
    /// </summary>
    public partial class SetupPage : Page
    {
        public SetupPage()
        {
            var vm = AppService.GetRequiredService<SetupWindowViewModel>();
            DataContext = vm;

            InitializeComponent();

            this.DefaultInitForBaseViewModel();
/*
            RoutedEventHandler loaded =null;
			loaded = new RoutedEventHandler(async (object v, RoutedEventArgs e) =>
			{
				var vmm = DataContext as BaseViewModel;
				await vmm.Loaded();
				((SetupPage)e.Source).Loaded -= loaded;
			});

			Loaded += loaded;
*/
            if (vm.EditMachine == null)
            {
                vm.EditMachine = mId =>
                {
                    var dlg = new MachineView();
                    if (dlg.DataContext is MachineViewModel viewModel)
                    {
                        Task.Run(() => { viewModel.LoadMachine(mId).ConfigureAwait(false).GetAwaiter().GetResult(); }).Wait();
                        dlg.ShowDialog();
                    }
                };
            }

            if (vm.ShowEeprom == null)
            {
                vm.ShowEeprom = () =>
                {
                    var dlg       = new EepromView();
                    var viewModel = dlg.DataContext as EepromViewModel;
                    dlg.ShowDialog();
                };
            }

            if (vm.EditJoystick == null)
            {
                vm.EditJoystick = () =>
                {
                    var dlg       = new JoystickView();
                    var viewModel = dlg.DataContext as JoystickViewModel;
                    dlg.ShowDialog();
                };
            }

            if (vm.Login == null)
            {
                vm.Login = () =>
                {
                    var dlg       = new LoginView();
                    var viewModel = dlg.DataContext as LoginViewModel;
                    viewModel.UserName = vm.UserName;
                    if (dlg.ShowDialog() ?? false)
                    {
                        return viewModel.UserName;
                    }

                    return null;
                };
            }
        }
    }
}