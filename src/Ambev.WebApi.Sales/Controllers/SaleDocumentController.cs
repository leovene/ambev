using Ambev.Application.Sales.DTOs;
using Ambev.Application.Sales.Queries;
using Ambev.WebApi.Sales.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.WebApi.Sales.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesDocumentController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly Serilog.ILogger _logger;

        public SalesDocumentController(IMediator mediator, Serilog.ILogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // GET: api/sales
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponseWithData<IEnumerable<SaleResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllSaleDocuments([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, bool withItems = true)
        {
            _logger.Information("Received request to retrieve all sale documents.");

            var query = new GetAllSaleDocumentsQuery();
            var result = await _mediator.Send(query);

            var paginatedResult = PaginatedList<SaleResponseDto>.Create(result, pageNumber, pageSize);

            _logger.Information("Successfully retrieved paginated sales: Page {PageNumber}, Size {PageSize}", paginatedResult.CurrentPage, paginatedResult.PageSize);

            return OkPaginated(paginatedResult);
        }
    }
}
