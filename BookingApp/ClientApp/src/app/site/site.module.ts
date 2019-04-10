import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { RegisterComponent } from './auth/register/register.component';
import { LoginComponent } from './auth/login/login.component';
import { ForgetComponent } from './auth/forget/forget.component';
import { ResetComponent } from './auth/reset/reset.component';
import { ErrorComponent } from './error/error.component';
import { ResourceComponent } from './resource/resource.component';
import { TreeComponent } from './tree/tree.component';
import { SiteRuleComponent } from './rule/rule.component';
import { AppHeaderComponent } from './header/header.component';
import { BreadcrumbsComponent } from './breadcrumbs/breadcrumbs.component';
import { AppFooterComponent } from './footer/footer.component';
import { BookingsModule } from '../bookings/bookings.module';
import { TosComponent } from './tos/tos.component';

@NgModule({
  declarations: [
    TreeComponent,
    TosComponent,
    ResourceComponent,
    RegisterComponent,
    LoginComponent,
    ForgetComponent,
    ResetComponent,
    ErrorComponent,
    AppHeaderComponent,
    AppFooterComponent,
    BreadcrumbsComponent,
    SiteRuleComponent,
  ],
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
    BookingsModule
  ],
  exports: [
    AppHeaderComponent,
    BreadcrumbsComponent,
    AppFooterComponent
  ],
  providers: [],
  bootstrap: []
})

export class SiteModule {
}
