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

using Framework.Tools;
using CNCLib.GCode.Load;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNCLib.GUI.Load
{
    public partial class LoadOptionForm : Form
    {
        public LoadOptionForm()
        {
            InitializeComponent();
        }

        public LoadInfo LoadInfo 
        {   
            get
			{
				return new LoadInfo()
				{
					FileName = _filename.Text,
					OfsX = decimal.Parse(_ofsX.Text),
					OfsY = decimal.Parse(_ofsY.Text),
					ScaleX = decimal.Parse(_scaleX.Text),
					ScaleY = decimal.Parse(_scaleY.Text),
					SwapXY = _swapXY.Checked,
					AutoScale = _autoScale.Checked,
					AutoScaleBorderDistX = decimal.Parse(_AutoScaleBorderDistX.Text),
					AutoScaleBorderDistY = decimal.Parse(_AutoScaleBorderDistY.Text),
					AutoScaleSizeX = decimal.Parse(_AutoScaleSizeX.Text),
					AutoScaleSizeY = decimal.Parse(_AutoScaleSizeY.Text),
					AutoScaleKeepRatio = _AutoScaleKeepRatio.Checked,
					PenMoveType = _generateForEngrave.Checked ? LoadInfo.PenType.ZMove : LoadInfo.PenType.CommandString,

					PenPosUp = decimal.Parse(_engraveZUp.Text),
					PenPosDown = decimal.Parse(_engraveZDown.Text),
					PenPosInParameter = _engraveUseParameter.Checked,

					PenDownCommandString = _laserOn.Text,
					PenUpCommandString = _laserOff.Text,

                    LaserSize = decimal.Parse(_laserSize.Text),
                };
			}
			set
            {
                _filename.Text = value.FileName;
                _ofsX.Text = value.OfsX.ToString();
                _ofsY.Text = value.OfsY.ToString();
                _scaleX.Text = value.ScaleX.ToString();
                _scaleY.Text = value.ScaleY.ToString();
                _swapXY.Checked = value.SwapXY;
				_autoScale.Checked = value.AutoScale;
				_AutoScaleBorderDistX.Text = value.AutoScaleBorderDistX.ToString();
				_AutoScaleBorderDistY.Text = value.AutoScaleBorderDistY.ToString();
				_AutoScaleSizeX.Text = value.AutoScaleSizeX.ToString();
				_AutoScaleSizeY.Text = value.AutoScaleSizeY.ToString();
				_AutoScaleKeepRatio.Checked = value.AutoScaleKeepRatio;

				_generateForEngrave.Checked = value.PenMoveType == LoadInfo.PenType.ZMove;
				_generateForLaser.Checked = value.PenMoveType == LoadInfo.PenType.CommandString;

				_engraveZUp.Text = value.PenPosUp.ToString();
				_engraveZDown.Text = value.PenPosDown.ToString();
				_engraveUseParameter.Checked = value.PenPosInParameter;

				_laserOn.Text = value.PenDownCommandString;
				_laserOff.Text = value.PenUpCommandString;

                _laserSize.Text = value.LaserSize.ToString();

                SetEnableState();
			}
        }

		void SetEnableState()
		{
			if (_autoScale.Checked)
			{
                _ofsX.Enabled = false;
				_ofsY.Enabled = false;
				_scaleX.Enabled = false;
				_scaleY.Enabled = false;

				_AutoScaleBorderDistX.Enabled = true;
				_AutoScaleBorderDistY.Enabled = true;
				_AutoScaleSizeX.Enabled = true;
				_AutoScaleSizeY.Enabled = true;
				_AutoScaleKeepRatio.Enabled = true;
			}
			else
			{
                _ofsX.Enabled = true;
				_ofsY.Enabled = true;
				_scaleX.Enabled = true;
				_scaleY.Enabled = true;

				_AutoScaleBorderDistX.Enabled = false;
				_AutoScaleBorderDistY.Enabled = false;
				_AutoScaleSizeX.Enabled = false;
				_AutoScaleSizeY.Enabled = false;
				_AutoScaleKeepRatio.Enabled = false;
			}
		}

        private void _fileopen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog form = new OpenFileDialog())
            {
                form.FileName = _filename.Text;
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _filename.Text = form.FileName;
                }
            }
        }

		private void _autoScale_CheckedChanged(object sender, EventArgs e)
		{
			SetEnableState();
		}
    }
}
