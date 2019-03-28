import { FolderEditComponent } from "../../admin/folder/folder-edit.component";
import { ComponentFixture, TestBed } from "@angular/core/testing";
import { ReactiveFormsModule } from "@angular/forms";
import { FolderService } from "../../services/folder.service";
import { HttpClientTestingModule } from "@angular/common/http/testing";
import { RouterTestingModule } from "@angular/router/testing";
import { Folder } from "../../models/folder";
import { RuleService } from "../../services/rule.service";


describe('Component: FolderEdit', () => {

  let component: FolderEditComponent;
  let fixture: ComponentFixture<FolderEditComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, RouterTestingModule, ReactiveFormsModule],
      declarations: [FolderEditComponent],
      providers: [FolderService, RuleService]
    });
    // create component and test fixture
    fixture = TestBed.createComponent(FolderEditComponent);
    // get test component from the fixture
    component = fixture.componentInstance;

  });


  it('check method isCreate', () => {
    component.isCreate(1);
    expect(component.folderId).toBe(1);
  });

  it('check method setParentFolderParam', () => {
    component.setParentFolderParam(1);
    expect(component.parentFolder).toBe(1);
  });

});
