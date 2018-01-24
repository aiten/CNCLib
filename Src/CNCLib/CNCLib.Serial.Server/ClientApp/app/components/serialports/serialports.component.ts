import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';
import { Router } from '@angular/router';
import { SerialPortDefinition } from '../models/serial.port.definition';

@Component({
    selector: 'serialports',
    templateUrl: './serialports.component.html'
})
export class SerialPortsComponent {
    public serialports: SerialPortDefinition[];
    public historyport: SerialPortDefinition;

    constructor(
        http: Http, 
        @Inject('BASE_URL') public baseUrl: string,
        public router: Router)
        {
            http.get(baseUrl + 'api/SerialPort').subscribe(result => 
            {
                this.serialports = result.json() as SerialPortDefinition[];
            }, error => console.error(error));
        }

    showHistory(showhistoryport: SerialPortDefinition)
    {
        this.historyport = showhistoryport;
    }
}
