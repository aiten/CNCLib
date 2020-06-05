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

import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormGroup, FormArray, FormControl, FormBuilder, Validators } from '@angular/forms';
import { MatDialog } from "@angular/material/dialog";

import { SerialServerConnection } from '../../serial-server/serial-server-connection';
import { JoystickServerConnection } from '../../serial-server/joystick-server-connection';

import { MachineControlGlobal } from '../machine-control.global';
import { MachineControlState } from '../machine-control-state';
import { Eeprom, ECommandSyntax, EReverenceSequence, ESignature } from "../../models/eeprom";

import { MessageBoxComponent } from "../../modal/message-box/message-box.component";
import { MessageBoxResult } from "../../modal/message-box-data";
import { EReverenceType } from "../../models/eeprom-axis";


@Component({
  selector: 'eeprom',
  templateUrl: './eeprom.component.html',
  styleUrls: ['./eeprom.component.css'],
})
export class EepromComponent implements OnInit, OnDestroy {

  isLoading = true;
  isEepromLoaded = false;
  isMore = false;

  isAxisExtended = false;

  eeprom: Eeprom = new Eeprom();

  eepromForm: FormGroup;
  axisFormArray: FormArray;

  ECommandSyntax: typeof ECommandSyntax = ECommandSyntax;
  EReverenceSequence: typeof EReverenceSequence = EReverenceSequence;
  EReverenceType: typeof EReverenceType = EReverenceType;

  keysECommandSyntax: any[];
  keysEReverenceSequence: any[];
  keysEReverenceType: any[];

  constructor(
    public serialServer: SerialServerConnection,
    public joystickServer: JoystickServerConnection,
    public machineControlGlobal: MachineControlGlobal,
    public machineControlState: MachineControlState,
    private dialog: MatDialog,
    private fb: FormBuilder
  ) {

    this.keysECommandSyntax = Object.keys(ECommandSyntax).filter(f => !isNaN(Number(f)));
    this.keysEReverenceSequence = Object.keys(EReverenceSequence).filter(f => !isNaN(Number(f)));
    this.keysEReverenceType = Object.keys(EReverenceType).filter(f => !isNaN(Number(f)));

    this.eepromForm = fb.group(
      {
        maxStepRate: [0, [Validators.required, Validators.min(1), Validators.max(99999999)]],
        acc: [0, [Validators.required, Validators.min(62), Validators.max(1024)]],
        dec: [0, [Validators.required, Validators.min(62), Validators.max(1024)]],
        jerkSpeed: [0, [Validators.required, Validators.min(1), Validators.max(65535)]],
        refMoveStepRate: [0, [Validators.required, Validators.min(0), Validators.max(99999999)]],
        moveAwayFromReference: [0, [Validators.required, Validators.min(0), Validators.max(99999999)]],
        stepsPerMm1000: [0, [Validators.required, Validators.min(0)]],

        maxSpindleSpeed: [0, [Validators.required, Validators.min(0), Validators.max(65535)]],
        spindleFadeTime: [0, [Validators.required, Validators.min(0), Validators.max(255)]],

        info1: [0, [Validators.required]],
        info2: [0, [Validators.required]],

        numAxis: [0, [Validators.required]],
        useAxis: [0, [Validators.required]],

        hasSpindle: [false, [Validators.required]],
        hasAnalogSpindle: [false, [Validators.required]],
        hasSpindleDirection: [false, [Validators.required]],
        hasCoolant: [false, [Validators.required]],
        hasProbe: [false, [Validators.required]],
        hasSD: [false, [Validators.required]],
        hasEeprom: [false, [Validators.required]],
        canRotate: [false, [Validators.required]],
        hasHold: [false, [Validators.required]],
        hasResume: [false, [Validators.required]],
        hasHoldResume: [false, [Validators.required]],
        hasKill: [false, [Validators.required]],
        isLaser: [false, [Validators.required]],
        commandSyntax: [0, [Validators.required]],
        dtrIsReset: [false, [Validators.required]],
        needEEpromFlush: [false, [Validators.required]],
        workOffsetCount: [0, [Validators.required]],

        penDownFeedrate: [0, [Validators.required]],
        penUpFeedrate: [0, [Validators.required]],
        movePenDownFeedrate: [0, [Validators.required]],
        movePenUpFeedrate: [0, [Validators.required]],
        movePenChangeFeedrate: [0, [Validators.required]],
        penDownPos: [0, [Validators.required]],
        penUpPos: [0, [Validators.required]],
        penChangePos_x: [0, [Validators.required]],
        penChangePos_y: [0, [Validators.required]],
        penChangePos_z: [0, [Validators.required]],
        penChangePos_x_ofs: [0, [Validators.required]],
        penChangePos_y_ofs: [0, [Validators.required]],
        servoClampOpenPos: [0, [Validators.required]],
        servoClampClosePos: [0, [Validators.required]],
        servoClampOpenDelay: [0, [Validators.required]],
        servoClampCloseDelay: [0, [Validators.required]],

        axis: fb.array([
          this.createAxisControl(),
          this.createAxisControl(),
          this.createAxisControl(),
          this.createAxisControl(),
          this.createAxisControl(),
          this.createAxisControl()
        ]),

        refSequence1: new FormControl(0, [Validators.required]),
        refSequence2: new FormControl(0, [Validators.required]),
        refSequence3: new FormControl(0, [Validators.required]),
        refSequence4: new FormControl(0, [Validators.required]),
        refSequence5: new FormControl(0, [Validators.required]),
        refSequence6: new FormControl(0, [Validators.required]),

      });

    this.eepromForm.valueChanges.subscribe((value) => {
      if (this.isEepromLoaded)
        Object.assign(this.eeprom, value);
    });

    this.axisFormArray = this.eepromForm.controls['axis'] as FormArray;
  }

  compareWithEnum(lt1, lt2) {
    return lt1 == lt2;
  }

  get f() { return this.eepromForm.controls; }

  ff(axis: number) {
    return (this.axisFormArray.at(axis) as FormGroup).controls;
  }

  createAxisControl(): FormGroup {
    return this.fb.group({
      size: new FormControl(0, [Validators.required]),
      refMove: new FormControl(0, [Validators.required]),
      stepperDirection: new FormControl(false, [Validators.required]),
      refHitValueMin: new FormControl(0, [Validators.required]),
      refHitValueMax: new FormControl(0, [Validators.required]),
      maxStepRate: new FormControl(0, [Validators.required]),
      acc: new FormControl(0, [Validators.required, Validators.min(0), Validators.max(1024)]),
      dec: new FormControl(0, [Validators.required, Validators.min(0), Validators.max(1024)]),
      stepsPerMm1000: new FormControl(0, [Validators.required]),
      refMoveStepRate: new FormControl(0, [Validators.required]),
      initPosition: new FormControl(0, [Validators.required]),
      probeSize: new FormControl(0, [Validators.required]),
    });
  }

  async ngOnInit(): Promise<void> {
    await this.machineControlState.load();
    this.isLoading = false;
  }

  async ngOnDestroy(): Promise<void> {
    await this.machineControlState.close();
  }

  isConnected() {
    return this.machineControlState.isConnected;
  }

  isPlotter() {
    return this.isEepromLoaded && this.eeprom.signature == ESignature.SIGNATUREPLOTTER;
  }

  async loadEeprom() {

    let eeprom = await this.machineControlState.readEeprom();
    console.log(eeprom);

    Object.assign(this.eeprom, eeprom);

    this.eeprom['axis'] = [this.eeprom.axisX, this.eeprom.axisY, this.eeprom.axisZ, this.eeprom.axisA, this.eeprom.axisB, this.eeprom.axisC];

    this.isAxisExtended = eeprom.axisX.dweeSizeOf > 3;

    this.eepromForm.patchValue(this.eeprom);

    console.log(this.eeprom);
    this.isEepromLoaded = true;
  }

  async storeEeprom(): Promise<void> {

    console.log("Store");

    // stop here if form is invalid
    if (this.eepromForm.invalid) {
      return;
    }

    this.eeprom.axisX = this.eeprom['axis'][0];
    this.eeprom.axisY = this.eeprom['axis'][1];
    this.eeprom.axisZ = this.eeprom['axis'][2];
    this.eeprom.axisA = this.eeprom['axis'][3];
    this.eeprom.axisB = this.eeprom['axis'][4];
    this.eeprom.axisC = this.eeprom['axis'][5];

    console.log(this.eeprom);

    const dialogRef = this.dialog.open(MessageBoxComponent,
      {
        width: '250px',
        data: { title: "Warning", message: "Write EEprom. Please restart the machine.", haveYes: true, haveCancel: true }
      });

    dialogRef.afterClosed().subscribe(async result => {
      if (result.result == MessageBoxResult.Yes) {
        await this.machineControlState.writeEeprom(this.eeprom);
        this.isEepromLoaded = false;
      }
    });

  }

  async eraseEeprom() {

    const dialogRef = this.dialog.open(MessageBoxComponent,
      {
        width: '250px',
        data: { title: "Warning", message: "Delete EEprom (factory reset). After the factory reset please restart the machine.", haveYes: true, haveCancel: true }
      });

    dialogRef.afterClosed().subscribe(async result => {
      if (result.result == MessageBoxResult.Yes) {
        await this.machineControlState.deleteEeprom();
        this.isEepromLoaded = false;
      }
    });
  }
}
