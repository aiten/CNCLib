using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using CNCLib.GCode.GUI.Load;
using CNCLib.GCode.GUI.Views;
using CNCLib.GCode.GUI.ViewModels;
using CNCLib.Wpf.ViewModels;
using Framework.Wpf.View;
using AutoMapper;
using Framework.Tools.Dependency;
using System.Windows.Input;

namespace CNCLib.Wpf.Views
{
    /// <summary>
    /// Interaction logic for PreviewPage.xaml
    /// </summary>
    public partial class PreviewPage : Page
	{
		public PreviewPage()
		{
			InitializeComponent();

            this.DefaulInitForBaseViewModel();

            Global.Instance.PropertyChanged += (object sender, PropertyChangedEventArgs e) => 
            {
                if (e.PropertyName == nameof(Global.SizeX) || e.PropertyName == nameof(Global.SizeY))
                {
                    gcode.SizeX = (double) Global.Instance.SizeX;
                    gcode.SizeY = (double) Global.Instance.SizeY;
                };
            };

            var vm = DataContext as PreviewViewModel;

			if (vm.GetLoadInfo == null)
				vm.GetLoadInfo = new Func<PreviewViewModel.GetLoadInfoArg, bool>((arg) =>
				{
                    if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
                    {
                        using (LoadOptionForm form = new LoadOptionForm())
                        {
                            form.LoadInfo = arg.LoadOption;
                            form.UseAzure = arg.UseAzure;

                            if (form.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                                return false;

                            arg.LoadOption = form.LoadInfo;
                            arg.UseAzure = form.UseAzure;
                            return true;
                        }
                    }
                    else
                    {
                        var dlg = new LoadOptionView();
                        var vmdlg = dlg.DataContext as LoadOptionViewModel;
                        vmdlg.LoadOptionsValue = Dependency.Resolve<IMapper>().Map<CNCLib.GCode.GUI.Models.LoadOptions>(arg.LoadOption);
                        vmdlg.UseAzure = arg.UseAzure;
                        if (!dlg.ShowDialog() ?? false)
                            return false;

                        arg.LoadOption = Dependency.Resolve<IMapper>().Map<CNCLib.Logic.Contracts.DTO.LoadOptions>(vmdlg.LoadOptionsValue);
                        arg.UseAzure = vmdlg.UseAzure;
                        return true;
                    }
                });


			if (vm.RefreshPreview == null)
			{
				vm.RefreshPreview = new Action(() =>
				{
					gcode.Dispatcher.Invoke(() => gcode.InvalidateVisual());
				});
			}
		}
	}
}
