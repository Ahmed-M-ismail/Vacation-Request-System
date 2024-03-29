
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 07/20/2017 11:44:50
-- Generated from EDMX file: E:\YouxelVacationRequest\DAL\YouxelVacationSystem.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [YouxelVacationSystem];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_Employee_EmployeeRole]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Employee] DROP CONSTRAINT [FK_Employee_EmployeeRole];
GO
IF OBJECT_ID(N'[dbo].[FK_EmployeeTeam_Employee]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EmployeeTeam] DROP CONSTRAINT [FK_EmployeeTeam_Employee];
GO
IF OBJECT_ID(N'[dbo].[FK_EmployeeTeam_EmployeeTeam]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EmployeeTeam] DROP CONSTRAINT [FK_EmployeeTeam_EmployeeTeam];
GO
IF OBJECT_ID(N'[dbo].[FK_Request_Employee]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Request] DROP CONSTRAINT [FK_Request_Employee];
GO
IF OBJECT_ID(N'[dbo].[FK_Request_RequestStatus]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Request] DROP CONSTRAINT [FK_Request_RequestStatus];
GO
IF OBJECT_ID(N'[dbo].[FK_Request_VacationType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Request] DROP CONSTRAINT [FK_Request_VacationType];
GO
IF OBJECT_ID(N'[dbo].[FK_RequestApproval_Employee]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RequestApproval] DROP CONSTRAINT [FK_RequestApproval_Employee];
GO
IF OBJECT_ID(N'[dbo].[FK_RequestApproval_Request]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RequestApproval] DROP CONSTRAINT [FK_RequestApproval_Request];
GO
IF OBJECT_ID(N'[dbo].[FK_RequestApproval_RequestStatus]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RequestApproval] DROP CONSTRAINT [FK_RequestApproval_RequestStatus];
GO
IF OBJECT_ID(N'[dbo].[FK_RequestApproval_Workflow]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RequestApproval] DROP CONSTRAINT [FK_RequestApproval_Workflow];
GO
IF OBJECT_ID(N'[dbo].[FK_Team_Employee]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Team] DROP CONSTRAINT [FK_Team_Employee];
GO
IF OBJECT_ID(N'[dbo].[FK_Workflow_Employee]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Workflow] DROP CONSTRAINT [FK_Workflow_Employee];
GO
IF OBJECT_ID(N'[dbo].[FK_Workflow_Team]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Workflow] DROP CONSTRAINT [FK_Workflow_Team];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Configuration]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Configuration];
GO
IF OBJECT_ID(N'[dbo].[Employee]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Employee];
GO
IF OBJECT_ID(N'[dbo].[EmployeeRole]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EmployeeRole];
GO
IF OBJECT_ID(N'[dbo].[EmployeeTeam]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EmployeeTeam];
GO
IF OBJECT_ID(N'[dbo].[Request]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Request];
GO
IF OBJECT_ID(N'[dbo].[RequestApproval]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RequestApproval];
GO
IF OBJECT_ID(N'[dbo].[RequestStatus]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RequestStatus];
GO
IF OBJECT_ID(N'[dbo].[Team]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Team];
GO
IF OBJECT_ID(N'[dbo].[VacationType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[VacationType];
GO
IF OBJECT_ID(N'[dbo].[Workflow]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Workflow];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Configuration'
CREATE TABLE [dbo].[Configuration] (
    [ID] varchar(50)  NOT NULL,
    [Value] varchar(255)  NOT NULL
);
GO

-- Creating table 'Employee'
CREATE TABLE [dbo].[Employee] (
    [ID] varchar(50)  NOT NULL,
    [FName] varchar(50)  NOT NULL,
    [LName] varchar(50)  NOT NULL,
    [JobTittle] varchar(255)  NOT NULL,
    [DateHired] datetime  NOT NULL,
    [Password] varchar(255)  NOT NULL,
    [Email] varchar(255)  NOT NULL,
    [UserRoleID] varchar(50)  NOT NULL,
    [VacationBalance] int  NOT NULL,
    [BalanceLimit] int  NULL,
    [AccountActive] bit  NOT NULL,
    [CreatedAT] datetime  NULL,
    [LastUpdated] datetime  NULL
);
GO

-- Creating table 'EmployeeRole'
CREATE TABLE [dbo].[EmployeeRole] (
    [ID] varchar(50)  NOT NULL,
    [Name] varchar(255)  NOT NULL
);
GO

-- Creating table 'EmployeeTeam'
CREATE TABLE [dbo].[EmployeeTeam] (
    [EmployeeID] varchar(50)  NOT NULL,
    [TeamID] varchar(50)  NOT NULL
);
GO

-- Creating table 'Request'
CREATE TABLE [dbo].[Request] (
    [ID] varchar(50)  NOT NULL,
    [EmployeeID] varchar(50)  NOT NULL,
    [DurationFrom] datetime  NULL,
    [DurationTo] datetime  NULL,
    [VacationTypeID] varchar(50)  NULL,
    [VacationORWorkHome] bit  NOT NULL,
    [RequestStatusID] varchar(50)  NOT NULL,
    [Comment] varchar(255)  NULL,
    [CreatedAT] datetime  NULL,
    [LastModified] datetime  NULL,
    [CountDays] int  NULL
);
GO

-- Creating table 'RequestApproval'
CREATE TABLE [dbo].[RequestApproval] (
    [ID] varchar(50)  NOT NULL,
    [WorkflowID] varchar(50)  NOT NULL,
    [RequestID] varchar(50)  NOT NULL,
    [StatusID] varchar(50)  NOT NULL,
    [Comment] varchar(255)  NULL,
    [ApprovalBy] varchar(50)  NOT NULL,
    [ApprovalAT] datetime  NULL,
    [LastModified] datetime  NULL
);
GO

-- Creating table 'RequestStatus'
CREATE TABLE [dbo].[RequestStatus] (
    [ID] varchar(50)  NOT NULL,
    [StatusName] varchar(50)  NOT NULL
);
GO

-- Creating table 'Team'
CREATE TABLE [dbo].[Team] (
    [ID] varchar(50)  NOT NULL,
    [Name] varchar(50)  NOT NULL,
    [ManagerID] varchar(50)  NOT NULL,
    [NumLevel] int  NOT NULL
);
GO

-- Creating table 'VacationType'
CREATE TABLE [dbo].[VacationType] (
    [ID] varchar(50)  NOT NULL,
    [Type] varchar(255)  NOT NULL
);
GO

-- Creating table 'Workflow'
CREATE TABLE [dbo].[Workflow] (
    [ID] varchar(50)  NOT NULL,
    [TeamID] varchar(50)  NOT NULL,
    [EmployeeID] varchar(50)  NOT NULL,
    [LevelNum] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ID] in table 'Configuration'
ALTER TABLE [dbo].[Configuration]
ADD CONSTRAINT [PK_Configuration]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Employee'
ALTER TABLE [dbo].[Employee]
ADD CONSTRAINT [PK_Employee]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'EmployeeRole'
ALTER TABLE [dbo].[EmployeeRole]
ADD CONSTRAINT [PK_EmployeeRole]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [EmployeeID], [TeamID] in table 'EmployeeTeam'
ALTER TABLE [dbo].[EmployeeTeam]
ADD CONSTRAINT [PK_EmployeeTeam]
    PRIMARY KEY CLUSTERED ([EmployeeID], [TeamID] ASC);
GO

-- Creating primary key on [ID] in table 'Request'
ALTER TABLE [dbo].[Request]
ADD CONSTRAINT [PK_Request]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'RequestApproval'
ALTER TABLE [dbo].[RequestApproval]
ADD CONSTRAINT [PK_RequestApproval]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'RequestStatus'
ALTER TABLE [dbo].[RequestStatus]
ADD CONSTRAINT [PK_RequestStatus]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Team'
ALTER TABLE [dbo].[Team]
ADD CONSTRAINT [PK_Team]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'VacationType'
ALTER TABLE [dbo].[VacationType]
ADD CONSTRAINT [PK_VacationType]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Workflow'
ALTER TABLE [dbo].[Workflow]
ADD CONSTRAINT [PK_Workflow]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [UserRoleID] in table 'Employee'
ALTER TABLE [dbo].[Employee]
ADD CONSTRAINT [FK_Employee_EmployeeRole]
    FOREIGN KEY ([UserRoleID])
    REFERENCES [dbo].[EmployeeRole]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Employee_EmployeeRole'
CREATE INDEX [IX_FK_Employee_EmployeeRole]
ON [dbo].[Employee]
    ([UserRoleID]);
GO

-- Creating foreign key on [EmployeeID] in table 'EmployeeTeam'
ALTER TABLE [dbo].[EmployeeTeam]
ADD CONSTRAINT [FK_EmployeeTeam_Employee]
    FOREIGN KEY ([EmployeeID])
    REFERENCES [dbo].[Employee]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [EmployeeID] in table 'Request'
ALTER TABLE [dbo].[Request]
ADD CONSTRAINT [FK_Request_Employee]
    FOREIGN KEY ([EmployeeID])
    REFERENCES [dbo].[Employee]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Request_Employee'
CREATE INDEX [IX_FK_Request_Employee]
ON [dbo].[Request]
    ([EmployeeID]);
GO

-- Creating foreign key on [ApprovalBy] in table 'RequestApproval'
ALTER TABLE [dbo].[RequestApproval]
ADD CONSTRAINT [FK_RequestApproval_Employee]
    FOREIGN KEY ([ApprovalBy])
    REFERENCES [dbo].[Employee]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RequestApproval_Employee'
CREATE INDEX [IX_FK_RequestApproval_Employee]
ON [dbo].[RequestApproval]
    ([ApprovalBy]);
GO

-- Creating foreign key on [ManagerID] in table 'Team'
ALTER TABLE [dbo].[Team]
ADD CONSTRAINT [FK_Team_Employee]
    FOREIGN KEY ([ManagerID])
    REFERENCES [dbo].[Employee]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Team_Employee'
CREATE INDEX [IX_FK_Team_Employee]
ON [dbo].[Team]
    ([ManagerID]);
GO

-- Creating foreign key on [EmployeeID] in table 'Workflow'
ALTER TABLE [dbo].[Workflow]
ADD CONSTRAINT [FK_Workflow_Employee]
    FOREIGN KEY ([EmployeeID])
    REFERENCES [dbo].[Employee]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Workflow_Employee'
CREATE INDEX [IX_FK_Workflow_Employee]
ON [dbo].[Workflow]
    ([EmployeeID]);
GO

-- Creating foreign key on [EmployeeID], [TeamID] in table 'EmployeeTeam'
ALTER TABLE [dbo].[EmployeeTeam]
ADD CONSTRAINT [FK_EmployeeTeam_EmployeeTeam]
    FOREIGN KEY ([EmployeeID], [TeamID])
    REFERENCES [dbo].[EmployeeTeam]
        ([EmployeeID], [TeamID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [RequestStatusID] in table 'Request'
ALTER TABLE [dbo].[Request]
ADD CONSTRAINT [FK_Request_RequestStatus]
    FOREIGN KEY ([RequestStatusID])
    REFERENCES [dbo].[RequestStatus]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Request_RequestStatus'
CREATE INDEX [IX_FK_Request_RequestStatus]
ON [dbo].[Request]
    ([RequestStatusID]);
GO

-- Creating foreign key on [VacationTypeID] in table 'Request'
ALTER TABLE [dbo].[Request]
ADD CONSTRAINT [FK_Request_VacationType]
    FOREIGN KEY ([VacationTypeID])
    REFERENCES [dbo].[VacationType]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Request_VacationType'
CREATE INDEX [IX_FK_Request_VacationType]
ON [dbo].[Request]
    ([VacationTypeID]);
GO

-- Creating foreign key on [RequestID] in table 'RequestApproval'
ALTER TABLE [dbo].[RequestApproval]
ADD CONSTRAINT [FK_RequestApproval_Request]
    FOREIGN KEY ([RequestID])
    REFERENCES [dbo].[Request]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RequestApproval_Request'
CREATE INDEX [IX_FK_RequestApproval_Request]
ON [dbo].[RequestApproval]
    ([RequestID]);
GO

-- Creating foreign key on [StatusID] in table 'RequestApproval'
ALTER TABLE [dbo].[RequestApproval]
ADD CONSTRAINT [FK_RequestApproval_RequestStatus]
    FOREIGN KEY ([StatusID])
    REFERENCES [dbo].[RequestStatus]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RequestApproval_RequestStatus'
CREATE INDEX [IX_FK_RequestApproval_RequestStatus]
ON [dbo].[RequestApproval]
    ([StatusID]);
GO

-- Creating foreign key on [WorkflowID] in table 'RequestApproval'
ALTER TABLE [dbo].[RequestApproval]
ADD CONSTRAINT [FK_RequestApproval_Workflow]
    FOREIGN KEY ([WorkflowID])
    REFERENCES [dbo].[Workflow]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RequestApproval_Workflow'
CREATE INDEX [IX_FK_RequestApproval_Workflow]
ON [dbo].[RequestApproval]
    ([WorkflowID]);
GO

-- Creating foreign key on [TeamID] in table 'Workflow'
ALTER TABLE [dbo].[Workflow]
ADD CONSTRAINT [FK_Workflow_Team]
    FOREIGN KEY ([TeamID])
    REFERENCES [dbo].[Team]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Workflow_Team'
CREATE INDEX [IX_FK_Workflow_Team]
ON [dbo].[Workflow]
    ([TeamID]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------