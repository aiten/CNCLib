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

import { Component, Inject } from '@angular/core';
import { MatLegacyDialog as MatDialog } from "@angular/material/legacy-dialog";
import { Router } from '@angular/router';

import { CNCLibLoggedinService } from '../../services/CNCLib-loggedin.service';
import { CNCLibUserService } from "../../services/CNCLib-user.service";

import { MessageBoxComponent } from "../../modal/message-box/message-box.component";
import { MessageBoxResult } from "../../modal/message-box-data";

import { homeURL } from "../../app.global";

@Component({
  selector: 'useradmin',
  templateUrl: './user-admin.component.html'
})
export class UserAdminComponent {

  isInRegister = false;
  isInChangePassword = false;
  isMore = false;

  constructor(
    public cncLibloggedinService: CNCLibLoggedinService,
    private dialog: MatDialog,
    private router: Router,
    private userService: CNCLibUserService,
    @Inject('WEBAPI_URL') public baseUrl: string
  ) {
  }

  async changePassword() {
    this.isInChangePassword = true;
  }

  async initialize() {
    const dialogRef = this.dialog.open(MessageBoxComponent,
      {
        width: '250px',
        data: { title: "Warning", message: "Revert your environment to the default. This will delete all machines, GCode definitions and files.", haveYes: true, haveCancel: true }
      });

    dialogRef.afterClosed().subscribe(async result => {
      if (result.result == MessageBoxResult.Yes) {
        await this.userService.initialize();
      }
    });
  }

  async initMachines() {
    const dialogRef = this.dialog.open(MessageBoxComponent,
      {
        width: '250px',
        data: { title: "Warning", message: "Revert the machines to the default. Only predefined machines are overwritten. Custom machinies are preserved.", haveYes: true, haveCancel: true }
      });

    dialogRef.afterClosed().subscribe(async result => {
      if (result.result == MessageBoxResult.Yes) {
        await this.userService.initMachines();
      }
    });
  }

  async initGCodes() {
    const dialogRef = this.dialog.open(MessageBoxComponent,
      {
        width: '250px',
        data: { title: "Warning", message: "Revert the GCode-Generation to the default. Only predefined are overwritten. Custom GCode-generation-configurations are preserved.", haveYes: true, haveCancel: true }
      });

    dialogRef.afterClosed().subscribe(async result => {
      if (result.result == MessageBoxResult.Yes) {
        await this.userService.initGCode();
      }
    });
  }

  async cleanup() {
    const dialogRef = this.dialog.open(MessageBoxComponent,
      {
        width: '250px',
        data: { title: "Warning", message: "Do you want to cleanup your environment. This will delete all machines, GCode definitions and files.", haveYes: true, haveCancel: true }
      });

    dialogRef.afterClosed().subscribe(async result => {
      if (result.result == MessageBoxResult.Yes) {
        this.userService.cleanup();
      }
    });
  }

  async leave() {
    const dialogRef = this.dialog.open(MessageBoxComponent,
      {
        width: '250px',
        data: { title: "Warning", message: "Do you want to leave. This will delete all machines, GCode definitions, files and login.", haveYes: true, haveCancel: true }
      });

    dialogRef.afterClosed().subscribe(async result => {
      if (result.result == MessageBoxResult.Yes) {
        await this.userService.leave();
        this.cncLibloggedinService.logout();
        await this.router.navigate([homeURL]);
      }
    });
  }
}
