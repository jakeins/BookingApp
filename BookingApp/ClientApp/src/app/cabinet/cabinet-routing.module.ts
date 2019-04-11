import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';

import { CabinetGuard } from './cabinet.guard';
import { HomeComponent } from './home/home.component';
import { CabinetComponent } from './cabinet.component';
import { UserBookingsComponent } from './bookings/bookings.user.component';
import { UserEditComponent } from './user/user-edit.component';
import { UserChangePasswordComponent } from './user-change-password/user-change-password.component';


const routesCabinet: Routes = [
    {
        path: '', component: CabinetComponent, canActivate: [CabinetGuard], children: [
          { path: '', component: HomeComponent, data: { breadcrumbIgnore: true } },
          { path: 'bookings', component: UserBookingsComponent, data: { breadcrumbLabel: 'My Bookings' } },
          { path: 'user/:id/edit', component: UserEditComponent, data: { breadcrumbLabel: 'Change Name' } },
          { path: 'user/:id/change-password', component: UserChangePasswordComponent, data: { breadcrumbLabel: 'Change Password' } },
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
