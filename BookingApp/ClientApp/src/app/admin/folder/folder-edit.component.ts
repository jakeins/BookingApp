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

  IsCreate: boolean = true;
  form: FolderFormGroup = new FolderFormGroup();
  newFolder: Folder = new Folder("");
  formSubmitted: boolean = false;
  folders: Folder;

  constructor(private folderService: FolderService, private actRoute: ActivatedRoute) { }

  ngOnInit() {
    this.isCreate(+this.actRoute.snapshot.params['id']);
    
    this.folderService.getList().subscribe((folders: Folder) => {
      folders[0] = new Folder("Root", null, null, false, undefined);
      this.folders = folders;
    });
  }

  isCreate(id: number) {
    if (!isNaN(id)) {
      this.IsCreate = false;
    }
  }

  createFolder() {
    console.log(this.newFolder);
    this.folderService.createFolder(this.newFolder)
      .subscribe(result => {
        console.log(result['folderId']);
      }, error => error);
  }

  submitForm(form: NgForm) {
    this.formSubmitted = true;
    if (form.valid) {
      this.createFolder();
      form.reset();
      this.formSubmitted = false;
    }
  }

}
