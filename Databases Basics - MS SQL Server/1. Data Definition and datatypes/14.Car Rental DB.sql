CREATE DATABASE CarRental
GO
USE CarRental

CREATE TABLE Categories(
Id BIGINT IDENTITY(1,1) PRIMARY KEY,
CategoryName VARCHAR(60) NOT NULL,
DailyRate DECIMAL NOT NULL,
WeeklyRate DECIMAL NOT NULL,
MonthlyRate DECIMAL NOT NULL,
WeekendRate DECIMAL NOT NULL
)

INSERT INTO Categories VALUES
('cat',3,6,9,13),
('Lee',4,5,6,7.3),
('Alby',2,3,4.5,6)

CREATE TABLE Cars(
Id BIGINT IDENTITY(1,1) PRIMARY KEY,
PlateNumber NVARCHAR(10) UNIQUE NOT NULL,
Manufacturer NVARCHAR(100) NOT NULL,
Model NVARCHAR(100) NOT NULL,
CarYear INT,
CategoryId BIGINT NOT NULL,
Doors INT,
Picture VARBINARY,
Condition VARCHAR(20),
Available VARCHAR(10) NOT NULL
CONSTRAINT FK_Category FOREIGN KEY (CategoryId) REFERENCES Categories (Id)
)

INSERT INTO Cars VALUES
('Fantasy','BestGenre','Heisenberg',2000,2,2,NULL,'good','yes'),
('Mystery','manu','model',1996,1,3,NULL,'excellent','no'),
('Sci-fi','Honda','Civic',2019,3,1,NULL,'amazing','yes')

CREATE TABLE Employees(
Id BIGINT IDENTITY(1,1) PRIMARY KEY,
FirstName VARCHAR(100) NOT NULL,
LastName VARCHAR(100) NOT NULL,
Title VARCHAR(100),
Notes VARCHAR(MAX)
)

INSERT INTO Employees VALUES
('Category','cat',NULL,NULL),
('Dull','Hog',NULL,NULL),
('Amazing','Boss',NULL,NULL)

CREATE TABLE Customers(
Id BIGINT IDENTITY(1,1) PRIMARY KEY,
DriverLicenceNumber VARCHAR(100) NOT NULL,
FullName VARCHAR(100) NOT NULL,
Address VARCHAR(300) NOT NULL,
ZIPCode VARCHAR(20) NOT NULL,
Notes VARCHAR(MAX)
)

INSERT INTO Customers VALUES
('Title','Hoss','Santana 22',4000,NULL),
('PB3000EU','Hoss Delgato','Plovdiv 34',2000,NULL),
('Batman','Jesus','Earth',2004,NULL)

CREATE TABLE RentalOrders(
Id BIGINT IDENTITY(1,1) PRIMARY KEY NOT NULL,
EmployeeId BIGINT NOT NULL,
CustomerId BIGINT NOT NULL,
CarId BIGINT NOT NULL,
TankLevel INT,
KilometrageStart BIGINT,
KilometrageEnd BIGINT,
TotalKilometrage BIGINT,
StartDate DATE,
EndDate DATE,
TotalDays INT,
RateApplied DECIMAL,
TaxRate DECIMAL,
OrderStatus VARCHAR(50),
Notes VARCHAR(MAX)
CONSTRAINT FK_Customer FOREIGN KEY (CustomerId) REFERENCES Customers (Id),
CONSTRAINT FK_Employee FOREIGN KEY (EmployeeId) REFERENCES Employees (Id),
CONSTRAINT FK_Car FOREIGN KEY (CarId) REFERENCES Cars (Id) 
)

INSERT INTO RentalOrders VALUES
(2,2,2,2,2,2,2,NULL,NULL,30,3.4,20,'completed',NULL),
(1,2,3,2,30,60,30,NULL,NULL,33,3.3,2,'pending',NULL),
(1,3,2,2,30,60,30,NULL,NULL,43,2.3,3,'pending',NULL)