export class Resource{
  constructor(
    public id: number,
    public title: string,
    public isActive: boolean,
    public parentFolderId?: number,
    public description?: string
  ) {
    }
}
