-- Elenca tutti i clienti e i relativi ordini, anche se non hanno effettuato ordini (LEFT JOIN).
use Northwind 
select 
  c.CompanyName, 
  o.OrderID 
from 
  Customers c 
  left join Orders o on c.CustomerID = o.CustomerID 
group by 
  c.CompanyName, 
  o.OrderID

-- Elenca tutti i prodotti che non sono stati mai ordinati (utilizza RIGHT JOIN o subquery).
use Northwind 
select 
  ProductName 
from 
  Products 
where 
  ProductID not in (
    select 
      ProductID 
    from 
      [Order Details]
  )

use Northwind 
select 
  p.ProductName 
from 
  [Order Details] od 
  right join Products p on od.ProductID = p.ProductID 
where 
  od.OrderID is null

-- Elenca i prodotti con un prezzo superiore alla media di tutti i prodotti.
use Northwind 
select 
  ProductName, 
  UnitPrice 
from 
  Products 
where 
  UnitPrice > (
    select 
      avg(UnitPrice) 
    from 
      Products
  )

-- Per ogni ordine, mostra l'importo totale calcolato (UnitPrice * Quantity) come "TotaleOrdine".
use Northwind 
select 
  od.OrderID, 
  p.ProductName, 
  (od.UnitPrice * od.Quantity) as TotaleOrdine 
from 
  [Order Details] od 
  join Products p on p.ProductID = od.ProductID 
group by 
  od.OrderID, 
  p.ProductName, 
  od.UnitPrice, 
  od.Quantity

-- Usa CASE WHEN per indicare "Economico", "Medio" o "Caro" in base al prezzo del prodotto.
use Northwind
select ProductID, ProductName, UnitPrice,
	case
		when UnitPrice > 100 then 'Caro'
		when UnitPrice > 50 Then 'Medio'
		else 'Economico'
	end as PrezzoIndicativo
from Products

-- Inserisci un nuovo cliente fittizio nella tabella Customers.
use Northwind
insert into Customers (CustomerID, CompanyName, ContactName, ContactTitle, Address, City, Region, PostalCode, Country, Phone, Fax, IsActive)
values ('TEST1', 'ClientTest', 'ContactTest', 'ContactTitleTest', 'Via Biella 10', 'Biella', 'Piedmont', '13862', 'Italy', '2141243242', '1272-0987', '1')

-- Aggiorna il nome dell'azienda del cliente appena inserito.
use Northwind 
update 
  Customers 
set 
  CompanyName = 'CompanyTest' 
where 
  CustomerID = 'TEST1'

-- Elimina il cliente fittizio appena inserito.
use Northwind 
delete from 
  Customers 
where 
  CustomerID = 'TEST1'

-- Mostra tutti i prodotti che non hanno un nome specificato (NULL o vuoto).
use Northwind 
select * 
from 
  Products 
where 
  ProductName is null

-- Mostra tutti i prodotti e, se non hanno un nome, visualizza 'Sconosciuto' al suo posto.
use Northwind 
select 
  ProductID, 
  coalesce(ProductName, 'Sconosciuto') as NomeProdotto, 
  UnitPrice 
from 
  Products

-- Crea un indice sul campo CompanyName della tabella Customers per velocizzare le ricerche.
use Northwind
create index IX_Customers_CompanyName
on Customers (CompanyName)