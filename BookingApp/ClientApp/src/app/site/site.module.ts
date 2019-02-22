import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import {RouterModule} from '@angular/router';


import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './auth/register/register.component';
import { LoginComponent } from './auth/login/login.component';
import { ForgetComponent } from './auth/forget/forget.component';
import { ResetComponent } from './auth/reset/reset.component';
import {ErrorComponent} from './error/error.component';



@NgModule({
  declarations: [
    HomeComponent,
    RegisterComponent,
    LoginComponent,
    ForgetComponent,
    ResetComponent,
      ErrorComponent
  ],
  imports: [
    CommonModule,
      RouterModule
  ],
  providers: [],
  bootstrap: []
})
export class SiteModule { }
