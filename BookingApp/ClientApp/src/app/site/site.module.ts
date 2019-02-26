import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { FolderComponent } from './folder/folder.component';
import { RegisterComponent } from './auth/register/register.component';
import { LoginComponent } from './auth/login/login.component';
import { ForgetComponent } from './auth/forget/forget.component';
import { ResetComponent } from './auth/reset/reset.component';
import { ErrorComponent } from './error/error.component';
import { ResourceListComponent } from './resource/resource-list.component';
import { ResourceComponent } from './resource/resource.component';
import { TreeComponent } from './tree/tree.component';
import { AppHeaderComponent } from './header/header.component';


@NgModule({
  declarations: [
    TreeComponent,
    ResourceListComponent,
    ResourceComponent,
    FolderComponent,
    RegisterComponent,
    LoginComponent,
    ForgetComponent,
    ResetComponent,
    ErrorComponent,
    AppHeaderComponent
  ],
  imports: [
    CommonModule,
    RouterModule
  ],
  exports: [AppHeaderComponent],
  providers: [],
  bootstrap: []
})
export class SiteModule { }
