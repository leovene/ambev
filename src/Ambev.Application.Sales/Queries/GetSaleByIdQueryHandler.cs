using Ambev.Application.Sales.DTOs;
using Ambev.Domain.Sales.Interfaces.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.Application.Sales.Queries
{
    public class GetSaleByIdQueryHandler : IRequestHandler<GetSaleByIdQuery, SaleResponseDto?>
    {
        private readonly ISaleQueryRepository _saleQueryRepository;
        private IMapper _mapper;

        public GetSaleByIdQueryHandler(ISaleQueryRepository saleQueryRepository, IMapper mapper)
        {
            _saleQueryRepository = saleQueryRepository;
            _mapper = mapper;
        }

        public async Task<SaleResponseDto?> Handle(GetSaleByIdQuery request, CancellationToken cancellationToken)
        {
            var result = request.WithItems ? await _saleQueryRepository.GetByIdWithItemsAsync(request.Id) : await _saleQueryRepository.GetByIdAsync("Sales", request.Id);

            if (result == null)
            {
                return null;
            }

            return _mapper.Map<SaleResponseDto>(result);
        }
    }
}
