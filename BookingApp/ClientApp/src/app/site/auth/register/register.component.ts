import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { AuthService } from '../../../services/auth.service';
import { RegisterFormModel } from '../../../models/register-form.model';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html'
})
export class RegisterComponent implements OnInit {

  private registerForm: FormGroup;

  constructor(private authService: AuthService) {
    this.registerForm = new FormGroup({
      'userName': new FormControl('', [Validators.required, Validators.minLength(3)]),
      'email': new FormControl('', [Validators.required, Validators.email]),
      'password': new FormControl('', Validators.required),
      'confirmPassword': new FormControl(''),
    }, {validators: this.comparePassword});
  }

  ngOnInit() {
  }

  register() {
    console.log(this.registerForm);
    this.authService.register(
      new RegisterFormModel(
        this.registerForm.value.userName,
        this.registerForm.value.email,
        this.registerForm.value.password,
        this.registerForm.value.confirmPassword
      )
    );
  }

  private comparePassword(group: FormGroup) {
    const pass = group.value.password;
    const confirm = group.value.confirmPassword;

    return pass === confirm ? null : {notSame: true};
}

}
