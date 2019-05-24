import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormArray, FormControl, FormBuilder, Validators } from '@angular/forms';
import { EepromConfigInput } from '../../models/eeprom-config-input';

@Component({
  selector: 'ha-eeprom-config-form',
  templateUrl: './eeprom-config-form.component.html',
  styleUrls: ['./eeprom-config-form.component.css']
})
export class EepromConfigFormComponent implements OnInit
{
  @Input() eepromConfigInput: EepromConfigInput;
  eepromConfigForm: FormGroup;

  constructor(
    private fb: FormBuilder
  )
  {
    this.eepromConfigInput = new EepromConfigInput();

    this.eepromConfigForm = fb.group(
      {
        teeth: [15, [Validators.required]],
        toothsizeinMm: [2.0, [Validators.required]],
        microsteps: [16, [Validators.required]],
        stepsPerRotation: [200, [Validators.required]],
        estimatedRotationSpeed: [7.8, [Validators.required]],
        timeToAcc: [0.2, [Validators.required]],
        timeToDec: [0.15, [Validators.required]],
      });

    this.eepromConfigForm.valueChanges.subscribe((value) =>
    {
      Object.assign(this.eepromConfigInput, value);
    });
  }

  ngOnInit()
  {
  }

  isValid() : boolean
  {
    return this.eepromConfigForm.valid;
  }

  getFormValues()
  {
      this.calcEepromConfig(this.eepromConfigForm.value);
  }

  calcEepromConfig(value: any)
  {
    console.log(value);
    Object.assign(this.eepromConfigInput, value);
  }

  userExistsValidator = (control) =>
  {
    /*
        return this.userService.checkUserExists(control.value)
          .map(checkResult =>
          {
            return (checkResult === false) ? { userNotFound: true } : null;
          });
    */
  }

  userExistsValidatorReused = (control) =>
  {
    /*    
        const validator = new UserExistsValidatorDirective(this.userService);
        return validator.validate(control);
    */
  };
}
