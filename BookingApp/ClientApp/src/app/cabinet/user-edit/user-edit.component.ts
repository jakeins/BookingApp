import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service'

@Component({
  selector: 'app-cabinet-edit-profile',
  templateUrl: './user-edit.component.html'
})
export class UserEditComponent implements OnInit {

  userName: any;

  constructor(private userService: UserService) { }

  ngOnInit() {
    this.userName = this.userService.getUserName();
  }

}
