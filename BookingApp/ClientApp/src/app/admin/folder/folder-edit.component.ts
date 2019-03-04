import { Component, OnInit, Inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
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
  IsCreate: boolean = true;
  form: FolderFormGroup;
  newFolder: Folder;
  formSubmitted: boolean = false;
  parentFolder: number;
  folderId: number;
  folders: Folder[];

  constructor(private folderService: FolderService, private actRoute: ActivatedRoute) { }

  ngOnInit() {

    this.setParentFolderParam(+this.actRoute.snapshot.queryParams['parentFolderId']);
    this.isCreate(+this.actRoute.snapshot.params['id']);
    this.newFolder = new Folder("", this.parentFolder, 1, false);

    if (!this.IsCreate) {
      this.folderService.getFolder(3).subscribe((folder: Folder) => {
        this.newFolder = folder;
      });
    }

    this.form = new FolderFormGroup();

    this.folderService.getList().subscribe((folders: Folder[]) => {
      folders.unshift(new Folder("Root", null, null, false, 0));
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
    if (this.parentFolder == 0) this.newFolder.parentFolderId = undefined;
    this.folderService.createFolder(this.newFolder)
      .subscribe(result => {
        console.log(result['folderId']);
      }, error => error);
  }

  updateFolder() {
    console.log(this.newFolder);
    this.newFolder.id = this.folderId;
    this.folderService.updateFolder(this.newFolder)
      .subscribe(result => {

      }, error => error);
  }

  submitForm(form: NgForm) {
    this.formSubmitted = true;
    if (form.valid) {
      (this.IsCreate) ? this.createFolder() : this.updateFolder();
      if (this.IsCreate) form.reset();
      this.formSubmitted = false;
    }
  }

}
