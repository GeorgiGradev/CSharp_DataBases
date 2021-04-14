USE Geography

--- 12.Highest Peaks in Bulgaria ---
SELECT 
	mc.[CountryCode],
	m.[MountainRange],
	p.[PeakName],
	p.[Elevation]
	FROM Mountains as m
	JOIN MountainsCountries as mc ON m.Id = mc.MountainId AND mc.[CountryCode] = 'BG'
	JOIN Peaks as p ON mc.MountainId = p.MountainId AND p.[Elevation] > 2835
	ORDER BY p.[Elevation] DESC

--SELECT c.CountryCode,
--	   m.MountainRange,
--	   p.PeakName,
--	   p.Elevation
--FROM Countries AS c
--INNER JOIN MountainsCountries AS mc ON c.CountryCode = mc.CountryCode
--INNER JOIN Mountains AS m ON mc.MountainId = m.Id
--INNER JOIN Peaks AS p ON m.Id = p.MountainId
--WHERE c.CountryName = 'Bulgaria' AND p.Elevation > 2835
--ORDER BY p.Elevation DESC


--- 13.Count Mountain Ranges ---
SELECT 
	[CountryCode], 
	COUNT(MountainId) AS [MouintainRanges]
	FROM MountainsCountries
	WHERE CountryCode IN ('BG','RU','US')
	GROUP BY CountryCode


--- 14.Countries with Rivers ---
SELECT TOP(5)
	c.CountryName,
	r.RiverName
	FROM Rivers as r
	JOIN CountriesRivers as cr ON r.Id = cr.RiverId
	RIGHT JOIN Countries as c ON cr.CountryCode = c.CountryCode
	WHERE c.ContinentCode = 'AF'
	ORDER BY c.CountryName

--SELECT TOP(5) 
-- 	c.CountryName,
--	r.RiverName
--	FROM Countries AS c
--	LEFT JOIN CountriesRivers AS cr ON c.CountryCode = cr.CountryCode
--	LEFT JOIN Rivers AS r ON cr.RiverId = r.Id
--	WHERE c.ContinentCode = 'AF'
--	ORDER BY c.CountryName


--- 15.*Continents and Currencies ---
SELECT 
	ContinentCode, --a
	CurrencyCode, -- b
	CurrencyCount AS CurrencyUsage -- c
	FROM (SELECT
		ContinentCode, -- a
		CurrencyCode,  -- b
		CurrencyCount, -- c
		DENSE_RANK() OVER (PARTITION BY ContinentCode ORDER BY CurrencyCount DESC) AS CurrencyRank -- d
		FROM (SELECT
			ContinentCode, -- a
			CurrencyCode,  -- b
			COUNT(CurrencyCode) AS CurrencyCount -- c 
			FROM Countries 
			GROUP BY ContinentCode, CurrencyCode) AS CurrencyCountQuery 
		WHERE CurrencyCount > 1) AS CurrencyRankingQuery
	WHERE CurrencyRank = 1 -- d
	ORDER BY ContinentCode -- a



--- 16.Countries without Any Mountains ---
SELECT
	COUNT(qwerty.CountryCode) AS [Count]
	FROM (SELECT 
	Countries.CountryCode,
	Mountains.Id
	FROM Countries 
	LEFT JOIN MountainsCountries ON Countries.CountryCode = MountainsCountries.CountryCode
	LEFT JOIN Mountains ON MountainsCountries.MountainId = Mountains.Id
	WHERE Mountains.Id IS NULL) as qwerty

--SELECT 
--	COUNT(*) 
--	FROM Countries AS c
--	LEFT OUTER JOIN MountainsCountries AS mc ON c.CountryCode = mc.CountryCode
--	WHERE mc.CountryCode IS NULL


--- 17.Highest Peak and Longest River by Country ---
SELECT TOP(5)
	Countries.CountryName AS CountryName,
	MAX(Peaks.Elevation) AS HighestPeakElevation,
	MAX(Rivers.[Length]) AS LongestRiverLength
	FROM Peaks
	LEFT JOIN Mountains ON Mountains.Id = Peaks.MountainId
	LEFT JOIN MountainsCountries ON Mountains.Id = MountainsCountries.MountainId
	RIGHT JOIN Countries ON MountainsCountries.CountryCode = Countries.CountryCode 
	LEFT JOIN CountriesRivers ON Countries.CountryCode = CountriesRivers.CountryCode
	LEFT JOIN Rivers ON CountriesRivers.RiverId = Rivers.Id
	GROUP BY Countries.CountryName
	ORDER BY HighestPeakElevation DESC,
			   LongestRiverLength DESC, 
			      Countries.CountryName

--SELECT TOP(5)
--	CountryName,
--	MAX(p.Elevation) AS HighestPeakElevation,
--	MAX(r.[Length]) AS LongestRiverLength
--	FROM Countries AS c
--	LEFT OUTER JOIN CountriesRivers AS cr ON c.CountryCode = cr.CountryCode
--	LEFT OUTER JOIN Rivers AS r ON cr.RiverId = r.Id
--	LEFT OUTER JOIN MountainsCountries AS mc ON c.CountryCode = mc.CountryCode
--	LEFT OUTER JOIN Mountains AS m ON mc.MountainId = m.Id
--	LEFT OUTER JOIN Peaks AS p ON m.Id = p.MountainId
--	GROUP BY c.CountryName
--	ORDER BY HighestPeakElevation DESC,
--			   LongestRiverLength DESC,
--			     CountryName ASC


--- 18.Highest Peak Name and Elevation by Country ---

SELECT TOP (5)
	Country, 
	[Highest Peak Name],
	[Highest Peak Elevation], 
	Mountain
	FROM (SELECT 
	*,
	DENSE_RANK () OVER (PARTITION BY Country ORDER BY [Highest Peak Elevation] DESC) AS [Rank]
	FROM (SELECT 
		Countries.CountryName AS Country,
		CASE
			WHEN Peaks.PeakName IS NULL THEN '(no highest peak)'
			ELSE Peaks.PeakName
		END AS [Highest Peak Name],
		CASE
			WHEN Peaks.Elevation IS NULL THEN '0'
			ELSE Peaks.Elevation
		END AS [Highest Peak Elevation],
		CASE
			WHEN Mountains.MountainRange IS NULL THEN '(no mountain)' 
			ELSE Mountains.MountainRange 
		END AS Mountain
		FROM Countries
		LEFT JOIN MountainsCountries ON Countries.CountryCode = MountainsCountries.CountryCode
		LEFT JOIN Mountains ON MountainsCountries.MountainId = Mountains.Id
		LEFT JOIN Peaks ON Mountains.Id = Peaks.MountainId)  AS RankingQuery) as FinalQuery
	WHERE [Rank] = 1
	ORDER BY Country ASC, [Highest Peak Name] ASC


--SELECT  TOP (5)
--			  Country,
--              ISNULL(PeakName, '(no highest peak)') AS HighestPeakName,
--	          ISNULL(Elevation, 0) AS HighestPeakElevation,
--	          ISNULL(MountainRange, '(no mountain)') AS MountainRange
--FROM (SELECT *,
--            DENSE_RANK() OVER (PARTITION BY Country ORDER BY Elevation DESC) AS PeakRank
--      FROM (SELECT CountryName AS Country,
--      		   p.PeakName,
--      		   p.Elevation,
--      		   m.MountainRange
--      	  FROM Countries AS c
--      	  LEFT OUTER JOIN MountainsCountries AS mc ON c.CountryCode = mc.CountryCode
--      	  LEFT OUTER JOIN Mountains AS m ON mc.MountainId = m.Id
--      	  LEFT OUTER JOIN Peaks AS p ON m.Id = p.MountainId
--      	 ) AS FullCountryMountainInfo
--	 ) AS PeaksRankingQuery
--WHERE PeakRank = 1
--ORDER BY Country ASC,
--		 HighestPeakName ASC

