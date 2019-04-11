import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { USERNAME_REGEX } from '../../globals';
import { UserUpdate } from '../../models/user-update';
import { User } from '../../models/user';
import { forkJoin } from 'rxjs/observable/forkJoin';
import { UserRoles } from '../../models/user-roles';


@Component({
  selector: 'app-admin-user-edit',
  templateUrl: './user-edit.component.html',
  styleUrls: ['./user.component.css']
})
export class UserChangeComponent implements OnInit {

  private editUserForm: FormGroup;
  private userId: string;
  private apiError: string = "";
  private successMessage = "";
  private userUpdate: UserUpdate = new UserUpdate();
  private isUser: boolean;
  private isAdmin: boolean;
  private mapRoles: string[];

  constructor(private userService: UserService, private router: Router, private actRoute: ActivatedRoute) {
  }

  ngOnInit(): void {
    this.userId = this.actRoute.snapshot.params['id'];
    this.initializeForm();

    this.userService.getUserById(this.userId).subscribe((res: User) => {
      this.userUpdate.userName = res.userName;
      this.initializeForm();
    }, error => this.handleError(error));
    this.userService.getUserRoleById(this.userId).subscribe((res: string[]) => {
      this.setRoles(res);
    }, error => this.handleError(error));
  }

  private initializeForm() {
    this.editUserForm = new FormGroup({
      userName: new FormControl(this.userUpdate.userName, [Validators.required, Validators.minLength(3), Validators.pattern(USERNAME_REGEX)]),
      isUser: new FormControl(this.isUser),
      isAdmin: new FormControl(this.isAdmin)
    });
  }

  setRoles(res: string[]) {
    this.mapRoles = res;
    this.isUser = res.some(role => role == "User");
    this.isAdmin = res.some(role => role == "Admin");
    this.initializeForm();
  }

  editUser() {
    let sources = [];
    let userRole = new UserRoles();

    this.userUpdate.userName = this.editUserForm.value.userName;
    sources.push(this.userService.updateUser(this.userId, this.userUpdate));
    if (this.mapRoles.some(role => role == "User")) {
      if (this.editUserForm.value.isUser == false) {
        userRole.Role = "User";
        sources.push(this.userService.removeRole(this.userId, userRole));
      }
    } else {
      if (this.editUserForm.value.isUser == true) {
        userRole.Role = "User";
        sources.push(this.userService.addRole(this.userId, userRole));
      }
    }
    if (this.mapRoles.some(role => role == "Admin")) {
      if (this.editUserForm.value.isAdmin == false) {
        userRole.Role = "Admin";
        sources.push(this.userService.removeRole(this.userId, userRole));
      }
    } else {
      if (this.editUserForm.value.isAdmin == true) {
        userRole.Role = "Admin";
        sources.push(this.userService.addRole(this.userId, userRole));
      }
    }
    console.log(userRole);

    forkJoin(...sources).subscribe(() => {
      this.successMessage = "Your name and role succesfully changed!";
      this.apiError = "";
      this.initializeForm();
    }, error => this.handleError(error));
  }


  private handleError(error: any) {
    console.log(error);
    this.apiError = error.error.Message;
    this.successMessage = "";
  }

}
