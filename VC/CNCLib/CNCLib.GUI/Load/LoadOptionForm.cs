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

using CNCLib.GCode.Load;
using System;
using System.Windows.Forms;
using CNCLib.Logic.Contracts;
using Framework.Tools.Dependency;

namespace CNCLib.GUI.Load
{
	public partial class LoadOptionForm : Form
    {
        protected class LoadOptionDefinition
        {
            public LoadOptions  Item { get; set; }
            public override string ToString()
            {
                return Item.SettingName;
            }
        }

        public LoadOptionForm()
        {
            InitializeComponent();

            ReadSettings();
        }

        private void ReadSettings()
        {
            _settingName.Items.Clear();

            using (var controller = Dependency.Resolve<ILoadOptionsController>())
            {
                var items = controller.GetAll();
                foreach (var s in items)
                {
                    _settingName.Items.Add(new LoadOptionDefinition() { Item = s });
                }
            }
        }

        static public Framework.Tools.Pattern.IFactory LogicFactory { get; set; }

        public LoadOptions LoadInfo 
        {   
            get
			{
                var r = new LoadOptions()
                {
					FileName = _filename.Text,
                    SettingName = _settingName.Text,
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
                    PenMoveType = _generateForEngrave.Checked ? LoadOptions.PenType.ZMove : LoadOptions.PenType.CommandString,

                    EngravePosUp = decimal.Parse(_engraveZUp.Text),
                    EngravePosDown = decimal.Parse(_engraveZDown.Text),
                    EngravePosInParameter = _engraveUseParameter.Checked,

                    LaserFirstOnCommand = _laserFirstOn.Text,
                    LaserOnCommand = _laserOn.Text,
                    LaserOffCommand = _laserOff.Text,

                    LaserSize = decimal.Parse(_laserSize.Text),
                    GrayThreshold = Byte.Parse(_grayThreshold.Text),

                    Dither = _floydSteinbergDither.Checked ? LoadOptions.DitherFilter.FloydSteinbergDither : LoadOptions.DitherFilter.NewspaperDither,

                    NewspaperDitherSize = int.Parse(_newspaperDotSize.Text),

                    ImageInvert = _imageInvert.Checked,

                    ImageWriteToFileName = _saveImageToFilename.Text,
                    GCodeWriteToFileName = _saveGCodeToFileName.Text,

                    DotSizeX = int.Parse(_holeDotSizeX.Text),
                    DotSizeY = int.Parse(_holeDotSizeY.Text),

                    DotDistX = decimal.Parse(_holeDotDistX.Text),
                    DotDistY = decimal.Parse(_holeDotDistY.Text),

                    UseYShift = _holeYShift.Checked,
                    RotateHeart = _holeRotateHeart.Checked
                };

				if (_loadGCode.Checked) r.LoadType = LoadOptions.ELoadType.GCode;
				else if (_loadHTML.Checked) r.LoadType = LoadOptions.ELoadType.HPGL;
				else if (_loadImage.Checked) r.LoadType = LoadOptions.ELoadType.Image;
				else if (_loadimageHole.Checked) r.LoadType = LoadOptions.ELoadType.ImageHole;

				if (string.IsNullOrEmpty(_penMoveSpeed.Text))
                    r.MoveSpeed = null;
                else
                    r.MoveSpeed = decimal.Parse(_penMoveSpeed.Text);

                if (string.IsNullOrEmpty(_penDownSpeed.Text))
                    r.EngraveDownSpeed = null;
                else
                    r.EngraveDownSpeed = decimal.Parse(_penDownSpeed.Text);

                if (string.IsNullOrEmpty(_imageDPIX.Text))
                    r.ImageDPIX = null;
                else
                    r.ImageDPIX = decimal.Parse(_imageDPIX.Text);

                if (string.IsNullOrEmpty(_imageDPIY.Text))
                    r.ImageDPIY = null;
                else
                    r.ImageDPIY = decimal.Parse(_imageDPIY.Text);

                if (_holeSquare.Checked)        r.HoleType = LoadOptions.EHoleType.Square;
                else if (_holeCircle.Checked)   r.HoleType = LoadOptions.EHoleType.Circle;
                else if (_holeHexagon.Checked)  r.HoleType = LoadOptions.EHoleType.Hexagon;
                else if (_holeDiamond.Checked)  r.HoleType = LoadOptions.EHoleType.Diamond;
                else if (_holeHeart.Checked)    r.HoleType = LoadOptions.EHoleType.Heart;

                return r;

            }
            set
            {
				switch (value.LoadType)
				{
					case LoadOptions.ELoadType.GCode: _loadGCode.Checked = true; break;
					case LoadOptions.ELoadType.HPGL: _loadHTML.Checked = true; break;
					case LoadOptions.ELoadType.Image: _loadImage.Checked = true; break;
					case LoadOptions.ELoadType.ImageHole: _loadimageHole.Checked = true; break;
				}

				_filename.Text = value.FileName;
                _settingName.Text = value.SettingName;
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

				_generateForEngrave.Checked = value.PenMoveType == LoadOptions.PenType.ZMove;
				_generateForLaser.Checked = value.PenMoveType == LoadOptions.PenType.CommandString;

				_engraveZUp.Text = value.EngravePosUp.ToString();
				_engraveZDown.Text = value.EngravePosDown.ToString();
				_engraveUseParameter.Checked = value.EngravePosInParameter;

                _penMoveSpeed.Text = value.MoveSpeed.ToString();
                _penDownSpeed.Text = value.EngraveDownSpeed.ToString();

				_laserFirstOn.Text = value.LaserFirstOnCommand;
				_laserOn.Text = value.LaserOnCommand;
				_laserOff.Text = value.LaserOffCommand;

                _laserSize.Text = value.LaserSize.ToString();
                _grayThreshold.Text = value.GrayThreshold.ToString();

                _imageDPIX.Text = value.ImageDPIX.ToString();
                _imageDPIY.Text = value.ImageDPIY.ToString();

                _saveImageToFilename.Text = value.ImageWriteToFileName;
                _saveGCodeToFileName.Text = value.GCodeWriteToFileName;

                _floydSteinbergDither.Checked = value.Dither == LoadOptions.DitherFilter.FloydSteinbergDither;
                _newspaperDither.Checked = value.Dither == LoadOptions.DitherFilter.NewspaperDither;

                _imageInvert.Checked = value.ImageInvert;

                _newspaperDotSize.Text = value.NewspaperDitherSize.ToString();


                switch (value.HoleType)
                {
                    case LoadOptions.EHoleType.Square:  _holeSquare.Checked = true; break;
                    case LoadOptions.EHoleType.Circle:  _holeCircle.Checked = true; break;
                    case LoadOptions.EHoleType.Hexagon: _holeHexagon.Checked = true; break;
                    case LoadOptions.EHoleType.Diamond: _holeDiamond.Checked = true; break;
                    case LoadOptions.EHoleType.Heart:   _holeHeart.Checked = true; break;
                }

                _holeDotSizeX.Text = value.DotSizeX.ToString();
                _holeDotSizeY.Text = value.DotSizeY.ToString();

                _holeDotDistX.Text = value.DotDistX.ToString();
                _holeDotDistY.Text = value.DotDistY.ToString();

                _holeYShift.Checked = value.UseYShift;
                _holeRotateHeart.Checked = value.RotateHeart;

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

        private void _dpiXeqY_Click(object sender, EventArgs e)
        {
            _imageDPIY.Text = _imageDPIX.Text;
        }

        private void _settingName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_settingName.Focused && _settingName.SelectedItem != null)
            {
                LoadOptionDefinition item = (LoadOptionDefinition)_settingName.SelectedItem;
				LoadInfo = item.Item;
            }
        }
        private void _saveSettings_Click(object sender, EventArgs e)
        {
            LoadOptions obj;
            try
            {
                obj = this.LoadInfo;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Get values failed: " + ex.Message);
                return;
            }

            if (!string.IsNullOrEmpty(obj.SettingName))
            {
                try
                {
                    using (var controller = Dependency.Resolve<ILoadOptionsController>())
                    {
                        if (_settingName.SelectedItem != null)
                        {
                            LoadOptionDefinition item = (LoadOptionDefinition)_settingName.SelectedItem;
							obj.Id = item.Item.Id;
							controller.Update(obj);
                        }
                        else
                        {
                            int id = controller.Add(obj);
                            ReadSettings();
                            int idx = 0;
                            foreach (LoadOptionDefinition o in _settingName.Items)
                            {
                                if (o.Item.Id == id)
                                {
                                    _settingName.SelectedIndex = idx;
                                    break;
                                }
                                idx++;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Save Options failed: " + ex.Message);
                }
            }
       }

        private void _deleteSettings_Click(object sender, EventArgs e)
        {
            LoadOptions obj;
            try
            {
                obj = this.LoadInfo;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Get values failed: " + ex.Message);
                return;
            }

            if (!string.IsNullOrEmpty(obj.SettingName))
            {
                try
                {
                    using (var controller = Dependency.Resolve<ILoadOptionsController>())
                    {
                        if (_settingName.SelectedItem != null)
                        {
                            LoadOptionDefinition item = (LoadOptionDefinition)_settingName.SelectedItem;
                            controller.Delete(item.Item);
                            ReadSettings();
                            _settingName.Text = "";
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Delete Options failed: " + ex.Message);
                }
            }
        }

        private void _holeDotSizeYEq_Click(object sender, EventArgs e)
        {
            _holeDotSizeY.Text = _holeDotSizeX.Text;
        }

        private void _holeDotDistYEq_Click(object sender, EventArgs e)
        {
            _holeDotDistY.Text = _holeDotDistX.Text;
        }
	}
}
