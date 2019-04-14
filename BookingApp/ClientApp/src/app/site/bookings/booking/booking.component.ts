import { Component, OnInit, Input, Inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Logger } from '../../../services/logger.service';
import { AuthService } from '../../../services/auth.service';
import { BookingService } from '../../../services/booking.service';
import { Booking } from '../../../models/booking';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
    selector: 'app-booking',
  templateUrl: './booking.component.html',
  styleUrls: ['./booking.component.css']
})
export class BookingComponent implements OnInit {

  form: FormGroup;
  booking: Booking;
  datePatern = "";
  mode: string;
  id: number;

  constructor(
    private bookingService: BookingService,
    private actRoute: ActivatedRoute,
    private authService: AuthService,
    private router: Router,
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<BookingComponent>,
    @Inject(MAT_DIALOG_DATA) public args: any,
  ) {
    this.mode = args.mode;
    this.id = args.id;
  }

  authChangedSubscription: any;

  ngOnInit() {
    this.form = this.fb.group({
      startTime: [0, Validators.compose([Validators.required])],
      endTime: [0, Validators.compose([Validators.required])],
      description: ['', Validators.compose([Validators.required, Validators.minLength(4), Validators.maxLength(64), Validators.pattern("[a-zA-Z0-9 ]*")])]
    });
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
          this.form.setValue({
            startTime: response.startTime,
            endTime: response.endTime,
            description: response.note
          });
        }
      }, error => { this.router.navigate(['/error']); });
    }
  }

  onSubmit() {
    // TODO: Use EventEmitter with form value
    console.warn(this.form.value);
  }

  //field getters
  description() {
    return this.form.get('description');
  }

  startTime() {
    return this.form.get('startTime');
  }

  endTime() {
    return this.form.get('endTime');
  }
}
