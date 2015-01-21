
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SpeedChart.ViewModels
{
   public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        TimeSampleList _list = new TimeSampleList();
        string _filename;
        string _directory;
        int _fileNo = 1;
        bool _loaded = false;

        public MainWindowViewModel()
        {
            _directory = System.IO.Path.GetTempPath();
            _filename = _directory + @"ProxxonMF70.csv";
        }

        #region Environment

        public class DelegateCommand : ICommand
        {
            private readonly Action _command;
            private readonly Func<bool> _canExecute;
            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public DelegateCommand(Action command, Func<bool> canExecute = null)
            {
                if (command == null)
                    throw new ArgumentNullException();
                _canExecute = canExecute;
                _command = command;
            }

            public void Execute(object parameter)
            {
                _command();
            }

            public bool CanExecute(object parameter)
            {
                return _canExecute == null || _canExecute();
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public MainWindow ViewWindow { get; set; }

        #endregion

        #region Properties

        public int FileNo
        {
            get { return _fileNo; }
            set { _fileNo = value; OnPropertyChanged(); }
        }

        #endregion

        #region Operations

        public void Browse()
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = _directory;
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
                _directory = System.IO.Path.GetDirectoryName(_filename) + @"\";
                LoadFile();
            }
        }

       public bool CanBrowse()
        {
            return true;
        }

        void Load()
        {
			FileNo = 1;
			LoadFile();
		}

		private void LoadFile()
		{
			try
			{
				string filename = GetFilename(_fileNo);
				_list.ReadFiles(filename);
                ViewWindow.SpeedChart.List = _list;
                ViewWindow.SpeedChart.InvalidateVisual();
                _loaded=true;
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
			if (fileno != 1)
			{
				int dotidx = filename.LastIndexOf('.');
				if (dotidx >= 0)
				{
					filename = filename.Insert(dotidx, "#" + fileno.ToString());
				}
			}
			return filename;
		}

        bool CanLoad()
        {
            return true;
        }

        public void LoadNextFile()
        {
            if (CanLoadNextFile())
            {
                FileNo++;
                LoadFile();
            }
        }

        public bool CanLoadNextFile()
        {
            return _loaded && File.Exists(GetFilename(FileNo + 1));
        }

        public void LoadPrevFile()
        {
            if (CanLoadPrevFile())
            {
                FileNo--;
                LoadFile();
            }
        }

        public bool CanLoadPrevFile()
        {
            return _loaded &&  File.Exists(GetFilename(FileNo - 1));
        }

        #endregion

        #region Commands

        public ICommand BrowseCommand { get { return new DelegateCommand(Browse, CanBrowse); } }
        public ICommand LoadCommand { get { return new DelegateCommand(Load, CanLoad); } }
        public ICommand LoadNextCommand { get { return new DelegateCommand(LoadNextFile, CanLoadNextFile); } }
        public ICommand LoadPrevCommand { get { return new DelegateCommand(LoadPrevFile, CanLoadPrevFile); } }

        #endregion
    }
}
