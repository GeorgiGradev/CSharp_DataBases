CREATE DATABASE SelfReferencing 
USE SelfReferencing

CREATE TABLE Teachers
(
	TeacherID INT PRIMARY KEY IDENTITY(101, 1),
	[Name] NVARCHAR(50) NOT NULL,
	ManagerID INT FOREIGN KEY
		REFERENCES Teachers(TeacherID)
)
--DROP TABLE Teachers
INSERT INTO Teachers
	VALUES 
		('John', NULL),
		('Maya', 106),
		('Silvia', 106),
		('Ted', 105),
		('Mark', 101),
		('Greta', 101)

SELECT * FROM Teachers