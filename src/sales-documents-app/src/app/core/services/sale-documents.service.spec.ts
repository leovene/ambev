import { TestBed } from '@angular/core/testing';

import { SaleDocumentsService } from './sale-documents.service';

describe('SaleDocumentsService', () => {
  let service: SaleDocumentsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SaleDocumentsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
