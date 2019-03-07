import { Component, OnInit } from '@angular/core';
import { User } from '../../models/user';
import { UserService } from '../../services/user.service';


@Component({
  selector: 'app-admin-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.css']
})
export class UserComponent implements OnInit {
  constructor(private userService: UserService) {

  }
  users: User[];
  ngOnInit() {
    this.userService.getUsers().subscribe(res => {
      this.users = res;
      //console.log(res);
    });
  }

}
