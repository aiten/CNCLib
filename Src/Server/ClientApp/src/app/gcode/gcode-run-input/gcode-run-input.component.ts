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
import { LoadOptions, ELoadType, PenType, ConvertTypeEnum, SmoothTypeEnum } from '../../models/load-options';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

import { UserFile } from "../../models/userFile";
import { CNCLibUserFileService } from '../../services/CNCLib-userFile.service';

@Component(
  {
    selector: 'gcode-run-input',
    templateUrl: './gcode-run-input.component.html',
    styleUrls: ['./gcode-run-input.component.css']
  })
export class GcodeRunInputComponent implements OnInit {
  @Input()
  entry: LoadOptions;
  gCodeForm: FormGroup;

  constructor(
    public userFileService: CNCLibUserFileService,
    private fb: FormBuilder
  ) {

    this.gCodeForm = fb.group(
      {
        fileName: ['', [Validators.required, Validators.maxLength(512)]],
        autoScaleKeepRatio: [false, [Validators.required]],
        autoScaleCenter: [false, [Validators.required]],
        autoScaleSizeX: [0.0, [Validators.required]],
        autoScaleSizeY: [0.0, [Validators.required]],
        autoScaleBorderDistX: [0.0, [Validators.required]],
        autoScaleBorderDistY: [0.0, [Validators.required]],
        moveSpeed: [450],
        cutterSize: [1.5],
        engravePosDown: [-0.5],
      });

    this.gCodeForm.valueChanges.subscribe((
      value) => {
      Object.assign(this.entry, value);
    });
  }

  async uploadFile(event) {
    let files = event.target.files;
    if (files.length > 0) {
      let tmpFileName = "$$$";
      const file = event.target.files[0];
      let userFile = new UserFile();
      userFile.image = file;
      userFile.fileName = tmpFileName;
      await this.userFileService.update(tmpFileName, userFile);
      this.gCodeForm.get('fileName').setValue(this.entry.fileName = 'db:' + tmpFileName);
    }
  }

  isDBFile(): boolean {
    return this.entry.fileName.startsWith("db:");
  }

  isHpgl() {
    return this.entry.loadType == ELoadType.Hpgl;
  }

  isHpglOrImageOrImageHole() {
    return this.isHpgl() || this.isImageOrImageHole();
  }

  isImage() {
    return this.entry.loadType == ELoadType.Image;
  }

  isImageHole() {
    return this.entry.loadType == ELoadType.ImageHole;
  }

  isImageOrImageHole() {
    return this.entry.loadType == ELoadType.Image || this.entry.loadType == ELoadType.ImageHole;
  }

  isAutoScale() {
    return this.isHpglOrImageOrImageHole() && this.entry.autoScale;
  }

  isScale() {
    return this.isHpglOrImageOrImageHole() && this.entry.autoScale == false;
  }

  isSmooth() {
    return this.isHpgl() && this.entry.smoothType != SmoothTypeEnum.NoSmooth;
  }

  isLaser() {
    return (this.isHpgl() && this.entry.penMoveType == PenType.CommandString) || this.isImageOrImageHole();
  }

  isInvertLineOrder() {
    return this.isHpgl() && this.entry.convertType == ConvertTypeEnum.InvertLineSequence;
  }

  isEngrave() {
    return this.isHpgl() && this.entry.penMoveType == PenType.ZMove;
  }

  async savegCode(value: any): Promise<void> {
  }

  async ngOnInit() {

    this.gCodeForm.patchValue(this.entry);
  }
}
