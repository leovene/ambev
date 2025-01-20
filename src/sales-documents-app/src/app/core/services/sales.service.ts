import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PaginatedResponse } from '../features/sales/dtos/paginated.response.dto';
import { SaleResponseDto } from '../features/sales/dtos/sales.dto';
import { SaleRequestDto } from '../features/sales/dtos/sale-request.dto';

@Injectable({
  providedIn: 'root',
})
export class SalesService {
  private readonly baseUrl = 'https://localhost:7237/api/sales';

  constructor(private http: HttpClient) {}

  getAllSales(pageNumber = 1, pageSize = 10, withItems = true): Observable<PaginatedResponse<SaleResponseDto>> {
    return this.http.get<PaginatedResponse<SaleResponseDto>>(this.baseUrl, {
      params: { pageNumber, pageSize, withItems },
    });
  }

  getSaleById(id: string, withItems = true): Observable<SaleResponseDto> {
    return this.http.get<SaleResponseDto>(`${this.baseUrl}/${id}`, { params: { withItems } });
  }

  createSale(sale: SaleRequestDto): Observable<string> {
    return this.http.post<string>(this.baseUrl, sale);
  }

  updateSale(id: string, sale: SaleRequestDto): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, sale);
  }

  cancelSale(id: string): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}/cancel`, null);
  }

  cancelSaleItems(saleId: string, itemIds: string[]): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${saleId}/items/cancel`, itemIds);
  }
}