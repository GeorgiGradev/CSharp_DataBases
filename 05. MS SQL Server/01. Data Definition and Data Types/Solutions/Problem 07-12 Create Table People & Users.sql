--  Problem 07.Create Table People--
USE Minions

 CREATE TABLE People 
 (
	 Id INT PRIMARY KEY IDENTITY NOT NULL,
	 [Name] NVARCHAR(200) NOT NULL,
	 Picture VARBINARY(MAX),
	 Height DECIMAL(3,2),
	 [Weight] DECIMAL(5,2),
	 Gender CHAR(1) NOT NULL,
	 Birthdate DATE NOT NULL,
	 Biography NVARCHAR(MAX)
 )
INSERT INTO People([Name],Height,[Weight],Gender,Birthdate,Biography)
	VALUES
		('Pesho1',1.50,80,'m','05.01.1999','This is some Biography'),
		('Maria1',1.80,59,'f','02.12.1990','This is some Biography'),
		('Pesho2',1.90,45.30,'m','05.21.2020','This is some Biography'),
		('Maria2',1.65,65,'f','05.21.2020','This is some Biography'),
		('Pesho3',1.70,45.30,'m','05.21.2020','This is some Biography')



--Problem 8.Create Table Users --
CREATE TABLE Users
(
	 Id BIGINT PRIMARY KEY IDENTITY NOT NULL,
	 Username VARCHAR(30) UNIQUE NOT NULL,
	 [Password] VARCHAR(26) NOT NULL,
	 ProfilePicture VARBINARY(MAX) 
	 CHECK(DATALENGTH(ProfilePicture) <= 900 * 1024),
	 LastLoginTime DateTime2 NOT NULL,
	 IsDeleted BIT NOT NULL
)

INSERT INTO Users(Username,[Password],LastLoginTime,IsDeleted)
	VALUES
		('Pesho1',123456,'05.21.2020',0),
		('Pesho2',123456,'05.21.2020',1),
		('Pesho3',123456,'05.21.2020',0),
		('Pesho4',123456,'05.21.2020',1),
		('Pesho5',123456,'05.21.2020',0)
		 
				--DELETE FROM Users WHERE Id = 5 ==> Изтриваме ред с Id 5
				--SET IDENTITY_INSERT Users ON ==> Включваме ръчно добавяне
				--INSERT INTO Users(Id, Username,[Password],LastLoginTime,IsDeleted)
				--	VALUES
				--			(5, 'Pesho55',123456,'05.21.2020',0) ==> Добавяме реда, като му задаваме изрично Id
				--SET IDENTITY_INSERT Users OFF ==> Изключваме ръчно добавяне

--Problem 9.Change Primary Key --
ALTER TABLE Users -- разрешаваме промяна в таблицата
DROP CONSTRAINT PK__Users__3214EC07EA54DA4D -- премахваме Primary Key от Id-то.

ALTER TABLE Users -- разрешаваме промяна в таблицата
ADD CONSTRAINT PK_Users_CompositeIdUsername 
PRIMARY KEY (Id, Username) -- добавя PRIMARY KEY за две полета



-- Problem 10.Add Check Constraint --
ALTER TABLE Users -- разрешаваме промяна в таблицата
ADD CONSTRAINT CK_Users_PasswordLenght -- добавяме ограничение
CHECK(LEN([Password]) >= 5) -- за дължина на паролата
 
  


 -- Problem 11.Set Default Value of a Field --
 ALTER TABLE Users
 ADD CONSTRAINT DF_Users_LastLoginTime
 DEFAULT GETDATE() FOR LastLoginTime 

 

 -- Problem 12.Set Unique Field -- 
 ALTER TABLE Users
 DROP CONSTRAINT PK_Users_CompositeIdUsername

 ALTER TABLE Users
 ADD CONSTRAINT [PK_Users_Id]
 PRIMARY KEY (Id)

 ALTER TABLE Users 
 ADD CONSTRAINT CK_Users_UsernameLenght
 CHECK(LEN(Username) >= 3)


