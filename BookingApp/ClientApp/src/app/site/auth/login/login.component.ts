import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../../services/auth.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html'
})
export class LoginComponent implements OnInit {

  private loginForm: FormGroup;
  private errorMessage: string;
  constructor(private authService: AuthService, private router: Router) {
    this.loginForm = new FormGroup({
      'email': new FormControl('', [Validators.required, Validators.email]),
      'password': new FormControl('', Validators.required),
    });
  }

  ngOnInit() {

  }

  login() {
    if (!this.authService.isUser) {
      this.authService.login(this.loginForm.value.email, this.loginForm.value.password)
        .subscribe(data => this.router.navigate(['/']),
          err => {
            if (err.error['loginFaillure'] !== undefined) {
              this.errorMessage = err.error['loginFailure'].toString();
              this.loginForm.setErrors({
                'loginFailure': true
              });
            } else {
              this.loginForm.setErrors({
                'loginError': true
              });
            }
          });
    }
  }

}
