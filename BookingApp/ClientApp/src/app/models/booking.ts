export class Booking {
  public id: number;
  public resourceId: number;
  public startTime: Date;
  public endTime: Date;
  public note: string;
  public terminationTime?: Date;
  public createdUserId: string;
}
