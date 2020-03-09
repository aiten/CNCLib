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

import { Component, OnInit } from '@angular/core';
import { LoadOptions, EHoleType, ELoadType, PenType, SmoothTypeEnum, ConvertTypeEnum, DitherFilter } from "../../models/load-options";
import { FormGroup, FormArray, FormControl, FormBuilder, Validators } from '@angular/forms';
import { CNCLibLoadOptionService } from '../../services/CNCLib-load-option.service';
import { Router, ActivatedRoute, Params, ParamMap } from '@angular/router';
import { MatDialog, MatDialogConfig } from "@angular/material/dialog";

import { MessageBoxComponent } from "../../modal/message-box/message-box.component";
import { MessageBoxData } from "../../modal/message-box-data";
import { gcodeURL } from '../../app.global';

import Hpgl = ELoadType.Hpgl;
import ZMove = PenType.ZMove;
import ImageHole = ELoadType.ImageHole;
import ImageHole1 = ELoadType.ImageHole;

@Component(
  {
    selector: 'gcode-detail',
    templateUrl: './gcode-detail.component.html',
    styleUrls: ['./gcode-detail.component.css']
  })
export class GcodeDetailComponent implements OnInit {
  entry: LoadOptions;
  errorMessage: string = '';
  isLoading: boolean = true;
  isLoaded: boolean = false;
  gCodeForm: FormGroup;
  keysELoadType: any[];
  keysPenType: any[];
  keysSmoothTypeEnum: any[];
  keysConvertTypeEnum: any[];
  keysDitherFilter: any[];
  keysEHoleType: any[];

  ELoadType: typeof
    ELoadType = ELoadType;

  PenType: typeof
    PenType = PenType;

  SmoothTypeEnum: typeof
    SmoothTypeEnum = SmoothTypeEnum;

  ConvertTypeEnum: typeof
    ConvertTypeEnum = ConvertTypeEnum;

  DitherFilter: typeof
    DitherFilter = DitherFilter;

  EHoleType: typeof
    EHoleType = EHoleType;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private loadOptionService: CNCLibLoadOptionService,
    private dialog: MatDialog,
    private fb: FormBuilder
  ) {

    this.keysELoadType = Object.keys(ELoadType).filter(f => !isNaN(Number(f)));
    this.keysPenType = Object.keys(PenType).filter(f => !isNaN(Number(f)));
    this.keysSmoothTypeEnum = Object.keys(SmoothTypeEnum).filter(f => !isNaN(Number(f)));
    this.keysConvertTypeEnum = Object.keys(ConvertTypeEnum).filter(f => !isNaN(Number(f)));
    this.keysDitherFilter = Object.keys(DitherFilter).filter(f => !isNaN(Number(f)));
    this.keysEHoleType = Object.keys(EHoleType).filter(f => !isNaN(Number(f)));

    this.gCodeForm = fb.group(
      {
        id: [0, [Validators.required]],
        settingName: ['', [Validators.required, Validators.maxLength(64)]],
        fileName: ['', [Validators.required, Validators.maxLength(512)]],
        loadType: [],
        startupCommands: ['', [Validators.maxLength(512)]],
        shutdownCommands: ['', [Validators.maxLength(512)]],
        substG82: [false, [Validators.required]],
        addLineNumbers: [false, [Validators.required]],

        swapXY: [false, [Validators.required]],
        autoScale: [false, [Validators.required]],

        autoScaleKeepRatio: [false, [Validators.required]],
        autoScaleCenter: [false, [Validators.required]],

        autoScaleSizeX: [0.0, [Validators.required]],
        autoScaleSizeY: [0.0, [Validators.required]],
        autoScaleBorderDistX: [0.0, [Validators.required]],
        autoScaleBorderDistY: [0.0, [Validators.required]],

        penMoveType: [],
        engravePosInParameter: [false, [Validators.required]],
        engravePosUp: [0.0, [Validators.required]],
        engravePosDown: [0.0, [Validators.required]],
        moveSpeed: [0.0, [Validators.required]],
        engraveDownSpeed: [0.0],

        scaleX: [0.0],
        scaleY: [0.0],
        ofsX: [0.0],
        ofsY: [0.0],

        laserFirstOnCommand: ['', [Validators.maxLength(512)]],
        laserOnCommand: ['', [Validators.maxLength(512)]],
        laserOffCommand: ['', [Validators.maxLength(512)]],
        laserLastOffCommand: ['', [Validators.maxLength(512)]],
        laserSize: [0.0],
        laserAccDist: [0.0],

        smoothType: [],
        convertType: [],
        smoothMinAngle: [0.0],
        smoothMinLineLength: [0.0],
        smoothMaxError: [0.0],

        imageWriteToFileName: ['', [Validators.maxLength(512)]],
        grayThreshold: [0.0],
        imageDPIX: [0.0],
        imageDPIY: [0.0],
        imageInvert: [false, [Validators.required]],
        dither: [],
        newspaperDitherSize: [0.0],

        holeType: [],
        useYShift: [false, [Validators.required]],
        dotDistX: [0.0],
        dotDistY: [0.0],
        dotSizeX: [0.0],
        dotSizeY: [0.0],
        rotateHeart: [0],


      });

    this.gCodeForm.valueChanges.subscribe((
      value) => {
      if (this.isLoaded)
        Object.assign(this.entry,
          value);
    });
  }

  compareWithEnum(lt1, lt2) {
    return lt1 == lt2;
  }

  newLoadOption() {

    const dialogRef = this.dialog.open(MessageBoxComponent,
      {
        width: '250px',
        data: { title: "Error", message: "Not implemented yet" }
      });

    dialogRef.afterClosed().subscribe(result => {
      //      this.animal = result;
    });

  }

  async savegCode(value: any): Promise<void> {
    Object.assign(this.entry, value);
    await this.loadOptionService.updateLoadOption(this.entry);
    window.location.reload();
// this.router.navigate([gcodeURL, 'detail', this.entry.Id]);
  }

  detailLoadOption(id:
    number) {

    const dialogRef = this.dialog.open(MessageBoxComponent,
      {
        width: '250px',
        data: { title: "Info", message: "Hallo" }
      });

    dialogRef.afterClosed().subscribe(result => {
//      this.animal = result;
    });

  }

  run() {
    this.router.navigate([gcodeURL, 'run', this.entry.id]);
  }

  isHpgl() {
    return this.entry.loadType == ELoadType.Hpgl;
  }

  isHpglorImageOrImageHole() {
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
    return this.isHpglorImageOrImageHole() && this.entry.autoScale;
  }

  isScale() {
    return this.isHpglorImageOrImageHole() && this.entry.autoScale == false;
  }

  isSmooth() {
    return this.isHpgl() && this.entry.smoothType != SmoothTypeEnum.NoSmooth;
  }

  isLaser() {
    return (this.isHpgl() && this.entry.penMoveType == PenType.CommandString) || this.isImageOrImageHole();
  }

  isEngrave() {
    return this.isHpgl() && this.entry.penMoveType == PenType.ZMove;
  }

  async ngOnInit() {
    let id = this.route.snapshot.paramMap.get('id');
    this.entry = await this.loadOptionService.getById(+id);
    this.gCodeForm.patchValue(this.entry);
    this.isLoaded = true;
  }
}
