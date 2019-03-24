"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var folder_edit_component_1 = require("../../admin/folder/folder-edit.component");
var testing_1 = require("@angular/core/testing");
var forms_1 = require("@angular/forms");
var folder_service_1 = require("../../services/folder.service");
var testing_2 = require("@angular/common/http/testing");
var testing_3 = require("@angular/router/testing");
describe('Component: Folder Edit Component', function () {
    var component;
    var fixture;
    beforeEach(function () {
        testing_1.TestBed.configureTestingModule({
            imports: [forms_1.ReactiveFormsModule, forms_1.FormsModule, testing_2.HttpClientTestingModule, testing_3.RouterTestingModule],
            declarations: [folder_edit_component_1.FolderEditComponent],
            providers: [folder_service_1.FolderService]
        });
        // create component and test fixture
        fixture = testing_1.TestBed.createComponent(folder_edit_component_1.FolderEditComponent);
        // get test component from the fixture
        component = fixture.componentInstance;
        component.ngOnInit();
    });
    it('Form invalid when empty', function () {
        expect(component.form.valid).toBeFalsy();
    });
    it('Submitting a form all expected', function () {
        expect(component.form.valid).toBeFalsy();
        component.form.controls['title'].setValue("Folder 1");
        component.form.controls['parentFolderId'].setValue(1);
        component.form.controls['defaultRuleId'].setValue(1);
        component.form.controls['isActive'].setValue(true);
        expect(component.form.valid).toBeTruthy();
    });
    it('Title field validity', function () {
        var errors = {};
        var title = component.form.controls['title'];
        expect(title.valid).toBeFalsy();
        // Title field is required
        errors = title.errors || {};
        expect(errors['required']).toBeTruthy();
        // Title field is min 3
        title.setValue("Fo");
        errors = title.errors || {};
        expect(errors['minlength']).toBeTruthy();
        // Set title to something
        title.setValue("Folder 1");
        errors = title.errors || {};
        expect(errors['required']).toBeFalsy();
        // Set title to something correct
        title.setValue("Folder 1");
        errors = title.errors || {};
        expect(errors['minlength']).toBeFalsy();
    });
});
//# sourceMappingURL=folder.form.spec.js.map