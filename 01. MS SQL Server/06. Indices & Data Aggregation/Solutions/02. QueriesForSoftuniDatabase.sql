USE SoftUni
SELECT * FROM Employees

--- 13.Departments Total Salaries ---
SELECT
	e.DepartmentID,
	SUM(e.Salary) AS TotalSalary
	FROM Employees as e
	GROUP BY e.DepartmentID
	ORDER BY DepartmentID ASC


--- 14.Employees Minimum Salaries ---
SELECT 
	e.DepartmentID,
	MIN(e.Salary) AS MinimumSalary 
	FROM Employees AS e
	WHERE e.DepartmentID IN (2,5,7) AND e.HireDate > '01.01.2000'
	GROUP BY e.DepartmentID


--- 15.Employees Average Salaries ---
SELECT * 
INTO EmployeesWithSalaryMoreThan30000
FROM Employees
WHERE Salary > 30000

DELETE FROM EmployeesWithSalaryMoreThan30000
WHERE ManagerID = 42

UPDATE EmployeesWithSalaryMoreThan30000
SET Salary += 5000
WHERE DepartmentID = 1

SELECT DepartmentID,
       AVG(Salary) AS AverageSalary
FROM EmployeesWithSalaryMoreThan30000
GROUP BY DepartmentID


--- 16. Employees Maximum Salaries
SELECT
	Employees.DepartmentID,
	MAX(Employees.Salary) AS MaxSalary
	FROM Employees
	GROUP BY Employees.DepartmentID
	HAVING MAX(Employees.Salary) NOT BETWEEN 30000 AND 70000 


--- 17.Employees Count Salaries ---
SELECT 
	COUNT(Employees.Salary)
	FROM Employees
	WHERE Employees.ManagerID IS NULL

--SELECT COUNT(*) AS Count FROM Employees
--WHERE ManagerID IS NULL


--- 18.*3rd Highest Salary ---
SELECT 
	DepartmentID,
	Salary AS ThirdHighestSalary
	FROM (SELECT 
		DepartmentID,
		Salary,
		DENSE_RANK() OVER (PARTITION BY DepartmentID ORDER BY Salary DESC) AS [Rank]
		FROM Employees
		GROUP BY DepartmentID, Salary
        ) AS RankSalariesInDepartments
	WHERE [Rank] = 3


--- 19.**Salary Challenge ---
SELECT TOP(10)
	e.FirstName,
	e.LastName,
	e.DepartmentID
	FROM Employees as e
	WHERE e.Salary > 
		(SELECT 
		AVG(emp.Salary)
		FROM Employees AS emp
		GROUP BY emp.DepartmentID
		HAVING e.DepartmentID = emp.DepartmentID)
	ORDER BY e.DepartmentID

	