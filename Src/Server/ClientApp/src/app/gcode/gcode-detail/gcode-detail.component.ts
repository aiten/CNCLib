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
    selector: 'ha-gcode-detail',
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
        Id: [0, [Validators.required]],
        SettingName: ['', [Validators.required, Validators.maxLength(64)]],
        FileName: ['', [Validators.required, Validators.maxLength(512)]],
        LoadType: [],
        StartupCommands: ['', [Validators.maxLength(512)]],
        ShutdownCommands: ['', [Validators.maxLength(512)]],
        SubstG82: [false, [Validators.required]],
        AddLineNumbers: [false, [Validators.required]],

        SwapXY: [false, [Validators.required]],
        AutoScale: [false, [Validators.required]],

        AutoScaleKeepRatio: [false, [Validators.required]],
        AutoScaleCenter: [false, [Validators.required]],

        AutoScaleSizeX: [0.0, [Validators.required]],
        AutoScaleSizeY: [0.0, [Validators.required]],
        AutoScaleBorderDistX: [0.0, [Validators.required]],
        AutoScaleBorderDistY: [0.0, [Validators.required]],

        PenMoveType: [],
        EngravePosInParameter: [false, [Validators.required]],
        EngravePosUp: [0.0, [Validators.required]],
        EngravePosDown: [0.0, [Validators.required]],
        MoveSpeed: [0.0, [Validators.required]],
        EngraveDownSpeed: [0.0],

        ScaleX: [0.0],
        ScaleY: [0.0],
        OfsX: [0.0],
        OfsY: [0.0],

        LaserFirstOnCommand: ['', [Validators.maxLength(512)]],
        LaserOnCommand: ['', [Validators.maxLength(512)]],
        LaserOffCommand: ['', [Validators.maxLength(512)]],
        LaserLastOffCommand: ['', [Validators.maxLength(512)]],
        LaserSize: [0.0],
        LaserAccDist: [0.0],

        SmoothType: [],
        ConvertType: [],
        SmoothMinAngle: [0.0],
        SmoothMinLineLength: [0.0],
        SmoothMaxError: [0.0],

        ImageWriteToFileName: ['', [Validators.maxLength(512)]],
        GrayThreshold: [0.0],
        ImageDPIX: [0.0],
        ImageDPIY: [0.0],
        ImageInvert: [false, [Validators.required]],
        Dither: [],
        NewspaperDitherSize: [0.0],

        HoleType: [],
        UseYShift: [false, [Validators.required]],
        DotDistX: [0.0],
        DotDistY: [0.0],
        DotSizeX: [0.0],
        DotSizeY: [0.0],
        RotateHeart: [0],


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
    console.log(value);
    Object.assign(this.entry, value);
    await this.loadOptionService.updateLoadOption(this.entry);
    console.log("saved");
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

  isHpgl() {
    return this.entry.LoadType == ELoadType.Hpgl;
  }

  isHpglorImageOrImageHole() {
    return this.isHpgl() || this.isImageOrImageHole();
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
    return this.isHpglorImageOrImageHole() && this.entry.AutoScale;
  }

  isScale() {
    return this.isHpglorImageOrImageHole() && this.entry.AutoScale == false;
  }

  isSmooth() {
    return this.isHpgl() && this.entry.SmoothType != SmoothTypeEnum.NoSmooth;
  }

  isLaser() {
    return (this.isHpgl() && this.entry.PenMoveType == PenType.CommandString) || this.isImageOrImageHole();
  }

  isEngrave() {
    return this.isHpgl() && this.entry.PenMoveType == PenType.ZMove;
  }

  async ngOnInit() {
    let id = this.route.snapshot.paramMap.get('id');
    this.entry = await this.loadOptionService.getById(+id);
    this.gCodeForm.patchValue(this.entry);
    this.isLoaded = true;
  }
}
