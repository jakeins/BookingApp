import { Component, OnInit, Input, ViewEncapsulation } from '@angular/core';
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
import { MatDialogConfig, MatDialog } from '@angular/material';
import { BookingComponent } from '../booking/booking.component';

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

  //For resource mode
  public ResourceTimeWindowT = ResourceTimeWindowType;
  resourceTimeWindows: ResourceTimeWindow[];
 
  serviceTime: number;
  
  //Bookings list
  bookings: Booking[];
  //Time range
  startTime: Date;
  endTime: Date;

  constructor(
    private bookingService: BookingService,
    private resourceService: ResourceService,
    private userService: UserService,
    private userInfoService: UserInfoService,
    private ruleService: RuleService,
    private actRoute: ActivatedRoute,
    private authService: AuthService,
    private dialog: MatDialog,
    private router: Router
  ) {
  }

  authChangedSubscription: any;

  ngOnInit() {
    this.startTime = new Date(Date.now());
    this.endTime = null;
    this.resetData();
    this.authChangedSubscription = this.authService.AuthChanged.subscribe(() => this.resetData());
  }

  ngOnDestroy() {
    this.authChangedSubscription.unsubscribe();
  };

  resetData() {
    switch (this.mode) {
      case "admin":
        this.bookingService.getBookings(this.startTime, this.endTime).subscribe((bookingsRaw: Booking[]) => {
          for (var booking of bookingsRaw) {
            this.userService.getUserById(booking.createdUserId).subscribe((response: User) => {
              booking.createdUserId = response.userName;
            }, error => { this.router.navigate(['/error']); });
          }
          this.bookings = bookingsRaw;
        }, error => { this.router.navigate(['/error']); });
        break;
      case "user":
        this.userService.getBookings(this.userId, this.startTime, this.endTime).subscribe((response: Booking[]) => {
          this.bookings = response;
        }, error => { this.router.navigate(['/error']); });
        break;
      case "res":
        //TODO: bug or fueture
        this.endTime = undefined;
        this.bookingService.getBookingOfResource(this.resourceId, this.startTime, this.endTime).subscribe((responseBookings: Booking[]) => {
          this.resourceService.getResource(this.resourceId).subscribe((response: Resource) => {
            this.ruleService.getRule(response.ruleId).subscribe((response: rule) => {
              this.serviceTime = response.serviceTime;
              this.endTime = new Date(Date.now());
              this.endTime.setTime(this.endTime.getTime() + response.preOrderTimeLimit * 60 * 1000);
              this.bookings = responseBookings.sort((a, b) => {
                return a.startTime > b.startTime ? 1 : -1;
              });
              this.genResourceTimeWindows();
            }, error => { this.router.navigate(['/error']); });
          }, error => { this.router.navigate(['/error']); });
        }, error => { this.router.navigate(['/error']); });
        break;
      default:
        //Never reach
        Logger.log("BookingComponent: invalid mode");
    }
  };

  genResourceTimeWindows() {
    this.resourceTimeWindows = new Array();
    if (this.bookings.length != 0) {
      for (var i = 0; i < this.bookings.length; i++) {
        var bookingTimeWindow = new ResourceTimeWindow;
        var serviceTimeWindow = new ResourceTimeWindow;
        bookingTimeWindow.type = ResourceTimeWindowType.Booked;
        bookingTimeWindow.startTime = new Date(this.bookings[i].startTime);
        bookingTimeWindow.endTime = new Date(this.bookings[i].endTime);
        bookingTimeWindow.booking = this.bookings[i];
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
          freeTimeWindowSize = this.endTime.getTime();
        }
        else {
          freeTimeWindowSize = new Date(this.bookings[i + 1].startTime).getTime();
        }
        freeTimeWindowSize -= freeTimeWindow.startTime.getTime();
        if (i == this.bookings.length - 1) {
          if (freeTimeWindowSize > 0) {
            freeTimeWindow.endTime = new Date(this.endTime);
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
      freeTimeWindow.startTime = this.startTime;
      freeTimeWindow.endTime = this.endTime;
      this.resourceTimeWindows.push(freeTimeWindow);
    }
  };

  onCreate(startTime: Date) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.width = "60%";
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
    dialogConfig.data = { id: id, mode: "edit" };
    const dialogRef = this.dialog.open(BookingComponent, dialogConfig);
    dialogRef.afterClosed().subscribe(res => {
      this.resetData();
    })
  }

  onDelete(rowId: number) {
    if (confirm('Are u sure to delete rule')) {
      this.bookingService.deleteBooking(rowId).subscribe(res => {
        this.resetData();
      });
    }
  }
}
