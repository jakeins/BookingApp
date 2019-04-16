import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AdminRoutingModule } from './admin-routing.module';
import { HomeComponent } from './home/home.component';
import { UserComponent } from './user/user.component';
import { AdminComponent } from './admin.component';
import { ResourceEditComponent } from './resource/resource-edit.component';
import { FolderEditComponent } from './folder/folder-edit.component';
import { RulesComponent } from './rules/rules.component';
 import { RuleComponent } from './rules/rule/rule.component';
import { RuleListComponent } from './rules/rule-list/rule-list.component';
import { UserNamePipe, RuleActivityPipe } from './rule.pipe';
import { AdminBookingComponent } from './bookings/bookings.admin.component';
import { BookingsModule } from '../bookings/bookings.module';
import { MaterialModule } from '../material/material.module';
import { UserCPComponent } from './user/user-read.component';
import { UserCreateComponent } from './user/user-create.cpmponent';
import { UserListComponent } from './user/user-list.component';
import { UserRenameComponent } from './user/user-edit.component';
import { StatsBookingComponent } from './stats/stats-bookings.component';
import { StatsResourcesComponent } from './stats/stats-resources.component';
import { StatsResourceComponent } from './stats/stats-resource.component';
import { StatsUsersComponent } from './stats/stats-users.component';
import { UserDetailsComponent } from './user/user-details.component';
import { TheirsBookingsComponent } from './bookings/theirs-bookings.admin.component';


@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    AdminRoutingModule,
    BookingsModule,
    MaterialModule
  ],
  declarations: [
      AdminComponent,
      HomeComponent,
      UserComponent,
      ResourceEditComponent,
      FolderEditComponent,
      RulesComponent,
      RuleComponent,
      UserNamePipe,
      RuleActivityPipe,
      AdminBookingComponent,
      RuleListComponent,
      UserCPComponent,
    UserCreateComponent,
    UserListComponent,
    UserRenameComponent,
    TheirsBookingsComponent,
      StatsBookingComponent,
      StatsResourcesComponent,
      StatsResourceComponent,
    StatsUsersComponent,
    UserDetailsComponent
  ],
    entryComponents: [RuleComponent] 
})
export class AdminModule { }
