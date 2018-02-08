import { Component, Inject, Input, OnChanges } from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { ActivatedRoute, Params } from '@angular/router';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/switchMap';
import { SerialCommand } from '../models/serial.command';
import { SerialPortDefinition } from '../models/serial.port.definition';

@Component({
    selector: 'serialporthistory',
    templateUrl: './serialporthistory.component.html'
})
export class SerialPortHistoryComponent implements OnChanges {
    @Input() forserialportid!: number;
    serialcommands!: SerialCommand[];

    constructor(
        public http: Http,
        @Inject('BASE_URL') public baseUrl: string
    ) {
    }

    public refresh() {
        console.log(this.forserialportid);
        this.http.get(this.baseUrl + 'api/SerialPort/' + this.forserialportid + '/history').
            subscribe(result => {
                this.serialcommands = result.json() as SerialCommand[];
            }, error => console.error(error));

    }

    public clear() {
        console.log(this.forserialportid);
        this.http.post(this.baseUrl + 'api/SerialPort/' + this.forserialportid + '/history/clear', "x").
            subscribe(result => {
                this.refresh();
            }, error => console.error(error));

    }

    ngOnChanges() {
        console.log('ngOnChanges');
        this.refresh();
    }
}

