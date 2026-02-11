create database StreamingPlatform
go
use StreamingPlatform
go

create table Actors (
	ActorID int identity(1,1) not null,
	ActorName nvarchar(20) not null,
	ActorSurname nvarchar(20) not null,

	constraint PK_Actors_ActorID primary key(ActorID)
)

create table Contents (
	ContentID int identity(1,1) not null,
	ContentTitle nvarchar(50) not null,
	ReleaseYear int not null,
	ContentType nvarchar(20) not null check(ContentType in ('Movie', 'TV Series')),

	constraint PK_Contents_ContentID primary key(ContentID)
)

create table Episodes (
	EpisodeID int identity(1,1) not null,
	ContentID int not null,
	SeasonNumber int not null,
	EpisodeNumber int not null,
	EpisodeTitle nvarchar(50) not null,

	constraint PK_Episodes_EpisodeID primary key(EpisodeID),
	constraint FK_Episodes_ContentID foreign key(ContentID) references Contents(ContentID)
)

create table Casts (
	ActorID int not null,
	ContentID int not null,

	constraint PK_Casts_ActorContent primary key(ActorID, ContentID),
	constraint FK_Casts_ActorID foreign key(ActorID) references Actors(ActorID),
	constraint FK_Casts_ContentID foreign key(ContentID) references Contents(ContentID)
)

create table Users (
	UserID int identity(1,1) not null,
	UserName nvarchar(20) not null,
	UserSurname nvarchar(20) not null,
	UserEmail nvarchar(50) unique not null,

	constraint PK_Users_UserID primary key(UserID)
)

create table Reviews (
	ReviewID int identity(1,1) not null,
	UserID int not null,
	ContentID int not null,
	Rating int not null check (Rating between 1 and 10),
	Comment text not null,
	ReviewDate datetime default current_timestamp

	constraint PK_Reviews_ReviewID primary key(ReviewID),
	constraint FK_Reviews_ContentID foreign key(ContentID) references Contents(ContentID),
	constraint FK_Reviews_UserID foreign key(UserID) references Users(UserID)
)