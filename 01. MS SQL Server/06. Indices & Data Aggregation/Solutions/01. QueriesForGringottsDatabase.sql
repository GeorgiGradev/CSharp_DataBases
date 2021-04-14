USE Gringotts

--- 1.Records’ Count ---
SELECT
	COUNT(*) 
	FROM WizzardDeposits

--- 2. Longest Magic Wand ---
SELECT 
	MAX(WizzardDeposits.MagicWandSize) AS LongestMagicWand
	FROM WizzardDeposits

--SELECT TOP (1)
--	WizzardDeposits.MagicWandSize AS LongestMagicWand
--	FROM WizzardDeposits
--	ORDER BY MagicWandSize DESC

--SELECT
--	qwerty.test
--	FROM (SELECT
--		WizzardDeposits.MagicWandSize AS test,
--		ROW_NUMBER () OVER (ORDER BY MagicWandSize DESC) AS [Rank]
--		FROM WizzardDeposits) AS qwerty
--	WHERE [Rank] = 1
	

--- 3.Longest Magic Wand Per Deposit Groups ---
SELECT
	WizzardDeposits.DepositGroup,
	MAX(WizzardDeposits.MagicWandSize) as LongestMagicWand
	FROM WizzardDeposits
	GROUP BY WizzardDeposits.DepositGroup


--- 4. Smallest Deposit Group Per Magic Wand Size ---
SELECT TOP(2)
	DepositGroup
	FROM (SELECT 
		DepositGroup,
		AVG(MagicWandSize) AS AverageSize
		FROM WizzardDeposits
		GROUP BY DepositGroup) AS AverageQuery
   ORDER BY AverageSize

--SELECT TOP(2) 
--	DepositGroup 
--	FROM WizzardDeposits
--	GROUP BY DepositGroup
--	ORDER BY AVG(MagicWandSize) ASC


--- 5.Deposits Sum ---
SELECT
	DepositGroup,
	SUM(DepositAmount) AS TotalSum
	FROM WizzardDeposits
	GROUP BY DepositGroup


--- 6.Deposits Sum for Ollivander Family ---
SELECT 
	DepositGroup,
	SUM(DepositAmount) AS TotalSum
	FROM WizzardDeposits
	GROUP BY DepositGroup, MagicWandCreator
	HAVING MagicWandCreator = 'Ollivander family'


--SELECT DepositGroup, SUM(DepositAmount) AS TotalSum	 FROM WizzardDeposits
--	WHERE MagicWandCreator = 'Ollivander family'
--	GROUP BY DepositGroup


--- 7.Deposits Filter ---
SELECT 
	DepositGroup,
	SUM(DepositAmount) AS TotalSum
	FROM WizzardDeposits
	GROUP BY DepositGroup, MagicWandCreator
	HAVING MagicWandCreator = 'Ollivander family' AND SUM(DepositAmount) < 150000
	ORDER BY SUM(DepositAmount) DESC

--SELECT * 
--FROM (SELECT 
	--DepositGroup, 
	--SUM(DepositAmount) AS TotalSum 
	--FROM WizzardDeposits
	--WHERE MagicWandCreator = 'Ollivander family'
	--GROUP BY DepositGroup) AS DepositGroupsTotalSum
--WHERE TotalSum < 150000
--ORDER BY TotalSum DESC


--- 8.Deposit Charge ---
SELECT
	WizzardDeposits.DepositGroup,
	WizzardDeposits.MagicWandCreator,
	MIN(WizzardDeposits.DepositCharge) AS MinDepositCharge
	FROM WizzardDeposits
	GROUP BY DepositGroup, MagicWandCreator
	ORDER BY MagicWandCreator, DepositGroup


--SELECT DepositGroup, 
--       MagicWandCreator, 
--       MIN(DepositCharge) AS MinDepositCharge 
--FROM WizzardDeposits
--GROUP BY DepositGroup, MagicWandCreator


--- 9.Age Groups ---
SELECT 
	AgeGroup,
	COUNT (*) AS WizzardCount
	FROM (SELECT
		CASE 
			WHEN Age BETWEEN 0 AND 10 THEN '[0-10]'
         			WHEN Age BETWEEN 11 AND 20 THEN '[11-20]'
         			WHEN Age BETWEEN 21 AND 30 THEN '[21-30]'
         			WHEN Age BETWEEN 31 AND 40 THEN '[31-40]'
         			WHEN Age BETWEEN 41 AND 50 THEN '[41-50]'
         			WHEN Age BETWEEN 51 AND 60 THEN '[51-60]'
         			ELSE '[61+]'
         		END AS AgeGroup
				FROM WizzardDeposits) AS WizzardAge
	GROUP BY AgeGroup

--SELECT
--	CASE 
--		WHEN Age BETWEEN 0 AND 10 THEN '[0-10]'
--         			WHEN Age BETWEEN 11 AND 20 THEN '[11-20]'
--         			WHEN Age BETWEEN 21 AND 30 THEN '[21-30]'
--         			WHEN Age BETWEEN 31 AND 40 THEN '[31-40]'
--         			WHEN Age BETWEEN 41 AND 50 THEN '[41-50]'
--         			WHEN Age BETWEEN 51 AND 60 THEN '[51-60]'
--         			ELSE '[61+]'
--        END AS AgeGroup,
--	COUNT (*) AS WizzardCount
--	FROM WizzardDeposits
--	GROUP BY (CASE 
--		WHEN Age BETWEEN 0 AND 10 THEN '[0-10]'
--         			WHEN Age BETWEEN 11 AND 20 THEN '[11-20]'
--         			WHEN Age BETWEEN 21 AND 30 THEN '[21-30]'
--         			WHEN Age BETWEEN 31 AND 40 THEN '[31-40]'
--         			WHEN Age BETWEEN 41 AND 50 THEN '[41-50]'
--         			WHEN Age BETWEEN 51 AND 60 THEN '[51-60]'
--         			ELSE '[61+]'
--      END)


--- 10.First Letter ---
SELECT 
	SUBSTRING(wd.FirstName,1,1) as FirstLetter
	FROM WizzardDeposits as wd
	WHERE wd.DepositGroup = 'Troll Chest'
	GROUP BY SUBSTRING(wd.FirstName,1,1)

--SELECT 
--	LEFT(FirstName, 1) AS FirstLetter 
--	FROM WizzardDeposits
--	WHERE DepositGroup = 'Troll Chest'
--	GROUP BY LEFT(FirstName, 1)
--	ORDER BY FirstLetter ASC


--- 11.Average Interest ---
SELECT 
		wd.DepositGroup, 
		wd.IsDepositExpired,
		AVG(wd.DepositInterest) AS AverageInterest 
		FROM WizzardDeposits AS wd
		WHERE wd.DepositStartDate > '01-01-1985'
		GROUP BY wd.DepositGroup, wd.IsDepositExpired
		ORDER BY wd.DepositGroup DESC


--- 12.* Rich Wizard, Poor Wizard ---
SELECT 
	SUM(HostWizardDeposit - GuestWizardDeposit) AS SumDifference
	FROM (SELECT 
		DepositAmount AS HostWizardDeposit,
        LEAD(DepositAmount) OVER (ORDER BY Id) AS GuestWizardDeposit
        FROM WizzardDeposits) AS WizzardsDiff

--SELECT 
--	SUM(host.DepositAmount - guest.DepositAmount) AS SumDifference
--	FROM WizzardDeposits AS host
--	JOIN WizzardDeposits AS guest ON guest.Id = host.Id + 1