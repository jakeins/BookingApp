import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';

import { CabinetGuard } from './cabinet.guard';
import { HomeComponent } from './home/home.component';
import { CabinetComponent } from './cabinet.component';
import { BookingsComponent } from './bookings/bookings.component';


const routesCabinet: Routes = [
    {
        path: '', component: CabinetComponent, canActivate: [CabinetGuard], children: [
            { path: '', component: HomeComponent },
            { path: 'bookings', component: BookingsComponent },
        ]
    }
];

@NgModule({
    imports: [
        CommonModule,
        RouterModule.forChild(routesCabinet)
    ],
    exports: [RouterModule],
    declarations: []
})
export class CabinetRoutingModule { }
