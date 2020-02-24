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
import { CNCLibLoadOptionService } from './CNCLib-load-option.service';

@Injectable()
export class LocalCNCLibLoadOptionService implements CNCLibLoadOptionService {
  constructor(
    private http: HttpClient,
    @Inject('WEBAPI_URL') public baseUrl: string,
  ) {
  }

  getAll(): Promise<LoadOptions[]> {
    console.log('LocalCNCLibLoadOptionService.getAll');
    const loadOptions = this.http
      .get<LoadOptions[]>(`${this.baseUrl}api/LoadOptions`)
      .toPromise()
      .catch(this.handleErrorPromise);

    return loadOptions;
  }

  getById(id: number): Promise<LoadOptions> {
    console.log('LocalCNCLibLoadOptionService.getById');
    const loadOption = this.http
      .get<LoadOptions>(`${this.baseUrl}api/LoadOptions/${id}`)
      .toPromise()
      .catch(this.handleErrorPromise);
    return loadOption;
  }

  addLoadOption(loadOption: LoadOptions): Promise<LoadOptions> {

    console.log('LocalCNCLibLoadOptionService.addLoadOptions');
    const m = this.http
      .post<LoadOptions>(`${this.baseUrl}api/LoadOptions`, loadOption)
      .toPromise()
      .catch(this.handleErrorPromise);
    return m;
  }

  updateLoadOption(loadOption: LoadOptions): Promise<void> {
    console.log('LocalCNCLibLoadOptionService.updateLoadOptions');
    const m = this.http
      .put<void>(`${this.baseUrl}api/LoadOptions/${loadOption.Id}`, loadOption)
      .toPromise()
      .catch(this.handleErrorPromise);
    return m;
  }

  deleteLoadOptionById(id: number): Promise<void> {
    console.log('LocalCNCLibLoadOptionService.addLoadOptions');
    const m = this.http
      .delete<void>(`${this.baseUrl}api/LoadOptions/${id}`)
      .toPromise()
      .catch(this.handleErrorPromise);
    return m;
    /*
    console.log('LocalCNCLibLoadOptionService.deleteLoadOptions');
    const m = this.http
      .delete(`${this.baseUrl}/loadOption/${id}`, this.getHeaders())
      .map((response: Response) => toLoadOptions(response.json()))
      .catch(this.handleError);
    return m;
*/
  }

  private handleErrorPromise(error: Response | any) {
    console.error(error.message || error);
    return Promise.reject(error.message || error);
  }
}
