import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AdminRoutingModule } from './admin-routing.module';
import { HomeComponent } from './home/home.component';
import { UserComponent } from './user/user.component';
import { AdminComponent } from './admin.component';
import { ResourceEditComponent } from './resource/resource-edit.component';
import { FolderEditComponent } from './folder/folder-edit.component';
import { RulesComponent } from './rules/rules.component';
import { RuleComponent } from './rules/rule/rule.component';
import { RuleListComponent } from './rules/rule-list/rule-list.component';
import { UserNamePipe } from './user-name.pipe';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    AdminRoutingModule
  ],
  declarations: [
      AdminComponent,
      HomeComponent,
      UserComponent,
      ResourceEditComponent,
      FolderEditComponent,
      RulesComponent,
      RuleComponent,
      RuleListComponent,
      UserNamePipe
  ],
})
export class AdminModule { }
