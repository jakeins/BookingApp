import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { AppRoutingModule } from './app-routing.module';

import { SiteModule } from './site/site.module';
import { AppComponent } from './app.component';

import { AuthService } from './services/auth.service';
import { FolderService } from './services/folder.service';
import { ResourceService } from './services/resource.service';
import { AppHeaderComponent } from './site/header/header.component';
import { TokenInterceptor } from './services/token.interceptor';
import { AccessTokenService } from './services/access-token.service';

@NgModule({
  declarations: [
    AppComponent,
    AppHeaderComponent
  ],
  imports: [
      BrowserModule,
      HttpClientModule,

      RouterModule,
      AppRoutingModule,
      SiteModule
  ],
  providers: [
    AuthService,
    AccessTokenService,
    FolderService,
    ResourceService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TokenInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})

export class AppModule {
}
