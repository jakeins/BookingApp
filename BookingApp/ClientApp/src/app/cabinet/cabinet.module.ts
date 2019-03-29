import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';


import { CabinetRoutingModule } from './cabinet-routing.module';
import { HomeComponent } from './home/home.component';
import { CabinetComponent } from './cabinet.component';
import { UserBookingsComponent } from './bookings/bookings.user.component';
import { UserEditComponent } from './user/user-edit.component';
import { BookingsModule } from '../bookings/bookings.module';


@NgModule({
    imports: [
      CommonModule,
      FormsModule,
      ReactiveFormsModule,
      CabinetRoutingModule,
      BookingsModule
    ],
    declarations: [
        CabinetComponent,
        HomeComponent,
        UserEditComponent,
        UserBookingsComponent
    ]
})
export class CabinetModule { }
