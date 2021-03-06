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

export class PreviewGCode {

  sizeX: number = 200;

  sizeY: number = 200;

  sizeZ: number = 200;

  keepRatio: boolean = true;

  zoom: number = 1;

  offsetX: number = 0;

  offsetY: number = 0;

  offsetZ: number = 0;

  cutterSize: number = 0.254;

  laserSize: number = 0;

  machineColor: string;

  laserOnColor: string;

  laserOffColor: string;

  cutColor: string;

  cutDotColor: string;

  cutEllipseColor: string;

  cutArcColor: string;

  fastMoveColor: string;

  helpLineColor: string;

// public Rotate3D Rotate

  rotate3DAngle: number = 0;

  rotate3DVectX: number = 0;
  rotate3DVectY: number = 0;
  rotate3DVectZ: number = 1.0;

  rotate3DVect: number[] = [this.rotate3DVectX, this.rotate3DVectY, this.rotate3DVectZ];

  renderSizeX: number = 800;

  renderSizeY: number = 800;

  setVector(vect: number[]) {
    this.rotate3DVectX = vect[0];
    this.rotate3DVectY = vect[1];
    this.rotate3DVectZ = vect[2];
  }
}
