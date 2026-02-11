create database HospitalDB
go
use HospitalDB
go


create table Departments (
	DepartmentID int identity(1,1) not null,
	DepartmentName nvarchar(30) not null,

	constraint PK_Departments_DepartmentID primary key(DepartmentID)
)

create table Doctors (
	DoctorID int identity(1,1) not null,
	DoctorName nvarchar(20) not null,
	DoctorSurname nvarchar(20) not null,
	DepartmentID int not null, 
	DoctorSpecialization nvarchar(20) not null,

	constraint PK_Doctors_DoctorID primary key(DoctorID),
	constraint FK_Doctors_DepartmentID foreign key(DepartmentID) references Departments(DepartmentID)
)


create table Patients (
	PatientID int identity(1,1) not null,
	PatientName nvarchar(20) not null,
	PatientSurname nvarchar(20) not null,

	constraint PK_Patients_PatientID primary key(PatientID)
)

create table Examinations (
	ExaminationID int identity(1,1) not null,
	DoctorID int not null,
	PatientID int not null,
	ExaminationDate datetime default current_timestamp,
	ExaminationDateFinish datetime default null,
	ExaminationReason nvarchar(100),

	constraint PK_Examinations_ExaminationID primary key(ExaminationID),
	constraint FK_Examinations_DoctorID foreign key(DoctorID) references Doctors(DoctorID),
	constraint FK_Examinations_PatientID foreign key(PatientID) references Patients(PatientID)
)



create table Therapies (
	TherapyID int identity(1,1) not null,
	PatientID int not null,
	TherapyDescription nvarchar(100) not null,
	TherapyStartDate datetime default current_timestamp,
	TherapyEndDate datetime not null,

	constraint PK_Therapies_TherapyID primary key(TherapyID),
	constraint FK_Therapies_PatientID foreign key(PatientID) references Patients(PatientID)
)