import { Component, Inject, OnInit } from '@angular/core';
import { Http } from '@angular/http';
import { Router } from '@angular/router';
import { SerialPortDefinition } from '../../models/serial.port.definition';
import { machinecontrolURL } from '../machinecontrol.routing';



@Component({
    selector: 'machinecontroloverview',
    templateUrl: './machinecontrol-overview.component.html'
})
export class MachineControlOverviewComponent
{
    serialports!: SerialPortDefinition[];

    constructor(
        private http: Http,
        @Inject('BASE_URL') public baseUrl: string,
        public router: Router) 
    {
    }

    useport(serialport: SerialPortDefinition)
    {
        this.router.navigate([machinecontrolURL, serialport.Id]);
    }

    reload()
    {
        this.http.get(this.baseUrl + 'api/SerialPort').subscribe(result => 
        {
            this.serialports = result.json() as SerialPortDefinition[];
        }, error => console.error(error));
    }

    ngOnInit() 
    {
        this.reload();
    }
}
