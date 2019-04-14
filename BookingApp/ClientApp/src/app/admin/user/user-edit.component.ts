import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { USERNAME_REGEX } from '../../globals';
import { UserUpdate } from '../../models/user-update';
import { User } from '../../models/user';
import { UserRoles } from '../../models/user-roles';
import 'rxjs/Rx';
import { Observable } from 'rxjs/Rx';


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
      isAdmin: new FormControl(this.isAdmin)
    });
  }

  setRoles(res: string[]) {
    this.mapRoles = res;
    this.isAdmin = res.some(role => role == "Admin");
    this.initializeForm();
  }

  editUser() {
    let userRole = new UserRoles();

    this.userUpdate.userName = this.editUserForm.value.userName;
    this.userService.updateUser(this.userId, this.userUpdate)
      .switchMap(() => {
        if (this.mapRoles.some(role => role == "Admin")) {
          if (this.editUserForm.value.isAdmin == false) {
            userRole.Role = "Admin";
            return this.userService.removeRole(this.userId, userRole);
          }
        } else {
          if (this.editUserForm.value.isAdmin == true) {
            userRole.Role = "Admin";
            return this.userService.addRole(this.userId, userRole);
          }
        }
        return Observable;
      }).subscribe(() => {
        this.successMessage = "Your name and role succesfully changed!";
        this.apiError = "";
      }, error => this.handleError(error));
  }

  private handleError(error: any) {
    this.apiError = error.error.Message;
    this.successMessage = "";
  }

}
