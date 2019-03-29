import { Injectable } from "@angular/core";

export enum BookingsComponentMode {
  Admin, User, Resource
}

@Injectable()
export class BookingsModeService {
  public currentMode: BookingsComponentMode;

  public userId: string;
  public resourceId: number;

  constructor() {
    this.currentMode = BookingsComponentMode.Resource;
    this.userId = "";
    this.resourceId = 1;
  }
}
