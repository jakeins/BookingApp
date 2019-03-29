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
import { AdminBookingComponent } from './bookings/bookings.admin.component';


const routesAdmin: Routes = [
  {
    path: '', component: AdminComponent, canActivate: [AdminGuard], children: [
      { path: '', component: HomeComponent, data: { breadcrumbIgnore: true } },
      { path: 'users', component: UserComponent, data: { breadcrumbLabel: 'Users management' } },
      { path: 'resources/create', component: ResourceEditComponent, data: { breadcrumbLabel: 'Resource Creation' } },
      { path: 'resources/:id/edit', component: ResourceEditComponent, data: { breadcrumbLabel: 'Resource Editing' } },
      { path: 'folders/create', component: FolderEditComponent, data: { breadcrumbLabel: 'Folder Creation' } },
      { path: 'folders/:id/edit', component: FolderEditComponent, data: { breadcrumbLabel: 'Folder Editing' } },
      { path: 'rules', component: RulesComponent, data: { breadcrumbLabel: 'Rules management' } },
      { path: 'bookings', component: AdminBookingComponent, data: { breadcrumbLabel: 'All bookings managment' } }
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
