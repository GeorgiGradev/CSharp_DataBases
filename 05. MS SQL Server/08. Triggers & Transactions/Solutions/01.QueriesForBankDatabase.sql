 USE Bank


--- 01.Create Table Logs ---
--CREATE TABLE Logs
--(
--	LogId INT PRIMARY KEY IDENTITY NOT NULL,
--	AccountId INT FOREIGN KEY REFERENCES Accounts(Id),
--	OldSum Money,
--	NewSum Money
--)

GO
CREATE TRIGGER tr_AccountChange 
ON Accounts FOR UPDATE
AS
BEGIN
	INSERT INTO Logs (AccountId, OldSum, NewSum)
	SELECT  i.AccountHolderId, d.Balance, i.Balance
		FROM inserted as i
		JOIN deleted AS d ON i.Id = d.Id
END
GO
 

--- 2.Create Table Emails ---
CREATE TABLE NotificationEmail
(
	Id INT PRIMARY KEY IDENTITY,
	Recipient INT FOREIGN KEY REFERENCES Accounts(Id),
	[Subject] NVARCHAR(50),
	Body NVARCHAR(MAX)
)

GO
CREATE TRIGGER tr_SendEmailOnUpdate
ON Logs FOR INSERT
AS
BEGIN
	INSERT INTO NotificationEmail (Recipient, [Subject], Body)
	SELECT 
	i.AccountId, 
		CONCAT(
			'Balance change for account: ',
			i.AccountId),
		CONCAT(
			'On ',
			FORMAT(GETDATE(),
			'MMM dd yyyy HH:mmtt'),
			' your balance was changed from ',
			i.OldSum, ' to ',
			i.NewSum,
			'.')
	FROM inserted AS i
END
GO

--UPDATE Accounts
--SET Balance += 100
--WHERE Id = 1;

--SELECT * FROM Logs
--SELECT * FROM NotificationEmail



--- 3.Deposit Money ---
--CREATE TABLE BankTable
--(
--	AccountId INT PRIMARY KEY IDENTITY,
--	AccountHolderId INT NOT NULL,
--	Balance DECIMAL(18,4)
--)

--GO
--CREATE OR ALTER PROC usp_DepositMoney (@AccountId INT, @MoneyAmount DECIMAL(18,4)) 
--AS
--BEGIN TRANSACTION
--	IF (@MoneyAmount <= 0 
--		OR (SELECT Balance FROM Accounts AS a WHERE a.Id = @AccountId)  + @MoneyAmount < 0)
--		BEGIN
--			ROLLBACK;
--			THROW 50004, 'Invalid amount value.',1
--		END
--	ELSE
--		BEGIN
--			UPDATE Accounts 
--			SET Balance += @MoneyAmount
--			WHERE Id = @AccountId

--			INSERT INTO BankTable (AccountHolderId, Balance)
--				SELECT 
--					AccountHolderId, Balance
--					FROM Accounts AS a
--				WHERE a.Id = @AccountId
--		END
--COMMIT
--GO

--EXEC usp_DepositMoney 14, 10000

--SELECT * FROM BankTable
--SELECT * FROM Logs
--SELECT * FROM NotificationEmail
--SELECT * FROM Accounts

--UPDATE Accounts
--SET Balance -= 2000
--WHERE Id = 14

GO

CREATE PROCEDURE usp_DepositMoney @AccountId INT, @MoneyAmount MONEY
AS
	IF (SELECT COUNT(*) FROM Accounts WHERE Id = @AccountId) > 0
	AND @MoneyAmount > 0
		BEGIN
			UPDATE Accounts
			SET Balance += @MoneyAmount
			WHERE Id = @AccountId
		END 


GO

--- 4.Withdraw Money ---
GO
CREATE PROCEDURE usp_WithdrawMoney @AccountId INT, @MoneyAmount MONEY
AS
	IF (SELECT COUNT(*) FROM Accounts WHERE Id = @AccountId) > 0
	AND @MoneyAmount > 0
		BEGIN
			UPDATE Accounts
			SET Balance -= @MoneyAmount
			WHERE Id = @AccountId
		END 

GO


--- 5.Money Transfer ---
CREATE OR ALTER PROC usp_TransferMoney @SenderId INT, @ReceiverId INT, @Amount MONEY
AS
	EXEC dbo.usp_WithdrawMoney @SenderId, @Amount
	EXEC dbo.usp_DepositMoney @ReceiverId, @Amount

