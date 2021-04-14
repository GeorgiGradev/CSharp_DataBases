USE Geography

SELECT Mountains.MountainRange, Peaks.PeakName, Peaks.Elevation
	FROM Peaks
	JOIN Mountains ON Peaks.MountainId = Mountains.Id
	WHERE Mountains.MountainRange = 'Rila'
	ORDER BY Elevation DESC

