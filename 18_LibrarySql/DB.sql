create database LibraryDB
go
use LibraryDB
go

create table Authors (
	AuthorID int identity not null,
	AuthorName nvarchar(20) not null,
	AuthorSurname nvarchar(20) not null,

	constraint PK_Authors_AuthorID primary key (AuthorID)
)

create table Users (
	UserID int identity not null,
	UserName nvarchar(20) not null,
	UserSurname nvarchar(20) not null,
	UserEmail nvarchar(50) not null,

	constraint PK_Users_UserID primary key (UserID)
)

create table Books (
	BookID int identity not null,
	BookTitle nvarchar(100) not null,
	BookISBN nvarchar(20) unique not null,

	constraint PK_Books_BookID primary key (BookID)
)

create table BookAuthors (
	BookID int not null,
	AuthorID int not null,

	constraint PK_BookAuthors primary key (BookID, AuthorID),
	constraint FK_BookAuthors_BookID foreign key (BookID) references Books(BookID),
	constraint FK_BookAuthors_AuthorID foreign key (AuthorID) references Authors(AuthorID)
)

create table BookLoans (
	LoanID int identity not null,
	BookID int not null,
	UserID int not null,
	LoanDate date default getDate(),
	ReturnDate date null,

	constraint PK_BookLoans_LoanID primary key (LoanID),
	constraint FK_BookLoans_BookID foreign key (BookID) references Books(BookID),
	constraint FK_BookLoans_UserID foreign key (UserID) references Users(UserID),
	constraint CHK_BookLoans_ReturnDate CHECK (ReturnDate > LoanDate or ReturnDate is null)
)