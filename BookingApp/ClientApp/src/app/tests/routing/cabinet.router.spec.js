"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var common_1 = require("@angular/common");
var testing_1 = require("@angular/core/testing");
var testing_2 = require("@angular/router/testing");
var router_1 = require("@angular/router");
var testing_3 = require("@angular/common/http/testing");
var cabinet_component_1 = require("../../cabinet/cabinet.component");
var home_component_1 = require("../../cabinet/home/home.component");
var bookings_component_1 = require("../../cabinet/bookings/bookings.component");
var user_edit_component_1 = require("../../cabinet/user-edit/user-edit.component");
var auth_service_1 = require("../../services/auth.service");
var token_service_1 = require("../../services/token.service");
var user_info_service_1 = require("../../services/user-info.service");
var user_service_1 = require("../../services/user.service");
var routesCabinet = [
    {
        path: '', component: cabinet_component_1.CabinetComponent, children: [
            { path: '', component: home_component_1.HomeComponent },
            { path: 'bookings', component: bookings_component_1.BookingsComponent },
            { path: 'user-edit', component: user_edit_component_1.UserEditComponent },
        ]
    }
];
describe('Router: Cabinet tests', function () {
    var location;
    var router;
    var fixture;
    beforeEach(function () {
        testing_1.TestBed.configureTestingModule({
            imports: [testing_2.RouterTestingModule.withRoutes(routesCabinet), testing_3.HttpClientTestingModule],
            declarations: [
                cabinet_component_1.CabinetComponent,
                home_component_1.HomeComponent,
                bookings_component_1.BookingsComponent,
                user_edit_component_1.UserEditComponent
            ],
            providers: [auth_service_1.AuthService, token_service_1.TokenService, user_info_service_1.UserInfoService, user_service_1.UserService]
        });
        router = testing_1.TestBed.get(router_1.Router);
        location = testing_1.TestBed.get(common_1.Location);
        fixture = testing_1.TestBed.createComponent(cabinet_component_1.CabinetComponent);
        router.initialNavigation();
    });
    it('navigate to home cabinet', testing_1.fakeAsync(function () {
        router.navigate([""]).then(function () {
            fixture.detectChanges();
            expect(location.path()).toBe("/");
        });
    }));
    it('navigate to cabinet -> bookings', testing_1.fakeAsync(function () {
        router.navigate(["bookings"]).then(function () {
            fixture.detectChanges();
            expect(location.path()).toBe("/bookings");
        });
    }));
    it('navigate to cabinet -> user-edit', testing_1.fakeAsync(function () {
        router.navigate(["user-edit"]).then(function () {
            fixture.detectChanges();
            expect(location.path()).toBe("/user-edit");
        });
    }));
});
//# sourceMappingURL=cabinet.router.spec.js.map