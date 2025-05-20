using Microsoft.AspNetCore.Mvc;
using PriceComparisonWebsite.Services.Interfaces;

namespace PriceComparisonWebsite.Controllers.Api
{
    /// <summary>
    /// API controller for managing product-related operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProductApiController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductApiController> _logger;

        public ProductApiController(IProductService productService, ILogger<ProductApiController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves the price history for a specific product
        /// </summary>
        /// <param name="id">The ID of the product</param>
        /// <returns>
        /// 200 OK with the price history data containing dates and prices
        /// 500 Internal Server Error if an error occurs during processing
        /// </returns>
        [HttpGet("price-history/{id}")]
        public async Task<IActionResult> GetPriceHistory(int id)
        {
            try
            {
                var history = await _productService.GetPriceHistory(id);
                var result = new
                {
                    dates = history.Select(h => h.Timestamp.ToString("yyyy-MM-dd")),
                    prices = history.Select(h => h.Price)
                };
                _logger.LogInformation("Price history for product {id} retrieved successfully", id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get price history for product {id}", id);
                return StatusCode(500);
            }
        }
    }
}
