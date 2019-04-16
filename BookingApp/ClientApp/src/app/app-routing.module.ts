import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
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
import { TosComponent } from './site/tos/tos.component';


const routes: Routes = [
  { path: '', component: TreeComponent, data: { breadcrumbIgnore: true } },
  { path: 'tos', component: TosComponent, data: { breadcrumbLabel: 'Terms of Service' } },
  { path: 'error/:status-code', component: ErrorComponent },
  { path: 'register', component: RegisterComponent, data: { breadcrumbLabel: 'Sign Up' } },
  { path: 'login', component: LoginComponent, data: { breadcrumbLabel: 'Sign In' } },
  { path: 'forget', component: ForgetComponent, data: { breadcrumbLabel: 'Restore Password' } },
  { path: 'reset', component: ResetComponent, data: { breadcrumbLabel: 'Change Password' } },
  { path: 'resources/:id', component: ResourceComponent, data: { breadcrumbLabel: 'Resource Details' } },
  {
    path: 'cabinet',
    loadChildren: () => CabinetModule,
    canLoad: [CabinetGuard],
    data: { breadcrumbLabel: 'User Cabinet' }
  },
  {
    path: 'admin',
    loadChildren: () => AdminModule,
    canLoad: [AdminGuard],
    data: { breadcrumbLabel: 'Admin CP' }
  },
  {
    path: '**',
    redirectTo: 'error/404'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
  providers: [CabinetGuard, AdminGuard],
})
export class AppRoutingModule { }
