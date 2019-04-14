import { Component, OnInit } from '@angular/core';
import { UserInfoService } from '../../services/user-info.service';
import { BookingsModeService, BookingsComponentMode } from '../../services/bookings-component-mode.service';

@Component({
  selector: 'app-admin-bookings',
  templateUrl: './bookings.admin.component.html',
})
export class AdminBookingComponent implements OnInit {

  name: string;

  constructor(
    private userInfo: UserInfoService,
    private bookingsConfigService: BookingsModeService)
  { }

  ngOnInit() {
    this.name = this.userInfo.username;
    this.bookingsConfigService.currentMode = BookingsComponentMode.Admin;
  }

}
