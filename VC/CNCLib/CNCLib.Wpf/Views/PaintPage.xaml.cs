using CNCLib.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CNCLib.Wpf.Views
{
	/// <summary>
	/// Interaction logic for PaintPage.xaml
	/// </summary>
	public partial class PaintPage : Page
	{
		public PaintPage()
		{
			InitializeComponent();
		}

		private void container_Loaded(object sender, RoutedEventArgs e)
		{
			if (_container.Child == null)
			{
				var f = new PaintForm();
				f.TopLevel = false;
				f.FormBorderStyle = FormBorderStyle.None;
				_container.Child = f;
			}
			((PaintForm)_container.Child).SetMachineSize();

		}
	}
}
