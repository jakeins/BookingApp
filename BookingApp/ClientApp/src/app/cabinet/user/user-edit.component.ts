import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service'
import { FormGroup, FormControl, FormBuilder, Validators, FormArray } from '@angular/forms';
import { User } from '../../models/user';
import { Logger } from '../../services/logger.service';
import { UserInfoService } from '../../services/user-info.service';
import { AuthService } from '../../services/auth.service';
import { ActivatedRoute } from '@angular/router';
import { UserUpdate } from '../../models/user-update';
import { userInfo } from 'os';

@Component({
  selector: 'app-cabinet-edit-profile',
  templateUrl: './user-edit.component.html'
})
export class UserEditComponent implements OnInit {

  userName: any;
  user: User;
  userUpdate: UserUpdate;
  isAdmin: boolean;

  userForm: FormGroup
  constructor(private fb: FormBuilder,
    private userService: UserService,
    private authService: AuthService,
    private actRoute: ActivatedRoute,
    private userInfoService: UserInfoService
  ) { }

  ngOnInit() {
    let userId = this.actRoute.snapshot.params['id'];
    this.isAdmin = this.userInfoService.roles.includes('Admin');
    this.userService.getUserById(userId).subscribe((res: User) => {
      this.user = res;
      this.initializeForm();
    }, error => this.handleError(error));
    
  }

  initializeForm() {
    this.userForm = new FormGroup({
      userName: new FormControl(this.user.userName),
      email: new FormControl(this.user.email),
    });
    
    Logger.log('Form initialized.');
    Logger.log(this.userForm);
  }

  onSubmit() {
    

    let formData = this.userForm.value;

    this.userUpdate = formData;
    this.userUpdate.isBlocked = this.user.isBlocked;
    this.userUpdate.approvalStatus = this.user.approvalStatus;
    //this.userUpdate.userName = formData.userName;
    //this.userUpdate.email = formData.email;
    //this.userUpdate.isBlocked = false;
    //this.userUpdate.approvalStatus = true;

   
    Logger.log(this.userUpdate);
    console.log(this.userUpdate);
    this.userService.updateUser(this.userUpdate, this.userInfoService.userId)
        .subscribe(result => {
          Logger.log(result);
          console.log(result);
          this.authService.refresh();
        }, error => this.handleError(error));

 
  }

  handleError(error: any) {
    console.log(error);
    //this.apiError = error['status'];

    //if (error['error'] != undefined)
    //  this.apiError += ': ' + error['error']['Message'];
  }
}
