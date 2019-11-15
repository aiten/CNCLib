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
import { LoadOptions, EHoleType, ELoadType, PenType, SmoothTypeEnum, ConvertTypeEnum, DitherFilter } from
  "../../models/load-options";
import { CNCLibLoadOptionService } from '../../services/CNCLib-load-option.service';
import { Router, ActivatedRoute, Params, ParamMap } from '@angular/router';
import { MatDialog, MatDialogConfig } from "@angular/material/dialog";

import { MessageBoxComponent } from "../../modal/message-box/message-box.component";
import { MessageBoxData } from "../../modal/message-box-data";
import HPGL = ELoadType.HPGL;
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

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private loadOptionService: CNCLibLoadOptionService,
    private dialog: MatDialog
  ) {
  }

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

  detailLoadOption(id: number) {

    const dialogRef = this.dialog.open(MessageBoxComponent,
      {
        width: '250px',
        data: { title: "Info", message: "Hallo" }
      });

    dialogRef.afterClosed().subscribe(result => {
//      this.animal = result;
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
    return this.isHPGL() && this.entry.PenMoveType == ZMove;
  }

  async ngOnInit() {

    let id = this.route.snapshot.paramMap.get('id');
    this.entry = await this.loadOptionService.getById(+id);
    /*
        this.entry = this.route.paramMap.pipe(
          switchMap(async (params: ParamMap) =>
            await this.machineService.getById(+(params.get('id'))))
        );
    */
  }
}
