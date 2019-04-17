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
  dataSource: MatTableDataSource<Booking>;
  @ViewChild(MatPaginator) paginator: MatPaginator;

  //For resource mode
  public ResourceTimeWindowT = ResourceTimeWindowType;
  resourceTimeWindows: ResourceTimeWindow[];
  resourceTimeWindowsdataSource: MatTableDataSource<ResourceTimeWindow>;

  isConfiguredSelector: boolean;

  serviceTime: number;

  error: any;

  //Bookings list
  bookings: Booking[];
  //Time range
  rangeselector: FormGroup;
  startTimeValue: Date;
  endTimeValue: Date;

  //For table
  displayedColumns: any;

  currentTime: Date;

  constructor(
    private bookingService: BookingService,
    private resourceService: ResourceService,
    private userService: UserService,
    private userInfoService: UserInfoService,
    private ruleService: RuleService,
    private actRoute: ActivatedRoute,
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
    this.isConfiguredSelector = true;
    this.rangeselector = this.fb.group({
      startDate: [0, Validators.compose([Validators.required])],
      startTime: [0, Validators.compose([Validators.required])],
      endDate: [0, Validators.compose([Validators.required])],
      endTime: [0, Validators.compose([Validators.required])],
    });
    this.startTimeValue = new Date(Date.now());
    this.endTimeValue = null;
    this.resetData();
    this.authChangedSubscription = this.authService.AuthChanged.subscribe(() => this.resetData());
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
    this.isConfiguredSelector = false;
    this.rangeselector.setValue({
      startDate: this.startTimeValue,
      startTime: this.datepipe.transform(this.startTimeValue, "shortTime"),
      endDate: this.endTimeValue,
      endTime: this.datepipe.transform(this.endTimeValue, "shortTime"),
    });
    this.rangeselector.valueChanges.subscribe(val => {
      this.startTimeValue = this.convertToDateTime(this.startDate().value, this.startTime().value);
      this.endTimeValue = this.convertToDateTime(this.endDate().value, this.endTime().value);
      this.resetData();
    });
  }

  resetData() {
    this.currentTime = new Date();
    switch (this.mode) {
      case "admin":
        this.bookingService.getBookings(this.startTimeValue, this.endTimeValue).subscribe((bookingsRaw: Booking[]) => {
          if (bookingsRaw.length != 0) {
            let userIds = new Array();
            for (let i = 0; i < bookingsRaw.length; i++) {
              if(!userIds.includes(bookingsRaw[i].createdUserId))
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
                if (this.isConfiguredSelector)
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
            if (this.isConfiguredSelector)
              this.configureRangeSelector();
          }
        }, err => {
          this.error = err.status + ': ' + err.error.Message + '.';
        });
        break;
      case "user":
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
            if (this.isConfiguredSelector)
              this.configureRangeSelector();
          }, err => {
            this.error = err.status + ': ' + err.error.Message + '.';
          });
        }, err => {
          this.error = err.status + ': ' + err.error.Message + '.';
        });
        break;
      case "res":
        ////TODO: bug or feture
        //this.endTimeValue = undefined;
        this.bookingService.getBookingOfResource(this.resourceId, this.startTimeValue, this.endTimeValue).subscribe((responseBookings: Booking[]) => {
          this.resourceService.getResource(this.resourceId).subscribe((response: Resource) => {
            this.ruleService.getRule(response.ruleId).subscribe((response: rule) => {
              let userIds = new Array();
              for (let i = 0; i < responseBookings.length; i++) {
                if (!userIds.includes(responseBookings[i].createdUserId))
                  userIds.push(responseBookings[i].createdUserId);
              }
              if (this.userInfoService.isAdmin) {
                this.userService.getUsersById(userIds).subscribe((users: User[]) => {
                  this.serviceTime = response.serviceTime;
                  this.endTimeValue = new Date(Date.now());
                  this.endTimeValue.setTime(this.endTimeValue.getTime() + response.preOrderTimeLimit * 60 * 1000);
                  this.bookings = responseBookings.sort((a, b) => {
                    return a.startTime > b.startTime ? 1 : -1;
                  });
                  if (this.isConfiguredSelector)
                    this.configureRangeSelector();
                  this.genResourceTimeWindows();
                  this.displayedColumns = ['startTimeT', 'endTimeT', 'terminationTimeT', 'userT', 'noteT', 'btns'];
                  this.resourceTimeWindowsdataSource = new MatTableDataSource<ResourceTimeWindow>(this.resourceTimeWindows);
                  this.resourceTimeWindowsdataSource.paginator = this.paginator;
                }, err => {
                  this.error = err.status + ': ' + err.error.Message + '.';
                });
              }
              else {
                this.serviceTime = response.serviceTime;
                this.endTimeValue = new Date(Date.now());
                this.endTimeValue.setTime(this.endTimeValue.getTime() + response.preOrderTimeLimit * 60 * 1000);
                this.bookings = responseBookings.sort((a, b) => {
                  return a.startTime > b.startTime ? 1 : -1;
                });
                if (this.isConfiguredSelector)
                  this.configureRangeSelector();
                this.genResourceTimeWindows();
                this.displayedColumns = ['startTimeT', 'endTimeT'];
                this.resourceTimeWindowsdataSource = new MatTableDataSource<ResourceTimeWindow>(this.resourceTimeWindows);
                this.resourceTimeWindowsdataSource.paginator = this.paginator;
              }
            }, err => {
              this.error = err.status + ': ' + err.error.Message + '.';
            });
          }, err => {
            this.error = err.status + ': ' + err.error.Message + '.';
          });
        }, err => {
          this.error = err.status + ': ' + err.error.Message + '.';
        });
        break;
      default:
        //Never reach
        Logger.log("BookingComponent: invalid mode");
    }
  };

  genResourceTimeWindows() {
    this.resourceTimeWindows = new Array();
    if (this.bookings.length != 0) {
      for (let i = 0; i < this.bookings.length; i++) {
        let bookingTimeWindow = new ResourceTimeWindow;
        let serviceTimeWindow = new ResourceTimeWindow;
        if (this.userInfoService.isUser && this.bookings[i].createdUserId == this.userInfoService.userId) {
          bookingTimeWindow.type = ResourceTimeWindowType.My;
        } else {
          bookingTimeWindow.type = ResourceTimeWindowType.Booked;
        }
        bookingTimeWindow.startTime = new Date(this.bookings[i].startTime);
        bookingTimeWindow.endTime = new Date(this.bookings[i].endTime);
        if (this.userInfoService.isUser && this.bookings[i].createdUserId == this.userInfoService.userId || this.userInfoService.isAdmin) {
          bookingTimeWindow.booking = this.bookings[i];
        }
        else {
          bookingTimeWindow.booking = null;
        }
        this.resourceTimeWindows.push(bookingTimeWindow);

        if (this.serviceTime != 0) {
          serviceTimeWindow.type = ResourceTimeWindowType.ServiceTime;
          serviceTimeWindow.startTime = new Date(this.bookings[i].endTime);
          serviceTimeWindow.endTime = new Date(this.bookings[i].endTime);
          serviceTimeWindow.endTime.setTime(serviceTimeWindow.endTime.getTime() + this.serviceTime * 60 * 1000);
          this.resourceTimeWindows.push(serviceTimeWindow);
        }

        var freeTimeWindow = new ResourceTimeWindow;
        freeTimeWindow.type = ResourceTimeWindowType.Free;
        let freeTimeWindowSize = 0;
        if (this.serviceTime != 0) {
          freeTimeWindow.startTime = new Date(serviceTimeWindow.endTime);
        }
        else {
          freeTimeWindow.startTime = new Date(this.bookings[i].endTime);
        }
        if (i == this.bookings.length - 1) {
          freeTimeWindowSize = this.endTimeValue.getTime();
        }
        else {
          freeTimeWindowSize = new Date(this.bookings[i + 1].startTime).getTime();
        }
        freeTimeWindowSize -= freeTimeWindow.startTime.getTime();
        if (i == this.bookings.length - 1) {
          if (freeTimeWindowSize > 0) {
            freeTimeWindow.endTime = new Date(this.endTimeValue);
            this.resourceTimeWindows.push(freeTimeWindow);
          }
        } else {
          if (freeTimeWindowSize > 0) {
            freeTimeWindow.endTime = new Date(this.bookings[i + 1].startTime);
            this.resourceTimeWindows.push(freeTimeWindow);
          }
        }

      }
    }
    else {
      var freeTimeWindow = new ResourceTimeWindow;
      freeTimeWindow.type = ResourceTimeWindowType.Free;
      freeTimeWindow.startTime = this.startTimeValue;
      freeTimeWindow.endTime = this.endTimeValue;
      this.resourceTimeWindows.push(freeTimeWindow);
    }
  };

  onCreate(startTime: Date) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.width = "60%";
    //dialogConfig.height = "60%";
    dialogConfig.data = { mode: "create", id: this.resourceId, startTime: startTime };
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
    //dialogConfig.height = "60%";
    dialogConfig.data = { id: id, mode: "edit" };
    const dialogRef = this.dialog.open(BookingComponent, dialogConfig);
    dialogRef.afterClosed().subscribe(res => {
      this.resetData();
    })
  }

  onDelete(rowId: number) {
    if (confirm('Are u sure to delete rule')) {
      this.bookingService.terminateBooking(rowId).subscribe(res => {
        this.notificationService.delete('Deleted successfully!');
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
    return ((new Date(startTime)).getTime() < this.currentTime.getTime()) &&
      ((new Date(endTime)).getTime() > this.currentTime.getTime());
  }

  isBefore(endTime: Date): boolean {
    return ((new Date(endTime)).getTime() < this.currentTime.getTime());
  }

  isAfter(startTime: Date): boolean {
    return ((new Date(startTime)).getTime() > this.currentTime.getTime());
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
        {
          return "lightcoral";
        }
      case this.ResourceTimeWindowT.ServiceTime:
        {
          return "gainsboro";
        }
      case this.ResourceTimeWindowT.Free:
        {
          return "palegreen";
        }
      case this.ResourceTimeWindowT.My:
        {
          return "lightblue";
        }
    }
    return "";
  }
}
