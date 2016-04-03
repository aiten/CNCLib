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
            DateTime now = DateTime.Now;
			Com.Trace.EnableTrace(string.Format(@"{0}CNCLibTrace_{1:D4}{2:D2}{3:D2}_{4:D2}{5:D2}{6:D2}.txt",
                    System.IO.Path.GetTempPath(),
                    now.Year,now.Month,now.Day,now.Hour,now.Minute,now.Second));
		}
		private Framework.Arduino.ArduinoSerialCommunication Com
		{
			get { return Framework.Tools.Pattern.Singleton<Framework.Arduino.ArduinoSerialCommunication>.Instance; }
		}
        private Framework.Arduino.ArduinoSerialCommunication ComJoystick
        {
            get { return Framework.Tools.Pattern.Singleton<Wpf.Helpers.JoystickArduinoSerialCommunication>.Instance; }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (Com.IsConnected)
				Com.Disconnect();

			Com.Trace.CloseTrace();

            if (ComJoystick.IsConnected)
                ComJoystick.Disconnect();

            ComJoystick.Trace.CloseTrace();
        }
    }
}
