import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'serialports',
    templateUrl: './serialports.component.html'
})
export class SerialPortsComponent {
    public serialports: SerialPortDefinition[];

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        http.get(baseUrl + 'api/SerialPort').subscribe(result => {
            this.serialports = result.json() as SerialPortDefinition[];
        }, error => console.error(error));
    }
}

interface SerialPortDefinition {
    id: number;
    portName: string;
    isConnected: number;
}
