import { Component, OnInit, Input, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Logger } from '../../../services/logger.service';
import { AuthService } from '../../../services/auth.service';
import { BookingService } from '../../../services/booking.service';
import { Booking } from '../../../models/booking';
import { ResourceTimeWindow, ResourceTimeWindowType } from '../../../models/resource-time-window'
import { ResourceService } from '../../../services/resource.service';
import { RuleService } from '../../../services/rule.service';
import { Resource } from '../../../models/resource';
import { rule } from '../../../models/rule';
import { UserService } from '../../../services/user.service';
import { User } from '../../../models/user';
import { forEach } from '@angular/router/src/utils/collection';
import { UserInfoService } from '../../../services/user-info.service';
import { MatDialogConfig, MatDialog, MatTableDataSource, MatPaginator } from '@angular/material';
import { BookingComponent } from '../booking/booking.component';
import { NotificationService } from '../../../services/notification.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-bookings',
  templateUrl: './bookings.component.html',
  styleUrls: ['./bookings.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class BookingsComponent implements OnInit {
  //Inputs for modes
  @Input() mode: string;
  @Input() resourceId: number;
  @Input() userId: string;
  //Table data and paginator
  @ViewChild(MatPaginator) paginator: MatPaginator;
  dataSource: MatTableDataSource<Booking>;

  //For resource mode
  public ResourceTimeWindowT = ResourceTimeWindowType;
  resourceTimeWindows: ResourceTimeWindow[];
  resourceTimeWindowsdataSource: MatTableDataSource<ResourceTimeWindow>;
  //is range selector intilized
  firstLoadComplete: boolean;

  serviceTime: number;
  //error storage
  error: any;

  //Bookings list
  bookings: Booking[];
  //Time range
  rangeselector: FormGroup;
  startTimeValue: Date;
  endTimeValue: Date;
  preOrderTime: Date;
  currentTime: Date;
  minBookingTimeInMinutes: number;
  maxBookingTimeInMunutes: number;
  preOrderTimeInMinutes: number;

  //For table
  displayedColumns: any;


  constructor(
    private bookingService: BookingService,
    private resourceService: ResourceService,
    private userService: UserService,
    private userInfoService: UserInfoService,
    private ruleService: RuleService,
    private authService: AuthService,
    private dialog: MatDialog,
    private router: Router,
    private fb: FormBuilder,
    public datepipe: DatePipe,
    private notificationService: NotificationService
  ) {
  }

  authChangedSubscription: any;

  ngOnInit() {
    this.rangeselector = this.fb.group({
      startDate: [0, Validators.compose([Validators.required])],
      startTime: [0, Validators.compose([Validators.required])],
      endDate: [0, Validators.compose([Validators.required])],
      endTime: [0, Validators.compose([Validators.required])],
    });
    this.firstLoadComplete = false;
    this.startTimeValue = new Date(Date.now());
    this.endTimeValue = new Date(Date.now());
    this.rangeselector.setValue({
      startDate: this.startTimeValue,
      startTime: this.datepipe.transform(this.startTimeValue, "shortTime"),
      endDate: this.endTimeValue,
      endTime: this.datepipe.transform(this.endTimeValue, "shortTime"),
    });
    this.resetData();
    this.authChangedSubscription = this.authService.AuthChanged.subscribe(() => this.resetData());
    this.rangeselector.valueChanges.subscribe(() => {
      this.startTimeValue = this.convertToDateTime(this.startDate().value, this.startTime().value);
      this.endTimeValue = this.convertToDateTime(this.endDate().value, this.endTime().value);
      this.resetData();
    });
  }

  ngOnDestroy() {
    this.authChangedSubscription.unsubscribe();
  };

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

  configureRangeSelector() {
    this.firstLoadComplete = true;
    this.rangeselector.setValue({
      startDate: this.startTimeValue,
      startTime: this.datepipe.transform(this.startTimeValue, "shortTime"),
      endDate: this.endTimeValue,
      endTime: this.datepipe.transform(this.endTimeValue, "shortTime"),
    });
  }

  resetDataAdmin() {
    this.preOrderTime = new Date();
    this.bookingService.getBookings(this.startTimeValue, this.endTimeValue).subscribe((bookingsRaw: Booking[]) => {
      if (bookingsRaw.length != 0) {
        let userIds = new Array();
        for (let i = 0; i < bookingsRaw.length; i++) {
          if (!userIds.includes(bookingsRaw[i].createdUserId))
            userIds.push(bookingsRaw[i].createdUserId);
        }
        this.resourceService.getResources().subscribe((resources: Resource[]) => {
          this.userService.getUsersById(userIds).subscribe((users: User[]) => {
            this.bookings = bookingsRaw;
            this.displayedColumns = ['idT', 'startTimeT', 'endTimeT', 'resourceIdT', 'noteT', 'userNameT', 'terminationTimeT', 'btns']
            this.dataSource = new MatTableDataSource<Booking>(this.bookings);
            this.dataSource.paginator = this.paginator;
            this.startTimeValue = new Date(Math.min.apply(Math, bookingsRaw.map(b => { return b.startTime })));
            this.endTimeValue = new Date(Math.max.apply(Math, bookingsRaw.map(b => { return b.endTime })));
            if (this.firstLoadComplete)
              this.configureRangeSelector();
          }, err => {
            this.error = err.status + ': ' + err.error.Message + '.';
          });
        }, err => {
          this.error = err.status + ': ' + err.error.Message + '.';
        });
      } else {
        this.bookings = bookingsRaw;
        this.startTimeValue = new Date();
        this.endTimeValue = new Date();
        if (this.firstLoadComplete)
          this.configureRangeSelector();
      }
    }, err => {
      this.error = err.status + ': ' + err.error.Message + '.';
    });
  }

  resetDataUser() {
    this.preOrderTime = new Date();
    this.userService.getBookings(this.userId, this.startTimeValue, this.endTimeValue).subscribe((response: Booking[]) => {
      this.resourceService.getResources().subscribe((resources: Resource[]) => {
        this.bookings = response;
        this.displayedColumns = ['startTimeT', 'endTimeT', 'resourceIdT', 'noteT', 'terminationTimeT', 'btns']
        this.dataSource = new MatTableDataSource<Booking>(this.bookings);
        this.dataSource.paginator = this.paginator;
        if (this.bookings.length != 0) {
          this.startTimeValue = new Date(Math.min.apply(Math, response.map(b => { return b.startTime })));
          this.endTimeValue = new Date(Math.max.apply(Math, response.map(b => { return b.endTime })));
        }
        else {
          this.startTimeValue = new Date();
          this.endTimeValue = new Date();
        }
        if (this.firstLoadComplete)
          this.configureRangeSelector();
      }, err => {
        this.error = err.status + ': ' + err.error.Message + '.';
      });
    }, err => {
      this.error = err.status + ': ' + err.error.Message + '.';
    });
  }

  resetDataResourcesProcessing(responseBookings: Booking[]) {
    this.bookings = responseBookings
      .filter((booking: Booking) => { return booking.terminationTime == null || (new Date(booking.terminationTime) > new Date(booking.startTime)) })
      .sort((a, b) => { return a.startTime > b.startTime ? 1 : -1; });
    if (!this.firstLoadComplete) {
      if (this.bookings.length != 0)
        this.endTimeValue = new Date(new Date(this.bookings[this.bookings.length - 1].endTime).getTime() + (this.maxBookingTimeInMunutes + this.preOrderTimeInMinutes + 5) * 60 * 1000);
      else
        this.endTimeValue = new Date(this.preOrderTime.getTime() + (this.maxBookingTimeInMunutes + 5) * 60 * 1000);
      this.configureRangeSelector();
    }
    this.genResourceTimeWindows();
    this.resourceTimeWindowsdataSource = new MatTableDataSource<ResourceTimeWindow>(this.resourceTimeWindows);
    this.resourceTimeWindowsdataSource.paginator = this.paginator;
  }

  resetBooking() {
    this.bookingService.getBookingOfResource(this.resourceId, this.startTimeValue, this.endTimeValue).subscribe((responseBookings: Booking[]) => {
      if (this.userInfoService.isAdmin) {
        let userIds = new Array();
        for (let i = 0; i < responseBookings.length; i++) {
          if (!userIds.includes(responseBookings[i].createdUserId))
            userIds.push(responseBookings[i].createdUserId);
        }
        this.userService.getUsersById(userIds).subscribe((users: User[]) => {
          this.displayedColumns = ['startTimeT', 'endTimeT', 'terminationTimeT', 'userT', 'noteT', 'btns'];
          this.resetDataResourcesProcessing(responseBookings);
        }, err => {
          this.error = err.status + ': ' + err.error.Message + '.';
        });
      }
      else {
        this.resetDataResourcesProcessing(responseBookings);
        if (this.userInfoService.isUser)
          this.displayedColumns = ['startTimeT', 'endTimeT', 'terminationTimeT', 'btns'];
        else
          this.displayedColumns = ['startTimeT', 'endTimeT', 'terminationTimeT'];
      }
    }, err => {
      this.error = err.status + ': ' + err.error.Message + '.';
    });
  }

  resetDataResources() {
    if (!this.firstLoadComplete) {
      this.resourceService.getResource(this.resourceId).subscribe((resource: Resource) => {
        this.ruleService.getRule(resource.ruleId).subscribe((ruleData: rule) => {
          this.preOrderTime = new Date(new Date().getTime() + (ruleData.preOrderTimeLimit + 4) * 60 * 1000);
          if (!this.firstLoadComplete) {
            this.currentTime = new Date();
            this.startTimeValue = this.currentTime;
            this.serviceTime = ruleData.serviceTime;
            this.endTimeValue = null;
            this.minBookingTimeInMinutes = ruleData.minTime;
            this.maxBookingTimeInMunutes = ruleData.maxTime;
            this.preOrderTimeInMinutes = ruleData.preOrderTimeLimit;
          }
          this.resetBooking();
        }, err => {
          this.error = err.status + ': ' + err.error.Message + '.';
        });
      }, err => {
        this.error = err.status + ': ' + err.error.Message + '.';
      });
    } else {
      this.resetBooking();
    }
  }

  resetData() {
    this.currentTime = new Date();
    switch (this.mode) {
      case "admin":
        this.resetDataAdmin();
        break;
      case "user":
        this.resetDataUser();
        break;
      case "res":
        this.resetDataResources();
        break;
      default:
        //Never reach
        Logger.log("BookingComponent: invalid mode");
    }
  };

  genFreeResourceTimeWindow(startTime: Date, endTime: Date) {
    if (this.userInfoService.isAdmin && endTime.getTime() < this.preOrderTime.getTime()) {
      if (endTime.getTime() - startTime.getTime() > 1 * 60 * 1000) {
        let lost: ResourceTimeWindow; lost = new ResourceTimeWindow; lost.type = ResourceTimeWindowType.Lost;
        lost.startTime = new Date(startTime);
        lost.endTime = new Date(endTime);
        this.resourceTimeWindows.push(lost);
      }
    } else
      if (this.preOrderTime.getTime() < startTime.getTime()) {
        if (endTime.getTime() - startTime.getTime() > this.minBookingTimeInMinutes * 60 * 1000) {
          let free: ResourceTimeWindow; free = new ResourceTimeWindow; free.type = ResourceTimeWindowType.Free;
          free.startTime = new Date(startTime);
          free.endTime = new Date(endTime);
          this.resourceTimeWindows.push(free);
        }
      }
      else {
        let lost: ResourceTimeWindow; lost = new ResourceTimeWindow; lost.type = ResourceTimeWindowType.Lost;
        let free: ResourceTimeWindow; free = new ResourceTimeWindow; free.type = ResourceTimeWindowType.Free;
        if (this.preOrderTime.getTime() - startTime.getTime() > 1 * 60 * 1000) {
          lost.startTime = new Date(startTime);
          lost.endTime = new Date(this.preOrderTime);
          if (this.userInfoService.isAdmin)
            this.resourceTimeWindows.push(lost);
        }
        free.startTime = new Date(lost.endTime.getTime() + 1 * 60 * 100);
        if (endTime.getTime() - free.startTime.getTime() > this.minBookingTimeInMinutes * 60 * 1000) {
          free.endTime = new Date(endTime);
          this.resourceTimeWindows.push(free);
        }
      }
  }

  genBookedResourceTimeWindows(i: number) {
    let bookingTimeWindow = new ResourceTimeWindow;

    if (this.userInfoService.isUser && this.bookings[i].createdUserId == this.userInfoService.userId) {
      bookingTimeWindow.type = ResourceTimeWindowType.My;
    } else {
      bookingTimeWindow.type = ResourceTimeWindowType.Booked;
    }
    bookingTimeWindow.startTime = new Date(this.bookings[i].startTime);

    if (this.bookings[i].terminationTime != null) {
      bookingTimeWindow.endTime == new Date(this.bookings[i].terminationTime);
    }
    else
      bookingTimeWindow.endTime = new Date(this.bookings[i].endTime);

    if (this.userInfoService.isUser && this.bookings[i].createdUserId == this.userInfoService.userId || this.userInfoService.isAdmin) {
      bookingTimeWindow.booking = this.bookings[i];
    }
    else {
      bookingTimeWindow.booking = null;
    }
    this.resourceTimeWindows.push(bookingTimeWindow);
  }

  genServiceResourceTimeWindow(i : number) : Date {
    let serviceTimeWindow = new ResourceTimeWindow;
    let endServiceTimeWindowDate: Date;
    if (this.serviceTime != 0) {
      serviceTimeWindow.type = ResourceTimeWindowType.ServiceTime;
      serviceTimeWindow.startTime = new Date(this.bookings[i].endTime);
      serviceTimeWindow.endTime = new Date(serviceTimeWindow.startTime.getTime() + this.serviceTime * 60 * 1000);
      endServiceTimeWindowDate = new Date(serviceTimeWindow.endTime.getTime() + 1 * 60 * 1000);
      this.resourceTimeWindows.push(serviceTimeWindow);
    } else {
      endServiceTimeWindowDate = new Date(new Date(this.bookings[i].endTime).getTime() + 1 * 60 * 1000);
    }
    return endServiceTimeWindowDate;
  }

  genResourceTimeWindows() {
    this.resourceTimeWindows = new Array<ResourceTimeWindow>();
    if (this.bookings.length != 0) {
      for (let i = 0; i < this.bookings.length; i++) {
        this.genBookedResourceTimeWindows(i);
        let endServiceTime = this.genServiceResourceTimeWindow(i);
        if (i != this.bookings.length - 1)
          this.genFreeResourceTimeWindow(endServiceTime, new Date(new Date(this.bookings[i + 1].startTime).getTime() + 1 * 60 * 1000));
        else
          this.genFreeResourceTimeWindow(endServiceTime, this.endTimeValue);
      }
    }
    else {
      this.genFreeResourceTimeWindow(this.startTimeValue, this.endTimeValue);
    }
  };

  onCreate(startTime: Date, endTime: Date) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.width = "60%";
    dialogConfig.data = {
      mode: "create",
      id: this.resourceId,
      startTime: startTime,
      availableMinutes: Math.ceil((new Date(endTime).getTime() - new Date(startTime).getTime()) / (60 * 1000))
    };
    const dialogRef = this.dialog.open(BookingComponent, dialogConfig);
    dialogRef.afterClosed().subscribe(res => {
      this.resetData();
    })
  }

  onEdit(id: number) {
    let dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.width = "60%";
    dialogConfig.data = { id: id, mode: "edit" };
    const dialogRef = this.dialog.open(BookingComponent, dialogConfig);
    dialogRef.afterClosed().subscribe(res => {
      this.resetData();
    })
  }

  onDelete(rowId: number) {
    if (confirm('Are u sure to cancel booking')) {
      this.bookingService.terminateBooking(rowId).subscribe(res => {
        this.notificationService.delete('Canceled successfully!');
        this.resetData();
      }, err => {
        this.error = err.status + ': ' + err.error.Message + '.';
      });
    }
  }

  //Field getters
  startDate() {
    return this.rangeselector.get('startDate');
  }

  startTime() {
    return this.rangeselector.get('startTime');
  }

  endDate() {
    return this.rangeselector.get('endDate');
  }

  endTime() {
    return this.rangeselector.get('endTime');
  }

  //Helpers
  isInRange(startTime: Date, endTime: Date): boolean {
    return ((new Date(startTime)).getTime() < this.preOrderTime.getTime()) &&
      ((new Date(endTime)).getTime() > this.preOrderTime.getTime());
  }

  isBefore(endTime: Date): boolean {
    return ((new Date(endTime)).getTime() < this.preOrderTime.getTime());
  }

  isAfter(startTime: Date): boolean {
    return ((new Date(startTime)).getTime() > this.preOrderTime.getTime());
  }

  isTerminated(terminationTime: Date): boolean {
    return terminationTime != null;
  }

  getUserName(userId: string) {
    if (this.userService.getUserCache(userId) != undefined)
      return this.userService.getUserCache(userId).userName;
    return userId;
  }

  getResourceNameById(id: number) {
    if (this.resourceService.getResourceCache(id) != undefined)
      return this.resourceService.getResourceCache(id).title;
    return id;
  }

  getBgColorForResourceRow(row: ResourceTimeWindow) {
    switch (row.type) {
      case this.ResourceTimeWindowT.Booked:
        return "lightcoral";
      case this.ResourceTimeWindowT.ServiceTime:
        return "gainsboro";
      case this.ResourceTimeWindowT.Free:
        return "palegreen";
      case this.ResourceTimeWindowT.My:
        return "lightblue";
      case this.ResourceTimeWindowT.Lost:
        return "gray";
    }
    return "";
  }
}
