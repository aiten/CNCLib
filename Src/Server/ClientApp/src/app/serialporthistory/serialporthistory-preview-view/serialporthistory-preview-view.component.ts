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

import { Component, OnInit, OnDestroy, Inject, AfterViewInit, ViewChild, ElementRef } from '@angular/core';

import { ActivatedRoute } from '@angular/router';

import { SerialServerService } from '../../services/serial-server.service';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';

import { SerialPortHistoryPreviewGlobal } from '../models/serialporthistory.global';
import { SerialPortHistoryInput } from '../models/serialporthistory.input';

import { interval } from 'rxjs';
import { takeWhile } from 'rxjs/operators';
import { SerialServerConnection } from "../../serial-server/serial-server-connection";

@Component(
  {
    selector: 'serialporthistroy-preview-view',
    templateUrl: './serialporthistory-preview-view.component.html',
    styleUrls: ['./serialporthistory-preview-view.component.css']
  })
export class SerialPortHistoryPreviewViewComponent implements OnInit, OnDestroy, AfterViewInit {
  public previewOpt: SerialPortHistoryInput;
  public serialId: number = -1;
  private hubConnection: HubConnection;

  @ViewChild('imagediv')
  imagediv: ElementRef;

  constructor(
    private route: ActivatedRoute,
    private serialServerService: SerialServerService,
    public serialServer: SerialServerConnection,
    public previewGlobal: SerialPortHistoryPreviewGlobal,
    @Inject('WEBAPI_URL') public baseUrl: string,
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
    if (this.serialId >= 0) {
      this.previewOpt.rotate3DVect = [this.previewOpt.rotate3DVectX, this.previewOpt.rotate3DVectY, this.previewOpt.rotate3DVectZ];
      let blob = await this.serialServerService.getGCodeAsImage(this.serialId, this.previewOpt);
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

  async ngOnInit(): Promise<void> {
    this.serialId = this.serialServer.getSerialServerPortId();

    this.refreshImage();

    console.log('SignalR to ' + this.serialServer.getSerialServerUrl() + 'serialSignalR');

    this.hubConnection = new HubConnectionBuilder()
      .configureLogging(LogLevel.Debug)
      .withUrl(this.serialServer.getSerialServerUrl() + 'serialSignalR/')
      .build();

    this.hubConnection.on('QueueChanged',
      (portid: number) => {
        if (portid == this.serialId) {
          this.refreshImage();
        }
      });
    this.hubConnection.on('HeartBeat',
      () => {
        console.log('SignalR received: HeartBeat');
      });

    console.log("hub Starting:");

    this.hubConnection.start()
      .then(() => {
        console.log('Hub connection started');
      })
      .catch(err => {
        console.log('Error while establishing connection');
      });
  }

  async ngOnDestroy(): Promise<void> {
    if (this.hubConnection != null) {
      await this.hubConnection.stop();
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
