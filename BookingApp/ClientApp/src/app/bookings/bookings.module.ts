import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { BookingComponent } from './booking/booking.component';
import { BookingsComponent } from './bookings/bookings.component';

@NgModule({
  declarations: [
    BookingComponent,
    BookingsComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
  ],
  exports: [
    BookingComponent,
    BookingsComponent
  ],
  providers: [],
  bootstrap: []
})

export class BookingsModule {
}
