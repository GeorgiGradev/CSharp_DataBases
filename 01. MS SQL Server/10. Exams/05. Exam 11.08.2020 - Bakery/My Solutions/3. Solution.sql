USE Bakery


---02.Insert---
INSERT INTO Distributors (Name, CountryId, AddressText, Summary)
	VALUES
('Deloitte & Touche', 2, '6 Arch St #9757',	'Customizable neutral traveling'),
('Congress Title', 13, '58 Hancock St',	'Customer loyalty'),
('Kitchen People', 1, '3 E 31st St #77', 'Triple-buffered stable delivery'),
('General Color Co Inc', 21, '6185 Bohn St #72', 'Focus group'),
('Beck Corporation',23,	'21 E 64th Ave', 'Quality-focused 4th generation hardware')

	
INSERT INTO Customers (FirstName, LastName, Age, Gender, PhoneNumber, CountryId)
	VALUES
('Francoise', 'Rautenstrauch',15,'M','0195698399',5),
('Kendra', 'Loud',22,'F','0063631526',11),
('Lourdes' ,'Bauswell',50,'M','0139037043',8),
('Hannah' ,'Edmison',18,'F','0043343686',1),
('Tom', 'Loeza'	,31,'M','0144876096',23),
('Queenie', 'Kramarczyk',30,'F','0064215793',29),
('Hiu' ,'Portaro',25,'M','0068277755',16),
('Josefa', 'Opitz',43,'F','0197887645',17)

---03.Update---
UPDATE
	Ingredients
	SET DistributorId = 35
	WHERE Name IN ('Bay Leaf', 'Paprika', 'Poppy')

UPDATE 
	Ingredients
	SET OriginCountryId = 14
	WHERE OriginCountryId = 8

---04.Delete---
DELETE
	FROM Feedbacks
	WHERE CustomerId = 14
	
DELETE
	FROM Feedbacks
	WHERE ProductId = 5


---05.Products by Price---
SELECT 
	Name,
	Price,
	Description
	FROM Products
	ORDER BY Price DESC, Name 


---06.Negative Feedback---
SELECT 
	f.ProductId,
	f.Rate,
	f.Description,
	c.Id,
	c.Age,
	c.Gender
	FROM Feedbacks AS f
	JOIN Customers AS c ON f.CustomerId = c.Id
	WHERE f.Rate < 5
	ORDER BY ProductId DESC, Rate


---07.Customers without Feedback---
SELECT
	CONCAT(c.FirstName, ' ', c.LastName),
	c.PhoneNumber,
	c.Gender
	FROM Customers AS c
	LEFT JOIN Feedbacks AS f ON c.Id = f.CustomerId
	WHERE f.Id IS NULL
	ORDER BY c.Id


---08.Customers by Criteria---
SELECT 
	cu.FirstName, 
	cu.Age, 
	cu.PhoneNumber
	FROM Customers AS cu
	JOIN Countries AS co ON cu.CountryId = co.Id
	WHERE (cu.Age >= 21 AND cu.FirstName LIKE '%an%')
		OR (cu.PhoneNumber LIKE '%38' AND co.Name <> 'Greece')
	ORDER BY cu.FirstName, Age DESC


---09.Middle Range Distributors---
SELECT
	DistributorName,
	IngredientName,
	ProductName, 
	AverageRate
	FROM (SELECT 
		d.Name AS DistributorName,
		i.Name AS IngredientName,
		p.Name AS ProductName,
		AVG(f.Rate) AS AverageRate
		FROM Distributors AS d
		JOIN Ingredients AS i ON d.Id = i.DistributorId
		JOIN ProductsIngredients AS pi ON i.Id = pi.IngredientId
		JOIN Products AS p ON pi.ProductId = p.Id
		JOIN Feedbacks AS f ON p.Id = f.ProductId
		GROUP BY d.Name, i.Name, p.Name) as temp
	WHERE AverageRate BETWEEN 5 AND 8
	ORDER BY DistributorName,
	IngredientName,
	ProductName


---10.Country Representative---
SELECT
	CountryName,
	DistributorName
	FROM (SELECT 
		c.Name AS CountryName,
		d.Name AS DistributorName,
		COUNT(i.Id) AS Count, 
		DENSE_RANK() OVER (PARTITION BY c.Name ORDER BY COUNT(i.Id) DESC) AS [Rank]
		FROM Countries AS c
		JOIN Distributors AS d ON c.Id = d.CountryId 
		LEFT JOIN Ingredients AS i ON d.Id = i.DistributorId
		GROUP BY c.Name, d.Name) as temp
	WHERE Rank = 1
	ORDER BY CountryName, DistributorName


---11.Customers with Countries---
GO
CREATE VIEW v_UserWithCountries AS
SELECT 
	CONCAT(cu.FirstName, ' ', cu.LastName) AS CustomerName,
	cu.Age,
	cu.Gender,
	co.Name
	FROM Customers as cu
	JOIN Countries as co ON cu.CountryId = co.Id
GO


SELECT TOP 5 *
  FROM v_UserWithCountries
 ORDER BY Age


 
---12. Delete Products---
GO
CREATE TRIGGER tr_DeleteProducts
    ON Products
    INSTEAD OF DELETE
AS
  BEGIN
	DELETE
	FROM Feedbacks
	WHERE ProductId IN 
	(SELECT P.Id FROM Products AS P
	JOIN deleted AS D
		ON P.Id = D.Id)

	DELETE FROM ProductsIngredients
	WHERE ProductId IN 
	(SELECT P.Id FROM Products AS P
	JOIN deleted AS D
		ON P.Id = D.Id)

	DELETE FROM Products
	WHERE Products.Id  IN 
	(SELECT P.Id FROM Products AS P
	JOIN deleted AS D
		ON P.Id = D.Id)
  END

--Delete from Products
--where Id = 30

--DELETE FROM Feedbacks
--WHERE ProductId = 5