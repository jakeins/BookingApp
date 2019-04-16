import { Component, OnInit } from '@angular/core';
import { BookingsModeService, BookingsComponentMode } from '../../services/bookings-component-mode.service';
import { UserInfoService } from '../../services/user-info.service';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'app-theirs-bookings',
  templateUrl: './theirs-bookings.admin.component.html'
})
export class TheirsBookingsComponent implements OnInit {

  constructor(
    private bookingsComponentConfig: BookingsModeService,
    private userInfoService: UserInfoService,
    private actRoute: ActivatedRoute
  ) { }

    ngOnInit() {
      this.bookingsComponentConfig.currentMode = BookingsComponentMode.User;
      this.bookingsComponentConfig.userId = this.actRoute.parent.snapshot.params['id'];
    }
}
