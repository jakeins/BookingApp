import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service'
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { UserUpdate } from '../../models/user-update';
import { User } from '../../models/user';
import { USERNAME_REGEX } from '../../globals';

@Component({
  selector: 'app-cabinet-edit-profile',
  styleUrls: ['../cabinet.component.css'],
  templateUrl: './user-edit.component.html'
})
export class UserEditComponent implements OnInit {

  private editUserForm: FormGroup;
  private apiError: string = "";
  private userId: string;
  private successMessage: string = "";
  private userUpdate: UserUpdate = new UserUpdate();

  constructor(private userService: UserService, private router: Router, private actRoute: ActivatedRoute) { }

  ngOnInit() {
    this.userId = this.actRoute.snapshot.params['id'];
    this.initializeForm();

    this.userService.getUserById(this.userId).subscribe((res: User) => {
      this.userUpdate.userName = res.userName;
      this.initializeForm();
    }, error => this.handleError(error));
    
  }

  editUser() {
    this.userUpdate.userName = this.editUserForm.value.userName;
    this.userService.updateUser(this.userId, this.userUpdate).subscribe(() => {
      this.successMessage = "Your name succesfully changed, relogin to review changes!";
      this.apiError = "";
      this.initializeForm();
    }, error => this.handleError(error));
  }

  private initializeForm() {
    this.editUserForm = new FormGroup({
      userName: new FormControl(this.userUpdate.userName, [Validators.required, Validators.minLength(3), Validators.pattern(USERNAME_REGEX)])
    });
  }

  private handleError(error: any) {
    this.apiError = error.error.Message;
    this.successMessage = "";
  }
  
}
