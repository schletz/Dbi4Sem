USE tempdb;
GO    
BEGIN
    DECLARE @DBNAME AS VARCHAR(MAX) = 'Webhoster'
    IF EXISTS(SELECT * FROM sys.databases WHERE Name = @DBNAME)
    BEGIN
        -- Disconnect all users and recreate database.
        EXEC('ALTER DATABASE ' + @DBNAME + ' SET SINGLE_USER WITH ROLLBACK IMMEDIATE');
        EXEC('DROP DATABASE ' + @DBNAME);
    END;
    EXEC('CREATE DATABASE ' + @DBNAME);    
END;
USE Webhoster;
GO

CREATE TABLE Domain (
    DomainID   INTEGER PRIMARY KEY,
    DomainName VARCHAR(45),
    Country    VARCHAR(10),
    [Use]      CHAR(1)
);

CREATE TABLE District (
    DistrictID   INTEGER PRIMARY KEY,
    DistrictName VARCHAR(45)
);

CREATE TABLE City (
    CityID     INTEGER PRIMARY KEY,
    ZIP        INTEGER,
    CityName   VARCHAR(45),
    DistrictID INTEGER NOT NULL,
    FOREIGN KEY (DistrictID) REFERENCES District (DistrictID)
);

CREATE TABLE Customer (
    CustomerID INTEGER PRIMARY KEY,
    FirstName  VARCHAR(45),
    LastName   VARCHAR(45),
    Gender     CHAR(1),
    AgeGroup   VARCHAR(45),
    Age        INTEGER,
    CityID     INTEGER NOT NULL,
    FOREIGN KEY (CityID) REFERENCES City (CityID)
);

CREATE TABLE Package (
    PackageID   INTEGER PRIMARY KEY,
    PackageName VARCHAR(45),
    PackageFrom DATE,
    PackageTo   DATE,
    Business    VARCHAR(45),
    [Database]  CHAR(1),
    Shell       CHAR(1)
);

CREATE TABLE Usage_Fact (
    UsageDate   DATE NOT NULL,
    DomainID    INTEGER NOT NULL,
    CustomerID  INTEGER NOT NULL,
    PackageID   INTEGER NOT NULL,
    UploadMB    DECIMAL(12,2),
    DownloadMB  DECIMAL(12,2),
    DiskspaceMB DECIMAL(12,2),
    UsageMB     DECIMAL(12,2),
    PRIMARY KEY (UsageDate, DomainID, CustomerID, PackageID),
    FOREIGN KEY (DomainID) REFERENCES Domain (DomainID),
    FOREIGN KEY (CustomerID) REFERENCES Customer (CustomerID),
    FOREIGN KEY (PackageID) REFERENCES Package (PackageID)
);

CREATE TABLE Payed (
    PayedID INTEGER PRIMARY KEY (PayedID),
    State   VARCHAR(45)
);

CREATE TABLE Bill_Fact (
    BillDate   DATE NOT NULL,
    CustomerID INTEGER NOT NULL,
    DomainID   INTEGER NOT NULL,
    PayedID    INTEGER NOT NULL,
    Price      DECIMAL(12,2),
    Discount   DECIMAL(12,2),
    PRIMARY KEY (BillDate, CustomerID, DomainID, PayedID),
    FOREIGN KEY (CustomerID) REFERENCES Customer (CustomerID),
    FOREIGN KEY (DomainID) REFERENCES Domain (DomainID),
    FOREIGN KEY (PayedID) REFERENCES Payed (PayedID)
);
