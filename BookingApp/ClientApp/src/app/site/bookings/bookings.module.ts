import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MaterialModule } from '../../material/material.module';
import { DatePipe } from '@angular/common'

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
    MaterialModule,
  ],
  exports: [
    BookingComponent,
    BookingsComponent
  ],
  providers: [DatePipe],
  bootstrap: []
})

export class BookingsModule {
}
