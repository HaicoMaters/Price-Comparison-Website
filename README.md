   # Price Comparison Website

   This project was built to help me learn **ASP.NET MVC** and enhance my **C# programming** skills. It is a simple platform where users can compare prices of a particular item across multiple vendors/websites. The platform includes several key features to facilitate the price comparison process and improve user experience.

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
   
   ### Unit Tests  
   Unit tests target the **service layer** and **controller layer**, using **mock repositories and services** to isolate logic.  

   **Service Layer**
   - **Mocks** simulate repository behavior without requiring a database.  
   - This ensures services are tested independently of the data layer.  

   Some services (**Category**, **Admin** and **PriceListings**) do not have unit tests.  
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

   7. **ApplicationUser and LoginActivity**:
      - **One-to-Many**: A **User** can have many **LoginActivities**, but each **LoginActivity** is associated with exactly one **User**.  


   ## TODO

   While the core functionality is complete, potential future improvements could include:

   - Adding price history charts and analytics
   - Automatic updating of pricelistings - using webscraping or api (ethical considerations exist, robots.txt etc. will have to do some research and only include vendors that will allow for it)
   - Expanding the notification system to include email alerts
   - Adding user preferences for notification settings
   - Adding social features like product reviews and ratings