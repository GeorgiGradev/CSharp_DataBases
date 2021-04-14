USE Bitbucket

---02.Insert---
INSERT INTO Files (Name, Size, ParentId, CommitId)
	VALUES
	('Trade.idk', 2598.0, 1, 1),
	('menu.net', 9238.31, 2, 2),
	('Administrate.soshy', 1246.93,	3,	3),
	('Controller.php', 7353.15,	4,	4),
	('Find.java', 9957.86,	5,	5),
	('Controller.json',	14034.87, 3,	6),
	('Operate.xix',	7662.92, 7,	7)

INSERT INTO Issues(Title, IssueStatus, RepositoryId, AssigneeId)
	VALUES 
	('Critical Problem with HomeController.cs file', 'open',1,4),
	('Typo fix in Judge.html', 'open',	4,	3),
	('Implement documentation for UsersService.cs',	'closed',8,	2),
	('Unreachable code in Index.cs', 'open', 9,	8)


---03.Update---
UPDATE
	Issues
	SET IssueStatus = 'Closed'
	WHERE AssigneeId = 6


---04.Delete---
DELETE
	FROM Issues
	WHERE RepositoryId = (SELECT Id FROM Repositories WHERE Name = 'Softuni-Teamwork')

DELETE 
	FROM RepositoriesContributors
	WHERE RepositoryId = (SELECT Id FROM Repositories WHERE Name = 'Softuni-Teamwork')


---05.Commits---
SELECT 
	Id, 
	Message,
	RepositoryId,
	ContributorId
	FROM Commits
	ORDER BY Id, Message, RepositoryId, ContributorId
	

---06.Front-end---
SELECT
	Id,
	Name,
	Size
	FROM Files
	WHERE Size > 1000 AND NAME LIKE '%html'
	ORDER BY Size DESC, Id, Name


---07.Issue Assignment---
SELECT
	i.Id,
	CONCAT(u.Username, ' : ', i.Title)
	FROM Issues AS i
	JOIN Users AS u ON i.AssigneeId = u.Id
	ORDER BY i.Id DESC, AssigneeId


---08.Single Files---
SELECT 
	f1.Id,
    f1.Name,
	CONCAT(f1.Size, 'KB') as Size
	FROM Files as f1
	LEFT JOIN Files as f2 ON f1.Id = f2.ParentId
	WHERE f2.ParentId IS NULL
	ORDER BY f1.Id ASC, f1.Name ASC, [Size] DESC


---9.Commits in Repositories---
SELECT TOP(5)
	r.Id,
    r.Name,
    COUNT(*) AS Commits
	FROM Repositories AS r 
	JOIN Commits AS c ON r.Id = c.RepositoryId
    JOIN RepositoriesContributors AS rc ON r.Id = rc.RepositoryId
	GROUP BY r.Id, r.Name
	ORDER BY Commits DESC, r.Id, r.Name


---10.Average Size---
SELECT 
	u.Username,
	AVG(f.Size) as Size
	FROM Users AS u
	JOIN Commits AS c ON u.Id = c.ContributorId
	JOIN Files AS f ON c.Id = f.CommitId
	GROUP BY u.Username
	ORDER BY Size DESC, u.Username


---11.All User Commits---
GO
CREATE FUNCTION udf_AllUserCommits(@username VARCHAR(50)) 
RETURNS INT
AS
BEGIN
	
RETURN (SELECT
			COUNT(c.Id)
			FROM Users AS u
			JOIN Commits AS c ON u.Id = c.ContributorId
			WHERE u.Username = @username)
END
GO

SELECT dbo.udf_AllUserCommits('UnderSinduxrein')

---12.Search for Files---
GO
CREATE PROCEDURE usp_SearchForFiles(@fileExtension VARCHAR(20))
AS
	SELECT 
		Id,
		Name,
		CONCAT(Size, 'KB')
		FROM Files
		WHERE Name LIKE CONCAT('%.',@fileExtension)


EXEC usp_SearchForFiles 'txt'