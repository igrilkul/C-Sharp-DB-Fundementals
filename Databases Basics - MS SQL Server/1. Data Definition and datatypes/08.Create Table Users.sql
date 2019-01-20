CREATE TABLE Users(
Id BIGINT IDENTITY(1,1),
Username VARCHAR(30) NOT NULL,
Password VARCHAR(26) NOT NULL,
ProfilePicture VARBINARY(MAX),
LastLoginTime DATETIME,
IsDeleted BIT DEFAULT(0)
CONSTRAINT PK_User_Id PRIMARY KEY (Id),
CONSTRAINT UQ_Username UNIQUE(Username),
)

INSERT INTO Users VALUES
('Victoria','0000',NULL,NULL,0),
('Stoqn','zero',NULL,NULL,1),
('Gosho','Oneeez',NULL,NULL,0),
('Pesho','Password',NULL, NULL,1),
('Chushki','Hohoho',NULL, NULL,0)