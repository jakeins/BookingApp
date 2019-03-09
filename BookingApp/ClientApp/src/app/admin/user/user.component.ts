import { Component, OnInit, Input } from '@angular/core';
import { User } from '../../models/user';
import { UserService } from '../../services/user.service';
import { Logger } from '../../services/logger.service';
import { Blocking } from '../../models/UserBlockingDto';
import { forEach } from '@angular/router/src/utils/collection';



@Component({
  selector: 'app-admin-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.css']
})
export class UserComponent implements OnInit {
  users: User[];
  index: number;
  constructor(private userService: UserService) {

  }
  public blocking: Blocking;

  blockButton(userId: string) {
    for (var i = 0, len = this.users.length; i < len; i++) {
      if (this.users[i].id == userId) {
        if (this.users[i].isBlocked == true) {
          this.blocking.IsBlocked = false;
          this.index = i;
        }
        if (this.users[i].isBlocked == false) {
          this.blocking.IsBlocked = true;
          this.index = i;
        }
      }
    }
    this.userService.blockUser(userId, this.blocking).subscribe(() => {
      Logger.warn(`User ${userId} deleted.`);   
    }, error => this.handleError(error));;
    this.users[this.index].isBlocked = this.blocking.IsBlocked; 
  }
  
  ngOnInit() {
    this.blocking = new Blocking;
    this.userService.getUsers().subscribe(res => {
      this.users = res;
      //console.log(res);
    });
  }

  handleError(error: any) {
    console.log(error);
    //this.apiError = error['status'];

    //if (error['error'] != undefined)
    //  this.apiError += ': ' + error['error']['Message'];
  }
}
