USE Geography

--- Problem 12.	Countries Holding ‘A’ 3 or More Times ---
SELECT 
	[CountryName] AS [Country Name]
	,[ISOCode] AS [ISO Code]
	FROM Countries 
	WHERE CountryName LIKE '%A%A%A%'
	ORDER BY IsoCode 


--- Problem 13.Mix of Peak and River Names ---
SELECT
	[PeakName]
	,[RiverName],
	LOWER(CONCAT(Peaks.PeakName, SUBSTRING(Rivers.RiverName, 2, LEN(Rivers.RiverName) - 1))) AS [Mix]
	FROM [Peaks], [Rivers]
	WHERE RIGHT(Peaks.PeakName, 1) = LEFT(Rivers.RiverName,1)
	ORDER BY [Mix]
--SELECT
--	[PeakName]
--	,[RiverName],
--	LOWER(CONCAT(Peaks.PeakName, SUBSTRING(Rivers.RiverName, 2, LEN(Rivers.RiverName) - 1))) AS [Mix]
--	FROM [Peaks]
--	JOIN [Rivers] ON RIGHT(Peaks.PeakName, 1) = LEFT(Rivers.RiverName,1)
--	ORDER BY [Mix]

