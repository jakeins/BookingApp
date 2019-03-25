import { Location } from "@angular/common";
import { TestBed, fakeAsync, tick } from '@angular/core/testing';
import { RouterTestingModule } from "@angular/router/testing";
import { Router, Routes } from "@angular/router";
import { HttpClientTestingModule } from "@angular/common/http/testing";
import { } from 'jasmine';

import { CabinetComponent } from "../../cabinet/cabinet.component";
import { CabinetGuard } from "../../cabinet/cabinet.guard";
import { HomeComponent } from "../../cabinet/home/home.component";
import { BookingsComponent } from "../../cabinet/bookings/bookings.component";
import { UserEditComponent } from "../../cabinet/user-edit/user-edit.component";
import { AuthService } from "../../services/auth.service";
import { TokenService } from "../../services/token.service";
import { UserInfoService } from "../../services/user-info.service";
import { UserService } from "../../services/user.service";



const routesCabinet: Routes = [
  {
    path: '', component: CabinetComponent, children: [
      { path: '', component: HomeComponent },
      { path: 'bookings', component: BookingsComponent },
      { path: 'user-edit', component: UserEditComponent },
    ]
  }
];


describe('Router: Cabinet tests', () => {

  let location: Location;
  let router: Router;
  let fixture;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [RouterTestingModule.withRoutes(routesCabinet), HttpClientTestingModule],
      declarations: [
        CabinetComponent,
        HomeComponent,
        BookingsComponent,
        UserEditComponent
      ],
      providers: [AuthService, TokenService, UserInfoService, UserService]
    });

    router = TestBed.get(Router);
    location = TestBed.get(Location);

    fixture = TestBed.createComponent(CabinetComponent);
    router.initialNavigation();
  });


  it('navigate to home cabinet', fakeAsync(() => {
    router.navigate([""]).then(() => {
      fixture.detectChanges();
      expect(location.path()).toBe("/");
    });
  }));

  it('navigate to cabinet -> bookings', fakeAsync(() => {
    router.navigate(["bookings"]).then(() => {
      fixture.detectChanges();
      expect(location.path()).toBe("/bookings");
    });
  }));

  it('navigate to cabinet -> user-edit', fakeAsync(() => {
    router.navigate(["user-edit"]).then(() => {
      fixture.detectChanges();
      expect(location.path()).toBe("/user-edit");
    });
  }));


});
