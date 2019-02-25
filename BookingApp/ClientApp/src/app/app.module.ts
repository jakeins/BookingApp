import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { AppRoutingModule } from './app-routing.module';

import { SiteModule } from './site/site.module';
import { AppComponent } from './app.component';

import { AuthService } from './services/auth.service';
import { FolderService } from './services/folder.service';
import { ResourceService } from './services/resource.service';
import { AppHeaderComponent } from './site/header/header.component';


@NgModule({
  declarations: [
    AppComponent,
    AppHeaderComponent
  ],
  imports: [
      BrowserModule,
      HttpClientModule,
      FormsModule,
      RouterModule,
      AppRoutingModule,
      SiteModule
  ],
  providers: [
    AuthService,
    FolderService,
    ResourceService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
