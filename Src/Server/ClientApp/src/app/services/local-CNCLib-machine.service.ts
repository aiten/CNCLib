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

import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';

import { Injectable, Inject } from '@angular/core';
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
    const machine = this.http
      .get<any[]>(`${this.baseUrl}api/machine`)
      .toPromise()
      .then((response) => response.map(toMachine))
      .catch(this.handleErrorPromise);

    return machine;
  }

  getById(id: number): Promise<Machine> {
    const m = this.http
      .get(`${this.baseUrl}api/machine/${id}`)
      .toPromise()
      .then((response) => toMachine(response))
      .catch(this.handleErrorPromise);
    return m;
  }

  getDefault(): Promise<Machine> {
    const m = this.http
      .get(`${this.baseUrl}api/machine/default`)
      .toPromise()
      .then((response) => toMachine(response))
      .catch(this.handleErrorPromise);
    return m;
  }

  add(machine: Machine): Promise<Machine> {

    const m = this.http
      .post(`${this.baseUrl}api/machine`, fromMachine(machine))
      .toPromise()
      .then((response) => toMachine(response))
      .catch(this.handleErrorPromise);
    return m;
  }

  update(machine: Machine): Promise<void> {
    const m = this.http
      .put<void>(`${this.baseUrl}api/machine/${machine.id}`, fromMachine(machine))
      .toPromise()
      .catch(this.handleErrorPromise);
    return m;
  }

  deleteById(id: number): Promise<void> {
    const m = this.http
      .delete<void>(`${this.baseUrl}api/machine/${id}`)
      .toPromise()
      .catch(this.handleErrorPromise);
    return m;
  }

  fromEeprom(machine: Machine, eepromValues: number[]): Promise<Machine> {

    let el: { item1: Machine, item2: number[] } = { item1: fromMachine(machine), item2: eepromValues };
    const m = this.http
      .put(`${this.baseUrl}api/machine/fromEeprom`, el)
      .toPromise()
      .then((response) => toMachine(response))
      .catch(this.handleErrorPromise);
    return m;
  }

  joystickMessage(id: number, message: string): Promise<string> {
    let params = new HttpParams();
    params = params.set('joystickMessage', message);
    const headers = new HttpHeaders().set('Content-Type', 'text/plain; charset=utf-8');

    const m = this.http
      .get(`${this.baseUrl}api/machine/${id}/joystick?${params.toString()}`, { headers, responseType: 'text' })
      .toPromise()
      .catch(this.handleErrorPromise);
    return m;
  }

  private handleErrorPromise(error: Response | any) {
    console.error(error.message || error);
    return Promise.reject(error.message || error);
  }
}

function toMachine(r: any): Machine {
  const machine = <Machine>(
    {
      id: r.machineId,
      description: r.name,
      serialServer: r.serialServer,
      serialServerUser: r.serialServerUser,
      serialServerPassword: r.serialServerPassword,
      comPort: r.comPort,
      sizeX: r.sizeX,
      sizeY: r.sizeY,
      sizeZ: r.sizeZ,
      baudRate: r.baudRate,
      dtrIsReset: r.dtrIsReset,
      axis: r.axis,
      sizeA: r.sizeA,
      sizeB: r.sizeB,
      sizeC: r.sizeC,
      bufferSize: r.bufferSize,
      commandToUpper: r.commandToUpper,
      probeSizeX: r.probeSizeX,
      probeSizeY: r.probeSizeY,
      probeSizeZ: r.probeSizeZ,
      probeDistUp: r.probeDistUp,
      probeDist: r.probeDist,
      probeFeed: r.probeFeed,
      sdSupport: r.sdSupport,
      spindle: r.spindle,
      coolant: r.coolant,
      laser: r.laser,
      rotate: r.rotate,
      workOffsets: r.workOffsets,
      commandSyntax: r.commandSyntax
    });

  machine.commands = [];

  r.machineCommands.forEach(element => {
    machine.commands.push(<MachineCommand>(
      {
        id: element.machineCommandId,
        commandString: element.commandString,
        commandName: element.commandName,
        posX: element.posX,
        posY: element.posY,
        joystickMessage: element.joystickMessage
      }));
  });

  machine.initCommands = [];

  r.machineInitCommands.forEach(element => {
    machine.initCommands.push(<MachineInitCommand>(
      {
        id: element.machineInitCommandId,
        commandString: element.commandString,
        seqNo: element.seqNo
      }));
  });

  return machine;
}

function fromMachine(r: Machine): any {
  const machine = <any>(
    {
      machineId: r.id,
      name: r.description,
      comPort: r.comPort,
      serialServer: r.serialServer,
      serialServerUser: r.serialServerUser,
      serialServerPassword: r.serialServerPassword,
      sizeX: r.sizeX,
      sizeY: r.sizeY,
      sizeZ: r.sizeZ,
      baudRate: r.baudRate,
      dtrIsReset: r.dtrIsReset,
      axis: r.axis,
      sizeA: r.sizeA,
      sizeB: r.sizeB,
      sizeC: r.sizeC,
      bufferSize: r.bufferSize,
      commandToUpper: r.commandToUpper,
      probeSizeX: r.probeSizeX,
      probeSizeY: r.probeSizeY,
      probeSizeZ: r.probeSizeZ,
      probeDistUp: r.probeDistUp,
      probeDist: r.probeDist,
      probeFeed: r.probeFeed,
      sdSupport: r.sdSupport,
      spindle: r.spindle,
      coolant: r.coolant,
      laser: r.laser,
      rotate: r.rotate,
      workOffsets: r.workOffsets,
      commandSyntax: r.commandSyntax
    });

  machine.machineCommands = [];

  if (r.commands != null) {
    r.commands.forEach(element => {
      machine.machineCommands.push(<any>(
        {
          machineId: r.id,
          machineCommandId: element.id,
          commandString: element.commandString,
          commandName: element.commandName,
          posX: element.posX,
          posY: element.posY,
          joystickMessage: element.joystickMessage
        }));
    });
  }

  machine.machineInitCommands = [];

  if (r.initCommands != null) {
    r.initCommands.forEach(element => {
      machine.machineInitCommands.push(<any>(
        {
          machineId: r.id,
          machineInitCommandId: element.id,
          commandString: element.commandString,
          seqNo: element.seqNo
        }));
    });
  }
  return machine;
}
