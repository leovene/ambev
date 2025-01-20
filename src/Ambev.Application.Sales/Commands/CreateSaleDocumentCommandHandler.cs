using Ambev.Domain.Sales.Entities;
using Ambev.Domain.Sales.Interfaces.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.Application.Sales.Commands
{
    public class CreateSaleDocumentCommandHandler : IRequestHandler<CreateSaleDocumentCommand>
    {
        private readonly ISaleDocumentRepository _saleDocumentRepository;
        private readonly IMapper _mapper;
        private readonly Serilog.ILogger _logger;

        public CreateSaleDocumentCommandHandler(ISaleDocumentRepository saleDocumentRepository, IMapper mapper, Serilog.ILogger logger)
        {
            _saleDocumentRepository = saleDocumentRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task Handle(CreateSaleDocumentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.Information("Starting to process CreateSaleDocumentCommand for sale with ID: {SaleId}", request.Id);

                var saleDocument = _mapper.Map<SaleDocumentEntity>(request);

                await _saleDocumentRepository.AddSaleAsync(saleDocument);

                _logger.Information("Sale document with ID: {SaleId} successfully saved to MongoDB.", request.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while saving sale document with ID: {SaleId} to MongoDB.", request.Id);
                throw;
            }
        }
    }
}
