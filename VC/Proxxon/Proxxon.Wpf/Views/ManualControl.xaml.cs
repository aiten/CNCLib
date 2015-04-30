using Proxxon.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace Proxxon.Wpf
{
    /// <summary>
    /// Interaction logic for ManualControl.xaml
    /// </summary>
    public partial class ManualControl : Window
    {
        public ManualControl()
        {
            InitializeComponent();
        }

        public bool IsConnected
        {
            get {
            var vm = DataContext as ManualControlViewModel;
            if (vm != null) return vm.Com.IsConnected;
                return false;
            }
        }

        #region Other

		private void BrowseFileOpenDialog_Click(object sender, RoutedEventArgs e)
		{
			// Configure open file dialog box
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
			//dlg.FileName = "Document"; // Default file name
			//dlg.DefaultExt = ".txt"; // Default file extension
			//dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

			// Show open file dialog box
			Nullable<bool> result = dlg.ShowDialog();

			// Process open file dialog box results
			if (result == true)
			{
				// Open document
				string filename = dlg.FileName;
				var vm = DataContext as ManualControlViewModel;
				if (vm != null) vm.SD.FileName = filename;
			}
		}

        #endregion
  
    }
}
