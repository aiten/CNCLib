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
import { CNCLibLoadOptionService } from '../../services/CNCLib-load-option.service';
import { CNCLibGCodeService } from '../../services/CNCLib-gcode.service';
import { Router, ActivatedRoute, Params, ParamMap } from '@angular/router';

@Component(
  {
    selector: 'ha-gcode-run',
    templateUrl: './gcode-run.component.html',
    styleUrls: ['./gcode-run.component.css']
  })
export class GcodeRunComponent implements OnInit {
  entry: LoadOptions;
  gCommands: string[];
  errorMessage: string = '';
  isLoading: boolean = true;
  isLoaded: boolean = false;
  isShowImage: boolean = true;

  constructor(
    private route: ActivatedRoute,
    private loadOptionService: CNCLibLoadOptionService,
    private gCodeService: CNCLibGCodeService,
  ) {
  }

  async createGCode() {
    this.gCommands = await this.gCodeService.createGCode(this.entry);
  }

  async ngOnInit() {

    let id = this.route.snapshot.paramMap.get('id');
    this.entry = await this.loadOptionService.getById(+id);
    console.log("loadOptions loaded");

    this.isLoaded = true;

  }
}
