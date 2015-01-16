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

using Plotter.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plotter.GUI
{
    public partial class MainForm : Form
    {
        private Communication Com
        {
            get { return Framework.Tools.Singleton<Communication>.Instance; }
        }

        public MainForm()
        {
            InitializeComponent();
            _com.SelectedItem = "COM3";
            UpdateButtons();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Framework.Tools.Singleton<Communication>.Free();
        }

        private void UpdateButtons()
        {
            if (Framework.Tools.Singleton<Communication>.Allocated && Com.IsConnected)
            {
                _paint.Enabled = true;
            }
            else
            {
 //               _paint.Enabled = false;
            }
        }

        private void _connect_Click(object sender, EventArgs e)
        {
            if (Com.IsConnected)
            {
                Framework.Tools.Singleton<Communication>.Free();
            }
            else 
            {
                Com.Connect(_com.SelectedItem.ToString());
            }
            UpdateButtons();
        }

        private void _paint_Click(object sender, EventArgs e)
        {
            using (PaintForm form = new PaintForm())
            {
                form.ShowDialog();
            }
        }
    }
}
