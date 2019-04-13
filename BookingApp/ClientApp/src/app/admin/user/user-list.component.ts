import { Component, OnInit, Input } from '@angular/core';
import { UserService } from '../../services/user.service';
import { UserPage } from '../../models/user-page';
import { Logger } from '../../services/logger.service';

@Component({
  selector: 'app-admin-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user.component.css']
})
export class UserListComponent implements OnInit {
  public userPage: UserPage;
  page: number = 1;
  loaded: boolean;
  pageSize: number = 10;
  rolesMap = {};

  constructor(private userService: UserService) { }

  getUserPages(page: number, pageSize: number) {
    this.userService.getUsersPage(page, pageSize).subscribe((res: UserPage) => {
      this.loaded = true;
      this.userPage = res;
      this.page = page;

      this.fillRoles();

    });
  }

  fillRoles() {
    for (let user of this.userPage.items) {
      this.userService.getUserRoleById(user.id).subscribe((resin: string[]) => {
        this.rolesMap[user.id] = 'User';

        if (resin.length == 0)
          Logger.error(`User [ ${user.userName} ] [ ${user.id} ] has no roles.`);

        for (let role of resin) {
          if (role == 'Admin')
            this.rolesMap[user.id] = 'Admin';
        }
      });
    }
  }

  ngOnInit() {
    this.getUserPages(this.page = 1, this.pageSize)
  }

}
