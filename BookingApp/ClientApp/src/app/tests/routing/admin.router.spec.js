"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var common_1 = require("@angular/common");
var testing_1 = require("@angular/core/testing");
var testing_2 = require("@angular/router/testing");
var router_1 = require("@angular/router");
var testing_3 = require("@angular/common/http/testing");
var auth_service_1 = require("../../services/auth.service");
var admin_component_1 = require("../../admin/admin.component");
var home_component_1 = require("../../admin/home/home.component");
var user_component_1 = require("../../admin/user/user.component");
var resource_edit_component_1 = require("../../admin/resource/resource-edit.component");
var folder_edit_component_1 = require("../../admin/folder/folder-edit.component");
var forms_1 = require("@angular/forms");
var token_service_1 = require("../../services/token.service");
var user_info_service_1 = require("../../services/user-info.service");
var user_service_1 = require("../../services/user.service");
var folder_service_1 = require("../../services/folder.service");
var resource_service_1 = require("../../services/resource.service");
var routesAdmin = [
    {
        path: '', component: admin_component_1.AdminComponent, children: [
            { path: '', component: home_component_1.HomeComponent },
            { path: 'users', component: user_component_1.UserComponent },
            { path: 'resources/create', component: resource_edit_component_1.ResourceEditComponent },
            { path: 'resources/:id/edit', component: resource_edit_component_1.ResourceEditComponent },
            { path: 'folders/create', component: folder_edit_component_1.FolderEditComponent },
            { path: 'folders/:id/edit', component: folder_edit_component_1.FolderEditComponent },
        ]
    }
];
describe('Router: Admin tests', function () {
    var location;
    var router;
    var fixture;
    beforeEach(function () {
        testing_1.TestBed.configureTestingModule({
            imports: [testing_2.RouterTestingModule.withRoutes(routesAdmin), testing_3.HttpClientTestingModule, forms_1.ReactiveFormsModule],
            declarations: [
                admin_component_1.AdminComponent,
                home_component_1.HomeComponent,
                user_component_1.UserComponent,
                resource_edit_component_1.ResourceEditComponent,
                folder_edit_component_1.FolderEditComponent
            ],
            providers: [auth_service_1.AuthService, token_service_1.TokenService, user_info_service_1.UserInfoService, user_service_1.UserService, folder_service_1.FolderService, resource_service_1.ResourceService]
        });
        router = testing_1.TestBed.get(router_1.Router);
        location = testing_1.TestBed.get(common_1.Location);
        fixture = testing_1.TestBed.createComponent(admin_component_1.AdminComponent);
        router.initialNavigation();
    });
    it('navigate to home admin', testing_1.fakeAsync(function () {
        router.navigate([""]).then(function () {
            fixture.detectChanges();
            expect(location.path()).toBe("/");
        });
    }));
    it('navigate to home admin -> users', testing_1.fakeAsync(function () {
        router.navigate(["users"]).then(function () {
            fixture.detectChanges();
            expect(location.path()).toBe("/users");
        });
    }));
    it('navigate to home admin -> resources/create', testing_1.fakeAsync(function () {
        router.navigate(["resources/create"]).then(function () {
            fixture.detectChanges();
            expect(location.path()).toBe("/resources/create");
        });
    }));
    it('navigate to home admin -> folders/create', testing_1.fakeAsync(function () {
        router.navigate(["folders/create"]).then(function () {
            fixture.detectChanges();
            expect(location.path()).toBe("/folders/create");
        });
    }));
    it('navigate to home admin -> resources/:id/edit', testing_1.fakeAsync(function () {
        router.navigate(["resources/1/edit"]).then(function () {
            fixture.detectChanges();
            expect(location.path()).toBe("/resources/1/edit");
        });
    }));
    it('navigate to home admin -> folders/:id/edit', testing_1.fakeAsync(function () {
        router.navigate(["folders/1/edit"]).then(function () {
            fixture.detectChanges();
            expect(location.path()).toBe("/folders/1/edit");
        });
    }));
});
//# sourceMappingURL=admin.router.spec.js.map