"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    }
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var forms_1 = require("@angular/forms");
var FolderFormControl = /** @class */ (function (_super) {
    __extends(FolderFormControl, _super);
    function FolderFormControl(typeField, label, property, value, validator) {
        var _this = _super.call(this, value, validator) || this;
        _this.typeField = typeField;
        _this.label = label;
        _this.modelProperty = property;
        return _this;
    }
    FolderFormControl.prototype.getValidationMessages = function () {
        var messages = [];
        if (this.errors) {
            for (var errorName in this.errors) {
                switch (errorName) {
                    case "required":
                        messages.push("You must enter a " + this.label);
                        break;
                    case "minlength":
                        messages.push("A " + this.label + " must be at least\n " + this.errors['minlength'].requiredLength + "\n characters");
                        break;
                    case "maxlength":
                        messages.push("A " + this.label + " must be no more than\n " + this.errors['maxlength'].requiredLength + "\n characters");
                        break;
                }
            }
        }
        return messages;
    };
    return FolderFormControl;
}(forms_1.FormControl));
exports.FolderFormControl = FolderFormControl;
var FolderFormGroup = /** @class */ (function (_super) {
    __extends(FolderFormGroup, _super);
    function FolderFormGroup() {
        return _super.call(this, {
            title: new FolderFormControl("text", "Title", "title", "", forms_1.Validators.compose([
                forms_1.Validators.required,
                forms_1.Validators.minLength(3),
                forms_1.Validators.maxLength(64)
            ])),
            parentFolderId: new FolderFormControl("select", "Parent Folder", "parentFolderId", null, false),
            defaultRuleId: new FolderFormControl("select", "Rule", "defaultRuleId", null, false),
            isActive: new FolderFormControl("checkbox", "Is active", "isActive", null, false),
        }) || this;
    }
    Object.defineProperty(FolderFormGroup.prototype, "folderControls", {
        get: function () {
            var _this = this;
            return Object.keys(this.controls)
                .map(function (k) { return _this.controls[k]; });
        },
        enumerable: true,
        configurable: true
    });
    FolderFormGroup.prototype.getFormValidationMessages = function (form) {
        var messages = [];
        this.folderControls.forEach(function (c) { return c.getValidationMessages()
            .forEach(function (m) { return messages.push(m); }); });
        return messages;
    };
    return FolderFormGroup;
}(forms_1.FormGroup));
exports.FolderFormGroup = FolderFormGroup;
//# sourceMappingURL=folder-form.model.js.map