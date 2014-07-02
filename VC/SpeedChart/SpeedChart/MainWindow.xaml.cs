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
		}

		TimeSampleList _list = new TimeSampleList();
		string _filename = @"P:\Arduino\Src\VC\Arduino.VC\ProxxonMF70\ProxxonMF70.csv";
		int _fileNo=1;

		private void LoadSamples_Click(object sender, RoutedEventArgs e)
		{
			_fileNo = 1;
			LoadFile();
		}
		private void LoadFile()
		{
			try
			{
				string filename = GetFilename(_fileNo);
				_list.ReadFiles(filename);
				_fileNoLbl.Content = _fileNo.ToString();
				_chart.List = _list;
				_chart.InvalidateVisual();
			}
			catch(FileNotFoundException e)
			{
				MessageBox.Show(e.Message);
			}
			catch (DirectoryNotFoundException e)
			{
				MessageBox.Show(e.Message);
			}
		}

		private string GetFilename(int fileno)
		{
			string filename = _filename;
			if (fileno > 1)
			{
				int dotidx = filename.LastIndexOf('.');
				if (dotidx >= 0)
				{
					filename = filename.Insert(dotidx, "#" + fileno.ToString());
				}
			}
			return filename;
		}
		private void ZoomOut_Click(object sender, RoutedEventArgs e)
		{
			_chart.ScaleX = _chart.ScaleX / 1.1;
			_chart.ScaleY = _chart.ScaleY / 1.1;
			_chart.InvalidateVisual();
		}
		private void ZoomIn_Click(object sender, RoutedEventArgs e)
		{
			_chart.ScaleX = _chart.ScaleX * 1.1;
			_chart.ScaleY = _chart.ScaleY * 1.1;
			_chart.InvalidateVisual();
		}
		private void XP_Click(object sender, RoutedEventArgs e)
		{
			_chart.OffsetX += 0.1 * _chart.ScaleX;
			_chart.InvalidateVisual();
		}
		private void XM_Click(object sender, RoutedEventArgs e)
		{
			_chart.OffsetX -= 0.1 * _chart.ScaleX;
			_chart.InvalidateVisual();
		}

		private void FileP_Click(object sender, RoutedEventArgs e)
		{
			if (File.Exists(GetFilename(_fileNo+1)))
			{
				_fileNo++;
				LoadFile();
			}
		}
		private void FileM_Click(object sender, RoutedEventArgs e)
		{
			if (_fileNo > 1 && File.Exists(GetFilename(_fileNo - 1)))
			{
				_fileNo--;
				LoadFile();
			}
		}

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
				_filename = dlg.FileName;
				_fileNo = 1;
				LoadFile();
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			Point pt = e.GetPosition(this);

			var ts = _chart.HitTest(pt);

			if (ts != null)
			{
				_pos.Content = ts.Index + ": " + ts.Info;
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
