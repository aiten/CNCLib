////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2015 Herbert Aitenbichler

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
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNCLib.GUI
{
	public class ColorComboBox : ComboBox
	{
		public ColorComboBox()
		{
			DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			DropDownHeight = 400;
			DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			DropDownWidth = 200;
			MaxDropDownItems = 20;
			Size = new System.Drawing.Size(134, 24);
			DrawItem += new System.Windows.Forms.DrawItemEventHandler(MyDrawItem);

			Initialize();
		}

		public Color Color
		{
			get
			{
				if (SelectedItem != null && SelectedItem.ToString() != "")
					return Color.FromName(SelectedItem.ToString());

				return Color.Black;
			}
			set
			{
				SelectedIndex = Items.IndexOf(value.Name.ToString());
			}
		}

		protected void Initialize()
		{
			Type colortype = typeof(System.Drawing.Color);
			PropertyInfo[] propertyInfo = colortype.GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public);
			foreach (PropertyInfo color in propertyInfo)
			{
				Items.Add(color.Name);
			}
			SelectedIndex = 8;
		}

		private void MyDrawItem(object sender, DrawItemEventArgs e)
		{
			e.DrawBackground();

			Rectangle rect = e.Bounds;
			if (e.Index >= 0)
			{
				string colorname = ((ComboBox)sender).Items[e.Index].ToString();
				Color color = Color.FromName(colorname);
				Font  font = new Font("Arial", 9, FontStyle.Regular);
				Brush brush = new SolidBrush(color);

				e.Graphics.DrawString(colorname, font, Brushes.Black, rect.X, rect.Top);
				e.Graphics.FillRectangle(brush, rect.X + 110, rect.Y + 5, rect.Width - 10, rect.Height - 10);
			}
		}
	}
}
