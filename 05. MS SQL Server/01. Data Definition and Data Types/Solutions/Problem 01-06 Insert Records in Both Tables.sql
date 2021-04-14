--  Problem 01.Create Database --

CREATE DATABASE Minions -- създаваме база данни за задачата
USE Minions -- да използва базата данни Minions

--  Problem 02.Create Tables --

CREATE TABLE Minions( -- създаваме таблица в базата данни
	Id INT PRIMARY KEY, -- задаваме на Id, че е int и че е ключът на таблицата, по който ще се номерират данните. По този ключ ще може да се търси в таблицата, и по който ще може да се свързва (join) с други таблици. Винаги по default е NOT NULL.
	[Name] NVARCHAR(50) NOT NULL,
	Age TINYINT
)

CREATE TABLE Towns( -- създаваме таблица в базата данни
	Id INT PRIMARY KEY NOT NULL,
	[Name] NVARCHAR(50) NOT NULL 
)

--  Problem 03.Alter Minions Table --

ALTER TABLE Minions -- променяме дизайна на таблицата
ADD TownId INT FOREIGN KEY REFERENCES Towns(Id) -- като добавяме нова колона TownId, която е свързана с колоната Id на таблицата Towns.

--  Problem 04.Insert Records into Both Tables --

INSERT INTO Towns -- вкарваме данни в таблицата
	VALUES
		(1, 'Sofia'),
		(2, 'Plovdiv'),
		(3, 'Varna')

SELECT Id, [Name] FROM Towns -- Избираме да видим цялата информация за тази таблица
-- Select * FROM Towns (друг вариант)

INSERT INTO Minions -- вкарваме данни в таблицата
	VALUES
		(1, 'Kevin', 22, 1),
		(2, 'Bob', 15, 3),
		(3, 'Steward', NULL, 2)

SELECT * FROM Minions -- Избираме да видим цялата информация за тази таблица

--  Problem 05.Truncate Table Minions --

TRUNCATE TABLE minions -- Изтриване на данните в таблицата
	
--  Problem 06.Drop All Tables --

DROP TABLE Minions -- Изтриване на таблицата
DROP TABLE Towns -- Изтриване на таблицата