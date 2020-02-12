import { Component, Inject, Input, OnChanges, OnInit, ViewChild } from '@angular/core';
import { SerialCommand } from '../models/serial.command';
import { SerialPortDefinition } from '../models/serial.port.definition';
import { SerialServerService } from '../services/serialserver.service';
import { HubConnection } from '@aspnet/signalr';
import { HubConnectionBuilder } from '@aspnet/signalr';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';

import { SerialServerConnection } from '../serialServer/serialServerConnection';

@Component({
  selector: 'serialporthistory',
  templateUrl: './serialporthistory.component.html',
  styleUrls: ['./serialporthistory.component.css']
})
export class SerialPortHistoryComponent implements OnChanges {

  forserialportid: number = -1;

  @Input()
  autoreloadonempty: boolean = false;

  serialcommands!: SerialCommand[];
  serialcommandsDataSource = new MatTableDataSource<SerialCommand>(this.serialcommands);

  displayedColumns: string[] = [/* 'SeqId', */ 'SentTime', 'CommandText', 'ReplyType', 'ResultText', 'ReplyReceivedTime'];

  @ViewChild(MatPaginator, { static: false })
  paginator: MatPaginator;

  private _hubConnection: HubConnection;
  private _initDone: boolean = false;

  constructor(
    private serivalServerService: SerialServerService,
    public serialServer: SerialServerConnection
  ) {
  }

  async ngOnInit() {

    console.log('ngOnInit');

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
