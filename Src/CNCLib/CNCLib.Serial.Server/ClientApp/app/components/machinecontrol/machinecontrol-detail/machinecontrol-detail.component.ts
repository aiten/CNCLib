import { Component, Injectable, Inject, OnInit } from '@angular/core';
import { Http } from '@angular/http';
import { Router } from '@angular/router';
import { ActivatedRoute, Params } from '@angular/router';
import { SerialPortDefinition } from '../../models/serial.port.definition';

@Component({
    selector: 'machinecontroldetail',
    templateUrl: './machinecontrol-detail.component.html'
})
export class MachineControlDetailComponent
{
    serialport: SerialPortDefinition = new SerialPortDefinition();
    errorMessage: string = '';
    isLoading: boolean = true;
    serialId: number = 0;

    constructor(
        private http: Http,
        @Inject('BASE_URL') public baseUrl: string,
        public router: Router,
        private route: ActivatedRoute
    ) 
    {
    }

    load(id: number)
    {
        this.http.get(this.baseUrl + 'api/SerialPort/' + id).subscribe(result =>
        {
            this.serialport = result.json() as SerialPortDefinition;
            this.isLoading = false;
            console.log(this.serialport);
        });
    }

    ngOnInit()
    {
        this.route.params.subscribe(params =>
        {
            this.serialId = params['id'];
            this.load(this.serialId);
        });
    }
}
