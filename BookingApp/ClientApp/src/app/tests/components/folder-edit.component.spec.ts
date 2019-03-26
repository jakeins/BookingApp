import { FolderEditComponent } from "../../admin/folder/folder-edit.component";
import { ComponentFixture, TestBed } from "@angular/core/testing";


describe('Component: FolderEdit', () => {

  let component: FolderEditComponent;
  let fixture: ComponentFixture<FolderEditComponent>;

  beforeEach(() => {

    TestBed.configureTestingModule({
      declarations: [FolderEditComponent]
    });

    // create component and test fixture
    fixture = TestBed.createComponent(FolderEditComponent);

    // get test component from the fixture
    component = fixture.componentInstance;

  });
});
