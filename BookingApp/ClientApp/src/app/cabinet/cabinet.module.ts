import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CabinetRoutingModule } from './cabinet-routing.module';
import { HomeComponent } from './home/home.component';
import { CabinetComponent } from './cabinet.component';
import { BookingsComponent } from './bookings/bookings.component';


@NgModule({
    imports: [
      CommonModule,
      CabinetRoutingModule
    ],
    declarations: [
        CabinetComponent,
        HomeComponent,
        BookingsComponent
    ]
})
export class CabinetModule { }
