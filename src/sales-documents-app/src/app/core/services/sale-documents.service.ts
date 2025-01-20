import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PaginatedResponse } from '../features/sales/dtos/paginated.response.dto';
import { SaleResponseDto } from '../features/sales/dtos/sales.dto';
import { HttpClient, HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class SaleDocumentsService {
  private readonly baseUrl = 'https://localhost:7237/api/SalesDocument';
  constructor(private http: HttpClient) { }

  getAllSales(
    pageNumber: number = 1,
    pageSize: number = 10,
    withItems: boolean = true
  ): Observable<PaginatedResponse<SaleResponseDto>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize)
      .set('withItems', withItems);

    return this.http.get<PaginatedResponse<SaleResponseDto>>(this.baseUrl, { params });
  }
}
