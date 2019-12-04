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
import { PreviewGCode } from '../../models/gcode-view-input';

@Component(
  {
    selector: 'ha-gcode-preview-input',
    templateUrl: './gcode-preview-input.component.html',
    styleUrls: ['./gcode-preview-input.component.css']
  })
export class GcodePreviewInputComponent implements OnInit {
  @Input()
  previewOpt: PreviewGCode;
  previewOptForm: FormGroup;

  constructor(
    private fb: FormBuilder
  ) {

    this.previewOptForm = fb.group(
      {
        SizeX: [0.0, [Validators.required]],
        SizeY: [0.0, [Validators.required]],
        SizeZ: [0.0, [Validators.required]],
        KeepRatio: [true, [Validators.required]],
        Zoom: [0.0, [Validators.required]],
        OffsetX: [0.0, [Validators.required]],
        OffsetY: [0.0, [Validators.required]],
        OffsetZ: [0.0, [Validators.required]],
        CutterSize: [0.0, [Validators.required]],
        LaserSize: [0.0, [Validators.required]],
        MachineColor: [0, [Validators.required]],
        LaserOnColor: [0, [Validators.required]],
        LaserOffColor: [0, [Validators.required]],
        CutColor: [0, [Validators.required]],
        CutDotColor: [0, [Validators.required]],
        CutEllipseColor: [0, [Validators.required]],
        CutArcColor: [0, [Validators.required]],
        FastMoveColor: [0, [Validators.required]],
        HelpLineColor: [0, [Validators.required]],

        Rotate3DAngle: [0.0, [Validators.required]],

        Rotate3DVectX: [0.0, [Validators.required]],
        Rotate3DVectY: [0.0, [Validators.required]],
        Rotate3DVectZ: [1.0, [Validators.required]],

        RenderSizeX: [0, [Validators.required]],
        RenderSizeY: [0, [Validators.required]],
      });

    this.previewOptForm.valueChanges.subscribe((
      value) => {
      Object.assign(this.previewOpt, value);
    });
  }

  rotate1() {
      this.previewOptForm.patchValue({
          Rotate3DAngle: -1.0407742907225024,
          Rotate3DVectX: 101,
          Rotate3DVectY: -13,
          Rotate3DVectZ: 1,
          OffsetX: -10,
          OffsetY: 10,
    });
  }

  async savegCode(value: any): Promise<void> {
  }

  async ngOnInit() {
    this.previewOptForm.patchValue(this.previewOpt);
  }
}
