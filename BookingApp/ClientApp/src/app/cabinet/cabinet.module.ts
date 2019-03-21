import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CabinetRoutingModule } from './cabinet-routing.module';
import { HomeComponent } from './home/home.component';
import { CabinetComponent } from './cabinet.component';
import { BookingsComponent } from './bookings/bookings.component';
import { UserEditComponent } from './user-edit/user-edit.component';


@NgModule({
    imports: [
      CommonModule,
      CabinetRoutingModule
    ],
    declarations: [
        CabinetComponent,
        HomeComponent,
        UserEditComponent,
        BookingsComponent
    ]
})
export class CabinetModule { }
