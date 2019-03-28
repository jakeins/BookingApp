import { NgModule } from '@angular/core';
import { PreloadAllModules, RouterModule, Routes } from '@angular/router';
import { ForgetComponent } from './site/auth/forget/forget.component';
import { LoginComponent } from './site/auth/login/login.component';
import { ResetComponent } from './site/auth/reset/reset.component';
import { RegisterComponent } from './site/auth/register/register.component';
import { CabinetGuard } from './cabinet/cabinet.guard';
import { AdminGuard } from './admin/admin.guard';
import { ErrorComponent } from './site/error/error.component';
import { ResourceComponent } from './site/resource/resource.component';
import { TreeComponent } from './site/tree/tree.component';
import { CabinetModule } from './cabinet/cabinet.module';
import { AdminModule } from './admin/admin.module';




const routes: Routes = [
  { path: '', component: TreeComponent, data: { breadcrumbIgnore: true } },
  //{ path: 'tree', component: TreeComponent },
  { path: 'error/:status-code', component: ErrorComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'login', component: LoginComponent },
  { path: 'forget', component: ForgetComponent },
  { path: 'reset', component: ResetComponent },
  { path: 'resources/:id', component: ResourceComponent, data:{ breadcrumbLabel: 'Resource details' } },
  
  {
    path: 'cabinet',
    loadChildren: () => CabinetModule,
    //loadChildren: './cabinet/cabinet.module#CabinetModule',
    canLoad: [CabinetGuard],
    data: { breadcrumbLabel: 'User Cabinet' }
  },
  {
    path: 'admin',
    loadChildren: () => AdminModule,
    //loadChildren: './admin/admin.module#AdminModule',
    canLoad: [AdminGuard],
    data: { breadcrumbLabel: 'Admin CP' }
  },
  {
    path: '**',
    redirectTo: 'error/404'
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
  providers: [CabinetGuard, AdminGuard],
})
export class AppRoutingModule { }
