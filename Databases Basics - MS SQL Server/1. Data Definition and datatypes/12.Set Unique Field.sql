ALTER TABLE Users
DROP CONSTRAINT PK_User_Id_Username

ALTER TABLE Users
ADD CONSTRAINT PK_Id PRIMARY KEY (Id)

ALTER TABLE Users
ADD CONSTRAINT CK_Username_Lenght CHECK (LEN(Username) >= 3)