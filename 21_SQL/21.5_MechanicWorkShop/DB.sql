create database MechanicalWorkShop
go
use MechanicalWorkShop
go

create table Clients(
	ClientID int identity(1,1) not null,
	ClientName nvarchar(20) not null,
	ClientSurname nvarchar(20) not null,
	ClientTelephone nvarchar(20) not null,

	constraint PK_Clients_ClientID primary key(ClientID)
)

create table Vehicles(
	VehicleID int identity(1,1) not null,
	ClientID int not null,
	VehiclePlate nvarchar(20) unique not null,
	VehicleBrand nvarchar(20) not null,

	constraint PK_Vehicles_VehicleID primary key(VehicleID),
	constraint FK_Vehicles_ClientID foreign key(ClientID) references Clients(ClientID)
)

create table Mechanics(
	MechanicID int identity(1,1) not null,
	MechanicName nvarchar(20) not null,
	MechanicSurname nvarchar(20) not null,

	constraint PK_Mechanics_MechanicID primary key(MechanicID)
)

create table Works(
	WorkID int identity(1,1) not null,
	VehicleID int not null,
	MechanicID int not null,
	WorkDate datetime,
	WorkCost decimal(10,2) not null,
	Description nvarchar(100) not null,

	constraint PK_Works_WorkID primary key(WorkID),
	constraint FK_Works_VehicleID foreign key(VehicleID) references Vehicles(VehicleID),
	constraint FK_Works_MechanicID foreign key(MechanicID) references Mechanics(MechanicID)
)

create table Replacements(
	ReplacementID int identity(1,1) not null,
	ReplacementCode nvarchar(20) not null,
	ReplacementName nvarchar(50) not null,
	ReplacementPrice decimal(10,2) not null

	constraint PK_Replacements_ReplacementID primary key(ReplacementID)
)

create table WorkDetails(
	WorkID int not null,
	ReplacementID int not null,
	Quantity int not null,

	constraint PK_WorkDetails_WorkIDReplacementID primary key(WorkID, ReplacementID),
	constraint FK_WorkDetails_WorkID foreign key(WorkID) references Works(WorkID),
	constraint FK_WorkDetails_ReplacementID foreign key(ReplacementID) references Replacements(ReplacementID)
)