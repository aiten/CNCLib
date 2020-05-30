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


@Injectable({ providedIn: 'root' })
export class AuthenticationService {
  constructor(private http: HttpClient,
    @Inject('WEBAPI_URL') public baseUrl: string,
  ) {
  }

  login(username: string, password: string): Promise<void> {

    const params = new HttpParams()
      .set('userName', username)
      .set('password', password);

    const authentication = this.http
      .get(`${this.baseUrl}api/user/isvaliduser`, { params })
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

  private handleErrorPromise(error: Response | any) {
    console.error(error.message || error);
    return Promise.reject(error.message || error);
  }

  logout() {
    // remove user from local storage to log user out
    localStorage.removeItem('CNCLib.currentUser');
  }
}
