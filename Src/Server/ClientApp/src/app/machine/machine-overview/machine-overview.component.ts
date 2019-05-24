import { Component, OnInit } from '@angular/core';
import { Machine } from '../../models/machine';
import { CNCLibMachineService } from '../../services/CNCLib-machine.service';
import { machineURL } from '../machine-routing';
import { Router } from '@angular/router';

@Component(
{
  selector: 'ha-machine-overview',
  templateUrl: './machine-overview.component.html',
  styleUrls: ['./machine-overview.component.css']
})
export class MachineOverviewComponent implements OnInit
{
  entries: Machine[] = [];
  errorMessage: string = '';
  isLoading: boolean = true;

  constructor(
    private router: Router,
    private machineService: CNCLibMachineService
  )
  {
  }

  detailMachine(id: number)
  {
    console.log('Detail machine');
    this.router.navigate([machineURL + '/detail', id])
  }

  async newMachine()
  {
/*
    this.machineService
      .getDefault()
      .subscribe(
        p => { this.machineService
                .addMachine(p)
                .subscribe( 
                  newp => 
                  { 
                    this.router.navigate([machineURL + '/detail',newp.id]); 
                  } ) } );
*/
  }

  async ngOnInit() {
    this.entries = await this.machineService.getAll();
  }
}
