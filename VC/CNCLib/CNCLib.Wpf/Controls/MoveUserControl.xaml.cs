using Framework.Wpf.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace CNCLib.Wpf.Controls
{
	/// <summary>
	/// Interaction logic for MoveUserControl.xaml
	/// </summary>
	public partial class MoveUserControl : UserControl
	{
		public MoveUserControl()
		{
			InitializeComponent();
		}

        public static readonly DependencyProperty LeftCommandProperty = DependencyProperty.Register(
			"Left", typeof(ICommand), typeof(MoveUserControl), new PropertyMetadata(default(ICommand)));

		public ICommand Left
		{
			get { return (ICommand)GetValue(LeftCommandProperty); }
            set { SetValue(LeftCommandProperty, value); }
        }

        public static readonly DependencyProperty RightCommandProperty = DependencyProperty.Register(
			"Right", typeof(ICommand), typeof(MoveUserControl), new PropertyMetadata(default(ICommand)));

		public ICommand Right
		{
			get { return (ICommand)GetValue(RightCommandProperty); }
            set { SetValue(RightCommandProperty, value); }
        }

        public static readonly DependencyProperty UpCommandProperty = DependencyProperty.Register(
			"Up", typeof(ICommand), typeof(MoveUserControl), new PropertyMetadata(default(ICommand)));

		public ICommand Up
		{
			get { return (ICommand)GetValue(UpCommandProperty); }
			set { SetValue(UpCommandProperty, value); }
		}

		public static readonly DependencyProperty DownCommandProperty = DependencyProperty.Register(
			"Down", typeof(ICommand), typeof(MoveUserControl), new PropertyMetadata(default(ICommand)));

		public ICommand Down
		{
			get { return (ICommand)GetValue(DownCommandProperty); }
			set { _down.Command = value; SetValue(DownCommandProperty, value); }
		}

        public double MoveDist { get; set; } = 10.0;
        public double MoveDistCtrl { get; set; } = 100.0;
        public double MoveDistShift { get; set; } = 1.0;
        public double MoveDistAlt { get; set; } = 0.1;


        private void Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    OnLeft();
                    e.Handled = true;
                    break;
                case Key.Right:
                    OnRight();
                    e.Handled = true;
                    break;
                case Key.Up:
                    OnUp();
                    e.Handled = true;
                    break;
                case Key.Down:
                    OnDown();
                    e.Handled = true;
                    break;
            }
        }

        private void OnDown()
        {
            double dist = GetDist();
            if (Down != null && Down.CanExecute(dist))
                Down.Execute(dist);
        }

        private void OnUp()
        {
            double dist = GetDist();
            if (Up != null && Up.CanExecute(dist))
                Up.Execute(dist);
        }

        private void OnRight()
        {
            double dist = GetDist();
            if (Right != null && Right.CanExecute(dist))
                Right.Execute(dist);
        }

        private void OnLeft()
        {
            double dist = GetDist();
            if (Left != null && Left.CanExecute(dist))
                Left.Execute(dist);
        }

        private double GetDist()
        {
            double dist;
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                dist = MoveDistCtrl;
            else if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                dist = MoveDistShift;
            //            else if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
            //                dist = MoveDistAlt;
            else
                dist = MoveDist;
            return dist;
        }

        private void _up_Click(object sender, RoutedEventArgs e)
        {
            OnUp();
        }

        private void _right_Click(object sender, RoutedEventArgs e)
        {
            OnRight();
        }

        private void _left_Click(object sender, RoutedEventArgs e)
        {
            OnLeft();
        }

        private void _down_Click(object sender, RoutedEventArgs e)
        {
            OnDown();
        }
    }
}
