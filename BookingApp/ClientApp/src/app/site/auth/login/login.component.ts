import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../../services/auth.service';
import { NgForm, FormGroup, FormControl, Validators } from '@angular/forms';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html'
})
export class LoginComponent implements OnInit {

  loginForm: FormGroup;
  constructor(private authService: AuthService) {
    this.loginForm = new FormGroup({
      'email': new FormControl('', [Validators.required, Validators.email]),
      'password': new FormControl('', Validators.required),
    });
  }

  ngOnInit() {

  }

  login() {
    console.log(this.loginForm);
    this.authService.login(this.loginForm.value.email, this.loginForm.value.password);
  }

}
