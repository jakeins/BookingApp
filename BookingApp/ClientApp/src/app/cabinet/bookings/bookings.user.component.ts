import { Component, OnInit } from '@angular/core';
import { BookingsModeService, BookingsComponentMode } from '../../services/bookings-component-mode.service';
import { UserInfoService } from '../../services/user-info.service';

@Component({
    selector: 'app-cabinet-bookings',
    templateUrl: './bookings.user.component.html'
})
export class UserBookingsComponent implements OnInit {

    constructor(private bookingsComponentConfig: BookingsModeService, private userInfoService: UserInfoService) { }

    ngOnInit() {
      this.bookingsComponentConfig.currentMode = BookingsComponentMode.User;
      this.bookingsComponentConfig.userId = this.userInfoService.userId;
    }

}
