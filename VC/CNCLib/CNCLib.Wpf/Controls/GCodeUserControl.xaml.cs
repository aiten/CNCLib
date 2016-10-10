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
using System.Collections.Generic;
using System.IO;
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
using CNCLib.GCode.Commands;
using CNCLib.GCode.Load;
using CNCLib.GUI;
using CNCLib.Logic.Contracts.DTO;
using Framework.Arduino;
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

			LoadOptions loadinfo = new LoadOptions();
			loadinfo.FileName = @"c:\tmp\test.nc";
			loadinfo.LoadType = LoadOptions.ELoadType.GCode;
			LoadBase load = LoadBase.Create(loadinfo);

			load.LoadOptions = loadinfo;
			load.Load();

			_bitmapDraw.RenderSize = new System.Drawing.Size((int) ActualWidth, (int)ActualHeight);

			_bitmapDraw.Commands.Clear();
			_bitmapDraw.Commands.AddRange(load.Commands);
			Zoom = 1.25;
		}

		#region Properties

		public static DependencyProperty ZoomProperty = DependencyProperty.Register("Zoom", typeof(double), typeof(GCodeUserControl));
		public double Zoom
		{
			get { return (double)GetValue(ZoomProperty); }
			set { SetValue(ZoomProperty, value); _bitmapDraw.Zoom = value; }
		}

		#endregion

		#region private Members

		private ArduinoSerialCommunication Com
		{
			get { return Framework.Tools.Pattern.Singleton<ArduinoSerialCommunication>.Instance; }
		}

		#endregion

		#region private

		private void ReInitDraw()
		{
			//InitPen();
			//Invalidate();
		}

		protected override void OnRenderSizeChanged(System.Windows.SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);
			_bitmapDraw.RenderSize = new System.Drawing.Size((int) sizeInfo.NewSize.Width, (int) sizeInfo.NewSize.Height);
			InvalidateVisual();
		}
		#endregion

		protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);
			this.DrawCommands(drawingContext);
		}

		private void DrawCommands(System.Windows.Media.DrawingContext context)
		{
			if (_bitmapDraw.RenderSize.Height == 0 || _bitmapDraw.RenderSize.Width == 0)
				return;

			_bitmapDraw.SizeX = Global.Instance.Machine.SizeX;
			_bitmapDraw.SizeY = Global.Instance.Machine.SizeY;

			var curBitmap = _bitmapDraw.DrawToBitmap();
			MemoryStream stream = new MemoryStream();
			curBitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
			var cc = new System.Windows.Media.ImageSourceConverter().ConvertFrom(stream);
			context.DrawImage((System.Windows.Media.ImageSource)cc, new System.Windows.Rect(0,0, this.ActualWidth, this.ActualHeight));
			curBitmap.Dispose();
		}
	}
}
