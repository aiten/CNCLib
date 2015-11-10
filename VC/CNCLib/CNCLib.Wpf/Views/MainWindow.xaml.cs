using System;
using System.Collections.Generic;
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

namespace CNCLib.Wpf.Views
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			Com.Trace.EnableTrace(System.IO.Path.GetTempPath() + @"CNCLibTrace.txt");
		}
		private CNCLib.Arduino.ArduinoSerialCommunication Com
		{
			get { return Framework.Tools.Pattern.Singleton<CNCLib.Arduino.ArduinoSerialCommunication>.Instance; }
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (Com.IsConnected)
				Com.Disconnect();

			Com.Trace.CloseTrace();
        }
	}
}
