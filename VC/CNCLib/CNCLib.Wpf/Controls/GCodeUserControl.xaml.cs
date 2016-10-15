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

using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using CNCLib.GCode.Commands;
using CNCLib.GUI;
using Framework.Tools.Drawing;

namespace CNCLib.Wpf.Controls
{
	/// <summary>
	/// Interaction logic for GCodeUserControl.xaml
	/// </summary>
	public partial class GCodeUserControl : System.Windows.Controls.UserControl
	{
		private GCodeBitmapDraw _bitmapDraw = new GCodeBitmapDraw();

		public GCodeUserControl()
		{
			InitializeComponent();

			MouseWheel += GCodeUserControl_MouseWheel;

			MouseDown += GCodeUserControl_MouseDown;
			MouseUp   += GCodeUserControl_MouseUp;
			MouseMove += GCodeUserControl_MouseMove;
		}

		#region Properties


		/// <summary>
		/// Command Property
		/// </summary>
		public static DependencyProperty CommandsProperty = DependencyProperty.Register("Commands", typeof(CommandList), typeof(GCodeUserControl), new PropertyMetadata(OnCommandsChanged));
		public CommandList Commands
		{
			get { return (CommandList)GetValue(CommandsProperty); }
			set { SetValue(CommandsProperty, value); }
		}
		private static void OnCommandsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var godeCtrl = (GCodeUserControl)dependencyObject;
			godeCtrl.InvalidateVisual();
		}

		#region View Properties

		/// <summary>
		/// Zoom Property
		/// </summary>
		public static DependencyProperty ZoomProperty = DependencyProperty.Register("Zoom", typeof(double), typeof(GCodeUserControl), new PropertyMetadata(OnZoomChanged));
		public double Zoom
		{
			get { return (double)GetValue(ZoomProperty); }
			set { SetValue(ZoomProperty, value); }
		}
		private static void OnZoomChanged(DependencyObject dependencyObject,  DependencyPropertyChangedEventArgs e)
		{
			var godeCtrl = (GCodeUserControl)dependencyObject;
			godeCtrl._bitmapDraw.Zoom = (double) e.NewValue;
			godeCtrl.InvalidateVisual();
		}

		/// <summary>
		/// OffsetX Property
		/// </summary>
		public static DependencyProperty OffsetXProperty = DependencyProperty.Register("OffsetX", typeof(decimal), typeof(GCodeUserControl), new PropertyMetadata(OnOffsetXChanged));
		public decimal OffsetX
		{
			get { return (decimal)GetValue(OffsetXProperty); }
			set { SetValue(OffsetXProperty, value); }
		}
		private static void OnOffsetXChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var godeCtrl = (GCodeUserControl)dependencyObject;
			godeCtrl._bitmapDraw.OffsetX = (decimal)e.NewValue;
			godeCtrl.InvalidateVisual();
		}

		/// <summary>
		/// OffsetY Property
		/// </summary>
		public static DependencyProperty OffsetYProperty = DependencyProperty.Register("OffsetY", typeof(decimal), typeof(GCodeUserControl), new PropertyMetadata(OnOffsetYChanged));
		public decimal OffsetY
		{
			get { return (decimal)GetValue(OffsetYProperty); }
			set { SetValue(OffsetYProperty, value); }
		}
		private static void OnOffsetYChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var godeCtrl = (GCodeUserControl)dependencyObject;
			godeCtrl._bitmapDraw.OffsetY = (decimal)e.NewValue;
			godeCtrl.InvalidateVisual();
		}

		#endregion

		#region Color Properties

		/// <summary>
		/// MachineColor Property
		/// </summary>
		public static DependencyProperty MachineColorProperty = DependencyProperty.Register("MachineColor", typeof(Color), typeof(GCodeUserControl), new PropertyMetadata(Colors.Black,OnMachineColorChanged));
		public Color MachineColor
		{
			get { return (Color)GetValue(MachineColorProperty); }
			set { SetValue(MachineColorProperty, value); }
		}
		private static void OnMachineColorChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var godeCtrl = (GCodeUserControl)dependencyObject;
			godeCtrl._bitmapDraw.MachineColor = ColorToColor((Color)e.NewValue);
			godeCtrl.InvalidateVisual();
		}

		/// <summary>
		/// LaserOnColor Property
		/// </summary>
		public static DependencyProperty LaserOnColorProperty = DependencyProperty.Register("LaserOnColor", typeof(Color), typeof(GCodeUserControl), new PropertyMetadata(Colors.Red,OnLaserOnColorChanged));
		public Color LaserOnColor
		{
			get { return (Color)GetValue(LaserOnColorProperty); }
			set { SetValue(LaserOnColorProperty, value); }
		}
		private static void OnLaserOnColorChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var godeCtrl = (GCodeUserControl)dependencyObject;
			godeCtrl._bitmapDraw.LaserOnColor = ColorToColor((Color)e.NewValue);
			godeCtrl.InvalidateVisual();
		}

		/// <summary>
		/// LaserOffColor Property
		/// </summary>
		public static DependencyProperty LaserOffColorProperty = DependencyProperty.Register("LaserOffColor", typeof(Color), typeof(GCodeUserControl), new PropertyMetadata(Colors.Orange,OnLaserOffColorChanged));
		public Color LaserOffColor
		{
			get { return (Color)GetValue(LaserOffColorProperty); }
			set { SetValue(LaserOffColorProperty, value); }
		}
		private static void OnLaserOffColorChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var godeCtrl = (GCodeUserControl)dependencyObject;
			godeCtrl._bitmapDraw.LaserOffColor = ColorToColor((Color)e.NewValue);
			godeCtrl.InvalidateVisual();
		}

		/// <summary>
		/// CutColor Property
		/// </summary>
		public static DependencyProperty CutColorProperty = DependencyProperty.Register("CutColor", typeof(Color), typeof(GCodeUserControl), new PropertyMetadata(Colors.Orange, OnCutColorChanged));
		public Color CutColor
		{
			get { return (Color)GetValue(CutColorProperty); }
			set { SetValue(CutColorProperty, value); }
		}
		private static void OnCutColorChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var godeCtrl = (GCodeUserControl)dependencyObject;
			godeCtrl._bitmapDraw.CutColor = ColorToColor((Color)e.NewValue);
			godeCtrl.InvalidateVisual();
		}

		/// <summary>
		/// CutDotColor Property
		/// </summary>
		public static DependencyProperty CutDotColorProperty = DependencyProperty.Register("CutDotColor", typeof(Color), typeof(GCodeUserControl), new PropertyMetadata(Colors.Orange, OnCutDotColorChanged));
		public Color CutDotColor
		{
			get { return (Color)GetValue(CutDotColorProperty); }
			set { SetValue(CutDotColorProperty, value); }
		}
		private static void OnCutDotColorChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var godeCtrl = (GCodeUserControl)dependencyObject;
			godeCtrl._bitmapDraw.CutDotColor = ColorToColor((Color)e.NewValue);
			godeCtrl.InvalidateVisual();
		}


		/// <summary>
		/// CutArcColor Property
		/// </summary>
		public static DependencyProperty CutEllipseColorProperty = DependencyProperty.Register("CutEllipseColor", typeof(Color), typeof(GCodeUserControl), new PropertyMetadata(Colors.Orange, OnCutEllipseColorChanged));
		public Color CutEllipseColor
		{
			get { return (Color)GetValue(CutEllipseColorProperty); }
			set { SetValue(CutEllipseColorProperty, value); }
		}
		private static void OnCutEllipseColorChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var godeCtrl = (GCodeUserControl)dependencyObject;
			godeCtrl._bitmapDraw.CutEllipseColor = ColorToColor((Color)e.NewValue);
			godeCtrl.InvalidateVisual();
		}

		/// <summary>
		/// CutArcColor Property
		/// </summary>
		public static DependencyProperty CutArcColorProperty = DependencyProperty.Register("CutArcColor", typeof(Color), typeof(GCodeUserControl), new PropertyMetadata(Colors.Orange, OnCutArcColorChanged));
		public Color CutArcColor
		{
			get { return (Color)GetValue(CutArcColorProperty); }
			set { SetValue(CutArcColorProperty, value); }
		}
		private static void OnCutArcColorChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var godeCtrl = (GCodeUserControl)dependencyObject;
			godeCtrl._bitmapDraw.CutArcColor = ColorToColor((Color)e.NewValue);
			godeCtrl.InvalidateVisual();
		}

		/// <summary>
		/// FastMoveColor Property
		/// </summary>
		public static DependencyProperty FastMoveColorProperty = DependencyProperty.Register("FastMoveColor", typeof(Color), typeof(GCodeUserControl), new PropertyMetadata(Colors.Orange, OnFastMoveColorChanged));
		public Color FastMoveColor
		{
			get { return (Color)GetValue(FastMoveColorProperty); }
			set { SetValue(FastMoveColorProperty, value); }
		}
		private static void OnFastMoveColorChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var godeCtrl = (GCodeUserControl)dependencyObject;
			godeCtrl._bitmapDraw.FastMoveColor = ColorToColor((Color)e.NewValue);
			godeCtrl.InvalidateVisual();
		}

		#endregion

		/// <summary>
		/// LaserSize Property
		/// </summary>
		public static DependencyProperty LaserSizeProperty = DependencyProperty.Register("LaserSize", typeof(decimal), typeof(GCodeUserControl), new PropertyMetadata(0.254m, OnLaserSizeChanged));
		public decimal LaserSize
		{
			get { return (decimal)GetValue(LaserSizeProperty); }
			set { SetValue(LaserSizeProperty, value); }
		}
		private static void OnLaserSizeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var godeCtrl = (GCodeUserControl)dependencyObject;
			godeCtrl._bitmapDraw.LaserSize = (decimal)e.NewValue;
			godeCtrl.InvalidateVisual();
		}

		/// <summary>
		/// CutterSize Property
		/// </summary>
		public static DependencyProperty CutterSizeProperty = DependencyProperty.Register("CutterSize", typeof(decimal), typeof(GCodeUserControl), new PropertyMetadata(0.254m, OnCutterSizeChanged));
		public decimal CutterSize
		{
			get { return (decimal)GetValue(CutterSizeProperty); }
			set { SetValue(CutterSizeProperty, value); }
		}
		private static void OnCutterSizeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var godeCtrl = (GCodeUserControl)dependencyObject;
			godeCtrl._bitmapDraw.CutterSize = (decimal)e.NewValue;
			godeCtrl.InvalidateVisual();
		}

		/// <summary>
		/// MouseOverX Property
		/// </summary>
		private static readonly DependencyPropertyKey MouseOverPositionXPropertyKey =
					  DependencyProperty.RegisterReadOnly("MouseOverPositionX",
					  typeof(decimal?), typeof(GCodeUserControl),
					  new FrameworkPropertyMetadata(null));

		public static readonly DependencyProperty MouseOverPositionXProperty =
					  MouseOverPositionXPropertyKey.DependencyProperty;

		public decimal? MouseOverPositionX
		{
			get { return (decimal?)GetValue(MouseOverPositionXProperty); }
			private set { SetValue(MouseOverPositionXPropertyKey, value); }
		}

		/// <summary>
		/// MouseOverY Property
		/// </summary>
		private static readonly DependencyPropertyKey MouseOverPositionYPropertyKey =
					  DependencyProperty.RegisterReadOnly("MouseOverPositionY",
					  typeof(decimal?), typeof(GCodeUserControl),
					  new FrameworkPropertyMetadata(null));

		public static readonly DependencyProperty MouseOverPositionYProperty =
					  MouseOverPositionYPropertyKey.DependencyProperty;

		public decimal? MouseOverPositionY
		{
			get { return (decimal?)GetValue(MouseOverPositionYProperty); }
			private set { SetValue(MouseOverPositionYPropertyKey, value); }
		}

		#endregion

		#region Drag/Drop

		private bool _isdragging = false;
		private Point3D _mouseDown;
		private decimal _mouseDownOffsetX;
		private decimal _mouseDownOffsetY;
		private Stopwatch _sw = new Stopwatch();

		private void GCodeUserControl_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (e.Delta > 0)
				Zoom *= 1.1;
			else
				Zoom /= 1.1;

			InvalidateVisual();
		}

		bool _rotated = false;

		private void GCodeUserControl_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.RightButton == MouseButtonState.Pressed)
			{
				if (_rotated)
					_bitmapDraw.Rotate = new Rotate3D();
				else
					_bitmapDraw.Rotate = new Rotate3D(45.0/180.0*Math.PI, new double[] { -1, -1, 0 });
				_rotated = !_rotated;
				InvalidateVisual();
			}
			else
			{
				if (!_isdragging)
				{
					var pt = new System.Drawing.Point((int) e.GetPosition(this).X, (int) e.GetPosition(this).Y);
					_mouseDown = _bitmapDraw.FromClient(pt);
					_mouseDownOffsetX = OffsetX;
					_mouseDownOffsetY = OffsetY;
					_sw.Start();
					Mouse.Capture(this);
				}
				_isdragging = true;
			}
		}

		private void GCodeUserControl_MouseMove(object sender, MouseEventArgs e)
		{
			var pt = new System.Drawing.Point((int)e.GetPosition(this).X, (int)e.GetPosition(this).Y);
			var gcodePosition = _bitmapDraw.FromClient(pt);
			MouseOverPositionX = gcodePosition.X;
			MouseOverPositionY = gcodePosition.Y;

			if (_isdragging)
			{
				OffsetX = _mouseDownOffsetX;
				OffsetY = _mouseDownOffsetY;
				Point3D c = _bitmapDraw.FromClient(pt);
				decimal newX = _mouseDownOffsetX - (c.X.Value - _mouseDown.X.Value);
				decimal newY = _mouseDownOffsetY + (c.Y.Value - _mouseDown.Y.Value);
				OffsetX = newX;
				OffsetY = newY;
				if (_sw.ElapsedMilliseconds > 300)
				{
					_sw.Start();
					InvalidateVisual();
				}
			}
		}

		private void GCodeUserControl_MouseUp(object sender, MouseEventArgs e)
		{
			if (_isdragging)
			{
				Mouse.Capture(null);
				InvalidateVisual();
			}
			_isdragging = false;
		}

		#endregion

		#region render
		protected override void OnRenderSizeChanged(System.Windows.SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);
			_bitmapDraw.RenderSize = new System.Drawing.Size((int) sizeInfo.NewSize.Width, (int) sizeInfo.NewSize.Height);
			InvalidateVisual();
		}

		protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);
			this.DrawCommands(drawingContext);
		}

		#endregion

		#region private

		static Color ColorToColor(System.Drawing.Color color)
		{
			return Color.FromArgb(color.A, color.R, color.G, color.B);
		}

		static System.Drawing.Color ColorToColor(Color color)
		{
			return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
		}


		private void DrawCommands(System.Windows.Media.DrawingContext context)
		{
			if (_bitmapDraw.RenderSize.Height == 0 || _bitmapDraw.RenderSize.Width == 0)
				return;

			if (Global.Instance.Machine != null)
			{
				_bitmapDraw.SizeX = Global.Instance.Machine.SizeX;
				_bitmapDraw.SizeY = Global.Instance.Machine.SizeY;
			}

			var curBitmap = _bitmapDraw.DrawToBitmap(Commands);
			MemoryStream stream = new MemoryStream();
			curBitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
			var cc = new System.Windows.Media.ImageSourceConverter().ConvertFrom(stream);
			context.DrawImage((System.Windows.Media.ImageSource)cc, new System.Windows.Rect(0,0, this.ActualWidth, this.ActualHeight));
			curBitmap.Dispose();
		}

		#endregion

	}
}
