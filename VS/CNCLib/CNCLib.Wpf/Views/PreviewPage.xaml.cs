using System;
using System.Windows;
using System.Windows.Controls;
using CNCLib.GCode.GUI.Load;
using CNCLib.Wpf.ViewModels;
using Framework.Wpf.ViewModels;

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

			Loaded += new RoutedEventHandler(async (object v, RoutedEventArgs e) =>
			{
				var vmm = DataContext as BaseViewModel;
				await vmm.Loaded();
			});

			var vm = DataContext as PreviewViewModel;

			if (vm.GetLoadInfo == null)
				vm.GetLoadInfo = new Func<PreviewViewModel.GetLoadInfoArg, bool>((arg) =>
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
				});

			if (vm.MessageBox == null)
			{
				vm.MessageBox = new Func<string, string, MessageBoxButton, MessageBoxImage, MessageBoxResult>((messageBoxText, caption, button, icon) =>
				{
					return MessageBox.Show(messageBoxText, caption, button, icon);
				});
			}

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
