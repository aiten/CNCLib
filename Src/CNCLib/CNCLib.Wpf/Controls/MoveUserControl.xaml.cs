////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

  CNCLib is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  CNCLib is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.
  http://www.gnu.org/licenses/
*/

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Framework.Wpf.Helpers;

namespace CNCLib.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for MoveUserControl.xaml
    /// </summary>
    public partial class MoveUserControl : UserControl, INotifyPropertyChanged
    {
        public MoveUserControl()
        {
            InitializeComponent();
            _dist.Text = MoveDist.ToString();
        }

        #region INPC 

        public virtual event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Commands

        public static readonly DependencyProperty LeftCommandProperty = DependencyProperty.Register(
            "Left", typeof(ICommand), typeof(MoveUserControl), new PropertyMetadata(default(ICommand)));

        public ICommand Left
        {
            get => (ICommand)GetValue(LeftCommandProperty);
            set => SetValue(LeftCommandProperty, value);
        }

        public static readonly DependencyProperty RightCommandProperty = DependencyProperty.Register(
            "Right", typeof(ICommand), typeof(MoveUserControl), new PropertyMetadata(default(ICommand)));

        public ICommand Right
        {
            get => (ICommand)GetValue(RightCommandProperty);
            set => SetValue(RightCommandProperty, value);
        }

        public static readonly DependencyProperty UpCommandProperty = DependencyProperty.Register(
            "Up", typeof(ICommand), typeof(MoveUserControl), new PropertyMetadata(default(ICommand)));

        public ICommand Up
        {
            get => (ICommand)GetValue(UpCommandProperty);
            set => SetValue(UpCommandProperty, value);
        }

        public static readonly DependencyProperty DownCommandProperty = DependencyProperty.Register(
            "Down", typeof(ICommand), typeof(MoveUserControl), new PropertyMetadata(default(ICommand)));

        public ICommand Down
        {
            get => (ICommand)GetValue(DownCommandProperty);
            set => SetValue(DownCommandProperty, value);
        }

        public static readonly DependencyProperty ZUpCommandProperty = DependencyProperty.Register(
            "ZUp", typeof(ICommand), typeof(MoveUserControl), new PropertyMetadata(default(ICommand)));

        public ICommand ZUp
        {
            get => (ICommand)GetValue(ZUpCommandProperty);
            set => SetValue(ZUpCommandProperty, value);
        }

        public static readonly DependencyProperty ZDownCommandProperty = DependencyProperty.Register(
            "ZDown", typeof(ICommand), typeof(MoveUserControl), new PropertyMetadata(default(ICommand)));

        public ICommand ZDown
        {
            get => (ICommand)GetValue(ZDownCommandProperty);
            set => SetValue(ZDownCommandProperty, value);
        }

        #endregion

        #region ICommand

        bool CanSend()
        {
            return true;
        }

        public ICommand RightDist1Command => new DelegateCommand(() => OnRight(MoveDistance.Distance1),CanSend);
        public ICommand RightDist2Command => new DelegateCommand(() => OnRight(MoveDistance.Distance2), CanSend);
        public ICommand RightDist3Command => new DelegateCommand(() => OnRight(MoveDistance.Distance3), CanSend);
        public ICommand RightDist4Command => new DelegateCommand(() => OnRight(MoveDistance.Distance4), CanSend);

        public ICommand LeftDist1Command => new DelegateCommand(() => OnLeft(MoveDistance.Distance1), CanSend);
        public ICommand LeftDist2Command => new DelegateCommand(() => OnLeft(MoveDistance.Distance2), CanSend);
        public ICommand LeftDist3Command => new DelegateCommand(() => OnLeft(MoveDistance.Distance3), CanSend);
        public ICommand LeftDist4Command => new DelegateCommand(() => OnLeft(MoveDistance.Distance4), CanSend);


        public ICommand UpDist1Command => new DelegateCommand(() => OnUp(MoveDistance.Distance1), CanSend);
        public ICommand UpDist2Command => new DelegateCommand(() => OnUp(MoveDistance.Distance2), CanSend);
        public ICommand UpDist3Command => new DelegateCommand(() => OnUp(MoveDistance.Distance3), CanSend);
        public ICommand UpDist4Command => new DelegateCommand(() => OnUp(MoveDistance.Distance4), CanSend);

        public ICommand DownDist1Command => new DelegateCommand(() => OnDown(MoveDistance.Distance1), CanSend);
        public ICommand DownDist2Command => new DelegateCommand(() => OnDown(MoveDistance.Distance2), CanSend);
        public ICommand DownDist3Command => new DelegateCommand(() => OnDown(MoveDistance.Distance3), CanSend);
        public ICommand DownDist4Command => new DelegateCommand(() => OnDown(MoveDistance.Distance4), CanSend);


        public ICommand ZUpDist1Command => new DelegateCommand(() => OnZUp(MoveDistance.Distance1), CanSend);
        public ICommand ZUpDist2Command => new DelegateCommand(() => OnZUp(MoveDistance.Distance2), CanSend);
        public ICommand ZUpDist3Command => new DelegateCommand(() => OnZUp(MoveDistance.Distance3), CanSend);
        public ICommand ZUpDist4Command => new DelegateCommand(() => OnZUp(MoveDistance.Distance4), CanSend);
                        
        public ICommand ZDownDist1Command => new DelegateCommand(() => OnZDown(MoveDistance.Distance1), CanSend);
        public ICommand ZDownDist2Command => new DelegateCommand(() => OnZDown(MoveDistance.Distance2), CanSend);
        public ICommand ZDownDist3Command => new DelegateCommand(() => OnZDown(MoveDistance.Distance3), CanSend);
        public ICommand ZDownDist4Command => new DelegateCommand(() => OnZDown(MoveDistance.Distance4), CanSend);

        #endregion

        #region Properties

        public enum MoveDistance
        {
            Distance3,
            Distance4,
            Distance2,
            Distance1,
            UseDefault
        };

        private double _moveDist = 10.0;
        private double _moveDistance3 = 10.0;
        private double _moveDistanze4 = 100.0;
        private double _moveDistance2 = 1.0;
        private double _moveDistance1 = 0.1;

        public double MoveDist
        {
            get => _moveDist;
            set
            {
                _moveDist = value;
                RaisePropertyChanged(nameof(MoveDist));
            }
        }

        public double MoveDistance3
        {
            get => _moveDistance3;
            set { _moveDistance3 = value; RaisePropertyChanged(); }
        }

        public double MoveDistance4
        {
            get => _moveDistanze4;
            set { _moveDistanze4 = value; RaisePropertyChanged(); }
        }

        public double MoveDistance2
        {
            get => _moveDistance2;
            set
            {
                _moveDistance2 = value;
                RaisePropertyChanged();
            }
        }
        public double MoveDistance1
        {
            get => _moveDistance1;
            set
            {
                _moveDistance1 = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region private events

        private bool IsModifyMoveDistKey(Key k)
        {
            switch (k)
            {
                case Key.LeftCtrl:
                case Key.RightCtrl:
                case Key.LeftShift:
                case Key.RightShift:
                    return true;
            }
            return false;
        }

        private void Grid_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (IsModifyMoveDistKey(e.Key))
                SetMoveDist();
        }

        private void Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (IsModifyMoveDistKey(e.Key))
                SetMoveDist();

            switch (e.Key)
            {
                case Key.Left:
                    OnLeft(MoveDistance.UseDefault);
                    e.Handled = true;
                    break;
                case Key.Right:
                    OnRight(MoveDistance.UseDefault);
                    e.Handled = true;
                    break;
                case Key.Up:
                    OnUp(MoveDistance.UseDefault);
                    e.Handled = true;
                    break;
                case Key.Down:
                    OnDown(MoveDistance.UseDefault);
                    e.Handled = true;
                    break;
                case Key.PageUp:
                    OnZUp(MoveDistance.UseDefault);
                    e.Handled = true;
                    break;
                case Key.PageDown:
                    OnZDown(MoveDistance.UseDefault);
                    e.Handled = true;
                    break;
            }
        }
 
        private void _dist_TextChanged(object sender, TextChangedEventArgs e)
        {
            double movedist;
            if (double.TryParse(_dist.Text, out movedist))
            {
                MoveDist = movedist;
            }
        }

        #endregion

        #region event forwards

        private void OnZDown(MoveDistance mode)
        {
            double? dist = GetDist(mode);
            if (ZDown != null && ZDown.CanExecute(dist))
                ZDown.Execute(dist);
        }

        private void OnZUp(MoveDistance mode)
        {
            double? dist = GetDist(mode);
            if (ZUp != null && ZUp.CanExecute(dist))
                ZUp.Execute(dist);
        }

        private void OnDown(MoveDistance mode)
        {
            double? dist = GetDist(mode);
            if (Down != null && Down.CanExecute(dist))
                Down.Execute(dist);
        }

        private void OnUp(MoveDistance mode)
        {
            double? dist = GetDist(mode);
            if (Up != null && Up.CanExecute(dist))
                Up.Execute(dist);
        }

        private void OnRight(MoveDistance mode)
        {
            double? dist = GetDist(mode);
            if (Right != null && Right.CanExecute(dist))
                Right.Execute(dist);
        }

        private void OnLeft(MoveDistance mode)
        {
            double? dist = GetDist(mode);
            if (Left != null && Left.CanExecute(dist))
                Left.Execute(dist);
        }

        #endregion

        #region helper

        private void SetMoveDist()
        {
            MoveDist = GetDist(GetDistanceMode());
            _dist.Text = MoveDist.ToString();
        }

        private MoveDistance GetDistanceMode()
        {
            bool isCtrl = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            bool isShift = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            if (isCtrl && isShift)
                return MoveDistance.Distance1;
            if (isShift)
                return MoveDistance.Distance4;
            if (isCtrl)
                return MoveDistance.Distance2;
            return MoveDistance.Distance3;
        }

        private double GetDist(MoveDistance mode)
        {
            switch (mode)
            {
                case MoveDistance.UseDefault: return MoveDist;
                case MoveDistance.Distance4: return MoveDistance4;
                case MoveDistance.Distance2: return MoveDistance2;
                case MoveDistance.Distance1: return MoveDistance1;
                default:
                case MoveDistance.Distance3: return MoveDistance3;
            }
        }

        #endregion
    }
}
