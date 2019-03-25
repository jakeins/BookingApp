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
import { AccessTokenService } from "../../services/access-token.service";



const routesCabinet: Routes = [
  {
    path: '', component: CabinetComponent, canActivate: [CabinetGuard], children: [
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
      providers: [AuthService, AccessTokenService]
    });

    router = TestBed.get(Router);
    location = TestBed.get(Location);

    fixture = TestBed.createComponent(CabinetComponent);
    router.initialNavigation();

  });


  it('navigate to home cabinet', fakeAsync(() => {
    router.navigate(["cabinet"]).then(() => {
      expect(location.path()).toBe("/cabinet");
    });
  }));

  it('navigate to cabinet -> bookings', fakeAsync(() => {
    router.navigate(["cabinet/bookings"]).then(() => {
      expect(location.path()).toBe("/cabinet/bookings");
    });
  }));

  it('navigate to cabinet -> user-edit', fakeAsync(() => {
    router.navigate(["cabinet/user-edit"]).then(() => {
      expect(location.path()).toBe("/cabinet/user-edit");
    });
  }));


});
