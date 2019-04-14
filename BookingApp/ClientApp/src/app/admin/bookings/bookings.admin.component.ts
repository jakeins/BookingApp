import { Component, OnInit } from '@angular/core';
import { UserInfoService } from '../../services/user-info.service';

@Component({
  selector: 'app-admin-bookings',
  templateUrl: './bookings.admin.component.html',
})
export class AdminBookingComponent implements OnInit {

  name: string;

  constructor(
    private userInfo: UserInfoService)
  { }

  ngOnInit() {
  }

}
