CREATE DATABASE SoftUni
GO
USE SoftUni

CREATE TABLE Towns(
    Id INT IDENTITY, 
    Name NVARCHAR(50) NOT NULL, 
    CONSTRAINT PK_Towns PRIMARY KEY (Id)
)

CREATE TABLE Addresses(
    Id INT IDENTITY, 
    AddressText NVARCHAR(200) NOT NULL, 
    TownId INT NOT NULL, 
    CONSTRAINT PK_Addresses PRIMARY KEY (Id),
    CONSTRAINT FK_TownId FOREIGN KEY (TownId) REFERENCES Towns(Id)
)

CREATE TABLE Departments(
    Id INT IDENTITY, 
    Name NVARCHAR(100) NOT NULL
    CONSTRAINT PK_Departments PRIMARY KEY (Id)
)

CREATE TABLE Employees(
    Id INT IDENTITY, 
    FirstName NVARCHAR(20) NOT NULL, 
    MiddleName NVARCHAR(20) NOT NULL, 
    LastName NVARCHAR(20) NOT NULL,
    JobTitle NVARCHAR(50) NOT NULL, 
    DepartmentId INT NOT NULL,
    HireDate DATE NOT NULL,
    Salary DECIMAL(7,2) NOT NULL, 
    AddressId INT NOT NULL,
    CONSTRAINT PK_Employees PRIMARY KEY (Id), 
    CONSTRAINT FK_Department FOREIGN KEY (DepartmentId) REFERENCES Departments(Id), 
    CONSTRAINT FK_Address FOREIGN KEY (AddressId) REFERENCES Addresses(Id)
)

ALTER TABLE Employees
ADD CONSTRAINT CHK_Salary CHECK (Salary > 0)