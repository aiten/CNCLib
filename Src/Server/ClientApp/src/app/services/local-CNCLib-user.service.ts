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

import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { User } from "../models/user";
import { CNCLibUserService } from "./CNCLib-user.service";


@Injectable({ providedIn: 'root' })
export class LocalCNCLibUserService implements CNCLibUserService {
  constructor(private http: HttpClient,
    @Inject('WEBAPI_URL') public baseUrl: string,
  ) {
  }

  register(username: string, password: string): Promise<void> {
    const params = new HttpParams()
      .set('userName', username)
      .set('password', password);

    const authentication = this.http
      .post(`${this.baseUrl}api/user/register`, null, { params })
      .toPromise()
      .then((response: Response) => {
        var user = new User();
        user.id = 1;
        user.username = username;
        user.password = password;
        user.authData = window.btoa(username + ':' + password);
        localStorage.setItem('CNCLib.currentUser', JSON.stringify(user));
      })
      .catch(this.handleErrorPromise);

    return authentication;
  }

  initialize(): Promise<void> {
    return this.http
      .put<void>(`${this.baseUrl}api/user/init`, null)
      .toPromise()
      .catch(this.handleErrorPromise);
  }

  initMachines(): Promise<void> {
    return this.http
      .put<void>(`${this.baseUrl}api/user/initMachines`, null)
      .toPromise()
      .catch(this.handleErrorPromise);
  }

  initGCode(): Promise<void> {
    return this.http
      .put<void>(`${this.baseUrl}api/user/initItems`, null)
      .toPromise()
      .catch(this.handleErrorPromise);
  }

  cleanup(): Promise<void> {
    return this.http
      .delete<void>(`${this.baseUrl}api/user/cleanup`)
      .toPromise()
      .catch(this.handleErrorPromise);
  }

  leave(): Promise<void> {
    return this.http
      .delete<void>(`${this.baseUrl}api/user/leave`)
      .toPromise()
      .catch(this.handleErrorPromise);
  }

  private handleErrorPromise(error: Response | any) {
    console.error(error.message || error);
    return Promise.reject(error.message || error);
  }
}
