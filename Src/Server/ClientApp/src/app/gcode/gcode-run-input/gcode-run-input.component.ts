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
import { LoadOptions, EHoleType, ELoadType, PenType, SmoothTypeEnum, ConvertTypeEnum, DitherFilter } from '../../models/load-options';
import { FormGroup, FormArray, FormControl, FormBuilder, Validators } from '@angular/forms';

@Component(
  {
    selector: 'ha-gcode-run-input',
    templateUrl: './gcode-run-input.component.html',
    styleUrls: ['./gcode-run-input.component.css']
  })
export class GcodeRunInputComponent implements OnInit {
  @Input()
  entry: LoadOptions;
  gCodeForm: FormGroup;

  constructor(
    private fb: FormBuilder
  ) {

    this.gCodeForm = fb.group(
      {
        FileName: ['', [Validators.required, Validators.maxLength(512)]],
        AutoScaleKeepRatio: [false, [Validators.required]],
        AutoScaleCenter: [false, [Validators.required]],
        AutoScaleSizeX: [0.0, [Validators.required]],
        AutoScaleSizeY: [0.0, [Validators.required]],
        AutoScaleBorderDistX: [0.0, [Validators.required]],
        AutoScaleBorderDistY: [0.0, [Validators.required]],
      });

    this.gCodeForm.valueChanges.subscribe((
      value) => {
      Object.assign(this.entry, value);
    });
  }

  isHPGL() {
    return this.entry.LoadType == ELoadType.HPGL;
  }

  isHPGLorImageOrImageHole() {
    return this.isHPGL() || this.isImageOrImageHole();
  }

  isImage() {
    return this.entry.LoadType == ELoadType.Image;
  }

  isImageHole() {
    return this.entry.LoadType == ELoadType.ImageHole;
  }

  isImageOrImageHole() {
    return this.entry.LoadType == ELoadType.Image || this.entry.LoadType == ELoadType.ImageHole;
  }

  isAutoScale() {
    return this.isHPGLorImageOrImageHole() && this.entry.AutoScale;
  }

  isScale() {
    return this.isHPGLorImageOrImageHole() && this.entry.AutoScale == false;
  }

  isSmooth() {
    return this.isHPGL() && this.entry.SmoothType != SmoothTypeEnum.NoSmooth;
  }

  isLaser() {
    return (this.isHPGL() && this.entry.PenMoveType == PenType.CommandString) || this.isImageOrImageHole();
  }

  isEngrave() {
    return this.isHPGL() && this.entry.PenMoveType == PenType.ZMove;
  }

  async savegCode(value: any): Promise<void> {
  }

  async ngOnInit() {
    this.gCodeForm.patchValue(this.entry);
  }
}
