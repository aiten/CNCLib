////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
			_dist.Text = MoveDist.ToString();
		}

		#region Commands

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

		public static readonly DependencyProperty ZUpCommandProperty = DependencyProperty.Register(
			"ZUp", typeof(ICommand), typeof(MoveUserControl), new PropertyMetadata(default(ICommand)));

		public ICommand ZUp
		{
			get { return (ICommand)GetValue(ZUpCommandProperty); }
			set { SetValue(ZUpCommandProperty, value); }
		}

		public static readonly DependencyProperty ZDownCommandProperty = DependencyProperty.Register(
			"ZDown", typeof(ICommand), typeof(MoveUserControl), new PropertyMetadata(default(ICommand)));

		public ICommand ZDown
		{
			get { return (ICommand)GetValue(ZDownCommandProperty); }
			set { SetValue(ZDownCommandProperty, value); }
		}

		#endregion

		#region Properties

		public double MoveDist { get; set; } = 10.0;
		public double MoveDistDefault { get; set; } = 10.0;
		public double MoveDistMax { get; set; } = 100.0;
		public double MoveDistMin { get; set; } = 1.0;
		public double MoveDistMinMin { get; set; } = 0.1;

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
				case Key.PageUp:
					OnZUp();
					e.Handled = true;
					break;
				case Key.PageDown:
					OnZDown();
					e.Handled = true;
					break;
			}
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
		private void _Zup_Click(object sender, RoutedEventArgs e)
		{
			OnZUp();
		}

		private void _Zdown_Click(object sender, RoutedEventArgs e)
		{
			OnZDown();
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

		private void OnZDown()
		{
			double dist = MoveDist;
			if (ZDown != null && ZDown.CanExecute(dist))
				ZDown.Execute(dist);
		}

		private void OnZUp()
		{
			double dist = MoveDist;
			if (ZUp != null && ZUp.CanExecute(dist))
				ZUp.Execute(dist);
		}

		private void OnDown()
		{
			double dist = MoveDist;
			if (Down != null && Down.CanExecute(dist))
				Down.Execute(dist);
		}

		private void OnUp()
		{
			double dist = MoveDist;
			if (Up != null && Up.CanExecute(dist))
				Up.Execute(dist);
		}

		private void OnRight()
		{
			double dist = MoveDist;
			if (Right != null && Right.CanExecute(dist))
				Right.Execute(dist);
		}

		private void OnLeft()
		{
			double dist = MoveDist;
			if (Left != null && Left.CanExecute(dist))
				Left.Execute(dist);
		}

		#endregion

		#region helper

		private void SetMoveDist()
		{
			MoveDist = GetDist();
			_dist.Text = MoveDist.ToString();
		}

		private double GetDist()
		{
			double dist;
			bool isCtrl = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
			bool isShift = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
			if (isCtrl && isShift)
				dist = MoveDistMinMin;
			else if (isShift)
				dist = MoveDistMax;
			else if (isCtrl)
				dist = MoveDistMin;
			else
				dist = MoveDistDefault;
			return dist;
		}

		#endregion
	}
}
