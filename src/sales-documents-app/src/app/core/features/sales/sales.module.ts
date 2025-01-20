import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SalesRoutingModule } from './sales-routing.module';
import { SalesDocumentListComponent } from './components/sales-document-list/sales-document-list.component';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { ReactiveFormsModule } from '@angular/forms';
import { SalesListComponent } from './components/sales-list/sales-list.component';
import { SalesFormComponent } from './components/sales-form/sales-form.component';

@NgModule({
  declarations:[
    SalesDocumentListComponent,
    SalesListComponent,
    SalesFormComponent,
  ],
  imports:[
    CommonModule,
    SalesRoutingModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatButtonModule,
    MatInputModule,
    ReactiveFormsModule,
  ],
})
export class SalesModule {}
