import { Component } from '@angular/core';

@Component({
    selector: 'home',
    templateUrl: './home.component.html'
})
export class HomeComponent {

    appName: string;
    appVersion: string;
    appCopyright: string;

    constructor() {
        this.appName = 'CNCLib.SerialServer';
        this.appVersion = '0.0.1';
        this.appCopyright = '(c) by Herbert Aitenbichler';
    }
}
