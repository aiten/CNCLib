/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) Herbert Aitenbichler

  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
  to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
  and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

import { Component, OnInit } from '@angular/core';
import { Machine } from '../../models/machine';
import { MachineCommand } from '../../models/machine-command';
import { MachineInitCommand } from '../../models/machine-init-command';
import { CNCLibMachineService } from '../../services/CNCLib-machine.service';
import { Router, ActivatedRoute, Params, ParamMap } from '@angular/router';
import { switchMap } from 'rxjs/operators';
import { machineURL } from '../../app.global';

@Component({
  selector: 'machine-entry-load',
  templateUrl: './machine-entry-load.component.html',
  styleUrls: ['./machine-entry-load.component.css']
})
export class MachineEntryLoadComponent implements OnInit {
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

  async deleteMachine(id: number) {
    await this.machineService.deleteMachineById(id);
    this.router.navigate([machineURL]);
  }

  updateMachine(id: number) {
    //  this.router.navigate([machineURL + '/detail',this.entry.id,'edit'])
    this.router.navigate([machineURL, 'detail', String(this.entry.id), 'edit']);
  }
/*
  testUpdateMachine(id: number)
  {
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

    let id = this.route.snapshot.paramMap.get('id');
    this.entry = await this.machineService.getById(+id);
/*
    this.entry = this.route.paramMap.pipe(
      switchMap(async (params: ParamMap) =>
        await this.machineService.getById(+(params.get('id'))))
    );
*/
  }
}
