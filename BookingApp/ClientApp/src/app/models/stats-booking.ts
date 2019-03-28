export class BookingStats {
  type: string;
  fromDate: Date;
  toDate: Date;
  interval: string;
  intervalsValues: Date[];
  bookingsAll: number[];
  bookingsByResources: { [key: number]: number[]; };
}
