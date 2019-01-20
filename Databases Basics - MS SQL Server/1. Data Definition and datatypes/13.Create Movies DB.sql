CREATE DATABASE Movies
GO
USE Movies

CREATE TABLE Directors(
Id BIGINT IDENTITY(1,1) PRIMARY KEY,
DirectorName VARCHAR(100) NOT NULL,
Notes VARCHAR(MAX)
)

INSERT INTO Directors VALUES
('Tarantino','God'),
('Lee',NULL),
('Alby',NULL),
('I Dont know any other directors','Sorry'),
('Hi',NULL)

CREATE TABLE Genres(
Id BIGINT IDENTITY(1,1) PRIMARY KEY,
GenreName VARCHAR(100) NOT NULL,
Notes VARCHAR(MAX)
)

INSERT INTO Genres VALUES
('Fantasy','BestGenre'),
('Mystery',NULL),
('Sci-fi',NULL),
('Bottle movie',NULL),
('Action',NULL)

CREATE TABLE Categories(
Id BIGINT IDENTITY(1,1) PRIMARY KEY,
CategoryName VARCHAR(100) NOT NULL,
Notes VARCHAR(MAX)
)

INSERT INTO Categories VALUES
('Category',NULL),
('Dull',NULL),
('Amazing',NULL),
('Idk','Sorry'),
('Silly',NULL)

CREATE TABLE Movies(
Id BIGINT IDENTITY(1,1) PRIMARY KEY,
Title VARCHAR(100) NOT NULL,
DirectorId BIGINT NOT NULL,
CopyrightYear INT,
Length INT,
GenreId BIGINT NOT NULL,
CategoryId BIGINT NOT NULL,
Rating INT,
Notes VARCHAR(MAX)
    CONSTRAINT FK_Director FOREIGN KEY (DirectorId) REFERENCES Directors (Id), 
    CONSTRAINT FK_Genre FOREIGN KEY (GenreId) REFERENCES Genres (Id), 
    CONSTRAINT FK_Category FOREIGN KEY (CategoryId) REFERENCES Categories (Id), 
    CONSTRAINT CHK_Movie_Length CHECK ([Length] > 0),
    CONSTRAINT CHK_Rating_Value CHECK (Rating <= 10)
)

INSERT INTO Movies VALUES
('Title',2,1995,56,3,2,6,NULL),
('Title 2 - Sequel',3,2000,98,2,1,9,NULL),
('Batman',1,2004,89,1,1,5,NULL),
('I Dont know',2,2004,77,2,2,3,'Sorry'),
('Hateful eight',5,2015,160,1,1,9,NULL)