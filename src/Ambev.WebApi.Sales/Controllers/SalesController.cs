using Ambev.Application.Sales.Commands;
using Ambev.Application.Sales.DTOs;
using Ambev.Application.Sales.Queries;
using Ambev.Domain.Common.Validation;
using Ambev.WebApi.Sales.Common;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.WebApi.Sales.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly Serilog.ILogger _logger;

        public SalesController(IMediator mediator, IMapper mapper, Serilog.ILogger logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        // POST: api/sales
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponseWithData<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateSale([FromBody] SaleRequestDto saleRequest)
        {
            _logger.Information("Received request to create a new sale.");

            var command = _mapper.Map<CreateSaleCommand>(saleRequest);
            try
            {
                var result = await _mediator.Send(command);

                _logger.Information("Sale created successfully with ID: {SaleId}", result);

                return Ok(new ApiResponseWithData<Guid>
                {
                    Success = true,
                    Message = "Sale created successfully.",
                    Data = result
                });
            }
            catch (ValidationException ex)
            {
                _logger.Warning("Validation failed while creating sale: {Errors}", ex.Errors);

                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Validation failed.",
                    Errors = ex.Errors.Select(e => new ValidationErrorDetail
                    {
                        Error = e.ErrorCode,
                        Detail = e.ErrorMessage
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while creating the sale.");
                throw;
            }
        }

        // PUT: api/sales/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSale(Guid id, [FromBody] SaleRequestDto saleRequest)
        {
            _logger.Information("Received request to update sale with ID: {SaleId}", id);

            var command = _mapper.Map<UpdateSaleCommand>(saleRequest);
            command.Id = id;

            try
            {
                await _mediator.Send(command);

                _logger.Information("Sale with ID {SaleId} updated successfully.", id);

                return Ok(new ApiResponse
                {
                    Message = "Sale updated successfully"
                });
            }
            catch (ValidationException ex)
            {
                _logger.Warning("Validation failed while updating sale with ID: {SaleId}. Errors: {Errors}", id, ex.Errors);

                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Validation failed.",
                    Errors = ex.Errors.Select(e => new ValidationErrorDetail
                    {
                        Error = e.ErrorCode,
                        Detail = e.ErrorMessage
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while updating the sale with ID: {SaleId}", id);
                throw; 
            }
        }

        // DELETE: api/sales/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSale(Guid id)
        {
            var command = new DeleteSaleCommand { Id = id };

            try
            {
                await _mediator.Send(command);
                return Ok(new ApiResponse
                {
                    Message = "Sale deleted successfully"
                });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Validation failed.",
                    Errors = ex.Errors.Select(e => new ValidationErrorDetail
                    {
                        Error = e.ErrorCode,
                        Detail = e.ErrorMessage
                    })
                });
            }
        }

        // GET: api/sales
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponseWithData<IEnumerable<SaleResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllSales([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, bool withItems = true)
        {
            _logger.Information("Received request to retrieve all sales with items: {WithItems}", withItems);

            var query = new GetAllSalesQuery { WithItems = withItems };
            var result = await _mediator.Send(query);

            var paginatedResult = PaginatedList<SaleResponseDto>.Create(result, pageNumber, pageSize);

            _logger.Information("Successfully retrieved paginated sales: Page {PageNumber}, Size {PageSize}", paginatedResult.CurrentPage, paginatedResult.PageSize);

            return OkPaginated(paginatedResult);
        }

        // GET: api/sales/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponseWithData<SaleResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSaleById(Guid id, bool withItems = true)
        {
            _logger.Information("Received request to get sale with ID: {SaleId}", id);

            var query = new GetSaleByIdQuery { Id = id, WithItems = withItems };
            var result = await _mediator.Send(query);

            return result == null ? NotFound(new ApiResponse { Message = "Sale not found" }) : Ok(new ApiResponseWithData<SaleResponseDto>
            {
                Success = true,
                Message = "Sale retrieved successfully",
                Data = result
            });
        }

        [HttpPut("{id}/cancel")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelSale(Guid id)
        {
            _logger.Information("Received request to cancel sale with ID: {SaleId}", id);

            var command = new CancelSaleCommand { Id = id };

            try
            {
                await _mediator.Send(command);

                _logger.Information("Sale with ID {SaleId} cancelled successfully.", id);

                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.Warning("Validation failed while cancelling sale with ID: {SaleId}. Errors: {Errors}", id, ex.Errors);

                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Validation failed.",
                    Errors = ex.Errors.Select(e => new ValidationErrorDetail
                    {
                        Error = e.ErrorCode,
                        Detail = e.ErrorMessage
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while updating the sale with ID: {SaleId}", id);
                throw;
            }
        }

        [HttpPut("{saleId}/items/cancel")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelSaleItems(Guid saleId, [FromBody] List<Guid> itemIds)
        {
            _logger.Information("Received request to cancel items for sale with ID: {SaleId}", saleId);

            var command = new CancelSaleItemsCommand
            {
                SaleId = saleId,
                ItemIds = itemIds
            };

            try
            {
                await _mediator.Send(command);

                _logger.Information("Items cancelled successfully for sale with ID: {SaleId}", saleId);

                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.Warning("Validation failed while cancelling items for sale with ID: {SaleId}. Errors: {Errors}", saleId, ex.Errors);

                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Validation failed.",
                    Errors = ex.Errors.Select(e => new ValidationErrorDetail
                    {
                        Error = e.ErrorCode,
                        Detail = e.ErrorMessage
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while updating the sale with ID: {SaleId}", saleId);
                throw;
            }
        }
    }
}
