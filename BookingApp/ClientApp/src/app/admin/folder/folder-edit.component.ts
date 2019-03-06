import { Component, OnInit, Inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FolderService } from '../../services/folder.service';
import { Folder } from '../../models/folder';
import { FolderFormGroup } from '../../models/folder-form.model';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-folder-edit',
  templateUrl: './folder-edit.component.html',
  styleUrls: ['./folder-edit.component.css']
})
export class FolderEditComponent implements OnInit {

  titleMessage: string = "Create";
  flashMessage: string = "";
  apiError: string = "";
  IsCreate: boolean = true;
  form: FolderFormGroup;
  newFolder: Folder;
  formSubmitted: boolean = false;
  parentFolder: number;
  folderId: number;
  folders: Folder[];

  constructor(private folderService: FolderService, private router: Router, private actRoute: ActivatedRoute) { }

  ngOnInit() {

    this.setParentFolderParam(+this.actRoute.snapshot.queryParams['parentFolderId']);
    this.isCreate(+this.actRoute.snapshot.params['id']);
    this.newFolder = new Folder("", this.parentFolder, 1, false);

    if (!this.IsCreate) {
      this.folderService.getFolder(this.folderId).subscribe((folder: Folder) => {
        if (folder.parentFolderId == undefined) folder.parentFolderId = 0;
        if (folder.defaultRuleId == undefined) folder.defaultRuleId = 1;
        this.newFolder = folder;
      });
    }
    
    this.form = new FolderFormGroup();

    this.folderService.getList().subscribe((folders: Folder[]) => {
      folders.unshift(new Folder("Root", null, null, false, 0));
      let fId = this.folderId;
      folders.forEach(function (item, index, object) {
        if (item.id == fId) {
          object.splice(index, 1);
        }
      }, fId);
      
      this.folders = folders;
    });
  }

  isCreate(id: number) {
    this.folderId = id;
    if (!isNaN(id)) {
      this.IsCreate = false;
      this.titleMessage = "Update";
    }
  }

  setParentFolderParam(parentFolder: number) {
    if (!isNaN(parentFolder)) {
      this.parentFolder = parentFolder;
    } else {
      this.parentFolder = 0;
    }
  }

  createFolder() {
    if (this.newFolder.parentFolderId == 0) this.newFolder.parentFolderId = undefined;
    this.folderService.createFolder(this.newFolder)
      .subscribe(result => {
        if (result['folderId'] != undefined) {
          this.apiError = "";
          this.flashMessage = "You succesfull added Folder";
        }
      }, error => this.handleError(error));
  }

  updateFolder() {
    if (this.newFolder.parentFolderId == 0) this.newFolder.parentFolderId = undefined;
    if (this.newFolder.defaultRuleId == null || this.newFolder.defaultRuleId == 0) this.newFolder.defaultRuleId = undefined;
    this.newFolder.id = this.folderId;
    this.folderService.updateFolder(this.newFolder)
      .subscribe(result => {
        if (this.newFolder.parentFolderId == undefined) this.newFolder.parentFolderId = 0;
        this.apiError = "";
        this.flashMessage = "You succesfull updated Folder";
      }, error => this.handleError(error));
  }

  delete() {
    this.folderService.deleteFolder(this.newFolder.id).subscribe(() => {
      this.router.navigate(['']);
    }, error => this.handleError(error));
  }

  submitForm(form: NgForm) {
    this.formSubmitted = true;
    if (form.valid) {
      (this.IsCreate) ? this.createFolder() : this.updateFolder();
      if (this.IsCreate) form.reset();
      this.formSubmitted = false;
    }
  }

  handleError(error: any) {
    this.apiError = error['status'];

    if (error['error'] != undefined) {
      this.flashMessage = "";
      this.apiError += ': ' + error['error']['Message'];
    }     
  }

}
