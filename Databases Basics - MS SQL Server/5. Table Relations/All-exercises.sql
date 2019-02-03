--1. One-To-One Relationship
CREATE TABLE Passports(
PassportID INT IDENTITY (101,1) PRIMARY KEY NOT NULL,
PassportNumber CHAR(9) NOT NULL UNIQUE
)

CREATE TABLE Persons(
PersonID INT IDENTITY PRIMARY KEY,
FirstName VARCHAR(50) NOT NULL,
Salary INT NOT NULL,
PassportID INT NOT NULL
CONSTRAINT FK_Persons_Passports
FOREIGN KEY (PassportID)
REFERENCES Passports(PassportID)
)

INSERT INTO Passports VALUES
('N34FG21B'),
('K65LO4R7'),
('ZE657QP2')

INSERT INTO Persons VALUES
('Roberto',43300,102),
('Tom',56100,103),
('Yana',60200,101)

--2.One-To-Many Relationship
CREATE TABLE Manufacturers(
ManufacturerID INT IDENTITY(1,1) PRIMARY KEY,
Name VARCHAR(30) NOT NULL UNIQUE,
EstablishedOn DATE NOT NULL
)

CREATE TABLE Models(
ModelID INT IDENTITY(101,1) PRIMARY KEY,
Name VARCHAR(50) NOT NULL,
ManufacturerID INT NOT NULL
CONSTRAINT FK_Models_Manufacturers FOREIGN KEY (ManufacturerID)
REFERENCES Manufacturers(ManufacturerID)
)

INSERT INTO Manufacturers VALUES
('BMW',CONVERT(datetime, '07-03-1916', 103)),
('Tesla',CONVERT(datetime, '01-01-2003', 103)),
('Lada',CONVERT(datetime, '01-05-1966', 103))

INSERT INTO Models VALUES
('X1',1),
('i6',1),
('Model S',2),
('Model X',2),
('Model 3',2),
('Nova',3)

--3. Many-To-Many Relationship
CREATE TABLE Students(
StudentID INT IDENTITY(1,1) PRIMARY KEY,
Name VARCHAR(30) NOT NULL
)

CREATE TABLE Exams(
ExamID INT IDENTITY(101,1) PRIMARY KEY,
Name VARCHAR(100) NOT NULL
)


CREATE TABLE StudentsExams(
StudentID INT NOT NULL,
ExamID INT NOT NULL,

CONSTRAINT PK_Students_Exams
PRIMARY KEY(StudentID,ExamID),

CONSTRAINT FK_StudentsExams_Students 
FOREIGN KEY(StudentID) REFERENCES Students(StudentID),

CONSTRAINT FK_StudentsExams_Exams
FOREIGN KEY(ExamID) REFERENCES Exams(ExamID)
)

INSERT INTO Students VALUES
('Mila'),
('Toni'),
('Ron')

INSERT INTO Exams VALUES
('SpringMVC'),
('Neo4j'),
('Oracle 11g')

INSERT INTO StudentsExams VALUES
(1,101),
(1,102),
(2,101),
(3,103),
(2,102),
(2,103)

--4. Self-Referencing
CREATE TABLE Teachers(
TeacherID INT IDENTITY(101,1) PRIMARY KEY,
Name VARCHAR(50) NOT NULL,
ManagerID INT

CONSTRAINT FK_Teacher_Manager FOREIGN KEY(ManagerID)
REFERENCES Teachers(TeacherID)
)

INSERT INTO Teachers VALUES
('John',NULL),
('Maya',106),
('Silvia',106),
('Ted',105),
('Mark',101),
('Greta',101)

--5. Online Store Database
CREATE TABLE Cities(
CityID INT IDENTITY(1,1) PRIMARY KEY,
Name VARCHAR(50) NOT NULL
)

CREATE TABLE Customers(
CustomerID INT IDENTITY(1,1) PRIMARY KEY,
Name VARCHAR(50) NOT NULL,
Birthday DATE,
CityID INT NOT NULL

CONSTRAINT FK_Customer_Town FOREIGN KEY(CityID)
REFERENCES Cities(CityID)
)

CREATE TABLE ItemTypes(
ItemTypeID INT IDENTITY PRIMARY KEY,
Name VARCHAR(50)
)

CREATE TABLE Items(
ItemID INT IDENTITY PRIMARY KEY,
Name VARCHAR(50),
ItemTypeID INT NOT NULL

CONSTRAINT FK_Item_ItemType FOREIGN KEY(ItemTypeID)
REFERENCES ItemTypes(ItemTypeID)
)

CREATE TABLE Orders(
OrderID INT Identity PRIMARY KEY,
CustomerID INT NOT NULL

CONSTRAINT FK_Order_Customer FOREIGN KEY(CustomerID)
REFERENCES Customers(CustomerID)
)

CREATE TABLE OrderItems(
OrderID INT NOT NULL,
ItemID INT NOT NULL

CONSTRAINT PK_Order_Items PRIMARY KEY (OrderID,ItemID),

CONSTRAINT FK_OrderItems_Order FOREIGN KEY(OrderID)
REFERENCES Orders(OrderID),

CONSTRAINT FK_OrderItems_Item FOREIGN KEY(ItemID)
REFERENCES Items(ItemID)
)

--6. University Database
CREATE TABLE Majors(
MajorID INT IDENTITY PRIMARY KEY,
Name VARCHAR(50) NOT NULL
)

CREATE TABLE Students(
StudentId INT IDENTITY PRIMARY KEY,
StudentNumber INT NOT NULL,
StudentName VARCHAR(100) NOT NULL,
MajorID INT NOT NULL,

CONSTRAINT FK_Student_Major FOREIGN KEY(MajorID)
REFERENCES Majors(MajorID)
)

CREATE TABLE Payments(
PaymentID INT IDENTITY PRIMARY KEY,
PaymentDate DATE NOT NULL,
PaymentAmount DECIMAL NOT NULL,
StudentID INT NOT NULL,

CONSTRAINT FK_Payment_Student FOREIGN KEY(StudentID)
REFERENCES Students(StudentID)
)

CREATE TABLE Subjects(
SubjectID INT IDENTITY PRIMARY KEY,
SubjectName VARCHAR(50) NOT NULL
)

CREATE TABLE Agenda(
StudentID INT NOT NULL,
SubjectID INT NOT NULL,

CONSTRAINT PK_Student_Subject PRIMARY KEY(StudentID,SubjectID),

CONSTRAINT FK_Agenda_Student FOREIGN KEY(StudentID)
REFERENCES Students(StudentID),

CONSTRAINT FK_Agenda_Subject FOREIGN KEY(SubjectID)
REFERENCES Subjects(SubjectID)
)

--9. Peaks in Rila*
SELECT MountainRange,PeakName,Elevation FROM Mountains
JOIN Peaks ON
MountainRange= 'Rila'  AND Peaks.MountainId=Mountains.Id
ORDER BY Elevation DESC