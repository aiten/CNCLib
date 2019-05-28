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
import { FormGroup, FormArray, FormControl, FormBuilder, Validators } from '@angular/forms';
import { Machine } from '../../models/machine';
import { MachineCommand } from '../../models/machine-command';
import { MachineInitCommand } from '../../models/machine-init-command';
import { CNCLibMachineService } from '../../services/CNCLib-machine.service';
import { machineURL } from '../machine-routing';
import { ActivatedRoute, Params } from '@angular/router';
import { Router } from '@angular/router';


@Component({
  selector: 'ha-machine-form',
  templateUrl: './machine-form.component.html',
  styleUrls: ['./machine-form.component.css']
})
export class MachineFormComponent implements OnInit {
  machine: Machine;
  errorMessage: string = '';
  isLoading: boolean = true;
  isLoaded: boolean = false;

  machineForm: FormGroup;
  commandsArray: FormArray;
  initCommandsArray: FormArray;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private machineService: CNCLibMachineService,
    private fb: FormBuilder
  ) {
    this.machine = new Machine();

    this.machineForm = fb.group(
      {
        description: ['', [Validators.required, Validators.maxLength(64)]],
        comPort: ['', [Validators.required, Validators.maxLength(32)]],
        // http://stackoverflow.com/questions/39847862/min-max-validator-in-angular-2-final
        baudRate: [115200, [Validators.required]],
        axis: [3, [Validators.required]],
        sizeX: [100, [Validators.required]],
        sizeY: [100],
        sizeZ: [100],
        sizeA: [100],
        sizeB: [100],
        sizeC: [100],

        bufferSize: [63, [Validators.required]],
        commandToUpper: [false, [Validators.required]],
        probeSizeX: [25],
        probeSizeY: [25],
        probeSizeZ: [25],
        probeDistUp: [3],
        probeDist: [10],
        probeFeed: [100],
        SDSupport: [false, [Validators.required]],
        spindle: [false, [Validators.required]],
        coolant: [false, [Validators.required]],
        laser: [false, [Validators.required]],
        rotate: [false, [Validators.required]],
        commandSyntax: [0],

        commands: fb.array(
          [
            this.createCommandsControl()
          ]),
        initCommands: fb.array(
          [
            this.createInitCommandsControl()
          ])

      });

    this.machineForm.valueChanges.subscribe((value) => {
      if (this.isLoaded)
        Object.assign(this.machine, value);
    });

    this.commandsArray = <FormArray>this.machineForm.controls['commands'];
    this.initCommandsArray = <FormArray>this.machineForm.controls['initCommands'];
  }

  createCommandsControl(): FormGroup {
    return this.fb.group({
      commandName: new FormControl('', [Validators.required, Validators.maxLength(64)]),
      commandString: new FormControl('', [Validators.required, Validators.maxLength(64)]),
      posX: new FormControl(0, [Validators.required]),
      posY: new FormControl(0, [Validators.required]),
      joystickMessage: new FormControl(''),

    });
  }

  createInitCommandsControl(): FormGroup {
    return this.fb.group({
      commandString: new FormControl('', [Validators.required, Validators.maxLength(64)]),
      seqNo: new FormControl(0, [Validators.required])
    });
  }

  async ngOnInit() {
    let id = this.route.snapshot.paramMap.get('id');
    this.machine = await this.machineService.getById(+id);
    this.loadMachine(this.machine);
  }

  addCommand() {
    this.commandsArray.push(this.createCommandsControl());
    return false;
  }

  removeCommand(i: number) {
    this.commandsArray.removeAt(i);
    return false;
  }

  private adjustCommandsArray(cmds: any[]) {
    const cmdCount = cmds ? cmds.length : 0;
    while (cmdCount > this.commandsArray.controls.length) {
      this.addCommand();
    }
    while (cmdCount < this.commandsArray.controls.length) {
      this.removeCommand(0);
    }
  }

  addInitCommand() {
    this.initCommandsArray.push(this.createInitCommandsControl());
    return false;
  }

  removeInitCommand(i: number) {
    this.initCommandsArray.removeAt(i);
    return false;
  }

  private adjustInitCommandsArray(cmds: any[]) {
    const cmdCount = cmds ? cmds.length : 0;
    while (cmdCount > this.initCommandsArray.controls.length) {
      this.addInitCommand();
    }
    while (cmdCount < this.initCommandsArray.controls.length) {
      this.removeInitCommand(0);
    }
  }

  loadMachine(m: Machine) {
    this.machine = m;
    this.adjustCommandsArray(this.machine.commands);
    this.adjustInitCommandsArray(this.machine.initCommands);

    this.machineForm.patchValue(this.machine);
    this.isLoaded = true;
  }

  async saveMachine(value: any) : Promise<void> {
    console.log(value);
    Object.assign(this.machine, value);
    await this.machineService.updateMachine(this.machine);
    console.log("saved");
    this.router.navigate([machineURL, 'detail', this.machine.id]);
  }

  userExistsValidator = (control) => {
    /*
        return this.userService.checkUserExists(control.value)
          .map(checkResult =>
          {
            return (checkResult === false) ? { userNotFound: true } : null;
          });
    */
  }

  userExistsValidatorReused = (control) => {
    /*    
        const validator = new UserExistsValidatorDirective(this.userService);
        return validator.validate(control);
    */
  };
}
