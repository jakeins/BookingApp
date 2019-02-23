import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { AppRoutingModule } from './app-routing.module';

import { FolderService } from './services/folder.service';
import { AuthService } from './services/auth.service';
import { SiteModule } from './site/site.module';

import { AppComponent } from './app.component';



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
  providers: [AuthService, FolderService],
  bootstrap: [AppComponent]
})
export class AppModule { }
