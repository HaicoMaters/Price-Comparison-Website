   # PriceComparisonWebsite

   This project was built to help me learn **ASP.NET MVC** and enhance my **C# programming** skills. It is a simple platform where users can compare prices of a particular item across multiple vendors/websites. The platform includes several key features to facilitate the price comparison process and improve user experience.

   Much of the codebase varies in quality due to my learning process resulting in the earlier code to be significantly worse than the later code. However, much of the earlier code has been rewritten as i learned more about it. For example; early on almost all the buisness logic was in the controllers, whilst as i learnt more about ASP.NET, much of this code was moved to the services and often rewritten to be better.

   ## Core Features

   - **Product Listings**: Users can view products, vendors, and price listings.
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
      - Price history tracking
      - Admin Panel for sending global notifications and viewing website statistics and monitoring site login activity/attempts
   
   - **Manual Updates**: 
      - Currently, all updates to products, vendors, and listings must be performed manually by an admin
      - Future plans may include some sort of webscraping or api use to update listings automatically

   ## Admin Login

   For testing purposes, an admin account has been created. You can log in using the following credentials:

   - **Email**: `admin@admin.com`
   - **Password**: `Test1234`

   These credentials are also available in the `Program.cs` file for convenience.

   ## Test Data

   The website includes some real product data with actual prices from various vendors to demonstrate the functionality. While this data is sufficient for basic testing, you're welcome to add your own data to:
   - Test pagination with larger datasets
   - Create more complex price comparisons
   - Experiment with different product categories
   - Test the notification system with custom price changes

   ## Testing

   The project uses **xUnit** for automated testing, covering two types:  

   - **Functional Tests** → Verify actual database behavior using an **in-memory database**.  
   - **Unit Tests** → Isolate and test individual service logic using **mocks** with **Moq**.  


   ###  Functional Tests  
   Located in `RepositoryTests.cs`, functional tests validate:  
   - **Database operations** → Adding, updating, and deleting records.  
   - **Query options** → Sorting, filtering.  

   These tests ensure that repository methods behave as expected with real database interactions.

   Much of the earlier tests were done while i was for the first time learning how to test for this application. So many of the older service, controller and repository tests may not be as good of some of the more recent ones, i.e. product controller (namely the latter functions) are better written than the tests for some of the earlier ones i.e. price listing controller.
   
   ### Unit Tests  
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

   ## Database Relationships

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
x
   7. **ApplicationUser and LoginActivity**:
      - **One-to-Many**: A **User** can have many **LoginActivities**, but each **LoginActivity** is associated with exactly one **User**.  


   ## TODO

   While the core functionality is complete, potential future improvements could include:

   - Finishing the testing of all current existing functions (did not start testing to much later in development) (could consider having tests for each function in own class file using inheritence for better readability)
   - Adding price history charts and analytics
   - Automatic updating of pricelistings - using webscraping or api (ethical considerations exist, robots.txt etc. will have to do some research and only include vendors that will allow for it)
   - Expanding the notification system to include email alerts
   - Adding user preferences for notification settings
   - Adding social features like product reviews and ratings

   Web Scraping:

Web scraping updates every 8 hours or manually from the admin panel. Making sure to follow robots.txt. 2s delay for each request, 3 retries

Vendors have a flag for supports web scraping/automatic updates, ones that don't may be due to implementation for them not being complete/implemented or being against the vendor site's terms of service.

For each price listing with a valid url and the vendor support flag.

Current plan for project structure as follows:

/Services
 ├── Core
 │      ├── Interfaces                  
 │      │       ├── IProductService.cs  
 │      │       └── UserService.cs   etc...
 │      │  
 │      ├── ProductService.cs  
 │      └── UserService.cs  etc...
 │    
 │
 ├── WebScraping  
 │      ├── PriceScraperService.cs  
 │      ├── PriceParserFactory.cs  
 │      └── Parsers/  
 │              ├── AmazonPriceParser.cs  
 │              ├── EbayPriceParser.cs  
 │            	└── etc....
 │  
 ├── HttpClients  
 │      └── ScraperHttpClient.cs  
 │
 ├── Utilities  
 │      ├── RobotsTxtChecker.cs  
 │      ├── RetryHandler.cs  
 │      └── RateLimiter.cs  

Main service for price scraping, with unique parsers for logic of how to parse the price from each indivdual website, with a priceparserfactory to handle picking between each parser. Adding new supported websites for each parser written.

ScraperHttpClient for handling http requests.

RobotsTxtChecker for ensuring compliance with robots.txt. Each robots.txt file should be cached after first retrieval. Maybe update every month or so (but this probably does not need to be changed).

Retry Handler, for retrying upon faillure (current plan 3 retries).

Rate Limtier for ratelimiting how fast requests are made (current plan 2s delay).

In controller move notification logic to api and also, create ScrapingControllerApi with functions for mannually scraping for use in admin panel.