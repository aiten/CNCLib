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

import { Injectable } from '@angular/core';

import { SerialPortDefinition } from '../models/serial.port.definition';
import { SerialServerService } from '../services/serial-server.service';
import { JoystickServerService } from "../services/joystick-server.service";

import { CNCLibJoystickService } from "../services/CNCLib-joystick.service";

import { HubConnection } from '@aspnet/signalr';
import { HubConnectionBuilder } from '@aspnet/signalr';

import { SerialServerConnection } from '../serial-server/serial-server-connection';
import { JoystickServerConnection } from '../serial-server/joystick-server-connection';

import { MachineControlGlobal } from './machine-control.global';

import { Joystick } from "../models/joystick";


@Injectable()
export class MachineControlState {

  serialport!: SerialPortDefinition;
  isConnected: boolean = false;

  private hubConnection: HubConnection;
  private joystickPortId: number;

  isJoystickConnect: boolean = false;
  isJoystickLoaded: boolean = false;
  joystick: Joystick;

  joystickServerName: string = 'https://localhost:5000';
  joystickPort: string = 'COM6';
  joystickUser: string = 'admin';
  joystickPassword: string = 'Serial.Server';

  constructor(
    private serialServerService: SerialServerService,
    private joystickServerService: JoystickServerService,
    private joystickService: CNCLibJoystickService,
    public serialServer: SerialServerConnection,
    public joystickServer: JoystickServerConnection,
    public machineControlGlobal: MachineControlGlobal
  ) {
  }

  async getJoystick() {
    if (!this.isJoystickLoaded) {
      this.joystick = (await this.joystickService.getAll())[0];
      this.joystickServerName = this.joystick.serialServer;
      this.joystickPort = this.joystick.comPort;
      this.joystickUser = this.joystick.serialServerUser;
      this.joystickPassword = this.joystick.serialServerPassword;
      this.isJoystickLoaded = true;
    }
  }

  async joystickSave() {
    if (this.isJoystickLoaded) {
      console.log(this.joystick);
      this.joystick.serialServer = this.joystickServerName;
      this.joystick.comPort = this.joystickPort;
      this.joystick.serialServerUser = this.joystickUser;
      this.joystick.serialServerPassword = this.joystickPassword;

      await this.joystickService.update(this.joystick);

      this.isJoystickLoaded = false;
      await this.getJoystick();
    }
  }

  async load(): Promise<void> {

    if (this.serialServer.getMachine() != null) {
      this.serialServerService.setBaseUrl(this.serialServer.getSerialServerUrl(), this.serialServer.getSerialServerAuth());
      var id = this.serialServer.getSerialServerPortId();

      this.serialport = await this.serialServerService.getPort(id);
      this.isConnected = this.serialport.isConnected !== 0;
    }
  }

  async loadJoystick(forceConnect: boolean): Promise<boolean> {

    await this.getJoystick();

    if (this.isConnected && (forceConnect || this.isJoystickConnect)) {

      await this.joystickServer.getInfoX(this.joystickServerName, this.joystickUser, this.joystickPassword, this.joystickPort);

      this.joystickServerService.setBaseUrl(this.joystickServer.getSerialServerUrl(), this.joystickServer.getSerialServerAuth());
      this.joystickPortId = this.joystickServer.getSerialServerPortId();

      console.log('JoystickSignalR to ' + this.joystickServer.getSerialServerUrl() + 'serialSignalR');

      this.hubConnection = new HubConnectionBuilder().withUrl(this.joystickServer.getSerialServerUrl() + 'serialSignalR/').build();

      this.hubConnection.on('Received',
        (portid: number, info: string) => {
          if (portid == this.joystickPortId) {
            console.log("Joystick received:" + info);
            if (!info.startsWith(";")) {
              this.postcommand(info);
            }
          }
        });
      this.hubConnection.on('HeartBeat',
        () => {
          console.log('JoystickSignalR received: HeartBeat');
        });

      console.log("hub Starting:");

      await this.hubConnection.start()
        .then(() => {
          console.log('Joystick Hub connection started');
        })
        .catch(err => {
          console.log('Joystick Error while establishing connection');
        });

      return true;
    }
  }

  async close(): Promise<void> {
    if (this.hubConnection != null) {
      await this.hubConnection.stop();
    }
  }

  async joystickConnect(): Promise<void> {
    if (!this.isJoystickConnect) {
      this.isJoystickConnect = await this.loadJoystick(true);
    }
  }

  async joystickDisconnect():
    Promise<void> {
    this.isJoystickConnect = false;
    await this.close();
  }

  async postcommand(command:
      string):
    Promise<void> {
    await this.serialServerService.queueCommands(this.serialServer.getSerialServerPortId(), [command], 1000);
  }

  async sendWhileOkcommands(commands:
      string[]):
    Promise<void> {
    await this.serialServerService.sendWhileOkCommands(this.serialServer.getSerialServerPortId(), commands, 10000);
  }

  toAxisName(axis: number): string {
    var axisNames = 'XYZABC';
    return axisNames.charAt(axis);
  }

  async setOfs(axis: number): Promise<void> {
    await this.postcommand("g92 " + this.toAxisName(axis) + this.machineControlGlobal.offsetG92[axis]);
  }

  async clearOfs(): Promise<void> {
    await this.postcommand("g92");
  }

  async probe(axis: number): Promise<void> {
    await this.sendWhileOkcommands([
      "g91 g31z-" + this.serialServer.getMachine().probeDist + " f" + this.serialServer.getMachine().probeFeed + " g90",
      "g92 z-" + this.serialServer.getMachine().probeSizeZ, " g91 g0z" + this.serialServer.getMachine().probeDistUp + " g90"
    ]);
  }

  async getPosition(): Promise<void> {
    var positions = await this.serialServerService.getPosition(this.serialServer.getSerialServerPortId());
    this.machineControlGlobal.position = positions[0];
    this.machineControlGlobal.positionAbs = positions[1];
  }

  async refMove(axis: number): Promise<void> {
    await this.serialServerService.queueCommands(this.serialServer.getSerialServerPortId(), ["g28 " + this.toAxisName(axis) + "0"], 1000);
  }

  addXYZ(offsetId: number): string {

    var cmd = "";

    if (this.machineControlGlobal.workOfsX[offsetId].toString().length > 0) {
      cmd = cmd + " x" + this.machineControlGlobal.workOfsX[offsetId];
    }

    if (this.machineControlGlobal.workOfsY[offsetId].toString().length > 0) {
      cmd = cmd + " y" + this.machineControlGlobal.workOfsY[offsetId];
    }

    if (this.machineControlGlobal.workOfsZ[offsetId].toString().length > 0) {
      cmd = cmd + " z" + this.machineControlGlobal.workOfsZ[offsetId];
    }

    return cmd;
  }

  async selectWOfs(offsetId: number): Promise<void> {
    await this.postcommand("g" + (54 + offsetId).toString())
  }

  async getWOfs(offsetId: number): Promise<void> {
    this.machineControlGlobal.workOfsX[offsetId] = await this.serialServerService.getParameter(this.serialServer.getSerialServerPortId(), 5221 + offsetId * 20);
    this.machineControlGlobal.workOfsY[offsetId] = await this.serialServerService.getParameter(this.serialServer.getSerialServerPortId(), 5222 + offsetId * 20);
    this.machineControlGlobal.workOfsZ[offsetId] = await this.serialServerService.getParameter(this.serialServer.getSerialServerPortId(), 5223 + offsetId * 20);
  }

  async setWOfs(offsetId: number): Promise<void> {
    var cmd = "g10 l2 p" + (offsetId + 1).toString() + this.addXYZ(offsetId);
    await this.postcommand(cmd);
  }

  async relWOfs(offsetId: number): Promise<void> {
    var cmd = "g10 l2 g91 p" + (offsetId + 1).toString() + " x0y0";
    await this.postcommand(cmd);
  }
}
