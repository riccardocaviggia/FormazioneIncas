use BankDB 
select 
  c.ClientName, 
  c.ClientSurname, 
  t.TransactionDate, 
  t.TransactionAmount, 
  t.TransactionType 
from 
  Clients c 
  join Accounts a on a.ClientID = c.ClientID 
  join Transactions t on a.AccountID = T.AccountID 
where 
  t.TransactionAmount > 1000
