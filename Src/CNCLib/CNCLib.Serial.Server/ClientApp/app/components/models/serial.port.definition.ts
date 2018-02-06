import { Component, Inject } from '@angular/core';

export class SerialPortDefinition 
{
    Id: number;
    PortName: string;
    IsConnected: number;
    IsAborted: number;
    IsSingleStep: number;
    CommandsInQueue: number;
}
