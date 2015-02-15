using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace SpeedChart
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{

			InitializeComponent();

            var vm = DataContext as SpeedChart.ViewModels.MainWindowViewModel;
            vm.ViewWindow = this;
        }

		TimeSampleList _list = new TimeSampleList();
        public SpeedChartControl SpeedChart { get { return _chart; } }

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			Point pt = e.GetPosition(this);

			var ts = _chart.HitTest(pt);

			if (ts != null)
			{
                (DataContext as SpeedChart.ViewModels.MainWindowViewModel).Content = ts.Index + ": " + ts.Info;
			}
		}
/*
		protected async override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			Point pt = e.GetPosition(this);

			var ts = Task<TimeSample>.Factory.StartNew(() => _chart.HitTest(pt));
			await ts;

			if (ts.Result != null)
			{
				_pos.Content = ts.Result.Index + ": " + ts.Result.Info;
			}
		}
 */
	}
}
