USE School

---02.Insert---
INSERT INTO Teachers
	VALUES
	('Ruthanne','Bamb','84948 Mesta Junction',3105500146,6),
	('Gerrard',	'Lowin','370 Talisman Plaza',3324874824,2),
	('Merrile',	'Lambdin',	'81 Dahle Plaza',4373065154,5),
	('Bert','Ivie','2 Gateway Circle',4409584510,4)

INSERT INTO Subjects
	VALUES
	('Geometry',12),
	('Health',10),
	('Drama',7),
	('Sports',9)


---03.Update---
UPDATE
	StudentsSubjects
	SET Grade = 6
	WHERE SubjectId IN (1,2) AND Grade >= 5.50


---04.Delete---
DELETE
	FROM StudentsTeachers
	WHERE TeacherID IN (SELECT
						Id 
						FROM Teachers 
						WHERE Phone LIKE '%72%')
DELETE
	FROM Teachers
	WHERE Phone LIKE '%72%'


---05.Teen Students---
SELECT 
	FirstName,
	LastName,
	Age
	FROM Students
	WHERE Age >= 12
	ORDER BY FirstName, LastName


---06.Cool Addresses---
SELECT 
	CONCAT(FirstName, ' ', IIF(MiddleName IS NULL, '', MiddleName + ' '), LastName),
	Address
	FROM Students
	WHERE Address LIKE '%road%'
	ORDER BY FirstName, LastName, Address


---07.42 Phones---
SELECT
	FirstName, 
	Address,
	Phone
	FROM Students
	WHERE MiddleName IS NOT NULL AND Phone LIKE '42%'
	ORDER BY FirstName



---08.Students Teachers---
SELECT 
	s.FirstName,
	s.LastName,
	COUNT(st.TeacherId)
	FROM Students as s
	JOIN StudentsTeachers as st ON s.Id = st.StudentId
	GROUP BY s.FirstName, s.LastName


---09.Subjects with Students---
SELECT
	CONCAT(t.FirstName, ' ', t.LastName),
	CONCAT(s.Name, '-', CAST(s.Lessons AS VARCHAR)),
	COUNT(st.StudentId)
	FROM Teachers as t
	JOIN StudentsTeachers as st ON t.Id = st.TeacherId
	JOIN Subjects as s ON t.SubjectId = s.Id
	GROUP BY CONCAT(t.FirstName, ' ', t.LastName),CONCAT(s.Name, '-', CAST(s.Lessons AS VARCHAR))
	ORDER BY COUNT(st.StudentId) DESC, CONCAT(t.FirstName, ' ', t.LastName), CONCAT(s.Name, '-', CAST(s.Lessons AS VARCHAR))


---10.Students to Go---
SELECT 
	CONCAT(FirstName, ' ', LastName)
	FROM Students AS s
	LEFT JOIN StudentsExams AS se ON s.Id = se.StudentId
	WHERE se.ExamId IS NULL
	ORDER BY CONCAT(FirstName, ' ', LastName)


---11.Busiest Teachers---
SELECT TOP(10)
	t.FirstName,
	t.LastName,
	COUNT(*)
	FROM Teachers AS t
	JOIN StudentsTeachers AS st ON t.Id = st.TeacherId
	GROUP BY t.FirstName, t.LastName
	ORDER BY t.FirstName, t.LastName


---12.Top Students---
SELECT TOP(10)
	FirstName,
	LastName,
	FORMAT(AverageGrade, 'N2') 
	FROM (SELECT
				s.FirstName AS FirstName,
				s.LastName AS LastName,
				AVG(se.Grade) AS AverageGrade
				FROM Students AS s
				JOIN StudentsExams AS se ON s.Id = se.StudentId
				GROUP BY s.FirstName, s.LastName) AS temp
	ORDER BY CAST(AverageGrade AS DECIMAL(3,2)) DESC, FirstName, LastName


--- 13.Second Highest Grade---
SELECT
	FirstName,
	LastName,
	Grade
	FROM (SELECT
				s.FirstName AS FirstName,
				s.LastName AS LastName,
				ss.Grade AS Grade,
				ROW_NUMBER () OVER (PARTITION BY s.FirstName,s.LastName ORDER BY ss.Grade DESC) AS [Rank]
				FROM Students AS s
				JOIN StudentsSubjects AS ss ON s.Id = ss.StudentId) as temp
	WHERE Rank = 2
	ORDER BY FirstName, LastName


---14.Not So In The Studying---
SELECT 
	CONCAT(s.FirstName, ' ', IIF(s.MiddleName IS NULL, '', s.MiddleName + ' '), LastName)
	FROM Students AS s 
	LEFT JOIN StudentsSubjects AS ss ON s.Id = ss.StudentId
	WHERE ss.Id IS NULL
	ORDER BY CONCAT(s.FirstName, ' ', IIF(s.MiddleName IS NULL, '', s.MiddleName + ' '), LastName)


---15.Top Student per Teacher---
SELECT
	[Teacher Full Name],
	[Subject Name],
	[Student Full Name],
	FORMAT(AverageGrade, 'N2')
	FROM (SELECT 
		[Teacher Full Name],
		[Subject Name],
		[Student Full Name],
		AverageGrade,
		ROW_NUMBER() OVER (PARTITION BY [Teacher Full Name] ORDER BY AverageGrade DESC) AS [Rank]
		FROM (SELECT
			CONCAT(t.FirstName,  ' ', t.LastName) AS [Teacher Full Name],
			teaSub.Name AS [Subject Name],
			CONCAT(stu.FirstName, ' ', stu.LastName) AS [Student Full Name],
			AVG(ss.Grade) AS AverageGrade
			FROM Teachers AS t
			JOIN StudentsTeachers AS st ON t.Id = st.TeacherId
			JOIN Students AS stu ON st.StudentId = stu.Id
			JOIN StudentsSubjects AS ss ON stu.Id = ss.StudentId
			JOIN Subjects as stuSub ON ss.SubjectId = stuSub.Id
			JOIN Subjects as teaSub ON t.SubjectId = teaSub.Id
			WHERE teaSub.Name = stuSub.Name
			GROUP BY CONCAT(t.FirstName,  ' ', t.LastName),
			teaSub.Name,
			CONCAT(stu.FirstName, ' ', stu.LastName)) AS temp) AS temp2
	WHERE Rank = 1
	ORDER BY [Subject Name], [Teacher Full Name], AverageGrade DESC

				
---16.Average Grade per Subject---
SELECT 
	s.Name,
	AVG(ss.Grade)
	FROM StudentsSubjects AS ss
	JOIN Subjects AS s ON ss.SubjectId = s.Id
	GROUP BY s.Name, s.Id
	ORDER BY s.Id


---17.Exams Information---
SELECT
	[Quarter],
	SubjectName,
	COUNT(StudentId)
	FROM (SELECT 
			CASE
		   					WHEN MONTH([Date]) >= 1 AND MONTH([Date]) <= 3 THEN 'Q1'
		   					WHEN MONTH([Date]) >= 4 AND MONTH([Date]) <= 6 THEN 'Q2'
		   					WHEN MONTH([Date]) >= 7 AND MONTH([Date]) <= 9 THEN 'Q3'
		   					WHEN MONTH([Date]) >= 10 AND MONTH([Date]) <= 12 THEN 'Q4'
		   					ELSE 'TBA'
							END AS [Quarter],
			sub.Name AS SubjectName,
			se.StudentId AS StudentId
			FROM Subjects AS sub
			JOIN Exams AS e ON sub.Id = e.SubjectId
			JOIN StudentsExams AS se ON e.Id = se.ExamId
			WHERE se.Grade >= 4) as temp
	GROUP BY Quarter, SubjectName
	ORDER BY Quarter


---18. Exam Grade---
GO
CREATE OR ALTER FUNCTION udf_ExamGradesToUpdate(@studentId INT, @grade DECIMAL(3,2))
RETURNS NVARCHAR(MAX)
AS
BEGIN	
	DECLARE @ReturnMessage NVARCHAR(MAX)
	DECLARE @CurrentStudentId INT = ISNULL((SELECT TOP(1)
										s.Id 
										FROM Students AS s
										JOIN StudentsExams AS ss ON s.Id = ss.StudentId
										WHERE s.Id = @studentId), 0)

	IF (@CurrentStudentId = 0)
		SET @ReturnMessage = 'The student with provided id does not exist in the school!';
	ELSE IF (@grade > 6)
		SET @ReturnMessage = 'Grade cannot be above 6.00!';
	ELSE
		BEGIN
			DECLARE @GradesToUpdate INT = (SELECT
										COUNT(*) 
										FROM StudentsExams
										WHERE StudentId = @studentId AND (Grade BETWEEN 5.50 AND @grade + 3.50))
			DECLARE @CurrentStudentName NVARCHAR(30) = (SELECT
									FirstName
									FROM Students
									WHERE Id = @studentId)
			SET @ReturnMessage = CONCAT('You have to update ', @GradesToUpdate,  ' grades for the student ',@CurrentStudentName)
		END

RETURN @ReturnMessage
END
GO


SELECT dbo.udf_ExamGradesToUpdate(12, 6.20)
SELECT dbo.udf_ExamGradesToUpdate(12, 5.50)
SELECT dbo.udf_ExamGradesToUpdate(121, 5.50)


---19.Exclude from school---
GO
CREATE PROCEDURE usp_ExcludeFromSchool(@StudentId INT)
AS
	DECLARE @CurrentStudentId INT = ISNULL((SELECT
									Id
									FROM Students 
									WHERE Id = @StudentId), 0)
	IF (@CurrentStudentId = 0)
		THROW 50001, 'This school has no student with the provided id!', 1
	ELSE
		DELETE
			FROM StudentsExams
			WHERE StudentId = @CurrentStudentId

		DELETE 
			FROM StudentsSubjects 
			WHERE StudentId = @CurrentStudentId

		DELETE 
			FROM StudentsTeachers
			WHERE StudentId = @CurrentStudentId

		DELETE 
			FROM Students
			WHERE Id = @CurrentStudentId

---20. Deleted Students---
CREATE TABLE ExcludedStudents
(
StudentId INT, 
StudentName VARCHAR(30)
)

GO
CREATE TRIGGER tr_StudentsDelete ON Students
INSTEAD OF DELETE
AS
INSERT INTO ExcludedStudents(StudentId, StudentName)
		SELECT Id, FirstName + ' ' + LastName FROM deleted