import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';
import { Router } from '@angular/router';

@Component({
    selector: 'serialports',
    templateUrl: './serialports.component.html'
})
export class SerialPortsComponent {
    public serialports: SerialPortDefinition[];

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

    showHistory(id: number)
    {
      console.log('Show history:' + this.baseUrl + id);
      this.router.navigate([this.baseUrl + 'serialports',id,'history']);
    }
}

interface SerialPortDefinition {
    id: number;
    portName: string;
    isConnected: number;
}
