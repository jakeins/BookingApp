import { Component, OnInit } from '@angular/core';
import { UserInfoService } from '../../services/user-info.service';

@Component({
    selector: 'app-cabinet-bookings',
    templateUrl: './bookings.user.component.html'
})
export class UserBookingsComponent implements OnInit {

    constructor(private userInfoService: UserInfoService) { }

    ngOnInit() {
    }
}
