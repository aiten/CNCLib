////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2018 Herbert Aitenbichler

  CNCLib is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  CNCLib is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.
  http://www.gnu.org/licenses/
*/

import { APP_BASE_HREF } from '@angular/common';
import { ApplicationRef, NgZone, enableProdMode } from '@angular/core';
import { INITIAL_CONFIG, PlatformState, platformDynamicServer } from '@angular/platform-server';
import { RenderResult, createServerRenderer } from 'aspnet-prerendering';
import 'reflect-metadata';
import 'zone.js';
import { Observable, Subject, asapScheduler, pipe, of, from, interval, merge, fromEvent } from 'rxjs';
import { first } from "rxjs/operators";
import { AppModule } from './app/app.server.module';

enableProdMode();

export default createServerRenderer(params => {

    const providers = [
        { provide: INITIAL_CONFIG, useValue: { document: '<app></app>', url: params.url } },
        { provide: APP_BASE_HREF, useValue: params.baseUrl },
        { provide: 'BASE_URL', useValue: params.origin + params.baseUrl },
    ];

    return platformDynamicServer(providers).bootstrapModule(AppModule).then(moduleRef => {
        const appRef: ApplicationRef = moduleRef.injector.get(ApplicationRef);
        const state = moduleRef.injector.get(PlatformState);
        const zone = moduleRef.injector.get(NgZone);

        return new Promise<RenderResult>((resolve, reject) => {
            zone.onError.subscribe((errorInfo: any) => reject(errorInfo));

            appRef.isStable.pipe(first((isStable: boolean) => isStable)).subscribe(() => {
                // Because 'onStable' fires before 'onError', we have to delay slightly before
                // completing the request in case there's an error to report
                setImmediate(() => {
                    resolve({
                        html: state.renderToString()
                    });
                    moduleRef.destroy();
                });
            });
        });
    });
});
