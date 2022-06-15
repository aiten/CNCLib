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

import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, UntypedFormGroup, Validators, ValidatorFn } from '@angular/forms';

import { AuthenticationService } from '../../services/authentication.service';
import { CNCLibUserService } from "../../services/CNCLib-user.service";
import { CNCLibLoggedinService } from '../../services/CNCLib-loggedin.service';

import { homeURL } from "../../app.global";

const passwordValidator: ValidatorFn = (fg: UntypedFormGroup) => {
  const start = fg.get('password').value;
  const end = fg.get('password2').value;
  return start !== null && end !== null && start === end
    ? null
    : { doesNotMatch: true };
};

@Component({
  templateUrl: 'change-password.component.html',
  selector: 'change-password'

})
export class ChangePasswordComponent implements OnInit {
  changePasswordForm: UntypedFormGroup;
  loading = false;
  submitted = false;
  finished = false;
  returnUrl: string;
  error = '';

  constructor(
    public cncLibloggedinService: CNCLibLoggedinService, 
    private formBuilder: FormBuilder,
    private userService: CNCLibUserService,
    private router: Router,
    private authenticationService: AuthenticationService
  ) {
  }

  ngOnInit() {
    this.changePasswordForm = this.formBuilder.group({
        current: ['', [Validators.required, Validators.minLength(8)]],
        password: ['', [Validators.required, Validators.minLength(8)]],
        password2: ['', [Validators.required, Validators.minLength(8)]]
      },
      { validator: passwordValidator });
  }

  get f() { return this.changePasswordForm.controls; }

  async onSubmit() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.changePasswordForm.invalid) {
      return;
    }

    this.loading = true;
    await this.userService
      .changePassword(this.cncLibloggedinService.username(), this.f.current.value, this.f.password.value)
      .then(() => {
          this.loading = false;
          this.finished = true;
          this.error = '';
        })
      .catch(
        error => {
          this.error = error;
          this.loading = false;
        });
  }
}
