import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { AuthService } from '../../../services/auth.service';
import { RegisterFormModel } from '../../../models/register-form.model';
import { Router } from '@angular/router';
import { USERNAME_REGEX } from '../../../globals';


@Component({
  selector: 'app-register',
  styleUrls: ['../form.auth.css'],
  templateUrl: './register.component.html'
})
export class RegisterComponent implements OnInit {

  private registerForm: FormGroup;
  private apiError: string = "";

  constructor(private authService: AuthService, private router: Router) {
    this.registerForm = new FormGroup({
      'userName': new FormControl('', [Validators.required, Validators.minLength(3), Validators.pattern(USERNAME_REGEX)]),
      'email': new FormControl('', [Validators.required, Validators.email]),
      'password': new FormControl('', Validators.required),
      'confirmPassword': new FormControl('', Validators.required),
    }, { validators: this.comparePassword });
  }

  ngOnInit() {
    
  }

  register() {
    this.authService.register(
      new RegisterFormModel(
        this.registerForm.value.userName,
        this.registerForm.value.email,
        this.registerForm.value.password,
        this.registerForm.value.confirmPassword
      )
    ).subscribe(data => this.router.navigate(['/login']), error => this.handleError(error));
  }

  private comparePassword(group: FormGroup) {
    const pass = group.value.password;
    const confirm = group.value.confirmPassword;

    return pass === confirm ? null : { notSame: true };
  }

  private handleError(error: any) {
    this.apiError = error['status'];
    if (error['error'] != undefined) {
      this.apiError += ': ' + error['error']['Message'];
    }
  }

}
