export interface PaginatedResponse<T> {
    data: T[];
    currentPage: number;
    totalPages: number;
    totalCount: number;
    success: boolean;
}