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
  Hpgl = 1,
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
  id: number;

  loadType: ELoadType;

  fileName: string;
  fileContent: string;

  settingName: string;

  gCodeWriteToFileName: string;

  //ALL (not GCode)
  startupCommands: string;
  shutdownCommands: string;

  // G-CODE

  substG82: boolean;
  addLineNumbers: boolean;

  //Hpgl
  swapXY: boolean;
  scaleX: number;
  scaleY: number;
  ofsX: number;
  ofsY: number;

  //Hpgl+IMG
  autoScale: boolean;
  autoScaleKeepRatio: boolean;

  autoScaleCenter: boolean;

  autoScaleSizeX: number;
  autoScaleSizeY: number;

  autoScaleBorderDistX: number;
  autoScaleBorderDistY: number;

  penMoveType: PenType;

  engravePosInParameter: boolean;
  engravePosUp: number;
  engravePosDown: number;

  moveSpeed: number | null;
  engraveDownSpeed: number | null;

  laserFirstOnCommand: string;
  laserOnCommand: string;
  laserOffCommand: string;
  laserLastOffCommand: string;

  laserSize: number;
  laserAccDist: number;


  smoothType: SmoothTypeEnum;

  convertType: ConvertTypeEnum;

  cutterSize: number | null;

  smoothMinAngle: number | null;
  smoothMinLineLength: number | null;
  smoothMaxError: number | null;

  //IMG

  imageWriteToFileName: string;

  grayThreshold: number;

  imageDPIX: number | null;
  imageDPIY: number | null;

  imageInvert: boolean;

  dither: DitherFilter;
  newspaperDitherSize: number;

  // Hole

  dotDistX: number;
  dotDistY: number;
  dotSizeX: number;
  dotSizeY: number;
  useYShift: boolean;
  rotateHeart: boolean;

  holeType: EHoleType;

  isHpgl(): boolean {
    return this.loadType == ELoadType.Hpgl;
  }

  isHpglorImageOrImageHole() {
    return this.isHpgl() || this.isImageOrImageHole();
  }

  isImage() {
    return this.loadType == ELoadType.Image;
  }

  isImageHole() {
    return this.loadType == ELoadType.ImageHole;
  }

  isImageOrImageHole() {
    return this.loadType == ELoadType.Image || this.loadType == ELoadType.ImageHole;
  }

  isAutoScale() {
    return this.isHpglorImageOrImageHole() && this.autoScale;
  }

  isScale() {
    return this.isHpglorImageOrImageHole() && this.autoScale == false;
  }

  isSmooth() {
    return this.isHpgl() && this.smoothType != SmoothTypeEnum.NoSmooth;
  }

  isLaser() {
    return (this.isHpgl() && this.penMoveType == PenType.CommandString) || this.isImageOrImageHole();
  }

  isEngrave() {
    return this.isHpgl() && this.penMoveType == PenType.ZMove;
  }
}
