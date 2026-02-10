create database EcommerceDB
go
use EcommerceDB
go

create table Clients (
	ClientID int identity not null,
	ClientName nvarchar(20) not null,
	ClientSurname nvarchar(20) not null,
	ClientEmail nvarchar(50) unique not null,

	constraint PK_Clients_ClientID primary key(ClientID)
)

create table Products (
	ProductID int identity not null,
	ProductName nvarchar(30) not null,
	ProductPrice decimal(10,2) not null,
	ProductDescription nvarchar(100) default null,

	constraint PK_Products_ProductID primary key(ProductID)
)

create table Storage (
	ProductID int not null,
	ProductQuantity int default 0,

	constraint PK_Storage_ProductID primary key(ProductID),
	constraint FK_Storage_ProductID foreign key(ProductID) references Products(ProductID)
)

create table Orders (
	OrderID int identity not null,
	ClientID int not null,
	OrderDate datetime default current_timestamp,
	OrderStatus varchar(20) default 'in elaborazione',
	OrderPayment varchar(20),

	constraint PK_Orders_OrderID primary key(OrderID),
	constraint FK_Orders_ClientID foreign key(ClientID) references Clients(ClientID)
)

create table Order_Details (
	OrderID int not null,
	ProductID int not null,
	ProductQuantity int not null,
	PriceAtPurchase decimal(10,2) not null,

	constraint PK_Order_Details primary key(OrderID, ProductID),
	constraint FK_Order_Details_OrderID foreign key(OrderID) references Orders(OrderID),
	constraint FK_Order_Details_ProductID foreign key(ProductID) references Products(ProductID)
)