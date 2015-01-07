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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Proxxon.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

		private void Button_PaintClick(object sender, RoutedEventArgs e)
		{
			using (Proxxon.GUI.PaintForm form = new Proxxon.GUI.PaintForm())
			{
				form.ShowDialog();
			}
		}

		private void Button_ManualControlClick(object sender, RoutedEventArgs e)
        {
			
			new ManualControl().ShowDialog();
		}

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
			Framework.Tools.Singleton<Framework.Logic.ArduinoSerialCommunication>.Free();
        }
    }
}
