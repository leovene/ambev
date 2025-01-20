using Ambev.Application.Sales.DTOs;
using Ambev.Domain.Sales.Interfaces.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.Application.Sales.Queries
{
    public class GetUnsyncedSalesHandler : IRequestHandler<GetUnsyncedSalesQuery, List<SaleResponseDto>>
    {
        private readonly ISaleQueryRepository _saleQueryRepository;
        private readonly IMapper _mapper;

        public GetUnsyncedSalesHandler(ISaleQueryRepository saleQueryRepository, IMapper mapper)
        {
            _saleQueryRepository = saleQueryRepository;
            _mapper = mapper;
        }

        public async Task<List<SaleResponseDto>> Handle(GetUnsyncedSalesQuery request, CancellationToken cancellationToken)
        {
            var result = await _saleQueryRepository.GetUnsyncedSalesAsync();
            return result.Select(e => _mapper.Map<SaleResponseDto>(e)).ToList();
        }
    }
}
