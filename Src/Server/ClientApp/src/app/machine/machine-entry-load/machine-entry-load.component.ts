import { Component, OnInit } from '@angular/core';
import { Machine } from '../../models/machine';
import { MachineCommand } from '../../models/machine-command';
import { MachineInitCommand } from '../../models/machine-init-command';
import { CNCLibMachineService } from '../../services/CNCLib-machine.service';
import { ActivatedRoute, Params } from '@angular/router';
import { machineURL } from '../machine-routing';
import { Router } from '@angular/router';

@Component({
  selector: 'ha-machine-entry-load',
  templateUrl: './machine-entry-load.component.html',
  styleUrls: ['./machine-entry-load.component.css']
})
export class MachineEntryLoadComponent  implements OnInit
{
  entry: Machine;
  errorMessage: string = '';
  isLoading: boolean = true;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private machineService: CNCLibMachineService
  ) {
    this.entry = new Machine();
  }

/*
  deleteMachine(id: number)
  {
    console.log('Detail machine');
    this.machineService
      .deleteMachineById(id)
      .subscribe(() => this.router.navigate([machineURL]));
  }

  updateMachine(id: number)
  {
    console.log('Update machine');
    this.router.navigate([machineURL + '/detail',this.entry.id,'edit'])
  }

  testUpdateMachine(id: number)
  {
    console.log('Update machine');
    this.machineService
      .getById(id)
      .subscribe(
      p =>
      {
        p.description = p.description + "X";

        let mc = new MachineCommand();
        mc.commandString = "Hallo";
        mc.commandName = "Hallo";
        mc.posX = p.commands.length + 1;
        mc.posY = 2;
        mc.joystickMessage = "JoystickMessage";
        p.commands.push(mc);

        let mi = new MachineInitCommand();
        mi.commandString = "Hallo";
        mi.seqNo = p.initCommands.length + 1;
        p.initCommands.push(mi);

        this.machineService
          .updateMachine(p)
          .subscribe(
          newp => 
          {
            this.router.navigate(['machineURL']);
          })
      });
  }
*/
  async ngOnInit() {
//    this.route.params
//      .switchMap((params: Params) => this.machineService.getById(+params['id']))
//      .subscribe(
//        /* happy path */
//        p => this.entry = p,
//        /* error path */
//        e => this.errorMessage = e,
//        /* onComplete */
//        () => this.isLoading = false);
  }
}

