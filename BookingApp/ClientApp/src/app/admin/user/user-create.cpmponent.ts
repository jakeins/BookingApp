import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AdminRegister } from '../../models/admin-register';
import { USERNAME_REGEX } from '../../globals';
import { Logger } from '../../services/logger.service';

@Component({
  selector: 'app-admin-user-create',
  templateUrl: './user-create.component.html',
  styleUrls: ['./user.component.css']
})
export class UserCreateComponent implements OnInit {

  private createUserForm: FormGroup;
  private apiError: string = "";
  private successMessage = "";

  constructor(private userService: UserService, private router: Router, private actRoute: ActivatedRoute) {
    this.createUserForm = new FormGroup({
      'userName': new FormControl('', [Validators.required, Validators.minLength(3), Validators.pattern(USERNAME_REGEX)]),
      'email': new FormControl('', [Validators.required, Validators.email]),
    });
  }

  ngOnInit(): void {
    
  }

  createUser() {
    let user: AdminRegister = new AdminRegister();
    user.userName = this.createUserForm.value.userName;
    user.email = this.createUserForm.value.email;
    console.log(user);
    this.userService.createAdmin(user).subscribe(result => {
      this.createUserForm.reset();
      this.successMessage = "You successfull create admin!";
      this.apiError = "";
    }, error => this.handleError(error));
  }
  
  private handleError(error: any) {
    this.apiError = error.error.Message;
    Logger.error(error);
    this.successMessage = "";
  }

}
