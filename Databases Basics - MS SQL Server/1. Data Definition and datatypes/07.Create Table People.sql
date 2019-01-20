CREATE TABLE People(
Id INT IDENTITY(1,1),
Name NVARCHAR(200) NOT NULL,
Picture VARBINARY(MAX),
Height DECIMAL(3,2),
Weight DECIMAL(5,2),
Gender CHAR(1) NOT NULL,
Birthdate DATE NOT NULL,
Biography NVARCHAR(MAX),
PRIMARY KEY(Id)
)

INSERT INTO People VALUES
('Victoria',NULL,1.70,80,'m','1999/12/12','The greatest!'),
('Stoqn', NULL, 3, 100, 'm', '2000/12/31', NULL),
('Gosho', NULL, 2.19, 100, 'f', '1980/02/10', 'hello world'),
('Pesho', NULL, 1.80, 95.03, 'f', '1993/03/17', '011001010101'),
('Chushki', NULL, 3.90, 15.4, 'm', '1970/12/31', NULL)