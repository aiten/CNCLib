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

import { SAxis } from "./eeprom-axis";

export enum ESignature {
  SIGNATURE = 0x21436502,
  SIGNATUREPLOTTER = 0x21438702
}

export enum ECommandSyntax {
  GCodeBasic = 0,
  GCode = 1,
  Grbl = 2,
  Hpgl = 7 // max 3 bit
}

export enum EReverenceSequence {
  XAxis = 0,
  YAxis = 1,
  ZAxis = 2,
  AAxis = 3,
  BAxis = 4,
  CAxis = 5,
  UAxis = 6,
  VAxis = 7,
  WAxis = 8,
  No = 255
};

export class Eeprom {

  values: number[];

  signature: number;

  maxStepRate: number;
  acc: number;
  dec: number;
  jerkSpeed: number;
  refMoveStepRate: number;
  moveAwayFromReference: number;
  stepsPerMm1000: number;
  maxSpindleSpeed: number;
  spindleFadeTime: number;

  info1: number;
  info2: number;

  numAxis: number;
  useAxis: number;

  hasSpindle: boolean;
  hasAnalogSpindle: boolean;
  hasSpindleDirection: boolean;
  hasCoolant: boolean;
  hasProbe: boolean;
  hasSD: boolean;
  hasEeprom: boolean;
  canRotate: boolean;
  hasHold: boolean;
  hasResume: boolean;
  hasHoldResume: boolean;
  hasKill: boolean;
  isLaser: boolean;
  commandSyntax: ECommandSyntax;
  dtrIsReset: boolean;
  needEEpromFlush: boolean;
  workOffsetCount: number;

  axisX: SAxis;
  axisY: SAxis;
  axisZ: SAxis;
  axisA: SAxis;
  axisB: SAxis;
  axisC: SAxis;

  refSequence1: EReverenceSequence;
  refSequence2: EReverenceSequence;
  refSequence3: EReverenceSequence;
  refSequence4: EReverenceSequence;
  refSequence5: EReverenceSequence;
  refSequence6: EReverenceSequence;

  penDownFeedrate: number;
  penUpFeedrate: number;
  movePenDownFeedrate: number;
  movePenUpFeedrate: number;
  movePenChangeFeedrate: number;
  penDownPos: number;
  penUpPos: number;
  penChangePos_x: number;
  penChangePos_y: number;
  penChangePos_z: number;
  penChangePos_x_ofs: number;
  penChangePos_y_ofs: number;
  servoClampOpenPos: number;
  servoClampClosePos: number;
  servoClampOpenDelay: number;
  servoClampCloseDelay: number;
}
