--1. DDL

CREATE TABLE Categories(
Id INT IDENTITY PRIMARY KEY
,[Name] NVARCHAR(30) NOT NULL)

CREATE TABLE Items(
Id INT IDENTITY PRIMARY KEY
,[Name] NVARCHAR(30) NOT NULL
,Price DECIMAL(16,2) NOT NULL
,CategoryId INT NOT NULL FOREIGN KEY REFERENCES Categories(Id))

CREATE TABLE Employees(
Id INT IDENTITY PRIMARY KEY
,FirstName NVARCHAR(50) NOT NULL
,LastName NVARCHAR(50) NOT NULL
,Phone CHAR(12) NOT NULL
,Salary DECIMAL(16,2) NOT NULL)

CREATE TABLE Orders(
Id INT IDENTITY PRIMARY KEY
,[DateTime] DATETIME NOT NULL
,EmployeeId INT NOT NULL FOREIGN KEY REFERENCES Employees(Id))

CREATE TABLE OrderItems(
OrderId INT NOT NULL FOREIGN KEY REFERENCES Orders(Id)
,ItemId INT NOT NULL FOREIGN KEY REFERENCES Items(Id)
,Quantity INT NOT NULL
 Constraint CK_OrderItems_QuantityMinOne Check ( Quantity >= 1 )
 PRIMARY KEY(OrderId,ItemId)
 )

 CREATE TABLE Shifts(
 Id INT IDENTITY
 ,EmployeeId INT FOREIGN KEY REFERENCES Employees(Id)
 ,CheckIn DATETIME NOT NULL
 ,CheckOut DATETIME NOT NULL
 
 
 PRIMARY KEY(Id,EmployeeId)
 )

 ALTER TABLE Shifts
 ADD CONSTRAINT CK_Shifts_chkout Check ( CheckOut>CheckIn )

 --2. Insert
 INSERT INTO Employees(FirstName,LastName,Phone,Salary) VALUES
 ('Stoyan','Petrov','888-785-8573',500.25),
 ('Stamat','Nikolov','789-613-1122',999995.25),
 ('Evgeni','Petkov','645-369-9517',1234.51),
 ('Krasimir','Vidolov','321-471-9982',50.25)

 INSERT INTO Items(Name,Price,CategoryId) VALUES
 ('Tesla battery',154.25,8),
 ('Chess',30.25,8),
 ('Juice',5.32,1),
 ('Glasses',10,8),
 ('Bottle of water',1,1)

 UPDATE Items
 SET Price *=1.27
 WHERE CategoryId IN (1,2,3)

 DELETE OrderItems
 WHERE OrderId = 48

 --3. Querying
 SELECT Id,FirstName FROM Employees
 WHERE Salary>6500
 ORDER BY FirstName,Id

 SELECT FirstName+' '+LastName AS 'Full Name',Phone AS 'Phone Number' FROM Employees
 WHERE Phone LIKE '3%'
 ORDER BY FirstName,Phone

 SELECT FirstName,LastName,COUNT(o.EmployeeId) AS [Count] FROM Employees AS e
 LEFT JOIN Orders AS o ON o.EmployeeId=e.Id
 GROUP BY o.EmployeeId,FirstName,LastName
 HAVING COUNT(o.EmployeeId)>0
 ORDER BY [Count] DESC,FirstName

 SELECT e.FirstName,e.LastName,AVG(DATEDIFF(HOUR,s.CheckIn,s.CheckOut)) AS 'Work Hours' FROM Employees AS e
 JOIN Shifts AS s ON e.Id=s.EmployeeId
 GROUP BY e.FirstName,e.LastName,e.Id
 HAVING AVG(DATEDIFF(HOUR,s.CheckIn,s.CheckOut))>7
 ORDER BY [Work Hours] DESC,e.Id

 SELECT TOP(1) oi.OrderId,SUM(i.Price*oi.Quantity) AS 'Total Price' FROM OrderItems AS oi
 LEFT JOIN Items AS i ON oi.ItemId = i.Id
 GROUP BY oi.OrderId
 ORDER BY [Total Price] DESC

 SELECT TOP(10) oi.OrderId,MAX(i.Price) AS ExpensivePrice
 ,MIN(i.Price) AS CheapPrice FROM OrderItems AS oi
 LEFT JOIN Items AS i ON oi.ItemId = i.Id
 GROUP BY oi.OrderId
 ORDER BY ExpensivePrice DESC,oi.OrderId

 SELECT e.Id,e.FirstName,e.LastName FROM Employees AS e
 INNER JOIN Orders AS o ON e.Id=o.EmployeeId
 GROUP BY e.Id,e.FirstName,e.LastName
 ORDER BY e.Id

 --12.
 SELECT DISTINCT e.Id,e.FirstName+' '+e.LastName AS 'Full Name'
 FROM Employees AS e
 JOIN Shifts AS s ON e.Id=s.EmployeeId
 WHERE DATEDIFF(HOUR,s.CheckIn,s.CheckOut)<4
 ORDER BY e.Id

 --13. Sellers
 SELECT TOP(10) e.FirstName+' '+e.LastName
  AS 'Full Name'
  ,SUM(i.Price*oi.Quantity) AS 'Total Price'
  ,SUM(oi.Quantity) AS [Items] FROM Employees AS e
 JOIN Orders AS o ON e.Id=o.EmployeeId
 JOIN OrderItems AS oi ON o.Id=oi.OrderId
 JOIN Items AS i ON i.Id=oi.ItemId
 WHERE o.DateTime<'2018-06-15'
 GROUP BY e.FirstName,e.LastName
 ORDER BY [Total Price] DESC,[Items] DESC

 --14. Tough Days
 SELECT e.FirstName+' '+e.LastName AS 'Full Name'
 ,DATENAME(WEEKDAY,s.CheckIn) AS [Day]
  FROM Employees AS e
  LEFT JOIN Orders AS o ON e.Id=o.EmployeeId
  JOIN Shifts AS s ON e.Id=s.EmployeeId
  WHERE DATEDIFF(HOUR,s.CheckIn,s.CheckOut)>12 AND o.EmployeeId IS NULL 
  ORDER BY e.Id

  --15.
  SELECT k.[Full Name],DATEDIFF(HOUR,s.CheckIn,s.CheckOut)AS WorkHours,k.TotalPrice FROM
  (SELECT  o.DateTime,o.EmployeeId,o.Id,e.FirstName+' '+e.LastName AS 'Full Name'
  ,SUM(oi.Quantity*i.Price) AS 'TotalPrice',
  ROW_NUMBER() OVER (PARTITION BY e.Id ORDER BY SUM(oi.Quantity*i.Price) DESC) AS PriceRank
   FROM Employees AS e
  JOIN Orders AS o ON o.EmployeeId=e.Id
  JOIN OrderItems AS oi ON o.Id=oi.OrderId
  JOIN Items AS i ON oi.ItemId=i.Id
  GROUP BY o.Id,e.FirstName,e.LastName,e.Id,o.EmployeeId,o.DateTime
  ) AS k
  JOIN Shifts AS s ON s.EmployeeId=k.EmployeeId
  WHERE k.PriceRank=1 AND k.DateTime BETWEEN s.CheckIn AND s.CheckOut
  ORDER BY k.[Full Name],WorkHours DESC,k.TotalPrice DESC

  --16. Average Profit per Day
  SELECT DATEPART(DAY,o.DateTime) AS [DayOfMonth],FORMAT(AVG(oi.Quantity*i.Price),'N2') AS 'TotalProfit' FROM Orders AS o
  JOIN OrderItems AS oi ON o.Id=oi.OrderId
  JOIN Items AS i ON oi.ItemId=i.Id
  GROUP BY DATEPART(DAY,o.DateTime)
  ORDER BY [DayOfMonth] 
  
  --17. Top Products
  SELECT i.Name,c.Name,SUM(oi.Quantity) AS [Count],SUM(oi.Quantity*i.Price) AS TotalPrice FROM Items AS i
  JOIN Categories AS c ON i.CategoryId=c.Id
 LEFT JOIN OrderItems AS oi ON i.Id=oi.ItemId
  GROUP BY c.Name,i.Name
  ORDER BY TotalPrice DESC,[Count] DESC
  
  --18. Promotion days
CREATE FUNCTION udf_GetPromotedProducts(@CurrentDate DATE,@StartDate DATE,@EndDate DATE,@Discount DECIMAL(15,2),@FirstItemId INT,@SecondItemId INT,@ThirdItemId INT)
	RETURNS VARCHAR(MAX)
	AS
	BEGIN
	DECLARE @firstItem INT = (SELECT Id FROM Items WHERE Id=@FirstItemId)
	DECLARE @secondItem INT = (SELECT Id FROM Items WHERE Id=@SecondItemId)
	DECLARE @thirdItem INT = (SELECT Id FROM Items WHERE Id=@ThirdItemId)
	IF(@firstItem IS NULL OR @secondItem IS NULL OR @thirdItem IS NULL)
	BEGIN
	RETURN 'One of the items does not exists!'
	END
	IF(@CurrentDate NOT BETWEEN @StartDate AND @EndDate)
	BEGIN
	RETURN 'The current date is not within the promotion dates!'
	END

	DECLARE @firstName VARCHAR(MAX) = (SELECT Name FROM Items WHERE Id=@firstItem)
	DECLARE @secondName VARCHAR(MAX) = (SELECT Name FROM Items WHERE Id=@secondItem)
	DECLARE @thirdName VARCHAR(MAX) = (SELECT Name FROM Items WHERE Id=@thirdItem)

	DECLARE @firstPrice DECIMAL(16,2) = (SELECT Price FROM Items WHERE Id=@firstItem)
	DECLARE @secondPrice DECIMAL(16,2) = (SELECT Price FROM Items WHERE Id=@secondItem)
	DECLARE @thirdPrice DECIMAL(16,2) = (SELECT Price FROM Items WHERE Id=@thirdItem)

	SET @firstPrice=@firstPrice - (@firstPrice*(@Discount/100))
	SET @secondPrice=@secondPrice - (@secondPrice*(@Discount/100))
	SET @thirdPrice=@thirdPrice - (@thirdPrice*(@Discount/100))

	RETURN @firstName+' price: '+CAST(@firstPrice AS VARCHAR(10))+' <-> '+@secondName+' price: '+CAST(@secondPrice AS VARCHAR(10))+' <-> '+@thirdName+' price: '+CAST(@thirdPrice AS VARCHAR(10))
  END

  SELECT dbo.udf_GetPromotedProducts('2018-08-02','2018-08-01','2018-08-03',13,3,4,5)

  --19 Cancel order
  CREATE PROC usp_CancelOrder(@OrderId INT,@CancelDate DATE)
  AS
  BEGIN
  DECLARE @order INT = (SELECT Id FROM Orders WHERE Id=@OrderId)

  IF(@order IS NULL)
  BEGIN
  RAISERROR('The order does not exist!',16,1)
  ROLLBACK
  RETURN
  END

  DECLARE @orderDate DATETIME = (SELECT DateTime FROM Orders WHERE Id=@OrderId)
  IF(DATEDIFF(DAY,@orderDate,@CancelDate)>=3)
  BEGIN
  RAISERROR('You cannot cancel the order!',16,1)
  ROLLBACK
  RETURN
  END

  DELETE FROM OrderItems
  WHERE OrderId=@OrderId

  DELETE FROM Orders
  WHERE Id=@OrderId

  END

  --20. Deleted Order
  CREATE TABLE DeletedOrders(
  OrderId INT IDENTITY PRIMARY KEY,
  ItemId INT NOT NULL FOREIGN KEY REFERENCES Items(Id),
  ItemQuantity INT NOT NULL)
  
  CREATE TRIGGER tr_logDeletedOrders ON OrderItems FOR DELETE
  AS
  INSERT INTO DeletedOrders (ItemId,OrderId,ItemQuantity)
  SELECT ItemId,OrderId,Quantity FROM deleted
