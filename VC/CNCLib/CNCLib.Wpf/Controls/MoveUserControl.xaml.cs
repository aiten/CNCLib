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
			set { SetValue(DownCommandProperty, value); }
		}


		public double MoveDist { get; set; }

		private void Grid_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			switch(e.Key)
			{
				case Key.Left:
					if (Left != null && Left.CanExecute(null))
						Left.Execute(null);
					break;
				case Key.Right:
					if (Right != null && Right.CanExecute(null))
						Right.Execute(null);
					break;
				case Key.Up:
					if (Up != null && Up.CanExecute(null))
						Up.Execute(null);
					break;
				case Key.Down:
					if (Down != null && Down.CanExecute(null))
						Down.Execute(null);
					break;
			}
		}
	}
}
