export class Folder {
  constructor(
    public id: number,
    public title: string,
    public parentFolderId?: number
  ) {}
}
