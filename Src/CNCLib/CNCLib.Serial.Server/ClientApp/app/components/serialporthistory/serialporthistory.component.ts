import { Component, Inject, Input, OnChanges } from '@angular/core';
import { SerialCommand } from '../../models/serial.command';
import { SerialPortDefinition } from '../../models/serial.port.definition';
import { SerialServerService } from '../../services/serialserver.service';
import { HubConnection } from '@aspnet/signalr-client';

@Component({
    selector: 'serialporthistory',
    templateUrl: './serialporthistory.component.html'
})
export class SerialPortHistoryComponent implements OnChanges 
{
    @Input() forserialportid!: number;
    @Input() autoreloadonempty: boolean = false;
    serialcommands!: SerialCommand[];
    private _hubConnection!: HubConnection;
 //  public async: any;
 //   message = '';
 //   messages: string[] = [];

    constructor(
        private serivalServerService: SerialServerService,
        @Inject('BASE_URL') public baseUrl: string,
    ) 
    {
    }

    ngOnInit()
    {
        if (this.autoreloadonempty) 
        {
            this._hubConnection = new HubConnection(this.baseUrl + 'serialSignalR');
            //this._hubConnection = new HubConnection('/serialSignalR');

            this._hubConnection.on('queueEmpty',
                (portid: number) => 
                {
                    if (portid == this.forserialportid) 
                    {
                        this.refresh();
                    }
//            const received = `Received: ${data}`;
//            this.messages.push(received);
                });

            this._hubConnection.start()
                .then(() => 
                {
                    console.log('Hub connection started')
                })
                .catch(err => 
                {
                    console.log('Error while establishing connection')
                });
        }
    }

    async ngOnChanges() : Promise<void>
    {
        console.log('ngOnChanges');
        await this.refresh();
    }

    async refresh(): Promise<void>
    {
        this.serialcommands = await this.serivalServerService.getHistory(this.forserialportid);
    }

    async clear(): Promise<void>
    {
        await this.serivalServerService.clearHistory(this.forserialportid);
        await this.refresh();
    }
}
