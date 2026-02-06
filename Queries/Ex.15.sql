-- Elenca tutti i clienti presenti nel database.
select CompanyName, ContactName from Customers

-- Elenca tutte le città diverse in cui si trovano i clienti.
select distinct City from Customers

-- Conta il numero di ordini effettuati da ciascun cliente.
select CompanyName, count(OrderID) as number_of_orders from Customers, Orders where Orders.CustomerID = Customers.CustomerID group by CompanyName
select CompanyName, count(OrderID) as number_of_orders from Customers inner join Orders on Orders.CustomerID = Customers.CustomerID group by CompanyName
select CompanyName, count(OrderID) as number_of_orders from Customers join Orders on Orders.CustomerID = Customers.CustomerID group by CompanyName

-- Elenca i clienti che hanno effettuato più di 5 ordini.
select CompanyName, count(OrderID) as number_of_orders from Customers, Orders where Orders.CustomerID = Customers.CustomerID group by CompanyName having count(OrderID) > 5
select CompanyName, count(OrderID) as number_of_orders from Customers inner join Orders on Orders.CustomerID = Customers.CustomerID group by CompanyName having count(OrderID) > 5

-- Crea una nuova tabella per raccogliere i feedback dei clienti, includendo ID, ID cliente, testo e data.
create table Feedback (
	FeedbackID int primary key identity(1,1),
	CustomerID nchar(5) not null,
	FeedbackText text not null,
	FeedbackDate datetime default current_timestamp,
	foreign key (CustomerID) references Customers(CustomerID)
	)

-- Aggiungi alla tabella Customers una colonna per indicare se il cliente è attivo.
alter table Customers add IsActive bit not null default 1

-- Imposta un vincolo UNIQUE sul nome azienda (CompanyName) dei clienti.
alter table Customers add constraint UQ_CompanyName UNIQUE (CompanyName)

-- Crea una vista che unisce gli ordini con i relativi nomi dei clienti.
go
create view V_GetRelationClientOrders as 
select o.OrderID, c.CompanyName, o.OrderDate, o.ShipCity from Orders o join Customers c on o.CustomerID = c.CustomerID
go
select * from V_GetRelationClientOrders
go

-- Crea una stored procedure che, dato un ID cliente, restituisca i suoi ordini.
go
create procedure GetClientOrders @CustomerID nchar(5) 
as begin
select * from Orders where CustomerID = @CustomerID
end;
go

exec GetClientOrders @CustomerID = 'BOLID'


-- Crea una funzione che calcoli uno sconto del 10% su un importo superiore a 100.
go
create function dbo.SaleCalculator(@Total decimal(5,2)) returns decimal(5,2)
as begin
	declare @Sale decimal(5,2)
	set @Sale = case when @Total > 100 then @Total * 0.1 else 0 end
	return @Sale
end;
go

exec dbo.SaleCalculator @Total = 150
select dbo.SaleCalculator(150) as discount

-- Crea un trigger che registri in una tabella di log ogni nuovo ordine inserito.
go
create table OrdersLog (
	LogID int primary key identity,
	OrderID int,
	OrderDate datetime default getDate(),
	foreign key(OrderID) references Orders(OrderID)
	)
go

create trigger trg_CreateLogOrder
on Orders
after insert
as
begin
	insert into OrdersLog(OrderID)
	select OrderID from inserted
end;

set nocount 