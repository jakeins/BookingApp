import { Component, OnInit, Inject } from '@angular/core';
import { FolderService } from '../../services/folder.service';
import { Folder } from '../../models/folder';

@Component({
  selector: 'app-folder',
  templateUrl: './folder.component.html'
})
export class FolderComponent implements OnInit {

  folders: Folder;

  constructor(private folderService: FolderService) { }

  ngOnInit() {
    this.folderService.getList().subscribe((folders: Folder) => {
      this.folders = folders;
    });
  }

}
