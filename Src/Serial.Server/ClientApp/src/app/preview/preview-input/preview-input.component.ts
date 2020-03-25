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

import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormArray, FormControl, FormBuilder, Validators } from '@angular/forms';
import { PreviewGCode } from '../../models/preview-input';

@Component(
  {
    selector: 'preview-input',
    templateUrl: './preview-input.component.html',
    styleUrls: ['./preview-input.component.css']
  })
export class PreviewInputComponent implements OnInit {
  @Input()
  previewOpt: PreviewGCode;
  public previewOptForm: FormGroup;

  public isMore: boolean = false;

  constructor(
    private fb: FormBuilder
  ) {

    this.previewOptForm = fb.group(
      {
        sizeX: [0.0, [Validators.required]],
        sizeY: [0.0, [Validators.required]],
        sizeZ: [0.0, [Validators.required]],
        keepRatio: [true, [Validators.required]],
        zoom: [0.0, [Validators.required]],
        offsetX: [0.0, [Validators.required]],
        offsetY: [0.0, [Validators.required]],
        offsetZ: [0.0, [Validators.required]],
        cutterSize: [0.0, [Validators.required]],
        laserSize: [0.0, [Validators.required]],
        machineColor: [""],
        laserOnColor: [""],
        laserOffColor: [""],
        cutColor: [""],
        cutDotColor: [""],
        cutEllipseColor: [""],
        cutArcColor: [""],
        fastMoveColor: [""],
        helpLineColor: [""],

        rotate3DAngle: [0.0, [Validators.required]],

        rotate3DVectX: [0.0, [Validators.required]],
        rotate3DVectY: [0.0, [Validators.required]],
        rotate3DVectZ: [1.0, [Validators.required]],

        renderSizeX: [0, [Validators.required]],
        renderSizeY: [0, [Validators.required]],
      });

    this.previewOptForm.valueChanges.subscribe((
      value) => {
      Object.assign(this.previewOpt, value);
    });
  }

  async savegCode(value: any): Promise<void> {
  }

  async ngOnInit() {
    console.log("nginit Input");
    this.previewOptForm.patchValue(this.previewOpt);
  }
}
