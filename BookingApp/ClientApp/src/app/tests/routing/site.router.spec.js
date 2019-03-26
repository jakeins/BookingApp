"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var common_1 = require("@angular/common");
var testing_1 = require("@angular/core/testing");
var testing_2 = require("@angular/router/testing");
var router_1 = require("@angular/router");
var testing_3 = require("@angular/common/http/testing");
var tree_component_1 = require("../../site/tree/tree.component");
var forget_component_1 = require("../../site/auth/forget/forget.component");
var login_component_1 = require("../../site/auth/login/login.component");
var reset_component_1 = require("../../site/auth/reset/reset.component");
var register_component_1 = require("../../site/auth/register/register.component");
var error_component_1 = require("../../site/error/error.component");
var resource_component_1 = require("../../site/resource/resource.component");
var header_component_1 = require("../../site/header/header.component");
var auth_service_1 = require("../../services/auth.service");
var resource_service_1 = require("../../services/resource.service");
var folder_service_1 = require("../../services/folder.service");
var token_service_1 = require("../../services/token.service");
var user_info_service_1 = require("../../services/user-info.service");
var forms_1 = require("@angular/forms");
var routes = [
    { path: '', component: tree_component_1.TreeComponent },
    { path: 'error/:status-code', component: error_component_1.ErrorComponent },
    { path: 'register', component: register_component_1.RegisterComponent },
    { path: 'login', component: login_component_1.LoginComponent },
    { path: 'forget', component: forget_component_1.ForgetComponent },
    { path: 'reset', component: reset_component_1.ResetComponent },
    { path: 'resources/:id', component: resource_component_1.ResourceComponent },
    { path: 'tree', component: tree_component_1.TreeComponent },
    {
        path: '**',
        redirectTo: 'error/404'
    }
];
describe('Router: App tests', function () {
    var location;
    var router;
    var fixture;
    beforeEach(function () {
        testing_1.TestBed.configureTestingModule({
            imports: [testing_2.RouterTestingModule.withRoutes(routes), testing_3.HttpClientTestingModule, forms_1.ReactiveFormsModule],
            declarations: [
                tree_component_1.TreeComponent,
                forget_component_1.ForgetComponent,
                login_component_1.LoginComponent,
                reset_component_1.ResetComponent,
                register_component_1.RegisterComponent,
                error_component_1.ErrorComponent,
                resource_component_1.ResourceComponent,
                header_component_1.AppHeaderComponent
            ],
            providers: [auth_service_1.AuthService, token_service_1.TokenService, user_info_service_1.UserInfoService, resource_service_1.ResourceService, folder_service_1.FolderService]
        });
        router = testing_1.TestBed.get(router_1.Router);
        location = testing_1.TestBed.get(common_1.Location);
        fixture = testing_1.TestBed.createComponent(tree_component_1.TreeComponent);
        router.initialNavigation();
        fixture.detectChanges();
    });
    it('navigate to home', testing_1.fakeAsync(function () {
        router.navigate([""]).then(function () {
            expect(location.path()).toBe("/");
        });
    }));
    it('navigate to register', testing_1.fakeAsync(function () {
        router.navigate(["register"]).then(function () {
            expect(location.path()).toBe("/register");
        });
    }));
    //it('navigate to login', fakeAsync(() => {
    //  router.navigate(["login"]).then(() => {
    //    expect(location.path()).toBe("/login");
    //  });
    //}));
    //it('navigate to forget', fakeAsync(() => {
    //  router.navigate(["forget"]).then(() => {
    //    expect(location.path()).toBe("/forget");
    //  });
    //}));
    //it('navigate to reset', fakeAsync(() => {
    //  router.navigate(["reset"]).then(() => {
    //    expect(location.path()).toBe("/reset");
    //  });
    //}));
    //it('navigate to tree', fakeAsync(() => {
    //  router.navigate(["tree"]).then(() => {
    //    expect(location.path()).toBe("/tree");
    //  });
    //}));
    //it('navigate to resources/1', fakeAsync(() => {
    //  router.navigate(["resources/1"]).then(() => {
    //    expect(location.path()).toBe("/resources/1");
    //  });
    //}));
    //it('navigate to error page', fakeAsync(() => {
    //  router.navigate(["resourcess"]).then(() => {
    //    expect(location.path()).toBe("/error/404");
    //  });
    //}));
});
//# sourceMappingURL=site.router.spec.js.map