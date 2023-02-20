CREATE SCHEMA People
GO

CREATE SCHEMA Buildings
GO

CREATE TABLE Buildings.Office (
	Id INT IDENTITY PRIMARY KEY,
	Name NVARCHAR(50) NOT NULL
)

CREATE TABLE People.Employee (
	Id INT IDENTITY PRIMARY KEY,
	FirstName NVARCHAR(50) NOT NULL,
	LastName NVARCHAR(50) NOT NULL,
	FullName AS CONCAT(FirstName, ' ', LastName),
	Active BIT NOT NULL DEFAULT(1),
	CreatedAt DATETIME DEFAULT(GETDATE()),
	UpdatedAt DATETIME NULL,
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
GO

CREATE TABLE Buildings.OfficeExtension (
	Id INT IDENTITY PRIMARY KEY,
	ExtensionsNumber NVARCHAR(50) NOT NULL,
	CONSTRAINT FK_OfficeExtension_Office FOREIGN KEY (Id) references Buildings.Office(Id),
)
GO

CREATE TABLE Test_Generated_PK_Scenario_1(
	Id1 INT NOT NULL,
	Id2 INT NOT NULL,
	CONSTRAINT PK_Test_PK_Scenarios_1 PRIMARY KEY (Id1, Id2),
)
GO

CREATE TABLE Test_Generated_PK_Scenario_2(
	Id1 INT NOT NULL,
	Id2 UNIQUEIDENTIFIER DEFAULT(newid()),
	CONSTRAINT PK_Test_PK_Scenarios_2 PRIMARY KEY (Id1, Id2),
)
GO

CREATE TABLE Test_PK_Less_Scenario(
	Id1 INT NOT NULL,
	Id2 INT NOT NULL,
)
GO

CREATE TABLE Test_PK_Sequential_Uuid(
	Id UNIQUEIDENTIFIER PRIMARY KEY NOT NULL DEFAULT(newsequentialid()),
)
GO

CREATE TABLE Test_Nullable_FK(
	Id INT PRIMARY KEY,
	OfficeId INT NULL,
	CONSTRAINT FK_Test_Nullable_FK_Office FOREIGN KEY (OfficeId) references Buildings.Office(Id),
)
GO

CREATE TABLE Test_Nullable_Text_Data_Type(
	Id INT PRIMARY KEY,
	Text TEXT NULL,
)
GO

CREATE TABLE Test_Geography_Data(
	Id INT IDENTITY PRIMARY KEY,
	Position GEOGRAPHY NULL,
)
GO

CREATE TRIGGER People.EmployeeUpdatedAt ON People.Employee AFTER UPDATE AS BEGIN
	UPDATE People.Employee 
		SET People.Employee.UpdatedAt = GETDATE()
	WHERE People.Employee.Id IN (SELECT Id FROM inserted)
END