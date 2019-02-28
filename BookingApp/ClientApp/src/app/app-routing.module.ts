import { NgModule } from '@angular/core';
import { PreloadAllModules, RouterModule, Routes } from '@angular/router';
import { ForgetComponent } from './site/auth/forget/forget.component';
import { LoginComponent } from './site/auth/login/login.component';
import { ResetComponent } from './site/auth/reset/reset.component';
import { RegisterComponent } from './site/auth/register/register.component';
import { FolderComponent } from './site/folder/folder.component';
import { CabinetGuard } from './cabinet/cabinet.guard';
import { AdminGuard } from './admin/admin.guard';
import { ErrorComponent } from './site/error/error.component';
import { ResourceComponent } from './site/resource/resource.component';
import { ResourceEditComponent } from './site/resource/resource-edit.component';
import { TreeComponent } from './site/tree/tree.component';
import { CabinetModule } from './cabinet/cabinet.module';
import { AdminModule } from './admin/admin.module';



const routes: Routes = [
  { path: '', component: TreeComponent },
  { path: 'error', component: ErrorComponent },

  { path: 'register', component: RegisterComponent },
  { path: 'login', component: LoginComponent },
  { path: 'forget', component: ForgetComponent },
  { path: 'reset', component: ResetComponent },

  { path: 'folders', component: FolderComponent },

  { path: 'resources/create', component: ResourceEditComponent, canActivate: [AdminGuard] },
  { path: 'resources/:id', component: ResourceComponent },
  { path: 'resources/:id/edit', component: ResourceEditComponent, canActivate: [AdminGuard] },

  { path: 'tree', component: TreeComponent },

  {
      path: 'cabinet',
      loadChildren: () => CabinetModule,
      //loadChildren: './cabinet/cabinet.module#CabinetModule',
      canLoad: [CabinetGuard]
  },
  {
      path: 'admin',
      loadChildren: () => AdminModule,
      canLoad: [AdminGuard]
  },
  {
      path: '**',
      redirectTo: 'error'
  }
];

@NgModule({
    /*
    imports: [
        RouterModule.forRoot(routes,
            {
                preloadingStrategy: PreloadAllModules
            })
    ],
    */
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule],
    providers: [ CabinetGuard, AdminGuard ],
})
export class AppRoutingModule { }
