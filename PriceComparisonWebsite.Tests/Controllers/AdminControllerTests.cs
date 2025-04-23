using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using PriceComparisonWebsite.Services.WebScraping.Interfaces;

namespace PriceComparisonWebsite.Tests.Controllers
{
    public class AdminControllerTests
    {
        // most of these tests need to be rewritten, due to adding notification api


      /*  private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<ILogger<AdminController>> _loggerMock;
        private readonly Mock<ILoginActivityService> _loginActivityServiceMock;
        private readonly Mock<IAdminService> _adminServiceMock;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<IScraperStatusService> _scraperStatusServiceMock;
        private readonly AdminController _adminController;

        public AdminControllerTests()
        {
            _adminServiceMock = new Mock<IAdminService>();
            _notificationServiceMock = new Mock<INotificationService>();
            _loggerMock = new Mock<ILogger<AdminController>>();
            _loginActivityServiceMock = new Mock<ILoginActivityService>();

            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _configurationMock = new Mock<IConfiguration>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _scraperStatusServiceMock = new Mock<IScraperStatusService>();

            // Mocking UserManager<ApplicationUser>
            var store = new Mock<IUserStore<ApplicationUser>>();

            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null
            );

            _adminController = new AdminController(
                _adminServiceMock.Object,
                _userManagerMock.Object,
                _notificationServiceMock.Object,
                _loggerMock.Object,
                _loginActivityServiceMock.Object,
                _httpClientFactoryMock.Object,
                _configurationMock.Object,
                _httpContextAccessorMock.Object,
                _scraperStatusServiceMock.Object
            );
        }

        */
    }
}