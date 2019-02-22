--1. Employee Address
SELECT TOP(5) EmployeeId,JobTitle,e.AddressID,a.AddressText FROM Employees AS e
JOIN Addresses AS a ON e.AddressID = a.AddressID
ORDER BY e.AddressID ASC

--2. Addresses with Towns
SELECT TOP(50) e.FirstName,e.LastName,t.Name AS 'Town',a.AddressText FROM Employees AS e
JOIN Addresses AS a ON e.AddressID = a.AddressID
JOIN Towns AS t ON a.TownID = t.TownID
ORDER BY e.FirstName ASC,e.LastName ASC

--3. Sales Employee
SELECT e.EmployeeID,e.FirstName,e.LastName,d.Name AS 'DeparmentName' FROM Employees AS e
JOIN Departments AS d ON e.DepartmentID=d.DepartmentID
WHERE d.Name = 'Sales'
ORDER BY e.EmployeeID ASC

--4. Employee Departments
SELECT TOP(5) e.EmployeeID,e.FirstName,e.Salary,d.Name AS 'DeparmentName' FROM Employees AS e
JOIN Departments AS d ON e.DepartmentID=d.DepartmentID
WHERE e.Salary>15000
ORDER BY d.DepartmentID ASC

--5. Employees Without Project
SELECT TOP 3 e.EmployeeID,e.FirstName FROM Employees AS e
LEFT OUTER JOIN EmployeesProjects AS ep ON e.EmployeeID=ep.EmployeeID
WHERE ep.ProjectID IS NULL

--6. Employees Hired After
SELECT e.FirstName,e.LastName,e.HireDate,d.Name AS 'DeparmentName' FROM Employees AS e
JOIN Departments AS d ON e.DepartmentID=d.DepartmentID
WHERE d.Name IN ('Sales','Finance') AND e.HireDate>'01-01-1999'
ORDER BY e.HireDate ASC

--7. Employees with Project
SELECT TOP 5 e.EmployeeID,e.FirstName,p.Name AS 'ProjectName' FROM Employees AS e
JOIN EmployeesProjects AS ep ON e.EmployeeID=ep.EmployeeID
JOIN Projects AS p ON p.ProjectID=ep.ProjectID
WHERE p.StartDate>'08-13-2002' AND p.EndDate IS NULL
ORDER BY e.EmployeeID ASC

--8. Employee 24
SELECT e.EmployeeID,e.FirstName,
CASE
WHEN DATEPART(YEAR,p.StartDate)>=2005 THEN NULL
ELSE p.[Name]
END AS ProjectName
 FROM Employees AS e
INNER JOIN EmployeesProjects AS ep 
ON e.EmployeeID=ep.EmployeeID AND e.EmployeeID=24
INNER JOIN Projects AS p ON p.ProjectID=ep.ProjectID

--9. Employee Manager
SELECT e.EmployeeID,e.FirstName,e.ManagerID,m.FirstName AS 'ManagerName'
 FROM Employees AS e
INNER JOIN Employees AS m ON e.ManagerID=m.EmployeeID
 WHERE e.ManagerID IN (3,7)

 --10. Employee Summary
 SELECT TOP 50 e.EmployeeID,e.FirstName+' '+e.LastName AS EmployeeName,
 m.FirstName +' '+m.LastName AS 'ManagerName',d.Name AS DepartmentName
 FROM Employees AS e
INNER JOIN Employees AS m ON e.ManagerID=m.EmployeeID
LEFT OUTER JOIN Departments AS d ON e.DepartmentID=d.DepartmentID
ORDER BY e.EmployeeID

--11. Min Average Salary
SELECT TOP 1 AVG(Salary) FROM Employees AS e
GROUP BY e.DepartmentID
ORDER BY AVG(Salary) ASC

--Use Geography
--12. Highest Peaks in Bulgaria
SELECT mc.CountryCode,m.MountainRange,p.PeakName,p.Elevation FROM Peaks AS p
INNER JOIN Mountains AS m ON p.MountainId=m.Id
INNER JOIN MountainsCountries AS mc ON m.Id=mc.MountainId
WHERE mc.CountryCode='BG' AND p.Elevation>2835
ORDER BY p.Elevation DESC

--13. Count Mountain Ranges
SELECT mc.CountryCode,COUNT(m.MountainRange) FROM Mountains AS m
JOIN MountainsCountries AS mc ON m.Id=mc.MountainId
WHERE mc.CountryCode IN ('BG','RU','US')
GROUP BY mc.CountryCode

--14. Countries with rivers
SELECT TOP(5) c.CountryName,r.RiverName FROM Countries AS c
LEFT JOIN CountriesRivers AS cr ON c.CountryCode = cr.CountryCode
LEFT JOIN Rivers AS r ON cr.RiverId=r.Id
WHERE c.ContinentCode='AF'
ORDER BY c.CountryName ASC

--15. Continents and Currencies
SELECT cq.ContinentCode,cq.CurrencyCode,
cq.CurrencyUsage
 FROM
(SELECT c.ContinentCode,c.CurrencyCode,
COUNT(c.CurrencyCode) AS CurrencyUsage,
DENSE_RANK() OVER (PARTITION BY c.ContinentCode 
ORDER BY COUNT(c.CurrencyCode) DESC) AS [CurrencyRank]
FROM Countries AS c
JOIN Continents AS co ON co.ContinentCode=c.ContinentCode
GROUP BY c.ContinentCode,c.CurrencyCode
HAVING COUNT(c.CurrencyCode) > 1) AS cq
WHERE cq.CurrencyRank = 1
ORDER BY cq.ContinentCode

--16. Countries Without any Mountains
SELECT COUNT(c.CountryCode) FROM Countries AS c
FULL JOIN MountainsCountries AS mc ON mc.CountryCode=c.CountryCode
FULL JOIN Mountains AS m ON m.Id=mc.MountainId
WHERE m.Id IS NULL

--17. Highest Peak and Longest River by Country
SELECT TOP(5) c.CountryName,MAX(p.Elevation) AS HighestPeakElevation,
MAX(r.Length) AS HighestRiverLength
 FROM Countries AS c
FULL JOIN MountainsCountries AS mc ON c.CountryCode=mc.CountryCode
FULL JOIN Mountains AS m ON m.Id=mc.MountainId
FULL JOIN Peaks AS p ON p.MountainId=m.Id
FULL JOIN CountriesRivers AS cr ON cr.CountryCode=c.CountryCode
FULL JOIN Rivers AS r ON r.Id=cr.RiverId
GROUP BY c.CountryName
ORDER BY HighestPeakElevation DESC,HighestRiverLength DESC,c.CountryName

--18. Highest Peak and Longest River by Country
SELECT TOP 5 s.Country,
ISNULL(s.[Highest Peak Name],'(no highest peak)'),
ISNULL(s.[Highest Peak Elevation],0),
ISNULL(s.Mountain,'(no mountain)')
 FROM
(SELECT c.CountryName AS Country,p.PeakName AS 'Highest Peak Name',MAX(p.Elevation)
 AS 'Highest Peak Elevation',m.MountainRange AS Mountain
,DENSE_RANK() OVER (PARTITION BY c.CountryName 
ORDER BY MAX(p.Elevation) DESC) AS [PeakRank]
 FROM Countries AS c
LEFT JOIN MountainsCountries AS mc ON c.CountryCode=mc.CountryCode
LEFT JOIN Mountains AS m ON m.Id=mc.MountainId
LEFT JOIN Peaks AS p ON p.MountainId=m.Id
GROUP BY c.CountryName,m.MountainRange,p.PeakName) AS s
WHERE PeakRank=1
ORDER BY s.Country,s.[Highest Peak Name]