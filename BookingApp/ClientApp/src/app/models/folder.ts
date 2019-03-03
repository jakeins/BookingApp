export class Folder {
  constructor(
    public title: string,
    public parentFolderId?: number,
    public defaultRuleId?: number,
    public isActive: boolean = false,
    public id?: number
  ) {}
}
