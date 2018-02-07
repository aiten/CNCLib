import { Component, Inject, Input } from '@angular/core';
import { FormGroup, FormArray, FormControl, FormBuilder, Validators } from '@angular/forms';
import { Http } from '@angular/http';
import { Router } from '@angular/router';
import { SerialPortDefinition } from '../../models/serial.port.definition';
import { SerialConnect } from '../../models/serial.connect';

@Component({
    selector: 'machinecontrolconnect',
    templateUrl: './machinecontrol-connect.component.html'
})
export class MachineControlConnectComponent
{
    @Input() entry: SerialPortDefinition;

    errorMessage: string = '';
    isLoading: boolean = true;
    isLoaded: boolean = false;
    connectOptions: SerialConnect;

    setupForm: FormGroup;

    constructor(
        private http: Http,
        @Inject('BASE_URL') public baseUrl: string,
        public router: Router,
        private fb: FormBuilder
    )
    {
        this.connectOptions = new SerialConnect();

        this.setupForm = fb.group(
            {
                baudrate: [115200]
            });

        this.setupForm.valueChanges.subscribe((value) =>
        {
            if (this.isLoaded)
                Object.assign(this.connectOptions, value);
        });
    }

    save(value: any) 
    {
        console.log(value);
        Object.assign(this.connectOptions, value);

        console.log('save:' + this.entry.Id);

        this.http.post(this.baseUrl + 'api/SerialPort/' + this.entry.Id + '/connect/?baudrate=' + this.connectOptions.baudRate, "x").
            subscribe(result =>
            {
               //  this.load
            }, error => console.error(error))



    }
}
