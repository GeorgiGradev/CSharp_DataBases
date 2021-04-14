USE ColonialJourney

---02.Insert---
INSERT INTO Planets (Name)
	VALUES
	('Mars'),
	('Earth'),
	('Jupiter'),
	('Saturn')

INSERT INTO Spaceships (Name, Manufacturer, LightSpeedRate)
	VALUES
	('Golf', 'VW',	3),
	('WakaWaka', 'Wakanda',	4),
	('Falcon9',	'SpaceX',	1),
	('Bed',	  'Vidolov',	6)


---03.Update---
UPDATE
	Spaceships
	SET LightSpeedRate += 1
	WHERE Id BETWEEN 8 AND 12

	
---04.Delete---
DELETE 
	FROM TravelCards
	WHERE JourneyId BETWEEN 1 AND 3

DELETE
	FROM Journeys
	WHERE Id BETWEEN 1 AND 3


---05.Select all military journeys---
SELECT 
	Id,
	FORMAT(JourneyStart, 'dd/MM/yyyy'),
	FORMAT(JourneyEnd, 'dd/MM/yyyy')
	FROM Journeys
	WHERE Purpose = 'Military'
	ORDER BY FORMAT(JourneyStart, 'dd/MM/yyyy')


---06.Select all pilots---
SELECT 
	c.Id,
	CONCAT(c.FirstName, ' ', c.LastName)
	FROM Colonists as c
	JOIN TravelCards as tc ON c.Id = tc.ColonistId
	WHERE tc.JobDuringJourney LIKE 'Pilot'
	ORDER BY c.Id


---07.Count colonists---
SELECT
	COUNT(*)
	FROM Colonists AS c
	JOIN TravelCards AS tc ON c.Id = tc.ColonistId
	JOIN Journeys AS j ON tc.JourneyId = j.Id
	WHERE j.Purpose LIKE 'Technical'


---08.Select spaceships with pilots younger than 30 years---
SELECT
	s.Name,
	s.Manufacturer
	FROM Spaceships as s
	JOIN Journeys as j ON s.Id = j.SpaceshipId
	JOIN TravelCards as tc ON j.Id = tc.JourneyId
	JOIN Colonists as c ON tc.ColonistId = c.Id
	WHERE DATEDIFF(YEAR, c.BirthDate, '01-01-2019' ) < 30 AND tc.JobDuringJourney LIKE 'Pilot'
	ORDER BY s.Name


---09.Select all planets and their journey count---
SELECT
	p.Name,
	COUNT(j.Id)
	FROM Planets as p
	JOIN Spaceports as sp ON p.Id = sp.PlanetId
	JOIN Journeys as j ON sp.Id = j.DestinationSpaceportId
	GROUP BY p.Name
	ORDER BY COUNT(j.Id) DESC, p.Name


---10.Select Second Oldest Important Colonist---

SELECT 
	Job,
	FullName,
	Rank
	FROM (SELECT 
		tc.JobDuringJourney as Job,
		CONCAT(c.FirstName, ' ', c.LastName) as FullName,
		c.Birthdate AS Burthdate,
		DENSE_RANK() OVER (PARTITION BY tc.JobDuringJourney ORDER BY c.Birthdate) AS [Rank]
		FROM Colonists as c
		JOIN TravelCards as tc ON c.Id = tc.ColonistId) as temp
	WHERE temp.Rank = 2
	

---11.Get Colonists Count---
GO
CREATE FUNCTION dbo.udf_GetColonistsCount(@PlanetName VARCHAR (30)) 
RETURNS INT
AS
BEGIN
	DECLARE @Result INT = (
	SELECT 
		COUNT(c.Id) 
		FROM Colonists as c
		JOIN TravelCards as tc ON c.Id = tc.ColonistId
		JOIN Journeys as j ON tc.JourneyId = j.Id
		JOIN Spaceports as sp ON j.DestinationSpaceportId = sp.Id
		JOIN Planets as p ON sp.PlanetId = p.Id
		WHERE p.Name = @PlanetName)
RETURN @Result
END
GO
--SELECT dbo.udf_GetColonistsCount('Otroyphus')


---12.Change Journey Purpose
GO
CREATE OR ALTER PROC usp_ChangeJourneyPurpose(@JourneyId INT, @NewPurpose VARCHAR(11))
AS
BEGIN
	DECLARE @CurrentJourneyId INT = (
		SELECT
			Id
			FROM Journeys
			WHERE Id = @JourneyId)

	DECLARE @CurrentJourneyPurpose VARCHAR(11) = (
		SELECT
			Purpose 
			FROM Journeys
			WHERE Id =  @JourneyId)


	IF (@CurrentJourneyId IS NULL)
		 THROW 50001, 'The journey does not exist!', 1;

	IF (@CurrentJourneyPurpose =  @NewPurpose)
		 THROW 50002, 'You cannot change the purpose!', 1;


	UPDATE
			Journeys 
			SET Purpose = @NewPurpose
			WHERE Id = @JourneyId
END

--EXEC usp_ChangeJourneyPurpose 2, 'Educational'
--EXEC usp_ChangeJourneyPurpose 196, 'Technical'		
--EXEC usp_ChangeJourneyPurpose 4, 'Technical'
--EXEC usp_ChangeJourneyPurpose 4, 'Educational'

