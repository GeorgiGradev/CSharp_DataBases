USE Airport

---02.Insert---
INSERT INTO Planes (Name, Seats, Range)
	VALUES
	('Airbus 336',	112,	5132),
	('Airbus 330',	432,	5325),
	('Boeing 369',	231,	2355),
	('Stelt 297',	254,	2143),
	('Boeing 338',	165,	5111),
	('Airbus 558',	387,	1342),
	('Boeing 128',	345,	5541)

INSERT INTO LuggageTypes (Type)
	VALUES
	('Crossbody Bag'),
	('School Backpack'),
	('Shoulder Bag')


---03.Update---
UPDATE
	Tickets
	SET Price *= 1.13
	FROM Tickets as t
	JOIN Flights as f ON t.FlightId = f.Id
	WHERE f.Destination = 'Carlsbad'


---04.Delete---
DELETE
	FROM Tickets 
	WHERE FlightId = (SELECT
							Id
							FROM Flights
							WHERE Destination = 'Ayn Halagim')

DELETE
	FROM Flights
	WHERE Destination = 'Ayn Halagim'

---05.Trips---
SELECT [Origin], [Destination] FROM Flights
	ORDER BY [Origin] ASC,
			 [Destination] ASC


---06.The "Tr" Planes---
SELECT 
	*
	FROM Planes
	WHERE Name LIKE '%tr%'
	ORDER BY Id, Name, Seats, Range


---07.Flight Profits---
SELECT
	f.Id AS FLightId,
	SUM(t.Price) AS Price
	FROM Flights AS f
	JOIN Tickets AS t ON f.Id = t.FlightId
	GROUP BY f.Id
	ORDER BY SUM(t.Price) DESC, f.Id


---08.Passengers and Prices---
SELECT TOP(10)
	p.FirstName,
	p.LastName,
	t.Price
	FROM Passengers AS p
	JOIN Tickets AS t ON p.Id = t.PassengerId
	ORDER BY Price DESC, p.FirstName, p.LastName


---09/Most Used Luggage's---
SELECT 
	Type,
	COUNT(*)
	FROM LuggageTypes as lt
	JOIN Luggages as l ON lt.Id = l.LuggageTypeId
	GROUP BY Type
	ORDER BY COUNT(*) DESC, Type


---10.Passenger Trips---
SELECT
	CONCAT(p.FirstName, ' ', p.LastName),
	f.Origin,
	f.Destination
	FROM Passengers as p
	JOIN Tickets as t ON p.Id = t.PassengerId
	JOIN Flights as f ON f.Id = t.FlightId
	ORDER BY CONCAT(p.FirstName, ' ', p.LastName), f.Origin, f.Destination


---11.Non Adventures People---
SELECT 
	FirstName, 
	LastName, 
	Age
	FROM Passengers AS p
	LEFT JOIN Tickets AS t ON p.Id = t.PassengerId
	WHERE t.Id IS NULL
	ORDER BY Age DESC, FirstName, LastName


---12.Lost Luggage's---
SELECT 
	PassportId,
	Address
	FROM Passengers AS p
	LEFT JOIN Luggages AS l ON p.Id = l.PassengerID
	WHERE l.Id IS NULL
	ORDER BY PassportId, Address


----13.Count of Trips---
SELECT	
	FirstName, 
	LastName,
	COUNT(t.FlightId)
	FROM Passengers AS p
	LEFT JOIN Tickets AS t ON p.Id = t.PassengerId
	GROUP BY FirstName, LastName
	ORDER BY COUNT(t.FlightId) DESC, FirstName, LastName


---	14.Full Info---
SELECT CONCAT(Passengers.FirstName, ' ', Passengers.LastName) AS [Full Name],
	   Planes.[Name],
	   CONCAT(Flights.Origin, ' - ', Flights.Destination),
	   LuggageTypes.[Type]
	FROM Passengers
	JOIN Tickets ON Passengers.Id = Tickets.PassengerId
	JOIN Flights ON Tickets.FlightId = Flights.Id
	JOIN Planes ON Flights.PlaneId = Planes.Id
	JOIN Luggages ON Tickets.LuggageId = Luggages.Id
	JOIN LuggageTypes ON Luggages.LuggageTypeId = LuggageTypes.Id
		ORDER BY [Full Name] ASC,
				 Planes.[Name] ASC,
				 Flights.Origin ASC,
				 Flights.Destination ASC,
				  LuggageTypes.[Type] ASC


---15.Most Expensive Trips---
SELECT FirstName,
       LastName,
	   Destination,
	   Price
	   FROM (SELECT 
				   Passengers.FirstName,
				   Passengers.LastName,
				   Flights.Destination,
				   Tickets.Price,
				   RANK() OVER (PARTITION BY Passengers.Id ORDER BY Price DESC) AS [Rank]
					FROM Passengers 
					JOIN Tickets ON Passengers.Id = Tickets.PassengerId
					JOIN Flights ON Tickets.FlightId = Flights.Id) AS [all PassengersQuery]
		WHERE [Rank] = 1
		ORDER BY Price DESC,
	         FirstName ASC,
	         LastName ASC,
	         Destination ASC


---16.Destinations Info---
SELECT 
	Destination,
	COUNT(t.Id)
	FROM Flights AS f
	LEFT JOIN Tickets AS t ON f.Id = t.FlightId
	GROUP BY Destination
	ORDER BY COUNT(t.Id) DESC, f.Destination


---17.PSP---
SELECT 
	p.Name,
	p.Seats,
	COUNT(t.Id)
	FROM Planes AS p
	LEFT JOIN Flights AS f ON p.Id = f.PlaneId
	LEFT JOIN Tickets AS t ON f.Id = t.FlightId
	GROUP BY p.Name, p.Seats
	ORDER BY COUNT(t.Id) DESC, p.Name, p.Seats

SELECT p.[Name],
       p.Seats,
	   COUNT(t.Id) AS [Passengers Count]
FROM Planes AS p
LEFT JOIN Flights AS f ON p.Id = f.PlaneId
LEFT JOIN Tickets AS t ON f.Id = t.FlightId
GROUP BY p.[Name], p.Seats
ORDER BY [Passengers Count] DESC,
	     p.[Name],
		 p.Seats


---18.Vacation---
GO
CREATE FUNCTION udf_CalculateTickets(@origin VARCHAR(50), @destination VARCHAR(50), @peopleCount INT) 
RETURNS NVARCHAR(30)
AS 
BEGIN
	DECLARE @TotalPrice DECIMAL(18,2)
	DECLARE @PriceForOnePerson DECIMAL (18,2)
	DECLARE @ReturnMessage NVARCHAR(30)
	DECLARE @CurrentFlightId INT = ISNULL((SELECT
						Id 
						FROM Flights
						WHERE Origin = @origin AND Destination = @destination), 0)

	IF (@peopleCount <= 0)
		BEGIN
		SET @ReturnMessage = 'Invalid people count!'
		RETURN @ReturnMessage;
		END

	ELSE IF (@CurrentFlightId = 0)
		BEGIN
		SET @ReturnMessage = 'Invalid flight!'
		RETURN @ReturnMessage;
		END
	
	ELSE
		BEGIN
		 SET @PriceForOnePerson = (SELECT
							t.Price
							FROM Tickets AS t
							JOIN Flights AS f ON t.FlightId = f.Id
							WHERE f.Id = @CurrentFlightId)
		SET @TotalPrice = @PriceForOnePerson * @peopleCount
		SET @ReturnMessage = CONCAT('Total price ', @TotalPrice)
		END

RETURN @ReturnMessage
END
GO


---19.Wrong Data---
CREATE PROCEDURE usp_CancelFlights
AS
	UPDATE
	   Flights 
	   SET DepartureTime = NULL, ArrivalTIme = NULL
	   WHERE DepartureTime < ArrivalTIme


---20.Deleted Planes---
CREATE TABLE DeletedPlanes(
	Id INT NOT NULL,
	[Name] VARCHAR(50) NOT NULL,
	Seats INT NOT NULL,
	[Range] INT NOT NULL
)

GO

CREATE TRIGGER trg_SaveOnPlaneDelete
ON Planes AFTER DELETE
AS
BEGIN
	INSERT INTO DeletedPlanes
		SELECT Id,
		       [Name],
			   Seats,
			   [Range]
		FROM deleted
END
