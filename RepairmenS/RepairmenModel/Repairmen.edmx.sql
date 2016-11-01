
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 08/21/2014 10:40:16
-- Generated from EDMX file: C:\Users\Emina\Desktop\RepairmenS\RepairmenModel\Repairmen.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [repairmen];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_CategoryAd]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Ads] DROP CONSTRAINT [FK_CategoryAd];
GO
IF OBJECT_ID(N'[dbo].[FK_AdUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Ads] DROP CONSTRAINT [FK_AdUser];
GO
IF OBJECT_ID(N'[dbo].[FK_UserRole]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Users] DROP CONSTRAINT [FK_UserRole];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Ads]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Ads];
GO
IF OBJECT_ID(N'[dbo].[Categories]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Categories];
GO
IF OBJECT_ID(N'[dbo].[Roles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Roles];
GO
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Ads'
CREATE TABLE [dbo].[Ads] (
    [Name] nvarchar(50)  NOT NULL,
    [Description] nvarchar(300)  NOT NULL,
    [Location] nvarchar(50)  NULL,
    [PhoneNumber] varchar(15)  NULL,
    [Email] varchar(30)  NULL,
    [CategoryId] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [Id] uniqueidentifier default newsequentialid() NOT NULL
);
GO

-- Creating table 'Categories'
CREATE TABLE [dbo].[Categories] (
    [CatName] nvarchar(30)  NOT NULL,
    [Id] uniqueidentifier default newsequentialid() NOT NULL
);
GO

-- Creating table 'Roles'
CREATE TABLE [dbo].[Roles] (
    [Name] nvarchar(30)  NOT NULL,
    [Id] uniqueidentifier default newsequentialid() NOT NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Username] nvarchar(30)  NOT NULL,
    [Password] nchar(128)  NOT NULL,
    [FirstName] nvarchar(20)  NOT NULL,
    [LastName] nvarchar(20)  NOT NULL,
    [Email] varchar(30)  NOT NULL,
    [RoleId] uniqueidentifier  NOT NULL,
    [Id] uniqueidentifier default newsequentialid() NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Ads'
ALTER TABLE [dbo].[Ads]
ADD CONSTRAINT [PK_Ads]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Categories'
ALTER TABLE [dbo].[Categories]
ADD CONSTRAINT [PK_Categories]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Roles'
ALTER TABLE [dbo].[Roles]
ADD CONSTRAINT [PK_Roles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [CategoryId] in table 'Ads'
ALTER TABLE [dbo].[Ads]
ADD CONSTRAINT [FK_CategoryAd]
    FOREIGN KEY ([CategoryId])
    REFERENCES [dbo].[Categories]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CategoryAd'
CREATE INDEX [IX_FK_CategoryAd]
ON [dbo].[Ads]
    ([CategoryId]);
GO

-- Creating foreign key on [UserId] in table 'Ads'
ALTER TABLE [dbo].[Ads]
ADD CONSTRAINT [FK_AdUser]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AdUser'
CREATE INDEX [IX_FK_AdUser]
ON [dbo].[Ads]
    ([UserId]);
GO

-- Creating foreign key on [RoleId] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [FK_UserRole]
    FOREIGN KEY ([RoleId])
    REFERENCES [dbo].[Roles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserRole'
CREATE INDEX [IX_FK_UserRole]
ON [dbo].[Users]
    ([RoleId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------