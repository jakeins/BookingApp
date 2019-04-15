import { Component, OnInit, Input, Inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Logger } from '../../../services/logger.service';
import { AuthService } from '../../../services/auth.service';
import { BookingService } from '../../../services/booking.service';
import { Booking } from '../../../models/booking';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { DatePipe } from '@angular/common';
import { ResourceService } from '../../../services/resource.service';
import { RuleService } from '../../../services/rule.service';
import { Resource } from '../../../models/resource';
import { rule } from '../../../models/rule';

@Component({
    selector: 'app-booking',
  templateUrl: './booking.component.html',
  styleUrls: ['./booking.component.css']
})
export class BookingComponent implements OnInit {

  form: FormGroup;
  booking: Booking;

  mode: string;
  id: number;
  time: Date;

  //Error controling
  error: any;

  //For slider
  min: number;
  max: number;
  step: number;
  current: number;

  constructor(
    private bookingService: BookingService,
    private resourceService: ResourceService,
    private ruleService: RuleService,
    private actRoute: ActivatedRoute,
    private authService: AuthService,
    private router: Router,
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<BookingComponent>,
    public datepipe: DatePipe,
    @Inject(MAT_DIALOG_DATA) public args: any,
  ) {
    this.mode = args.mode;
    this.id = args.id;
    this.time = args.startTime;
  }

  authChangedSubscription: any;

  ngOnInit() {
    this.form = this.fb.group({
      startTime: [0, Validators.compose([Validators.required])],
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
        this.resourceService.getResource(response.resourceId).subscribe((resource: Resource) => {
          this.ruleService.getRule(resource.ruleId).subscribe((thisRule: rule) => {
            console.log(response);
            this.booking = response;
            this.min = thisRule.minTime;
            this.max = thisRule.maxTime;
            this.step = thisRule.stepTime;

            this.initializeForm();
          }, error => { this.router.navigate(['/error']); });
        }, error => { this.router.navigate(['/error']); });
      }, error => { this.router.navigate(['/error']); });
    }
    else
      if (this.mode == "create") {
        this.resourceService.getResource(this.id).subscribe((resource: Resource) => {
          this.ruleService.getRule(resource.ruleId).subscribe((thisRule: rule) => {
            this.min = thisRule.minTime;
            this.max = thisRule.maxTime;
            this.step = thisRule.stepTime;
          }, error => { this.router.navigate(['/error']); });
        }, error => { this.router.navigate(['/error']); });
      }
  }

  initializeForm() {
    if (this.mode == "edit") {
      this.form.setValue({
        startTime: this.datepipe.transform(this.booking.startTime, 'yyyy-MM-ddThh:mm'),
        description: this.booking.note
      });
      this.current = Math.round(((new Date(this.booking.endTime)).getTime() - (new Date(this.booking.startTime)).getTime()) / 1000);
    } else
      if (this.mode == "create") {
        this.current = this.min;
        this.form.setValue({
          startTime: this.datepipe.transform(this.time, 'yyyy-MM-ddThh:mm')
        });
      }
  }

  onSubmit() {
    if (this.mode == "edit") {
      if (this.form.valid) {
        let newValue: Booking;
        newValue = new Booking();
        newValue.id = this.booking.id;
        newValue.startTime = new Date(this.startTime().value);
        newValue.endTime = new Date(newValue.startTime.getTime() + this.current * 1000);
        newValue.note = this.description().value;

        this.bookingService.updateBooking(newValue).subscribe(
          () => {
            this.onClose();
            //this.notificationService.submit('Submitted successfully!');
          },
          err => {
            this.error = err.status + ': ' + err.error.Message + '.';
          });
      }
      console.warn(this.form.value);
    }
    else
      if (this.mode = "create") {
        let newValue: Booking;
        newValue = new Booking();
        newValue.resourceId = this.id;
        newValue.startTime = new Date(this.startTime().value);
        newValue.endTime = new Date(newValue.startTime.getTime() + this.current * 1000);
        newValue.note = this.description().value;

        this.bookingService.createBooking(newValue).subscribe(
          () => {
            this.onClose();
            //this.notificationService.submit('Submitted successfully!');
          },
          err => {
            this.error = err.status + ': ' + err.error.Message + '.';
          });
      }
  }

  onClear() {
    this.error = null;
    this.onReset();
  }

  onCancel() {
    this.error = null;
    this.dialogRef.close();
  }

  onClose() {
    this.onReset();
    this.dialogRef.close();
  }

  onReset() {
    this.form.reset();
    this.initializeForm();
    Object.keys(this.form.controls).forEach(key => {                //reset form validation errors
      this.form.controls[key].setErrors(null)
    });
    this.form.clearValidators();
  }

  //field getters
  description() {
    return this.form.get('description');
  }

  startTime() {
    return this.form.get('startTime');
  }
}
