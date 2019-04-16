import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';

import { AdminGuard } from './admin.guard';
import { HomeComponent } from './home/home.component';
import { AdminComponent } from './admin.component';
import { UserComponent } from './user/user.component';
import { ResourceEditComponent } from './resource/resource-edit.component';
import { FolderEditComponent } from './folder/folder-edit.component';
import { RulesComponent } from './rules/rules.component';
import { RuleListComponent } from './rules/rule-list/rule-list.component';
import { AdminBookingComponent } from './bookings/bookings.admin.component';
import { UserCPComponent } from './user/user-read.component';
import { UserCreateComponent } from './user/user-create.cpmponent';
import { UserListComponent } from './user/user-list.component';
import { UserRenameComponent } from './user/user-edit.component';
import { StatsBookingComponent } from './stats/stats-bookings.component';
import { StatsResourcesComponent } from './stats/stats-resources.component';
import { StatsUsersComponent } from './stats/stats-users.component';
import { UserDetailsComponent } from './user/user-details.component';


const routesAdmin: Routes = [
  {
    path: '', component: AdminComponent, canActivate: [AdminGuard], children: [
      { path: '', component: HomeComponent, data: { breadcrumbIgnore: true } },
      {
        path: 'users', component: UserComponent, data: { breadcrumbLabel: 'Users Management' }, children: [
          { path: '', component: UserListComponent, data: { breadcrumbLabel: 'Users List', breadcrumbIgnore: true } },
          { path: 'create', component: UserCreateComponent, data: { breadcrumbLabel: 'Create Admin' } },
          {
            path: ':id', component: UserDetailsComponent, data: { breadcrumbLabel: 'User Details' }, children: [
              { path: '', component: UserCPComponent, data: { breadcrumbLabel: 'User Control Panel', breadcrumbIgnore: true } },
              { path: 'rename', component: UserRenameComponent, data: { breadcrumbLabel: 'User Rename' } },
            ]
          },
          
        ]
      },
      { path: 'resources/create', component: ResourceEditComponent, data: { breadcrumbLabel: 'Resource Creation' } },
      { path: 'resources/:id/edit', component: ResourceEditComponent, data: { breadcrumbLabel: 'Resource Editing' } },
      { path: 'folders/create', component: FolderEditComponent, data: { breadcrumbLabel: 'Folder Creation' } },
      { path: 'folders/:id/edit', component: FolderEditComponent, data: { breadcrumbLabel: 'Folder Editing' } },
      { path: 'rules', component: RulesComponent, data: { breadcrumbLabel: 'Rules Management' } },
      { path: 'rules/:id', component: RuleListComponent, data: {breadcrumbLabel: 'Rule Details'} },
      { path: 'rules/:id/edit', component: RuleListComponent, data: { breadcrumbLabel: 'Rule Editing'} },
      { path: 'rules/create', component: RuleListComponent, data: { breadcrumbLabel: 'Rule Creation'} },
      { path: 'bookings', component: AdminBookingComponent, data: { breadcrumbLabel: 'All Bookings' } },
      { path: 'stats/bookings', component: StatsBookingComponent, data: { breadcrumbLabel: 'Bookings Statistics' } },
      { path: 'stats/resources', component: StatsResourcesComponent, data: { breadcrumbLabel: 'Resources Statistics' } },
      { path: 'stats/users', component: StatsUsersComponent, data: { breadcrumbLabel: 'Users Statistics' } }
    ]
  }
];

@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild(routesAdmin)
  ],
  exports: [RouterModule],
  declarations: []
})
export class AdminRoutingModule { }
