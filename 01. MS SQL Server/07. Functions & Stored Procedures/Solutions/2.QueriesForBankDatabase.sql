USE Bank


--- 9.Find Full Name ---
GO
CREATE PROC usp_GetHoldersFullName 
AS
	SELECT 
		CONCAT([FirstName],' ', [LastName]) AS [Full Name]
		FROM AccountHolders

--EXEC dbo.usp_GetHoldersFullName


--- 10.People with Balance Higher Than ---
GO
CREATE PROC usp_GetHoldersWithBalanceHigherThan (@MinBalance DECIMAL (18,4))
AS
	SELECT
		ah.FirstName AS [First Name],
		ah.LastName AS [Last Name]
		FROM AccountHolders AS ah
		JOIN Accounts as a  ON ah.Id = a.AccountHolderId
		GROUP BY ah.FirstName, ah.LastName
		HAVING SUM(a.Balance) > @MinBalance
		ORDER BY ah.FirstName, ah.LastName

--EXEC dbo.usp_GetHoldersWithBalanceHigherThan 20000


--- 11.Future Value Function ---
GO
CREATE FUNCTION ufn_CalculateFutureValue 
	(@InitialSum DECIMAL(18,4),
	@YearlyInterestRate FLOAT, 
	@NumberOfYears INT)
RETURNS DECIMAL(18,4)
BEGIN
	DECLARE @Output DECIMAL(18,4);
	SET @Output = 
	@InitialSum
	* 
	(POWER(1 + (@YearlyInterestRate), @NumberOfYears))
RETURN @Output 
END
GO
--SELECT dbo.ufn_CalculateFutureValue (1000, 0.1, 5)


--- 12.Calculating Interest ---
GO
CREATE PROCEDURE usp_CalculateFutureValueForAccount 
	(@AccountID INT, 
	@InterestRate DECIMAL(18,2))
AS
	SELECT 
		a.Id AS [Account Id],
		ah.FirstName AS [First Name],
		ah.LastName AS [Last Name],
		a.Balance AS [Current Balance],
		dbo.ufn_CalculateFutureValue (a.Balance, @InterestRate, 5) AS [Balance in 5 years]

		FROM Accounts as a
		JOIN AccountHolders as ah ON a.AccountHolderId = ah.Id
		WHERE a.Id = @AccountID
-- EXEC dbo.usp_CalculateFutureValueForAccount 1, 0.1