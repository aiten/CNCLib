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

import { Component, OnInit, Input, SimpleChanges } from '@angular/core';

import { CNCLibGCodeService } from '../../services/CNCLib-gcode.service';
import { PreviewGCode } from '../../models/gcode-view-input';
import { Router, ActivatedRoute, Params, ParamMap } from '@angular/router';

@Component(
  {
    selector: 'ha-gcode-preview',
    templateUrl: './gcode-preview.component.html',
    styleUrls: ['./gcode-preview.component.css']
  })
export class GcodePreviewComponent implements OnInit {
  @Input()
  gCommands: string[];
  previewOpt: PreviewGCode;
  isShowParam: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private gCodeService: CNCLibGCodeService,
  ) {
    this.previewOpt = new PreviewGCode();
    this.previewOpt.sizeX = 200;
    this.previewOpt.sizeY = 200;
    this.previewOpt.sizeZ = 200;

    this.previewOpt.offsetX = 0;
    this.previewOpt.offsetY = 0;
    this.previewOpt.offsetZ = 0;

    this.previewOpt.zoom = 1.0;
    this.previewOpt.laserSize = 0.254;
    this.previewOpt.cutterSize = 0;
    this.previewOpt.renderSizeX = 800;
    this.previewOpt.renderSizeY = 800;

    this.previewOpt.rotate3DAngle = 0;
    this.previewOpt.rotate3DVectX = 0;
    this.previewOpt.rotate3DVectY = 0;
    this.previewOpt.rotate3DVectZ = 1.0;
    this.previewOpt.rotate3DVect = [this.previewOpt.rotate3DVectX, this.previewOpt.rotate3DVectY, this.previewOpt.rotate3DVectZ];
  }

  async refreshImage() {
    await this.getThumbnail();
  }

  ngOnChanges(changes: SimpleChanges) {
    console.log(changes);
  }

  imageBlobUrl: string;

  async getThumbnail(): Promise<void> {
    this.previewOpt.commands = this.gCommands;

    console.log("getGCode");
    this.previewOpt.rotate3DVect = [this.previewOpt.rotate3DVectX, this.previewOpt.rotate3DVectY, this.previewOpt.rotate3DVectZ];
    let blob = await this.gCodeService.getGCodeAsImage(this.previewOpt);
    console.log("getGCode image");
    let ok = await this.createImageFromBlob(blob);
  }

  async createImageFromBlob(image: Blob): Promise<void> {
    let reader = new FileReader();
    reader.addEventListener("load",
      () => {
        this.imageBlobUrl = reader.result as string;
      },
      false);
    if (image) {
      reader.readAsDataURL(image);
    }
  }

  async ngOnInit() {

    await this.refreshImage();
  }
}
