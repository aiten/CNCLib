import { NgModule } from '@angular/core';
import { MachineEntryComponent } from './machine-entry/machine-entry.component';
import { MachineOverviewComponent } from './machine-overview/machine-overview.component';
import { MachineEntryLoadComponent } from './machine-entry-load/machine-entry-load.component';
import { MachineFormComponent } from './machine-form/machine-form.component';
import { MachineComponent } from './machine.component';
import { Routes, RouterModule } from '@angular/router';

export const machineURL = '/machine';

export const machineRoutes =
  [
    {   path: 'machine', component: MachineComponent,
      children:
      [
        { path: '', component: MachineOverviewComponent }, 
        { path: 'detail/:id/edit', component: MachineFormComponent },
        { path: 'detail/:id', component: MachineEntryLoadComponent }
      ]
    }    
];

export const machineRoutingComponents = 
[
  MachineComponent,
  MachineOverviewComponent,
  MachineFormComponent,
  MachineEntryLoadComponent,
  MachineEntryComponent,
];