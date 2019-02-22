--1. Employees with Salary Above 35000
CREATE PROC usp_GetEmployeesSalaryAbove35000
AS
SELECT FirstName,LastName FROM Employees
WHERE Salary>35000

EXEC usp_GetEmployeesSalaryAbove35000

--2. Employees with Salary Above Number
CREATE PROC usp_GetEmployeesSalaryAboveNumber(@Number DECIMAL(18,4))
AS
SELECT FirstName,LastName FROM Employees
WHERE Salary>=@Number

--3. Town Names Starting With
CREATE PROC usp_GetTownsStartingWith(@Start NVARCHAR)
AS
BEGIN
SELECT Name AS Town FROM Towns
WHERE [Name] LIKE (@Start+'%')
END

--4. Employees from Town
CREATE PROC usp_GetEmployeesFromTown(@Town NVARCHAR(20))
AS
SELECT FirstName,LastName FROM Employees AS e
JOIN Addresses AS a ON a.AddressID=e.AddressID
JOIN Towns AS t ON t.TownID=a.TownID
WHERE t.Name=@Town

--5. Salary Level Function
CREATE FUNCTION ufn_GetSalaryLevel(@salary DECIMAL(18,4))
RETURNS NVARCHAR(10)
AS
BEGIN
DECLARE @salaryLevel VARCHAR(10)
IF(@salary<30000)
SET @salaryLevel = 'Low'
ELSE IF(@salary>=30000 AND @salary<50000)
SET @salaryLevel='Average'
ELSE
SET @salaryLevel = 'High'

RETURN @salaryLevel
END;

--6. Employees by Salary Level
CREATE PROC usp_EmployeesBySalaryLevel(@salaryLevel VARCHAR(10))
AS
SELECT FirstName, LastName
FROM Employees
WHERE dbo.ufn_GetSalaryLevel(Salary)=@salaryLevel

EXEC usp_EmployeesBySalaryLevel 'Low'

--7. Define Function
CREATE FUNCTION ufn_IsWordComprised(@setOfLetters VARCHAR(50),@word VARCHAR(50))
RETURNS BIT
AS
BEGIN
DECLARE @boolValue BIT,
@count INT =1

WHILE(@count <=LEN(@word))
BEGIN
DECLARE @currentLetter CHAR(1) = SUBSTRING(@word,@count,1)
DECLARE @charIndex INT =CHARINDEX(@currentLetter,@setOfLetters)
IF(@charIndex = 0)
BEGIN
RETURN 0
END

SET @count+=1
END

RETURN 1

END

--8. Delete Employees and Departments *
CREATE PROCEDURE usp_DeleteEmployeesFromDepartment (@departmentId INT)
AS
ALTER TABLE Departments
ALTER COLUMN ManagerId INT

DELETE FROM EmployeesProjects
WHERE EmployeeID IN 
(SELECT EmployeeID FROM Employees 
WHERE DepartmentID = @departmentId)

UPDATE Departments
SET ManagerID=NULL
WHERE DepartmentID=@departmentId

UPDATE Employees
SET ManagerID=NULL
WHERE ManagerID IN 
(SELECT EmployeeID FROM Employees 
WHERE DepartmentID=@departmentId)

DELETE FROM Employees
WHERE DepartmentID=@departmentId

DELETE FROM Departments
WHERE DepartmentID=@departmentId

SELECT COUNT(*)
FROM Employees
WHERE DepartmentID=@departmentId

--9. Find Full Name
CREATE PROC usp_GetHoldersFullName
AS
SELECT FirstName+' '+LastName AS 'Full Name' 
FROM AccountHolders

--10. People with Balance Higher Than
CREATE PROC usp_GetHoldersWithBalanceHigherThan @value DECIMAL(18,4)
AS
SELECT ah.FirstName,ah.LastName
 FROM AccountHolders AS ah
JOIN Accounts AS a ON ah.Id=a.AccountHolderId
GROUP BY ah.Id,ah.FirstName,ah.LastName
HAVING SUM(a.Balance)>@value
ORDER BY ah.FirstName,ah.LastName

--11. Future Value Function
CREATE FUNCTION ufn_CalculateFutureValue 
(@sum DECIMAL(16,2),@yearlyInterestRate FLOAT,@numberOfYears INT)
RETURNS DECIMAL(16,4)
AS
BEGIN
DECLARE @futureValue DECIMAL(16,4)
SET @futureValue = @sum*(POWER(1+@yearlyInterestRate,@numberOfYears))
RETURN @futureValue
END

--12. Calculating Interest
CREATE PROC usp_CalculateFutureValueForAccount
(@accountId INT, @interestRate FLOAT)
AS
SELECT 
ah.Id AS 'Account Id',ah.FirstName,ah.LastName
,a.Balance AS 'Current Balance'
,(SELECT ROUND(
dbo.ufn_CalculateFutureValue(a.Balance,@interestRate,5),4)
 )
 AS 'Balance In 5 years'
 FROM Accounts AS a
 JOIN AccountHolders AS ah ON a.AccountHolderId=ah.Id
 WHERE a.Id=@accountId

EXEC dbo.usp_CalculateFutureValueForAccount 1,0.1

--13. Scalar Function: Cash in User Games Odd Rows *
CREATE FUNCTION ufn_CashInUsersGames(@gameName VARCHAR(50))
RETURNS TABLE
AS
RETURN(
SELECT SUM(k.Cash) AS SumCash FROM(
SELECT Cash,
ROW_NUMBER() OVER (PARTITION BY g.Name ORDER BY ug.Cash DESC) AS Row
 FROM UsersGames AS ug
JOIN Games AS g ON ug.GameId=g.Id
WHERE g.Name=@gameName
) AS k
WHERE k.Row%2 !=0 
)
