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

import { Component, OnInit, Inject, Input, SimpleChanges } from '@angular/core';

import { Router, ActivatedRoute, Params, ParamMap } from '@angular/router';

import { SerialServerService } from '../../services/serialserver.service';
import { HubConnection, HubConnectionBuilder, HttpTransportType, LogLevel } from '@aspnet/signalr';

import { PreviewGlobal } from '../preview.global';
import { PreviewGCode } from '../../models/preview-input';

@Component(
  {
    selector: 'preview-detail',
    templateUrl: './preview-view.component.html',
    styleUrls: ['./preview-view.component.css']
  })
export class PreviewViewComponent implements OnInit {
  public previewOpt: PreviewGCode;
  public serialId: number = -1;
  private _hubConnection: HubConnection;

  constructor(
    private route: ActivatedRoute,
    private serialServerService: SerialServerService,
    public previewGlobal: PreviewGlobal,
    @Inject('WEBAPI_URL') public baseUrl: string,
  ) {
    this.previewOpt = previewGlobal.previewOpt;
  }

  async refreshImage() {
    await this.getThumbnail();
  }

  ngOnChanges(changes: SimpleChanges) {
  }

  imageBlobUrl: string;

  async getThumbnail(): Promise<void> {
    if (this.serialId >= 0) {
      this.previewOpt.rotate3DVect = [this.previewOpt.rotate3DVectX, this.previewOpt.rotate3DVectY, this.previewOpt.rotate3DVectZ];
      let blob = await this.serialServerService.getGCodeAsImage(this.serialId, this.previewOpt);
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

  async ngOnInit(): Promise<void> {
    this.route.params.subscribe(params => {
      this.serialId = params['id'];
      this.refreshImage();

      console.log('SignalR to ' + this.baseUrl + 'serialSignalR');

      this._hubConnection = new HubConnectionBuilder()
        .configureLogging(LogLevel.Debug)
        .withUrl(this.baseUrl + 'serialSignalR/')
        .build();

      this._hubConnection.on('QueueChanged',
        (portid: number) => {
          if (portid == this.serialId) {
            this.refreshImage();
          }
        });
      this._hubConnection.on('HeartBeat',
        () => {
          console.log('SignalR received: HeartBeat');
        });

      console.log("hub Starting:");

      this._hubConnection.start()
        .then(() => {
          console.log('Hub connection started');
        })
        .catch(err => {
          console.log('Error while establishing connection');
        });
    });
  }
}
