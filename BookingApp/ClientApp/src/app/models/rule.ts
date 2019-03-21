export class rule {
  id?: number;
  title: string;
  minTime: number;
  maxTime: number;
  stepTime: number;
  serviceTime: number;
  preOrderTimeLimit: number;
  reuseTimeout: number;
  isActive: boolean
}
