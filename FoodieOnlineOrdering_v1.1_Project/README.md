# Foodie - Online Food Ordering & Management System v1.1

ASP.NET Core 8 MVC + C# + MySQL + HTML + CSS + JavaScript.

## Scope
Version 1.0 follows the Requirements Baseline. Online payment is implemented as CR-001 / Version 1.1 because the baseline explicitly lists the online payment gateway as out of scope for v1.0.

## Main features
- Customer registration/login/logout
- Password rule: minimum 8 characters + letter + number + special character
- Server-side role authorization for Admin functions
- Menu search and category filtering
- Login required before Add to Cart
- Cart quantity update/remove and total calculation
- Checkout and delivery address
- Cash on Delivery
- Online Payment DEMO: validates 16-digit card, expiry and CVV, stores only last four digits and a demo transaction reference
- Customer order history and invoice
- Admin dashboard, food CRUD, user roles, order status management
- Admin CSV export, browser PDF export and print
- Responsive HTML/CSS/JS interface

## Setup
1. Install Visual Studio 2022 with ASP.NET and web development, or VS Code + .NET 8 SDK.
2. Install MySQL Server and MySQL Workbench/XAMPP MySQL.
3. Create/open the project.
4. Edit appsettings.json and set the MySQL connection string.
5. Run database.sql in MySQL if you want manual database creation. The app also calls EnsureCreated on first run.
6. Restore packages and run the project.
7. Admin login: Admin / Admin123@
8. Register a customer account and test the customer flow.

## Important
The online payment is a safe coursework DEMO. It is not a real payment gateway. Do not enter real card information.
