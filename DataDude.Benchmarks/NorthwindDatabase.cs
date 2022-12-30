namespace Dude.Benchmarks
{
    public static class NorthwindDatabase
    {
        public const string Schema = """
                CREATE TABLE "Employees" (
                	"EmployeeID" "int" IDENTITY (1, 1) NOT NULL ,
                	"LastName" nvarchar (20) NOT NULL ,
                	"FirstName" nvarchar (10) NOT NULL ,
                	"Title" nvarchar (30) NULL ,
                	"TitleOfCourtesy" nvarchar (25) NULL ,
                	"BirthDate" "datetime" NULL ,
                	"HireDate" "datetime" NULL ,
                	"Address" nvarchar (60) NULL ,
                	"City" nvarchar (15) NULL ,
                	"Region" nvarchar (15) NULL ,
                	"PostalCode" nvarchar (10) NULL ,
                	"Country" nvarchar (15) NULL ,
                	"HomePhone" nvarchar (24) NULL ,
                	"Extension" nvarchar (4) NULL ,
                	"Photo" "image" NULL ,
                	"Notes" "ntext" NULL ,
                	"ReportsTo" "int" NULL ,
                	"PhotoPath" nvarchar (255) NULL ,
                	CONSTRAINT "PK_Employees" PRIMARY KEY  CLUSTERED 
                	(
                		"EmployeeID"
                	),
                	CONSTRAINT "FK_Employees_Employees" FOREIGN KEY 
                	(
                		"ReportsTo"
                	) REFERENCES "dbo"."Employees" (
                		"EmployeeID"
                	),
                	CONSTRAINT "CK_Birthdate" CHECK (BirthDate < getdate())
                )
                 CREATE  INDEX "LastName" ON "dbo"."Employees"("LastName")
                 CREATE  INDEX "PostalCode" ON "dbo"."Employees"("PostalCode")

                CREATE TABLE "Categories" (
                	"CategoryID" "int" IDENTITY (1, 1) NOT NULL ,
                	"CategoryName" nvarchar (15) NOT NULL ,
                	"Description" "ntext" NULL ,
                	"Picture" "image" NULL ,
                	CONSTRAINT "PK_Categories" PRIMARY KEY  CLUSTERED 
                	(
                		"CategoryID"
                	)
                )
                 CREATE  INDEX "CategoryName" ON "dbo"."Categories"("CategoryName")

                CREATE TABLE "Customers" (
                	"CustomerID" nchar (5) NOT NULL ,
                	"CompanyName" nvarchar (40) NOT NULL ,
                	"ContactName" nvarchar (30) NULL ,
                	"ContactTitle" nvarchar (30) NULL ,
                	"Address" nvarchar (60) NULL ,
                	"City" nvarchar (15) NULL ,
                	"Region" nvarchar (15) NULL ,
                	"PostalCode" nvarchar (10) NULL ,
                	"Country" nvarchar (15) NULL ,
                	"Phone" nvarchar (24) NULL ,
                	"Fax" nvarchar (24) NULL ,
                	CONSTRAINT "PK_Customers" PRIMARY KEY  CLUSTERED 
                	(
                		"CustomerID"
                	)
                )
                 CREATE  INDEX "City" ON "dbo"."Customers"("City")
                 CREATE  INDEX "CompanyName" ON "dbo"."Customers"("CompanyName")
                 CREATE  INDEX "PostalCode" ON "dbo"."Customers"("PostalCode")
                 CREATE  INDEX "Region" ON "dbo"."Customers"("Region")

                CREATE TABLE "Shippers" (
                	"ShipperID" "int" IDENTITY (1, 1) NOT NULL ,
                	"CompanyName" nvarchar (40) NOT NULL ,
                	"Phone" nvarchar (24) NULL ,
                	CONSTRAINT "PK_Shippers" PRIMARY KEY  CLUSTERED 
                	(
                		"ShipperID"
                	)
                )

                CREATE TABLE "Suppliers" (
                	"SupplierID" "int" IDENTITY (1, 1) NOT NULL ,
                	"CompanyName" nvarchar (40) NOT NULL ,
                	"ContactName" nvarchar (30) NULL ,
                	"ContactTitle" nvarchar (30) NULL ,
                	"Address" nvarchar (60) NULL ,
                	"City" nvarchar (15) NULL ,
                	"Region" nvarchar (15) NULL ,
                	"PostalCode" nvarchar (10) NULL ,
                	"Country" nvarchar (15) NULL ,
                	"Phone" nvarchar (24) NULL ,
                	"Fax" nvarchar (24) NULL ,
                	"HomePage" "ntext" NULL ,
                	CONSTRAINT "PK_Suppliers" PRIMARY KEY  CLUSTERED 
                	(
                		"SupplierID"
                	)
                )
                 CREATE  INDEX "CompanyName" ON "dbo"."Suppliers"("CompanyName")
                 CREATE  INDEX "PostalCode" ON "dbo"."Suppliers"("PostalCode")

                CREATE TABLE "Orders" (
                	"OrderID" "int" IDENTITY (1, 1) NOT NULL ,
                	"CustomerID" nchar (5) NULL ,
                	"EmployeeID" "int" NULL ,
                	"OrderDate" "datetime" NULL ,
                	"RequiredDate" "datetime" NULL ,
                	"ShippedDate" "datetime" NULL ,
                	"ShipVia" "int" NULL ,
                	"Freight" "money" NULL CONSTRAINT "DF_Orders_Freight" DEFAULT (0),
                	"ShipName" nvarchar (40) NULL ,
                	"ShipAddress" nvarchar (60) NULL ,
                	"ShipCity" nvarchar (15) NULL ,
                	"ShipRegion" nvarchar (15) NULL ,
                	"ShipPostalCode" nvarchar (10) NULL ,
                	"ShipCountry" nvarchar (15) NULL ,
                	CONSTRAINT "PK_Orders" PRIMARY KEY  CLUSTERED 
                	(
                		"OrderID"
                	),
                	CONSTRAINT "FK_Orders_Customers" FOREIGN KEY 
                	(
                		"CustomerID"
                	) REFERENCES "dbo"."Customers" (
                		"CustomerID"
                	),
                	CONSTRAINT "FK_Orders_Employees" FOREIGN KEY 
                	(
                		"EmployeeID"
                	) REFERENCES "dbo"."Employees" (
                		"EmployeeID"
                	),
                	CONSTRAINT "FK_Orders_Shippers" FOREIGN KEY 
                	(
                		"ShipVia"
                	) REFERENCES "dbo"."Shippers" (
                		"ShipperID"
                	)
                )
                 CREATE  INDEX "CustomerID" ON "dbo"."Orders"("CustomerID")
                 CREATE  INDEX "CustomersOrders" ON "dbo"."Orders"("CustomerID")
                 CREATE  INDEX "EmployeeID" ON "dbo"."Orders"("EmployeeID")
                 CREATE  INDEX "EmployeesOrders" ON "dbo"."Orders"("EmployeeID")
                 CREATE  INDEX "OrderDate" ON "dbo"."Orders"("OrderDate")
                 CREATE  INDEX "ShippedDate" ON "dbo"."Orders"("ShippedDate")
                 CREATE  INDEX "ShippersOrders" ON "dbo"."Orders"("ShipVia")
                 CREATE  INDEX "ShipPostalCode" ON "dbo"."Orders"("ShipPostalCode")

                CREATE TABLE "Products" (
                	"ProductID" "int" IDENTITY (1, 1) NOT NULL ,
                	"ProductName" nvarchar (40) NOT NULL ,
                	"SupplierID" "int" NULL ,
                	"CategoryID" "int" NULL ,
                	"QuantityPerUnit" nvarchar (20) NULL ,
                	"UnitPrice" "money" NULL CONSTRAINT "DF_Products_UnitPrice" DEFAULT (0),
                	"UnitsInStock" "smallint" NULL CONSTRAINT "DF_Products_UnitsInStock" DEFAULT (0),
                	"UnitsOnOrder" "smallint" NULL CONSTRAINT "DF_Products_UnitsOnOrder" DEFAULT (0),
                	"ReorderLevel" "smallint" NULL CONSTRAINT "DF_Products_ReorderLevel" DEFAULT (0),
                	"Discontinued" "bit" NOT NULL CONSTRAINT "DF_Products_Discontinued" DEFAULT (0),
                	CONSTRAINT "PK_Products" PRIMARY KEY  CLUSTERED 
                	(
                		"ProductID"
                	),
                	CONSTRAINT "FK_Products_Categories" FOREIGN KEY 
                	(
                		"CategoryID"
                	) REFERENCES "dbo"."Categories" (
                		"CategoryID"
                	),
                	CONSTRAINT "FK_Products_Suppliers" FOREIGN KEY 
                	(
                		"SupplierID"
                	) REFERENCES "dbo"."Suppliers" (
                		"SupplierID"
                	),
                	CONSTRAINT "CK_Products_UnitPrice" CHECK (UnitPrice >= 0),
                	CONSTRAINT "CK_ReorderLevel" CHECK (ReorderLevel >= 0),
                	CONSTRAINT "CK_UnitsInStock" CHECK (UnitsInStock >= 0),
                	CONSTRAINT "CK_UnitsOnOrder" CHECK (UnitsOnOrder >= 0)
                )
                 CREATE  INDEX "CategoriesProducts" ON "dbo"."Products"("CategoryID")
                 CREATE  INDEX "CategoryID" ON "dbo"."Products"("CategoryID")
                 CREATE  INDEX "ProductName" ON "dbo"."Products"("ProductName")
                 CREATE  INDEX "SupplierID" ON "dbo"."Products"("SupplierID")
                 CREATE  INDEX "SuppliersProducts" ON "dbo"."Products"("SupplierID")

                CREATE TABLE "Order Details" (
                	"OrderID" "int" NOT NULL ,
                	"ProductID" "int" NOT NULL ,
                	"UnitPrice" "money" NOT NULL CONSTRAINT "DF_Order_Details_UnitPrice" DEFAULT (0),
                	"Quantity" "smallint" NOT NULL CONSTRAINT "DF_Order_Details_Quantity" DEFAULT (1),
                	"Discount" "real" NOT NULL CONSTRAINT "DF_Order_Details_Discount" DEFAULT (0),
                	CONSTRAINT "PK_Order_Details" PRIMARY KEY  CLUSTERED 
                	(
                		"OrderID",
                		"ProductID"
                	),
                	CONSTRAINT "FK_Order_Details_Orders" FOREIGN KEY 
                	(
                		"OrderID"
                	) REFERENCES "dbo"."Orders" (
                		"OrderID"
                	),
                	CONSTRAINT "FK_Order_Details_Products" FOREIGN KEY 
                	(
                		"ProductID"
                	) REFERENCES "dbo"."Products" (
                		"ProductID"
                	),
                	CONSTRAINT "CK_Discount" CHECK (Discount >= 0 and (Discount <= 1)),
                	CONSTRAINT "CK_Quantity" CHECK (Quantity > 0),
                	CONSTRAINT "CK_UnitPrice" CHECK (UnitPrice >= 0)
                )
                 CREATE  INDEX "OrderID" ON "dbo"."Order Details"("OrderID")
                 CREATE  INDEX "OrdersOrder_Details" ON "dbo"."Order Details"("OrderID")
                 CREATE  INDEX "ProductID" ON "dbo"."Order Details"("ProductID")
                 CREATE  INDEX "ProductsOrder_Details" ON "dbo"."Order Details"("ProductID")
                """;
    }
}