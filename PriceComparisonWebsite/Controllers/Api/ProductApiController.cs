using Microsoft.AspNetCore.Mvc;
using PriceComparisonWebsite.Services.Interfaces;

namespace PriceComparisonWebsite.Controllers.Api
{
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
