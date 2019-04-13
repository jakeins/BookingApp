import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Logger } from '../../services/logger.service';
import { AuthService } from '../../services/auth.service';
import { BookingService } from '../../services/booking.service';
import { Booking } from '../../models/booking';
import { FormGroup, FormBuilder } from '@angular/forms';

@Component({
    selector: 'app-booking',
    templateUrl: './booking.component.html'
})
export class BookingComponent implements OnInit {

  @Input() mode: string;
  @Input() id: number;
  form: FormGroup;
  booking: Booking;

  constructor(
    private bookingService: BookingService,
    private actRoute: ActivatedRoute,
    private authService: AuthService,
    private router: Router,
    private fb: FormBuilder
  ) {
  }

  authChangedSubscription: any;

  ngOnInit() {
    this.resetData();
    this.authChangedSubscription = this.authService.AuthChanged.subscribe(() => this.resetData());
  }

  ngOnDestroy() {
    this.authChangedSubscription.unsubscribe();
  };

  resetData() {
    if (this.mode == "edit" || this.mode == "view") {
      this.bookingService.getBooking(this.id).subscribe((response: Booking) => {
        console.log(response);
        this.booking = response;
        if (this.mode == "edit") {
          this.form = this.fb.group({
            startTime: this.booking.startTime,
            endTime: this.booking.endTime,
            description: this.booking.note
          });
        }
      }, error => { this.router.navigate(['/error']); });
    }
  }

  onSubmit() {
    // TODO: Use EventEmitter with form value
    console.warn(this.form.value);
  }
}
