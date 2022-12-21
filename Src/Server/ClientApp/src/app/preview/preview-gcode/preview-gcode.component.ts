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

import { Component, OnInit, ViewChild } from '@angular/core';
import { MatLegacyPaginator as MatPaginator } from '@angular/material/legacy-paginator';
import { MatLegacyTableDataSource as MatTableDataSource } from '@angular/material/legacy-table';

import { PreviewGlobal } from '../preview.global';


@Component(
  {
    selector: 'preview-gcode',
    templateUrl: './preview-gcode.component.html',
    styleUrls: ['./preview-gcode.component.css']
  })
export class PreviewGCodeComponent implements OnInit {

  gCommands: string[];

  gCommandsDataSource = new MatTableDataSource<string>([]);

  displayedColumns: string[] = ['Cmd'];

  @ViewChild(MatPaginator, { static: true })
  paginator: MatPaginator;

  constructor(
    private previewGlobal: PreviewGlobal
  ) {

  }

  async ngOnInit() {
    this.gCommands = this.previewGlobal.commands;
    this.gCommandsDataSource = new MatTableDataSource<string>(this.gCommands);
    this.gCommandsDataSource.paginator = this.paginator;
  }
}
