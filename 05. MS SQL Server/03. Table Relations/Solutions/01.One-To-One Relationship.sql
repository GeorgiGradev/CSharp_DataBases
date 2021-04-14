CREATE DATABASE OneToOneRelationship
USE OneToOneRelationship

CREATE TABLE Passports
(
 PassportID INT PRIMARY KEY,
 PassportNumber CHAR(8) NOT NULL
)
CREATE TABLE Persons
(
 PersonID INT PRIMARY KEY, 
 FirstName NVARCHAR(50) NOT NULL,
 Salary DECIMAL(7,2) NOT NULL,
 PassportID INT FOREIGN KEY
		REFERENCES Passports(PassportID) UNIQUE NOT NULL
)

INSERT INTO Passports 
	VALUES
		(101, 'N34FG21B'),
		(102, 'K65LO4R7'),
		(103, 'ZE657QP2')
	
INSERT INTO Persons (PersonID, FirstName, Salary, PassportID)
	VALUES
		(1,'Roberto', 43300, 102),
		(2, 'Tom', 56100, 103),
		(3,'Yana', 60200, 101)
