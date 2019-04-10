import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../../services/auth.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserInfoService } from '../../../services/user-info.service';


@Component({
  selector: 'app-login',
  styleUrls: ['../form.auth.css'],
  templateUrl: './login.component.html'
})
export class LoginComponent implements OnInit {

  private loginForm: FormGroup;
  private apiError: string = "";
  constructor(private authService: AuthService, private router: Router, private userInfo: UserInfoService) {
    this.loginForm = new FormGroup({
      'email': new FormControl('', [Validators.required, Validators.email]),
      'password': new FormControl('', Validators.required),
    });
  }

  ngOnInit() {

  }

  login() {
    if (!this.userInfo.isUser) {
      this.authService.login(this.loginForm.value.email, this.loginForm.value.password)
        .subscribe(data => this.router.navigate(['/']),
        err => {
          if (err.error['loginFailure']) {
            this.apiError = err.error['loginFailure'];
          } else {
            this.apiError = err.error.Message;
          }
        });
    }
  }

}
