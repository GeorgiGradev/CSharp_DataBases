-- Problem 14.Car Rental Database --
CREATE DATABASE CarRental
USE CarRental
CREATE TABLE Categories
(
  Id INT PRIMARY KEY IDENTITY NOT NULL,
  CategoryName NVARCHAR(100) NOT NULL,
  DailyRate DECIMAL(4,2) NOT NULL,
  WeeklyRate DECIMAL(5,2) NOT NULL,
  MonthlyRate DECIMAL(6,2) NOT NULL,
  WeekendRate DECIMAL (4,2) NOT NULL
)
INSERT INTO Categories(CategoryName, DailyRate, WeeklyRate, MonthlyRate, WeekendRate)
	VALUES
		('catName1', 20.20, 15.15, 10.10, 5.5),
		('catName2', 20.20, 15.15, 10.10, 5.5),
		('catName3', 20.20, 15.15, 10.10, 5.5)


CREATE TABLE Cars
(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	PlateNumber VARCHAR(10) UNIQUE NOT NULL,
	Manufacturer NVARCHAR(50) NOT NULL,
	Model VARCHAR(10) NOT NULL,
	CarYear INT NOT NULL,
	CategoryId INT FOREIGN KEY REFERENCES Categories(Id) NOT NULL,
	Doors INT,
	Picture VARBINARY,
	Condition CHAR(10),
	Available BIT NOT NULL
)
INSERT INTO Cars(PlateNumber, Manufacturer, Model, CarYear, CategoryId, Available)
	VALUES
		('X1212K', 'Manu', 'Model', '2015', 1, 1),
		('X1213K', 'Manu', 'Model', '2015', 2, 1),
		('X1214K', 'Manu', 'Model', '2015', 2, 1)


CREATE TABLE Employees
(
	Id INT PRIMARY KEY IDENTITY NOT NULL, 
	FirstName NVARCHAR(100) NOT NULL,
	LastName NVARCHAR(100) NOT NULL,
	Title NVARCHAR(30),
	Notes NVARCHAR(MAX)
)
INSERT INTO Employees(FirstName, LastName)
	VALUES
		('FirstName1', 'LastName1'),
		('FirstName2', 'LastName2'),
		('FirstName3', 'LastName3')


CREATE TABLE Customers
(
	Id INT PRIMARY KEY IDENTITY NOT NULL, 
	DriverLicenseNumber VARCHAR(20) NOT NULL,
	FullName NVARCHAR(100) NOT NULL, 
	[Address] NVARCHAR (100),
	City NVARCHAR (20),
	ZIPCode CHAR(5),
	Notes NVARCHAR(MAX)
)
INSERT INTO Customers (DriverLicenseNumber, FullName)
	VALUES
		('123456', 'FullName1'),
		('123456', 'FullName2'),
		('123456', 'FullName3')


CREATE TABLE RentalOrders
(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	EmployeeId INT FOREIGN KEY REFERENCES Employees(Id) NOT NULL, 
	CustomerId INT FOREIGN KEY REFERENCES Customers(Id) NOT NULL,
	CarId INT FOREIGN KEY REFERENCES Cars(Id) NOT NULL,
	TankLevel DECIMAL NOT NULL,
	KilometrageStart DECIMAL NOT NULL,
	KilometrageEnd DECIMAL NOT NULL,
	TotalKilometrage DECIMAL NOT NULL,
	StartDate DATE NOT NULL,
	EndDate DATE NOT NULL,
	TotalDays INT,
	RateApplied DECIMAL, 
	TaxRate DECIMAL,
	OrderStatus BIT NOT NULL,
	Notes NVARCHAR(MAX)
)
INSERT INTO RentalOrders (EmployeeId, CustomerId, CarId, TankLevel, KilometrageStart, KilometrageEnd, TotalKilometrage, StartDate, EndDate, OrderStatus)
	VALUES
		(1, 1, 1, 20.20, 234.45, 345.43, 200.00, '04.05.2020', '06.05.2020', 0),
		(2, 2, 2, 20.20, 234.45, 345.43, 200.00, '04.05.2020', '06.05.2020', 1),
		(3, 3, 3, 20.20, 234.45, 345.43, 200.00, '04.05.2020', '06.05.2020', 1)

SELECT * FROM Categories
SELECT * FROM Cars 
SELECT * FROM Employees 
SELECT * FROM Customers  
SELECT * FROM RentalOrders
