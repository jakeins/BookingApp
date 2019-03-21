import { Component, OnInit, Input } from '@angular/core';
import { User } from '../../models/user';
import { UserService } from '../../services/user.service';
import { Logger } from '../../services/logger.service';
import { forEach } from '@angular/router/src/utils/collection';
import { UserPage } from '../../models/user-page';
import { UserRegister } from '../../models/user-register';
import { error } from 'protractor';



@Component({
  selector: 'app-admin-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.css']
})
export class UserComponent implements OnInit {
  users: User[];
  public userPage: UserPage;
  userRegister: UserRegister;
  page: number = 1;
  loaded: boolean;
  pageSize:number = 10;
  indexBlocking: number = 1;
  indexApproval: number = 1;
 // pages: number;
 
  constructor(private userService: UserService) {

  }
  public blocking: boolean;

  blockButton(userId: string) {
    console.log(userId);
    for (var i = 0, len = this.userPage.items.length; i < len; i++) {
      if (this.userPage.items[i].id === userId) {
        if (this.userPage.items[i].isBlocked === true) {
          this.blocking = false;
          this.indexBlocking = i;
        }
        if (this.userPage.items[i].isBlocked === false) {
          this.blocking = true;
          this.indexBlocking = i;
        }
      }
    }
    this.userService.blockUser(userId, this.blocking).subscribe(() => {
      this.userPage.items[this.indexBlocking].isBlocked = this.blocking; 
    }, error => this.handleError(error));
 

  }

  approvalButton(userId: string, approval: boolean) {
    for (var i = 0, len = this.userPage.items.length; i < len; i++) {
      if (this.userPage.items[i].id === userId) {
        this.indexApproval = i;
      }
    }
    this.userService.approvalUser(userId, approval).subscribe(() => {
      this.userPage.items[this.indexApproval].approvalStatus = approval;
    }, error => this.handleError(error));
  }

  getUserPages(page: number, pageSize: number) {
    this.userService.getUsersPage(page, pageSize).subscribe((res: UserPage) => {
      this.loaded = true;
      this.userPage = res;
      this.page = page;
    });
  }

  ngOnInit() {
    this.getUserPages(this.page = 1, this.pageSize)
    //this.userService.createUser(this.userRegister).subscribe((res) => {
    //   console.log(res);
    //});
  }

  handleError(error: any) {
    console.log(error);
    //this.apiError = error['status'];

    //if (error['error'] != undefined)
    //  this.apiError += ': ' + error['error']['Message'];
  }
}
