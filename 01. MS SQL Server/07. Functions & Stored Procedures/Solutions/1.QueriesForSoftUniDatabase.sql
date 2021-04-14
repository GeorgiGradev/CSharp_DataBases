USE SoftUni 

--- 1.Employees with Salary Above 35000 ---
 GO
 CREATE PROCEDURE usp_GetEmployeesSalaryAbove35000 
 AS
	SELECT 
		FirstName AS [First Name],
		LastName AS [Last Name]
		FROM Employees
		WHERE Salary > 35000
--EXEC dbo.usp_GetEmployeesSalaryAbove35000


--- 2.Employees with Salary Above Number ---
GO
CREATE PROCEDURE usp_GetEmployeesSalaryAboveNumber (@MinSalary DECIMAL(18,4))
AS 
	SELECT
		FirstName AS [First Name],
		LastName AS [LastName]
		FROM Employees
		WHERE Salary >= @MinSalary

-- EXEC dbo.usp_GetEmployeesSalaryAboveNumber 48100


--- 3.Town Names Starting With ---
GO
CREATE PROC usp_GetTownsStartingWith (@StartString NVARCHAR(20))
AS
	SELECT 
	[Name]
	FROM Towns
	WHERE [Name] LIKE @StartString + '%'

--EXEC usp_GetTownsStartingWith 'be'


--- 4.Employees from Town ---
GO 
CREATE PROCEDURE usp_GetEmployeesFromTown(@TownName NVARCHAR(MAX))
AS
	SELECT 
		e.FirstName,
		e.LastName
		FROM Employees AS e
		JOIN Addresses AS a ON e.AddressID = a.AddressID
		JOIN Towns AS t ON a.TownID = t.TownID
		WHERE t.[Name] = @TownName

--EXEC usp_GetEmployeesFromTown 'Sofia'


--- 5.Salary Level Function ---
GO 
CREATE FUNCTION ufn_GetSalaryLevel(@Salary DECIMAL(18,4)) 
RETURNS NVARCHAR(10)
AS
BEGIN
	DECLARE @SalaryLevel NVARCHAR(10)
		IF @Salary < 30000
			BEGIN
			SET @SalaryLevel = 'Low'
			END
		ELSE IF @Salary >= 30000 AND @Salary <= 50000
			BEGIN
			SET @SalaryLevel = 'Average'
			END
		ELSE
			BEGIN
			SET @SalaryLevel = 'High'
			END
RETURN @SalaryLevel
END
GO
SELECT dbo.ufn_GetSalaryLevel (50001)


--- 6.Employees by Salary Level ---
GO
CREATE PROCEDURE usp_EmployeesBySalaryLevel (@SalaryLevel NVARCHAR(8))
AS
	IF @SalaryLevel = 'Low'
		SELECT 
			FirstName AS [First Name],
			LastName AS [Last Name]
			FROM Employees
			WHERE dbo.ufn_GetSalaryLevel ([Salary]) = 'Low'
	ELSE IF @SalaryLevel = 'Average'
		SELECT 
			FirstName AS [First Name],
			LastName AS [Last Name]
			FROM Employees
			WHERE dbo.ufn_GetSalaryLevel ([Salary]) = 'Average'
	ELSE
			SELECT 
			FirstName AS [First Name],
			LastName AS [Last Name]
			FROM Employees
			WHERE dbo.ufn_GetSalaryLevel ([Salary]) = 'High'

--EXECUTE dbo.usp_EmployeesBySalaryLevel 'High'
 

--GO
--CREATE OR ALTER PROCEDURE usp_EmployeesBySalaryLevel @salaryLevel VARCHAR(7)
--AS
--BEGIN
--	SELECT FirstName, LastName FROM Employees
--	WHERE dbo.ufn_GetSalaryLevel (Salary) = @salaryLevel
--END
--EXECUTE dbo.usp_EmployeesBySalaryLevel 'Low'
--GO



--- 7.Define Function ---
GO
CREATE FUNCTION ufn_IsWordComprised(@SetOfLetters NVARCHAR(MAX), @Word NVARCHAR(MAX))
RETURNS BIT 
AS
BEGIN 
DECLARE @WordCount INT = LEN(@Word); 
DECLARE @Counter INT = 1;
DECLARE @IsWordCompromised BIT
	WHILE (@WordCount > 0)
		BEGIN
		DECLARE @CurrentLetter CHAR(1)
		SET @CurrentLetter = SUBSTRING(@Word,@Counter, 1)
			IF CHARINDEX(@CurrentLetter, @SetOfLetters) = 0
				BEGIN
				SET @IsWordCompromised = 0
				RETURN @IsWordCompromised 
				END
		SET @Counter += 1;
		SET @WordCount -=1;
		END
SET @IsWordCompromised = 1
RETURN @IsWordCompromised 
END
GO

--SELECT dbo.ufn_IsWordComprised ('oistmiahf', 'Sofia') -- 1
--SELECT dbo.ufn_IsWordComprised ('oistmiahf', 'halves') --0
--SELECT dbo.ufn_IsWordComprised ('bobr', 'Rob') -- 1
--SELECT dbo.ufn_IsWordComprised ('pppp', 'Guy') -- 0

--GO
--CREATE FUNCTION ufn_IsWordComprised(@setOfLetters VARCHAR(50), @word VARCHAR(50)) 
--RETURNS BIT
--BEGIN
--	DECLARE @counter INT = 1
--	DECLARE @isWordComprised BIT = 1

--	WHILE @counter <= LEN(@word)
--		BEGIN
--			DECLARE @currLetter CHAR(1) = SUBSTRING(@word, @counter, 1)
--			DECLARE @currLetterIndex INT = CHARINDEX(@currLetter, @setOfLetters , 1)

--			IF @currLetterIndex = 0
--				BEGIN 
--					SET @isWordComprised = 0
--					BREAK
--				END

--			SET @counter += 1
--		END 
     
--	 RETURN @isWordComprised
--END
--GO


--- 8.* Delete Employees and Departments ---

CREATE PROCEDURE usp_DeleteEmployeesFromDepartment (@departmentId INT)
AS
	-- 01. Delete employees from EmployeesProjects
	DELETE FROM EmployeesProjects
	WHERE EmployeeID IN (
	                     SELECT EmployeeID FROM Employees
	                     WHERE DepartmentID = @departmentId
						)
		            
	-- 02. Set ManagerId to NULL in Employees
	UPDATE Employees
	SET ManagerID = NULL
	WHERE ManagerID IN (
	                     SELECT EmployeeID FROM Employees
	                     WHERE DepartmentID = @departmentId
						)

	-- 03. Alter column ManagerID in Departments and make it NULLable
	ALTER TABLE Departments
	ALTER COLUMN ManagerID INT

	-- 04. Set ManagerId to NULL in Departments
	UPDATE Departments
	SET ManagerID = NULL
	WHERE ManagerID IN (
	                     SELECT EmployeeID FROM Employees
	                     WHERE DepartmentID = @departmentId
						)

   -- 05. Delete all employees from current department
   DELETE FROM Employees
   WHERE DepartmentID = @departmentId

   -- 06. Delete current department
   DELETE FROM Departments
   WHERE DepartmentID = @departmentId

   -- 07. Return 0 count if DELETE was succesfull
   SELECT 
		COUNT(*) 
		FROM Employees 
		WHERE DepartmentID = @departmentId


-- EXECUTE dbo.usp_DeleteEmployeesFromDepartment 16
