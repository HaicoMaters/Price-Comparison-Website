using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace Price_Comparison_Website.Tests.Services
{
    public class LoginActivityServiceTests
    {
        
        private readonly Mock<IRepository<LoginActivity>> _loginActivityRepoMock;
        private readonly Mock<ILogger<LoginActivityService>> _loggerMock;
        private readonly ILoginActivityService _loginActivityService;

        public LoginActivityServiceTests(){
            _loginActivityRepoMock = new Mock<IRepository<LoginActivity>>();
            _loggerMock = new Mock<ILogger<LoginActivityService>>();

            _loginActivityService = new LoginActivityService(_loginActivityRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetNMostRecentActivities_WhenNActivitesExist_ShouldReturnNMostRecentActivitesMostRecentFirst(){
            // Arrange
            int n = 4;
            
            var loginActivities = new LoginActivity[]
            {
                new LoginActivity { Id = 1, UserId = "1", IpAddress = "1", IsSuccessful = true, LoginTime = DateTime.Now }, // expected order 1,4,2,3
                new LoginActivity { Id = 2, UserId = "2", IpAddress = "2", IsSuccessful = false, LoginTime = DateTime.Now.AddHours(-1) },
                new LoginActivity { Id = 3, UserId = "3", IpAddress = "3", IsSuccessful = true, LoginTime = DateTime.Now.AddDays(-1) },
                new LoginActivity { Id = 4, UserId = "4", IpAddress = "4", IsSuccessful = false, LoginTime = DateTime.Now.AddMinutes(-30) }
            };

            _loginActivityRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(loginActivities.OrderByDescending(la => la.LoginTime));

            // Act
            var activities = await _loginActivityService.GetNMostRecentActivities(n);
            List<LoginActivity> activityList = activities.ToList();

            // Assert
            Assert.Equal(n, activityList.Count);
            Assert.Equal(1, activityList[0].Id); // Most Recent
            Assert.Equal(4, activityList[1].Id);
            Assert.Equal(2, activityList[2].Id);
            Assert.Equal(3, activityList[3].Id); // Least Recent
        }

        [Fact]
        public async Task GetNMostRecentActivities_WhenMoreThanNActivitesExist_ShouldReturnNMostRecentActivitesMostRecentFirst(){
            // Arrange
            int n = 4;
            
            var loginActivities = new LoginActivity[]
            {
                new LoginActivity { Id = 1, UserId = "1", IpAddress = "1", IsSuccessful = true, LoginTime = DateTime.Now }, // expected order 1,4,2,3
                new LoginActivity { Id = 2, UserId = "2", IpAddress = "2", IsSuccessful = false, LoginTime = DateTime.Now.AddHours(-1) },
                new LoginActivity { Id = 3, UserId = "3", IpAddress = "3", IsSuccessful = true, LoginTime = DateTime.Now.AddDays(-1) },
                new LoginActivity { Id = 4, UserId = "4", IpAddress = "4", IsSuccessful = false, LoginTime = DateTime.Now.AddMinutes(-30) },
                new LoginActivity { Id = 5, UserId = "5", IpAddress = "5", IsSuccessful = false, LoginTime = DateTime.Now.AddMinutes(-5) }
            };

            _loginActivityRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(loginActivities.OrderByDescending(la => la.LoginTime));

            // Act
            var activities = await _loginActivityService.GetNMostRecentActivities(n);
            List<LoginActivity> activityList = activities.ToList();

            // Assert
            Assert.Equal(n, activityList.Count);
            Assert.Equal(1, activityList[0].Id); // Most Recent
            Assert.Equal(5, activityList[1].Id);
            Assert.Equal(4, activityList[2].Id); 
            Assert.Equal(2, activityList[3].Id); // Least Recent (Does not have id 5 due to n being 4)
        }   

        [Fact]
        public async Task GetNMostRecentActivities_WhenLessThanNActivitesExist_ShouldReturnAllActivitesMostRecentFirst(){
            // Arrange
            int n = 6;
            
            var loginActivities = new LoginActivity[]
            {
                new LoginActivity { Id = 1, UserId = "1", IpAddress = "1", IsSuccessful = true, LoginTime = DateTime.Now }, // expected order 1,4,2,3
                new LoginActivity { Id = 2, UserId = "2", IpAddress = "2", IsSuccessful = false, LoginTime = DateTime.Now.AddHours(-1) },
                new LoginActivity { Id = 3, UserId = "3", IpAddress = "3", IsSuccessful = true, LoginTime = DateTime.Now.AddDays(-1) },
                new LoginActivity { Id = 4, UserId = "4", IpAddress = "4", IsSuccessful = false, LoginTime = DateTime.Now.AddMinutes(-30) }
            };

            _loginActivityRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(loginActivities.OrderByDescending(la => la.LoginTime));

            // Act
            var activities = await _loginActivityService.GetNMostRecentActivities(n);
            List<LoginActivity> activityList = activities.ToList();

            // Assert
            Assert.Equal(4, activityList.Count); // Should take 4 although n is 6
            Assert.Equal(1, activityList[0].Id); // Most Recent
            Assert.Equal(4, activityList[1].Id);
            Assert.Equal(2, activityList[2].Id);
            Assert.Equal(3, activityList[3].Id); // Least Recent
        }

        [Fact]
        public async Task GetNMostRecentActivities_WhenNoActivitesExist_ShouldReturnEmpty(){
                // Arrange
                int n = 4;

                _loginActivityRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<LoginActivity>());

                // Act
                var activities = await _loginActivityService.GetNMostRecentActivities(n);
                var activityList = activities.ToList();

                // Assert
                Assert.Empty(activityList);  
        }
    }
}
