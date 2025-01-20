import { Routes } from '@angular/router';

export const routes: Routes = [
    { path: 'sales', loadChildren: () => import('./core/features/sales/sales.module').then(m => m.SalesModule) },
    { path: '', redirectTo: '/sales', pathMatch: 'full' },
];

