using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Roboter.Wpf.Helpers;
using Roboter.Wpf.Models;
using Framework.Wpf.ViewModels;
using Framework.Wpf.Helpers;

namespace Roboter.Wpf.ViewModels
{
    public class _MainWindowViewModel : BaseViewModel
    {
        #region Properties

        #region MyDateTime

        private DateTime _myDateTime;
        public DateTime MyDateTime
        {
            get { return _myDateTime; }
            set
            {
                if (_myDateTime != value)
                {
                    _myDateTime = value;
                    RaisePropertyChanged(() => MyDateTime);
                }
            }
        }

        #endregion

        #region PersonsCollection

        private ObservableCollection<_Person> _personsCollection;
        public ObservableCollection<_Person> PersonsCollection
        {
            get { return _personsCollection; }
            set
            {
                if (_personsCollection != value)
                {
                    _personsCollection = value;
                    RaisePropertyChanged(() => PersonsCollection);
                }
            }
        }

        #endregion

        #endregion

        #region Commands

        public ICommand RefreshDateCommand { get { return new DelegateCommand(OnRefreshDate); } }
        public ICommand RefreshPersonsCommand { get { return new DelegateCommand(OnRefreshPersons); } }
        public ICommand DoNothingCommand { get { return new DelegateCommand(OnDoNothing, CanExecuteDoNothing); } }

        #endregion

        #region Ctor
        public _MainWindowViewModel()
        {
            RandomizeData();
        }
        #endregion

        #region Command Handlers

        private void OnRefreshDate()
        {
            MyDateTime = DateTime.Now;
        }

        private void OnRefreshPersons()
        {
            RandomizeData();
        }

        private void OnDoNothing()
        {
        }

        private bool CanExecuteDoNothing()
        {
            return false;
        }

        #endregion

        private void RandomizeData()
        {
            PersonsCollection = new ObservableCollection<_Person>();

            for (var i = 0; i < 10; i++)
            {
                PersonsCollection.Add(new _Person(
                    RandomHelper.RandomString(10, true),
                    RandomHelper.RandomInt(1, 43),
                    RandomHelper.RandomBool(),
                    RandomHelper.RandomNumber(50, 180, 1),
                    RandomHelper.RandomDate(new DateTime(1980, 1, 1), DateTime.Now),
                    RandomHelper.RandomColor()
                    ));
            }
        }
    }
}