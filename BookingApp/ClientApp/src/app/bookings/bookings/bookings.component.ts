import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Logger } from '../../services/logger.service';
import { AuthService } from '../../services/auth.service';
import { BookingService } from '../../services/booking.service';
import { Booking } from '../../models/booking';
import { ResourceTimeWindow, ResourceTimeWindowType } from '../../models/resource-time-window'
import { ResourceService } from '../../services/resource.service';
import { RuleService } from '../../services/rule.service';
import { Resource } from '../../models/resource';
import { rule } from '../../models/rule';
import { BookingsModeService, BookingsComponentMode } from '../../services/bookings-component-mode.service';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-bookings',
  templateUrl: './bookings.component.html',
  styleUrls: ['./bookings.component.css']
})
export class BookingsComponent implements OnInit {
  //For modes
  public modeType = BookingsComponentMode;
  public mode;
  //For resource mode
  public ResourceTimeWindowT = ResourceTimeWindowType;
  resourceTimeWindows: ResourceTimeWindow[];
  resourceId: number;
  serviceTime: number;
  //For user mode
  userId: string;
  //Bookings list
  bookings: Booking[];
  //Time range
  startTime: Date;
  endTime: Date;

  constructor(
    private bookingsConfigService: BookingsModeService,
    private bookingService: BookingService,
    private resourceService: ResourceService,
    private userService: UserService,
    private ruleService: RuleService,
    private actRoute: ActivatedRoute,
    private authService: AuthService,
    private router: Router
  ) {
    this.mode = bookingsConfigService.currentMode;
  }

  authChangedSubscription: any;

  ngOnInit() {
    this.startTime = new Date(Date.now());
    this.endTime = null;
    this.resourceId = this.bookingsConfigService.resourceId;
    this.userId = this.bookingsConfigService.userId;
    this.mode = this.bookingsConfigService.currentMode;
    this.resetData();
    this.authChangedSubscription = this.authService.AuthChanged.subscribe(() => this.resetData());
  }

  ngOnDestroy() {
    this.authChangedSubscription.unsubscribe();
  };

  resetData() {
    switch (this.mode) {
      case BookingsComponentMode.Admin:
        this.bookingService.getBookings().subscribe((response: Booking[]) => {
          console.log(response);
          this.bookings = response;
        }, error => { this.router.navigate(['/error']); });
        break;
      case BookingsComponentMode.User:
        this.userService.getBookings(this.userId, this.startTime, this.endTime).subscribe((response: Booking[]) => {
          console.log(response);
          this.bookings = response;
        }, error => { this.router.navigate(['/error']); });
        break;
      case BookingsComponentMode.Resource:
        this.bookingService.getBookingOfResource(3).subscribe((responseBookings: Booking[]) => {
          this.resourceService.getResource(this.resourceId).subscribe((response: Resource) => {
            this.ruleService.getRule(response.ruleId).subscribe((response: rule) => {
              this.serviceTime = response.serviceTime;
              this.endTime = new Date(Date.now());
              this.endTime.setTime(this.endTime.getTime() + response.preOrderTimeLimit * 60 * 1000);
              console.log(responseBookings);
              this.bookings = responseBookings.sort((a, b) => {
                return a.startTime > b.startTime ? 1 : -1;
              });
              this.genResourceTimeWindows();
            }, error => { this.router.navigate(['/error']); });
          }, error => { this.router.navigate(['/error']); });
        }, error => { this.router.navigate(['/error']); });
        Logger.log("BookingsComponentMode " + this.mode);
        break;
      default:
        //Never reach
        Logger.log("BookingComponent: invalid mode");
    }
  };

  genResourceTimeWindows() {
    this.resourceTimeWindows = new Array();
    for (var i = 0; i < this.bookings.length; i++) {
      var bookingTimeWindow = new ResourceTimeWindow;
      var serviceTimeWindow = new ResourceTimeWindow;
      bookingTimeWindow.type = ResourceTimeWindowType.Booked;
      bookingTimeWindow.startTime = new Date(this.bookings[i].startTime);
      bookingTimeWindow.endTime = new Date(this.bookings[i].endTime);
      bookingTimeWindow.booking = this.bookings[i];
      this.resourceTimeWindows.push(bookingTimeWindow);
      Logger.log(this.bookings[i]);
      serviceTimeWindow.type = ResourceTimeWindowType.ServiceTime;
      serviceTimeWindow.startTime = new Date(this.bookings[i].startTime);
      serviceTimeWindow.endTime = new Date(this.bookings[i].startTime);
      Logger.log(serviceTimeWindow);
      serviceTimeWindow.endTime.setTime(serviceTimeWindow.endTime.getTime() + this.serviceTime * 60 * 1000);
      this.resourceTimeWindows.push(serviceTimeWindow);

      if (i == this.bookings.length - 1) {
        let freeTimeWindowSize = this.endTime.getTime() - serviceTimeWindow.endTime.getTime();
        if (freeTimeWindowSize > 0) {
          var freeTimeWindow = new ResourceTimeWindow;
          freeTimeWindow.type = ResourceTimeWindowType.Free;
          freeTimeWindow.startTime = new Date(serviceTimeWindow.endTime);
          freeTimeWindow.endTime = new Date(this.endTime);
          this.resourceTimeWindows.push(freeTimeWindow);
        }
      } else {
        let freeTimeWindowSize = new Date(this.bookings[i + 1].startTime).getTime();
        freeTimeWindowSize = freeTimeWindowSize - serviceTimeWindow.endTime.getTime();
        if (freeTimeWindowSize > 0) {
          var freeTimeWindow = new ResourceTimeWindow;
          freeTimeWindow.type = ResourceTimeWindowType.Free;
          freeTimeWindow.startTime = new Date(serviceTimeWindow.endTime);
          freeTimeWindow.endTime = new Date(this.bookings[i + 1].startTime);
          this.resourceTimeWindows.push(freeTimeWindow);
        }
      }
    }

  };
}
