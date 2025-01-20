using Microsoft.AspNetCore.Mvc;

namespace Ambev.WebApi.Sales.Common
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IActionResult Ok(string message) =>
                base.Ok(new ApiResponse { Message = message, Success = true });

        protected IActionResult Ok<T>(T data) =>
                base.Ok(new ApiResponseWithData<T> { Data = data, Success = true });

        protected IActionResult Created<T>(string routeName, object routeValues, T data) =>
            base.CreatedAtRoute(routeName, routeValues, new ApiResponseWithData<T> { Data = data, Success = true });

        protected IActionResult BadRequest(string message) =>
            base.BadRequest(new ApiResponse { Message = message, Success = false });

        protected IActionResult NotFound(string message = "Resource not found") =>
            base.NotFound(new ApiResponse { Message = message, Success = false });

        protected IActionResult OkPaginated<T>(PaginatedList<T> pagedList) =>
                base.Ok(new PaginatedResponse<T>
                {
                    Data = pagedList.AsEnumerable(),
                    CurrentPage = pagedList.CurrentPage,
                    TotalPages = pagedList.TotalPages,
                    TotalCount = pagedList.TotalCount,
                    Success = true
                });
    }
}
