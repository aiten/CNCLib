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

  constructor(
    private route: ActivatedRoute,
    private gCodeService: CNCLibGCodeService,
  ) {
    this.previewOpt = new PreviewGCode();
    this.previewOpt.SizeX = 200;
    this.previewOpt.SizeY = 200;
    this.previewOpt.SizeZ = 200;

    this.previewOpt.OffsetX = 0;
    this.previewOpt.OffsetY = 0;
    this.previewOpt.OffsetZ = 0;

    this.previewOpt.Zoom = 1.0;
    this.previewOpt.LaserSize = 0.254;
    this.previewOpt.CutterSize = 0;
    this.previewOpt.RenderSizeX = 800;
    this.previewOpt.RenderSizeY = 800;

    this.previewOpt.Rotate3DAngle = 0;
    this.previewOpt.Rotate3DVectX = 0;
    this.previewOpt.Rotate3DVectY = 0;
    this.previewOpt.Rotate3DVectZ = 1.0;
    this.previewOpt.Rotate3DVect = [this.previewOpt.Rotate3DVectX, this.previewOpt.Rotate3DVectY, this.previewOpt.Rotate3DVectZ];
  }

  async refreshImage() {
    await this.getThumbnail();
  }

  imageBlobUrl: string;

  async getThumbnail(): Promise<void> {
    this.previewOpt.commands = this.gCommands;

    console.log("getGCode");
    this.previewOpt.Rotate3DVect = [this.previewOpt.Rotate3DVectX, this.previewOpt.Rotate3DVectY, this.previewOpt.Rotate3DVectZ];
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
