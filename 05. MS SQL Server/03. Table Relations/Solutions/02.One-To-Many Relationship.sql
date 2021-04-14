CREATE DATABASE OneToManyRelationship
USE OneToManyRelationship

CREATE TABLE Manufacturers
(
 ManufacturerID INT PRIMARY KEY,
 [Name] NVARCHAR(50) NOT NULL,
 EstablishedOn DATE NOT NULL
)

CREATE TABLE Models
(
 ModelID INT PRIMARY KEY, 
 [Name] NVARCHAR(50) NOT NULL,
 ManufacturerID INT NOT NULL FOREIGN KEY
	REFERENCES Manufacturers(ManufacturerID)
)

INSERT INTO Manufacturers
	VALUES
	(1, 'BMW', '03/07/1916'),
	(2,	'Tesla', '01/01/2003'),
	(3,	'Lada',	'05/01/1966')

INSERT INTO Models
	VALUES
		(101, 'X1', 1),
		(102, 'i6',	1),
		(103, 'ModelS', 2),
		(104, 'ModelX', 2),
		(105, 'Model3', 2),
		(106, 'Nova', 3)

--SELECT * FROM Manufacturers
--SELECT * FROM Models
