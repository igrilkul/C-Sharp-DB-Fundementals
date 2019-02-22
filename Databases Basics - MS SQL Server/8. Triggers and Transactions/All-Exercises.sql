--14. Create Table Logs
CREATE TABLE Logs(
LogId INT IDENTITY PRIMARY KEY,
AccountId INT NOT NULL,
OldSum DECIMAL(18,2) NOT NULL,
NewSum DECIMAL(18,2) NOT NULL)

ALTER TABLE Logs
ADD CONSTRAINT Fk_Logs_Account FOREIGN KEY(AccountId) REFERENCES Accounts(Id)

CREATE TRIGGER tr_LogsUpdate ON Accounts FOR UPDATE
AS
DECLARE @newSum DECIMAL(18,2) = (SELECT Balance FROM inserted)
DECLARE @oldSum DECIMAL(18,2) = (SELECT Balance FROM deleted)
DECLARE @accountId INT = (SELECT Id FROM inserted)

IF(@newSum IS NULL)
BEGIN
RAISERROR('Balance cannot be empty.',16,1)
ROLLBACK
RETURN
END
INSERT INTO Logs(AccountId,OldSum,NewSum) VALUES
(@accountId,@oldSum,@newSum)


UPDATE Accounts
SET Balance +=10
WHERE Id=1

--15. Create Table Emails
CREATE TABLE NotificationEmails(
Id INT IDENTITY PRIMARY KEY,
Recipient INT NOT NULL FOREIGN KEY REFERENCES Accounts(Id),
Subject NVARCHAR(150) NOT NULL,
Body NVARCHAR(MAX) NOT NULL
)

CREATE TRIGGER tr_InsertIntoEmail ON Logs FOR INSERT
AS
DECLARE @newSum DECIMAL(18,2) = (SELECT newSum FROM inserted)
DECLARE @oldSum DECIMAL(18,2) = (SELECT oldSum FROM inserted)
DECLARE @accountId INT = (SELECT accountId FROM inserted)

INSERT INTO NotificationEmails(Recipient,Subject,Body) VALUES
(@accountId,
'Balance change for account: '+CAST(@accountId AS varchar),
'On '+CONVERT(varchar,GETDATE(),103)+' your balance was changed from '+CAST(@oldSum AS varchar)+' to '+CAST(@newSum AS varchar)+'.'
)

--16. Deposit Money
CREATE PROC usp_DepositMoney (@AccountId INT, @MoneyAmount DECIMAL(18,4))
AS
IF(@MoneyAmount<0)
BEGIN
RAISERROR('Money must be positive!',16,1)
ROLLBACK
RETURN
END
UPDATE Accounts
SET Balance+=@MoneyAmount
WHERE Id=@AccountId

--17. Withdraw Money
CREATE PROC usp_WithdrawMoney (@AccountId INT, @MoneyAmount DECIMAL(18,4))
AS
IF(@MoneyAmount<0)
BEGIN
RAISERROR('Money amount cannot be negative!',16,1)
ROLLBACK
RETURN
END
UPDATE Accounts
SET Balance-=@MoneyAmount
WHERE Id=@AccountId

--18. Money transfer
CREATE PROC usp_TransferMoney(@SenderId INT, @ReceiverId INT, @Amount DECIMAL(18,4))
AS
BEGIN TRANSACTION
EXEC dbo.usp_WithdrawMoney @SenderId,@Amount
EXEC dbo.usp_DepositMoney @ReceiverId,@Amount
COMMIT

--19. Trigger
--1)
CREATE TRIGGER tr_StopBuyingItemsHigherLevel ON UserGameItems FOR INSERT
AS
DECLARE @itemId INT = (SELECT ItemId FROM inserted)
DECLARE @userGameId INT = (SELECT UserGameId FROM inserted)

DECLARE @itemLevel INT = 
(SELECT i.MinLevel FROM inserted AS ug
JOIN Items AS i ON i.Id=ug.ItemId)

DECLARE @userLevel INT = 
(SELECT ug.Level FROM inserted AS ugi
JOIN UsersGames AS ug ON ugi.UserGameId=ug.Id)

IF(@userLevel<@itemLevel)
BEGIN
RAISERROR('Level does not meet requirement.',16,1)
ROLLBACK
RETURN
END
INSERT INTO UserGameItems (ItemId,UserGameId) VALUES
(@itemId,@userGameId)

--2)
UPDATE UsersGames
SET Cash+=50000
WHERE GameId = (SELECT Id FROM Games WHERE Name = 'Bali')
AND UserId = (SELECT Id FROM USERS WHERE Username IN ('baleremuda', 'loosenoise', 'inguinalself', 'buildingdeltoid', 'monoxidecos'))

--3)
SELECT u.Id,i.Id FROM Users AS u
CROSS JOIN Items AS i
WHERE (Username IN ('baleremuda', 'loosenoise', 'inguinalself', 'buildingdeltoid', 'monoxidecos')) AND (i.Id>=251 AND i.Id<=299) AND (i.Id >= 501 AND i.Id<=539)

UPDATE UserGameItems
SELECT Id FROM Items 
WHERE Id BETWEEN 251 AND 299 INTO UserGameItems 

--20. Massive shopping *
CREATE PROC pr_PurchaseItems (@minLevel INT,@maxLevel INT,@username VARCHAR,@gameName VARCHAR)
AS
DECLARE @cash DECIMAL(16,2)= 
(SELECT ug.Cash FROM Users AS u 
JOIN UsersGames AS ug ON ug.UserId=u.Id
WHERE u.Username=@username)
DECLARE @totalPrice DECIMAL(16,2) = (SELECT SUM(Price) FROM Items WHERE MinLevel BETWEEN @minLevel AND @maxLevel)

BEGIN TRANSACTION
IF(@totalPrice>@cash)
BEGIN
RAISERROR('Not enough cash!',16,1)
ROLLBACK
END
UPDATE UsersGames
SET Cash-=@totalPrice 
WHERE UserId = (SELECT Id FROM Users WHERE Username = @username)
AND GameId = (SELECT Id From Games WHERE Name = @gameName)

COMMIT
