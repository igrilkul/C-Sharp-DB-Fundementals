-- 1. Find Names of All Emplyees by First Name
SELECT FirstName,LastName FROM Employees
WHERE FirstName LIKE 'SA%'

-- 2. Find Names of All Emplyees by Last Name
SELECT FirstName,LastName FROM Employees
WHERE FirstName LIKE '%ei%'

--3. Find First Names of All Employees
SELECT FirstName FROM Employees
WHERE DepartmentID IN (3,10) 
AND DATEPART(YEAR, HireDate) BETWEEN 1995 AND 2005

--4. Find All Employees Except Engineers
SELECT FirstName,LastName FROM Employees
WHERE JobTitle NOT LIKE '%engineer%'

--5. Find Towns with Name Length
SELECT [Name] FROM Towns
WHERE LEN([Name]) IN (5,6) 
ORDER BY [Name]

--6. Find Towns Starting With
SELECT TownID,[Name] FROM Towns
WHERE [Name] LIKE '[mkbe]%'
ORDER BY [Name]

--7. Find Towns Not Starting With
SELECT TownID,[Name] FROM Towns
WHERE [Name] NOT LIKE '[rbd]%'
ORDER BY [Name]
 
--8. Create View Employees Hired After 2000 Year
CREATE VIEW V_EmployeesHiredAfter2000 AS
SELECT FirstName,LastName FROM Employees
WHERE DATEPART(YEAR,HireDate) > 2000

--9. Length of Last Name
SELECT FirstName,LastName FROM Employees
WHERE LEN(LastName) =5

--10. Rank Employees by Salary
SELECT EmployeeID,FirstName,LastName,Salary,
DENSE_RANK() OVER (PARTITION BY Salary ORDER BY EmployeeId) AS 'Rank' FROM Employees
WHERE Salary BETWEEN 10000 AND 50000
ORDER BY Salary DESC

--11. Find All Employees with Rank 2 *
SELECT * FROM (
SELECT EmployeeID,FirstName,LastName,Salary,
DENSE_RANK() OVER (PARTITION BY Salary ORDER BY EmployeeId) AS 'Rank' FROM Employees
WHERE Salary BETWEEN 10000 AND 50000 
) AS SpecialTable 
WHERE SpecialTable.Rank = 2
ORDER BY Salary DESC

-- Geography DB
USE Geography
--12. Countries Holding 'A' 3 or More Times
SELECT CountryName AS 'Country Name',IsoCode AS 'ISO Code'
FROM Countries
WHERE CountryName LIKE '%a%a%a%'
ORDER BY IsoCode

--13. Mix of Peak and River Names
SELECT PeakName, RiverName, LOWER(PeakName + SUBSTRING(RiverName,2,LEN(RiverName)))
FROM Peaks, Rivers
WHERE RIGHT(PeakName,1) = LEFT(RiverName,1)
ORDER BY PeakName,RiverName


--Diablo DB
USE Diablo
--14 Games from 2011 and 2012 year
SELECT TOP(50) [Name],FORMAT(Start, 'yyyy-MM-dd') AS [Start]
FROM Games
WHERE DATEPART(YEAR,[Start]) IN (2011,2012)
ORDER BY [Start],[Name]

--15. User Email Providers
SELECT Username,RIGHT(Email, LEN(Email) - CHARINDEX('@', Email, 1)) AS [Email Provider]
FROM Users
ORDER BY [Email Provider],Username

--16. Get Users with IPAdress Like Pattern
SELECT Username, IpAddress AS [IP Address]
FROM USERS
WHERE IpAddress LIKE '___.1_%._%.___'
ORDER BY Username

--17. Show All Games with Duration and Part of the Day
SELECT Name,
CASE 
WHEN DATEPART(HOUR,Start) >=0 AND DATEPART(HOUR,Start)<12  THEN 'Morning'
WHEN DATEPART(HOUR,Start) >=12 AND DATEPART(HOUR,Start)<18 THEN 'Afternoon'
WHEN DATEPART(HOUR,Start) >=18 AND DATEPART(HOUR,Start)<24 THEN 'Evening'
END AS [Part of the Day],
CASE
WHEN Duration <=3 THEN 'Extra Short'
WHEN Duration >3 AND Duration<=6 THEN 'Short'
WHEN Duration >6 THEN 'Long'
WHEN Duration IS NULL THEN 'Extra Long'
END AS [Duration]
 FROM Games
 ORDER BY [Name],[Duration],[Part of the Day]

 --18. Orders Table
 SELECT ProductName,OrderDate,
 DATEADD(DAY,3,OrderDate) AS 'Pay Due',
 DATEADD(MONTH,1,OrderDate) AS 'Deliver Due'
  FROM Orders

  --19. People Table
  CREATE TABLE People(
  Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  Name VARCHAR(80) NOT NULL,
  Birthdate DATETIME NOT NULL
  )

  INSERT INTO People([Name], Birthdate) VALUES
('Victor', CONVERT(datetime, '07-12-2000', 103)),
('Steven', CONVERT(datetime, '10-09-1992', 103)),
('Stephen', CONVERT(datetime, '19-10-1932', 103)),
('John', CONVERT(datetime, '01-01-2000', 103))

SELECT [Name],
DATEDIFF(YEAR,Birthdate,GETDATE()) AS [Age in Years],
DATEDIFF(MONTH,Birthdate,GETDATE()) AS [Age in Months],
DATEDIFF(DAY,Birthdate,GETDATE()) AS [Age in Days],
DATEDIFF(MINUTE,Birthdate,GETDATE()) AS [Age in Minutes]
 FROM People