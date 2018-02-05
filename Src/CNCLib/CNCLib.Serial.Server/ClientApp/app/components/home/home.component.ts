import { Component, Inject, OnInit } from '@angular/core';
import { Http } from '@angular/http';
import { CNCLibServerInfo } from '../models/CNCLib.Server.Info'

@Component({
    selector: 'home',
    templateUrl: './home.component.html'
})
export class HomeComponent implements OnInit
{
    appName: string;
    appVersion: string;
    appCopyright: string;
    appVersionInfo: CNCLibServerInfo;

    constructor(
        private http: Http,
        @Inject('BASE_URL') public baseUrl: string
        )
    {
    }

    ngOnInit() 
    {
        this.http.get(this.baseUrl + 'api/Info').subscribe(result => 
        {
            this.appVersionInfo = result.json() as CNCLibServerInfo;
            this.appVersion = this.appVersionInfo.Version;
            this.appName = this.appVersionInfo.Name;
            this.appCopyright = this.appVersionInfo.Copyright;
        }, error => console.error(error));
    }
}
