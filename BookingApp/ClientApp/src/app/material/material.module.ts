import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import * as Material from '@angular/material';
import { MatDatepickerModule } from '@angular/material';
import { MccTimerPickerModule } from 'material-community-components';
import { UserNamePipe } from '../admin/rule.pipe';
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

@NgModule({
  imports: [
    CommonModule,
    Material.MatToolbarModule,
    Material.MatGridListModule,
    Material.MatInputModule,
    Material.MatFormFieldModule,
    Material.MatCheckboxModule,
    Material.MatButtonModule,
    Material.MatTableModule,
    Material.MatIconModule,
    Material.MatPaginatorModule,
    Material.MatSortModule,
    Material.MatDialogModule,
    Material.MatSnackBarModule,
    Material.MatSliderModule,
    Material.MatDatepickerModule,
    Material.MatNativeDateModule,
    Material.MatSelectModule,
    ReactiveFormsModule,
    Material.MatCardModule,
    Material.MatInputModule,
    Material.MatFormFieldModule,
    Material.MatIconModule,
    Material.MatTabsModule,
    MccTimerPickerModule
  ],
  exports: [
    Material.MatToolbarModule,
    Material.MatGridListModule,
    Material.MatInputModule,
    Material.MatFormFieldModule,
    Material.MatCheckboxModule,
    Material.MatButtonModule,
    Material.MatTableModule,
    Material.MatIconModule,
    Material.MatPaginatorModule,
    Material.MatSortModule,
    Material.MatDialogModule,
    Material.MatSnackBarModule, 
    Material.MatSliderModule,
    Material.MatDatepickerModule,
    Material.MatNativeDateModule,
    Material.MatTable,
    Material.MatSelectModule,
    UserNamePipe,
    ReactiveFormsModule,
    Material.MatCardModule,
    Material.MatInputModule,
    Material.MatFormFieldModule,
    Material.MatIconModule,
    Material.MatTabsModule,
    MccTimerPickerModule
  ],
  declarations: [
    UserNamePipe
  ]
})
export class MaterialModule { }
