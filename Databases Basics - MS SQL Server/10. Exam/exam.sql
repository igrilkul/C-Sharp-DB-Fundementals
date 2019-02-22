--1. DDL
CREATE DATABASE School
USE School

CREATE TABLE Students(
Id INT IDENTITY PRIMARY KEY,
FirstName NVARCHAR(30) NOT NULL,
MiddleName NVARCHAR(25),
LastName NVARCHAR(30) NOT NULL,
Age INT CHECK(Age>=5 AND Age<=100),
Address NVARCHAR(50),
Phone NCHAR(10))

CREATE TABLE Subjects(
Id INT IDENTITY PRIMARY KEY,
Name NVARCHAR(20) NOT NULL,
Lessons INT NOT NULL CHECK(Lessons>0))

CREATE TABLE StudentsSubjects(
Id INT IDENTITY PRIMARY KEY,
StudentId INT NOT NULL FOREIGN KEY REFERENCES Students(Id),
SubjectId INT NOT NULL FOREIGN KEY REFERENCES Subjects(Id),
Grade DECIMAL(15,2) NOT NULL CHECK(Grade>=2 AND Grade<=6))

CREATE TABLE Exams(
Id INT IDENTITY PRIMARY KEY,
Date DATETIME,
SubjectId INT NOT NULL FOREIGN KEY REFERENCES Subjects(Id))

CREATE TABLE StudentsExams(
StudentId INT NOT NULL FOREIGN KEY REFERENCES Students(Id),
ExamId INT NOT NULL FOREIGN KEY REFERENCES Exams(Id),
Grade DECIMAL(15,2) NOT NULL CHECK(Grade>=2 AND Grade<=6)
PRIMARY KEY(StudentId,ExamId))

CREATE TABLE Teachers(
Id INT IDENTITY PRIMARY KEY,
FirstName NVARCHAR(20) NOT NULL,
LastName NVARCHAR(20) NOT NULL,
Address NVARCHAR(20) NOT NULL,
Phone CHAR(10),
SubjectId INT NOT NULL FOREIGN KEY REFERENCES Subjects(Id))

CREATE TABLE StudentsTeachers(
StudentId INT NOT NULL FOREIGN KEY REFERENCES Students(Id),
TeacherId INT NOT NULL FOREIGN KEY REFERENCES Teachers(Id)
PRIMARY KEY(StudentId,TeacherId))


--2.
INSERT INTO Teachers VALUES
('Ruthanne','Bamb','84948 Mesta Junction','3105500146',6),
('Gerrard','Lowin','370 Talisman Plaza','3324874824',2),
('Merrile','Lambdin','81 Dahle Plaza','4373065154',5),
('Bert','Ivie','2 Gateway Circle','4409584510',4)

INSERT INTO Subjects VALUES
('Geometry',12),
('Health',10),
('Drama',7),
('Sports',9)

--Update
UPDATE StudentsSubjects
SET Grade=6
WHERE SubjectId IN(1,2) AND Grade>=5.50

--DELETE
DELETE FROM StudentsTeachers
WHERE TeacherId IN (SELECT Id FROM Teachers WHERE Phone LIKE('%72%'))

DELETE FROM Teachers
WHERE Phone LIKE ('%72%')

--3. Querying
SELECT FirstName,LastName,Age FROM Students
WHERE Age>=12
ORDER BY FirstName,LastName

--Cool Addresses
SELECT FirstName+' '+ISNULL(MiddleName,'')+' '+LastName AS 'Full Name',Address FROM Students
WHERE Address LIKE ('%road%')
ORDER BY FirstName,LastName,Address

--42 Phones
SELECT FirstName,Address,Phone FROM Students
WHERE MiddleName IS NOT NULL AND Phone LIKE ('42%')
ORDER BY FirstName

--Students Teachers
SELECT FirstName,LastName,COUNT(st.TeacherId) AS [TeachersCount] FROM Students AS s
JOIN StudentsTeachers AS st ON s.Id=st.StudentId
GROUP BY s.FirstName,s.LastName

--Subjects with Students
SELECT t.FirstName+' '+t.LastName AS [FullName],su.Name+'-'+CAST(su.Lessons AS NVARCHAR(10)) AS [Subjects],COUNT(st.StudentId) AS [Students] FROM Teachers AS t
JOIN Subjects AS su ON t.SubjectId=su.Id
JOIN StudentsTeachers AS st ON st.TeacherId=t.Id
GROUP BY t.FirstName,t.LastName,su.Name,su.Lessons
ORDER BY [Students] DESC,[FullName],[Subjects]

--Students to Go
SELECT s.FirstName+' '+s.LastName AS FullName FROM Students AS s
LEFT JOIN StudentsExams AS st ON s.Id=st.StudentId
WHERE st.ExamId IS NULL
ORDER BY FullName

--Busiest Teachers
SELECT TOP(10) t.FirstName,t.LastName,COUNT(st.StudentId) AS StudentsCount FROM Teachers AS t
JOIN StudentsTeachers AS st ON t.Id=st.TeacherId
GROUP BY t.FirstName,t.LastName,t.Id
ORDER BY StudentsCount DESC,t.FirstName,t.LastName

--Top Students
SELECT TOP(10) s.FirstName,s.LastName,FORMAT(AVG(se.Grade),'N2') AS Grade FROM Students AS s
JOIN StudentsExams AS se ON s.Id=se.StudentId
GROUP BY s.FirstName,s.LastName
ORDER BY AVG(se.Grade) DESC,s.FirstName,s.LastName

--Second Highest Grade
SELECT k.FirstName,k.LastName,k.Grade FROM
(SELECT DISTINCT s.FirstName,s.LastName,se.Grade,ROW_NUMBER() OVER(PARTITION BY s.Id ORDER BY se.Grade DESC) AS GradeRank FROM Students AS s
JOIN StudentsSubjects AS se ON s.Id=se.StudentId
--GROUP BY s.FirstName,s.LastName,s.Id,se.Grade
) AS k
WHERE k.GradeRank=2

Order BY k.FirstName,k.LastName

--Not So In The Studying
SELECT s.FirstName+ISNULL(' '+s.MiddleName,'')+' '+s.LastName AS 'Full Name' FROM Students AS s
LEFT JOIN StudentsSubjects AS ss ON s.Id=ss.StudentId
GROUP BY s.FirstName,s.LastName,s.Id,s.MiddleName
HAVING COUNT(ss.SubjectId)=0
ORDER BY [Full Name]

--TOP Student per Teacher
SELECT k.[Teacher Full Name],k.Name,k.[Student Full Name],k.AVGGrade FROM
(SELECT t.FirstName+' '+t.LastName AS 'Teacher Full Name'
,su.Name,FORMAT(AVG(ss.Grade),'N2') AS AVGGrade,ROW_NUMBER() OVER (PARTITION BY s.Id ORDER BY FORMAT(AVG(ss.Grade),'N2')) AS GradeRank,s.FirstName+' '+s.LastName AS 'Student Full Name'
 FROM Teachers AS t
 JOIN Subjects AS su ON t.SubjectId=su.Id
 JOIN StudentsSubjects AS ss ON su.Id=ss.SubjectId
 JOIN Students AS s ON s.Id=ss.StudentId
 GROUP BY t.FirstName,t.LastName,t.Id,t.SubjectId,s.FirstName,s.LastName,s.Id,su.Name,ss.Grade
 ) AS k
 GROUP BY k.[Teacher Full Name],k.Name,k.AVGGrade,k.[Student Full Name]
 HAVING GradeRank=1
 ORDER BY k.Name,k.[Teacher Full Name],k.AVGGrade DESC
 

 SELECT t.FirstName+' '+t.LastName AS 'Teacher Full Name'
,su.[Name],FORMAT(AVG(ss.Grade),'N2') AS AVGGrade,ROW_NUMBER() OVER (PARTITION BY st.TeacherId ORDER BY FORMAT(AVG(ss.Grade),'N2') DESC) AS GradeRank,s.FirstName+' '+s.LastName AS 'Student Full Name'
 FROM Teachers AS t
 JOIN StudentsTeachers AS st ON st.TeacherId=t.Id
 JOIN Subjects AS su ON t.SubjectId=su.Id
 JOIN StudentsSubjects AS ss ON su.Id=ss.SubjectId
 JOIN Students AS s ON s.Id=ss.StudentId
 GROUP BY t.FirstName,t.LastName,st.TeacherId,su.Name,s.FirstName,s.LastName
  HAVING su.Name = 'Art'
 ORDER BY [Teacher Full Name],AVGGrade DESC

 SELECT 
 k.[Teacher Full Name]
 ,k.[Subject Name]
 ,k.[Student Full Name]
 ,FORMAT(MAX(k.AvgGrade),'N2') AS MaxGrade 
 
 FROM

 (SELECT t.FirstName+' '+t.LastName AS 'Teacher Full Name'
 ,su.Name AS 'Subject Name'
 ,s.FirstName+' '+s.LastName AS 'Student Full Name'
 , AVG(ss.Grade) AS AvgGrade
 ,ROW_NUMBER() OVER (PARTITION BY st.TeacherId ORDER BY FORMAT(AVG(ss.Grade),'N2') DESC) AS GradeRank
  FROM Teachers AS t

 JOIN StudentsTeachers AS st ON st.TeacherId=t.Id
 JOIN Students AS s ON s.Id=st.StudentId
 JOIN StudentsSubjects AS ss ON ss.StudentId=s.Id
 JOIN Subjects AS su ON ss.SubjectId=su.Id 
 AND t.SubjectId=su.Id
 GROUP BY st.TeacherId,s.Id,t.Id,t.FirstName,t.LastName,su.Name,s.FirstName,s.LastName
 ) AS k
 WHERE GradeRank=1
 GROUP BY k.[Teacher Full Name],k.[Subject Name],k.[Student Full Name]
 ORDER BY k.[Subject Name],k.[Teacher Full Name],MaxGrade DESC

 --Average Grade per Subject
 SELECT s.[Name],AVG(ss.Grade) FROM Subjects AS s
 JOIN StudentsSubjects AS ss ON s.Id=ss.SubjectId
 GROUP BY s.Id,s.Name
 ORDER BY s.Id

 --Exams Information
 SELECT 

 ISNULL('Q'+CAST(DATEPART(QUARTER,e.Date) AS VARCHAR),'TBA') AS [Quarter],e.Date,
 
 s.Name,COUNT(se.StudentId) AS StudentsCount FROM Exams AS e
 JOIN StudentsExams AS se ON se.ExamId=e.Id AND Grade>=4
 JOIN Subjects AS s ON e.SubjectId=s.Id
 
 GROUP BY e.Id,s.Name,e.Date

 ORDER BY ISNULL(CAST(DATEPART(QUARTER,e.Date) AS VARCHAR),'TBA') ASC

 --Programmability
 --Exam Grades
 CREATE FUNCTION udf_ExamGradesToUpdate(@studentId INT, @grade DECIMAL(15,2))
 RETURNS VARCHAR(MAX)
 AS
 BEGIN
 DECLARE @studentName NVARCHAR(50)= (SELECT s.FirstName FROM Students AS s WHERE Id=@studentId)

 IF(@grade>6)
 BEGIN
 RETURN 'Grade cannot be above 6.00!'
 END

 IF(@studentName IS NULL)
 BEGIN
 RETURN 'The student with provided id does not exist in the school!'
 END

 DECLARE @count INT = 
 (SELECT ss.Grade FROM Students AS s
 JOIN StudentsExams AS ss ON ss.StudentId=s.Id 
 AND s.Id=@studentId
 AND ss.Grade BETWEEN @grade AND @grade+0.50)

 RETURN 'You have to update '+CAST(@count AS VARCHAR(6))+' grades for the student '+@studentName
 END

 --Exclude From School
 CREATE PROC usp_ExcludeFromSchool(@StudentId INT)
 AS
 BEGIN
 DECLARE @stId INT = (SELECT Id FROM Students WHERE Id=@StudentId)

 IF(@stId IS NULL)
 BEGIN
 RAISERROR('This school has no student with the provided id!',16,1)
 ROLLBACK
 RETURN
 END

 --studentsteachers,studentssubjects,studentsexams,students
 DELETE FROM StudentsTeachers
 WHERE StudentId=@stId

 DELETE FROM StudentsSubjects
 WHERE StudentId=@stId

 DELETE FROM StudentsExams
 WHERE StudentId=@stId

 DELETE FROM Students
 WHERE Id=@stId

 END

 --Deleted Student
 CREATE TABLE ExcludedStudents(
 StudentId INT IDENTITY PRIMARY KEY,
 StudentName NVARCHAR(100))

 CREATE TRIGGER tr_logDeletedStudents ON Students FOR DELETE
 AS
 INSERT INTO ExcludedStudents(StudentId,StudentName)
 SELECT Id,FirstName+' '+LastName FROM deleted