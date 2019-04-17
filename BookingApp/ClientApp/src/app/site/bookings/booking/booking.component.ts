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
import { NotificationService } from '../../../services/notification.service';
import { UserInfoService } from '../../../services/user-info.service';
import { UserService } from '../../../services/user.service';
import { User } from '../../../models/user';

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
  
  startDateTime: Date;
  endDateTime: Date;
  startTimeField: Date;

  //Error controling
  error: any;

  //For slider
  min: number;
  max: number;
  step: number;

  users: Array<User>;
  selectedUserId: string;

  constructor(
    private bookingService: BookingService,
    private resourceService: ResourceService,
    private ruleService: RuleService,
    private actRoute: ActivatedRoute,
    private authService: AuthService,
    private router: Router,
    private fb: FormBuilder,
    private userInfoService: UserInfoService,
    private userService: UserService,
    public dialogRef: MatDialogRef<BookingComponent>,
    public datepipe: DatePipe,
    public notificationService:NotificationService,
    @Inject(MAT_DIALOG_DATA) public args: any,
  ) {
    this.mode = args.mode;
    this.id = args.id;
    this.startDateTime = args.startTime;
    this.endDateTime = args.endTime;
  }

  authChangedSubscription: any;

  ngOnInit() {
    this.form = this.fb.group({
      startDate: [0, Validators.compose([Validators.required])],
      startTime: [0, Validators.compose([Validators.required])],
      current: [0, Validators.compose([Validators.required])],
      description: ['', Validators.compose([Validators.required, Validators.minLength(4), Validators.maxLength(512)])]
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
            if (this.userInfoService.isAdmin) {
              this.userService.getUsers().subscribe((users: User[]) => {
                this.users = users;
                this.min = thisRule.minTime;
                this.max = thisRule.maxTime;
                this.step = thisRule.stepTime;
                
                this.selectedUserId = this.userInfoService.userId;
                this.initializeForm();
              }, error => { this.router.navigate(['/error']); });
            }
            else {
              this.min = thisRule.minTime;
              this.max = thisRule.maxTime;
              this.step = thisRule.stepTime;
              this.initializeForm();
            }
          }, error => { this.router.navigate(['/error']); });
        }, error => { this.router.navigate(['/error']); });
      }
  }

  initializeForm() {
    if (this.mode == "edit") {
      this.startTimeField = new Date(this.booking.startTime);
      this.form.setValue({
        startDate: this.booking.startTime,
        startTime: this.datepipe.transform(this.startTimeField, "shortTime"),
        description: this.booking.note,
        current: Math.round((((new Date(this.booking.endTime)).getTime()) - new Date(this.booking.startTime).getTime()) / (1000 * 60))
      });
    } else
      if (this.mode == "create") {
        this.startTimeField = new Date(this.startDateTime);
        this.form.setValue({
          startDate: this.startDateTime,
          startTime: this.datepipe.transform(this.startTimeField, "shortTime"),
          current: this.min,
          description: ""
        });
      }
  }

  parseTime(t: any) {
    let d = new Date();
    let time = t.match(/(\d+)(?::(\d\d))?\s*(p?)/);
    d.setHours(parseInt(time[1]) + (time[3] ? 12 : 0));
    d.setMinutes(parseInt(time[2]) || 0);
    return d;
  }

  convertToDateTime(date: Date, time: Date) {
    let res: Date;
    res = new Date(date);
    res.setHours(0, 0, 0, 0);
    let timeDate = this.parseTime(time);
    res = new Date(res.getTime() + timeDate.getHours() * 60 * 60 * 1000 + timeDate.getMinutes() * 60 * 1000);
    return res;
  }

  onSubmit() {
    if (this.mode == "edit") {
      if (this.form.valid) {
        let newValue: Booking;
        newValue = new Booking();
        newValue.id = this.booking.id;
        newValue.startTime = this.convertToDateTime(this.startDate().value, this.startTime().value);
        newValue.endTime = new Date(newValue.startTime.getTime() + this.current().value * 60 * 1000);
        newValue.note = this.description().value;
          
        this.bookingService.updateBooking(newValue).subscribe(
          () => {
            this.onClose();
            this.notificationService.submit('Edited successfully!');
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
        newValue.startTime = this.convertToDateTime(this.startDate().value, this.startTime().value);
        newValue.endTime = new Date(newValue.startTime.getTime() + this.current().value * 60 * 1000);
        newValue.note = this.description().value;
        if (this.userInfoService.isAdmin)
          newValue.createdUserId = this.selectedUserId;
        this.bookingService.createBooking(newValue).subscribe(
          () => {
            this.onClose();
            this.notificationService.success('Created successfully!');
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

  startDate() {
    return this.form.get('startDate');
  }

  startTime() {
    return this.form.get('startTime');
  }

  current() {
    return this.form.get('current');
  }
}
