import { FormControl, FormGroup, Validators } from "@angular/forms";
import { Folder } from "./folder";

export class FolderFormControl extends FormControl {
  typeField: string;
  label: string;
  modelProperty: string;
  valueField: any;

  constructor(typeField: string, label: string, property: string, value: any, validator: any) {
    super(value, validator);
    this.typeField = typeField;
    this.label = label;
    this.modelProperty = property;
    this.valueField = value;
  }

  getValidationMessages() {
    let messages: string[] = [];
    if (this.errors) {
      for (let errorName in this.errors) {
        switch (errorName) {
          case "required":
            messages.push(`You must enter a ${this.label}`);
            break;
          case "minlength":
            messages.push(`A ${this.label} must be at least
 ${this.errors['minlength'].requiredLength}
 characters`);
            break;
          case "maxlength":
            messages.push(`A ${this.label} must be no more than
 ${this.errors['maxlength'].requiredLength}
 characters`);
            break;
        }
      }
    }
    return messages;
  }

}


export class FolderFormGroup extends FormGroup {
  constructor(folder: Folder) {
    super({
      title: new FolderFormControl("text", "Title", "title", folder.title,
        Validators.compose([
          Validators.required,
          Validators.minLength(3),
          Validators.maxLength(64)
        ])),
      parentFolderId: new FolderFormControl("select", "Parent Folder", "parentFolderId", folder.parentFolderId, false),
      defaultRuleId: new FolderFormControl("select", "Rule", "defaultRuleId", folder.defaultRuleId, false),
      isActive: new FolderFormControl("checkbox", "Is active", "isActive", folder.isActive, false),
    });
  }

  get folderControls(): FolderFormControl[] {
    return Object.keys(this.controls)
      .map(k => this.controls[k] as FolderFormControl);
  }

  getFormValidationMessages(form: any): string[] {
    let messages: string[] = [];
    this.folderControls.forEach(c => c.getValidationMessages()
      .forEach(m => messages.push(m)));
    return messages;
  }
}
