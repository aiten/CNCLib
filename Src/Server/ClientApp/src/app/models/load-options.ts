/*
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

export enum ELoadType {
  GCode = 0,
  HPGL = 1,
  Image = 2,
  ImageHole = 3
}

export enum PenType {
  ZMove = 0,
  CommandString = 1
}

export enum SmoothTypeEnum {
  NoSmooth = 0,
  SplitLine = 1,
  SplineLine = 2
}

export enum ConvertTypeEnum {
  NoConvert = 0,
  InvertLineSequence = 1
}

export enum DitherFilter {
  FloydSteinbergDither = 0,
  NewspaperDither = 1
}

export enum EHoleType {
  Square = 0,
  Circle = 1,
  Hexagon = 2,
  Diamond = 3,
  Heart = 4
}

export class LoadOptions {
  Id: number;

  LoadType: ELoadType;

  FileName: string;
  FileContent: string;

  SettingName: string;

  GCodeWriteToFileName: string;

  //ALL (not GCode)
  StartupCommands: string;
  ShutdownCommands: string;

  // G-CODE

  SubstG82: boolean;
  AddLineNumbers: boolean;

  //HPGL
  SwapXY: boolean;
  ScaleX: number;
  ScaleY: number;
  OfsX: number;
  OfsY: number;

  //HPGL+IMG
  AutoScale: boolean;
  AutoScaleKeepRatio: boolean;

  AutoScaleCenter: boolean;

  AutoScaleSizeX: number;
  AutoScaleSizeY: number;

  AutoScaleBorderDistX: number;
  AutoScaleBorderDistY: number;

  PenMoveType: PenType;

  EngravePosInParameter: boolean;
  EngravePosUp: number;
  EngravePosDown: number;

  MoveSpeed: number | null;
  EngraveDownSpeed: number | null;

  LaserFirstOnCommand: string;
  LaserOnCommand: string;
  LaserOffCommand: string;
  LaserLastOffCommand: string;

  LaserSize: number;
  LaserAccDist: number;


  SmoothType: SmoothTypeEnum;

  ConvertType: ConvertTypeEnum;

  SmoothMinAngle: number | null;
  SmoothMinLineLength: number | null;
  SmoothMaxError: number | null;

  //IMG

  ImageWriteToFileName: string;

  GrayThreshold: number;

  ImageDPIX: number | null;
  ImageDPIY: number | null;

  ImageInvert: boolean;

  Dither: DitherFilter;
  NewspaperDitherSize: number;

  // Hole

  DotDistX: number;
  DotDistY: number;
  DotSizeX: number;
  DotSizeY: number;
  UseYShift: boolean;
  RotateHeart: boolean;

  HoleType: EHoleType;

  isHPGL(): boolean {
    return this.LoadType == ELoadType.HPGL;
  }

  isHPGLorImageOrImageHole() {
    return this.isHPGL() || this.isImageOrImageHole();
  }

  isImage() {
    return this.LoadType == ELoadType.Image;
  }

  isImageHole() {
    return this.LoadType == ELoadType.ImageHole;
  }

  isImageOrImageHole() {
    return this.LoadType == ELoadType.Image || this.LoadType == ELoadType.ImageHole;
  }

  isAutoScale() {
    return this.isHPGLorImageOrImageHole() && this.AutoScale;
  }

  isScale() {
    return this.isHPGLorImageOrImageHole() && this.AutoScale == false;
  }

  isSmooth() {
    return this.isHPGL() && this.SmoothType != SmoothTypeEnum.NoSmooth;
  }

  isLaser() {
    return (this.isHPGL() && this.PenMoveType == PenType.CommandString) || this.isImageOrImageHole();
  }

  isEngrave() {
    return this.isHPGL() && this.PenMoveType == PenType.ZMove;
  }
}
