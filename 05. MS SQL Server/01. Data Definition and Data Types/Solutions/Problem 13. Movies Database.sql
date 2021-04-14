--  Problem 13.Movies Database --
CREATE DATABASE Movies
USE Movies

CREATE TABLE Directors 
(
	 Id INT PRIMARY KEY IDENTITY NOT NULL,
	 DirectorName NVARCHAR(100) NOT NULL,
	 Notes VARCHAR(MAX)
)
INSERT INTO Directors (DirectorName, Notes)
	VALUES
		('Name1', 'List of notes'),
		('Name2', 'List of notes'),
		('Name3', 'List of notes'),
		('Name4', 'List of notes'),
		('Name5', 'List of notes')


CREATE TABLE Genres 
(
	 Id INT PRIMARY KEY IDENTITY NOT NULL,
	 GenreName NVARCHAR(100) NOT NULL,
	 Notes VARCHAR(MAX)
)
INSERT INTO Genres (GenreName, Notes)
	VALUES
		('Genre1', 'List of notes'),
		('Genre2', 'List of notes'),
		('Genre3', 'List of notes'),
		('Genre4', 'List of notes'),
		('Genre5', 'List of notes')


CREATE TABLE Categories 
(
	 Id INT PRIMARY KEY IDENTITY NOT NULL,
	 CategoryName NVARCHAR(100) NOT NULL,
	 Notes VARCHAR(MAX)
)
INSERT INTO Categories (CategoryName, Notes)
	VALUES
		('Category1', 'List of notes'),
		('Category2', 'List of notes'),
		('Category3', 'List of notes'),
		('Category4', 'List of notes'),
		('Category5', 'List of notes')


CREATE TABLE Movies
(
	 Id INT PRIMARY KEY IDENTITY NOT NULL,
	 Title NVARCHAR(100) NOT NULL,
	 DirectorId INT FOREIGN KEY REFERENCES Directors(Id) NOT NULL,
	 CopyrightYear INT, 
	 Lenght INT, 
	 GenreId INT FOREIGN KEY REFERENCES Genres(Id) NOT NULL,
	 CategoryId INT FOREIGN KEY REFERENCES Categories(Id) NOT NULL,
	 Rating DECIMAL, 
	 Notes NVARCHAR(MAX)
)
INSERT INTO Movies (Title, DirectorId, CopyrightYear, Lenght, GenreId, CategoryId, Rating, Notes)
	VALUES
   		('SomeMovie1',1,2000,120,1,1,10,NULL),
		('SomeMovie2',2,2000,120,2,2,10,NULL),
		('SomeMovie3',3,2000,120,3,3,10,NULL),
		('SomeMovie4',4,2000,120,4,4,10,NULL),
		('SomeMovie5',5,2000,120,5,5,10,'This is note')

SELECT * FROM Movies