IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'ItemHierarchyDB')
BEGIN
    CREATE DATABASE ItemHierarchyDB;
END
GO

USE ItemHierarchyDB;
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users
    (
        UserId      INT IDENTITY(1,1) PRIMARY KEY,
        Email       NVARCHAR(100) NOT NULL UNIQUE,
        Password    NVARCHAR(255) NOT NULL
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Items')
BEGIN
    CREATE TABLE Items
    (
        ItemId        INT IDENTITY(1,1) PRIMARY KEY,
        ItemName      NVARCHAR(200) NOT NULL,
        Weight        DECIMAL(18,2) NOT NULL CHECK (Weight > 0),
        ParentItemId  INT NULL,
        IsProcessed   BIT NOT NULL DEFAULT 0,
        CreatedDate   DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_Items_ParentItem FOREIGN KEY (ParentItemId) 
            REFERENCES Items(ItemId)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'admin@example.com')
BEGIN
    INSERT INTO Users (Email, Password)
    VALUES ('admin@example.com', 'admin123');
END
GO

IF NOT EXISTS (SELECT 1 FROM Items WHERE ItemName = 'Root Item A')
BEGIN
    INSERT INTO Items (ItemName, Weight, ParentItemId, IsProcessed, CreatedDate)
    VALUES ('Root Item A', 100.00, NULL, 0, GETDATE());

    INSERT INTO Items (ItemName, Weight, ParentItemId, IsProcessed, CreatedDate)
    VALUES ('Root Item B', 200.00, NULL, 0, GETDATE());

    INSERT INTO Items (ItemName, Weight, ParentItemId, IsProcessed, CreatedDate)
    VALUES ('Root Item C', 150.00, NULL, 0, GETDATE());
END
GO

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_ValidateUser')
    DROP PROCEDURE sp_ValidateUser;
GO

CREATE PROCEDURE sp_ValidateUser
    @Email NVARCHAR(100),
    @Password NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UserId, Email
    FROM Users
    WHERE Email = @Email AND Password = @Password;
END
GO

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetAllItems')
    DROP PROCEDURE sp_GetAllItems;
GO

CREATE PROCEDURE sp_GetAllItems
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ItemId, ItemName, Weight, ParentItemId, IsProcessed, CreatedDate
    FROM Items
    ORDER BY CreatedDate DESC;
END
GO

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetItemById')
    DROP PROCEDURE sp_GetItemById;
GO

CREATE PROCEDURE sp_GetItemById
    @ItemId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ItemId, ItemName, Weight, ParentItemId, IsProcessed, CreatedDate
    FROM Items
    WHERE ItemId = @ItemId;
END
GO

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_AddItem')
    DROP PROCEDURE sp_AddItem;
GO

CREATE PROCEDURE sp_AddItem
    @ItemName NVARCHAR(200),
    @Weight DECIMAL(18,2),
    @ParentItemId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Items (ItemName, Weight, ParentItemId, IsProcessed, CreatedDate)
    VALUES (@ItemName, @Weight, @ParentItemId, 0, GETDATE());

    SELECT SCOPE_IDENTITY() AS NewItemId;
END
GO

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_UpdateItem')
    DROP PROCEDURE sp_UpdateItem;
GO

CREATE PROCEDURE sp_UpdateItem
    @ItemId INT,
    @ItemName NVARCHAR(200),
    @Weight DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Items
    SET ItemName = @ItemName,
        Weight = @Weight
    WHERE ItemId = @ItemId;
END
GO

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_DeleteItem')
    DROP PROCEDURE sp_DeleteItem;
GO

CREATE PROCEDURE sp_DeleteItem
    @ItemId INT
AS
BEGIN
    SET NOCOUNT ON;
    ;WITH ChildItems AS
    (
        SELECT ItemId FROM Items WHERE ParentItemId = @ItemId
        UNION ALL
        SELECT i.ItemId FROM Items i
        INNER JOIN ChildItems ci ON i.ParentItemId = ci.ItemId
    )
    DELETE FROM Items WHERE ItemId IN (SELECT ItemId FROM ChildItems);

    DELETE FROM Items WHERE ItemId = @ItemId;
END
GO

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_SearchItems')
    DROP PROCEDURE sp_SearchItems;
GO

CREATE PROCEDURE sp_SearchItems
    @SearchTerm NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ItemId, ItemName, Weight, ParentItemId, IsProcessed, CreatedDate
    FROM Items
    WHERE ItemName LIKE '%' + @SearchTerm + '%'
    ORDER BY CreatedDate DESC;
END
GO

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetChildItems')
    DROP PROCEDURE sp_GetChildItems;
GO

CREATE PROCEDURE sp_GetChildItems
    @ParentItemId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ItemId, ItemName, Weight, ParentItemId, IsProcessed, CreatedDate
    FROM Items
    WHERE ParentItemId = @ParentItemId
    ORDER BY CreatedDate;
END
GO

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_MarkAsProcessed')
    DROP PROCEDURE sp_MarkAsProcessed;
GO

CREATE PROCEDURE sp_MarkAsProcessed
    @ItemId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Items SET IsProcessed = 1 WHERE ItemId = @ItemId;
END
GO

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetProcessedItems')
    DROP PROCEDURE sp_GetProcessedItems;
GO

CREATE PROCEDURE sp_GetProcessedItems
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ItemId, ItemName, Weight, ParentItemId, IsProcessed, CreatedDate
    FROM Items
    WHERE IsProcessed = 1
    ORDER BY CreatedDate DESC;
END
GO

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetAllItemsForTree')
    DROP PROCEDURE sp_GetAllItemsForTree;
GO

CREATE PROCEDURE sp_GetAllItemsForTree
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ItemId, ItemName, Weight, ParentItemId, IsProcessed, CreatedDate
    FROM Items
    ORDER BY ParentItemId, ItemName;
END
GO

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetChildWeightSum')
    DROP PROCEDURE sp_GetChildWeightSum;
GO

CREATE PROCEDURE sp_GetChildWeightSum
    @ParentItemId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ISNULL(SUM(Weight), 0) AS TotalChildWeight
    FROM Items
    WHERE ParentItemId = @ParentItemId;
END
GO
