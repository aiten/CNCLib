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

        public ICommand RightMinMinCommand => new DelegateCommand(() => OnRight(MoveDistance.MinMinDistance),CanSend);
        public ICommand RightMinCommand => new DelegateCommand(() => OnRight(MoveDistance.MinDistance), CanSend);
        public ICommand RightDefaultCommand => new DelegateCommand(() => OnRight(MoveDistance.DefaultDistance), CanSend);
        public ICommand RightMaxCommand => new DelegateCommand(() => OnRight(MoveDistance.MaxDistance), CanSend);

        #endregion

        #region Properties

        public enum MoveDistance
        {
            DefaultDistance,
            MaxDistance,
            MinDistance,
            MinMinDistance,
            UseDefault
        };

        private double _moveDist = 10.0;
        private double _moveDistDefault = 10.0;
        private double _moveDistMax = 100.0;
        private double _moveDistMin = 1.0;
        private double _moveDistMinMin = 0.1;

        public double MoveDist
        {
            get => _moveDist;
            set
            {
                _moveDist = value;
                RaisePropertyChanged(nameof(MoveDist));
            }
        }

        public double MoveDistDefault
        {
            get => _moveDistDefault;
            set { _moveDistDefault = value; RaisePropertyChanged(); }
        }

        public double MoveDistMax
        {
            get => _moveDistMax;
            set { _moveDistMax = value; RaisePropertyChanged(); }
        }

        public double MoveDistMin
        {
            get => _moveDistMin;
            set
            {
                _moveDistMin = value;
                RaisePropertyChanged();
            }
        }
        public double MoveDistMinMin
        {
            get => _moveDistMinMin;
            set
            {
                _moveDistMinMin = value;
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
                return MoveDistance.MinMinDistance;
            if (isShift)
                return MoveDistance.MaxDistance;
            if (isCtrl)
                return MoveDistance.MinDistance;
            return MoveDistance.DefaultDistance;
        }

        private double GetDist(MoveDistance mode)
        {
            switch (mode)
            {
                case MoveDistance.UseDefault: return MoveDist;
                case MoveDistance.MaxDistance: return MoveDistMax;
                case MoveDistance.MinDistance: return MoveDistMin;
                case MoveDistance.MinMinDistance: return MoveDistMinMin;
                default:
                case MoveDistance.DefaultDistance: return MoveDistDefault;
            }
        }

        #endregion
    }
}
