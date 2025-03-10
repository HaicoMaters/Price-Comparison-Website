# Price Comparison Website

This project was built to help me learn **ASP.NET MVC** and enhance my **C# programming** skills. It is a simple platform where users can compare prices of a particular item across multiple vendors/websites. The platform includes several key features to facilitate the price comparison process and improve user experience.

## Core Features

- **Product Listings**: Users can view products, vendors, and price listings.
- **Search Functionality**: Users can search for products by keywords in their description or title, and filter by categories. Pagination has been added to allow for better navigation of large product listings.
- **User Accounts and Roles**:
  - **Admin**: Admin users can add, edit, and delete vendors, products, and listings.
  - **Basic User**: Regular users can create wishlists and view their browsing history. Wishlisted items are visually distinguished with a yellow shade, a gold star, and a tooltip indicating that the item is on the wishlist.

## Key Features

- **Wishlist & Viewing History**: 
  - Users can add items to their wishlist and track their browsing history. Items that are on the wishlist are visually highlighted with changes and tooltips.
  
- **Admin Capabilities**: 
  - Admin users have full control over product data, such as adding, editing, and deleting product listings. Each product listing also includes a "last updated" timestamp for transparency.
  
- **Manual Updates**: 
  - Currently, all updates to products, vendors, and listings must be performed manually by an admin. Ideally, an API integration would be introduced to automate these updates.

## Admin Login

For testing purposes, an admin account has been created. You can log in using the following credentials:

- **Email**: `admin@admin.com`
- **Password**: `Test1234`

These credentials are also available in the `Program.cs` file for convenience.

## TODO

While there are areas for potential improvement (outlined in the "TODO" section on the homepage of the website), I have largely ceased work on this project. The features mentioned in the TODO section are mostly repetitive or outside the scope of my current learning focus. For example, enhancing the visual appeal of the website is not my priority, as my primary goal was to improve my back-end development skills, specifically with ASP.NET and C#.

## Test Data

Please note that much of the test data on the website is fictional. If you wish to experience the full functionality (e.g., viewing actual product listings or interacting with real websites), you can create your own data. This will give you access to the complete set of features.

## Database Relationships

Here’s an overview of the key relationships in the database:

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

