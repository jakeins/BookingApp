import { Component, OnInit, Input } from '@angular/core';
import { User } from '../../models/user';
import { UserService } from '../../services/user.service';
import { Logger } from '../../services/logger.service';
import { Blocking } from '../../models/UserBlockingDto';
import { forEach } from '@angular/router/src/utils/collection';
import { UserPage } from '../../models/user-page';



@Component({
  selector: 'app-admin-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.css']
})
export class UserComponent implements OnInit {
  users: User[];
 public userPage: UserPage;
  page: number = 1;
  loaded: boolean;
  pageSize:number = 10;
  index: number = 1;
 // pages: number;
 
  constructor(private userService: UserService) {
    
  }
  public blocking: Blocking;

  blockButton(userId: string) {
    console.log(userId);
    for (var i = 0, len = this.userPage.items.length; i < len; i++) {
      if (this.userPage.items[i].id === userId) {
        if (this.userPage.items[i].isBlocked === true) {
          this.blocking.IsBlocked = false;
          this.index = i;
        }
        if (this.userPage.items[i].isBlocked === false) {
          this.blocking.IsBlocked = true;
          this.index = i;
        }
      }
    }
    this.userService.blockUser(userId, this.blocking).subscribe(() => {
      this.userPage.items[this.index].isBlocked = this.blocking.IsBlocked; 
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
    this.blocking = new Blocking;
   this.getUserPages(this.page = 1, this.pageSize)
    //this.userService.getUsersPage(this.page, this.pageSize = 10).subscribe((res: UserPage) => {
    //this.userPage = res;
    //});
  }

  handleError(error: any) {
    console.log(error);
    //this.apiError = error['status'];

    //if (error['error'] != undefined)
    //  this.apiError += ': ' + error['error']['Message'];
  }
}
