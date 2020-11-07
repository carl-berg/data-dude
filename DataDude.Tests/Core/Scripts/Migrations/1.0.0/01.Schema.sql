﻿CREATE SCHEMA People
GO

CREATE SCHEMA Buildings
GO

CREATE TABLE Buildings.Office (
	Id INT IDENTITY PRIMARY KEY,
	Name NVARCHAR(50)
)

CREATE TABLE People.Employee (
	Id INT IDENTITY PRIMARY KEY,
	FirstName NVARCHAR(50) NOT NULL,
	LastName NVARCHAR(50) NOT NULL,
	FullName AS CONCAT(FirstName, ' ', LastName),
	Active BIT NOT NULL DEFAULT(1),
)

CREATE TABLE Buildings.OfficeOccupant (
	OfficeId INT NOT NULL,
	EmployeeId INT NOT NULL,
	CONSTRAINT PK_OfficeOccupant PRIMARY KEY (OfficeId, EmployeeId),
	CONSTRAINT FK_OfficeOccupant_Office FOREIGN KEY (OfficeId) references Buildings.Office(Id),
	CONSTRAINT FK_OfficeOccupant_Employee FOREIGN KEY (EmployeeId) references People.Employee(Id)
)

CREATE TABLE People.OfficeOccupancy (
	Id INT IDENTITY PRIMARY KEY,
	OfficeId INT NOT NULL,
	EmployeeId INT NOT NULL,
	StartDate DATE NOT NULL,
	EndDate DATE NULL,
	CONSTRAINT FK_OfficeOccupancy_OfficeOccupant FOREIGN KEY (OfficeId, EmployeeId) REFERENCES Buildings.OfficeOccupant(OfficeId, EmployeeId)
)

