use BankDB
select c.ClientName, c.ClientSurname, sum(a.Balance) as total_balance
from Clients c
join Accounts a on a.ClientID = c.ClientID
group by c.ClientName, c.ClientSurname