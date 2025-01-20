using Ambev.Application.Sales.DTOs;
using Ambev.Domain.Sales.Interfaces.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.Application.Sales.Queries
{
    public class GetAllSaleDocumentsQueryHandler : IRequestHandler<GetAllSaleDocumentsQuery, List<SaleResponseDto>>
    {
        private readonly ISaleDocumentRepository _saleDocumentRepository;
        private readonly IMapper _mapper;

        public GetAllSaleDocumentsQueryHandler(ISaleDocumentRepository saleDocumentRepository, IMapper mapper)
        {
            _saleDocumentRepository = saleDocumentRepository;
            _mapper = mapper;
        }

        public async Task<List<SaleResponseDto>> Handle(GetAllSaleDocumentsQuery request, CancellationToken cancellationToken)
        {
            var result = await _saleDocumentRepository.GetAllSalesAsync();
            return result.Select(e => _mapper.Map<SaleResponseDto>(e)).ToList();
        }
    }
}
