import { Component, OnInit, Input } from '@angular/core';
import { UserService } from '../../services/user.service';
import { UserPage } from '../../models/user-page';

@Component({
  selector: 'app-admin-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.css']
})
export class UserComponent implements OnInit {
  public userPage: UserPage;
  page: number = 1;
  loaded: boolean;
  pageSize:number = 10;
 
  constructor(private userService: UserService) { }

  getUserPages(page: number, pageSize: number) {
    this.userService.getUsersPage(page, pageSize).subscribe((res: UserPage) => {
      this.loaded = true;
      this.userPage = res;
      this.page = page;
    });
  }

  ngOnInit() {
    this.getUserPages(this.page = 1, this.pageSize)
  }

}
