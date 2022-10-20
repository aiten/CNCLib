﻿/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) Herbert Aitenbichler

  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
  to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
  and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
*/

namespace CNCLib.Logic.Abstraction.DTO;

using System;

public class LoadOptions
{
    public int Id { get; set; }

    public enum ELoadType
    {
        GCode     = 0,
        Hpgl      = 1,
        Image     = 2,
        ImageHole = 3
    }

    public ELoadType LoadType { get; set; } = ELoadType.GCode;

    public string FileName    { get; set; }
    public byte[] FileContent { get; set; }

    public string SettingName { get; set; } = @"new";

    public string GCodeWriteToFileName { get; set; } = @"%USERPROFILE%\Documents\test.gcode";

    //ALL (not GCode)
    public string StartupCommands  { get; set; }
    public string ShutdownCommands { get; set; } = @"M5";

    // G-CODE

    public bool SubstG82       { get; set; } = false;
    public bool AddLineNumbers { get; set; } = false;

    //Hpgl
    public bool    SwapXY { get; set; } = false;
    public decimal ScaleX { get; set; } = 1;
    public decimal ScaleY { get; set; } = 1;
    public decimal OfsX   { get; set; } = 0;
    public decimal OfsY   { get; set; } = 0;

    //Hpgl+IMG
    public bool AutoScale          { get; set; } = false;
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

    public PenType PenMoveType { get; set; } = PenType.ZMove;

    public bool    EngravePosInParameter { get; set; } = true;
    public decimal EngravePosUp          { get; set; } = 1m;
    public decimal EngravePosDown        { get; set; } = -0.5m;

    public decimal? MoveSpeed        { get; set; } = 500m;
    public decimal? EngraveDownSpeed { get; set; }
    public decimal  CutterSize       { get; set; } = 1.5m;

    public string LaserFirstOnCommand { get; set; } = "M106 S255";
    public string LaserOnCommand      { get; set; } = "M106";
    public string LaserOffCommand     { get; set; } = "M107";
    public string LaserLastOffCommand { get; set; } = "M107";

    public decimal LaserSize    { get; set; } = 0.333m;
    public decimal LaserAccDist { get; set; } = 1m;

    public enum SmoothTypeEnum
    {
        NoSmooth  = 0,
        SplitLine = 1,
    }

    public SmoothTypeEnum SmoothType { get; set; } = SmoothTypeEnum.NoSmooth;

    public enum ConvertTypeEnum
    {
        NoConvert          = 0,
        InvertLineSequence = 1
    }

    public ConvertTypeEnum ConvertType { get; set; } = ConvertTypeEnum.NoConvert;

    public decimal? SmoothMinAngle      { get; set; } = (decimal)(45 * (Math.PI / 180));
    public decimal? SmoothMinLineLength { get; set; } = 1m;
    public decimal? SmoothMaxError      { get; set; } = 1m / 40m;

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

    public bool ImageInvert { get; set; } = false;

    public DitherFilter Dither              { get; set; } = DitherFilter.FloydSteinbergDither;
    public int          NewspaperDitherSize { get; set; } = 5;

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

    public EHoleType HoleType { get; set; } = EHoleType.Hexagon;
}