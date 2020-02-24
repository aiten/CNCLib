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

import { Injectable, Inject, Pipe } from '@angular/core';

import { LoadOptions } from "../models/load-options";
import { PreviewGCode } from '../models/gcode-view-input';

import { CNCLibGCodeService } from './CNCLib-gcode.service';

@Injectable()
export class LocalCNCLibGCodeService implements CNCLibGCodeService {
  constructor(
    private http: HttpClient,
    @Inject('WEBAPI_URL') public baseUrl: string,
  ) {
  }

  createGCode(loadOption: LoadOptions): Promise<string[]> {
    console.log('LocalCNCLibGCodeService.createGCode');
    const m = this.http
      .post<string[]>(`${this.baseUrl}api/GCode`, loadOption)
      .toPromise()
      .catch(this.handleErrorPromise);
    return m;
  }

  getGCodeAsImage(viewInput: PreviewGCode): Promise<Blob> {
    console.log('LocalCNCLibGCodeService.getGCodeAsImage');
    const m = this.http
      .put<Blob>(`${this.baseUrl}api/GCode/render`, viewInput, { responseType: 'blob' as 'json' })
      .toPromise()
      .catch(this.handleErrorPromise);
    return m;
  }

  private handleErrorPromise(error: Response | any) {
    console.error(error.message || error);
    return Promise.reject(error.message || error);
  }
}
