import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service'
import { FormGroup, FormControl, FormBuilder, Validators, FormArray } from '@angular/forms';
import { User } from '../../models/user';
import { Logger } from '../../services/logger.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-cabinet-edit-profile',
  templateUrl: './user-edit.component.html'
})
export class UserEditComponent implements OnInit {

  userName: any;
  user: User;

  userForm: FormGroup
  constructor(private fb: FormBuilder,
    private userService: UserService,
     private actRoute: ActivatedRoute
  ) { }

  ngOnInit() {
    this.userName = this.userService.getUserName();
    this.userService.getUserByUserName(this.userName).subscribe((res: User) => {
      this.user = res;
      this.initializeForm();
    }, error => this.handleError(error));;
    console.log(this.actRoute.snapshot.params);
    console.log(this.actRoute.snapshot.params['some']);
  }

  initializeForm() {
    this.userForm = new FormGroup({
      userName: new FormControl(this.user.userName),
      email: new FormControl(this.user.email),
    });
    
    Logger.log('Form initialized.');
    Logger.log(this.userForm);
  }

  handleError(error: any) {
    console.log(error);
    //this.apiError = error['status'];

    //if (error['error'] != undefined)
    //  this.apiError += ': ' + error['error']['Message'];
  }
}
