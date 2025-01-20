import { SaleItemRequestDto } from "./sale-item-request.dto";

export interface SaleRequestDto {
    customerId: string;
    branch: string;
    items: SaleItemRequestDto[];
}  