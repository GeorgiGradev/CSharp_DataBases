-- Problem 15.Hotel Database --
CREATE DATABASE Hotel
USE Hotel

CREATE TABLE Employees(
	Id INT PRIMARY KEY IDENTITY,
	FirstName VARCHAR(30) NOT NULL,
	LastName VARCHAR(30) NOT NULL,
	Title VARCHAR(30) NOT NULL,
	Notes TEXT)

INSERT INTO Employees(FirstName,LastName,Title)
	VALUES
		('Gosho', 'Goshev' , 'Shop Seller'),
		('Gosho1', 'Goshev1' , 'Manager'),
		('Gosho2', 'Goshev2' , 'Administrator')

CREATE TABLE Customers(
	AccountNumber INT PRIMARY KEY IDENTITY,
	FirstName VARCHAR(30) NOT NULL,
	LastName VARCHAR(30) NOT NULL,
	PhoneNumber BIGINT NOT NULL)

INSERT INTO Customers(FirstName,LastName,PhoneNumber)
	VALUES
		('Customer1','Customerov1',1111111111),
		('Customer2','Customerov2',2222222222),
		('Customer3','Customerov3',2222222222)

CREATE TABLE RoomStatus(
	RoomStatus VARCHAR(10) PRIMARY KEY,
	Notes TEXT)

INSERT INTO RoomStatus(RoomStatus)
	VALUES
		('Available'),
		('Cleaning'),
		('Reserved')

CREATE TABLE RoomTypes(
	RoomType VARCHAR(10) PRIMARY KEY,
	Notes TEXT)

INSERT INTO RoomTypes(RoomType)
	VALUES
		('Single'),
		('Double'),
		('Family')

CREATE TABLE BedTypes(
	BedType VARCHAR(10) PRIMARY KEY,
	Notes TEXT)

INSERT INTO BedTypes(BedType)
	VALUES
		('Small'),
		('Big'),
		('ExtraBig')

CREATE TABLE Rooms(
	RoomNumber INT PRIMARY KEY Identity,
	RoomType VARCHAR(10) FOREIGN KEY REFERENCES RoomTypes(RoomType) NOT NULL,
	BedType VARCHAR(10) FOREIGN KEY REFERENCES BedTypes(BedType) NOT NULL,
	Rate INT,
	RoomStatus VARCHAR(10) FOREIGN KEY REFERENCES RoomStatus(RoomStatus) NOT NULL,
	Notes TEXT)

INSERT INTO Rooms(RoomType,BedType,RoomStatus)
	VALUES
		('Single','Small','Available'),
		('Double','Big','Cleaning'),
		('Family','ExtraBig','Reserved')

CREATE TABLE Payments(
	Id INT PRIMARY KEY IDENTITY,
	EmployeeId INT FOREIGN KEY REFERENCES Employees(Id) NOT NULL,
	PaymentDate DATE NOT NULL,
	AccountNumber INT FOREIGN KEY REFERENCES Customers(AccountNumber) NOT NULL,
	FirstDateOccupied DATE NOT NULL,
	LastDateOccupied DATE NOT NULL,
	TotalDays INT NOT NULL,
	AmountCharged DECIMAL(7,2) NOT NULL,
	TaxRate DECIMAL(4,2) NOT NULL,
	TaxAmount DECIMAL(7,2) NOT NULL,
	PaymentTotal DECIMAL(7,2) NOT NULL,
	Notes TEXT)

INSERT INTO Payments(EmployeeId,PaymentDate,AccountNumber,FirstDateOccupied,LastDateOccupied,TotalDays,AmountCharged,TaxRate,TaxAmount,PaymentTotal)
	VALUES
		(1,'01.10.2020',1,'01.05.2020','01.10.2020',5,500,10,50,550),
		(2,'01.10.2020',2,'01.05.2020','01.10.2020',5,500,10,50,550),
		(3,'01.10.2020',3,'01.05.2020','01.10.2020',5,500,10,50,550)

CREATE TABLE Occupancies(
	Id INT PRIMARY KEY IDENTITY,
	EmployeeId INT FOREIGN KEY REFERENCES Employees(Id) NOT NULL,
	DateOccupied DATE NOT NULL,
	AccountNumber INT FOREIGN KEY REFERENCES Customers(AccountNumber) NOT NULL,
	RoomNumber INT FOREIGN KEY REFERENCES Rooms(RoomNumber) NOT NULL,
	RateApplied INT,
	PhoneCharge DECIMAL(6,2),
	Notes TEXT)

INSERT INTO Occupancies(EmployeeId,DateOccupied,AccountNumber,RoomNumber)
	VALUES
		(1,'01.01.2020',1,1),
		(2,'01.01.2020',2,2),
		(1,'01.01.2020',3,3)