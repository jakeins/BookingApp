import { Component, OnInit, Inject } from '@angular/core';
import { FolderService } from '../../services/folder.service';
import { Folders } from '../../models/folders';

@Component({
  selector: 'app-folder',
  templateUrl: './folder.component.html'
})
export class FolderComponent implements OnInit {

  folders: Folders;

  constructor(private folderService: FolderService) { }

  ngOnInit() {
    this.folderService.getList().subscribe((folders: Folders) => {
      this.folders = folders;
    });
  }

}
