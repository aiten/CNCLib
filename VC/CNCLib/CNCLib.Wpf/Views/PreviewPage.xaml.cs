using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CNCLib.GCode.Commands;
using CNCLib.GUI.Load;
using CNCLib.Logic.Contracts.DTO;
using CNCLib.Wpf.ViewModels;

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
		}
	}
}
