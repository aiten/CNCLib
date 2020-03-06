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

import { SerialServerConnection } from "../../serialServer/serialServerConnection";
import { SerialServerService } from '../../services/serialserver.service';

import { PreviewGlobal } from '../preview.global';

@Component(
  {
    selector: 'preview-detail',
    templateUrl: './preview-view.component.html',
    styleUrls: ['./preview-view.component.css']
  })
export class PreviewViewComponent implements OnInit {
  public previewOpt: PreviewGCode;

  constructor(
    private route: ActivatedRoute,
    private gCodeService: CNCLibGCodeService,
    public serialServer: SerialServerConnection,
    private serialServerService: SerialServerService,
    public previewGlobal: PreviewGlobal
  ) {
    this.previewOpt = previewGlobal.previewOpt;
  }

  async refreshImage() {
    await this.getThumbnail();
  }

  ngOnChanges(changes: SimpleChanges) {
    console.log(changes);
  }

  imageBlobUrl: string;

  async getThumbnail(): Promise<void> {
    this.previewOpt.commands = this.previewGlobal.commands;

    if (this.previewOpt.commands != null) {

      console.log("getGCode");
      this.previewOpt.rotate3DVect = [this.previewOpt.rotate3DVectX, this.previewOpt.rotate3DVectY, this.previewOpt.rotate3DVectZ];
      let blob = await this.gCodeService.getGCodeAsImage(this.previewOpt);
      console.log("getGCode image");
      let ok = await this.createImageFromBlob(blob);
    }
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

  canSendToMachine(): boolean {
    return this.serialServer.getMachine() != null;
  }

  async sendToMachine(): Promise<void> {
    if (this.serialServer.getMachine() != null) {
      console.log("sendToMachine");
      console.log(this.serialServer.getSerialServerUrl());

      this.serialServerService.setBaseUrl(this.serialServer.getSerialServerUrl(), this.serialServer.getSerialServerAuth());
      var id = this.serialServer.getSerialServerPortId();

      this.serialServerService.queueCommands(id, this.previewOpt.commands, 1000000);
      //this.serialport = await this.serialServerService.getPort(id);
      //this.isConnected = this.serialport.isConnected != 0;

    }
  }

  async ngOnInit() {

    await this.refreshImage();
  }
}
