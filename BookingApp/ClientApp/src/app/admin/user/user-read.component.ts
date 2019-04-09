import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { Router, ActivatedRoute } from '@angular/router';
import { User } from '../../models/user';


@Component({
  selector: 'app-admin-user-read',
  templateUrl: './user-read.component.html'
})
export class UserReadComponent implements OnInit {

  user: User;
  private userId: string;
  private apiError: string = "";
  private successMessage = "";

  constructor(private userService: UserService, private router: Router, private actRoute: ActivatedRoute) {}

  ngOnInit(): void {
    this.userId = this.actRoute.snapshot.params['id'];

    this.userService.getUserById(this.userId).subscribe((user: User) => {
      this.user = user;
    }, err => this.handleError(err));
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

  private handleError(error: any) {
    console.log(error);
    this.apiError = error.error.Message;
    this.successMessage = "";
  }
 
}
