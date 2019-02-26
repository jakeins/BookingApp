export class Folder {
  constructor(
    public id: number,
    public title: string,
    public isActive: boolean,
    public parentFolderId?: number
  ) {}
}
