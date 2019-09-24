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
import { LoadOptions } from "../../models/load-options";
import { CNCLibLoadOptionService } from '../../services/CNCLib-load-option.service';
import { Router } from '@angular/router';
import { MatDialog, MatDialogConfig } from "@angular/material/dialog";

import { ModalComponent } from "../../modal/modal.component";
import { DialogData } from "../../modal/DialogData";

import { MessageBoxComponent } from "../../modal/message-box/message-box.component";
import { MessageBoxData } from "../../modal/message-box-data";


@Component(
  {
    selector: 'ha-gcode-overview',
    templateUrl: './gcode-overview.component.html',
    styleUrls: ['./gcode-overview.component.css']
  })
export class GcodeOverviewComponent implements OnInit {
  entries: LoadOptions[] = [];
  errorMessage: string = '';
  isLoading: boolean = true;

  animal: string;
  name: string;

  constructor(
    private router: Router,
    private loadOptionService: CNCLibLoadOptionService,
    private dialog: MatDialog
  ) {
  }

  newLoadOption() {

    const dialogRef = this.dialog.open(MessageBoxComponent,
      {
        width: '250px',
        data: { title: "Error", message: "Not implemented yet" }
      });

    dialogRef.afterClosed().subscribe(result => {
      //      this.animal = result;
    });

  }

  detailLoadOption(id: number) {

    const dialogRef = this.dialog.open(MessageBoxComponent,
      {
        width: '250px',
        data: { title: "Info", message: "Hallo" }
      });

    dialogRef.afterClosed().subscribe(result => {
//      this.animal = result;
    });

/*
    const dialogRef = this.dialog.open(ModalComponent,
      {
        width: '250px',
        data: { name: this.name, animal: this.animal }
      });

    dialogRef.afterClosed().subscribe(result => {
      this.animal = result;
    });
*/
  }


  async ngOnInit() {
    this.entries = await this.loadOptionService.getAll();
  }
}