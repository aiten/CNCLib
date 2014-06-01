using Roboter.Wpf.ViewModels;
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

namespace Roboter.Wpf
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

        #region X

        private void Button_XUp2Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ManualControlViewModel;
            if (vm != null) vm.SendRoboterCommand("rx 10000");
        }
        private void Button_XUp1Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ManualControlViewModel;
            if (vm != null) vm.SendRoboterCommand("rx 1000");
        }

        private void Button_XDown1Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ManualControlViewModel;
            if (vm != null) vm.SendRoboterCommand("rx -1000");
        }
        private void Button_XDown2Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ManualControlViewModel;
            if (vm != null) vm.SendRoboterCommand("rx -10000");
        }
        private void Button_XRefClick(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ManualControlViewModel;
            if (vm != null) vm.SendRoboterCommand("ix");
        }

        #endregion

        #region Y

        private void Button_YUp2Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ManualControlViewModel;
            if (vm != null) vm.SendRoboterCommand("ry 10000");
        }
        private void Button_YUp1Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ManualControlViewModel;
            if (vm != null) vm.SendRoboterCommand("ry 1000");
        }

        private void Button_YDown1Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ManualControlViewModel;
            if (vm != null) vm.SendRoboterCommand("ry -1000");
        }
        private void Button_YDown2Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ManualControlViewModel;
            if (vm != null) vm.SendRoboterCommand("ry -10000");
        }
        private void Button_YRefClick(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ManualControlViewModel;
            if (vm != null) vm.SendRoboterCommand("iy");
        }

        #endregion

        #region Z

        private void Button_ZUp2Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ManualControlViewModel;
            if (vm != null) vm.SendRoboterCommand("rz 10000");
        }
        private void Button_ZUp1Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ManualControlViewModel;
            if (vm != null) vm.SendRoboterCommand("rz 1000");
        }

        private void Button_ZDown1Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ManualControlViewModel;
            if (vm != null) vm.SendRoboterCommand("rz -1000");
        }
        private void Button_ZDown2Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ManualControlViewModel;
            if (vm != null) vm.SendRoboterCommand("rz -10000");
        }

        private void Button_ZRefClick(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ManualControlViewModel;
            if (vm != null) vm.SendRoboterCommand("iz");
        }

        #endregion

        #region E0
        private void Button_E0Up2Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ManualControlViewModel;
            if (vm != null) vm.SendRoboterCommand("re 10000");
        }
        private void Button_E0Up1Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ManualControlViewModel;
            if (vm != null) vm.SendRoboterCommand("re 1000");
        }

        private void Button_E0Down1Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ManualControlViewModel;
            if (vm != null) vm.SendRoboterCommand("re -1000");
        }
        private void Button_E0Down2Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ManualControlViewModel;
            if (vm != null) vm.SendRoboterCommand("re -10000");
        }

        #endregion

        #region E1
        private void Button_E1Up2Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ManualControlViewModel;
            if (vm != null) vm.SendRoboterCommand("rf 10000");
        }
        private void Button_E1Up1Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ManualControlViewModel;
            if (vm != null) vm.SendRoboterCommand("rf 1000");
        }

        private void Button_E1Down1Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ManualControlViewModel;
            if (vm != null) vm.SendRoboterCommand("rf -1000");
        }
        private void Button_E1Down2Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ManualControlViewModel;
            if (vm != null) vm.SendRoboterCommand("rf -10000");
        }

        #endregion
 
        #region Memory
        private void Button_MCClick(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ManualControlViewModel;
            if (vm != null) vm.SendRoboterCommand("mc");
        }
        private void Button_MMClick(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ManualControlViewModel;
            if (vm != null) vm.SendRoboterCommand("m-");
        }

        private void Button_MPClick(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ManualControlViewModel;
            if (vm != null) vm.SendRoboterCommand("m+");
        }
        private void Button_MRClick(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ManualControlViewModel;
            if (vm != null) vm.SendRoboterCommand("mr");
        }
        private void Button_MGClick(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ManualControlViewModel;
            if (vm != null) vm.SendRoboterCommand("mg");
        }

        #endregion
  
    }
}
