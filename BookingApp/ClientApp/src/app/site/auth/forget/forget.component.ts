import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../../services/auth.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-forget',
  templateUrl: './forget.component.html'
})
export class ForgetComponent implements OnInit {

  private forgetForm: FormGroup;

  constructor(private authServie: AuthService) {
    this.forgetForm = new FormGroup({
      'email': new FormControl('', [Validators.required, Validators.email])
    });
   }

  ngOnInit() {
  }

  forget() {
    this.authServie.forget(this.forgetForm.value.email);
  }
}
