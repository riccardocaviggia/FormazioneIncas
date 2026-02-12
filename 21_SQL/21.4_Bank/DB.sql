create database BankDB
go
use BankDB
go

create table Clients (
	ClientID int identity(1,1) not null,
	ClientName nvarchar(20) not null,
	ClientSurname nvarchar(20) not null,
	ClientTaxCode nvarchar(20) unique not null,

	constraint PK_Clients_ClientID primary key(ClientID)
)

create table Branches (
	BranchID int identity(1,1) not null,
	BranchName nvarchar(20) not null,
	BranchAddress nvarchar(20) not null,

	constraint PK_Branches_BranchID primary key(BranchID)
)

create table Accounts (
	AccountID int identity(1,1) not null,
	ClientID int not null,
	BranchID int not null,
	Balance decimal(10,2) not null,
	IBAN varchar(30) unique not null,

	constraint PK_Accounts_AccountID primary key(AccountID),
	constraint FK_Accounts_ClientID foreign key(ClientID) references Clients(ClientID),
	constraint FK_Accounts_BranchID foreign key(BranchID) references Branches(BranchID)
)

create table Transactions (
	TransactionID int identity(1,1) not null,
	AccountID int not null,
	TransactionDate datetime default current_timestamp,
	TransactionAmount decimal(10,2) not null,
	TransactionType varchar(20) not null,

	constraint PK_Transactions_TransactionID primary key(TransactionID),
	constraint FK_Transactions_AccountID foreign key(AccountID) references Accounts(AccountID)
)