using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceComparisonWebsite.Attributes;
using PriceComparisonWebsite.Services.WebScraping.Interfaces;

namespace PriceComparisonWebsite.Controllers.Api
{
    /// <summary>
    /// API controller for managing web scraping operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [InternalOrAuthorized("Admin")]
    public class ScraperApiController : ControllerBase
    {
        private readonly IPriceScraperService _priceScraperService;
        private readonly ILogger<ScraperApiController> _logger;

        public ScraperApiController(IPriceScraperService priceScraperService, ILogger<ScraperApiController> logger)
        {
            _priceScraperService = priceScraperService;
            _logger = logger;
        }

        /// <summary>
        /// Initiates the process to update all product listings through web scraping
        /// </summary>
        /// <returns>
        /// 204 No Content if the update process completed successfully
        /// 500 Internal Server Error if an error occurs during processing
        /// </returns>
        [HttpPatch("update-all-listings")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]      // Successful operation
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Failure
        public async Task<IActionResult> UpdateAllListings()
        {
            try
            {
                await _priceScraperService.UpdateAllListings();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update listings");
                return StatusCode(500, new { error = "An error occurred while updating listings.", details = ex.Message });
            }
        }
    }
}