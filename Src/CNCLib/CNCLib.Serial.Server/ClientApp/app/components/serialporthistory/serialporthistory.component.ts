import { Component, Inject, Input, OnChanges } from '@angular/core';
import { SerialCommand } from '../../models/serial.command';
import { SerialPortDefinition } from '../../models/serial.port.definition';
import { SerialServerService } from '../../services/serialserver.service';

@Component({
    selector: 'serialporthistory',
    templateUrl: './serialporthistory.component.html'
})
export class SerialPortHistoryComponent implements OnChanges 
{
    @Input() forserialportid!: number;
    serialcommands!: SerialCommand[];

    constructor(
        private serivalServerService: SerialServerService,
    ) 
    {
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
