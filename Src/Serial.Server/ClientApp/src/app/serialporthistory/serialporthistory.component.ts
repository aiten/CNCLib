import { Component, Inject, Input, OnChanges, OnInit, ViewChild } from '@angular/core';
import { SerialCommand } from '../models/serial.command';
import { SerialPortDefinition } from '../models/serial.port.definition';
import { SerialServerService } from '../services/serialserver.service';
import { HubConnection } from '@aspnet/signalr';
import { HubConnectionBuilder } from '@aspnet/signalr';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'serialporthistory',
  templateUrl: './serialporthistory.component.html',
  styleUrls: ['./serialporthistory.component.css']
})
export class SerialPortHistoryComponent implements OnChanges {
  @Input()
  forserialportid!: number;
  @Input()
  autoreloadonempty: boolean = false;

  serialcommands!: SerialCommand[];
  serialcommandsDataSource = new MatTableDataSource<SerialCommand>(this.serialcommands);

  displayedColumns: string[] = ['SeqId', 'SentTime', 'CommandText', 'ReplyType', 'ResultText', 'ReplyReceivedTime'];

  @ViewChild(MatPaginator, { static: false })
  paginator: MatPaginator;

  private _hubConnection: HubConnection;

  constructor(
    private serivalServerService: SerialServerService,
    @Inject('BASE_URL') public baseUrl: string,
  ) {
  }

  async ngOnInit() {

    console.log('ngOnInit');

    if (this.autoreloadonempty) {
      console.log('SignalR to ' + this.baseUrl + 'serialSignalR');

      this._hubConnection = new HubConnectionBuilder().withUrl(this.baseUrl + 'serialSignalR').build();

      this._hubConnection.on('queueEmpty',
        (portid: number) => {
          if (portid == this.forserialportid) {
            this.refresh();
          }
        });
      this._hubConnection.on('heartbeat',
        () => {
          console.log('SignalR received: heartbeat');
        });

      this._hubConnection.start()
        .then(() => {
          console.log('Hub connection started');
        })
        .catch(err => {
          console.log('Error while establishing connection');
        });
    }

    console.log('ngOnInit done');

    await this.refresh();
  }

  async ngOnChanges(): Promise<void> {
    console.log('ngOnChanges');
    await this.refresh();
  }

  async refresh(): Promise<void> {

    console.log('refresh');

    this.serialcommands = await this.serivalServerService.getHistory(this.forserialportid);

    console.log('refresh1');
/*
    this.serialcommands = [
      { SeqId: 1, CommandText: 'g0x0y0', ReplyType: 'OK', SentTime: Date.now(), ReplyReceivedTime: Date.now() },
      { SeqId: 2, CommandText: 'g0x0y0', ReplyType: 'OK', SentTime: Date.now(), ReplyReceivedTime: Date.now() },
      { SeqId: 3, CommandText: 'g0x0y0', ReplyType: 'OK', SentTime: Date.now(), ReplyReceivedTime: Date.now() },
      { SeqId: 4, CommandText: 'g0x0y0', ReplyType: 'OK', SentTime: Date.now(), ReplyReceivedTime: Date.now() },
      { SeqId: 5, CommandText: 'g0x0y0', ReplyType: 'OK', SentTime: Date.now(), ReplyReceivedTime: Date.now() },
      { SeqId: 6, CommandText: 'g0x0y0', ReplyType: 'OK', SentTime: Date.now(), ReplyReceivedTime: Date.now() },
      { SeqId: 7, CommandText: 'g0x0y0', ReplyType: 'OK', SentTime: Date.now(), ReplyReceivedTime: Date.now() },
      { SeqId: 8, CommandText: 'g0x0y0', ReplyType: 'OK', SentTime: Date.now(), ReplyReceivedTime: Date.now() },
      { SeqId: 9, CommandText: 'g0x0y0', ReplyType: 'OK', SentTime: Date.now(), ReplyReceivedTime: Date.now() },
      { SeqId: 10, CommandText: 'g0x0y0', ReplyType: 'OK', SentTime: Date.now(), ReplyReceivedTime: Date.now() },
      { SeqId: 11, CommandText: 'g0x0y0', ReplyType: 'OK', SentTime: Date.now(), ReplyReceivedTime: Date.now() },
      { SeqId: 12, CommandText: 'g0x0y0', ReplyType: 'OK', SentTime: Date.now(), ReplyReceivedTime: Date.now() },
      { SeqId: 13, CommandText: 'g0x0y0', ReplyType: 'OK', SentTime: Date.now(), ReplyReceivedTime: Date.now() },
      { SeqId: 14, CommandText: 'g0x0y0', ReplyType: 'OK', SentTime: Date.now(), ReplyReceivedTime: Date.now() },
    ];
*/
    this.serialcommandsDataSource = new MatTableDataSource<SerialCommand>(this.serialcommands);
    this.serialcommandsDataSource.paginator = this.paginator;
    console.log('refresh done');
  }

  async clear(): Promise<void> {
    await this.serivalServerService.clearHistory(this.forserialportid);
    await this.refresh();
  }
}
