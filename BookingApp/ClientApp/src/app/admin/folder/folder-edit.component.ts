import { Component, OnInit, Inject } from '@angular/core';
import { FolderService } from '../../services/folder.service';
import { Folder } from '../../models/folder';

@Component({
  selector: 'app-folder-edit',
  templateUrl: './folder-edit.component.html'
})
export class FolderEditComponent implements OnInit {

  folders: Folder;

  constructor(private folderService: FolderService) { }

  ngOnInit() {
    this.folderService.getList().subscribe((folders: Folder) => {
      this.folders = folders;
    });
  }

}
