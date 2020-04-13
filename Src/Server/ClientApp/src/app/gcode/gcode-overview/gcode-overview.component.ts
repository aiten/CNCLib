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
import { LoadOptions } from "../../models/load-options";
import { CNCLibLoadOptionService } from '../../services/CNCLib-load-option.service';

import { Router } from '@angular/router';
import { MatDialog, MatDialogConfig } from "@angular/material/dialog";
import { gcodeURL } from '../../app.global';

import { MessageBoxComponent } from "../../modal/message-box/message-box.component";
import { MessageBoxData, MessageBoxResult } from "../../modal/message-box-data";


@Component(
  {
    selector: 'gcode-overview',
    templateUrl: './gcode-overview.component.html',
    styleUrls: ['./gcode-overview.component.css']
  })
export class GcodeOverviewComponent implements OnInit {
  entries: LoadOptions[] = [];
  errorMessage: string = '';
  isLoading: boolean = true;

  displayedColumns: string[] = [/* 'id', */ 'settingName', 'detail', 'run'];

  @ViewChild("fileUpload", { static: false })
  fileUpload: ElementRef;

  constructor(
    private router: Router,
    private loadOptionService: CNCLibLoadOptionService,
    private dialog: MatDialog
  ) {
  }

  TestnewLoadOption() {

    const dialogRef = this.dialog.open(MessageBoxComponent,
      {
        width: '250px',
        data: { title: "Error", message: "Not implemented yet" }
      });

    dialogRef.afterClosed().subscribe(result => {
      //      this.animal = result;
    });

  }

  newLoadOption() {
    this.router.navigate([gcodeURL, 'detail', 'new']);
  }

  detailLoadOption(id: number) {
    this.router.navigate([gcodeURL, 'detail', String(id)]);
  }

  async importgCode() {
    this.fileUpload.nativeElement.click();
  }

  uploadgCode(event) {
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
        content.settingName = content.settingName + `(import:${formattedDt})`;
        content.id = 0;
        let newentry = await this.loadOptionService.addLoadOption(content);
        await this.router.navigate([gcodeURL]);
        await this.router.navigate([gcodeURL, 'detail', String(newentry.id)]);
      }
      fileReader.onerror = (error) => {
        console.log(error);
      }
    }
  }

  runLoadOption(id: number) {
    this.router.navigate([gcodeURL, 'run', String(id)]);
  }

  async ngOnInit() {
    this.entries = await this.loadOptionService.getAll();
  }
}
