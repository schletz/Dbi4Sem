CREATE DATABASE Weatherdata;
USE Weatherdata;

CREATE TABLE Bundesland (
    Id        INT  IDENTITY(1,1) PRIMARY KEY,
    Shortname VARCHAR(4) NOT NULL UNIQUE,
    Name      VARCHAR(255) NOT NULL
);


CREATE TABLE Station (
    Id           INT PRIMARY KEY,
    Name         VARCHAR(255) NOT NULL,
    BundeslandId INT NOT NULL,
    Lat          DECIMAL(9,6) NOT NULL,
    Lng          DECIMAL(9,6) NOT NULL,
    Alt          INT NOT NULL,
    FOREIGN KEY (BundeslandId) REFERENCES Bundesland(Id)
);

CREATE TABLE MeasurementDaily (
	Id        INT  IDENTITY(1,1) PRIMARY KEY,
	Date      DATE NOT NULL,
	StationId INT NOT NULL,
	Tavg      DECIMAL(3,1),  -- Durchschnittstemperatur pro Tag
	Tmax      DECIMAL(3,1),  -- Höchsttemperatur
	Tmin      DECIMAL(3,1),  -- Tiefsttemperatur
	FOREIGN KEY (StationId) REFERENCES Station(Id),
	UNIQUE(Date, StationId)
);

CREATE TABLE MeasurementHourly (
	Id        INT  IDENTITY(1,1) PRIMARY KEY,
	Datetime  SMALLDATETIME NOT NULL,
	StationId INT NOT NULL,
	Temp      DECIMAL(3,1) NOT NULL,  -- Temperatur
	Dewp      DECIMAL(3,1) NOT NULL,  -- Taupunkt (dew point)
	FOREIGN KEY (StationId) REFERENCES Station(Id),
	UNIQUE(Datetime, StationId)
);


INSERT INTO Bundesland (Shortname, Name) VALUES ('BGL', 'Burgenland');
INSERT INTO Bundesland (Shortname, Name) VALUES ('KNT', 'Kärnten');
INSERT INTO Bundesland (Shortname, Name) VALUES ('NOE', 'Niederösterreich');
INSERT INTO Bundesland (Shortname, Name) VALUES ('OOE', 'Oberösterreich');
INSERT INTO Bundesland (Shortname, Name) VALUES ('SAL', 'Salzburg');
INSERT INTO Bundesland (Shortname, Name) VALUES ('STMK', 'Steiermark');
INSERT INTO Bundesland (Shortname, Name) VALUES ('TIR', 'Tirol');
INSERT INTO Bundesland (Shortname, Name) VALUES ('VBG', 'Vorarlberg');
INSERT INTO Bundesland (Shortname, Name) VALUES ('WIE', 'Wien');

