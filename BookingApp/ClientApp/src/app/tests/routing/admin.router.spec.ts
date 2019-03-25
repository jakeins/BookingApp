import { Location } from "@angular/common";
import { TestBed, fakeAsync, tick } from '@angular/core/testing';
import { RouterTestingModule } from "@angular/router/testing";
import { Router, Routes } from "@angular/router";
import { HttpClientTestingModule } from "@angular/common/http/testing";
import { } from 'jasmine';


import { AuthService } from "../../services/auth.service";
import { AdminGuard } from "../../admin/admin.guard";
import { AdminComponent } from "../../admin/admin.component";
import { HomeComponent } from "../../admin/home/home.component";
import { UserComponent } from "../../admin/user/user.component";
import { ResourceEditComponent } from "../../admin/resource/resource-edit.component";
import { FolderEditComponent } from "../../admin/folder/folder-edit.component";
import { ReactiveFormsModule } from "@angular/forms";
import { TokenService } from "../../services/token.service";
import { UserInfoService } from "../../services/user-info.service";
import { UserService } from "../../services/user.service";
import { FolderService } from "../../services/folder.service";
import { ResourceService } from "../../services/resource.service";


const routesAdmin: Routes = [
  {
    path: '', component: AdminComponent, children: [
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
      providers: [AuthService, TokenService, UserInfoService, UserService, FolderService, ResourceService]
    });

    router = TestBed.get(Router);
    location = TestBed.get(Location);

    fixture = TestBed.createComponent(AdminComponent);
    router.initialNavigation();
  });


  it('navigate to home admin', fakeAsync(() => {
    router.navigate([""]).then(() => {
      fixture.detectChanges();
      expect(location.path()).toBe("/");
    });
  }));

  it('navigate to home admin -> users', fakeAsync(() => {
    router.navigate(["users"]).then(() => {
      fixture.detectChanges();
      expect(location.path()).toBe("/users");
    });
  }));

  it('navigate to home admin -> resources/create', fakeAsync(() => {
    router.navigate(["resources/create"]).then(() => {
      fixture.detectChanges();
      expect(location.path()).toBe("/resources/create");
    });
  }));

  it('navigate to home admin -> folders/create', fakeAsync(() => {
    router.navigate(["folders/create"]).then(() => {
      fixture.detectChanges();
      expect(location.path()).toBe("/folders/create");
    });
  }));

  it('navigate to home admin -> resources/:id/edit', fakeAsync(() => {
    router.navigate(["resources/1/edit"]).then(() => {
      fixture.detectChanges();
      expect(location.path()).toBe("/resources/1/edit");
    });
  }));

  it('navigate to home admin -> folders/:id/edit', fakeAsync(() => {
    router.navigate(["folders/1/edit"]).then(() => {
      fixture.detectChanges();
      expect(location.path()).toBe("/folders/1/edit");
    });
  }));



});
