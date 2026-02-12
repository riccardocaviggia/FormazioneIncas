use BankDB 
select 
  c.ClientName, 
  c.ClientSurname, 
  a.IBAN, 
  a.Balance 
from 
  Clients c 
  join Accounts a on a.ClientID = c.ClientID
