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

import { Router } from '@angular/router';
import { Component, Inject, Input, Output, OnChanges, OnInit, ViewChild, EventEmitter } from '@angular/core';
import { SerialCommand } from '../models/serial.command';
import { SerialPortDefinition } from '../models/serial.port.definition';
import { SerialServerService } from '../services/serialserver.service';
import { HubConnection, HubConnectionBuilder, HttpTransportType, LogLevel } from '@aspnet/signalr';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'serialportpending',
  templateUrl: './serialportpending.component.html',
  styleUrls: ['./serialportpending.component.css']
})
export class SerialPortPendingComponent implements OnChanges {

  @Input()
  forserialportid!: number;

  @Input()
  autoreloadonempty: boolean = false;

  @Output('update')
  change: EventEmitter<number> = new EventEmitter<number>();

  serialcommands: SerialCommand[] = [];
  serialcommandsDataSource = new MatTableDataSource<SerialCommand>(this.serialcommands);

  displayedColumns: string[] = [/* 'SeqId', */ 'SentTime', 'CommandText'];

  @ViewChild(MatPaginator)
  paginator: MatPaginator;

  private _hubConnection: HubConnection;
  private _initDone: boolean = false;

  constructor(
    private serivalServerService: SerialServerService,
    public router: Router,
    @Inject('WEBAPI_URL') public baseUrl: string,
  ) {
  }

  async ngOnInit() {

    if (this.autoreloadonempty) {
      console.log('SignalR to ' + this.baseUrl + 'serialSignalR');

      this._hubConnection = new HubConnectionBuilder()
        .configureLogging(LogLevel.Debug)
        .withUrl(this.baseUrl + 'serialSignalR/')
        .build();

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

  history() {
    this.change.emit(1);
  }

  async refresh(): Promise<void> {

    if (this._initDone) {
      this.serialcommands = await this.serivalServerService.getPending(this.forserialportid);

      this.serialcommandsDataSource = new MatTableDataSource<SerialCommand>(this.serialcommands);
      this.serialcommandsDataSource.paginator = this.paginator;
    }
  }

  async preview(): Promise<void> {
    this.router.navigate(["machinecontrol", this.forserialportid, "preview"]);
  }

}
