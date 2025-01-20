import { Component, OnInit } from '@angular/core';
import { SalesService } from '../../../../services/sales.service';
import { SaleResponseDto } from '../../dtos/sales.dto';

@Component({
  selector: 'app-sales-list',
  templateUrl: './sales-list.component.html',
  styleUrls: ['./sales-list.component.scss'],
  standalone: false,
})
export class SalesListComponent implements OnInit {
  displayedColumns: string[] = ['id', 'customer', 'saleDate', 'actions'];
  sales: SaleResponseDto[] = [];

  constructor(private salesService: SalesService) {}

  ngOnInit(): void {
    this.loadSales();
  }

  loadSales(): void {
    this.salesService.getAllSales().subscribe({
      next: (data) => {
        this.sales = data.data;
      },
      error: (err) => console.error('Error loading sales', err),
    });
  }

  onDelete(id: string): void {
    this.salesService.cancelSale(id).subscribe({
      next: () => this.loadSales(),
      error: (err) => console.error('Error deleting sale', err),
    });
  }
}
