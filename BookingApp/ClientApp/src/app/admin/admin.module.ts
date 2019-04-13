import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AdminRoutingModule } from './admin-routing.module';
import { HomeComponent } from './home/home.component';
import { UserComponent } from './user/user.component';
import { AdminComponent } from './admin.component';
import { ResourceEditComponent } from './resource/resource-edit.component';
import { FolderEditComponent } from './folder/folder-edit.component';
import { RulesComponent } from './rules/rules.component'
 import { RuleComponent } from './rules/rule/rule.component';
import { RuleListComponent } from './rules/rule-list/rule-list.component';
import { UserNamePipe, RuleActivityPipe } from './rule.pipe';
import { AdminBookingComponent } from './bookings/bookings.admin.component';
import { BookingsModule } from '../bookings/bookings.module';
import { MaterialModule } from '../material/material.module';
import { UserReadComponent } from './user/user-read.component';
import { UserCreateComponent } from './user/user-create.cpmponent';
import { UserListComponent } from './user/user-list.component';
import { UserChangeComponent } from './user/user-edit.component';


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
      UserReadComponent,
    UserCreateComponent,
    UserListComponent,
    UserChangeComponent
  ],
    entryComponents: [RuleComponent] 
})
export class AdminModule { }
