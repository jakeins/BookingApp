import { Booking } from "./booking";

export enum ResourceTimeWindowType {
  Booked, ServiceTime, Free
}

export class ResourceTimeWindow {
  public startTime: Date;
  public endTime: Date;
  public type: ResourceTimeWindowType;
  public booking?: Booking;
}
