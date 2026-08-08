IF COL_LENGTH('dbo.Users', 'IsDeveloper') IS NULL
BEGIN
    ALTER TABLE dbo.Users
    ADD IsDeveloper bit NOT NULL
        CONSTRAINT DF_Users_IsDeveloper DEFAULT (0);
END
GO

IF OBJECT_ID('dbo.AdminMenuSettings', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.AdminMenuSettings
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_AdminMenuSettings PRIMARY KEY,
        MenuKey nvarchar(120) NOT NULL,
        Label nvarchar(150) NOT NULL,
        IsEnabled bit NOT NULL CONSTRAINT DF_AdminMenuSettings_IsEnabled DEFAULT (1)
    );

    CREATE UNIQUE INDEX IX_AdminMenuSettings_MenuKey
    ON dbo.AdminMenuSettings(MenuKey);
END
GO
