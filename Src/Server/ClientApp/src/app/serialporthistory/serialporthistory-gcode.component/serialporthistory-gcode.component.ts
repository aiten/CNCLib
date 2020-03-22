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

import { Component, Input, OnChanges, ViewChild } from '@angular/core';
import { SerialCommand } from '../../models/serial.command';
import { SerialServerService } from '../../services/serialserver.service';
import { HubConnection } from '@aspnet/signalr';
import { HubConnectionBuilder } from '@aspnet/signalr';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';

import { SerialServerConnection } from '../../serialServer/serialServerConnection';
import { SerialPortHistoryInput } from "../models/serialporthistory.input";
import { SerialPortHistoryPreviewGlobal } from "../models/serialporthistory.global";

@Component({
  selector: 'serialporthistorygcode',
  templateUrl: './serialporthistory-gcode.component.html',
  styleUrls: ['./serialporthistory-gcode.component.css']
})
export class SerialPortHistoryGCodeComponent implements OnChanges {

  forserialportid: number = -1;

  @Input()
  autoreloadonempty: boolean = false;

  serialcommands: SerialCommand[] = [];
  serialcommandsDataSource = new MatTableDataSource<SerialCommand>(this.serialcommands);

  displayedColumns: string[] = [/* 'SeqId', */ 'sentTime', 'commandText', 'replyType', 'resultText', 'replyReceivedTime'];

  @ViewChild(MatPaginator)
  paginator: MatPaginator;

  private _hubConnection: HubConnection;
  private _initDone: boolean = false;

  constructor(
    private serivalServerService: SerialServerService,
    public serialServer: SerialServerConnection,
    public previewGlobal: SerialPortHistoryPreviewGlobal
  ) {
  }

  async ngOnInit() {

    this.forserialportid = this.serialServer.getSerialServerPortId();

    if (this.autoreloadonempty) {
      console.log('SignalR to ' + this.serialServer.getSerialServerUrl() + 'serialSignalR');

      this._hubConnection = new HubConnectionBuilder().withUrl(this.serialServer.getSerialServerUrl() + 'serialSignalR/').build();

      this._hubConnection.on('QueueEmpty',
        (portid: number) => {
          if (portid == this.forserialportid) {
            this.refresh();
          }
        });
      this._hubConnection.on('HeartBeat',
        () => {
          console.log('SignalR received: HeartBeat');
        });

      console.log("hub Starting:");

      await this._hubConnection.start()
        .then(() => {
          console.log('Hub connection started');
        })
        .catch(err => {
          console.log('Error while establishing connection');
        });
    }

    this._initDone = true;

    await this.refresh();
  }

  async ngOnChanges(): Promise<void> {
    await this.refresh();
  }

  preview() {
    this.previewGlobal.isGCode = false;
  }

  async refresh(): Promise<void> {

    if (this._initDone) {
      this.serialcommands = await this.serivalServerService.getHistory(this.forserialportid);

      this.serialcommandsDataSource = new MatTableDataSource<SerialCommand>(this.serialcommands);
      this.serialcommandsDataSource.paginator = this.paginator;
    }
  }

  async clear(): Promise<void> {
    await this.serivalServerService.clearHistory(this.forserialportid);
    await this.refresh();
  }
}
