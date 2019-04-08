import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { AppRoutingModule } from './app-routing.module';

import { SiteModule } from './site/site.module';
import { AppComponent } from './app.component';

import { AuthService } from './services/auth.service';
import { FolderService } from './services/folder.service';
import { ResourceService } from './services/resource.service';
import { TokenInterceptor } from './services/token.interceptor';
import { TokenService } from './services/token.service';
import { UserService } from './services/user.service';
import { UserInfoService } from './services/user-info.service';
import { RuleService } from './services/rule.service';
import { BookingService } from './services/booking.service';
import { BookingsModeService } from './services/bookings-component-mode.service';
import { MaterialModule } from './material/material.module';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
      BrowserModule,
      HttpClientModule,
      RouterModule,
      AppRoutingModule,
      SiteModule,
      BrowserAnimationsModule,
      MaterialModule
  ],
  providers: [
    AuthService,
    TokenService,
    UserInfoService,
    FolderService,
    UserService,
    ResourceService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TokenInterceptor,
      multi: true
    },
    RuleService,
    BookingService,
    BookingsModeService
  ],
  bootstrap: [AppComponent]
})


export class AppModule {
}
