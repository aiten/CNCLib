import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';
import { Router } from '@angular/router';
import { SerialPortDefinition } from '../models/serial.port.definition';

@Component({
    selector: 'serialports',
    templateUrl: './serialports.component.html'
})
export class SerialPortsComponent
{
    public serialports: SerialPortDefinition[];
    public historyportid: number;
    public counter: number = 0;

    constructor(
        private http: Http,
        @Inject('BASE_URL') public baseUrl: string,
        public router: Router)
    {
        this.reload();
    }

    reload()
    {
        this.http.get(this.baseUrl + 'api/SerialPort').subscribe(result => {
            this.serialports = result.json() as SerialPortDefinition[];
            this.counter = this.counter + 1;
        }, error => console.error(error));
    }

    showHistory(showhistoryportid: number)
    {
        this.historyportid = showhistoryportid;
    }

    abort(serialportid: number)
    {
        console.log('Abort ' + serialportid);
        this.http.post(this.baseUrl + 'api/SerialPort/' + serialportid + '/abort',"x").
            subscribe(result =>
            {
                this.http.post(this.baseUrl + 'api/SerialPort/' + serialportid + '/resume', "x").
                    subscribe(result => {
                    }, error => console.error(error))
            }, error => console.error(error));
    }
}
