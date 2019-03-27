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


const routesAdmin: Routes = [
  {
    path: '', component: AdminComponent, canActivate: [AdminGuard], children: [
      { path: '', component: HomeComponent },
      { path: 'users', component: UserComponent },
      { path: 'resources/create', component: ResourceEditComponent },
      { path: 'resources/:id/edit', component: ResourceEditComponent },
      { path: 'folders/create', component: FolderEditComponent },
      { path: 'folders/:id/edit', component: FolderEditComponent },
      { path: 'rules', component: RulesComponent }
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
