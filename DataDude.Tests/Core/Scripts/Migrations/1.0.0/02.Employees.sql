CREATE SCHEMA People

CREATE TABLE Employee (
	Id INT IDENTITY PRIMARY KEY,
	FirstName NVARCHAR(50) NOT NULL,
	LastName NVARCHAR(50) NOT NULL,
	FullName AS CONCAT(FirstName, ' ', LastName),
	OfficeId INT NOT NULL,
	Active BIT NOT NULL DEFAULT(1),
	CONSTRAINT FK_Employee_Office foreign key (OfficeId) references Buildings.Office(Id)
)