import { Component, OnInit, ViewChild } from '@angular/core';
import { SaleResponseDto } from '../../dtos/sales.dto';
import { SaleDocumentsService } from '../../../../services/sale-documents.service';
import { PaginatedResponse } from '../../dtos/paginated.response.dto';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-sales-list',
  templateUrl: './sales-document-list.component.html',
  styleUrls: ['./sales-document-list.component.scss'],
  standalone: false,
})
export class SalesDocumentListComponent implements OnInit {
  displayedColumns: string[] = ['id', 'customerId', 'saleDate', 'branch', 'itemsCount'];
  dataSource = new MatTableDataSource<SaleResponseDto>([]);
  @ViewChild(MatPaginator) paginator!: MatPaginator;


  constructor(private salesService: SaleDocumentsService) {}

  ngOnInit(): void {
    this.fetchSales();
  }

  fetchSales(): void {
    this.salesService.getAllSales().subscribe({
      next: (response) => {
        this.dataSource.data = response.data;
        this.dataSource.paginator = this.paginator;
      },
      error: (err) => {
        console.error('Error fetching sales', err);
      },
    });
  }
}
