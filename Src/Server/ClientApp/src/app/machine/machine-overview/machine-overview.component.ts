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

import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { Machine } from '../../models/machine';
import { CNCLibMachineService } from '../../services/CNCLib-machine.service';

import { SerialServerConnection } from '../../serial-server/serial-server-connection';

import { machineURL, machineControlURL } from '../../app.global';
import { Router } from '@angular/router';


@Component(
  {
    selector: 'machine-overview',
    templateUrl: './machine-overview.component.html',
    styleUrls: ['./machine-overview.component.css'],
  })
export class MachineOverviewComponent implements OnInit {
  entries: Machine[] = [];
  errorMessage: string = '';
  isLoading: boolean = true;

  displayedColumns: string[] = [/* 'Id', */ 'Description', 'SerialServer', 'ComPort', 'SizeX', 'SizeY', 'SizeZ', 'Detail', 'Connect'];

  @ViewChild("fileUpload", { static: false })
  fileUpload: ElementRef;

  constructor(
    private router: Router,
    private machineService: CNCLibMachineService,
    public serialServer: SerialServerConnection
  ) {
  }

  detailMachine(id: number) {
    this.router.navigate([machineURL, String(id)]);
  }

  async connectToMachine(machine: Machine) {

    await this.serialServer.connectTo(machine);
    this.router.navigate([machineControlURL]);
  }

  async disconnectFromMachine(machine: Machine) {

    await this.serialServer.disconnectFrom();
  }

  isConnected(machine: Machine): boolean {
    return this.serialServer.isConnected(machine);
  }

  canConnectTo(machine: Machine): boolean {
    return machine.serialServer?.length > 0;
  }

  async newMachine() {

    var newmachineDefault = await this.machineService.getDefault();
    var newmachine = await this.machineService.add(newmachineDefault);
    this.router.navigate([machineURL, String(newmachine.id)]);
  }

  async importMachine() {
    this.fileUpload.nativeElement.click();
  }

  uploadMachine(event) {
    let files = event.target.files;
    if (files.length > 0) {
      console.log("Load");
      const selectedFile = event.target.files[0];
      const fileReader = new FileReader();
      fileReader.readAsText(selectedFile, "UTF-8");
      fileReader.onload = async () => {
        const content = JSON.parse(fileReader.result as string);
        console.log(content);
        let formattedDt = new Date().toLocaleString();
        content.description = content.description + `(import:${formattedDt})`;
        content.id = 0;
        content.commands.forEach(function(value) {
          value.id = 0;
        });
        content.initCommands.forEach(function(value) {
          value.id = 0;
        });

        let newentry = await this.machineService.add(content);
        await this.router.navigate([machineURL]);
        await this.router.navigate([machineURL, String(newentry.id)]);
      }
      fileReader.onerror = (error) => {
        console.log(error);
      }
    }
  }

  async ngOnInit() {
    this.entries = (await this.machineService.getAll()).sort((a, b) => a.description >= b.description ? 1 : -1);
  }
}
