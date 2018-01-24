import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';
import { Router } from '@angular/router';

@Component({
    selector: 'serialports',
    templateUrl: './serialports.component.html'
})
export class SerialPortsComponent {
    public serialports: SerialPortDefinition[];
    public historyid: number;

    constructor(
        http: Http, 
        @Inject('BASE_URL') public baseUrl: string,
        public router: Router)
        {
            http.get(baseUrl + 'api/SerialPort').subscribe(result => 
            {
                this.serialports = result.json() as SerialPortDefinition[];
            }, error => console.error(error));
            this.historyid = -1;
        }

    showHistory(id: number)
    {
        this.historyid = id;
    }
}

interface SerialPortDefinition {
    id: number;
    portName: string;
    isConnected: number;
}
