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

import { HttpClient } from '@angular/common/http';
import { Response, Headers, RequestOptions, } from '@angular/http';

import { Injectable, Inject, Pipe } from '@angular/core';
import { Machine } from '../models/machine';
import { MachineCommand } from '../models/machine-command';
import { MachineInitCommand } from '../models/machine-init-command';
import { CNCLibMachineService } from './CNCLib-machine.service';

@Injectable()
export class LocalCNCLibMachineService implements CNCLibMachineService {
  constructor(
    private http: HttpClient,
    @Inject('WEBAPI_URL') public baseUrl: string,
  ) {
  }

  getAll(): Promise<Machine[]> {
    console.log('LocalCNCLibMachineService.getAll');
    const machine = this.http
      .get<any[]>(`${this.baseUrl}api/machine`)
      .toPromise()
      .then((response) => response.map(toMachine))
      .catch(this.handleErrorPromise);

    return machine;
  }

  getById(id: number): Promise<Machine> {
    console.log('LocalCNCLibMachineService.getById');
    const m = this.http
      .get(`${this.baseUrl}api/machine/${id}`)
      .toPromise()
      .then((response) => toMachine(response))
      .catch(this.handleErrorPromise);
    return m;
  }

  getDefault(): Promise<Machine> {

    console.log('LocalCNCLibMachineService.getDefault');
    const m = this.http
      .get(`${this.baseUrl}api/machine/default`)
      .toPromise()
      .then((response) => toMachine(response))
      .catch(this.handleErrorPromise);
    return m;
  }

  addMachine(machine: Machine): Promise<Machine> {

    console.log('LocalCNCLibMachineService.addMachine');
    const m = this.http
      .post(`${this.baseUrl}api/machine`, fromMachine(machine))
      .toPromise()
      .then((response) => toMachine(response))
      .catch(this.handleErrorPromise);
    return m;
  }

  updateMachine(machine: Machine): Promise<void> {
    console.log('LocalCNCLibMachineService.updateMachine');
    const m = this.http
      .put<void>(`${this.baseUrl}api/machine/${machine.id}`, fromMachine(machine))
      .toPromise()
      .catch(this.handleErrorPromise);
    return m;
  }

  deleteMachineById(id: number): Promise<void> {
    console.log('LocalCNCLibMachineService.addMachine');
    const m = this.http
      .delete<void>(`${this.baseUrl}api/machine/${id}`)
      .toPromise()
      .catch(this.handleErrorPromise);
    return m;
    /*
    console.log('LocalCNCLibMachineService.deleteMachine');
    const m = this.http
      .delete(`${this.baseUrl}/machine/${id}`, this.getHeaders())
      .map((response: Response) => toMachine(response.json()))
      .catch(this.handleError);
    return m;
*/
  }

  private handleErrorPromise(error: Response | any) {
    console.error(error.message || error);
    return Promise.reject(error.message || error);
  }
}

function toMachine(r: any): Machine {
  const machine = <Machine>(
    {
      id: r.MachineId,
      description: r.Name,
      serialServer: r.SerialServer,
      serialServerUser: r.SerialServerUser,
      serialServerPassword: r.SerialServerPassword,
      comPort: r.ComPort,
      sizeX: r.SizeX,
      sizeY: r.SizeY,
      sizeZ: r.SizeZ,
      baudRate: r.BaudRate,
      axis: r.Axis,
      sizeA: r.SizeA,
      sizeB: r.SizeB,
      sizeC: r.SizeC,
      bufferSize: r.BufferSize,
      commandToUpper: r.CommandToUpper,
      probeSizeX: r.ProbeSizeX,
      probeSizeY: r.ProbeSizeY,
      probeSizeZ: r.ProbeSizeZ,
      probeDistUp: r.ProbeDistUp,
      probeDist: r.ProbeDist,
      probeFeed: r.ProbeFeed,
      SDSupport: r.SDSupport,
      spindle: r.Spindle,
      coolant: r.Coolant,
      laser: r.Laser,
      rotate: r.Rotate,
      workOffsets: r.WorkOffsets,
      commandSyntax: r.CommandSyntax
    });

  machine.commands = [];

  r.MachineCommands.forEach(element => {
    machine.commands.push(<MachineCommand>(
      {
        id: element.MachineCommandId,
        commandString: element.CommandString,
        commandName: element.CommandName,
        posX: element.PosX,
        posY: element.PosY,
        joystickMessage: element.JoystickMessage
      }));
  });

  machine.initCommands = [];

  r.MachineInitCommands.forEach(element => {
    machine.initCommands.push(<MachineInitCommand>(
      {
        id: element.MachineInitCommandId,
        commandString: element.CommandString,
        seqNo: element.SeqNo
      }));
  });

  console.log('Parsed toMachine:', machine);
  return machine;
}

function fromMachine(r: Machine): any {
  const machine = <any>(
    {
      MachineId: r.id,
      Name: r.description,
      ComPort: r.comPort,
      SerialServer: r.serialServer,
      SerialServerUser: r.serialServerUser,
      SerialServerPassword: r.serialServerPassword,
      SizeX: r.sizeX,
      SizeY: r.sizeY,
      SizeZ: r.sizeZ,
      BaudRate: r.baudRate,
      Axis: r.axis,
      SizeA: r.sizeA,
      SizeB: r.sizeB,
      SizeC: r.sizeC,
      BufferSize: r.bufferSize,
      CommandToUpper: r.commandToUpper,
      ProbeSizeX: r.probeSizeX,
      ProbeSizeY: r.probeSizeY,
      ProbeSizeZ: r.probeSizeZ,
      ProbeDistUp: r.probeDistUp,
      ProbeDist: r.probeDist,
      ProbeFeed: r.probeFeed,
      SDSupport: r.SDSupport,
      Spindle: r.spindle,
      Coolant: r.coolant,
      Laser: r.laser,
      Rotate: r.rotate,
      WorkOffsets: r.workOffsets,
      CommandSyntax: r.commandSyntax
    });

  machine.MachineCommands = [];

  if (r.commands != null) {
    r.commands.forEach(element => {
      machine.MachineCommands.push(<any>(
        {
          MachineId: r.id,
          MachineCommandId: element.id,
          CommandString: element.commandString,
          CommandName: element.commandName,
          PosX: element.posX,
          PosY: element.posY,
          JoystickMessage: element.joystickMessage
        }));
    });
  }

  machine.MachineInitCommands = [];

  if (r.initCommands != null) {
    r.initCommands.forEach(element => {
      machine.MachineInitCommands.push(<any>(
        {
          MachineId: r.id,
          MachineInitCommandId: element.id,
          CommandString: element.commandString,
          SeqNo: element.seqNo
        }));
    });
  }
  console.log('Parsed FromMachine:', machine);
  return machine;
}
