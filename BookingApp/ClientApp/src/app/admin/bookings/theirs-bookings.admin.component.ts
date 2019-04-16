import { Component, OnInit } from '@angular/core';
import { UserInfoService } from '../../services/user-info.service';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'app-theirs-bookings',
  templateUrl: './theirs-bookings.admin.component.html'
})
export class TheirsBookingsComponent implements OnInit {

  constructor(
    private userInfoService: UserInfoService,
    private actRoute: ActivatedRoute
  ) { }

    ngOnInit() {
    }
}
