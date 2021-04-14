---01.Number of Users for Email Provider---
SELECT a.[Email Provider], COUNT(a.[Email Provider])
FROM (SELECT RIGHT(u.Email, LEN(u.Email) - CHARINDEX('@', u.Email)) AS 'Email Provider'
FROM Users AS u) AS a
GROUP BY a.[Email Provider] ORDER BY COUNT(a.[Email Provider]) DESC, a.[Email Provider]


---02.All Users in Games---
SELECT DISTINCT g.Name AS 'Game', gt.Name AS 'Game Type', u.Username, ug.Level, ug.Cash, ch.Name AS 'Character'
FROM Games AS g, UsersGames AS ug, UserGameItems AS ugi, Users AS u,
GameTypes AS gt, Characters AS ch
WHERE u.Id = ug.UserId AND ug.GameId = ugi.UserGameId
AND ug.GameId = g.Id AND g.GameTypeId = gt.Id
AND ug.CharacterId = ch.Id
ORDER BY ug.Level DESC, u.Username, g.Name


---03.Users in Games with Their Items---
SELECT u.Username, g.Name AS Game, COUNT(ugi.ItemId) AS 'Items Count', SUM(i.Price) AS 'Items Price'
FROM Users AS u
JOIN UsersGames AS ug ON u.Id = ug.UserId
JOIN Games AS g ON g.Id = ug.GameId
JOIN UserGameItems AS ugi ON ugi.UserGameId = ug.Id
JOIN Items AS i ON ugi.ItemId = i.Id
GROUP BY u.Username, g.Name 
HAVING COUNT(ugi.ItemId) >= 10
ORDER BY COUNT(ugi.ItemId) DESC, SUM(i.Price) DESC, u.Username


---04.User in Games with Their Statistics---
SELECT u.Username AS Username,
		g.[Name] AS Game,
		MAX(ch.[Name]) AS Character,
		SUM(sta.Strength) + MAX(gtst.Strength) + MAX(chst.Strength) AS Strength,
		SUM(sta.Defence) + MAX(gtst.Defence) + MAX(chst.Defence) AS Defence,
		SUM(sta.Speed) + MAX(gtst.Speed) + MAX(chst.Speed) AS Speed,
		SUM(sta.Mind) + MAX(gtst.Mind) + MAX(chst.Mind) AS Mind,
		SUM(sta.Luck) + MAX(gtst.Luck) + MAX(chst.Luck) AS Luck
	FROM Users AS u
	JOIN UsersGames AS ug ON u.Id = ug.UserId
	JOIN Games AS g ON ug.GameId = g.Id
	JOIN Characters AS ch ON ug.CharacterId = ch.Id
	JOIN UserGameItems AS ugi ON ug.Id = ugi.UserGameId
    JOIN Items AS i ON ugi.ItemId = i.Id
    JOIN [Statistics] AS sta ON i.StatisticId = sta.Id
    JOIN GameTypes AS gt ON g.GameTypeId = gt.Id
    JOIN [Statistics] AS chst ON ch.StatisticId = chst.Id
    JOIN [Statistics] AS gtst ON gt.BonusStatsId = gtst.Id
		GROUP BY u.Username, g.Name
		ORDER BY Strength DESC, Defence DESC, Speed DESC, Mind DESC, Luck DESC


---05.All Items with Greater than Average Statistics---
SELECT i.Name, i.Price, i.MinLevel, stat.Strength, stat.Defence, stat.Speed, stat.Luck, stat.Mind
FROM Items AS i, [Statistics] AS stat, 
(SELECT AVG(stat.Mind) AS AVGMind, AVG(stat.Luck) AS AVGLuck, AVG(stat.Speed) AS AVGSpeed
FROM [Statistics] AS stat) AS a
WHERE stat.Mind > a.AVGMind AND stat.Luck > a.AVGLuck
AND stat.Speed > a.AVGSpeed AND i.StatisticId = stat.Id


---06.Display All Items about Forbidden Game Type---
SELECT i.Name AS Item, i.Price, i.MinLevel, gt.Name AS 'Forbidden Game Type'
FROM Items AS i
LEFT OUTER JOIN GameTypeForbiddenItems AS gtfi ON i.Id = gtfi.ItemId
LEFT OUTER JOIN GameTypes AS gt ON gt.Id = gtfi.GameTypeId
ORDER BY gt.Name DESC, i.Name


---07.Buy Items for User in Game---
DECLARE @AlexUserGameId  INT = (SELECT Id 
									FROM UsersGames
										WHERE GameId = (SELECT Id FROM Games WHERE [Name] = 'Edinburgh') AND
											  UserId = (SELECT Id FROM Users WHERE Username = 'Alex'))

DECLARE @AlexItemsPrice MONEY = (SELECT SUM(Price) 
									FROM Items
										WHERE [Name] IN ('Blackguard', 
														 'Bottomless Potion of Amplification', 
														 'Eye of Etlich (Diablo III)', 
														 'Gem of Efficacious Toxin', 
													     'Golden Gorget of Leoric', 
														 'Hellfire Amulet'))

DECLARE @GameID INT = (Select GameId 
						FROM UsersGames 
						  WHERE Id = @AlexUserGameId)

INSERT UserGameItems
	SELECT Id, @AlexUserGameId
		FROM Items
			WHERE [Name] IN ('Blackguard', 
							 'Bottomless Potion of Amplification', 
							 'Eye of Etlich (Diablo III)', 
						     'Gem of Efficacious Toxin', 
							 'Golden Gorget of Leoric', 
							 'Hellfire Amulet')

UPDATE UsersGames
	SET Cash = Cash - @AlexItemsPrice
		WHERE Id = @AlexUserGameId

SELECT u.Username, 
	   g.Name, 
	   ug.Cash, 
	   i.Name AS [Item Name]
	FROM Users AS u
	JOIN UsersGames AS ug ON ug.UserId = u.Id
	JOIN Games AS g ON g.Id = ug.GameId
    JOIN UserGameItems AS ugi ON ugi.UserGameId = ug.Id
    JOIN Items AS i ON i.Id = ugi.ItemId
		WHERE ug.GameId = @GameID
			ORDER BY [Item Name]


---08.Peaks and Mountains---
SELECT p.PeakName, m.MountainRange AS [Mountain], p.Elevation
FROM Peaks AS p
JOIN Mountains AS m
	ON p.MountainId = m.Id
ORDER BY p.Elevation DESC, p.PeakName


---09.Peaks with Mountain, Country and Continent---
SELECT p.PeakName, m.MountainRange AS [Mountain], c.CountryName, cont.ContinentName
FROM Peaks AS p
JOIN Mountains AS m
	ON p.MountainId = m.Id
JOIN MountainsCountries AS mc
	ON p.MountainId = mc.MountainId
JOIN Countries AS c
	ON c.CountryCode = mc.CountryCode
JOIN Continents AS cont
	ON cont.ContinentCode = c.ContinentCode
ORDER BY p.PeakName, c.CountryName


---10.Rivers by Country---
SELECT c.CountryName, cont.ContinentName, COUNT(cr.RiverId) AS [RiversCount], 
CASE
	WHEN (SUM(r.Length) IS NULL) THEN 0
	ELSE SUM(r.Length)
END AS [TotalLength]
FROM Countries AS c
LEFT OUTER JOIN CountriesRivers AS cr
	ON cr.CountryCode = c.CountryCode
LEFT OUTER JOIN Rivers AS r
	ON cr.RiverId = r.Id
JOIN Continents AS cont
	ON cont.ContinentCode = c.ContinentCode
GROUP BY c.CountryName, cont.ContinentName
ORDER BY COUNT(cr.RiverId) DESC, [TotalLength] DESC, c.CountryName


---11.Count of Countries by Currency---
SELECT curr.CurrencyCode, curr.Description AS [Currency], COUNT(c.CountryCode) AS [NumberOfCountries]
FROM Currencies AS curr
LEFT JOIN Countries AS c
	ON curr.CurrencyCode = c.CurrencyCode
GROUP BY curr.CurrencyCode, curr.Description
ORDER BY [NumberOfCountries] DESC, [Currency]


---12.Population and Area by Continent---
SELECT cont.ContinentName, SUM(c.AreaInSqKm) AS [CountriesArea], SUM(CAST(C.Population AS BIGINT)) AS [CountriesPopulation]
FROM Continents AS cont
LEFT JOIN Countries AS c
	ON cont.ContinentCode = c.ContinentCode
GROUP BY cont.ContinentName
ORDER BY [CountriesPopulation] DESC


---13.Monasteries by Country---
CREATE TABLE Monasteries (
  Id INT PRIMARY KEY IDENTITY, 
  Name VARCHAR (100) NOT NULL, 
  CountryCode CHAR(2) FOREIGN KEY REFERENCES Countries(CountryCode))

--SUB 2
  INSERT INTO Monasteries(Name, CountryCode) VALUES
('Rila Monastery “St. Ivan of Rila”', 'BG'), 
('Bachkovo Monastery “Virgin Mary”', 'BG'),
('Troyan Monastery “Holy Mother''s Assumption”', 'BG'),
('Kopan Monastery', 'NP'),
('Thrangu Tashi Yangtse Monastery', 'NP'),
('Shechen Tennyi Dargyeling Monastery', 'NP'),
('Benchen Monastery', 'NP'),
('Southern Shaolin Monastery', 'CN'),
('Dabei Monastery', 'CN'),
('Wa Sau Toi', 'CN'),
('Lhunshigyia Monastery', 'CN'),
('Rakya Monastery', 'CN'),
('Monasteries of Meteora', 'GR'),
('The Holy Monastery of Stavronikita', 'GR'),
('Taung Kalat Monastery', 'MM'),
('Pa-Auk Forest Monastery', 'MM'),
('Taktsang Palphug Monastery', 'BT'),
('Sümela Monastery', 'TR')

--SUB 3
--ALTER TABLE Countries
--ADD IsDeleted BIT NOT NULL DEFAULT 0

--SUB 4
UPDATE Countries
SET [IsDeleted] = 1
WHERE CountryCode IN(
       SELECT a.Code 
	     FROM (SELECT c.CountryCode AS Code, 
		             COUNT(cr.RiverId) AS CountRivers FROM Countries AS c
                JOIN CountriesRivers AS cr ON cr.CountryCode = c.CountryCode
               GROUP BY c.CountryCode
              ) AS a
        WHERE a.CountRivers > 3)

--SUB 5
SELECT m.Name AS Monastery,
       f.CountryName AS Country 
	   FROM Monasteries AS m
JOIN (SELECT * FROM Countries
      WHERE IsDeleted = 0) AS f
	  ON f.CountryCode = m.CountryCode
ORDER BY m.Name


---14.Monasteries by Continents and Countries---
UPDATE Countries
	SET CountryName = 'Burma'
WHERE CountryName = 'Myanmar'

INSERT INTO Monasteries (Name, CountryCode)
(
	SELECT 'Hanga Abbey',
		    CountryCode
	FROM Countries
	WHERE CountryName='Tanzania'
)

INSERT INTO Monasteries (Name, CountryCode)
  (SELECT
     'Myin-Tin-Daik',
     CountryCode
   FROM Countries
WHERE CountryName = 'Myanmar')

SELECT cont.ContinentName, c.CountryName, COUNT(mon.Id) AS [MonasteriesCount]
FROM Countries AS c
JOIN Continents AS cont
	ON c.ContinentCode = cont.ContinentCode
LEFT JOIN Monasteries AS mon
	ON c.CountryCode = mon.CountryCode
WHERE c.IsDeleted = 0
GROUP BY c.CountryName, cont.ContinentName
ORDER BY [MonasteriesCount] DESC, c.CountryName


