import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SalesDocumentListComponent } from './components/sales-document-list/sales-document-list.component';
import { SalesListComponent } from './components/sales-list/sales-list.component';
import { SalesFormComponent } from './components/sales-form/sales-form.component';

const routes: Routes = [
  { path: '', redirectTo: 'sales-list', pathMatch: 'full' },
  { path: 'sales-list', component: SalesListComponent },
  { path: 'sales-documents', component: SalesDocumentListComponent },
  { path: 'sales-form', component: SalesFormComponent },
  { path: 'sales-form/:id', component: SalesFormComponent }, 
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class SalesRoutingModule {}
