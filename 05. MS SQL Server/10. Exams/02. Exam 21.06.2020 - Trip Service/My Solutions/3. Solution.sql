USE TripService


--- 02.Insert ---
INSERT INTO Accounts (FirstName, MiddleName, LastName, CityId, BirthDate, Email)
	VALUES
('John',	'Smith',	'Smith',	34,	'1975-07-21',	'j_smith@gmail.com'),
('Gosho',	NULL,	'Petrov',	11,	'1978-05-16',	'g_petrov@gmail.com'),
('Ivan',	'Petrovich',	'Pavlov',	59,	'1849-09-26',	'i_pavlov@softuni.bg'),
('Friedrich',	'Wilhelm',	'Nietzsche',	2,	'1844-10-15',	'f_nietzsche@softuni.bg')

INSERT INTO Trips (RoomId, BookDate, ArrivalDate, ReturnDate, CancelDate)
	VALUES
(101,	'2015-04-12',	'2015-04-14',	'2015-04-20',	'2015-02-02'),
(102,	'2015-07-07',	'2015-07-15',	'2015-07-22',	'2015-04-29'),
(103,	'2013-07-17',	'2013-07-23',	'2013-07-24',	NULL		),
(104,	'2012-03-17',	'2012-03-31',	'2012-04-01',	'2012-01-10'),
(109,	'2017-08-07',	'2017-08-28',	'2017-08-29',	NULL		)


--- 03.UPDATE---
UPDATE 
	Rooms
	SET PRICE *= 1.14
	WHERE HotelId IN (5, 7, 9)


--- 04.Delete ---
DELETE
FROM AccountsTrips
WHERE AccountId = 47


---05.EEE-Mails ---
SELECT 
	a.FirstName, 
	a.LastName,
	FORMAT(a.BirthDate, 'MM-dd-yyyy') as BirthDate,
	c.Name, 
	a.Email
	FROM Accounts as a
	JOIN Cities as c ON a.CityId = c.Id
	WHERE LEFT(a.FirstName, 1) = 'e'
	ORDER BY c.Name


--- 06.City Statistics
SELECT
	temp.City as City, 
	COUNT(temp.Id) as Hotels
	FROM (SELECT 
		c.Name as City,
		h.Id as Id
		FROM Cities as c
		JOIN Hotels as h ON c.Id = h.CityId) as temp
	GROUP BY temp.City
	ORDER BY Hotels DESC, City


SELECT
	c.Name as City,
	COUNT(h.Id) as Hotels
	FROM Cities as c
	JOIN Hotels as h ON c.Id = h.CityId
	GROUP BY c.Name
	ORDER BY Hotels DESC, City


--- 7. Longest and Shortest Trips
SELECT 
	a.Id as AccountId,
	CONCAT(a.FirstName, ' ', a.LastName) as FullName,
	MAX(DATEDIFF(DAY, t.ArrivalDate, t.ReturnDate)) as LongestTrip,
	MIN(DATEDIFF(DAY, t.ArrivalDate, t.ReturnDate)) as ShortestTrip
	FROM Accounts as a
	JOIN AccountsTrips as at ON a.Id = at.AccountId
	JOIN Trips as t ON at.TripId = t.Id
	WHERE a.MiddleName IS NULL AND t.CancelDate IS NULL
	GROUP BY a.Id, CONCAT(a.FirstName, ' ', a.LastName)
	ORDER BY LongestTrip DESC, ShortestTrip

---8.Metropolis ---
SELECT TOP(10)
	c.Id as Id,
	c.Name as City,
	c.CountryCode as Country,
	COUNT(c.Id) as Accounts
	FROM Cities as c
	JOIN Accounts as a ON c.Id = a.CityId
	GROUP BY c.Id, c.Name, c.CountryCode
	ORDER BY COUNT(c.Id) DESC


--9.Romantic Getaways ---
SELECT
	a.Id,
	a.Email,
	c.Name AS City,
	COUNT(t.Id) as Trips
	FROM Accounts as a
	JOIN Cities as c ON a.CityId = c.Id
	JOIN AccountsTrips as at ON at.AccountId = a.Id
	JOIN Trips as t ON at.TripId = t.Id
	JOIN Rooms as r ON r.Id = t.RoomId
	JOIN Hotels as h ON h.Id = r.HotelId
	WHERE a.CityId = h.CityId
	GROUP BY a.Id, Email, c.Name
	ORDER BY COUNT(t.Id) DESC, a.Id


--- 10. GDPR Violation --- 
SELECT 
	t.Id as Id,
	CONCAT(a.FirstName, ' ', COALESCE(a.MiddleName+ ' ',''), a.LastName) AS FullName,
	(SELECT TOP(1) Cities.Name FROM Cities JOIN Accounts ON Cities.Id = a.CityId) as [From],
	c.Name as [To],
	(CASE 
		WHEN t.CancelDate IS NULL THEN CONCAT(DATEDIFF(DAY, t.ArrivalDate, t.ReturnDate), ' days')
		ELSE 'Canceled'
	END) as Duration
	FROM Trips AS t
	JOIN AccountsTrips AS at ON t.Id = at.TripId
	JOIN Accounts AS a ON a.Id = at.AccountId
	JOIN Rooms as r ON r.Id = t.RoomId
	JOIN Hotels as h ON h.Id = r.HotelId
	JOIN Cities as c ON c.Id = h.CityId
	ORDER BY FullName, Id


---11.Available Room ---
GO
CREATE FUNCTION udf_GetAvailableRoom(@HotelId INT, @Date DATETIME, @People INT)
RETURNS VARCHAR(Max)
AS
BEGIN
	DECLARE @RoomPrice DECIMAL(18,2)
	DECLARE	@BedsInRoom INT
	DECLARE @RoomType VARCHAR(20)
	DECLARE @RoomId INT
	DECLARE @HotelBaseRate DECIMAL(18,2)

	SET @RoomId  = 
		(SELECT TOP(1)
		r.Id as RoomId
		FROM Hotels as h 
		JOIN Rooms as r ON h.Id = r.HotelId
		JOIN Trips as t ON r.Id = t.RoomId
		WHERE (r.Id NOT IN (
			SELECT 
			Rooms.Id
			FROM Hotels 
			JOIN Rooms ON Hotels.Id = Rooms.HotelId 
			JOIN Trips ON Rooms.Id = Trips.RoomId
			WHERE 
				(@Date BETWEEN FORMAT(Trips.ArrivalDate, 'yyyy-MM-dd') AND FORMAT(Trips.ReturnDate, 'yyyy-MM-dd'))))
		AND t.CancelDate IS NULL 
		AND h.Id = @HotelId AND r.Beds >= @People
		
		ORDER BY r.Price DESC)

		IF (@RoomId IS NOT NULL)
			BEGIN		
				SET @RoomPrice = 
					(SELECT
						Price
						FROM Rooms
						WHERE Id = @RoomId)

				SET @BedsInRoom =
					(SELECT	
						Beds
						FROM Rooms
						WHERE Id = @RoomId)

				SET @RoomType =
					(SELECT
						Type
						FROM Rooms
						WHERE Id = @RoomId)

				SET @HotelBaseRate =
				(SELECT
						BaseRate
						FROM Hotels
						JOIN Rooms ON Hotels.Id = Rooms.HotelId
						WHERE Rooms.Id = @RoomId)
			END
		ELSE
		 RETURN 'No rooms available';

RETURN CONCAT('Room ', @RoomId,': ', @RoomType, ' (',@BedsInRoom, ' beds) - $',(@HotelBaseRate + @RoomPrice) * @People)
END	
GO


SELECT dbo.udf_GetAvailableRoom(112, '2011-12-17', 2)
SELECT dbo.udf_GetAvailableRoom(94, '2015-07-26', 3)


--- 12.Switch Room --- 
GO
CREATE PROC usp_SwitchRoom (@TripId INT, @TargetRoomId INT)
AS
BEGIN
	DECLARE	@CurrentRoom INT = (SELECT TOP(1) RoomId FROM Trips WHERE Id = @TripId)

	IF (SELECT TOP(1) HotelId FROM Rooms WHERE Id = @CurrentRoom) !=
	   (SELECT TOP(1) HotelId FROM Rooms WHERE Id = @TargetRoomId)
			THROW 51000, 'Target room is in another hotel!', 1

	
	IF (SELECT Beds FROM Rooms WHERE Id = @TargetRoomId) <
	   (
	    SELECT 
			COUNT(*)
			FROM Trips AS t
			JOIN AccountsTrips AS at ON t.Id = at.TripId
			WHERE t.Id = @TripId
			GROUP BY t.Id
		   )
				THROW 51001, 'Not enough beds in target room!', 1

	   UPDATE Trips
	   SET RoomId = @TargetRoomId
	   WHERE Id = @TripId
END

GO

EXEC usp_SwitchRoom 10, 7
EXEC usp_SwitchRoom 10, 8