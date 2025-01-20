import { SaleItemDto } from "./sale.item.dto";

export interface SaleResponseDto {
    id: string;
    customerId: string;
    saleDate: string;
    branch: string;
    items: SaleItemDto[];
}  