import { NgModule } from '@angular/core';
import { MachineControlOverviewComponent } from './machinecontrol-overview/machinecontrol-overview.component';
import { MachineControlConnectComponent } from './machinecontrol-connect/machinecontrol-connect.component';
import { MachineControlDetailComponent } from './machinecontrol-detail/machinecontrol-detail.component';
import { MachineControlComponent } from './machinecontrol.component';
import { Routes, RouterModule } from '@angular/router';

export const machinecontrolURL = '/machinecontrol';

export const machineControlRoutes =
    [
        {
            path: 'machinecontrol', component: MachineControlComponent,
            children:
            [
                { path: '', component: MachineControlOverviewComponent },
                { path: ':id', component: MachineControlDetailComponent },
            ]
        }
    ];

export const machineControlRoutingComponents =
    [
        MachineControlComponent,
        MachineControlOverviewComponent,
        MachineControlDetailComponent,
        MachineControlConnectComponent
    ];