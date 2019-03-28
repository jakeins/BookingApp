import { FolderEditComponent } from "../../admin/folder/folder-edit.component";
import { TestBed, ComponentFixture } from "@angular/core/testing";
import { ReactiveFormsModule, FormsModule } from "@angular/forms";
import { FolderService } from "../../services/folder.service";
import { HttpClientTestingModule } from "@angular/common/http/testing";
import { RouterTestingModule } from '@angular/router/testing';
import { RuleService } from "../../services/rule.service";


describe('Folder reactive form tests', () => {

  let component: FolderEditComponent;
  let fixture: ComponentFixture<FolderEditComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, FormsModule, HttpClientTestingModule, RouterTestingModule],
      declarations: [FolderEditComponent],
      providers: [FolderService, RuleService]
    });

    // create component and test fixture
    fixture = TestBed.createComponent(FolderEditComponent);

    // get test component from the fixture
    component = fixture.componentInstance;
    component.ngOnInit();
  });


  it('Form invalid when empty', () => {
    expect(component.form.valid).toBeFalsy();
  });


  it('Submitting a form all expected', () => {
    expect(component.form.valid).toBeFalsy();
    component.form.controls['title'].setValue("Folder 1");
    component.form.controls['parentFolderId'].setValue(1);
    component.form.controls['defaultRuleId'].setValue(1); 
    component.form.controls['isActive'].setValue(true);
    expect(component.form.valid).toBeTruthy();
  });


  it('Title field validity', () => {
    let errors = {};
    let title = component.form.controls['title'];
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
