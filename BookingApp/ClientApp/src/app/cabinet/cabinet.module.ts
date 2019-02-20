import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {RouterModule} from '@angular/router';

import { HomeComponent } from './home/home.component';
import { CabinetRoutingModule } from './cabinet-routing.module';
import { CabinetComponent } from './cabinet.component';
import { BookingsComponent } from './bookings/bookings.component';


@NgModule({
    imports: [
        CommonModule,
        CabinetRoutingModule,
        RouterModule
    ],
    declarations: [
        CabinetComponent,
        HomeComponent,
        BookingsComponent
    ]
})
export class CabinetModule { }
