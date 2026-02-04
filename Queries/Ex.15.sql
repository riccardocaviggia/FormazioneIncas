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


-- Crea una stored procedure che, dato un ID cliente, restituisca i suoi ordini.


-- Crea una funzione che calcoli uno sconto del 10% su un importo superiore a 100.


-- Crea un trigger che registri in una tabella di log ogni nuovo ordine inserito.
