import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { UserNewPassword } from '../../models/user-new-password';
import { UserService } from '../../services/user.service';
import { FolderFormGroup } from '../../models/folder-form.model';


@Component({
  selector: 'app-user-change-password',
  styleUrls: ['../cabinet.component.css'],
  templateUrl: './user-change-password.component.html'
})
export class UserChangePasswordComponent implements OnInit {

  private changePaswordForm: FormGroup;
  private apiError: string = "";
  private userId: string;
  private successMessage: string = "";
  private userPass: UserNewPassword = new UserNewPassword();


  constructor(private userService: UserService, private router: Router, private actRoute: ActivatedRoute) {}

  ngOnInit() {
    this.changePaswordForm = new FormGroup({
      'currentPassword': new FormControl('', Validators.required),
      'password': new FormControl('', Validators.required),
      'confirmPassword': new FormControl('', Validators.required),
    }, { validators: this.comparePassword });
    this.userId = this.actRoute.snapshot.params['id'];
  }

  changePassword() {
    this.userPass.CurrentPassword = this.changePaswordForm.value.currentPassword;
    this.userPass.NewPassword = this.changePaswordForm.value.password;
    this.userPass.ConfirmNewPassword = this.changePaswordForm.value.confirmPassword;
    this.userService.changePassword(this.userId, this.userPass).subscribe(() => {
      this.successMessage = "You change successfull your password!";
      this.apiError = "";
      this.changePaswordForm.reset();
    }, err => {
      this.apiError = err.error;
      this.successMessage = "";
    });
  }

  private comparePassword(group: FormGroup) {
    const pass = group.value.password;
    const confirm = group.value.confirmPassword;
    return pass === confirm ? null : { notSame: true };
  }

}
