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

import { Injectable } from '@angular/core';

import { Machine } from '../models/machine';


@Injectable()
export class MachineControlGlobal {

  lastCommands: string[] = [];

  moveDist: number = 0.5;

  workOfsX: number[] = [null, null, null, null, null, null];
  workOfsY: number[] = [null, null, null, null, null, null];
  workOfsZ: number[] = [null, null, null, null, null, null];

  offsetG92: number[] = [0, 0, 0, 0, 0, 0];

  position: number[] = [null, null, null, null, null, null];
  positionAbs: number[] = [null, null, null, null, null, null];

  sdFileName: string = 'auto0.g';

  showSpindle: boolean = true;
  showLaser: boolean = true;
  showCoolant: boolean = false;
  showRotate: boolean = false;
  showRefMove: boolean = false;
  showCustom: boolean = false;
  showSd: boolean = false;
  showMove: boolean = true;
  showInfo: boolean = true;
  showCommand: boolean = true;
  showPosition: boolean = true;
  showOffset: boolean = true;
  showWorkOffset: boolean = false;

  setFromMachine(machine: Machine) {
  }
}
