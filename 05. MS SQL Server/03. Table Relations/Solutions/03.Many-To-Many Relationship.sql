CREATE DATABASE ManyToManyRelationship
USE ManyToManyRelationship 

CREATE TABLE Students
(
	StudentID INT PRIMARY KEY,
	[Name] NVARCHAR(50) NOT NULL
)

CREATE TABLE Exams
(
	ExamID INT PRIMARY KEY,
	[Name] NVARCHAR(50) NOT NULL
)

CREATE TABLE StudentsExams
(
	StudentID INT NOT NULL FOREIGN KEY 
		REFERENCES Students(StudentID), 
	ExamID INT NOT NULL FOREIGN KEY 
		REFERENCES Exams(ExamID), 
	PRIMARY KEY(StudentID, ExamID)		
)
DROP TABLE StudentsExams
INSERT INTO Students
	VALUES
		(1, 'Mila'),
		(2, 'Toni'),
		(3, 'Ron')

INSERT INTO Exams
	VALUES
		(101, 'SpringMVC'),
		(102, 'Neo4j'),
		(103, 'Oracle 11g')

INSERT INTO StudentsExams
	VALUES
	(1, 101),
	(1, 102),
	(2, 101),
	(3,	103),
	(2,	102),
	(2,	103)

--SELECT * FROM Students
--SELECT * FROM Exams
--SELECT * FROM StudentsExams
