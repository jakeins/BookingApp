import { Folders } from "./folders";
import { Resource } from "./resource";
import { Logger } from "../services/logger.service";

export class TreeEntry {
  //mutual original
  id: number;
  title: string;
  isActive: boolean;

  //derived original
  isFolder?: boolean;
  parentFolderId?: number;

  //resource-specific
  occupancy?: number;
  occupancyTitle?: string;

  constructor(original: Folders | Resource) {

    this.isFolder = (<Folders>original).parentFolderId !== undefined;

    this.id = original.id;
    this.title = original.title;
    this.isActive = true;//this.isActive;
    
    if (this.isFolder) {
      this.parentFolderId = (<Folders>original).parentFolderId;
    }
    else {
      this.parentFolderId = (<Resource>original).folderId;
    }

    //Logger.log(`${original.title} - ${this.isFolder}`);

  }
}
