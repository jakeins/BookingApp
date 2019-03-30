import { Component, OnInit, Input, NgModule } from '@angular/core';

import { ResourceService } from '../../services/resource.service';
import { Resource } from '../../models/resource';
import { ActivatedRoute, Router } from '@angular/router';
import { Logger } from '../../services/logger.service';
import { AuthService } from '../../services/auth.service';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import { BookingsModeService, BookingsComponentMode } from '../../services/bookings-component-mode.service';
import { BookingsComponent } from '../../bookings/bookings/bookings.component';

@NgModule({
  declarations: [
    BookingsComponent
  ]
})
@Component({
  selector: 'app-resource',
  templateUrl: './resource.component.html',
  styleUrls: ['./resource.component.css']
})
export class ResourceComponent implements OnInit {

  resource: Resource;
  id: number;
  selectedBookings: BookingsComponent;
  loading: boolean;

  constructor(
    private resourceService: ResourceService,
    private bookingsModeService: BookingsModeService,
    private actRoute: ActivatedRoute,
    private authService: AuthService,
    private router: Router
  ) {
  }

  authChangedSubscription: any;

  ngOnInit() {
    this.loading = true;
    this.actRoute.params.subscribe(params => {this.id = +params['id'];});
    this.bookingsModeService.currentMode = BookingsComponentMode.Resource;
    this.bookingsModeService.resourceId = this.id;
    this.resetData();

    this.authChangedSubscription = this.authService.AuthChanged.subscribe(() => this.resetData());
  }

  ngOnDestroy() {
    this.authChangedSubscription.unsubscribe();
  };

  resetData() {
    this.resourceService.getResource(this.id).subscribe((response: Resource) => {
      this.resource = response;
    }, error => { this.router.navigate(['/error']); });
  }
}
