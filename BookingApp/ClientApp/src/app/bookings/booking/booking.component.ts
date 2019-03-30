import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Logger } from '../../services/logger.service';
import { AuthService } from '../../services/auth.service';
import { BookingService } from '../../services/booking.service';
import { Booking } from '../../models/booking';

@Component({
    selector: 'app-booking',
    templateUrl: './booking.component.html'
})
export class BookingComponent implements OnInit {

  booking: Booking;
  id: number;

  constructor(
    private bookingService: BookingService,
    private actRoute: ActivatedRoute,
    private authService: AuthService,
    private router: Router
  ) {
  }

  authChangedSubscription: any;

  ngOnInit() {
    this.actRoute.params.subscribe(params => { this.id = +params['id']; });
    this.resetData();
    this.authChangedSubscription = this.authService.AuthChanged.subscribe(() => this.resetData());
  }

  ngOnDestroy() {
    this.authChangedSubscription.unsubscribe();
  };

  resetData() {
    this.bookingService.getBooking(this.id).subscribe((response: Booking) => {
      console.log(response);
      this.booking = response;
    }, error => { this.router.navigate(['/error']); });
  }
}
