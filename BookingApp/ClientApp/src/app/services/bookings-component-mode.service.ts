import { Injectable } from "@angular/core";

@Injectable()
export class BookingsModeService {
  public currentMode: string;

  public userId: string;
  public resourceId: number;

  constructor() {
    this.currentMode = "res";
    this.userId = "";
    this.resourceId = 1;
  }
}
