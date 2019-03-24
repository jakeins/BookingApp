import { Location } from "@angular/common";
import { TestBed, fakeAsync, tick } from '@angular/core/testing';
import { RouterTestingModule } from "@angular/router/testing";
import { Router, Routes } from "@angular/router";
import { HttpClientTestingModule } from "@angular/common/http/testing";
import { } from 'jasmine';


import { AuthService } from "../../services/auth.service";
import { AccessTokenService } from "../../services/access-token.service";
import { AdminGuard } from "../../admin/admin.guard";
import { AdminComponent } from "../../admin/admin.component";
import { HomeComponent } from "../../admin/home/home.component";
import { UserComponent } from "../../admin/user/user.component";
import { ResourceEditComponent } from "../../admin/resource/resource-edit.component";
import { FolderEditComponent } from "../../admin/folder/folder-edit.component";
import { ReactiveFormsModule } from "@angular/forms";


const routesAdmin: Routes = [
  {
    path: '', component: AdminComponent, canActivate: [AdminGuard], children: [
      { path: '', component: HomeComponent },
      { path: 'users', component: UserComponent },
      { path: 'resources/create', component: ResourceEditComponent },
      { path: 'resources/:id/edit', component: ResourceEditComponent },
      { path: 'folders/create', component: FolderEditComponent },
      { path: 'folders/:id/edit', component: FolderEditComponent },
    ]
  }
];


describe('Router: Admin tests', () => {

  let location: Location;
  let router: Router;
  let fixture;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [RouterTestingModule.withRoutes(routesAdmin), HttpClientTestingModule, ReactiveFormsModule],
      declarations: [
        AdminComponent,
        HomeComponent,
        UserComponent,
        ResourceEditComponent,
        FolderEditComponent
      ],
      providers: [AuthService, AccessTokenService]
    });

    router = TestBed.get(Router);
    location = TestBed.get(Location);

    fixture = TestBed.createComponent(AdminComponent);
    router.initialNavigation();

  });


  it('navigate to home admin', fakeAsync(() => {
    router.navigate(["admin"]).then(() => {
      expect(location.path()).toBe("/admin");
    });
  }));

  it('navigate to home admin -> users', fakeAsync(() => {
    router.navigate(["admin/users"]).then(() => {
      expect(location.path()).toBe("/admin/users");
    });
  }));

  it('navigate to home admin -> resources/create', fakeAsync(() => {
    router.navigate(["admin/resources/create"]).then(() => {
      expect(location.path()).toBe("/admin/resources/create");
    });
  }));

  it('navigate to home admin -> folders/create', fakeAsync(() => {
    router.navigate(["admin/folders/create"]).then(() => {
      expect(location.path()).toBe("/admin/folders/create");
    });
  }));

  it('navigate to home admin -> resources/:id/edit', fakeAsync(() => {
    router.navigate(["admin/resources/1/edit"]).then(() => {
      expect(location.path()).toBe("/admin/resources/1/edit");
    });
  }));

  it('navigate to home admin -> folders/:id/edit', fakeAsync(() => {
    router.navigate(["admin/folders/1/edit"]).then(() => {
      expect(location.path()).toBe("/admin/folders/1/edit");
    });
  }));



});
