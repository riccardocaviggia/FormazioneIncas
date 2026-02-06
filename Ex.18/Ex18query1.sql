select b.BookTitle 
from Books b
join BookLoans bl on b.BookID = bl.BookID
where bl.ReturnDate is null