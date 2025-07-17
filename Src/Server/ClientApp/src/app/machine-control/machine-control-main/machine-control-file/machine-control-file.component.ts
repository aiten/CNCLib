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

import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MachineControlState } from "../../machine-control-state";
import { UserFile } from "../../../models/userFile";
import { CNCLibUserFileService } from '../../../services/CNCLib-userFile.service';

@Component({
  selector: 'machinecontrolfile',
  templateUrl: './machine-control-file.component.html',
  styleUrls: ['./machine-control-file.component.css'],
  imports: [CommonModule]
})
export class MachineControlFileComponent {

  constructor(
    public userFileService: CNCLibUserFileService,
  ) {
  }

  @Input()
  machineControlState: MachineControlState;

  async uploadFile(event) {
    let files = event.target.files;
    if (files.length > 0) {
      let tmpFileName = "$$$";
      const file = event.target.files[0];
      let userFile = new UserFile();
      userFile.image = file;
      userFile.fileName = tmpFileName;
      await this.userFileService.update(tmpFileName, userFile);
      this.machineControlState.machineControlGlobal.fileName = 'db:' + tmpFileName;
    }
  }
  async sendFile() {
    const action = await this.userFileService.get(this.machineControlState.machineControlGlobal.fileName);
    console.log(action);
    const commands = await action.text();
    //console.log(commands);

    await this.machineControlState.postFile(commands);
  }
}
