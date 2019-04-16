import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { Router, ActivatedRoute } from '@angular/router';
import { User } from '../../models/user';
import { UserRole } from '../../models/user-roles';
import { Logger } from '../../services/logger.service';


@Component({
  selector: 'app-admin-user-read',
  templateUrl: './user-read.component.html'
})
export class UserCPComponent implements OnInit {

  user: User;
  private userId: string;
  private isAdmin: boolean;
  private apiError: string = "";
  private successMessage = "";

  constructor(private userService: UserService, private router: Router, private actRoute: ActivatedRoute) {}

  ngOnInit(): void {
    this.userId = this.actRoute.snapshot.params['id'];

    this.userService.getUserById(this.userId).subscribe((user: User) => {
      this.user = user;
    }, err => this.handleError(err));

    this.userService.getUserRoleById(this.userId).subscribe((res: string[]) => {
      this.isAdmin = res.some(role => role == "Admin");
    }, error => this.handleError(error));
  }

  blockedUser(isBlocked: boolean) {
    this.userService.blockUser(this.userId, isBlocked).subscribe((user: User) => {
      this.successMessage = "Operation success";
      this.apiError = "";
      this.user.isBlocked = isBlocked;
    }, err => this.handleError(err));
  } 

  approveUser(isApprove: boolean) {
    this.userService.approvalUser(this.userId, isApprove).subscribe((user: User) => {
      this.successMessage = "Operation success";
      this.apiError = "";
      this.user.approvalStatus = isApprove;
    }, err => this.handleError(err));
  } 

  deleteUser() {
    this.userService.deleteUser(this.userId).subscribe(() => {
      this.router.navigate(['/admin/users']);
    }, err => this.handleError(err));
  }

  setAdminPrivileges(admin: boolean) {
    let userRole = new UserRole();
    userRole.Role = "Admin";

    let obs = admin ? this.userService.addRole(this.userId, userRole) : this.userService.removeRole(this.userId, userRole);
    obs.subscribe(() => {
      this.successMessage = "Role succesfully changed!";
      this.apiError = "";
      this.isAdmin = admin;
    }, error => this.handleError(error));
  }

  private handleError(error: any) {
    Logger.log(error);
    this.apiError = error.error.Message;
    this.successMessage = "";
  }
 
}
