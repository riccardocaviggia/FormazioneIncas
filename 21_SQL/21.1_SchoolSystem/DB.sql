create database SchoolSystemDB
go 
use SchoolSystemDB
go

create table Teachers (
	TeacherID int identity(1,1) not null,
	TeacherName nvarchar(20) not null,
	TeacherSurname nvarchar(20) not null,
	TeacherEmail nvarchar(50) unique not null,

	constraint PK_Teachers_TeacherID primary key(TeacherID)
)

create table Students (
	StudentID int identity(1,1) not null,
	StudentName nvarchar(20) not null,
	StudentSurname nvarchar(20) not null,
	StudentEmail nvarchar(50) unique not null,

	constraint PK_Students_StudentID primary key(StudentID)
)

create table Courses (
	CourseID int identity(1,1) not null,
	CourseName nvarchar(30) not null,
	TeacherID int not null,

	constraint PK_Courses_CourseID primary key(CourseID),
	constraint FK_Courses_TeacherID foreign key(TeacherID) references Teachers(TeacherID)
)

create table Exams (
	ExamID int identity(1,1) not null,
	CourseID int not null,
	StudentID int not null,
	ExamGrade int not null,
	ExamDate datetime default current_timestamp,

	constraint PK_Exams_ExamID primary key(ExamID),
	constraint FK_Exams_CourseID foreign key(CourseID) references Courses(CourseID),
	constraint FK_Exams_StudentID foreign key(StudentID) references Students(StudentID)
)