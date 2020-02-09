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

import { Injectable } from '@angular/core';
import { HttpRequest, HttpResponse, HttpHandler, HttpEvent, HttpInterceptor, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { delay, mergeMap, materialize, dematerialize } from 'rxjs/operators';

@Injectable()
export class FakeBackendInterceptor implements HttpInterceptor {

  constructor() {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    let testUser = { id: 1, username: 'test', password: 'test', firstName: 'Test', lastName: 'User' };

    // wrap in delayed observable to simulate server api call
    return of(null).pipe(mergeMap(() => {

        // authenticate
        if (request.url.endsWith('/users/authenticate') && request.method === 'POST') {
          if (request.body.username === testUser.username && request.body.password === testUser.password) {
            // if login details are valid return user details
            let body = {
              id: testUser.id,
              username: testUser.username,
              firstName: testUser.firstName,
              lastName: testUser.lastName
            };
            return of(new HttpResponse({ status: 200, body }));
          } else {
            // else return 400 bad request
            return throwError({ error: { message: 'Username or password is incorrect' } });
          }
        }

        // get users
        if (request.url.endsWith('/users') && request.method === 'GET') {
          // check for fake auth token in header and return users if valid, this security 
          // is implemented server side in a real application
          if (request.headers.get('Authorization') === `Basic ${window.btoa('test:test')}`) {
            return of(new HttpResponse({ status: 200, body: [testUser] }));
          } else {
            // return 401 not authorised if token is null or invalid
            return throwError({ status: 401, error: { message: 'Unauthorised' } });
          }
        }

        // pass through any requests not handled above
        return next.handle(request);

      }))

      // call materialize and dematerialize to ensure delay even if an error is thrown (https://github.com/Reactive-Extensions/RxJS/issues/648)
      .pipe(materialize())
      .pipe(delay(500))
      .pipe(dematerialize());
  }
}

export let fakeBackendProvider = {
  // use fake backend in place of Http service for backend-less development
  provide: HTTP_INTERCEPTORS,
  useClass: FakeBackendInterceptor,
  multi: true
};
