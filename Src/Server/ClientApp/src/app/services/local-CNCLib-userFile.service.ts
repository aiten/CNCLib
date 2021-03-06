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
import { UserFile } from "../models/userFile";
import { CNCLibUserFileService } from './CNCLib-userFile.service';
import { UserFileInfo } from '../models/userFileInfo';

@Injectable()
export class LocalCNCLibUserFileService implements CNCLibUserFileService {
  constructor(
    private http: HttpClient,
    @Inject('WEBAPI_URL') public baseUrl: string,
  ) {
  }

  getAll(wildcard: string): Promise<UserFileInfo[]> {
    const userFile = this.http
      .get<UserFileInfo[]>(`${this.baseUrl}api/userfile`)
      .toPromise()
      .catch(this.handleErrorPromise);

    return userFile;
  }

  add(userFile: UserFile): Promise<UserFile> {
    const formData = new FormData();

    formData.append('image', userFile.image);
    formData.append('fileName', userFile.fileName);

    return this.http.post<UserFile>(`${this.baseUrl}api/userFile`, formData)
      .toPromise()
      .catch(this.handleErrorPromise);
  }

  update(fileName: string, userFile: UserFile): Promise<any> {

    const params = new HttpParams()
      .set('fileName', fileName);

    const formData = new FormData();

    formData.append('image', userFile.image);
    formData.append('fileName', userFile.fileName);

    return this.http.put<any>(`${this.baseUrl}api/userFile`, formData, { params }).toPromise()
      .catch(this.handleErrorPromise);
  }

  delete(fileName: string): Promise<any> {

    const params = new HttpParams()
      .set('fileName', fileName);

    return this.http.delete<any>(`${this.baseUrl}api/userFile`, { params: params }).toPromise()
      .catch(this.handleErrorPromise);
  }
  
  get(fileName: string): Promise<Blob> {

    const params = new HttpParams()
      .set('fileName', fileName);

    return this.http.get(`${this.baseUrl}api/userFile/download`, { params: params, responseType: "blob" })
      .toPromise()
      .catch(this.handleErrorPromise);
  }
 
  private handleErrorPromise(error: Response | any) {
    console.error(error.message || error);
    return Promise.reject(error.message || error);
  }
}

