export class Folders {
  constructor(
    public id: number,
    public title: string,
    public parentFolderId?: number
  ) {}
}
