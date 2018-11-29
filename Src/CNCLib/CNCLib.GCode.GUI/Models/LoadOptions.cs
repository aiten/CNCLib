////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2018 Herbert Aitenbichler

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

using Framework.Wpf.ViewModels;

namespace CNCLib.GCode.GUI.Models
{
    public class LoadOptions : BindableBase
    {
        public int Id { get; set; }

        public enum ELoadType
        {
            GCode     = 0,
            HPGL      = 1,
            Image     = 2,
            ImageHole = 3
        }

        private ELoadType _loadType = ELoadType.GCode;

        public ELoadType LoadType
        {
            get => _loadType;
            set => SetProperty(ref _loadType, value);
        }

        private string _fileName;

        public string FileName
        {
            get => _fileName;
            set => SetProperty(ref _fileName, value);
        }

        public byte[] FileContent { get; set; }

        public string SettingName { get; set; }

        public string GCodeWriteToFileName { get; set; } = @"%USERPROFILE%\Documents\test.gcode";

        //ALL (not GCode)
        public string StartupCommands  { get; set; }
        public string ShutdownCommands { get; set; } = @"M5";

        // G-CODE

        public bool SubstG82       { get; set; } = false;
        public bool AddLineNumbers { get; set; } = false;

        //HPGL
        public bool    SwapXY { get; set; } = false;
        public decimal ScaleX { get; set; } = 1;
        public decimal ScaleY { get; set; } = 1;
        public decimal OfsX   { get; set; } = 0;
        public decimal OfsY   { get; set; } = 0;

        //HPGL+IMG
        private bool _autoScale;

        public bool AutoScale
        {
            get => _autoScale;
            set => SetProperty(ref _autoScale, value);
        }

        public bool AutoScaleKeepRatio { get; set; } = true;

        public bool AutoScaleCenter { get; set; } = false;

        public decimal AutoScaleSizeX { get; set; } = 0;
        public decimal AutoScaleSizeY { get; set; } = 0;

        public decimal AutoScaleBorderDistX { get; set; } = 0.5m;
        public decimal AutoScaleBorderDistY { get; set; } = 0.5m;

        public enum PenType
        {
            ZMove         = 0,
            CommandString = 1
        }

        private PenType _penMoveType = PenType.ZMove;

        public PenType PenMoveType
        {
            get => _penMoveType;
            set => SetProperty(ref _penMoveType, value);
        }

        public bool    EngravePosInParameter { get; set; } = true;
        public decimal EngravePosUp          { get; set; } = 1m;
        public decimal EngravePosDown        { get; set; } = -0.5m;

        public decimal? MoveSpeed        { get; set; } = 500m;
        public decimal? EngraveDownSpeed { get; set; }

        public string LaserFirstOnCommand { get; set; } = "M106 S255";
        public string LaserOnCommand      { get; set; } = "M106";
        public string LaserOffCommand     { get; set; } = "M107";
        public string LaserLastOffCommand { get; set; } = "M107";

        public decimal LaserSize    { get; set; } = 0.333m;
        public decimal LaserAccDist { get; set; } = 1m;

        public enum SmoothTypeEnum
        {
            NoSmooth   = 0,
            SplitLine  = 1,
            SplineLine = 2
        }

        private SmoothTypeEnum _smoothType = SmoothTypeEnum.NoSmooth;

        public SmoothTypeEnum SmoothType
        {
            get => _smoothType;
            set
            {
                SetProperty(ref _smoothType, value);
                RaisePropertyChanged(nameof(SmoothEnabled));
            }
        }

        public decimal? SmoothMinAngle      { get; set; } = (decimal)(45 * (Math.PI / 180));
        public decimal? SmoothMinLineLength { get; set; } = 1m;
        public decimal? SmoothMaxError      { get; set; } = 1m / 40m;

        public enum ConvertTypeEnum
        {
            NoConvert          = 0,
            InvertLineSequence = 1
        }

        public ConvertTypeEnum ConvertType { get; set; } = ConvertTypeEnum.NoConvert;

        // calculated
        public bool SmoothEnabled
        {
            get => SmoothType != SmoothTypeEnum.NoSmooth;
            set => SmoothType = value?SmoothTypeEnum.SplitLine:SmoothTypeEnum.NoSmooth;
        }

        public bool ConvertEnabled
        {
            get => ConvertType != ConvertTypeEnum.NoConvert;
            set => ConvertType = value?ConvertTypeEnum.InvertLineSequence:ConvertTypeEnum.NoConvert;
        }

        //IMG
        public string ImageWriteToFileName { get; set; } = @"%USERPROFILE%\Documents\image.bmp";

        public byte GrayThreshold { get; set; } = 127;

        public decimal? ImageDPIX { get; set; }
        public decimal? ImageDPIY { get; set; }

        public enum DitherFilter
        {
            FloydSteinbergDither = 0,
            NewspaperDither      = 1
        }

        private DitherFilter _dither = DitherFilter.FloydSteinbergDither;

        public DitherFilter Dither
        {
            get => _dither;
            set => SetProperty(ref _dither, value);
        }

        public bool ImageInvert { get; set; } = false;

        public int NewspaperDitherSize { get; set; } = 5;

        // Hole

        public decimal DotDistX    { get; set; } = 0.333m;
        public decimal DotDistY    { get; set; } = 0.333m;
        public int     DotSizeX    { get; set; } = 1;
        public int     DotSizeY    { get; set; } = 1;
        public bool    UseYShift   { get; set; } = true;
        public bool    RotateHeart { get; set; } = false;

        public enum EHoleType
        {
            Square  = 0,
            Circle  = 1,
            Hexagon = 2,
            Diamond = 3,
            Heart   = 4
        }

        private EHoleType _holeType = EHoleType.Hexagon;

        public EHoleType HoleType
        {
            get => _holeType;
            set => SetProperty(ref _holeType, value);
        }
    }
}