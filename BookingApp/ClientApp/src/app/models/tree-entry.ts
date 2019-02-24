export class TreeEntry {
  constructor(
    public id: number,
    public title: string,
    public isFolder: boolean,
    public isActive: boolean,
    public parentFolderId?: number
  ) {}
}
