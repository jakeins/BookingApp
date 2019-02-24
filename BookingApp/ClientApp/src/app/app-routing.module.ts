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


const routes: Routes = [
  { path: '', component: FolderComponent },
    { path: 'error', component: ErrorComponent},
    { path: 'register', component: RegisterComponent },
    { path: 'login', component: LoginComponent },
    { path: 'forget', component: ForgetComponent },
    { path: 'reset', component: ResetComponent },
    {
        path: 'cabinet',
        loadChildren: './cabinet/cabinet.module#CabinetModule',
        canLoad: [CabinetGuard]
    },
    {
        path: 'admin',
        loadChildren: './admin/admin.module#AdminModule',
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
