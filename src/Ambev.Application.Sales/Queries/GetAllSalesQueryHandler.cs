using Ambev.Application.Sales.DTOs;
using Ambev.Domain.Sales.Interfaces.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.Application.Sales.Queries
{
    public class GetAllSalesQueryHandler : IRequestHandler<GetAllSalesQuery, List<SaleResponseDto>>
    {
        private readonly ISaleQueryRepository _saleQueryRepository;
        private readonly IMapper _mapper;

        public GetAllSalesQueryHandler(ISaleQueryRepository saleQueryRepository, IMapper mapper)
        {
            _saleQueryRepository = saleQueryRepository;
            _mapper = mapper;
        }

        public async Task<List<SaleResponseDto>> Handle(GetAllSalesQuery request, CancellationToken cancellationToken)
        {
            var result = request.WithItems ? await _saleQueryRepository.GetAllWithItemsAsync() : await _saleQueryRepository.GetAllAsync("Sales");
            return result.Select(e => _mapper.Map<SaleResponseDto>(e)).ToList();
        }
    }
}
