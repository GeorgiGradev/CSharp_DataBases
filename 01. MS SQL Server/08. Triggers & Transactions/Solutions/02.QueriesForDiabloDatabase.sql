USE Diablo


--- 6.Trigger ---
GO

--create trigger
CREATE OR ALTER TRIGGER trg_UsersShouldByItemsOnTheirLeverOrLower
ON UserGameItems INSTEAD OF INSERT
AS
BEGIN
	DECLARE @UserGameId INT = (SELECT TOP(1) i.UserGameId FROM inserted AS i)
	DECLARE @ItemId INT = (SELECT TOP(1) i.ItemId FROM inserted AS i)

	DECLARE @UserLevel INT = (SELECT ug.[Level] 
		                      FROM UsersGames AS ug
							  JOIN Games AS g ON ug.GameId = g.Id
							  WHERE ug.Id = @UserGameId
							  )
	DECLARE @ItemLevel INT = (SELECT MinLevel FROM Items WHERE Id = @ItemId )

	IF @UserLevel >= @ItemLevel
		BEGIN
		INSERT INTO UserGameItems(ItemId, UserGameId)
			VALUES (@ItemId, @UserGameId)
		END
END

--give cash to users
UPDATE UsersGames
SET Cash += 50000
FROM UsersGames AS ug
JOIN Games AS g ON ug.GameId = g.Id
JOIN Users AS u ON ug.UserId = u.Id
WHERE u.Username IN ('baleremuda', 'loosenoise', 'inguinalself', 'buildingdeltoid', 'monoxidecos')
	AND g.[Name] = 'Bali'

--buy items for users 
DECLARE @FirstGroupCurrItemNum INT = 251
DECLARE @FirstGroupLastItemNum INT = 299

DECLARE @SecondGroupCurrItemNum INT = 501
DECLARE @SecondGroupLastItemNum INT = 539

WHILE 1 = 1
BEGIN
	IF @FirstGroupCurrItemNum <= @FirstGroupLastItemNum
		BEGIN
		BEGIN TRAN
			INSERT INTO UserGameItems(ItemId, UserGameId)
				SELECT @FirstGroupCurrItemNum,
					   ug.Id
				FROM UsersGames AS ug
				JOIN Games AS g ON ug.GameId = g.Id
				JOIN Users AS u ON ug.UserId = u.Id
				WHERE u.Username IN ('baleremuda', 'loosenoise', 'inguinalself', 'buildingdeltoid', 'monoxidecos')
				AND g.[Name] = 'Bali'

			UPDATE UsersGames
				SET Cash -= (SELECT Price FROM Items WHERE id = @FirstGroupCurrItemNum)
				FROM UsersGames AS ug
				JOIN Games AS g ON ug.GameId = g.Id
				JOIN Users AS u ON ug.UserId = u.Id
				WHERE u.Username IN ('baleremuda', 'loosenoise', 'inguinalself', 'buildingdeltoid', 'monoxidecos')
					AND g.[Name] = 'Bali'
        COMMIT
		END

	IF @SecondGroupCurrItemNum <= @SecondGroupCurrItemNum
		BEGIN
		BEGIN TRAN
			INSERT INTO UserGameItems(ItemId, UserGameId)
				SELECT @SecondGroupCurrItemNum,
					   ug.Id
				FROM UsersGames AS ug
				JOIN Games AS g ON ug.GameId = g.Id
				JOIN Users AS u ON ug.Id = u.Id
				WHERE u.Username IN ('baleremuda', 'loosenoise', 'inguinalself', 'buildingdeltoid', 'monoxidecos')
				AND g.[Name] = 'Bali'

			UPDATE UsersGames
				SET Cash -= (SELECT Price FROM Items WHERE id = @SecondGroupCurrItemNum)
				FROM UsersGames AS ug
				JOIN Games AS g ON ug.GameId = g.Id
				JOIN Users AS u ON ug.Id = u.Id
				WHERE u.Username IN ('baleremuda', 'loosenoise', 'inguinalself', 'buildingdeltoid', 'monoxidecos')
					AND g.[Name] = 'Bali'
		COMMIT
		END

		IF @FirstGroupCurrItemNum >= @FirstGroupLastItemNum
		AND @SecondGroupCurrItemNum >= @SecondGroupLastItemNum
		BEGIN
			BREAK;
		END

		SET @FirstGroupCurrItemNum += 1
		SET @SecondGroupCurrItemNum += 1
END

--Display results


--- 7.*Massive Shopping ---
DECLARE @UserName VARCHAR(50) = 'Stamat'
DECLARE @GameName VARCHAR(50) = 'Safflower'
DECLARE @UserID INT = (SELECT Id
                       FROM Users
                       WHERE Username = @UserName)
DECLARE @GameID INT = (SELECT Id
                       FROM Games
                       WHERE Name = @GameName)
DECLARE @UserMoney MONEY = (SELECT Cash
                            FROM UsersGames
                            WHERE UserId = @UserID
                              AND GameId = @GameID)
DECLARE @ItemsTotalPrice MONEY
DECLARE @UserGameID INT = (SELECT Id
                           FROM UsersGames
                           WHERE UserId = @UserID
                             AND GameId = @GameID)

BEGIN TRANSACTION
    SET @ItemsTotalPrice = (SELECT SUM(Price) FROM Items WHERE MinLevel BETWEEN 11 AND 12)

    IF (@UserMoney - @ItemsTotalPrice >= 0)
        BEGIN
            INSERT INTO UserGameItems
            SELECT i.Id, @UserGameID
            FROM Items AS i
            WHERE i.Id IN (SELECT Id FROM Items WHERE MinLevel BETWEEN 11 AND 12)

            UPDATE UsersGames
            SET Cash -= @ItemsTotalPrice
            WHERE GameId = @GameID
              AND UserId = @UserID
            COMMIT
        END
    ELSE
        BEGIN
            ROLLBACK
        END

    SET @UserMoney = (SELECT Cash FROM UsersGames WHERE UserId = @UserID AND GameId = @GameID)
    BEGIN TRANSACTION
        SET @ItemsTotalPrice = (SELECT SUM(Price) FROM Items WHERE MinLevel BETWEEN 19 AND 21)

        IF (@UserMoney - @ItemsTotalPrice >= 0)
            BEGIN
                INSERT INTO UserGameItems
                SELECT i.Id, @UserGameID
                FROM Items AS i
                WHERE i.Id IN (SELECT Id FROM Items WHERE MinLevel BETWEEN 19 AND 21)

                UPDATE UsersGames
                SET Cash -= @ItemsTotalPrice
                WHERE GameId = @GameID
                  AND UserId = @UserID
                COMMIT
            END
        ELSE
            BEGIN
                ROLLBACK
            END

        SELECT Name AS [Item Name]
        FROM Items
        WHERE Id IN (SELECT ItemId FROM UserGameItems WHERE UserGameId = @userGameID)
        ORDER BY [Item Name]