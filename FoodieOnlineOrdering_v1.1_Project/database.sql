CREATE DATABASE IF NOT EXISTS foodie_ordering CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE foodie_ordering;

CREATE TABLE IF NOT EXISTS Users (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Username VARCHAR(50) NOT NULL UNIQUE,
    Email VARCHAR(150) NOT NULL UNIQUE,
    PasswordHash LONGTEXT NOT NULL,
    Role VARCHAR(20) NOT NULL DEFAULT 'Customer',
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS Foods (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(120) NOT NULL,
    Category VARCHAR(60) NOT NULL,
    Description VARCHAR(500) NOT NULL,
    Price DECIMAL(12,2) NOT NULL,
    ImageUrl VARCHAR(500) NOT NULL,
    IsAvailable BOOLEAN NOT NULL DEFAULT TRUE,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX IX_Foods_Category (Category),
    INDEX IX_Foods_Name (Name)
);

CREATE TABLE IF NOT EXISTS Orders (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    DeliveryAddress VARCHAR(200) NOT NULL,
    PaymentMethod VARCHAR(30) NOT NULL,
    Status VARCHAR(30) NOT NULL DEFAULT 'Pending',
    TotalAmount DECIMAL(12,2) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT FK_Orders_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
);

CREATE TABLE IF NOT EXISTS OrderItems (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    OrderId INT NOT NULL,
    FoodId INT NOT NULL,
    FoodName VARCHAR(120) NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(12,2) NOT NULL,
    LineTotal DECIMAL(12,2) NOT NULL,
    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItems_Foods FOREIGN KEY (FoodId) REFERENCES Foods(Id)
);

CREATE TABLE IF NOT EXISTS Payments (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    OrderId INT NOT NULL UNIQUE,
    Method VARCHAR(30) NOT NULL,
    Status VARCHAR(30) NOT NULL,
    TransactionReference VARCHAR(50) NOT NULL UNIQUE,
    Amount DECIMAL(12,2) NOT NULL,
    CardLast4 VARCHAR(4) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT FK_Payments_Orders FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS ContactMessages (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Email VARCHAR(150) NOT NULL,
    Message VARCHAR(1000) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Admin is created automatically by Program.cs using a secure password hash.
-- Username: Admin
-- Password: Admin123@
-- Online Payment in this coursework build is a DEMO payment-record flow.
-- No real card number/CVV is stored and no real payment gateway is connected.
