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

import { Component, OnInit, AfterViewInit, ViewChild, ElementRef } from '@angular/core';

import { CNCLibGCodeService } from '../../services/CNCLib-gcode.service';
import { PreviewGCode } from '../../models/gcode-view-input';
import { Router, ActivatedRoute } from '@angular/router';

import { SerialServerConnection } from "../../serial-server/serial-server-connection";
import { SerialServerService } from '../../services/serial-server.service';

import { PreviewGlobal } from '../preview.global';
import { gcodeURL } from '../../app.global';

import { interval } from 'rxjs';
import { takeWhile } from 'rxjs/operators';

@Component(
  {
    selector: 'preview-detail',
    templateUrl: './preview-view.component.html',
    styleUrls: ['./preview-view.component.css']
  })
export class PreviewViewComponent implements OnInit, AfterViewInit {
  public previewOpt: PreviewGCode;

  @ViewChild('imagediv')
  imagediv: ElementRef;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private gCodeService: CNCLibGCodeService,
    public serialServer: SerialServerConnection,
    private serialServerService: SerialServerService,
    public previewGlobal: PreviewGlobal
  ) {
    this.previewOpt = previewGlobal.previewOpt;
  }

  async refreshImage() {
    this.needRedraw = false;
    await this.getThumbnail();
  }

  async toggleShowParam(enable: boolean) {
    this.previewGlobal.isShowParam = enable;
    this.refreshImage();
  }

  setRenderSizeFromDiv() {
    if (this.imagediv !== null && this.imagediv.nativeElement !== null) {
      this.previewOpt.renderSizeX = this.imagediv.nativeElement.clientWidth - 40;
    }
  }

  ngAfterViewInit() {
    this.setRenderSizeFromDiv();
    this.refreshImage();
  }

  imageBlobUrl: string;

  async getThumbnail(): Promise<void> {
    this.previewOpt.commands = this.previewGlobal.commands;

    if (this.previewOpt.commands != null) {
      this.previewOpt.rotate3DVect = [this.previewOpt.rotate3DVectX, this.previewOpt.rotate3DVectY, this.previewOpt.rotate3DVectZ];
      let blob = await this.gCodeService.getGCodeAsImage(this.previewOpt);
      await this.createImageFromBlob(blob);
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

  async ngOnInit() {
    await this.refreshImage();
  }

  reRun() {
    this.router.navigate([gcodeURL, 'run']);
  }

  canSendToMachine(): boolean {
    return this.serialServer.getMachine() != null && this.serialServer.getMachine().commandSyntax != 7;
  }

  async sendToMachine(): Promise<void> {
    if (this.serialServer.getMachine() != null) {
      this.serialServerService.setBaseUrl(this.serialServer.getSerialServerUrl(), this.serialServer.getSerialServerAuth());
      let id = this.serialServer.getSerialServerPortId();
      await this.serialServerService.queueCommands(id, this.previewOpt.commands, 1000000);
    }
  }

  sendCommandToMachine(command: string) {
    if (this.serialServer.getMachine() != null) {
      this.serialServerService.setBaseUrl(this.serialServer.getSerialServerUrl(), this.serialServer.getSerialServerAuth());
      let id = this.serialServer.getSerialServerPortId();
      this.serialServerService.queueCommands(id, [command], 1000000);
    }
  }

  setDefaultPreview() {
    this.previewOpt.zoom = 1;
    this.previewOpt.offsetX = 0;
    this.previewOpt.offsetY = 0;
  }

  async setXY() {
    this.setDefaultPreview();
    this.previewOpt.rotate3DAngle = 0;
    await this.refreshImage();
  }

  async setXZ() {
    this.setDefaultPreview();
    this.previewOpt.setVector([1, 1, 1]);
    this.previewOpt.rotate3DAngle = Math.PI * 4.0 / 3.0;
    await this.refreshImage();
  }

  async setYZ() {
    this.setDefaultPreview();
    this.previewOpt.setVector([1, 0, 0]);
    this.previewOpt.rotate3DAngle = -Math.PI / 2.0;
    await this.refreshImage();
  }

  async mouseWheelUpFunc() {
    this.previewOpt.zoom = this.previewOpt.zoom * 1.1;
    await this.refreshImage();
  }

  async mouseWheelDownFunc() {
    this.previewOpt.zoom = this.previewOpt.zoom / 1.1;
    await this.refreshImage();
  }

  public machineMoveTo: boolean = false;
  public clickPos: string;

  private mouseCaptured: boolean = false;
  private needRedraw: boolean = false;

  private shiftX: number;
  private shiftY: number;
  private ofsX: number;
  private ofsY: number;
  private scaleX: number;
  private scaleY: number;
  private mousebutton: number;

  calcScale(event) {
    this.scaleX = this.previewOpt.sizeX / this.previewOpt.renderSizeX / this.previewOpt.zoom;
    this.scaleY = this.previewOpt.sizeY / this.previewOpt.renderSizeY / this.previewOpt.zoom;

    if (this.scaleX === 0) this.scaleX = 200 / this.previewOpt.renderSizeX / this.previewOpt.zoom;
    if (this.scaleY === 0) this.scaleY = 200 / this.previewOpt.renderSizeY / this.previewOpt.zoom;

    //console.log(`sX:${this.scaleX}:Y:${this.scaleY}`);
    if (this.previewOpt.keepRatio) {
      if (this.scaleX < this.scaleY) {
        this.scaleX = this.scaleY;
      } else if (this.scaleX > this.scaleY) {
        this.scaleY = this.scaleX;
      }
      //console.log(`sX:${this.scaleX}:Y:${this.scaleY}`);
    }
  }

  onMouseDown(event) {
    if (this.machineMoveTo && event.button === 0 && (event.ctrlKey || event.altKey)) {
      this.calcScale(event);
      let diffX = event.clientX - event.layerX + event.srcElement.offsetLeft;
      let diffY = event.clientY - event.layerY;
      let posX = (event.clientX - diffX) * this.scaleX + this.previewOpt.offsetX;
      let posY = this.previewOpt.sizeY - (event.clientY - diffY) * this.scaleY - this.previewOpt.offsetY;

      let posXround = Math.round(posX * 100) / 100;
      let posYround = Math.round(posY * 100) / 100;
      this.clickPos = `${posXround}:${posYround}`;

      if (event.ctrlKey) {
        this.sendCommandToMachine(`G0X${posXround}Y${posYround}`);
      }

      event.preventDefault();
    } else {
      event.preventDefault();
      this.mouseCaptured = true;
      this.shiftX = event.clientX;
      this.shiftY = event.clientY;
      this.ofsX = this.previewOpt.offsetX;
      this.ofsY = this.previewOpt.offsetY;
      this.calcScale(event);
      this.mousebutton = event.button;

      interval(250)
        .pipe(
          takeWhile(() => this.mouseCaptured))
        .subscribe(
          (value) => { if (this.needRedraw) this.refreshImage() },
          (error) => console.error(error),
          () => console.log('Interval completed')
        );
    }
  }

  async onMouseMove(event) {
    if (this.mouseCaptured) {
      event.preventDefault();
      let diffX = this.shiftX - event.clientX;
      let diffY = this.shiftY - event.clientY;

      // console.log(`ofsX:${this.ofsX}:${this.ofsX + diffX * this.scaleX}, ofsY:${this.ofsY}:${this.ofsY + diffY * this.scaleY}`);

      if (this.mousebutton == 0) {
        this.previewOpt.offsetX = this.ofsX + diffX * this.scaleX;
        this.previewOpt.offsetY = this.ofsY + diffY * this.scaleY;
        // console.log(`ofsX:${this.ofsX}:${this.previewOpt.offsetX}, ofsY:${this.ofsY}:${this.previewOpt.offsetY}`);

      } else if (this.mousebutton === 2) {
        let maxDiffX = this.previewOpt.renderSizeX;
        let maxDiffY = this.previewOpt.renderSizeY;

        let rotateX = diffX / maxDiffX;
        let rotateY = diffY / maxDiffY;

        this.previewOpt.rotate3DAngle = 2.0 * Math.PI * (Math.abs(rotateX) > Math.abs(rotateY) ? rotateX : rotateY);

        this.previewOpt.rotate3DVectY = diffX;
        this.previewOpt.rotate3DVectX = -diffY;
      }

      this.needRedraw = true;
    }
  }

  onMouseUp(event) {
    if (this.mouseCaptured) {
      this.mouseCaptured = false;
      event.preventDefault();
    }
  }
}
