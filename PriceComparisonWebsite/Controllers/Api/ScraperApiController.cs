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
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [InternalOrAuthorized("Admin")]
    public class ScraperApiController : ControllerBase
    {
        IPriceScraperService _priceScraperService;
        ILogger<ScraperApiController> _logger;

        public ScraperApiController(IPriceScraperService priceScraperService, ILogger<ScraperApiController> logger)
        {
            _priceScraperService = priceScraperService;
            _logger = logger;
        }

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