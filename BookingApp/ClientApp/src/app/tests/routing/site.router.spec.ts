import { Location } from "@angular/common";
import { TestBed, fakeAsync, tick } from '@angular/core/testing';
import { RouterTestingModule } from "@angular/router/testing";
import { Router, Routes } from "@angular/router";
import { HttpClientTestingModule } from "@angular/common/http/testing";
import { } from 'jasmine';

import { TreeComponent } from '../../site/tree/tree.component';
import { ForgetComponent } from '../../site/auth/forget/forget.component';
import { LoginComponent } from '../../site/auth/login/login.component';
import { ResetComponent } from '../../site/auth/reset/reset.component';
import { RegisterComponent } from '../../site/auth/register/register.component';
import { ErrorComponent } from '../../site/error/error.component';
import { ResourceComponent } from '../../site/resource/resource.component';
import { AppHeaderComponent } from '../../site/header/header.component';
import { AuthService } from "../../services/auth.service";
import { AccessTokenService } from "../../services/access-token.service";
import { ResourceService } from "../../services/resource.service";
import { FolderService } from "../../services/folder.service";

const routes: Routes = [
  { path: '', component: TreeComponent },
  { path: 'error/:status-code', component: ErrorComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'login', component: LoginComponent },
  { path: 'forget', component: ForgetComponent },
  { path: 'reset', component: ResetComponent },
  { path: 'resources/:id', component: ResourceComponent },
  { path: 'tree', component: TreeComponent },
  {
    path: '**',
    redirectTo: 'error/404'
  }
];


describe('Router: App tests', () => {

  let location: Location;
  let router: Router;
  let fixture;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [RouterTestingModule.withRoutes(routes), HttpClientTestingModule],
      declarations: [
        TreeComponent,
        ForgetComponent,
        LoginComponent,
        ResetComponent,
        RegisterComponent,
        ErrorComponent,
        ResourceComponent,
        AppHeaderComponent
      ],
      providers: [AuthService, AccessTokenService, ResourceService, FolderService]
    });

    router = TestBed.get(Router);
    location = TestBed.get(Location);

    fixture = TestBed.createComponent(TreeComponent);
    router.initialNavigation();

  });


  it('navigate to home', fakeAsync(() => {
    router.navigate([""]).then(() => {
      expect(location.path()).toBe("/");
    });
  }));

  it('navigate to register', fakeAsync(() => {
    router.navigate(["register"]).then(() => {
      expect(location.path()).toBe("/register");
    });
  }));

  it('navigate to login', fakeAsync(() => {
    router.navigate(["login"]).then(() => {
      expect(location.path()).toBe("/login");
    });
  }));

  it('navigate to forget', fakeAsync(() => {
    router.navigate(["forget"]).then(() => {
      expect(location.path()).toBe("/forget");
    });
  }));

  it('navigate to reset', fakeAsync(() => {
    router.navigate(["reset"]).then(() => {
      expect(location.path()).toBe("/reset");
    });
  }));

  it('navigate to tree', fakeAsync(() => {
    router.navigate(["tree"]).then(() => {
      expect(location.path()).toBe("/tree");
    });
  }));

  it('navigate to resources/1', fakeAsync(() => {
    router.navigate(["resources/1"]).then(() => {
      expect(location.path()).toBe("/resources/1");
    });
  }));

  it('navigate to error page', fakeAsync(() => {
    router.navigate(["resourcess"]).then(() => {
      expect(location.path()).toBe("/error/404");
    });
  }));


});
