import { Component, Inject, Output } from '@angular/core';
import { Http } from '@angular/http';
import { Router } from '@angular/router';
import { SerialPortDefinition } from '../models/serial.port.definition';

@Component({
    selector: 'machinecontroloverview',
    templateUrl: './machinecontrol-overview.component.html'
})
export class MachineControlOverviewComponent
{
    serialports: SerialPortDefinition[];
    useserialport: SerialPortDefinition;

    constructor(
        private http: Http,
        @Inject('BASE_URL') public baseUrl: string,
        public router: Router) 
    {
        this.reload();
    }

    useport(serialport: SerialPortDefinition) 
    {
        this.useserialport = serialport;
    }

    reload()
    {
        this.http.get(this.baseUrl + 'api/SerialPort').subscribe(result => 
        {
            this.serialports = result.json() as SerialPortDefinition[];
        }, error => console.error(error));
    }
}
