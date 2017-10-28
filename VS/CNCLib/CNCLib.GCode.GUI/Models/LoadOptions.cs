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

using System;
using Framework.Wpf.ViewModels;

namespace CNCLib.GCode.GUI.Models
{
    public class LoadOptions : BindableBase
    {
        public int Id { get; set; }

        public enum ELoadType
        {
            GCode,
            HPGL,
            Image,
            ImageHole

        }
        private ELoadType _LoadType = ELoadType.GCode;
        public ELoadType LoadType { get => _LoadType; set { SetProperty(ref _LoadType, value); } }

        public string _FileName;
        public string FileName { get => _FileName; set { SetProperty(ref _FileName, value); } }


        public Byte[] FileContent { get; set; }

        public string SettingName { get; set; }

        public string GCodeWriteToFileName { get; set; } = @"%USERPROFILE%\Documents\test.gcode";

        //ALL (not GCode)
        public string StartupCommands { get; set; }
        public string ShutdownCommands { get; set; } = @"M5";

        // G-CODE

        public bool SubstG82 { get; set; } = false;
        public bool AddLineNumbers { get; set; } = false;

        //HPGL
        public bool SwapXY { get; set; } = false;
        public decimal ScaleX { get; set; } = 1;
        public decimal ScaleY { get; set; } = 1;
        public decimal OfsX { get; set; } = 0;
        public decimal OfsY { get; set; } = 0;

        //HPGL+IMG
        public bool _AutoScale;
        public bool AutoScale { get => _AutoScale; set { SetProperty(ref _AutoScale, value); } }

        public bool AutoScaleKeepRatio { get; set; } = true;

        public bool AutoScaleCenter { get; set; } = false;

        public decimal AutoScaleSizeX { get; set; } = 0;
        public decimal AutoScaleSizeY { get; set; } = 0;

        public decimal AutoScaleBorderDistX { get; set; } = 0.5m;
        public decimal AutoScaleBorderDistY { get; set; } = 0.5m;


        public enum PenType
        {
            ZMove,
            CommandString
        }

        public PenType _PenMoveType = PenType.ZMove;
        public PenType PenMoveType { get => _PenMoveType; set { SetProperty(ref _PenMoveType, value); } }

        public bool EngravePosInParameter { get; set; } = true;
        public decimal EngravePosUp { get; set; } = 1m;
        public decimal EngravePosDown { get; set; } = -0.5m;

        public decimal? MoveSpeed { get; set; } = 500m;
        public decimal? EngraveDownSpeed { get; set; }

        public string LaserFirstOnCommand { get; set; } = "M106 S255";
        public string LaserOnCommand { get; set; } = "M106";
        public string LaserOffCommand { get; set; } = "M107";
        public string LaserLastOffCommand { get; set; } = "M107";

        public decimal LaserSize { get; set; } = 0.333m;
        public decimal LaserAccDist { get; set; } = 1m;

        public enum SmoothTypeEnum
        {
            NoSmooth = 0,
            SplitLine = 1,
            SplineLine = 2
        }

        public SmoothTypeEnum _SmoothType = SmoothTypeEnum.NoSmooth;
        public SmoothTypeEnum SmoothType { get => _SmoothType; set { SetProperty(ref _SmoothType, value); RaisePropertyChanged(nameof(SmoothEnabled)); } }

        public decimal? SmoothMinAngle { get; set; } = (decimal) (45 * (Math.PI / 180));
        public decimal? SmoothMinLineLenght { get; set; } = 1m;
        public decimal? SmoothMaxError { get; set; } = 1m / 40m;

        public enum ConvertTypeEnum
        {
            NoConvert = 0,
            InvertLineSequence = 1,
        }
        public ConvertTypeEnum ConvertType { get; set; } = ConvertTypeEnum.NoConvert;

        // calculated
        public bool SmoothEnabled { get { return SmoothType != SmoothTypeEnum.NoSmooth; } set { SmoothType = value ? SmoothTypeEnum.SplitLine : SmoothTypeEnum.NoSmooth; } }
        public bool ConvertEnabled { get { return ConvertType != ConvertTypeEnum.NoConvert; } set { ConvertType = value ? ConvertTypeEnum.InvertLineSequence : ConvertTypeEnum.NoConvert; } }

        //IMG
        public string ImageWriteToFileName { get; set; } = @"%USERPROFILE%\Documents\image.bmp";

        public Byte GrayThreshold { get; set; } = 127;

        public decimal? ImageDPIX { get; set; }
        public decimal? ImageDPIY { get; set; }
        public enum DitherFilter
        {
            FloydSteinbergDither,
            NewspaperDither
        }

        public DitherFilter _Dither = DitherFilter.FloydSteinbergDither;
        public DitherFilter Dither { get => _Dither; set { SetProperty(ref _Dither, value); } }

        public bool ImageInvert { get; set; } = false;

        public int NewspaperDitherSize { get; set; } = 5;

        // Hole

        public decimal DotDistX { get; set; } = 0.333m;
        public decimal DotDistY { get; set; } = 0.333m;
        public int DotSizeX { get; set; } = 1;
        public int DotSizeY { get; set; } = 1;
        public bool UseYShift { get; set; } = true;
        public bool RotateHeart { get; set; } = false;

        public enum EHoleType
        {
            Square,
            Circle,
            Hexagon,
            Diamond,
            Heart
        }
        public EHoleType _HoleType = EHoleType.Hexagon;
        public EHoleType HoleType { get => _HoleType; set { SetProperty(ref _HoleType, value); } }
    }
}
