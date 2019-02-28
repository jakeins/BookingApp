import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { FolderComponent } from './folder/folder.component';
import { RegisterComponent } from './auth/register/register.component';
import { LoginComponent } from './auth/login/login.component';
import { ForgetComponent } from './auth/forget/forget.component';
import { ResetComponent } from './auth/reset/reset.component';
import { ErrorComponent } from './error/error.component';

import { ResourceComponent } from './resource/resource.component';
import { ResourceEditComponent } from './resource/resource-edit.component';

import { TreeComponent } from './tree/tree.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    TreeComponent,

    ResourceComponent,
    ResourceEditComponent,

    FolderComponent,

    RegisterComponent,
    LoginComponent,
    ForgetComponent,
    ResetComponent,

    ErrorComponent    
  ],
  imports: [
    CommonModule,
    RouterModule,

    FormsModule,
    ReactiveFormsModule,
  ],
  providers: [],
  bootstrap: []
})

export class SiteModule {
}
