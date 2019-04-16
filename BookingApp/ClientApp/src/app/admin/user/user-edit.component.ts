import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { USERNAME_REGEX } from '../../globals';
import { UserUpdate } from '../../models/user-update';
import { User } from '../../models/user';
import 'rxjs/Rx';
import { Logger } from '../../services/logger.service';


@Component({
  selector: 'app-admin-user-edit',
  templateUrl: './user-edit.component.html',
  styleUrls: ['./user.component.css']
})
export class UserRenameComponent implements OnInit {

  private editUserForm: FormGroup;
  private userId: string;
  private apiError: string = "";
  private successMessage = "";
  private userUpdate: UserUpdate = new UserUpdate();


  constructor(private userService: UserService, private router: Router, private actRoute: ActivatedRoute) {
  }

  ngOnInit(): void {
    this.userId = this.actRoute.parent.snapshot.params['id'];
    this.initializeForm();

    this.userService.getUserById(this.userId).subscribe((res: User) => {
      this.userUpdate.userName = res.userName;
      this.initializeForm();
    }, error => this.handleError(error));

  }

  private initializeForm() {
    this.editUserForm = new FormGroup({
      userName: new FormControl(this.userUpdate.userName, [Validators.required, Validators.minLength(3), Validators.pattern(USERNAME_REGEX)]),
    });
  }



  editUser() {
    this.userUpdate.userName = this.editUserForm.value.userName;
    this.userService.updateUser(this.userId, this.userUpdate).subscribe(() => {
      this.router.navigate(['/admin', 'users', this.userId]);
      }, error => this.handleError(error));
  }

  private handleError(error: any) {
    this.apiError = error.error.Message;
    this.successMessage = "";
  }

}
