USE AI_Travel_Agent;
GO

-- Drop child table first if already exists
IF OBJECT_ID('dbo.DayPlans', 'U') IS NOT NULL
    DROP TABLE dbo.DayPlans;
GO

IF OBJECT_ID('dbo.TripPlans', 'U') IS NOT NULL
    DROP TABLE dbo.TripPlans;
GO

CREATE TABLE dbo.TripPlans
(
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),

    Destination NVARCHAR(150) NOT NULL,
    Days INT NOT NULL,
    BudgetLevel NVARCHAR(50) NOT NULL,
    TravelerType NVARCHAR(50) NULL,

    TripSummary NVARCHAR(MAX) NULL,
    BudgetBand NVARCHAR(100) NULL,

    Status NVARCHAR(50) NOT NULL DEFAULT 'SUCCESS',
    FallbackActivated BIT NOT NULL DEFAULT 0,

    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    UpdatedAt DATETIME2 NULL,

    CONSTRAINT CK_TripPlans_Days CHECK (Days BETWEEN 1 AND 15),
    CONSTRAINT CK_TripPlans_Status CHECK (Status IN ('SUCCESS', 'FALLBACK', 'FAILED'))
);
GO

CREATE TABLE dbo.DayPlans
(
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),

    TripPlanId UNIQUEIDENTIFIER NOT NULL,
    DayNumber INT NOT NULL,

    Morning NVARCHAR(MAX) NULL,
    Afternoon NVARCHAR(MAX) NULL,
    Evening NVARCHAR(MAX) NULL,
    Notes NVARCHAR(MAX) NULL,

    CONSTRAINT FK_DayPlans_TripPlans
        FOREIGN KEY (TripPlanId)
        REFERENCES dbo.TripPlans(Id)
        ON DELETE CASCADE,

    CONSTRAINT CK_DayPlans_DayNumber CHECK (DayNumber > 0)
);
GO

CREATE INDEX IX_DayPlans_TripPlanId
ON dbo.DayPlans(TripPlanId);
GO

CREATE UNIQUE INDEX UX_DayPlans_TripPlanId_DayNumber
ON dbo.DayPlans(TripPlanId, DayNumber);
GO

CREATE INDEX IX_TripPlans_CreatedAt
ON dbo.TripPlans(CreatedAt DESC);
GO