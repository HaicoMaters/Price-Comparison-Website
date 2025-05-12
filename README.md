# PriceComparisonWebsite

   This project was built to help me learn **ASP.NET MVC** and enhance my **C# programming** skills. It is a simple platform where users can compare prices of a particular item across multiple vendors/websites. The platform includes several key features to facilitate the price comparison process and improve user experience.

   Much of the codebase varies in quality due to my learning process resulting in the earlier code to be significantly worse than the later code. However, much of the earlier code has been rewritten as i learned more about it. For example; early on almost all the buisness logic was in the controllers, whilst as i learnt more about ASP.NET, much of this code was moved to the services and often rewritten to be better.

   ## Table of Contents
   - [Getting Started](#getting-started)
   - [Core Features](#core-features)
   - [Key Features](#key-features)
   - [Test Data](#test-data)
   - [Admin Login](#admin-login)
   - [Technical Details](#technical-details)
     - [Web Scraping System](#web-scraping-system)
     - [Database Relationships](#database-relationships)
     - [Testing](#testing)
   - [Technologies Used](#technologies-used)
   - [TODO](#todo)
   - [Project Structure](#project-structure)

   ## Getting Started

   ### Prerequisites
   - .NET 7.0 SDK
   - SQL Server (LocalDB or higher)
   - Visual Studio 2022 or VS Code

   ### Installation
   1. Clone the repository (replace `YourRepoDirectory` with where you want to store the project)
   ```bash
   git clone https://github.com/HaicoMaters/PriceComparisonWebsite.git YourRepoDirectory
   ```

   2. Navigate to the project directory (Of main project PriceComparionWebsite/PriceComparisonWebsite)
   ```bash
   cd PriceComparisonWebsite
   ```

   3. Restore dependencies
   ```bash
   dotnet restore
   ```

   4. Update database
   ```bash
   dotnet ef database update
   ```

   5. Run the application
   ```bash
   dotnet run
   ```
Note: In VSCode I would just use the run web app launch configuration for ease of use

   To run the tests I recommend using this (from the main directiory i.e. where both the PriceComparisonWebsite and PriceComparisonWebsite.Tests files are) to output to a file, all tests should be  automatically ran with every push to github so you can also just view from github actions.
   
   ```bash
   dotnet test "PriceComparisonWebsite.Tests/PriceComparisonWebsite.Tests.csproj" --verbosity normal --logger "console;verbosity=detailed" --logger "trx;LogFileName=test-results.trx"
   ```

   ## Core Features

   - **Product Listings**: Users can view products, vendors, and price listings as well as graphs including the price history of individual products.
   - **Search Functionality**: Users can search for products by keywords in their description or title, and filter by categories. Pagination has been added to allow for better navigation of large product listings.
   - **User Accounts and Roles**:
   - **Admin**: Admin users can add, edit, and delete vendors, products, and listings. Also, admins have access to a admin panel.
   - **Basic User**: Regular users can create wishlists, view their browsing history, and receive notifications. Wishlisted items are visually distinguished yellow star icon

   ## Key Features

   - **Wishlist & Viewing History**: 
      - Users can add items to their wishlist and track their browsing history
      - Items on the wishlist are visually highlighted
      - Price drop notifications for wishlisted items
   
   - **Notification System**:
      - Real-time price drop alerts for wishlisted items
      - Read/Unread status tracking
      - Dismissible notifications
      - Notification counter badge
      - Automated notifications when prices decrease
   
   - **Admin Capabilities**: 
      - Full control over product data (add, edit, delete)
      - Last updated timestamps for transparency
      - Login history tracking
      - Admin Panel for sending global notifications and viewing website statistics and monitoring site login activity/attempts
   
   - **Manual Updates With Partial Support of Automatic Updates**: 
      - Some vendors that are added for example amazon support automatic updates
      - This is done via a web scraping system
      - For supportted vendors with a parser i.e. amazon automatic updates can be done
      - Otherwise the updates are required to be done manually

   ## Test Data

   The website includes some real product data with actual prices from various vendors to demonstrate the functionality. While this data is sufficient for basic testing, you're welcome to add your own data to:
   - Test pagination with larger datasets
   - Create more complex price comparisons
   - Experiment with different product categories
   - Test the notification system with custom price changes

   ## Admin Login

   For testing purposes, an admin account has been created. You can log in using the following credentials:

   - **Email**: `admin@admin.com`
   - **Password**: `Test1234`

   These credentials are also available in the `Program.cs` file for convenience.

   ## Technical Details

   ### Web Scraping System

   The website implements a robust, automated web scraping system that maintains up-to-date product prices across multiple vendors while ensuring ethical and efficient data collection.

   #### Key Components

   ##### Automated Price Updates
   - **Scheduled Updates**: 
      - Runs automatically every 8 hours
      - Configurable update intervals via `PriceScraperBackgroundService`
      - Manual triggers available through admin dashboard
      - Real-time progress monitoring via SignalR

   ##### Smart Vendor Management
   - **Automatic Vendor Detection**:
      - Dynamic parser assignment based on vendor domains
      - Automatic capability detection for new vendors
      - Self-updating vendor support flags

   - **Vendor Compatibility**:
      - `SupportsAutomaticUpdates` flag tracks scraping eligibility
      - Automatic verification of:
         - Parser availability
         - Robots.txt compliance
         - Site structure compatibility

   ##### Safety & Compliance Features
   - **Ethical Scraping**:
      - Robots.txt validation for each domain
      - Cached robots.txt responses (24-hour TTL)
      - Respects vendor-specific crawl delays
   
   - **Rate Limiting**:
      - Domain-based request queuing
      - 2-second minimum delay between requests
      - Per-domain cooldown periods
      - Concurrent request management

   - **Compression Handling**:
      - Automatic GZIP compression/decompression
      - Reduced bandwidth usage (70-90% smaller responses)
      - Browser-like request headers
      - Efficient memory management
      - Proper encoding of international characters

   - **Fault Tolerance**:
      - 3-attempt retry mechanism with exponential backoff
      - Intelligent error categorization
      - Automatic recovery from transient failures
      - Detailed failure logging

   ##### Real-time Monitoring
   - **Live Updates**:
      - SignalR-powered progress indicators
      - Real-time log streaming to admin dashboard
      - Success/failure notifications
      - Price change tracking

   #### Technical Architecture

   ```
   /Services
   ├── WebScraping/
   │   ├── PriceScraperService.cs         # Main orchestration service
   │   ├── PriceParserFactory.cs          # Parser management
   │   ├── ScraperStatusService.cs        # Update tracking
   │   ├── ScraperLogService.cs           # Real-time logging
   │   │
   │   ├── Parsers/                       # Site-specific parsers
   │   │   ├── Interfaces/
   │   │   │   └── IPriceParser.cs        # Parser contract
   │   │   └── AmazonPriceParser.cs       # Amazon implementation
   │   │
   │   └── Interfaces/                    # Service contracts
   │       ├── IPriceScraperService.cs
   │       └── IScraperStatusService.cs
   │
   ├── HttpClients/                       # HTTP handling
   │   ├── ScraperHttpClient.cs           # Request management
   │   └── Interfaces/
   │       └── IScraperHttpClient.cs
   │
   └── Utilities/                         # Support components
      ├── RobotsTxtChecker.cs             # Compliance validation
      ├── RetryHandler.cs                 # Failure recovery
      ├── RateLimiter.cs                  # Request throttling
      ├── ContentCompressor.cs            # GZIP compression handling
      └── Interfaces/
         ├── IRobotsTxtChecker.cs
         ├── IScraperRateLimiter.cs
         └── IContentCompressor.cs
   ```

   #### API Integration

   The scraping system exposes secure RESTful endpoints:

   ```http
   PATCH  api/ScraperApi/update-all-listings   # Trigger manual price update
   ```

   Security Features:
   - Requires admin authentication or internal system authorization
   - Rate limited to prevent abuse
   - Supports both synchronous and asynchronous operations

   #### Background Processing

   The system uses a dedicated background service (`PriceScraperBackgroundService`) that:
   - Manages automatic update schedules
   - Handles long-running operations
   - Provides fault isolation
   - Ensures consistent update intervals

   ### Database Relationships

   Here's an overview of the key relationships in the database:

   1. **Category and Product**:
      - **One-to-Many**: A **Category** can have multiple **Products**, but each **Product** belongs to exactly one **Category**.

   2. **Product and PriceListing**:
      - **One-to-Many**: A **Product** can have many **PriceListings**, but each **PriceListing** is linked to exactly one **Product**.

   3. **Vendor and PriceListing**:
      - **One-to-Many**: A **Vendor** can have many **PriceListings**, but each **PriceListing** is associated with exactly one **Vendor**.

   4. **ApplicationUser and Product (Wishlist)**:
      - **Many-to-Many**: A **User** can have many **Products** in their **Wishlist**, and a **Product** can appear in many **Users' Wishlists**.

   5. **ApplicationUser and Product (ViewingHistory)**:
      - **Many-to-Many**: A **User** can view many **Products**, and a **Product** can be viewed by many **Users**.

   6. **ApplicationUser and Notification**:
      - **Many-to-Many**: A **User** can have many **Notifications**, and a **Notification** can be sent to many **Users** through the **UserNotification** join table.
      - The **UserNotification** table tracks read status and allows for individual notification management.

   7. **ApplicationUser and LoginActivity**:
      - **One-to-Many**: A **User** can have many **LoginActivities**, but each **LoginActivity** is associated with exactly one **User**.  

   ### Testing

   The project uses **xUnit** for automated testing, covering two types:  

   - **Functional Tests** → Verify actual database behavior using an **in-memory database**.  
   - **Unit Tests** → Isolate and test individual service logic using **mocks** with **Moq**.  


   ####  Functional Tests  
   Located in `RepositoryTests.cs`, functional tests validate:  
   - **Database operations** → Adding, updating, and deleting records.  
   - **Query options** → Sorting, filtering.  

   These tests ensure that repository methods behave as expected with real database interactions.

   Much of the earlier tests were done while i was for the first time learning how to test for this application. So many of the older service, controller and repository tests may not be as good of some of the more recent ones i.e. product controller (namely the latter functions) are better written than the tests for some of the earlier ones i.e. price listing controller.
   
   #### Unit Tests  
   Unit tests target the **service layer** and **controller layer**, using **mock repositories and services** to isolate logic.

   **Service Layer**
   - **Mocks** simulate repository behavior without requiring a database.  
   - This ensures services are tested independently of the data layer.  

   Some services (**Category**, **Admin** etc.. ) do not have unit tests.  
   - These services contain no business logic beyond repository calls.  
   - Since repository operations are covered by **functional tests**, additional unit tests would be redundant.  

   **Controller Layer**
   - **Controller tests** isolate and verify the logic within the controllers.  
   - **Mocks** are used for all dependencies (e.g., services, logger).  
   - These tests verify:  
      - Correct **return types** (e.g., `ViewResult`, `BadRequestResult`, `NotFoundResult`).  
      - Proper handling of **exceptions** and error statuses.  
      - Interaction with services (e.g., verifying correct calls and times). 
   
   For now only a few controllers have unit tests due the massive amount of code requrired to be written and the associated time investment ~600 lines of code for pricelisting one of the smaller controllers. (This will slowly be done)

   ## Technologies Used
   - **Backend**: ASP.NET Core MVC 7.0
   - **Database**: Entity Framework Core with SQL Server
   - **Frontend**: 
     - Bootstrap 5
     - jQuery
     - SignalR for real-time updates
   - **Testing**: 
     - xUnit
     - Moq
     - Entity Framework In-Memory Provider
   - **Other Tools**:
     - HtmlAgilityPack for web scraping
     - Serilog for logging

   ## TODO

   While the core functionality is complete, potential future improvements could include:

   - Add additional parsers for the webscraping side of thing to support more vendors having automatic updates
   - Finishing the testing of all current existing functions (did not start testing to much later in development) (could consider having tests for each function in own class file using inheritence for better readability)
   - Expanding the notification system to include email alerts
   - Adding user preferences for notification settings
   - Adding social features like product reviews and ratings

   ## Project Structure

   ```
   PriceComparisonWebsite/
   ├── Controllers/                    # MVC Controllers
   │   ├── Api/                        # API Controllers
   │   │   ├── NotificationApiController.cs
   │   │   └── ScraperApiController.cs
   │   ├── AdminController.cs
   │   ├── HomeController.cs
   │   ├── PriceListingController.cs
   │   └── ProductController.cs
   │
   ├── Data/                          # Database Context and Migrations
   │   ├── Migrations/
   │   └── ApplicationDbContext.cs
   │
   ├── Models/                        # Domain Models
   │   ├── Category.cs
   │   ├── LoginActivity.cs
   │   ├── Notification.cs
   │   ├── PriceListing.cs
   │   ├── Product.cs
   │   ├── UserNotification.cs
   │   ├── UserViewingHistory.cs
   │   ├── Vendor.cs
   │   └── Wishlist.cs
   │
   ├── Services/                      # Business Logic Layer
   │   ├── Core/                      # Core Business Services
   │   │   ├── CategoryService.cs
   │   │   ├── NotificationService.cs
   │   │   ├── PriceListingService.cs
   │   │   ├── ProductService.cs
   │   │   ├── UserService.cs
   │   │   └── VendorService.cs
   │   │
   │   ├── HttpClients/              # HTTP Client Services
   │   │   └── ScraperHttpClient.cs
   │   │
   │   ├── WebScraping/             # Web Scraping Components
   │   │   ├── Parsers/
   │   │   ├── PriceScraperService.cs
   │   │   ├── PriceScraperBackgroundService.cs
   │   │   └── PriceParserFactory.cs
   │   │
   │   └── Utilities/               # Helper Services
   │       ├── FileSystemWrapper.cs
   │       ├── RateLimiter.cs
   │       ├── RetryHandler.cs
   │       ├── ContentCompressor.cs
   │       └── RobotsTxtChecker.cs
   │
   ├── Views/                       # Razor Views
   │   ├── Admin/
   │   ├── Home/
   │   ├── PriceListing/
   │   ├── Product/
   │   └── Shared/
   │
   ├── wwwroot/                    # Static Files
   │   ├── css/
   │   ├── js/
   │   └── lib/
   │
   │
   └── Program.cs                  # Application Entry Point
   ```

   ### Key Directories

   - **Controllers/**: Contains MVC and API controllers that handle user requests
   - **Data/**: Database context and migrations for Entity Framework Core
   - **Models/**: Domain entities that represent the database structure
   - **Services/**: Business logic implementation divided into core services, HTTP clients, web scraping, and utilities
   - **ViewModels/**: Data transfer objects specifically designed for view requirements
   - **Views/**: Razor views organized by controller
   - **wwwroot/**: Static files including CSS, JavaScript, and third-party libraries
