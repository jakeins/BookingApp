import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { AppRoutingModule } from './app-routing.module';

import { AppComponent } from './app.component';
import { AuthService } from './services/auth.service';
import { SiteModule } from './site/site.module';


@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
      BrowserModule,
      HttpClientModule,
      FormsModule,
      RouterModule,
      AppRoutingModule,
      SiteModule
  ],
  providers: [AuthService],
  bootstrap: [AppComponent]
})
export class AppModule { }
