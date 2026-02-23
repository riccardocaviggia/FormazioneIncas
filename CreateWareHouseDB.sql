create database WareHouseDB
go

use WareHouseDB
go

create table Contexts (
	ContextId int identity(1,1) not null constraint PK_Contexts_ContextId primary key,
	ContextCode nvarchar(50) not null,
	ContextName nvarchar(100) not null,
	IsEnabled bit not null constraint DF_Contexts_IsEnabled default 1,
	CreatedAt datetime2(3) not null constraint DF_Contexts_CreatedAt default SYSUTCDATETIME(),
	UpdatedAt datetime2(3) default null
)

create unique index UX_Contexts_ContextCode on Contexts(ContextCode);

create table ServiceLogs (
	LogId bigint identity(1,1) not null constraint PK_ServiceLogs_LogId primary key,
	TimeStampUtc datetime2(3) not null constraint DF_ServiceLogs_TimeStampUtc default SYSUTCDATETIME(),
	ServiceName nvarchar(50) not null,
	LevelType nvarchar(50) not null,
	MessageType nvarchar(1000) not null,
	CorrelationId uniqueidentifier null,
	Barcode nvarchar(50) null,
	ContextCode nvarchar(50) null,
	Exception nvarchar(max) null
)

create index IX_ServiceLogs_TimeStampUtc on ServiceLogs(TimeStampUtc desc);
create index IX_ServiceLogs_ServiceName_TimeStampUtc on ServiceLogs(ServiceName, TimeStampUtc);
create index IX_ServiceLogs_CorrelationId on ServiceLogs(CorrelationId);