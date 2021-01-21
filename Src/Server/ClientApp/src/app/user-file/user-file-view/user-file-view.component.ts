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

import { Component, OnInit, AfterViewInit, ViewChild, ElementRef } from '@angular/core';

import { switchMap } from 'rxjs/operators';

import { Router, ActivatedRoute } from '@angular/router';
import { UserFile } from '../../models/userFile';
import { UserFileInfo } from '../../models/userFileInfo';

import { MessageBoxComponent } from "../../modal/message-box/message-box.component";
import { MessageBoxResult } from "../../modal/message-box-data";

import { CNCLibUserFileService } from '../../services/CNCLib-userFile.service';

import { MatDialog } from "@angular/material/dialog";

import { UserFileGlobal } from '../user-file.global';
import { ConsoleReporter } from 'jasmine';

@Component(
  {
    selector: 'user-file-detail',
    templateUrl: './user-file-view.component.html',
    styleUrls: ['./user-file-view.component.css']
  })
export class UserFileViewComponent {

  entries: UserFileInfo[] = [];
  errorMessage: string = '';
  isLoading: boolean = true;

  displayedColumns: string[] = [/* 'Id', */ 'fileName', 'fileSize', 'uploadTime', 'update','delete', 'download'];

  @ViewChild("fileUpload", { static: false })
  fileUpload: ElementRef;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private dialog: MatDialog,
    private userFileService: CNCLibUserFileService,
  ) {
  }

  async deleteUserFile(userFile: UserFileInfo) {

    const dialogRef = this.dialog.open(MessageBoxComponent,
      {
        width: '250px',
        data: { title: "Warning", message: `Delete file ${userFile.fileName}`, haveYes: true, haveCancel: true }
      });

    dialogRef.afterClosed().subscribe(async result => {
      if (result.result == MessageBoxResult.Yes) {
        await this.userFileService.delete(userFile.fileName);
        await this.reload();
      }
    });
  }

  async updateUserFile(event,userFileInfo: UserFileInfo) {
    let files = event.target.files;
    if (files.length > 0) {
      let tmpFileName = userFileInfo.fileName;
      const file = event.target.files[0];
      let userFile = new UserFile();
      userFile.image = file;
      userFile.fileName = tmpFileName;
      await this.userFileService.update(tmpFileName, userFile);
      await this.reload();
    }
  }

  async uploadNewFile(event) {
    let files = event.target.files;
    if (files.length > 0) {
      const file = event.target.files[0];
      let userFile = new UserFile();

      userFile.image = file;
      userFile.fileName = file.name;
      await this.userFileService.add(userFile);
      await this.reload();
    }
  }

  public async downloadUserFile(userFileInfo: UserFileInfo) {
    const action = await this.userFileService.get(userFileInfo.fileName);
    saveAs(action, userFileInfo.fileName);
  }

  async reload() {
    this.entries = (await this.userFileService.getAll("")).sort((a, b) => a.fileName >= b.fileName ? 1 : -1);
  }

  async ngOnInit() {
    await this.reload();
  }
}
