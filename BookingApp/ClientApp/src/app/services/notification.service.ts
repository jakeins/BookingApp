import { Injectable } from '@angular/core';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material';

@Injectable()
export class NotificationService {

  constructor(
    public snackBar: MatSnackBar
  ) { }
  
  config: MatSnackBarConfig = {
    duration: 2500,
    horizontalPosition: 'right',
    verticalPosition: 'top'
  }

  success(message:string, action?: string, config?: string){
    this.config['panelClass'] = ['notification', 'success'];
    this.snackBar.open(message, '', this.config);
  }

  delete(message:string, action?: string, config?: string){
    this.config['panelClass'] = ['notification', 'delete'];
    this.snackBar.open(message, '', this.config);
  }

  submit(message:string, action?: string, config?: string){
    this.config['panelClass'] = ['notification', 'submit'];
    this.snackBar.open(message, '', this.config);
  }
}
