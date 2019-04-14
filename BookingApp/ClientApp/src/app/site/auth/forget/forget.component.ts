import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../../services/auth.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Logger } from '../../../services/logger.service';

@Component({
  selector: 'app-forget',
  styleUrls: ['../form.auth.css'],
  templateUrl: './forget.component.html'
})
export class ForgetComponent implements OnInit {

  private forgetForm: FormGroup;
  private successMessage: string = "";
  private apiError: string = "";

  constructor(private authServie: AuthService) {
    this.forgetForm = new FormGroup({
      'email': new FormControl('', [Validators.required, Validators.email])
    });
   }

  ngOnInit() {
  }

  forget() {
    this.authServie.forget(this.forgetForm.value.email)
      .subscribe(data => {
        this.successMessage = 'Reset mail have been sent';
        this.apiError = "";
      }, err => {
        this.successMessage = "";
        Logger.error(err);
        this.apiError = err.error.Message;
      });
  }
}
