-- ProcessTree Database Script
-- Alternative to EF Core migrations — run this manually if needed

CREATE DATABASE ProcessTreeDb;
GO

USE ProcessTreeDb;
GO

CREATE TABLE ProcessingRules (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    Name        NVARCHAR(100) NOT NULL,
    OutputCount INT NOT NULL CHECK (OutputCount BETWEEN 1 AND 20),
    SplitMode   INT NOT NULL DEFAULT 0,
    CreatedAt   DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE ProcessItems (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    Name        NVARCHAR(100) NOT NULL,
    Weight      DECIMAL(10,2) NOT NULL CHECK (Weight > 0),
    ParentId    INT NULL REFERENCES ProcessItems(Id),
    IsProcessed BIT NOT NULL DEFAULT 0,
    CreatedAt   DATETIME NOT NULL DEFAULT GETDATE(),
    Notes       NVARCHAR(500) NULL
);

CREATE INDEX IX_ProcessItems_ParentId ON ProcessItems(ParentId);

-- Seed default rules
INSERT INTO ProcessingRules (Name, OutputCount, SplitMode, CreatedAt)
VALUES
    ('Equal Split — 2 Outputs', 2, 0, '2025-01-01'),
    ('Equal Split — 3 Outputs', 3, 0, '2025-01-01');
GO