--1. Records' Count
SELECT COUNT(*) FROM WizzardDeposits

--2. Longest Magic Wand
SELECT TOP(1) MagicWandSize AS LongestMagicWand FROM WizzardDeposits
ORDER BY MagicWandSize DESC

--3. Longest Magic Wand per Deposit Groups
SELECT DepositGroup, MAX(MagicWandSize) AS [LongestMagicWand]
FROM WizzardDeposits
GROUP BY DepositGroup

--4. Smallest Deposit Group per Magic Wand Size *
SELECT TOP(2) DepositGroup
FROM WizzardDeposits
GROUP BY DepositGroup
ORDER BY AVG(MagicWandSize)

--5. Deposits Sum
SELECT DepositGroup, SUM(DepositAmount) AS [TotalSum]
FROM WizzardDeposits
GROUP BY DepositGroup

--6. Deposits Sum for Ollivander Family
SELECT DepositGroup, SUM(DepositAmount) AS [TotalSum]
FROM WizzardDeposits
WHERE MagicWandCreator = 'Ollivander family'
GROUP BY DepositGroup

--6
SELECT DepositGroup, SUM(DepositAmount) AS [TotalSum] 
FROM WizzardDeposits
GROUP BY DepositGroup, MagicWandCreator
HAVING MagicWandCreator = 'Ollivander family'

--7. Deposits Filter
SELECT e.DepositGroup, SUM(e.DepositAmount) AS [TotalSum]
FROM WizzardDeposits AS e
WHERE e.MagicWandCreator = 'Ollivander family'
GROUP BY DepositGroup
HAVING SUM(e.DepositAmount)<150000
ORDER BY [TotalSum] DESC

--8. Deposits Charge
SELECT DepositGroup,MagicWandCreator,MIN(DepositCharge)
 FROM WizzardDeposits
 GROUP BY DepositGroup,MagicWandCreator
 ORDER BY MagicWandCreator ASC

 --9. Age Groups
 SELECT AgeGroup,COUNT(*) AS WizardCount FROM
 (SELECT 
 CASE
 WHEN Age BETWEEN 0 AND 10 THEN '[0-10]'
 WHEN Age BETWEEN 11 AND 20 THEN '[11-20]'
 WHEN Age BETWEEN 21 AND 30 THEN '[21-30]'
 WHEN Age BETWEEN 31 AND 40 THEN '[31-40]'
 WHEN Age BETWEEN 41 AND 50 THEN '[41-50]'
 WHEN Age BETWEEN 51 AND 60 THEN '[51-60]'
 ELSE '[61+]'
 END AS AgeGroup
 FROM WizzardDeposits) AS AgeGroupTable
 GROUP BY AgeGroup

 --10. First Letter
 SELECT LEFT(FirstName,1) AS FirstLetter FROM WizzardDeposits
 WHERE DepositGroup = 'Troll Chest'
 GROUP BY LEFT(FirstName,1)

 --11. Average Interest
 SELECT DepositGroup,IsDepositExpired
 ,AVG(DepositInterest) AS AverageInterest
  FROM WizzardDeposits
  WHERE DepositStartDate > '01/01/1985'
  GROUP BY DepositGroup,IsDepositExpired
  ORDER BY DepositGroup DESC,IsDepositExpired ASC

  --12. Rich Wizard, Poor Wizard *
  SELECT FirstName AS 'Host Wizard',
  DepositAmount AS 'Host Wizard Deposit',
  LEAD(FirstName,1) OVER(ORDER BY Id) AS 'Guest Wizard',
  LEAD(DepositAmount,1) OVER(ORDER BY Id) AS 'Guest Wizard Deposit',
  DepositAmount - LEAD(DepositAmount, 1) OVER(ORDER BY Id) AS Diffrence
   FROM WizzardDeposits
  
  SELECT SUM(Diffrence) AS [SumDifference] FROM
(SELECT DepositAmount - LEAD(DepositAmount, 1) OVER(ORDER BY Id) AS Diffrence
-- OR DepositAmount - LAG(DepositAmount, 1) OVER(ORDER BY Id DESC) AS Diffrence
FROM WizzardDeposits) AS e

--USE SoftUni
--13. Departments Total Salaries
SELECT DepartmentId,SUM(Salary) AS [TotalSalary] FROM Employees
GROUP BY DepartmentID

--14. Employees Minimum Salaries
SELECT DepartmentId,MIN(Salary) AS [MinimumSalary] FROM Employees
WHERE DepartmentID IN(2,5,7)
GROUP BY DepartmentID

--15. Employees Average Salaries
SELECT * 
INTO NewTable 
FROM Employees 
WHERE Salary>30000 

DELETE FROM NewTable
WHERE ManagerID = 42

UPDATE NewTable
SET Salary+=5000
WHERE DepartmentID=1

SELECT DepartmentID,AVG(Salary) FROM NewTable
GROUP BY(DepartmentID)

--16. Employees Maximum Salaries
SELECT DepartmentID,MAX(Salary) AS [MaxSalary] FROM Employees
GROUP BY(DepartmentID)
HAVING MAX(Salary) NOT BETWEEN 30000 AND 70000

--17. Employees Count Salaries
SELECT COUNT(*) FROM Employees
WHERE ManagerID IS NULL

--18. 3rd Highest Salary
