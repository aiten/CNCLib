import { Component, Inject, OnInit } from '@angular/core';
import { SerialPortDefinition } from '../../models/serial.port.definition';
import { SerialServerService } from '../../services/serialserver.service';

@Component({
    selector: 'serialports',
    templateUrl: './serialports.component.html'
})
export class SerialPortsComponent
{
    public serialports!: SerialPortDefinition[];
    public historyportid!: number;

    constructor(
        private serivalServerService: SerialServerService
    )
    {
    }

    async ngOnInit(): Promise<void>
    {
        await this.reload();
    }

    async reload(): Promise<void>
    {
        this.serialports = await this.serivalServerService.getPorts();
    }

    async refresh(): Promise<void>
    {
        this.serialports = await this.serivalServerService.refresh();
    }

    showHistory(showhistoryportid: number)
    {
        this.historyportid = showhistoryportid;
    }

    async clearQueue(serialportid: number): Promise<void>
    {
        await this.serivalServerService.abort(serialportid);
        await this.serivalServerService.resume(serialportid);
    }

    async disconnect(serialportid: number): Promise<void>
    {
        await this.serivalServerService.disconnect(serialportid);
    }
}
