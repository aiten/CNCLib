import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { SerialPortsComponent } from './components/serialports/serialports.component';
import { SerialPortHistoryComponent } from './components/serialporthistory/serialporthistory.component';
import { machineControlRoutes, machineControlRoutingComponents } from './components/machinecontrol/machinecontrol.routing';

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        SerialPortsComponent,
        SerialPortHistoryComponent,
        ...machineControlRoutingComponents,
        HomeComponent
    ],
    imports: [
        CommonModule,
        HttpModule,
        FormsModule,
        ReactiveFormsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'serialports', component: SerialPortsComponent },
            ...machineControlRoutes,

            { path: '**', redirectTo: 'home' }
        ])
    ]
})
export class AppModuleShared {
}
